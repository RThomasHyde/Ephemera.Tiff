using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class DoubleTiffField : TiffFieldBase<double>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }
        

        internal DoubleTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.Double;
            if (reader != null) ReadTag(reader);
        }

        private DoubleTiffField(DoubleTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<double>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            Values = reader.ReadNDoubles(offset, count).ToList();
        }

        void ITiffFieldInternal.WriteTag(Stream s)
        {
            var bytes = BitConverter.GetBytes(TagNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(TypeNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(Count);
            s.Write(bytes, 0, 4);

            bytes = BitConverter.GetBytes(((ITiffFieldInternal)this).Offset);
            s.Write(bytes, 0, 4);
        }

        void ITiffFieldInternal.WriteData(Stream s)
        {
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            foreach (var value in Values)
                s.Write(BitConverter.GetBytes(value), 0, 8);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new DoubleTiffField(this);
        }
    }
}