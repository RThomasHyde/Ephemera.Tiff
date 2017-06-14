using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class ShortTiffField : TiffFieldBase<ushort>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

        internal ShortTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.Short;
            if (reader != null) ReadTag(reader);
        }

        private ShortTiffField(ShortTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<ushort>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            var pos = reader.BaseStream.Position;
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 2)
                Values = reader.ReadNUInt16(offset, count).ToList();
            else
            {
                Values = new List<ushort>();
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                int index = 0;
                for (int i = 0; i < 4; i +=2, index++)
                {
                    ushort val = reader.ReadUInt16();
                    if (index >= count) continue;
                    Values.Add(val);
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

            if (Count <= 2)
            {
                bytes = new byte[4];
                int index = 0;
                foreach (ushort value in Values)
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

        void ITiffFieldInternal.WriteData(Stream s)
        {
            if (Count <= 2) return;
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            foreach (var value in Values)
                s.Write(BitConverter.GetBytes(value), 0, 2);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new ShortTiffField(this);
        }
    }
}