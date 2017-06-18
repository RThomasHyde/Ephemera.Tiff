using System.IO;
using System.Linq;
using Ephemera.Tiff;
using Xunit;

namespace TiffDocumentTest
{
    public static class TestUtils
    {
        public static TiffDocument SaveLoad(TiffDocument tiffDoc)
        {
            using (var s = new MemoryStream())
            {
                tiffDoc.Write(s);
                s.Seek(0, SeekOrigin.Begin);
                return new TiffDocument(s);
            }
        }

        public static void FieldCheckCramps(TiffDirectory dir)
        {

            Assert.Equal(14, dir.Fields.Count);
            Assert.Equal(800, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(607, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(CompressionType.PACKBITS, dir.CompressionType);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(72d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(8, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SubfileType].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(12, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(51, dir[TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(51, dir[TiffTag.StripByteCounts].GetValues<int>().Count());
        }

        public static void FieldCheckCrampsTiled(TiffDirectory dir)
        {
            Assert.Equal(18, dir.Fields.Count);
            Assert.Equal(800, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(607, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(CompressionType.None, dir.CompressionType);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(256, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(12, dir[TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(12, dir[TiffTag.StripByteCounts].GetValues<int>().Count());
            Assert.Equal(256, dir[TiffTag.TileWidth].GetValue<int>());
            Assert.Equal(256, dir[TiffTag.TileLength].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.MinSampleValue].GetValue<int>());
            Assert.Equal(255, dir[TiffTag.MaxSampleValue].GetValue<int>());
            Assert.Equal(0, dir[32996].GetValue<int>());
            Assert.Equal(1, dir[32997].GetValue<int>());
            Assert.Equal(1, dir[32998].GetValue<int>());
        }

        public static void FieldCheckFax2D(TiffDirectory dir)
        {
            Assert.Equal(21, dir.Fields.Count);
            Assert.Equal(1728, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(1082, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.CCITT_T4, dir.CompressionType);
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.StripOffsets].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(4294967295, dir[TiffTag.RowsPerStrip].GetValue<uint>());
            Assert.Equal(32525, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(204d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(98d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(4, dir[292].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.PageNumber].GetValues<int>().Count());
            Assert.Equal("fax2tiff", dir[TiffTag.Software].GetValue<string>());
            Assert.Equal(0, dir[TiffTag.BadFaxLines].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.CleanFaxData].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.ConsecutiveBadFaxLines].GetValue<int>());
        }

        public static void FieldCheckG3Test(TiffDirectory dir)
        {
            Assert.Equal(21, dir.Fields.Count);
            Assert.Equal(1728, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(1103, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.CCITT_T4, dir.CompressionType);
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.StripOffsets].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(4294967295, dir[TiffTag.RowsPerStrip].GetValue<uint>());
            Assert.Equal(50110, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(204d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(98d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0, dir[292].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.PageNumber].GetValues<int>().Count());
            Assert.Equal("fax2tiff", dir[TiffTag.Software].GetValue<string>());
            Assert.Equal(0, dir[TiffTag.BadFaxLines].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.CleanFaxData].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.ConsecutiveBadFaxLines].GetValue<int>());
        }

        public static void FieldCheckJello(TiffDirectory dir)
        {
            Assert.Equal(12, dir.Fields.Count);
            Assert.Equal(256, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(192, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.PACKBITS, dir.CompressionType);
            Assert.Equal(3, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(6, dir[TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(32, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(6, dir[TiffTag.StripByteCounts].GetValues<int>().Count());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(768, dir[TiffTag.ColorMap].GetValues<int>().Count());
        }

        public static void FieldCheckJim___Ah(TiffDirectory dir)
        {
            Assert.Equal(14, dir.Fields.Count);
            Assert.Equal(0, dir[TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(664, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(813, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, dir.CompressionType);
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.Thresholding].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(813, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(67479, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(300d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(300d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
        }

        public static void FieldCheckJim___Cg(TiffDirectory dir)
        {
            Assert.Equal(14, dir.Fields.Count);
            Assert.Equal(0, dir[TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(277, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(339, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, dir.CompressionType);
            Assert.Equal(1, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(339, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(93903, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(125d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(125d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.HalftoneHints].GetValues<int>().Count());
            Assert.Equal(new short[] { 203, 8 }, dir[TiffTag.HalftoneHints].GetValues<short>());
        }

        public static void FieldCheckJim___Dg(TiffDirectory dir)
        {
            Assert.Equal(13, dir.Fields.Count);
            Assert.Equal(0, dir[TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(277, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(339, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, dir.CompressionType);
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(339, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(93903, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(125d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(125d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
        }

        public static void FieldCheckJim___Gg(TiffDirectory dir)
        {
            Assert.Equal(14, dir.Fields.Count);
            Assert.Equal(0, dir[TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(277, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(339, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, dir.CompressionType);
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(339, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(93903, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(125d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(125d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(new short[] { 1, 254 }, dir[TiffTag.HalftoneHints].GetValues<short>());
        }

        public static void FieldCheckOff_L16(TiffDirectory dir)
        {
            Assert.Equal(16, dir.Fields.Count);
            Assert.Equal(333, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(225, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(16, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal((ushort)34676, dir[TiffTag.Compression].GetValue<ushort>());
            Assert.Equal(32844, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(29, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(29, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(72d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.SampleFormat].GetValue<int>());
            Assert.Equal(179d, dir[37439].GetValue<double>());
        }

        public static void FieldCheckOff_Luv24(TiffDirectory dir)
        {
            Assert.Equal(16, dir.Fields.Count);
            Assert.Equal(333, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(225, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 16, 16, 16 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal((ushort)34677, dir[TiffTag.Compression].GetValue<ushort>());
            Assert.Equal(32845, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(29, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(29, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(72d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(new[] { 2, 2, 2 }, dir[TiffTag.SampleFormat].GetValues<int>());
            Assert.Equal(179d, dir[37439].GetValue<double>());
        }

        public static void FieldCheckOff_Luv32(TiffDirectory dir)
        {
            Assert.Equal(16, dir.Fields.Count);
            Assert.Equal(333, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(225, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 16, 16, 16 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal((ushort)34676, dir[TiffTag.Compression].GetValue<ushort>());
            Assert.Equal(32845, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(29, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(29, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(72d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(new[] { 2, 2, 2 }, dir[TiffTag.SampleFormat].GetValues<int>());
            Assert.Equal(179d, dir[37439].GetValue<double>());
        }

        public static void FieldCheckOxford(TiffDirectory dir)
        {
            Assert.Equal(10, dir.Fields.Count);
            Assert.Equal(601, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(81, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, dir.CompressionType);
            Assert.Equal(2, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(243, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(243, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(2, dir[TiffTag.PlanarConfiguration].GetValue<int>());
        }

        public static void FieldCheckQuadJpeg(TiffDirectory dir)
        {
            Assert.Equal(14, dir.Fields.Count);
            Assert.Equal(512, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(384, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.JPEG, dir.CompressionType);
            Assert.Equal(6, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(24, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(16, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(24, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0d, dir[TiffTag.XPosition].GetValue<double>());
            Assert.Equal(0d, dir[TiffTag.YPosition].GetValue<double>());
            Assert.Equal(574, dir[TiffTag.JPEGTables].Count);
            Assert.Equal(new[] { 0, 255, 128, 255, 128, 255 }, dir[TiffTag.ReferenceBlackWhite].GetValues<int>());
        }

        public static void FieldCheckQuadLzw(TiffDirectory dir)
        {
            Assert.Equal(13, dir.Fields.Count);
            Assert.Equal(512, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(384, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, dir.CompressionType);
            Assert.Equal(2, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(77, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(5, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(77, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0d, dir[TiffTag.XPosition].GetValue<double>());
            Assert.Equal(0d, dir[TiffTag.YPosition].GetValue<double>());
            Assert.Equal(0, dir[32995].GetValue<int>());
        }

        public static void FieldCheckQuadTile(TiffDirectory dir)
        {
            Assert.Equal(18, dir.Fields.Count);
            Assert.Equal(512, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(384, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, dir.CompressionType);
            Assert.Equal(2, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(12, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(128, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(12, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(new[] { 0, 0, 0 }, dir[TiffTag.MinSampleValue].GetValues<int>());
            Assert.Equal(new[] { 255, 255, 255 }, dir[TiffTag.MaxSampleValue].GetValues<int>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(128, dir[TiffTag.TileWidth].GetValue<int>());
            Assert.Equal(128, dir[TiffTag.TileLength].GetValue<int>());
            Assert.Equal(new[] { 0, 0, 0 }, dir[32996].GetValues<int>());
            Assert.Equal(1, dir[32997].GetValue<int>());
            Assert.Equal(1, dir[32998].GetValue<int>());
        }

        public static void FieldCheckSmallliz(TiffDirectory dir)
        {
            Assert.Equal(26, dir.Fields.Count);
            Assert.Equal(0, dir[TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(160, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(160, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.OJPEG, dir.CompressionType);
            Assert.Equal(6, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(160, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(3447, dir[TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(100d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(100d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal("HP IL v1.1", dir[TiffTag.Software].GetValue<string>());
            Assert.Equal(1, dir[TiffTag.JPEGProc].GetValue<int>());
            Assert.True(dir.HasTag(TiffTag.JPEGInterchangeFormat));
            Assert.Equal(4608, dir[TiffTag.JPEGInterchangeFormatLength].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.JPEGRestartInterval].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.JPEGQTables].Count);
            Assert.Equal(3, dir[TiffTag.JPEGDCTables].Count);
            Assert.Equal(3, dir[TiffTag.JPEGACTables].Count);
            Assert.Equal(new[] { 0.299, 0.587, 0.114 }, dir[TiffTag.YCbCrCoefficients].GetValues<double>());
            Assert.Equal(new[] { 2, 2 }, dir[TiffTag.YCbCrSubSampling].GetValues<int>());
            Assert.Equal(1, dir[TiffTag.YCbCrPositioning].GetValue<int>());
            Assert.Equal(new[] { 0, 255, 128, 255, 128, 255 }, dir[TiffTag.ReferenceBlackWhite].GetValues<int>());
        }

        public static void FieldCheckStrike(TiffDirectory dir)
        {
            Assert.Equal(16, dir.Fields.Count);
            Assert.Equal(256, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(200, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, dir.CompressionType);
            Assert.Equal(2, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(25, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(4, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(25, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(1d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(1d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0d, dir[TiffTag.XPosition].GetValue<double>());
            Assert.Equal(0d, dir[TiffTag.YPosition].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.ExtraSamples].GetValue<int>());

        }

        public static void FieldCheckTextPage1(TiffDirectory dir)
        {
            Assert.Equal(15, dir.Fields.Count);
            Assert.Equal(1, dir[TiffTag.SubfileType].GetValue<int>());
            Assert.Equal(1512, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(359, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(4, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(32809, dir[TiffTag.Compression].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(6, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(64, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(6, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(296.64d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(296.64d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());


        }

        public static void FieldCheckTextPage2(TiffDirectory dir)
        {
            Assert.Equal(15, dir.Fields.Count);
            Assert.Equal(1, dir[TiffTag.SubfileType].GetValue<int>());
            Assert.Equal(1512, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(359, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.Compression].GetValue<int>());
            Assert.Equal(0, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(6, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(64, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(6, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(296.64d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(296.64d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
        }

        public static void FieldCheckYcbcrCat(TiffDirectory dir)
        {
            Assert.Equal(16, dir.Fields.Count);
            Assert.Equal(250, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(325, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, dir.CompressionType);
            Assert.Equal(6, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal("YCbCr conversion of cat.tif", dir[TiffTag.ImageDescription].GetValue<string>());
            Assert.Equal(33, dir[TiffTag.StripOffsets].Count);
            Assert.Equal(1, dir[TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(10, dir[TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(33, dir[TiffTag.StripByteCounts].Count);
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(new[] { 0.2989, 0.587, 0.114 }, dir[TiffTag.YCbCrCoefficients].GetValues<double>());
            Assert.Equal(new[] { 2, 2 }, dir[TiffTag.YCbCrSubSampling].GetValues<int>());
            Assert.Equal(1, dir[TiffTag.YCbCrPositioning].GetValue<int>());
            Assert.Equal(new[] { 0, 255, 128, 255, 128, 255 }, dir[TiffTag.ReferenceBlackWhite].GetValues<int>());
        }

        public static void FieldCheckZackTheCat(TiffDirectory dir)
        {
            Assert.Equal(21, dir.Fields.Count);
            Assert.Equal(234, dir[TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(213, dir[TiffTag.ImageLength].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, dir[TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.OJPEG, dir.CompressionType);
            Assert.Equal(6, dir[TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(75d, dir[TiffTag.XResolution].GetValue<double>());
            Assert.Equal(75d, dir[TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, dir[TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, dir[TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(240, dir[TiffTag.TileWidth].GetValue<int>());
            Assert.Equal(224, dir[TiffTag.TileLength].GetValue<int>());
            Assert.Equal(8, dir[TiffTag.TileOffsets].GetValue<int>());
            Assert.Equal(7076, dir[TiffTag.TileByteCounts].GetValue<int>());
            Assert.Equal(1, dir[TiffTag.JPEGProc].GetValue<int>());
            Assert.Equal(3, dir[TiffTag.JPEGQTables].Count);
            Assert.Equal(3, dir[TiffTag.JPEGDCTables].Count);
            Assert.Equal(3, dir[TiffTag.JPEGACTables].Count);
            Assert.Equal(new[] { 0.299, 0.587, 0.114 }, dir[TiffTag.YCbCrCoefficients].GetValues<double>());
            Assert.Equal(new[] { 2, 2 }, dir[TiffTag.YCbCrSubSampling].GetValues<int>());
            Assert.Equal(new[] { 16, 235, 128, 240, 128, 240 }, dir[TiffTag.ReferenceBlackWhite].GetValues<int>());
        }
    }
}