using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal class LongTiffField : TiffFieldBase<uint>,  ITiffFieldInternal
    {
        public override bool IsComplex => Count > 1;

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
            Offset = value;
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
            Offset = reader.ReadUInt32();

            if (count > 1)
            {
                if (Offset >= reader.BaseStream.Length)
                {
                    Values = (new uint[count]).ToList();
                }
                else
                {
                    Values = reader.ReadNUInt32(Offset, count).ToList();
                }
            }
            else
            {
                Values = new List<uint> { Offset };
            }
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            writer.Write(Count == 1 ? Values[0] : Offset);
        }

        public virtual void WriteData(TiffWriter writer)
        {
            if (Count == 1) return;
            Offset = (uint)writer.Position;
            writer.WriteN(Values);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new LongTiffField(this);
        }
    }
}