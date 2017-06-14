using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal class LongTiffField : TiffFieldBase<uint>,  ITiffFieldInternal
    {
        public virtual uint Offset { get; set; }

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
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<uint>(original.Values);
        }

        protected virtual void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 1)
                Values = reader.ReadNUInt32(offset, count).ToList();
            else
            {
                Values = new List<uint> {offset};
            }
        }

        public virtual void WriteTag(Stream s)
        {
            var bytes = BitConverter.GetBytes(TagNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(TypeNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(Count);
            s.Write(bytes, 0, 4);

            bytes = BitConverter.GetBytes(Count == 1 ? Values[0] : ((ITiffFieldInternal)this).Offset);
            s.Write(bytes, 0, 4);
        }

        public virtual void WriteData(Stream s)
        {
            if (Count == 1) return;
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            foreach (var value in Values)
                s.Write(BitConverter.GetBytes(value), 0, 4);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new LongTiffField(this);
        }
    }
}