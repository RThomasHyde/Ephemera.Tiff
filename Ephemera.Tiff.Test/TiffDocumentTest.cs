using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Ephemera.Tiff;
using Xunit;

namespace TiffDocumentTest
{
    /// <summary>
    /// These unit tests cover all of the images from the libtiff sample set. Each image
    /// is loaded from disk into a TiffDocument instance, which is then saved to a memory 
    /// stream, which is subsequently loaded back into a new TiffDocument instance. The 
    /// fields of each directory of the latter are then checked against the original values
    /// (derived from the output of tiffdump for each image) to insure that:
    /// a) the data is being read from the tiff file correctly, and
    /// b) all directories and their fields survive a roundtrip intact. 
    /// 
    /// These tests do not confirm that TIFF files produced by a TiffDocument instance 
    /// are able to be decoded (i.e. are valid image files). Not all of the images in the 
    /// libtiff sample set are able to be decoded by .NET, so I am not sure how to 
    /// reliably test this without taking a dependency (in the test project) on another 
    /// library which knows how to decode all the different TIFF flavors (e.g. LibTiff.Net).
    /// </summary>
    public class TiffDocumentTest : IDisposable
    {
        private readonly string inputDir;
        private readonly string outputDir;

        public TiffDocumentTest()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            inputDir = Path.Combine(dirPath, "TestImages");
            outputDir = Path.Combine(dirPath, "TestOutput");
            Directory.CreateDirectory(outputDir);
        }

        public void Dispose()
        {
            Directory.Delete(outputDir, true);
        }

        [Fact]
        public void TestCramps()
        {
            var imagePath = Path.Combine(inputDir, "cramps.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckCramps(saved);
        }

        [Fact]
        public void TestCrampsTiled()
        {
            var imagePath = Path.Combine(inputDir, "cramps-tile.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckCrampsTiled(saved);
        }

        [Fact]
        public void TestFax2D()
        {
            var imagePath = Path.Combine(inputDir, "fax2d.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckFax2D(saved);
        }

        [Fact]
        public void TestG3Test()
        {
            var imagePath = Path.Combine(inputDir, "g3test.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckG3Test(saved);
        }

        [Fact]
        public void TestJello()
        {
            var imagePath = Path.Combine(inputDir, "jello.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckJello(saved);
        }

        [Fact]
        public void TestJim___Ah()
        {
            var imagePath = Path.Combine(inputDir, "jim___ah.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckJim___Ah(saved);
        }

        [Fact]
        public void TestJim___Cg()
        {
            var imagePath = Path.Combine(inputDir, "jim___cg.tif");
            var tiffDoc = new TiffDocument(imagePath);
             TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckJim___Cg(saved);
        }

        [Fact]
        public void TestJim___Dg()
        {
            var imagePath = Path.Combine(inputDir, "jim___dg.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckJim___Dg(saved);
        }

        [Fact]
        public void TestJim___Gg()
        {
            var imagePath = Path.Combine(inputDir, "jim___gg.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckJim___Gg(saved);
        }

        [Fact]
        public void TestOff_L16()
        {
            var imagePath = Path.Combine(inputDir, "off_l16.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckOff_L16(saved);
        }

        [Fact]
        public void TestOff_Luv24()
        {
            var imagePath = Path.Combine(inputDir, "off_luv24.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckOff_Luv24(saved);
        }

        [Fact]
        public void TestOff_Luv32()
        {
            var imagePath = Path.Combine(inputDir, "off_luv32.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckOff_Luv32(saved);
        }

        [Fact]
        public void TestOxford()
        {
            var imagePath = Path.Combine(inputDir, "oxford.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckOxford(saved);
        }

        [Fact]
        public void TestQuadJpeg()
        {
            var imagePath = Path.Combine(inputDir, "quad-jpeg.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckQuadJpeg(saved);
        }

        [Fact]
        public void TestQuadLzw()
        {
            var imagePath = Path.Combine(inputDir, "quad-lzw.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckQuadLzw(saved);
        }

        [Fact]
        public void TestQuadTile()
        {
            var imagePath = Path.Combine(inputDir, "quad-tile.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckQuadTile(saved);
        }

        [Fact]
        public void TestSmallliz()
        {
            var imagePath = Path.Combine(inputDir, "smallliz.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckSmallliz(saved);
        }

        [Fact]
        public void TestStrike()
        {
            var imagePath = Path.Combine(inputDir, "strike.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckStrike(saved);
        }

        [Fact]
        public void TestText()
        {
            var imagePath = Path.Combine(inputDir, "text.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckText(saved);
        }

        [Fact]
        public void TestYcbcrCat()
        {
            var imagePath = Path.Combine(inputDir, "ycbcr-cat.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckYcbcrCat(saved);
        }

        [Fact]
        public void TestZackTheCat()
        {
            var imagePath = Path.Combine(inputDir, "zackthecat.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = SaveLoad(tiffDoc);
            FieldCheckZackTheCat(saved);
        }

        private static TiffDocument SaveLoad(TiffDocument tiffDoc)
        {
            using (var s = new MemoryStream())
            {
                tiffDoc.Write(s);
                s.Seek(0, SeekOrigin.Begin);
                return new TiffDocument(s);
            }
        }

        private static void FieldCheckCramps(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(14, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(800, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(607, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(CompressionType.PackBits, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SubfileType].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(12, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(51, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(51, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValues<int>().Count());
        }

        private static void FieldCheckCrampsTiled(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(18, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(800, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(607, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(CompressionType.None, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(256, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(12, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(12, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValues<int>().Count());
            Assert.Equal(256, tiffDoc.Directories[0][TiffTag.TileWidth].GetValue<int>());
            Assert.Equal(256, tiffDoc.Directories[0][TiffTag.TileLength].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.MinSampleValue].GetValue<int>());
            Assert.Equal(255, tiffDoc.Directories[0][TiffTag.MaxSampleValue].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][32996].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][32997].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][32998].GetValue<int>());
        }

        private static void FieldCheckFax2D(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(21, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(1728, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(1082, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.CCITT_T4, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(4294967295, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<uint>());
            Assert.Equal(32525, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(204d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(98d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(4, tiffDoc.Directories[0][292].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PageNumber].GetValues<int>().Count());
            Assert.Equal("fax2tiff", tiffDoc.Directories[0][TiffTag.Software].GetValue<string>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.BadFaxLines].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.CleanFaxData].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.ConsecutiveBadFaxLines].GetValue<int>());
        }

        private static void FieldCheckG3Test(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(21, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(1728, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(1103, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.CCITT_T4, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(4294967295, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<uint>());
            Assert.Equal(50110, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(204d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(98d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][292].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PageNumber].GetValues<int>().Count());
            Assert.Equal("fax2tiff", tiffDoc.Directories[0][TiffTag.Software].GetValue<string>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.BadFaxLines].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.CleanFaxData].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.ConsecutiveBadFaxLines].GetValue<int>());
        }

        private static void FieldCheckJello(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(12, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(256, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(192, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.PackBits, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(32, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValues<int>().Count());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(768, tiffDoc.Directories[0][TiffTag.ColorMap].GetValues<int>().Count());
        }

        private static void FieldCheckJim___Ah(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(14, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(664, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(813, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.Thresholding].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValues<int>().Count());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(813, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(67479, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(300d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(300d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
        }

        private static void FieldCheckJim___Cg(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(14, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(277, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(339, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal((uint)198, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValue<uint>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(339, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(93903, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(125d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(125d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.HalftoneHints].GetValues<int>().Count());
            Assert.Equal(new short[] { 203, 8 }, tiffDoc.Directories[0][TiffTag.HalftoneHints].GetValues<short>());
        }

        private static void FieldCheckJim___Dg(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(13, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(277, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(339, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal((uint)186, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValue<uint>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(339, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(93903, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(125d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(125d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
        }

        private static void FieldCheckJim___Gg(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(14, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(277, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(339, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(CompressionType.None, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal((uint)198, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValue<uint>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(339, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(93903, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(125d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(125d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(new short[] {1, 254}, tiffDoc.Directories[0][TiffTag.HalftoneHints].GetValues<short>());
        }

        private static void FieldCheckOff_L16(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(16, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(333, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(225, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(16, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal((ushort)34676, tiffDoc.Directories[0][TiffTag.Compression].GetValue<ushort>());
            Assert.Equal(32844, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(29, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(29, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.SampleFormat].GetValue<int>());
            Assert.Equal(179d, tiffDoc.Directories[0][37439].GetValue<double>());
        }

        private static void FieldCheckOff_Luv24(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(16, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(333, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(225, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] {16, 16, 16}, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal((ushort)34677, tiffDoc.Directories[0][TiffTag.Compression].GetValue<ushort>());
            Assert.Equal(32845, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(29, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(29, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(new[] {2, 2, 2}, tiffDoc.Directories[0][TiffTag.SampleFormat].GetValues<int>());
            Assert.Equal(179d, tiffDoc.Directories[0][37439].GetValue<double>());
        }

        private static void FieldCheckOff_Luv32(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(16, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(333, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(225, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 16, 16, 16 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal((ushort)34676, tiffDoc.Directories[0][TiffTag.Compression].GetValue<ushort>());
            Assert.Equal(32845, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(29, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(29, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(72d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(new[] { 2, 2, 2 }, tiffDoc.Directories[0][TiffTag.SampleFormat].GetValues<int>());
            Assert.Equal(179d, tiffDoc.Directories[0][37439].GetValue<double>());
        }

        private static void FieldCheckOxford(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(10, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(601, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(81, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(243, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(243, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
        }

        private static void FieldCheckQuadJpeg(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(14, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(512, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(384, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.JPEG, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(24, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(16, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(24, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0d, tiffDoc.Directories[0][TiffTag.XPosition].GetValue<double>());
            Assert.Equal(0d, tiffDoc.Directories[0][TiffTag.YPosition].GetValue<double>());
            Assert.Equal(574, tiffDoc.Directories[0][TiffTag.JPEGTables].Count);
            Assert.Equal(new[] {0,255,128,255,128,255}, tiffDoc.Directories[0][TiffTag.ReferenceBlackWhite].GetValues<int>());
        }

        private static void FieldCheckQuadLzw(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(13, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(512, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(384, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(77, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(5, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(77, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0d, tiffDoc.Directories[0][TiffTag.XPosition].GetValue<double>());
            Assert.Equal(0d, tiffDoc.Directories[0][TiffTag.YPosition].GetValue<double>());
            Assert.Equal(0, tiffDoc.Directories[0][32995].GetValue<int>());
        }

        private static void FieldCheckQuadTile(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(18, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(512, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(384, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(12, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(128, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(12, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(new[] {0, 0, 0}, tiffDoc.Directories[0][TiffTag.MinSampleValue].GetValues<int>());
            Assert.Equal(new[] {255, 255, 255}, tiffDoc.Directories[0][TiffTag.MaxSampleValue].GetValues<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(128, tiffDoc.Directories[0][TiffTag.TileWidth].GetValue<int>());
            Assert.Equal(128, tiffDoc.Directories[0][TiffTag.TileLength].GetValue<int>());
            Assert.Equal(new[] {0, 0, 0}, tiffDoc.Directories[0][32996].GetValues<int>());
            Assert.Equal(1, tiffDoc.Directories[0][32997].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][32998].GetValue<int>());
        }

        private static void FieldCheckSmallliz(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(26, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.NewSubfileType].GetValue<int>());
            Assert.Equal(160, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(160, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.OJPEG, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(610, tiffDoc.Directories[0][TiffTag.StripOffsets].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(160, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(3447, tiffDoc.Directories[0][TiffTag.StripByteCounts].GetValue<int>());
            Assert.Equal(100d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(100d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal("HP IL v1.1", tiffDoc.Directories[0][TiffTag.Software].GetValue<string>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.JPEGProc].GetValue<int>());
            Assert.True(tiffDoc.Directories[0].HasTag(TiffTag.JPEGInterchangeFormat));
            Assert.Equal(4608, tiffDoc.Directories[0][TiffTag.JPEGInterchangeFormatLength].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.JPEGRestartInterval].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.JPEGQTables].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.JPEGDCTables].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.JPEGACTables].Count);
            Assert.Equal(new[] { 0.299, 0.587, 0.114 }, tiffDoc.Directories[0][TiffTag.YCbCrCoefficients].GetValues<double>());
            Assert.Equal(new[] { 2, 2 }, tiffDoc.Directories[0][TiffTag.YCbCrSubSampling].GetValues<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.YCbCrPositioning].GetValue<int>());
            Assert.Equal(new[] { 0, 255, 128, 255, 128, 255 }, tiffDoc.Directories[0][TiffTag.ReferenceBlackWhite].GetValues<int>());
        }

        private static void FieldCheckStrike(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(16, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(256, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(200, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(25, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(4, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(25, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(1d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(1d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(0d, tiffDoc.Directories[0][TiffTag.XPosition].GetValue<double>());
            Assert.Equal(0d, tiffDoc.Directories[0][TiffTag.YPosition].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.ExtraSamples].GetValue<int>());
            
        }

        private static void FieldCheckText(TiffDocument tiffDoc)
        {
            Assert.Equal(2, tiffDoc.PageCount);

            Assert.Equal(15, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SubfileType].GetValue<int>());
            Assert.Equal(1512, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(359, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(4, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(32809, tiffDoc.Directories[0][TiffTag.Compression].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(64, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(296.64d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(296.64d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());

            Assert.Equal(15, tiffDoc.Directories[1].Tags.Count);
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.SubfileType].GetValue<int>());
            Assert.Equal(1512, tiffDoc.Directories[1][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(359, tiffDoc.Directories[1][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.BitsPerSample].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.Compression].GetValue<int>());
            Assert.Equal(0, tiffDoc.Directories[1][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.FillOrder].GetValue<int>());
            Assert.Equal(6, tiffDoc.Directories[1][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(64, tiffDoc.Directories[1][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(6, tiffDoc.Directories[1][TiffTag.StripByteCounts].Count);
            Assert.Equal(296.64d, tiffDoc.Directories[1][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(296.64d, tiffDoc.Directories[1][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[1][TiffTag.PlanarConfiguration].GetValue<int>());
        }

        private static void FieldCheckYcbcrCat(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(16, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(250, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(325, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.LZW, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal("YCbCr conversion of cat.tif", tiffDoc.Directories[0][TiffTag.ImageDescription].GetValue<string>());
            Assert.Equal(33, tiffDoc.Directories[0][TiffTag.StripOffsets].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.Orientation].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(10, tiffDoc.Directories[0][TiffTag.RowsPerStrip].GetValue<int>());
            Assert.Equal(33, tiffDoc.Directories[0][TiffTag.StripByteCounts].Count);
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(new[] { 0.2989, 0.587, 0.114 }, tiffDoc.Directories[0][TiffTag.YCbCrCoefficients].GetValues<double>());
            Assert.Equal(new[] { 2, 2 }, tiffDoc.Directories[0][TiffTag.YCbCrSubSampling].GetValues<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.YCbCrPositioning].GetValue<int>());
            Assert.Equal(new[] { 0, 255, 128, 255, 128, 255 }, tiffDoc.Directories[0][TiffTag.ReferenceBlackWhite].GetValues<int>());
        }

        private static void FieldCheckZackTheCat(TiffDocument tiffDoc)
        {
            Assert.Equal(1, tiffDoc.PageCount);
            Assert.Equal(21, tiffDoc.Directories[0].Tags.Count);
            Assert.Equal(234, tiffDoc.Directories[0][TiffTag.ImageWidth].GetValue<int>());
            Assert.Equal(213, tiffDoc.Directories[0][TiffTag.ImageHeight].GetValue<int>());
            Assert.Equal(new[] { 8, 8, 8 }, tiffDoc.Directories[0][TiffTag.BitsPerSample].GetValues<int>());
            Assert.Equal(CompressionType.OJPEG, tiffDoc.Directories[0].CompressionType);
            Assert.Equal(6, tiffDoc.Directories[0][TiffTag.PhotometricInterpretation].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.SamplesPerPixel].GetValue<int>());
            Assert.Equal(75d, tiffDoc.Directories[0][TiffTag.XResolution].GetValue<double>());
            Assert.Equal(75d, tiffDoc.Directories[0][TiffTag.YResolution].GetValue<double>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.PlanarConfiguration].GetValue<int>());
            Assert.Equal(2, tiffDoc.Directories[0][TiffTag.ResolutionUnit].GetValue<int>());
            Assert.Equal(240, tiffDoc.Directories[0][TiffTag.TileWidth].GetValue<int>());
            Assert.Equal(224, tiffDoc.Directories[0][TiffTag.TileLength].GetValue<int>());
            Assert.Equal(8, tiffDoc.Directories[0][TiffTag.TileOffsets].GetValue<int>());
            Assert.Equal(7076, tiffDoc.Directories[0][TiffTag.TileByteCounts].GetValue<int>());
            Assert.Equal(1, tiffDoc.Directories[0][TiffTag.JPEGProc].GetValue<int>());
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.JPEGQTables].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.JPEGDCTables].Count);
            Assert.Equal(3, tiffDoc.Directories[0][TiffTag.JPEGACTables].Count);
            Assert.Equal(new[] { 0.299, 0.587, 0.114 }, tiffDoc.Directories[0][TiffTag.YCbCrCoefficients].GetValues<double>());
            Assert.Equal(new[] { 2, 2 }, tiffDoc.Directories[0][TiffTag.YCbCrSubSampling].GetValues<int>());
            Assert.Equal(new[] { 16, 235, 128, 240, 128, 240 }, tiffDoc.Directories[0][TiffTag.ReferenceBlackWhite].GetValues<int>());
        }
    }
}
