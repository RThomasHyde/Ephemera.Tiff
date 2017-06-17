using System;
using System.Collections.Generic;
using System.IO;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff
{
    /// <summary>
    ///     Represents a TIFF image document and its contents.
    /// </summary>
    /// <remarks>
    ///     This class is intended for parsing TIFF files, inspecting their properties,
    ///     and modifying their structure (e.g. adding/removing pages and fields). It
    ///     does not provide any means to decode the actual image data, convert any
    ///     image to or from any other format, or alter the compression of the image data.
    /// </remarks>
    public class TiffDocument
    {
        /// <summary>
        ///     Copies an existing <see cref="TiffDocument" /> instance.
        /// </summary>
        /// <param name="source">The <see cref="TiffDocument" /> instance to copy.</param>
        public TiffDocument(TiffDocument source)
        {
            foreach (var dir in source.Directories)
            {
                Directories.Add(new TiffDirectory(dir));
            }
        }

        /// <summary>
        ///     Creates a new <see cref="TiffDocument" /> instnace from a file.
        /// </summary>
        /// <param name="fileName">The filename of a TIFF file.</param>
        public TiffDocument(string fileName)
        {
            using (Stream tiffStream = File.OpenRead(fileName))
            {
                if (!tiffStream.CanRead || !tiffStream.CanSeek)
                    throw new TiffException("Stream cannot be read.");

                ReadDocument(tiffStream);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="TiffDocument" /> instance from a stream.
        /// </summary>
        /// <param name="stream">A readable stream containing a TIFF image.</param>
        /// <remarks>
        ///     The stream which is passed in is expected to be positioned at the start
        ///     of the TIFF header.
        /// </remarks>
        public TiffDocument(Stream stream)
        {
            ReadDocument(stream);
        }

        /// <summary>
        ///     Creates an empty <see cref="TiffDocument" /> instance.
        /// </summary>
        public TiffDocument()
        {
        }

        /// <summary>
        ///     Gets the TIFF document's directories.
        /// </summary>
        public List<TiffDirectory> Directories { get; } = new List<TiffDirectory>();

        /// <summary>
        ///     Gets the number of directories in the TIFF document.
        /// </summary>
        public int PageCount => Directories.Count;

        /// <summary>
        ///     Adds a directory.
        /// </summary>
        /// <param name="directory">A <see cref="TiffDirectory" /> instance.</param>
        public void AddPage(TiffDirectory directory)
        {
            Directories.Add(directory);
        }

        /// <summary>
        ///     Appends the pages of another <see cref="TiffDirectory" /> instance.
        /// </summary>
        /// <param name="tiff">A TiffDocument instance</param>
        public void Append(TiffDocument tiff)
        {
            Directories.AddRange(tiff.Directories);
        }

        /// <summary>
        ///     Appends the pages of a TIFF file.
        /// </summary>
        /// <param name="filename">The file name.</param>
        public void Append(string filename)
        {
            var doc = new TiffDocument(filename);
            Append(doc);
        }

        /// <summary>
        ///     Gets the TIFF image as a stream.
        /// </summary>
        /// <returns>A stream containing a TIFF image.</returns>
        public Stream GetStream()
        {
            Stream result = new MemoryStream();
            Write(result);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        /// <summary>
        ///     Removes the directory at the specified index.
        /// </summary>
        /// <param name="index">The index of the directory to remove (1-based).</param>
        public void RemoveDirectory(int index)
        {
            if (PageCount == 1) return;
            Directories.RemoveAt(index - 1);
        }

        /// <summary>
        ///     Removes the specified directory.
        /// </summary>
        /// <param name="page">The directory to remove.</param>
        public void RemoveDirectory(TiffDirectory page)
        {
            if (PageCount == 1) return;
            Directories.Remove(page);
        }

        /// <summary>
        ///     Writes the TIFF image to a file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        public void Write(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                Write(stream);
                stream.Close();
            }
        }

        /// <summary>
        ///     Writes a TIFF file consisting of the specified directory.
        /// </summary>
        /// <param name="fileName">The output file name</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        public void Write(string fileName, int index)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                Write(stream, index);
                stream.Close();
            }
        }

        /// <summary>
        ///     Writes the TIFF image to the specified stream.
        /// </summary>
        /// <param name="stream">A writable wtream.</param>
        public void Write(Stream stream)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanWrite)
                throw new TiffException("Stream is not writeable");

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
                throw new TiffException(
                    "The TiffDocument could not be written to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        ///     Writes a TIFF image consisting of the specified directory to the specified stream.
        /// </summary>
        /// <param name="stream">A writable wtream.</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        public void Write(Stream stream, int index)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanWrite)
                throw new TiffException("Stream is not writeable.");

            try
            {
                var writer = new BinaryWriter(stream);
                {
                    //write the header
                    writer.Write(TiffConstants.BOM_LSB2_MSB);
                    writer.Write(TiffConstants.MAGIC);

                    //write the directory
                    var block = Directories[index - 1].Write(writer);
                    writer.BaseStream.Seek(block.IFDPointerPosition, SeekOrigin.Begin);
                    writer.Write(block.IFDPosition);
                    writer.Flush();
                    writer.BaseStream.Seek(block.NextIFDPosition, SeekOrigin.Begin);
                }
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The TiffDocument could not be written to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        ///     Appends the directories of this instance to the specified TIFF file.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <remarks>
        ///     If the specified file does not already exist it will be created.
        /// </remarks>
        public void WriteAppend(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                WriteAppend(fs);
                fs.Close();
            }
        }

        /// <summary>
        ///     Appends the specified directory to an existing TIFF file.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        /// <remarks>
        ///     If the specified file does not exist, it will be created.
        /// </remarks>
        public void WriteAppend(string filename, int index)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                WriteAppend(fs, index);
                fs.Close();
            }
        }

        /// <summary>
        ///     Appends this instance's directories to a TIFF image stream.
        /// </summary>
        /// <param name="stream">A readable/writable TIFF stream.</param>
        /// <remarks>
        ///     If the stream is empty, a TIFF header will be written so that the stream will contain
        ///     a complete TIFF image. Otherwise the stream is expected to be positioned at the start
        ///     of the TIFF header.
        /// </remarks>
        public void WriteAppend(Stream stream)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanRead || !stream.CanWrite)
                throw new TiffException("Stream is not readable/writeable.");

            try
            {
                BinaryWriter writer;

                if (stream.Length == 0)
                {
                    // write header
                    writer = new BinaryWriter(stream);
                    writer.Write(TiffConstants.BOM_LSB2_MSB);
                    writer.Write(TiffConstants.MAGIC);
                }
                else
                {
                    // seek to the "end" of the TIFF, which is the first IFD pointer with a value of zero
                    var reader = new TiffReader(stream); //also validates the TIFF header
                    SeekToZeroIfd(reader);

                    writer = new BinaryWriter(stream);
                }

                // write the directories
                WriteDirectories(writer);
                writer.Flush();
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The TiffDocument could not be appended to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        ///     Appends the specified directory to a TIFF stream.
        /// </summary>
        /// <param name="stream">A readable/writable TIFF stream.</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        /// <remarks>
        ///     If the stream is empty, a TIFF header will be written so that the stream will contain
        ///     a complete TIFF image. Otherwise the stream is expected to be positioned at the start
        ///     of the TIFF header.
        /// </remarks>
        public void WriteAppend(Stream stream, int index)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanRead || !stream.CanWrite)
                throw new TiffException("Stream is not readable/writeable.");

            if (index > PageCount)
            {
                throw new ArgumentException(
                    "Value out of range, must be greater than 0 and less then or equal to the page count.",
                    nameof(index));
            }

            try
            {
                var doc = new TiffDocument();
                doc.AddPage(Directories[index - 1]);
                doc.WriteAppend(stream);
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The specified page could not be written to the stream. See inner exception for details.", e);
            }
        }

        private void ReadDocument(Stream stream)
        {
            var reader = new TiffReader(stream);
            var ifd = reader.ReadUInt32();

            try
            {
                while (ifd != 0)
                {
                    reader.Seek(ifd, SeekOrigin.Begin);
                    var directory = new TiffDirectory(reader);
                    Directories.Add(directory);
                    ifd = directory.NextIfdOffset;
                }
            }
            catch (Exception e)
            {
                throw new TiffException("Could not read TIFF pages. See inner exception for details.", e);
            }
        }

        private void SeekToZeroIfd(TiffReader reader)
        {
            var ifd = reader.ReadUInt32();
            while (ifd != 0)
            {
                reader.Seek(ifd, SeekOrigin.Begin);
                var numTags = reader.ReadInt16();
                reader.Seek(numTags * TiffConstants.TIFF_TAG_SIZE, SeekOrigin.Current);
                ifd = reader.ReadUInt32();
            }
            reader.Seek(-sizeof(uint), SeekOrigin.Current);
        }

        private void WriteDirectories(BinaryWriter writer)
        {
            foreach (var directory in Directories)
            {
                // write the directory
                var directoryBlock = directory.Write(writer);

                // write zero as next IFD offset
                writer.Write(0);

                // seek back to the 'next IFD' marker and write the position of the directory
                writer.BaseStream.Seek(directoryBlock.IFDPointerPosition, SeekOrigin.Begin);
                writer.Write((uint) directoryBlock.IFDPosition);

                // return to the start of the next directory
                writer.BaseStream.Seek(directoryBlock.NextIFDPosition, SeekOrigin.Begin);
            }
        }
    }
}