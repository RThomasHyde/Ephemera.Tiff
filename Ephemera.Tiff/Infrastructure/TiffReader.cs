using System;
using System.IO;

namespace Ephemera.Tiff.Infrastructure
{
    internal sealed class TiffReader : BinaryReader
    {
        private bool sameEndian;

        public TiffReader(Stream input) : base(input)
        {
            ReadHeader();
        }

        public ByteOrder ByteOrder { get; private set; }

        private void ReadHeader()
        {
            var byteOrderMark = base.ReadUInt16();
            switch (byteOrderMark)
            {
                case TiffConstants.BOM_LSB2_MSB:
                    ByteOrder = ByteOrder.LittleEndian;
                    sameEndian = BitConverter.IsLittleEndian;
                    break;
                case TiffConstants.BOM_MSB2_LSB:
                    ByteOrder = ByteOrder.BigEndian;
                    sameEndian = !BitConverter.IsLittleEndian;
                    break;
                default:
                    throw new TiffException("Invalid byte order mark (BOM) for a TIFF image.");
            }

            if (ReadUInt16() != TiffConstants.MAGIC)
                throw new TiffException("TIFF magic not found in file header.");
        }

        public long Position => BaseStream.Position;

        public long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override float ReadSingle()
        {
            var val = base.ReadSingle();
            return sameEndian ? val : EndianTools.ReverseBytes(val);
        }

        public float[] ReadNSingles(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new float[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            float[] result = new float[n];
            for (int i = 0; i < n; ++i)
                result[i] = ReadSingle();
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public override double ReadDouble()
        {
            var val = base.ReadDouble();
            return sameEndian ? val : EndianTools.ReverseBytes(val);
        }

        public double[] ReadNDoubles(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new double[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            var result = new double[n];
            for (int i = 0; i < n; ++i)
                result[i] = ReadDouble();
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public override short ReadInt16()
        {
            var val = base.ReadInt16();
            return sameEndian ? val : EndianTools.ReverseBytes(val);
        }

        public short[] ReadNInt16(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new short[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            short[] result = new short[n];
            for (int i = 0; i < n; ++i)
                result[i] = ReadInt16();
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public override ushort ReadUInt16()
        {
            var val = base.ReadUInt16();
            return sameEndian ? val : EndianTools.ReverseBytes(val);
        }

        public ushort[] ReadNUInt16(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new ushort[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            ushort[] result = new ushort[n];
            for (int i = 0; i < n; ++i)
                result[i] = ReadUInt16();
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public override int ReadInt32()
        {
            var val = base.ReadInt32();
            return sameEndian ? val : EndianTools.ReverseBytes(val);
        }

        public int[] ReadNInt32(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new int[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            int[] result = new int[n];
            for (int i = 0; i < n; ++i)
                result[i] = ReadInt32();
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public override uint ReadUInt32()
        {
            var val = base.ReadUInt32();
            return sameEndian ? val : EndianTools.ReverseBytes(val);
        }

        public uint[] ReadNUInt32(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new uint[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            uint[] result = new uint[n];
            for (int i = 0; i < n; ++i)
                result[i] = ReadUInt32();
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public byte[] ReadNBytes(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new byte[0];
            try
            {
                var pos = BaseStream.Position;
                BaseStream.Seek(offset, SeekOrigin.Begin);
                var array = new byte[n];
                Read(array, 0, (int)n);
                if (restore)
                    BaseStream.Seek(pos, SeekOrigin.Begin);
                return array;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public sbyte[] ReadNSBytes(uint offset, uint n, bool restore = true)
        {
            if (n == 0) return new sbyte[0];
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            var array = new sbyte[n];

            for (int i = 0; i < n; ++i)
                array[i] = ReadSByte();

            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return array;
        }
    }
}
