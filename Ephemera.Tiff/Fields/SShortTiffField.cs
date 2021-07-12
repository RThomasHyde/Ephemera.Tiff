using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Offset = reader.ReadUInt32();
            if (count > 2)
                Values = reader.ReadNInt16(Offset, count).ToList();
            else
            {
                Values = new List<short>();
                var bytes = BitConverter.GetBytes(Offset);
                int index = 0;
                for (int i = 0; i < 4; i += 2)
                {
                    if (index >= count) break;
                    Values.Add(BitConverter.ToInt16(bytes, i));
                    index++;
                }
            }
        }

        protected override void WriteOffset(TiffWriter writer)
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

        public void WriteData(TiffWriter writer)
        {
            if (Count <= 2) return;
            Offset = (uint)writer.Position;
            writer.WriteN(Values);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SShortTiffField(this);
        }
    }
}