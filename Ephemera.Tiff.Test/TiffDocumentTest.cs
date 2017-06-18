using System;
using System.IO;
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
            var dirPath = Path.GetDirectoryName(codeBasePath) ?? "";
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
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckCramps(saved.Directories[0]);
        }

        [Fact]
        public void TestCrampsTiled()
        {
            var imagePath = Path.Combine(inputDir, "cramps-tile.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckCrampsTiled(saved.Directories[0]);
        }

        [Fact]
        public void TestFax2D()
        {
            var imagePath = Path.Combine(inputDir, "fax2d.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckFax2D(saved.Directories[0]);
        }

        [Fact]
        public void TestG3Test()
        {
            var imagePath = Path.Combine(inputDir, "g3test.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckG3Test(saved.Directories[0]);
        }

        [Fact]
        public void TestJello()
        {
            var imagePath = Path.Combine(inputDir, "jello.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckJello(saved.Directories[0]);
        }

        [Fact]
        public void TestJim___Ah()
        {
            var imagePath = Path.Combine(inputDir, "jim___ah.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckJim___Ah(saved.Directories[0]);
        }

        [Fact]
        public void TestJim___Cg()
        {
            var imagePath = Path.Combine(inputDir, "jim___cg.tif");
            var tiffDoc = new TiffDocument(imagePath);
             TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckJim___Cg(saved.Directories[0]);
        }

        [Fact]
        public void TestJim___Dg()
        {
            var imagePath = Path.Combine(inputDir, "jim___dg.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckJim___Dg(saved.Directories[0]);
        }

        [Fact]
        public void TestJim___Gg()
        {
            var imagePath = Path.Combine(inputDir, "jim___gg.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckJim___Gg(saved.Directories[0]);
        }

        [Fact]
        public void TestOff_L16()
        {
            var imagePath = Path.Combine(inputDir, "off_l16.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckOff_L16(saved.Directories[0]);
        }

        [Fact]
        public void TestOff_Luv24()
        {
            var imagePath = Path.Combine(inputDir, "off_luv24.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckOff_Luv24(saved.Directories[0]);
        }

        [Fact]
        public void TestOff_Luv32()
        {
            var imagePath = Path.Combine(inputDir, "off_luv32.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckOff_Luv32(saved.Directories[0]);
        }

        [Fact]
        public void TestOxford()
        {
            var imagePath = Path.Combine(inputDir, "oxford.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckOxford(saved.Directories[0]);
        }

        [Fact]
        public void TestQuadJpeg()
        {
            var imagePath = Path.Combine(inputDir, "quad-jpeg.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckQuadJpeg(saved.Directories[0]);
        }

        [Fact]
        public void TestQuadLzw()
        {
            var imagePath = Path.Combine(inputDir, "quad-lzw.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckQuadLzw(saved.Directories[0]);
        }

        [Fact]
        public void TestQuadTile()
        {
            var imagePath = Path.Combine(inputDir, "quad-tile.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckQuadTile(saved.Directories[0]);
        }

        [Fact]
        public void TestSmallliz()
        {
            var imagePath = Path.Combine(inputDir, "smallliz.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckSmallliz(saved.Directories[0]);
        }

        [Fact]
        public void TestStrike()
        {
            var imagePath = Path.Combine(inputDir, "strike.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckStrike(saved.Directories[0]);
        }

        [Fact]
        public void TestText()
        {
            var imagePath = Path.Combine(inputDir, "text.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            Assert.Equal(2, saved.PageCount);
            TestUtils.FieldCheckTextPage1(saved.Directories[0]);
            TestUtils.FieldCheckTextPage2(saved.Directories[1]);
        }

        [Fact]
        public void TestYcbcrCat()
        {
            var imagePath = Path.Combine(inputDir, "ycbcr-cat.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckYcbcrCat(saved.Directories[0]);
        }

        [Fact]
        public void TestZackTheCat()
        {
            var imagePath = Path.Combine(inputDir, "zackthecat.tif");
            var tiffDoc = new TiffDocument(imagePath);
            TiffDocument saved = TestUtils.SaveLoad(tiffDoc);
            TestUtils.FieldCheckZackTheCat(saved.Directories[0]);
        }

        [Fact]
        public void TestReadSpecificPage()
        {
            var imagePath = Path.Combine(inputDir, "text.tif");
            var tiffDoc = new TiffDocument(imagePath, 2);
            Assert.Equal(1, tiffDoc.PageCount);
            TestUtils.FieldCheckTextPage2(tiffDoc.Directories[0]);
        }

        [Fact]
        public void TestCountPages()
        {
            var imagePath = Path.Combine(inputDir, "text.tif");
            Assert.Equal(2, TiffDocument.CountPages(imagePath));
        }

        [Fact]
        public void TestWriteAppendFile()
        {
            var sourcePath = Path.Combine(inputDir, "cramps.tif");
            var targetPath = Path.Combine(outputDir, "merged.tif");
            File.Copy(sourcePath, targetPath, true);
            sourcePath = Path.Combine(inputDir, "text.tif");
            var tiffDoc = new TiffDocument(sourcePath);
            tiffDoc.WriteAppend(targetPath);
            tiffDoc = new TiffDocument(targetPath);
            Assert.Equal(3, tiffDoc.PageCount);
            TestUtils.FieldCheckCramps(tiffDoc.Directories[0]);
            TestUtils.FieldCheckTextPage1(tiffDoc.Directories[1]);
            TestUtils.FieldCheckTextPage2(tiffDoc.Directories[2]);
        }

        [Fact]
        public void TestWriteAppendSpecificPage()
        {
            var sourcePath = Path.Combine(inputDir, "cramps.tif");
            var targetPath = Path.Combine(outputDir, "merged.tif");
            File.Copy(sourcePath, targetPath, true);
            sourcePath = Path.Combine(inputDir, "text.tif");
            var tiffDoc = new TiffDocument(sourcePath);
            tiffDoc.WriteAppend(targetPath, 1);
            tiffDoc = new TiffDocument(targetPath);
            Assert.Equal(2, tiffDoc.PageCount);
            TestUtils.FieldCheckCramps(tiffDoc.Directories[0]);
            TestUtils.FieldCheckTextPage1(tiffDoc.Directories[1]);
        }

        [Fact]
        public void TestReadFailsWhenImageInvalid()
        {
            var imagePath = Path.Combine(inputDir, "NonTiff.bmp");
            Assert.Throws<TiffException>(() => new TiffDocument(imagePath));
        }

        [Fact]
        public void TestWriteAppendFailsWhenTargetInvalid()
        {
            var sourcePath = Path.Combine(inputDir, "NonTiff.bmp");
            var targetPath = Path.Combine(outputDir, "nontiff.tif");
            File.Copy(sourcePath, targetPath, true);
            var imagePath = Path.Combine(inputDir, "cramps.tif");
            var tiffDoc = new TiffDocument(imagePath);
            Assert.Throws<TiffException>(() => tiffDoc.WriteAppend(targetPath));
        }
    }
}
