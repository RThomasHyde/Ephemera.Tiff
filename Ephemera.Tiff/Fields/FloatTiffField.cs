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
            var pos = reader.Position;
            Offset = reader.ReadUInt32();
            if (count > 1)
                Values = reader.ReadNSingles(Offset, count).ToList();
            else
            {
                reader.Seek(pos, SeekOrigin.Begin);
                Values = new List<float> {reader.ReadSingle()};
            }
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            writer.Write(Count == 1 ? Values[0] : Offset);
        }

        public void WriteData(TiffWriter writer)
        {
            if (Count == 1) return;
            Offset = (uint)writer.Position;
            writer.WriteN(Values);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new FloatTiffField(this);
        }
    }
}