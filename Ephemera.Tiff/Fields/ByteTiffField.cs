using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class ByteTiffField : TiffFieldBase<byte>, ITiffFieldInternal
    {
        public override bool IsComplex => Count > 4;

        internal ByteTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.Byte;
            if (reader != null) ReadTag(reader);
        }

        private ByteTiffField(ByteTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            Offset = original.Offset;
            Values = new List<byte>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            if (count > 4)
                Values = reader.ReadNBytes(Offset, count).ToList();
            else
            {
                Values = new List<byte>();
                var bytes = BitConverter.GetBytes(Offset);
                for (int i = 0; i < count; ++i)
                    Values.Add(bytes[i]);
            }
        }

        protected override void WriteOffset(TiffWriter writer)
        {
            if (Count <= 4)
            {
                var array = new byte[4];
                for (int i = 0; i < Count; ++i)
                    array[i] = Values[i];
                writer.Write(array);
            }
            else
                writer.Write(Offset);
        }

        void ITiffFieldInternal.WriteData(TiffWriter writer)
        {
            if (Count <= 4) return;
            Offset = (uint)writer.Position;
            writer.Write(Values.ToArray());
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new ByteTiffField(this);
        }
    }
}