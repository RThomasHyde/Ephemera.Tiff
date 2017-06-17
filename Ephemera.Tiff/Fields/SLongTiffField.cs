using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class SLongTiffField : TiffFieldBase<int>, ITiffFieldInternal
    {
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
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 1)
                Values = reader.ReadNInt32(offset, count).ToList();
            else
            {
                Values = new List<int>();
                var bytes = BitConverter.GetBytes(offset);
                Values.Add(BitConverter.ToInt32(bytes, 0));
            }
        }

        protected override void WriteOffset(BinaryWriter writer)
        {
            if (Count == 1) writer.Write(Values[0]);
            else writer.Write(Offset);
        }

        public void WriteData(BinaryWriter writer)
        {
            if (Count == 1) return;
            Offset = (uint)writer.BaseStream.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new SLongTiffField(this);
        }
    }
}