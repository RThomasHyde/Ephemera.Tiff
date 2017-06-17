using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class FloatTiffField : TiffFieldBase<float>, ITiffFieldInternal
    {
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
            Offset = original.Offset;
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

        protected override void WriteOffset(BinaryWriter writer)
        {
            writer.Write(Count == 1 ? Values[0] : Offset);
        }

        public void WriteData(BinaryWriter writer)
        {
            if (Count == 1) return;
            ((ITiffFieldInternal)this).Offset = (uint)writer.BaseStream.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new FloatTiffField(this);
        }
    }
}