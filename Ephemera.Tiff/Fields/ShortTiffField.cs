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

        protected override void WriteOffset(BinaryWriter writer)
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

        void ITiffFieldInternal.WriteData(BinaryWriter writer)
        {
            if (Count <= 2) return;
            ((ITiffFieldInternal)this).Offset = (uint)writer.BaseStream.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new ShortTiffField(this);
        }
    }
}