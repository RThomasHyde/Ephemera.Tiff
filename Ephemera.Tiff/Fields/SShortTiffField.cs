using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class SShortTiffField : TiffFieldBase<short>, ITiffFieldInternal
    {
        internal SShortTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.SShort;
            if (reader != null) ReadTag(reader);
        }

        private SShortTiffField(SShortTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            Offset = original.Offset;
            Values = new List<short>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 2)
                Values = reader.ReadNInt16(offset, count).ToList();
            else
            {
                Values = new List<short>();
                var bytes = BitConverter.GetBytes(offset);
                int index = 0;
                for (int i = 0; i < 4; i += 2)
                {
                    if (index >= count) break;
                    Values.Add(BitConverter.ToInt16(bytes, i));
                    index++;
                }
            }
        }

        protected override void WriteOffset(BinaryWriter writer)
        {
            if (Count <= 2)
            {
                Values.ForEach(writer.Write);
                if (Values.Count == 1)
                    writer.Write((short)0);
            }
            else
            {
                writer.Write(Offset);
            }
        }

        public void WriteData(BinaryWriter writer)
        {
            if (Count <= 2) return;
            ((ITiffFieldInternal)this).Offset = (uint)writer.BaseStream.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SShortTiffField(this);
        }
    }
}