using System;
using System.IO;

namespace Ephemera.Tiff
{
    internal sealed class TiffReader : BinaryReader
    {
        private readonly bool sameEndian;

        public TiffReader(Stream input, ByteOrder order) : base(input)
        {
            sameEndian = BitConverter.IsLittleEndian && order == ByteOrder.LittleEndian;
        }

        public override float ReadSingle()
        {
            var val = base.ReadSingle();
            return sameEndian ? val : ReverseBytes(val);
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
            return sameEndian ? val : ReverseBytes(val);
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
            return sameEndian ? val : ReverseBytes(val);
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
            return sameEndian ? val : ReverseBytes(val);
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
            return sameEndian ? val : ReverseBytes(val);
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
            return sameEndian ? val : ReverseBytes(val);
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
            var pos = BaseStream.Position;
            BaseStream.Seek(offset, SeekOrigin.Begin);
            var array = new byte[n];
            Read(array, 0, (int)n);
            if (restore)
                BaseStream.Seek(pos, SeekOrigin.Begin);
            return array;
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

        private short ReverseBytes(short x)
        {
            return (short)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        private ushort ReverseBytes(ushort x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        private int ReverseBytes(int x)
        {
            return (int)((x & 0x000000FFU) << 24 | (x & 0x0000FF00U) << 8 |
                         (x & 0x00FF0000U) >> 8 | (x & 0xFF000000U) >> 24);
        }

        private uint ReverseBytes(uint x)
        {
            x = (x >> 16) | (x << 16);
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        private float ReverseBytes(float x)
        {
            byte[] temp = BitConverter.GetBytes(x);
            ArrayReverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        private double ReverseBytes(double val)
        {
            byte[] temp = BitConverter.GetBytes(val);
            ArrayReverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        // manual array reverse (because builtin Array.Reverse does unneeded extra checks)
        private static void ArrayReverse(byte[] arr)
        {
            for (int i = 0; i < arr.Length / 2; i++)
            {
                byte tmp = arr[i];
                arr[i] = arr[arr.Length - i - 1];
                arr[arr.Length - i - 1] = tmp;
            }
        }
    }
}
