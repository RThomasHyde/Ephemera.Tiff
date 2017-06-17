using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal class LongTiffField : TiffFieldBase<uint>,  ITiffFieldInternal
    {
        internal LongTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.Long;
            if (reader != null) ReadTag(reader);
        }

        internal LongTiffField(ushort tag, uint value)
        {
            TagNum = tag;
            TypeNum = (ushort)TiffFieldType.Long;
            ((ITiffFieldInternal) this).Offset = value;
            Values = new List<uint> {value};
        }

        private LongTiffField(LongTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            Offset = original.Offset;
            Values = new List<uint>(original.Values);
        }

        protected virtual void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            var offset = reader.ReadUInt32();
            Offset = offset;
            Values = count > 1 ? reader.ReadNUInt32(offset, count).ToList() : new List<uint> {offset};
        }

        protected override void WriteOffset(BinaryWriter writer)
        {
            writer.Write(Count == 1 ? Values[0] : Offset);
        }

        public virtual void WriteData(BinaryWriter writer)
        {
            if (Count == 1) return;
            Offset = (uint)writer.BaseStream.Position;
            Values.ForEach(writer.Write);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new LongTiffField(this);
        }
    }
}