using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class SByteTiffField : TiffFieldBase<sbyte>, ITiffFieldInternal
    {
        internal SByteTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.SByte;
            if (reader != null) ReadTag(reader);
        }

        private SByteTiffField(SByteTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            Offset = original.Offset;
            Values = new List<sbyte>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            var pos = reader.Position;
            Offset = reader.ReadUInt32();
            if (count > 4)
                Values = reader.ReadNSBytes(Offset, count).ToList();
            else
            {
                reader.Seek(pos, SeekOrigin.Begin);
                for (int i = 0; i < 4; ++i)
                {
                    sbyte sb = reader.ReadSByte();
                    if (i >= count) continue;
                    Values.Add(sb);
                }
            }
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            if (Count <= 4)
            {
                var sbytes = new sbyte[4];
                for (int i = 0; i < Count; ++i)
                    sbytes[i] = Values[i];
                // The CLR apparently supports this cast despite the warning
                var bytes = (byte[])(object)sbytes;
                writer.Write(bytes, 0, 4);
            }
            else
            {
                writer.Write(Offset);
            }
        }

        void ITiffFieldInternal.WriteData(TiffWriter writer)
        {
            if (Count <= 4) return;
            Offset = (uint)writer.Position;
            var bytes = (byte[]) (object) Values.ToArray();
            writer.Write(bytes);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SByteTiffField(this);
        }
    }
}