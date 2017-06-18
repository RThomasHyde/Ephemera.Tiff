using System;

namespace Ephemera.Tiff.Infrastructure
{
    internal static class EndianTools
    {
        public static short ReverseBytes(short x)
        {
            return (short)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        public static ushort ReverseBytes(ushort x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        public static int ReverseBytes(int x)
        {
            return (int)((x & 0x000000FFU) << 24 | (x & 0x0000FF00U) << 8 |
                         (x & 0x00FF0000U) >> 8 | (x & 0xFF000000U) >> 24);
        }

        public static uint ReverseBytes(uint x)
        {
            x = (x >> 16) | (x << 16);
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        public static float ReverseBytes(float x)
        {
            byte[] temp = BitConverter.GetBytes(x);
            ArrayReverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        public static double ReverseBytes(double val)
        {
            byte[] temp = BitConverter.GetBytes(val);
            ArrayReverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        // manual array reverse (because builtin Array.Reverse does extra checks that aren't needed here)
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