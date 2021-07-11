using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class SLongTiffField : TiffFieldBase<int>, ITiffFieldInternal
    {
        public override bool IsComplex => Count > 1;

        internal SLongTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.SLong;
            if (reader != null) ReadTag(reader);
        }

        private SLongTiffField(SLongTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            Offset = original.Offset;
            Values = new List<int>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            if (count > 1)
                Values = reader.ReadNInt32(Offset, count).ToList();
            else
            {
                Values = new List<int>();
                var bytes = BitConverter.GetBytes(Offset);
                Values.Add(BitConverter.ToInt32(bytes, 0));
            }
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            if (Count == 1) writer.Write(Values[0]);
            else writer.Write(Offset);
        }

        public void WriteData(TiffWriter writer)
        {
            if (Count == 1) return;
            Offset = (uint)writer.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SLongTiffField(this);
        }
    }
}