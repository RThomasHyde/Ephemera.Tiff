using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class AsciiTiffField : TiffFieldBase<string>, ITiffFieldInternal
    {
        public override int Count
        {
            get { return Values.Sum(x => Encoding.ASCII.GetByteCount(x) + 1); }
        }

        internal AsciiTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort)TiffFieldType.ASCII;
            if (reader != null) ReadTag(reader);
        }

        private AsciiTiffField(AsciiTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            Offset = original.Offset;
            Values = new List<string>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var nBytes = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            ((ITiffFieldInternal)this).Offset = offset;
            var bytes = nBytes > 4 ? reader.ReadNBytes(offset, nBytes) : BitConverter.GetBytes(offset);
            Values = ReadStrings(bytes);
        }

        private List<string> ReadStrings(byte[] bytes)
        {
            var strings = new List<string>();
            var accumulator = new List<byte>();
            foreach (var @byte in bytes)
            {
                if (@byte == 0)
                {
                    strings.Add(Encoding.ASCII.GetString(accumulator.ToArray()));
                    accumulator.Clear();
                }
                else accumulator.Add(@byte);
            }
            return strings;
        }

        void ITiffFieldInternal.WriteEntry(BinaryWriter writer)
        {
            writer.Write(TagNum);
            writer.Write(TypeNum);

            var stringBytes = new List<byte[]>();
            foreach (var @string in Values)
            {
                stringBytes.Add(Encoding.ASCII.GetBytes(@string));
            }

            uint count = (uint)stringBytes.Sum(x => x.Length + 1);
            writer.Write(count);

            // if the value fits into 4 bytes, write it directly to the 
            // offset field, otherwise, write the offset to where the 
            // values were stored.
            if (count <= 4)
            {
                stringBytes.ForEach(a =>
                                    {
                                        writer.Write(a);
                                        writer.Write((byte)0);
                                    });
            }
            else
            {
                writer.Write(Offset);
            }
        }

        protected override void WriteOffset(BinaryWriter writer)
        {
            // no need to do anything in here since we're overriding ITiffFieldInternal.WriteEntry
        }

        void ITiffFieldInternal.WriteData(BinaryWriter writer)
        {
            var stringBytes = new List<byte[]>();
            foreach (var @string in Values)
            {
                stringBytes.Add(Encoding.ASCII.GetBytes(@string));
            }

            var count = stringBytes.Sum(x => x.Length + 1);

            // if all values fit inside 4 bytes, they will be stored directly in the tag's offset field 
            // when WriteEntry is called, so we don't need to write anything here.
            if (count <= 4) return;

            // update the tag's offset to point to this location in the stream
            ((ITiffFieldInternal)this).Offset = (uint)writer.BaseStream.Position;

            stringBytes.ForEach(a =>
                                {
                                    writer.Write(a);
                                    writer.Write((byte) 0);
                                });
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new AsciiTiffField(this);
        }
    }
}