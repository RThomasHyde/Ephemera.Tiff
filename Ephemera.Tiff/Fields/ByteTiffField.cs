using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class ByteTiffField : TiffFieldBase<byte>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

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
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
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

        void ITiffFieldInternal.WriteTag(Stream s)
        {
            var bytes = BitConverter.GetBytes(TagNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(TypeNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(Values.Count);
            s.Write(bytes, 0, 4);

            if (Values.Count <= 4)
            {
                bytes = new byte[4];
                for (int i = 0; i < Count; ++i)
                    bytes[i] = Values[i];
            }
            else
            {
                bytes = BitConverter.GetBytes(((ITiffFieldInternal)this).Offset);
            }
            s.Write(bytes, 0, 4);
        }

        void ITiffFieldInternal.WriteData(Stream s)
        {
            if (Count <= 4) return;
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            s.Write(Values.ToArray(), 0, Count);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new ByteTiffField(this);
        }
    }
}