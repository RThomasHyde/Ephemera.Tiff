using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            Values = reader.ReadNDoubles(offset, count).ToList();
        }

        protected override void WriteOffset(BinaryWriter writer)
        {
            writer.Write(Offset);
        }

        void ITiffFieldInternal.WriteData(BinaryWriter writer)
        {
            ((ITiffFieldInternal)this).Offset = (uint)writer.BaseStream.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new DoubleTiffField(this);
        }
    }
}