using System;
using System.Collections.Generic;
using System.IO;

namespace Ephemera.Tiff
{
    /// <summary>
    /// Represents a TIFF image document and its contents.
    /// </summary>
    /// <remarks>
    /// This class is intended for parsing TIFF files, inspecting their properties,
    /// and modifying their structure (e.g. adding/removing pages). It does not 
    /// provide any means to decode the actual image data, convert any image to or 
    /// from any other format, or alter the compression of the image data.
    /// </remarks>
    public sealed class TiffDocument
    {
        /// <summary>
        /// Copies an existing <see cref="TiffDocument"/> instance.
        /// </summary>
        /// <param name="source">The source document.</param>
        public TiffDocument(TiffDocument source)
        {
            foreach (var dir in source.Directories)
            {
                Directories.Add(new TiffDirectory(dir));
            }
        }

        /// <summary>
        /// Creates a new <see cref="TiffDocument"/> from a file.
        /// </summary>
        /// <param name="fileName">The filename of a TIFF file.</param>
        public TiffDocument(string fileName)
        {
            using (Stream tiffStream = File.OpenRead(fileName))
            {
                if (!tiffStream.CanRead || !tiffStream.CanSeek)
                {
                    throw new TiffException("Stream cannot be read.");
                }

                ReadDocument(tiffStream);
            }
        }

        /// <summary>
        /// Creates a new TiffDocument from a stream
        /// </summary>
        /// <param name="stream">A readable TIFF Stream</param>
        public TiffDocument(Stream stream)
        {
            ReadDocument(stream);
        }

        /// <summary>
        /// Creates an empty TiffDocument
        /// </summary>
        public TiffDocument() { }

        /// <summary>
        /// A list of the TIFF document's directories.
        /// </summary>
        public List<TiffDirectory> Directories { get; } = new List<TiffDirectory>();

        /// <summary>
        /// Gets the number of pages in the TIFF document
        /// </summary>
        public int PageCount => Directories.Count;

        /// <summary>
        /// Appends the pages of another TIFF document
        /// to this TIFF document
        /// </summary>
        /// <param name="tiff">A TiffDocument instance</param>
        public void Append(TiffDocument tiff)
        {
            Directories.AddRange(tiff.Directories);
        }

        /// <summary>
        /// Appends TIFF pages from the specified file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Append(string filename)
        {
            var doc = new TiffDocument(filename);
            Append(doc);
        }

        /// <summary>
        /// Adds a single existing page to this TIFF document
        /// </summary>
        /// <param name="directory">A TiffDirectory instance.</param>
        public void AddPage(TiffDirectory directory)
        {
            Directories.Add(directory);
        }

        /// <summary>
        /// Removes a page from the TIFF document.
        /// </summary>
        /// <param name="page">The page number to remove (1-based).</param>
        public void RemovePage(int page)
        {
            Directories.RemoveAt(page - 1);
        }

        /// <summary>
        /// Removes a page from the TIFF document.
        /// </summary>
        /// <param name="page">The page to remove.</param>
        public void RemovePage(TiffDirectory page)
        {
            Directories.Remove(page);
        }

        /// <summary>
        /// Writes the TIFF document to a file
        /// </summary>
        /// <param name="fileName">The output filename</param>
        public void Write(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                Write(stream);
                stream.Close();
            }
        }

        /// <summary>
        /// Writes a single page of the TIFF document a file
        /// </summary>
        /// <param name="fileName">The output filename</param>
        /// <param name="page">The page number to write (1-based)</param>
        public void Write(string fileName, int page)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                Write(stream, page);
                stream.Close();
            }
        }

        /// <summary>
        /// Writes the pages of this TIFF document to an
        /// existing TIFF file.  If the file does not yet exist,
        /// it will be created.
        /// </summary>
        /// <param name="filename">The output filename</param>
        public void WriteAppend(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                WriteAppend(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// Writes a specific page of the TIFF document to a file.
        /// If the file does not yet exist, the result will be a
        /// new single-page TIFF file.
        /// </summary>
        /// <param name="filename">The output filename</param>
        /// <param name="page">The page number to write (1-based)</param>
        public void WriteAppend(string filename, int page)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                WriteAppend(fs, page);
                fs.Close();
            }
        }

        /// <summary>
        /// Gets the TIFF document as a TIFF stream
        /// </summary>
        /// <returns>A new TIFF Stream</returns>
        public Stream GetStream()
        {
            Stream result = new MemoryStream();
            Write(result);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        /// <summary>
        /// Writes the TIFF document to a stream
        /// </summary>
        /// <param name="stream">A writable Stream</param>
        public void Write(Stream stream)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanWrite)
            {
                throw new TiffException("Stream is not writeable");
            }

            try
            {
                var writer = new BinaryWriter(stream);

                // write the header
                writer.Write(TiffConstants.BOM_LSB2_MSB);
                writer.Write(TiffConstants.MAGIC);

                // write the directories
                WriteDirectories(writer);
                writer.Flush();
            }
            catch (Exception e)
            {
                throw new TiffException("The TiffDocument could not be written to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        /// Writes a single page of the TIFF document to a stream
        /// </summary>
        /// <param name="stream">A writable Stream</param>
        /// <param name="page">The page number to write (1-based)</param>
        public void Write(Stream stream, int page)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanWrite)
            {
                throw new TiffException("Stream is not writeable.");
            }

            try
            {
                var writer = new BinaryWriter(stream);
                {
                    //write the header
                    writer.Write(TiffConstants.BOM_LSB2_MSB);
                    writer.Write(TiffConstants.MAGIC);

                    //write the directory
                    var block = Directories[page - 1].Write(writer.BaseStream);
                    writer.BaseStream.Seek(block.IFDPointerPosition, SeekOrigin.Begin);
                    writer.Write(block.IFDPosition);
                    writer.Flush();
                    writer.BaseStream.Seek(block.NextIFDPosition, SeekOrigin.Begin);
                }
            }
            catch (Exception e)
            {
                throw new TiffException("The TiffDocument could not be written to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        /// Appends the pages of the TIFF document to a TIFF stream
        /// </summary>
        /// <param name="stream">A readable/writable TIFF Stream</param>
        public void WriteAppend(Stream stream)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanRead || !stream.CanWrite)
            {
                throw new TiffException("Stream is not readable/writeable.");
            }

            try
            {
                var writer = new BinaryWriter(stream);
                {
                    if (writer.BaseStream.Length == 0)
                    {
                        // write header
                        writer.Write(TiffConstants.BOM_LSB2_MSB);
                        writer.Write(TiffConstants.MAGIC);
                    }
                    else
                    {
                        // seek to the zero IFD marker
                        SeekToZeroIfd(writer.BaseStream);
                    }

                    // write the directories
                    WriteDirectories(writer);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                throw new TiffException("The TiffDocument could not be appended to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        /// Writes a single page of the TIFF document to a TIFF stream
        /// </summary>
        /// <param name="stream">A readable/writable TIFF Stream</param>
        /// <param name="page">The page number to write (1-based)</param>
        public void WriteAppend(Stream stream, int page)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanRead || !stream.CanWrite)
            {
                throw new TiffException("Stream is not readable/writeable.");
            }

            try
            {
                var writer = new BinaryWriter(stream);
                {
                    if (writer.BaseStream.Length == 0)
                    {
                        // write header
                        writer.Write(TiffConstants.BOM_LSB2_MSB);
                        writer.Write(TiffConstants.MAGIC);
                    }
                    else
                    {
                        // find the 'zero IFD' marker
                        SeekToZeroIfd(writer.BaseStream);
                    }

                    // write the directory
                    var block = Directories[page - 1].Write(writer.BaseStream);
                    writer.BaseStream.Seek(block.IFDPointerPosition, SeekOrigin.Begin);
                    writer.Write(block.IFDPosition);
                    writer.Flush();
                    writer.BaseStream.Seek(block.NextIFDPosition, SeekOrigin.Begin);
                }
            }
            catch (Exception e)
            {
                throw new TiffException("The specified page could not be written to the stream. See inner exception for details.", e);
            }
        }

        private void ReadDocument(Stream stream)
        {
            ByteOrder byteOrder;

            var reader = new BinaryReader(stream);
            var byteOrderMark = reader.ReadInt16();
            switch (byteOrderMark)
            {
                case TiffConstants.BOM_LSB2_MSB:
                    byteOrder = ByteOrder.LittleEndian;
                    break;
                case TiffConstants.BOM_MSB2_LSB:
                    byteOrder = ByteOrder.BigEndian;
                    break;
                default:
                    throw new TiffException("Invalid byte order mark (BOM) for a TIFF image.");
            }

            var endianReader = new TiffReader(stream, byteOrder);

            if (endianReader.ReadInt16() != TiffConstants.MAGIC)
                throw new TiffException("TIFF magic not found in file header.");

            var ifd = endianReader.ReadUInt32();

            try
            {
                while (ifd != 0)
                {
                    stream.Seek(ifd, SeekOrigin.Begin);
                    var directory = new TiffDirectory(endianReader);
                    Directories.Add(directory);
                    ifd = directory.NextIfdOffset;
                }
            }
            catch (Exception e)
            {
                throw new TiffException("Could not read TIFF pages. See inner exception for details.", e);
            }
        }

        private void SeekToZeroIfd(Stream stream)
        {
            var reader = new BinaryReader(stream);
            reader.BaseStream.Seek(4, SeekOrigin.Begin);
            var ifd = reader.ReadUInt32();
            while (ifd != 0)
            {
                reader.BaseStream.Seek(ifd, SeekOrigin.Begin);
                short numTags = reader.ReadInt16();
                reader.BaseStream.Seek(numTags * TiffConstants.TIFF_TAG_SIZE, SeekOrigin.Current);
                ifd = reader.ReadUInt32();
            }
            reader.BaseStream.Seek(-sizeof(int), SeekOrigin.Current);
        }

        private void WriteDirectories(BinaryWriter writer)
        {
            // write this TIFF's directories to the file
            foreach (var directory in Directories)
            {
                // write the directory
                var directoryBlock = directory.Write(writer.BaseStream);

                // write zero as next IFD offset
                writer.Write(0);

                // seek back to the 'next IFD' marker and write the position of the directory
                writer.BaseStream.Seek(directoryBlock.IFDPointerPosition, SeekOrigin.Begin);
                writer.Write((uint)directoryBlock.IFDPosition);

                // return to the start of the next directory
                writer.BaseStream.Seek(directoryBlock.NextIFDPosition, SeekOrigin.Begin);
            }
        }
    }
}
