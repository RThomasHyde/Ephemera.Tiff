using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TiffDocument : IDisposable
    {
        /// <summary>
        /// Counts the pages.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static int CountPages(string fileName)
        {
            using (var tiffStream = File.OpenRead(fileName))
            {
                var reader = new TiffReader(tiffStream);
                return SeekToZeroIfd(reader);
            }
        }

        /// <summary>
        ///     Copies an existing <see cref="TiffDocument" /> instance.
        /// </summary>
        /// <param name="source">The <see cref="TiffDocument" /> instance to copy.</param>
        public TiffDocument(TiffDocument source)
        {
            foreach (var dir in source.Directories)
                Directories.Add(new TiffDirectory(dir));
        }

        /// <summary>
        ///     Creates a new <see cref="TiffDocument" /> instnace from a TIFF file.
        /// </summary>
        /// <param name="fileName">The filename of a TIFF file.</param>
        public TiffDocument(string fileName)
        {
            using (Stream tiffStream = File.OpenRead(fileName))
                ReadDocument(tiffStream);
        }

        /// <summary>
        ///     Creates a new <see cref="TiffDocument" /> instnace from the specified page number of a TIFF file.
        /// </summary>
        /// <param name="fileName">The filename of a TIFF file.</param>
        /// <param name="pageNumber">The page number to read (1-based).</param>
        public TiffDocument(string fileName, int pageNumber)
        {
            using (Stream tiffStream = File.OpenRead(fileName))
                ReadDocument(tiffStream, pageNumber);
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
        public TiffDocument() { }

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
        public void AddDirectory(TiffDirectory directory)
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
        /// Writes the TIFF image to a file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        /// <param name="options">Option flags.</param>
        /// <remarks>
        /// By default, OJPEG compression will be retained in the output file. Passing
        /// TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        public void Write(string fileName, TiffOptions options = TiffOptions.None)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
                Write(stream, options);
        }

        /// <summary>
        /// Writes a TIFF file consisting of the specified directory.
        /// </summary>
        /// <param name="fileName">The output file name</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        /// <param name="options">Option flags.</param>
        /// <remarks>
        /// By default, OJPEG compression will be retained in the output file. Passing
        /// TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        public void Write(string fileName, int index, TiffOptions options = TiffOptions.None)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
                Write(stream, index, options);
        }

        /// <summary>
        /// Writes the TIFF image to the specified stream.
        /// </summary>
        /// <param name="stream">A writable wtream.</param>
        /// <param name="options">Option flags.</param>
        /// <remarks>
        /// By default, OJPEG compression will be retained in the output file. Passing
        /// TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        /// <exception cref="TiffException">
        /// Cannot write a TiffDocument with no pages.
        /// or
        /// Stream is not writeable
        /// or
        /// The TiffDocument could not be written to the stream. See inner exception for details.
        /// </exception>
        public void Write(Stream stream, TiffOptions options = TiffOptions.None)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanWrite)
                throw new TiffException("Stream is not writeable");

            try
            {
                var writer = new TiffWriter(stream);
                writer.WriteHeader();
                Directories.ForEach(dir => WriteDirectory(writer, dir, options));
                writer.Write((uint)0);
                writer.Flush();
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The TiffDocument could not be written to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        /// Writes a TIFF image consisting of the specified directory to the specified stream.
        /// </summary>
        /// <param name="stream">A writable wtream.</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        /// <param name="options">Option flags.</param>
        /// <exception cref="TiffException">
        /// <remarks>
        /// By default, OJPEG compression will be retained in the output file. Passing
        /// TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        /// Cannot write a TiffDocument with no pages.
        /// or
        /// Stream is not writeable.
        /// or
        /// The TiffDocument could not be written to the stream. See inner exception for details.
        /// </exception>
        public void Write(Stream stream, int index, TiffOptions options = TiffOptions.None)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanWrite)
                throw new TiffException("Stream is not writeable.");

            try
            {
                var writer = new TiffWriter(stream);
                {
                    writer.WriteHeader();

                    // store the position of the directory offset
                    var ifdPointer = writer.Position;

                    // write a placeholder for the IFD pointer
                    writer.Write((uint)0);

                    // write the directory
                    var block = Directories[index - 1].Write(writer, options);

                    // go back and update the pointer to the directory
                    writer.Seek(ifdPointer, SeekOrigin.Begin);
                    writer.Write((uint)block.IFDPosition);

                    writer.Seek(block.NextIFDPosition, SeekOrigin.Begin);
                    writer.Write((uint)0);
                }
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The TiffDocument could not be written to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        /// Appends the directories of this instance to the specified TIFF file.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <param name="options">Option flags.</param>
        /// <remarks>
        /// If the specified file does not already exist it will be created.
        /// By default, OJPEG compression will be retained in the output file. Passing
        /// TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        public void WriteAppend(string filename, TiffOptions options = TiffOptions.None)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
                WriteAppend(fs, options);
        }

        /// <summary>
        /// Appends the specified directory to an existing TIFF file.
        /// </summary>
        /// <param name="filename">The output file name.</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        /// <param name="options">Option flags.</param>
        /// <remarks>
        /// If the specified file does not already exist it will be created.
        /// By default, OJPEG compression will be retained in the output file. Passing
        /// TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        public void WriteAppend(string filename, int index, TiffOptions options = TiffOptions.None)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
                WriteAppend(fs, index, options);
        }

        /// <summary>
        /// Appends this instance's directories to a TIFF image stream.
        /// </summary>
        /// <param name="stream">A readable/writable TIFF stream.</param>
        /// <param name="options">Option flags.</param>
        /// <exception cref="TiffException">
        /// Cannot write a TiffDocument with no pages.
        /// or
        /// Stream is not readable/writeable.
        /// or
        /// The TiffDocument could not be appended to the stream. See inner exception for details.
        /// </exception>
        /// <remarks>
        /// If the stream is empty, a TIFF header will be written so that the stream will contain
        /// a complete TIFF image. Otherwise the stream is expected to be positioned at the start
        /// of the TIFF header. By default, OJPEG compression will be retained in the output file.
        /// Passing TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        public void WriteAppend(Stream stream, TiffOptions options = TiffOptions.None)
        {
            if (Directories.Count == 0)
                throw new TiffException("Cannot write a TiffDocument with no pages.");

            if (!stream.CanRead || !stream.CanWrite)
                throw new TiffException("Stream is not readable/writeable.");

            try
            {
                TiffWriter writer;

                if (stream.Length == 0)
                {
                    writer = new TiffWriter(stream);
                    writer.WriteHeader();
                }
                else
                {
                    // seek to the "end" of the TIFF (the first IFD pointer with a value of zero)
                    var reader = new TiffReader(stream);
                    SeekToZeroIfd(reader);

                    writer = new TiffWriter(stream, reader.ByteOrder);
                }

                Directories.ForEach(dir => WriteDirectory(writer, dir, options));
                writer.Flush();
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The TiffDocument could not be appended to the stream. See inner exception for details.", e);
            }
        }

        /// <summary>
        /// Appends the specified directory to a TIFF stream.
        /// </summary>
        /// <param name="stream">A readable/writable TIFF stream.</param>
        /// <param name="index">The index of the directory to write (1-based).</param>
        /// <param name="options">Option flags.</param>
        /// <exception cref="TiffException">
        /// Cannot write a TiffDocument with no pages.
        /// or
        /// Stream is not readable/writeable.
        /// or
        /// The specified page could not be written to the stream. See inner exception for details.
        /// </exception>
        /// <exception cref="ArgumentException">Value out of range, must be greater than 0 and less then or equal to the page count. - index</exception>
        /// <remarks>
        /// If the stream is empty, a TIFF header will be written so that the stream will contain
        /// a complete TIFF image. Otherwise the stream is expected to be positioned at the start
        /// of the TIFF header. By default, OJPEG compression will be retained in the output file.
        /// Passing TiffOptions.ConvertOJPEGToJPEG will rewrite JPEG tags to comply with the newer
        /// JPEG spec defined in Tech Note 2 of the TIFF specification.
        /// </remarks>
        public void WriteAppend(Stream stream, int index, TiffOptions options = TiffOptions.None)
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
                doc.AddDirectory(Directories[index - 1]);
                doc.WriteAppend(stream, options);
            }
            catch (Exception e)
            {
                throw new TiffException(
                    "The specified page could not be written to the stream. See inner exception for details.", e);
            }
        }

        private void ReadDocument(Stream stream, int pageNumber = 0)
        {
            var reader = new TiffReader(stream);
            var ifd = reader.ReadUInt32();

            try
            {
                int page = 0;
                while (ifd != 0)
                {
                    page++;
                    if (pageNumber > 0 && page != pageNumber)
                    {
                        ifd = SkipIfd(reader, ifd);
                        continue;
                    }
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

        private static int SeekToZeroIfd(TiffReader reader)
        {
            int count = 0;
            var ifd = reader.ReadUInt32();
            while (ifd != 0)
            {
                count++;
                ifd = SkipIfd(reader, ifd);
            }
            reader.Seek(-sizeof(uint), SeekOrigin.Current);
            return count;
        }

        private static uint SkipIfd(TiffReader reader, uint ifd)
        {
            reader.Seek(ifd, SeekOrigin.Begin);
            var numTags = reader.ReadInt16();
            reader.Seek(numTags * TiffConstants.TIFF_TAG_SIZE, SeekOrigin.Current);
            return reader.ReadUInt32();
        }

        private static void WriteDirectory(TiffWriter writer, TiffDirectory directory, TiffOptions options)
        {
            // store the position of the offset to the directory we're about to write
            var ifdPointerPos = writer.BaseStream.Position;

            // write zero as a temporary offset marker for this directory
            writer.Write(0);

            // seek to the end of the stream and write the directory
            writer.BaseStream.Seek(0, SeekOrigin.End);
            var directoryBlock = directory.Write(writer, options);

            // write zero as next IFD offset
            writer.BaseStream.Seek(directoryBlock.NextIFDPosition, SeekOrigin.Begin);
            writer.Write(0);

            // seek back to the 'next IFD' offset marker and write the position of the directory
            writer.BaseStream.Seek(ifdPointerPos, SeekOrigin.Begin);
            writer.Write((uint) directoryBlock.IFDPosition);

            // return to the start of the next directory
            writer.BaseStream.Seek(directoryBlock.NextIFDPosition, SeekOrigin.Begin);
        }

        /// <summary>
        /// TiffDocument does not need to be disposed; this was left in for backward-compatibility
        /// </summary>
        public void Dispose()
        {
        }
    }
}