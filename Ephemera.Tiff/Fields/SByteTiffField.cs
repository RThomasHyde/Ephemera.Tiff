using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class SByteTiffField : TiffFieldBase<sbyte>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

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
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<sbyte>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            var pos = reader.BaseStream.Position;
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 4)
                Values = reader.ReadNSBytes(offset, count).ToList();
            else
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                for (int i = 0; i < 4; ++i)
                {
                    sbyte sb = reader.ReadSByte();
                    if (i >= count) continue;
                    Values.Add(sb);
                }
            }
        }

        void ITiffFieldInternal.WriteTag(Stream s)
        {
            var bytes = BitConverter.GetBytes(TagNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(TypeNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(Count);
            s.Write(bytes, 0, 4);

            sbyte[] sbytes = new sbyte[4];
            if (Count <= 4)
            {
                sbytes = new sbyte[4];
                for (int i = 0; i < Count; ++i)
                    sbytes[i] = Values[i];
            }
            else
            {
                for (int i = 0; i < Count; ++i)
                    sbytes[i] = Values[i];
            }

            // The CLR apparently supports this cast despite the warning
            bytes = (byte[])(object)sbytes; 
            s.Write(bytes, 0, 4);
        }

        void ITiffFieldInternal.WriteData(Stream s)
        {
            if (Count <= 4) return;
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            var bytes = (byte[]) (object) Values.ToArray();
            s.Write(bytes, 0, Count);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SByteTiffField(this);
        }
    }
}