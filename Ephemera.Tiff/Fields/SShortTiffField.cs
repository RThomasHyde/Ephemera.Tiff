using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class SShortTiffField : TiffFieldBase<short>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

        internal SShortTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.SShort;
            if (reader != null) ReadTag(reader);
        }

        private SShortTiffField(SShortTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<short>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 2)
                Values = reader.ReadNInt16(offset, count).ToList();
            else
            {
                Values = new List<short>();
                var bytes = BitConverter.GetBytes(offset);
                int index = 0;
                for (int i = 0; i < 4; i += 2)
                {
                    if (index >= count) break;
                    Values.Add(BitConverter.ToInt16(bytes, i));
                    index++;
                }
            }
        }

        public void WriteTag(Stream s)
        {
            var bytes = BitConverter.GetBytes(TagNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(TypeNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(Count);
            s.Write(bytes, 0, 4);

            if (Count <= 2)
            {
                bytes = new byte[4];
                int index = 0;
                foreach (short value in Values)
                {
                    var shortBytes = BitConverter.GetBytes(value);
                    bytes[index] = shortBytes[0];
                    bytes[index + 1] = shortBytes[1];
                    index += 2;
                }
            }
            else
            {
                bytes = BitConverter.GetBytes(((ITiffFieldInternal)this).Offset);
            }
            s.Write(bytes, 0, 4);
        }

        public void WriteData(Stream s)
        {
            if (Count <= 2) return;
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            foreach (var value in Values)
                s.Write(BitConverter.GetBytes(value), 0, 2);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SShortTiffField(this);
        }
    }
}