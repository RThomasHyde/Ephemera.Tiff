namespace Ephemera.Tiff.Infrastructure
{
    internal static class TiffConstants
    {
        public const byte TIFF_TAG_SIZE = 12;
        public const ushort BOM_LSB2_MSB = 0x4949;
        public const ushort BOM_MSB2_LSB = 0x4D4D;
        public const ushort MAGIC = 0x002a;

        public static readonly ushort[] RequiredTags = 
        {
            (ushort)TiffTag.ImageWidth,
            (ushort)TiffTag.ImageLength,
            (ushort)TiffTag.Compression,
            (ushort)TiffTag.PhotometricInterpretation,
            (ushort)TiffTag.StripOffsets,
            (ushort)TiffTag.RowsPerStrip,
            (ushort)TiffTag.StripByteCounts,
            (ushort)TiffTag.XResolution,
            (ushort)TiffTag.YResolution,
            (ushort)TiffTag.TileWidth,
            (ushort)TiffTag.TileLength
        };
    }
}