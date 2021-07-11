using System;
using System.Collections.Generic;
using Ephemera.Tiff.Fields;

namespace Ephemera.Tiff.Infrastructure
{
    internal static class TiffFieldFactory
    {
        public static ITiffFieldInternal ReadField(TiffReader reader)
        {
            var tagNumber = reader.ReadUInt16();
            var tagType = reader.ReadUInt16();

            // special handling for old-style JPEG huffman tables
            switch (tagNumber)
            {
                case (ushort)TiffTag.JPEGQTables:
                    return new FixedSizeTableTiffField(tagNumber, reader);
                case (ushort)TiffTag.JPEGDCTables:
                case (ushort)TiffTag.JPEGACTables:
                    return new VariableSizeTableTiffField(tagNumber, reader);
                case (ushort)TiffTag.SubIFDs:
                    return new SubIfdTiffField(reader);
            }

            if (fieldFuncs.ContainsKey(tagType))
            {
                var field = fieldFuncs[tagType](tagNumber, reader);
                if (field.IsComplex && field.Offset >= reader.BaseStream.Length) 
                    return null;
                return field;
            }

            return null;
        }

        public static ITiffField CreateField(ushort tagNumber, Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            if (!typeMap.ContainsKey(typeCode))
                throw new TiffException($"Cannot create a field of type {typeCode}.");
            return fieldFuncs[(ushort)typeMap[typeCode]](tagNumber, null);
        }

        private static readonly Dictionary<TypeCode, TiffFieldType> typeMap =
            new Dictionary<TypeCode, TiffFieldType>
            {
                [TypeCode.UInt16] = TiffFieldType.Short,
                [TypeCode.Int16] = TiffFieldType.Long,
                [TypeCode.UInt32] = TiffFieldType.SShort,
                [TypeCode.Int32] = TiffFieldType.SLong,
                [TypeCode.String] = TiffFieldType.ASCII,
                [TypeCode.Single] = TiffFieldType.Float,
                [TypeCode.Double] = TiffFieldType.Double,
                [TypeCode.Byte] = TiffFieldType.Byte,
                [TypeCode.SByte] = TiffFieldType.SByte,
                [TypeCode.Decimal] = TiffFieldType.Rational
            };

        private static readonly Dictionary<ushort, Func<ushort, TiffReader, ITiffFieldInternal>> fieldFuncs =
            new Dictionary<ushort, Func<ushort, TiffReader, ITiffFieldInternal>>
            {
                [1] = (tag, reader) => new ByteTiffField(tag, reader),
                [2] = (tag, reader) => new AsciiTiffField(tag, reader),
                [3] = (tag, reader) => new ShortTiffField(tag, reader),
                [4] = (tag, reader) => new LongTiffField(tag, reader),
                [5] = (tag, reader) => new RationalTiffField(tag, reader),
                [6] = (tag, reader) => new SByteTiffField(tag, reader),
                [7] = (tag, reader) => new ByteTiffField(tag, reader),
                [8] = (tag, reader) => new SShortTiffField(tag, reader),
                [9] = (tag, reader) => new SLongTiffField(tag, reader),
                [10] = (tag, reader) => new RationalTiffField(tag, reader),
                [11] = (tag, reader) => new FloatTiffField(tag, reader),
                [12] = (tag, reader) => new DoubleTiffField(tag, reader)
            };
    }
}