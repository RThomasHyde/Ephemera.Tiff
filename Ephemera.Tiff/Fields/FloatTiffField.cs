using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class FloatTiffField : TiffFieldBase<float>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

        internal FloatTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.Float;
            if (reader != null) ReadTag(reader);
        }

        private FloatTiffField(FloatTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<float>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            var pos = reader.BaseStream.Position;
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 1)
                Values = reader.ReadNSingles(offset, count).ToList();
            else
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                Values = new List<float> {reader.ReadSingle()};
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

            bytes = BitConverter.GetBytes(Count == 1 ? Values[0] : ((ITiffFieldInternal)this).Offset);
            s.Write(bytes, 0, 4);
        }

        public void WriteData(Stream s)
        {
            if (Count == 1) return;
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            foreach (var value in Values)
                s.Write(BitConverter.GetBytes(value), 0, 4);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new FloatTiffField(this);
        }
    }
}