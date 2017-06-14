using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class RationalTiffField : TiffFieldBase<double>, ITiffFieldInternal
    {
        uint ITiffFieldInternal.Offset { get; set; }

        internal RationalTiffField(ushort tag, TiffReader reader = null)
        {
            TagNum = tag;
            TypeNum = (ushort) TiffFieldType.Rational;
            if (reader != null) ReadTag(reader);
        }

        private RationalTiffField(RationalTiffField original)
        {
            TagNum = original.TagNum;
            TypeNum = original.TypeNum;
            ((ITiffFieldInternal)this).Offset = ((ITiffFieldInternal)original).Offset;
            Values = new List<double>(original.Values);
        }

        private void ReadTag(TiffReader reader)
        {
            uint count = reader.ReadUInt32();
            var offset = reader.ReadUInt32();
            ((ITiffFieldInternal) this).Offset = offset;

            Values = new List<double>();

            var numDenomArray = reader.ReadNInt32(offset, count * 2);
            int index = 0;
            for (int i = 0; i < count; ++i, index += 2)
            {
                int numerator = numDenomArray[index];
                int denominator = numDenomArray[index + 1];
                if (numerator == 0) Values.Add(0);
                else if (denominator == 0) Values.Add(double.NaN);
                else Values.Add(numerator / (double) denominator);
            }
        }

        void ITiffFieldInternal.WriteTag(Stream s)
        {
            var bytes = BitConverter.GetBytes(TagNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(TypeNum);
            s.Write(bytes, 0, 2);

            bytes = BitConverter.GetBytes(Count);
            s.Write(bytes, 0, 4);

            bytes = BitConverter.GetBytes(((ITiffFieldInternal)this).Offset);
            s.Write(bytes, 0, 4);
        }

        void ITiffFieldInternal.WriteData(Stream s)
        {
            ((ITiffFieldInternal)this).Offset = (uint)s.Position;
            foreach (var value in Values)
            {
                int numerator, denominator;
                ToFraction(value, out numerator, out denominator);
                s.Write(BitConverter.GetBytes(numerator), 0, 4);
                s.Write(BitConverter.GetBytes(denominator), 0, 4);
            }
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new RationalTiffField(this);
        }

        private void ToFraction(double inValue, out int numerator, out int denominator)
        {
            if (inValue == 0.0d)
            {
                numerator = denominator = 0;
                return;
            }

            if (inValue > Int32.MaxValue)
                throw new OverflowException($"Double {inValue} too large");

            if (inValue < -Int32.MaxValue)
                throw new OverflowException($"Double {inValue} too small");

            if (-double.Epsilon < inValue && inValue < double.Epsilon)
                throw new ArithmeticException($"Double {inValue} cannot be represented");

            int sign = Math.Sign(inValue);
            inValue = Math.Abs(inValue);

            ConvertPositiveDouble(sign, inValue, out numerator, out denominator);
        }

        private void ConvertPositiveDouble(int sign, double inValue, out int numerator, out int denominator)
        {
            // Shamelessly stolen from http://homepage.smc.edu/kennedy_john/CONFRAC.PDF
            // with AccuracyFactor == double.Episilon
            int fractionNumerator = (int)inValue;
            double fractionDenominator = 1;
            double previousDenominator = 0;
            double remainingDigits = inValue;
            int maxIterations = 594;    // found at http://www.ozgrid.com/forum/archive/index.php/t-22530.html

            while (remainingDigits != Math.Floor(remainingDigits)
                   && Math.Abs(inValue - (fractionNumerator / fractionDenominator)) > double.Epsilon)
            {
                remainingDigits = 1.0 / (remainingDigits - Math.Floor(remainingDigits));

                double scratch = fractionDenominator;

                fractionDenominator = (Math.Floor(remainingDigits) * fractionDenominator) + previousDenominator;
                fractionNumerator = (int)(inValue * fractionDenominator + 0.5);

                previousDenominator = scratch;

                if (maxIterations-- < 0)
                    break;
            }

            numerator = fractionNumerator * sign;
            denominator = (int)fractionDenominator;
        }
    }
}