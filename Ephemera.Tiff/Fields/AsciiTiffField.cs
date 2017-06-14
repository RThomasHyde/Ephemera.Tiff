using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class AsciiTiffField : TiffFieldBase<string>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

        internal AsciiTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.ASCII;
            if (reader != null) ReadTag(reader);
        }

        private AsciiTiffField(AsciiTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            ((ITiffFieldInternal) this).Offset = ((ITiffFieldInternal) original).Offset;
            Values = new List<string>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var nBytes = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
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

        void ITiffFieldInternal.WriteTag(Stream s)
        {
            var writer = new BinaryWriter(s);

            writer.Write(TagNum);

            writer.Write(TypeNum);

            var stringBytes = new List<byte[]>();
            foreach (var @string in Values)
            {
                stringBytes.Add(Encoding.ASCII.GetBytes(@string));
            }

            uint count = (uint) stringBytes.Sum(x => x.Length + 1);
            writer.Write(count);

            // if the value fits into 4 bytes, write it directly to the 
            // offset field, otherwise, write the offset to where the 
            // values were stored.
            if (count <= 4)
            {
                foreach (var array in stringBytes)
                {
                    foreach (var @byte in array)
                        s.WriteByte(@byte);
                    s.WriteByte(0);
                }
            }
            else
            {
                writer.Write(((ITiffFieldInternal)this).Offset);
            }
        }

        void ITiffFieldInternal.WriteData(Stream s)
        {
            var stringBytes = new List<byte[]>();
            foreach (var @string in Values)
            {
                stringBytes.Add(Encoding.ASCII.GetBytes(@string));
            }

            uint count = (uint)stringBytes.Sum(x => x.Length + 1);

            // if the string value(s) fits inside 4 bytes, it will be
            // stored directly in the tag's offset field, so we shouldn't
            // write anything here.
            if (count <= 4) return;

            // update the tag's offset to point to this location in the stream
            ((ITiffFieldInternal) this).Offset = (uint)s.Position;

            foreach (var array in stringBytes)
            {
                foreach (var @byte in array)
                {
                    s.WriteByte(@byte);
                }
                s.WriteByte(0);
            }
        }

        public override bool DataExceeds4Bytes
        {
            get { return Values.Sum(x => x.Length + 1) > 4; }
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new AsciiTiffField(this);
        }
    }
}