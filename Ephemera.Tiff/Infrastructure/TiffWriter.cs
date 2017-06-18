using System;
using System.Collections.Generic;
using System.IO;

namespace Ephemera.Tiff.Infrastructure
{
    internal sealed class TiffWriter : BinaryWriter
    {
        private readonly ByteOrder byteOrder;
        private readonly bool sameEndian;

        public TiffWriter(Stream stream, ByteOrder? byteOrder = null) : base(stream)
        {
            if (byteOrder == null)
                byteOrder = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            this.byteOrder = byteOrder.Value;
            sameEndian = byteOrder == ByteOrder.LittleEndian && BitConverter.IsLittleEndian;
        }

        public long Position => BaseStream.Position;

        public void WriteHeader()
        {
            var bom = byteOrder == ByteOrder.LittleEndian ? TiffConstants.BOM_LSB2_MSB : TiffConstants.BOM_MSB2_LSB;
            Write(bom);
            Write(TiffConstants.MAGIC);
        }

        public void WritePosition()
        {
            Write((uint) Position);
        }

        public void AlignToWordBoundary()
        {
            var wordAlignBytes = Position % 4;
            if (wordAlignBytes > 0)
            {
                var padBytes = new byte[4 - wordAlignBytes];
                Write(padBytes, 0, padBytes.Length);
            }
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void Write(ushort value)
        {
            if (!sameEndian)
                value = EndianTools.ReverseBytes(value);
            base.Write(value);
        }

        public void WriteN(IEnumerable<ushort> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public override void Write(short value)
        {
            if (!sameEndian)
                value = EndianTools.ReverseBytes(value);
            base.Write(value);
        }

        public void WriteN(IEnumerable<short> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public override void Write(uint value)
        {
            if (!sameEndian)
                value = EndianTools.ReverseBytes(value);
            base.Write(value);
        }

        public void WriteN(IEnumerable<uint> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public override void Write(int value)
        {
            if (!sameEndian)
                value = EndianTools.ReverseBytes(value);
            base.Write(value);
        }

        public void WriteN(IEnumerable<int> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public override void Write(float value)
        {
            if (!sameEndian)
                value = EndianTools.ReverseBytes(value);
            base.Write(value);
        }

        public void WriteN(IEnumerable<float> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public override void Write(double value)
        {
            if (!sameEndian)
                value = EndianTools.ReverseBytes(value);
            base.Write(value);
        }

        public void WriteN(IEnumerable<double> values)
        {
            foreach (var value in values)
                Write(value);
        }
    }
}