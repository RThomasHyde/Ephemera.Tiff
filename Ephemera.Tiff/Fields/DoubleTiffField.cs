using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class DoubleTiffField : TiffFieldBase<double>, ITiffFieldInternal
    {
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
            Offset = original.Offset;
            Values = new List<double>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            Values = reader.ReadNDoubles(Offset, count).ToList();
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            writer.Write(Offset);
        }

        void ITiffFieldInternal.WriteData(TiffWriter writer)
        {
            Offset = (uint)writer.Position;
            writer.WriteN(Values);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new DoubleTiffField(this);
        }
    }
}