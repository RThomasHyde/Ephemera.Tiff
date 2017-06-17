using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class ByteTiffField : TiffFieldBase<byte>, ITiffFieldInternal
    {
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
            ((ITiffFieldInternal)this).Offset = original.Offset;
            Values = new List<byte>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            var count = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;
            if (count > 4)
                Values = reader.ReadNBytes(offset, count).ToList();
            else
            {
                Values = new List<byte>();
                var bytes = BitConverter.GetBytes(offset);
                for (int i = 0; i < count; ++i)
                    Values.Add(bytes[i]);
            }
        }

        protected override void WriteOffset(BinaryWriter writer)
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

        void ITiffFieldInternal.WriteData(BinaryWriter writer)
        {
            if (Count <= 4) return;
            ((ITiffFieldInternal)this).Offset = (uint)writer.BaseStream.Position;
            writer.Write(Values.ToArray());
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new ByteTiffField(this);
        }
    }
}