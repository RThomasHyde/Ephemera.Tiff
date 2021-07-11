using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class ShortTiffField : TiffFieldBase<ushort>, ITiffFieldInternal
    {
        public override bool IsComplex => Count > 2;

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
            Offset = original.Offset;
            Values = new List<ushort>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            var pos = reader.Position;
            Offset = reader.ReadUInt32();
            if (count > 2)
                Values = reader.ReadNUInt16(Offset, count).ToList();
            else
            {
                Values = new List<ushort>();
                reader.Seek(pos, SeekOrigin.Begin);
                int index = 0;
                for (int i = 0; i < 4; i +=2, index++)
                {
                    ushort val = reader.ReadUInt16();
                    if (index >= count) continue;
                    Values.Add(val);
                }
            }
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            if (Count <= 2)
            {
                Values.ForEach(writer.Write);
                if (Values.Count == 1)
                    writer.Write((ushort)0);
            }
            else
            {
                writer.Write(Offset);
            }
        }

        void ITiffFieldInternal.WriteData(TiffWriter writer)
        {
            if (Count <= 2) return;
            Offset = (uint)writer.BaseStream.Position;
            writer.WriteN(Values);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new ShortTiffField(this);
        }
    }
}