using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Fields;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff
{
    /// <summary>
    ///     Represents a TIFF image file directory (IFD), which is essentially a page within a TIFF document.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TiffDirectory
    {
        private readonly SortedDictionary<ushort, ITiffField> fields = new SortedDictionary<ushort, ITiffField>();
        private List<byte[]> imageData = new List<byte[]>();
        private byte[] jfifData;
        private uint jfifLength;
        private uint jfifPointer;

        internal TiffDirectory(TiffReader reader)
        {
            ReadDirectory(reader);
        }

        internal TiffDirectory(TiffDirectory original)
        {
            NextIfdOffset = original.NextIfdOffset;
            jfifPointer = original.jfifPointer;
            jfifLength = original.jfifLength;
            jfifData = original.jfifData;
            foreach (var tag in original.Fields)
            {
                fields[tag.Key] = ((ITiffFieldInternal)tag.Value).Clone();
            }
            foreach (var array in original.imageData)
            {
                var newArray = new byte[array.Length];
                array.CopyTo(newArray, 0);
                imageData.Add(array);
            }
        }

        /// <summary>
        ///     Gets the this directory's fields.
        /// </summary>
        public IReadOnlyDictionary<ushort, ITiffField> Fields => fields;

        /// <summary>
        ///     Gets the <see cref="ITiffField" /> with the specified tag number.
        /// </summary>
        /// <param name="tagNumber">The tag number.</param>
        public ITiffField this[ushort tagNumber] => Fields.ContainsKey(tagNumber) ? Fields[tagNumber] : null;

        /// <summary>
        ///     Gets the <see cref="ITiffField" /> with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public ITiffField this[TiffTag tag] => HasField(tag) ? Fields[(ushort)tag] : null;

        /// <summary>
        ///     Gets the value of the XResolution field.
        /// </summary>
        public double DpiX
        {
            get
            {
                if (!HasField(TiffTag.XResolution)) return 200d;
                return this[TiffTag.XResolution].GetValues<double>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the YResolution field.
        /// </summary>
        public double DpiY
        {
            get
            {
                if (!HasField(TiffTag.YResolution)) return 200d;
                return this[TiffTag.YResolution].GetValues<double>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the ImageWidth field.
        /// </summary>
        public int Width
        {
            get
            {
                if (!HasField(TiffTag.ImageWidth)) return 0;
                return (int)this[TiffTag.ImageWidth].GetValues<uint>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the ImageLength field.
        /// </summary>
        public int Height
        {
            get
            {
                if (!HasField(TiffTag.ImageLength)) return 0;
                return (int)this[TiffTag.ImageLength].GetValues<uint>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the SamplesPerPixel field.
        /// </summary>
        public int SamplesPerPixel
        {
            get
            {
                if (!HasField(TiffTag.SamplesPerPixel)) return 1;
                return this[TiffTag.SamplesPerPixel].GetValues<ushort>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the Compression field.
        /// </summary>
        public CompressionType CompressionType
        {
            get
            {
                if (!HasField(TiffTag.Compression)) return CompressionType.Unknown;
                return (CompressionType)this[TiffTag.Compression].GetValues<ushort>().First();
            }
        }

        /// <summary>
        /// Gets the value of the PhotometricInterpretation field.
        /// </summary>
        public PhotometricInterpretation PhotometricInterpretation
        {
            get
            {
                if (!HasField(TiffTag.PhotometricInterpretation)) return PhotometricInterpretation.Unknown;
                return (PhotometricInterpretation)this[TiffTag.PhotometricInterpretation].GetValues<ushort>().First();
            }
        }

        /// <summary>
        /// Gets the value of the Threshholding field.
        /// </summary>
        public Threshholding Threshholding
        {
            get
            {
                if (!HasField(TiffTag.Thresholding)) return Threshholding.Unknown;
                return (Threshholding)this[TiffTag.Thresholding].GetValues<ushort>().First();
            }
        }

        /// <summary>
        /// Gets the value of the FillOrder field.
        /// </summary>
        public FillOrder FillOrder
        {
            get
            {
                if (!HasField(TiffTag.FillOrder)) return FillOrder.Unknown;
                return (FillOrder)this[TiffTag.FillOrder].GetValues<ushort>().First();
            }
        }

        /// <summary>
        ///     Gets the subdirectories, if any.
        /// </summary>
        public IEnumerable<TiffDirectory> Subdirectories
        {
            get
            {
                if (!HasField(TiffTag.SubIFDs)) return new List<TiffDirectory>();
                if (this[TiffTag.SubIFDs] is SubIfdTiffField tag)
                    return tag.SubIFDs;
                return new List<TiffDirectory>();
            }
        }

        internal uint NextIfdOffset { get; private set; }

        /// <summary>
        ///     Adds a new field to the directory, with the specified tag number and values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagNumber">The tag number.</param>
        /// <param name="values">The values.</param>
        /// <returns>An <see cref="ITiffField"></see> instance.</returns>
        /// <remarks>
        ///     If you are adding a custom field, the TIFF 6 specification recommends using a
        ///     tag number in the range of 65000-65535 in order to avoid collision with baseline
        ///     and reserved tags. Be aware that there is still no guarantee of avoiding
        ///     collisions using this range, so caution should be used. The best thing to do
        ///     in order to avoid collision would be to choose an unallocated tag number in the
        ///     32768-64999 range and contact the Adobe TIFF administrator to register it as a
        ///     private tag before using it in any TIFF files that might escape your private
        ///     environment.
        /// </remarks>
        public ITiffField AddField<T>(ushort tagNumber, IEnumerable<T> values)
            where T : IComparable, IConvertible, IEquatable<T>
        {
            var field = TiffFieldFactory.CreateField(tagNumber, typeof(T));
            foreach (var value in values)
            {
                field.AddValue(value);
            }
            fields[tagNumber] = field;
            return field;
        }

        /// <summary>
        ///     Adds a subdirectory.
        /// </summary>
        /// <param name="subDirectory">The subdirectory to add.</param>
        public void AddSubdirectory(TiffDirectory subDirectory)
        {
            if (!HasField(TiffTag.SubIFDs))
                fields[(ushort)TiffTag.SubIFDs] = new SubIfdTiffField(subDirectory);
            else
            {
                if (this[TiffTag.SubIFDs] is SubIfdTiffField tag)
                    tag.AddDirectory(subDirectory);
            }
        }

        /// <summary>
        ///     Gets whether the directory has a field with the specified tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns><c>true</c> if the directory has the field, else <c>false</c>.</returns>
        public bool HasField(TiffTag tag)
        {
            return Fields.ContainsKey((ushort)tag);
        }

        /// <summary>
        ///     Gets whether the directory has a field with the specified tag number.
        /// </summary>
        /// <param name="tagNumber"></param>
        /// <returns><c>true</c> if the directory has the field, else <c>false</c>.</returns>
        public bool HasField(ushort tagNumber)
        {
            return Fields.ContainsKey(tagNumber);
        }

        /// <summary>
        ///     Removes the field with the corresponding tag number, if it exists.
        /// </summary>
        /// <param name="tagNumber">The tag number.</param>
        /// <exception cref="System.InvalidOperationException">Tag number designates a required field, which cannot be removed.</exception>
        public void RemoveField(ushort tagNumber)
        {
            if (!HasField(tagNumber)) return;
            var field = this[tagNumber];
            if (TiffConstants.RequiredTags.Contains(tagNumber))
                throw new InvalidOperationException(
                    $"Tag number {tagNumber} ({field.Tag}) designates a required field, which cannot be removed.");
            fields.Remove(tagNumber);
        }

        /// <summary>
        ///     Removes the field with the corresponding tag, if it exists.
        /// </summary>
        /// <param name="tag">The tag number.</param>
        /// <exception cref="System.InvalidOperationException">Tag number designates a required field, which cannot be removed.</exception>
        public void RemoveField(TiffTag tag)
        {
            RemoveField((ushort)tag);
        }

        internal DirectoryBlock Write(TiffWriter writer, TiffOptions options)
        {
            var dir = this;

            if (options.HasFlag(TiffOptions.ConvertOJPEGToJPEG) && dir.HasField(TiffTag.JPEGInterchangeFormat) && dir.jfifData != null)
            {
                dir = ConvertOjpegToJpeg(dir);
            }
            else
            {
                // if there is jfif data, write it out and update the JPEGInterchageFormat tag
                if (HasField(TiffTag.JPEGInterchangeFormat) && jfifData != null)
                {
                    writer.AlignToWordBoundary();
                    this[TiffTag.JPEGInterchangeFormat].SetValue((uint)writer.Position);
                    writer.Write(jfifData);
                }
            }

            // write out the image data and update the offsets in the appropriate tag
            var offsetsTag = dir.HasField(TiffTag.TileOffsets) ? dir[TiffTag.TileOffsets] : dir[TiffTag.StripOffsets];
            for (var i = 0; i < dir.imageData.Count; ++i)
            {
                writer.AlignToWordBoundary();
                offsetsTag.SetValue((uint)writer.Position, i);
                writer.Write(dir.imageData[i], 0, dir.imageData[i].Length);
            }

            // write the tags' data, excluding unknown tags
            foreach (var tag in dir.Fields)
            {
                writer.AlignToWordBoundary();
                ((ITiffFieldInternal)tag.Value).WriteData(writer);
            }

            // store the position of the start of the IFD
            writer.AlignToWordBoundary();
            var ifdPos = writer.Position;

            writer.Write((ushort)dir.Fields.Count);

            foreach (var field in dir.Fields)
            {
                ((ITiffFieldInternal)field.Value).WriteEntry(writer);
            }

            return new DirectoryBlock(ifdPos, writer.BaseStream.Position);
        }

        private TiffDirectory ConvertOjpegToJpeg(TiffDirectory dir)
        {
            dir = new TiffDirectory(this);
            var jfifEnd = dir.jfifPointer + dir.jfifLength;

            if (dir.HasField(TiffTag.JPEGQTables))
            {
                if (dir[TiffTag.JPEGQTables].GetValues<int>().All(x => x > dir.jfifPointer && x < jfifEnd))
                    dir.RemoveField(TiffTag.JPEGQTables);
            }

            if (dir.HasField(TiffTag.JPEGDCTables))
            {
                if (dir[TiffTag.JPEGDCTables].GetValues<int>().All(x => x > dir.jfifPointer && x < jfifEnd))
                    dir.RemoveField(TiffTag.JPEGDCTables);
            }

            if (dir.HasField(TiffTag.JPEGACTables))
            {
                if (dir[TiffTag.JPEGACTables].GetValues<int>().All(x => x > dir.jfifPointer && x < jfifEnd))
                    dir.RemoveField(TiffTag.JPEGACTables);
            }

            dir.RemoveField(TiffTag.JPEGInterchangeFormat);
            dir.RemoveField(TiffTag.JPEGInterchangeFormatLength);
            dir.RemoveField(TiffTag.JPEGProc);
            dir.RemoveField(TiffTag.JPEGRestartInterval);
            dir.RemoveField(TiffTag.JPEGLosslessPredictors);
            dir.RemoveField(TiffTag.JPEGPointTransforms);

            dir[TiffTag.Compression].SetValue((ushort)CompressionType.JPEG);
            dir.imageData = new List<byte[]> { dir.jfifData };

            var offsetsTag = new LongTiffField((ushort)TiffTag.StripOffsets, 0);
            dir.fields[(ushort)TiffTag.StripOffsets] = offsetsTag;

            var countsTag = new LongTiffField((ushort)TiffTag.StripByteCounts, dir.jfifLength);
            dir.fields[(ushort)TiffTag.StripByteCounts] = countsTag;

            return dir;
        }

        private void ReadDirectory(TiffReader reader)
        {
            var numTags = reader.ReadUInt16();
            for (var i = 0; i < numTags; ++i)
            {
                var field = TiffFieldFactory.ReadField(reader);
                if (field == null) continue;

                fields[field.TagNum] = field;

                if (field.Tag == TiffTag.JPEGInterchangeFormat)
                    jfifPointer = field.GetValue<uint>();

                if (field.Tag == TiffTag.JPEGInterchangeFormatLength)
                    jfifLength = field.GetValue<uint>();
            }

            NextIfdOffset = reader.ReadUInt32();

            ReadJpegData(reader);
            ReadImageData(reader);
        }

        private void ReadImageData(TiffReader reader)
        {
            var offsets = HasField(TiffTag.TileOffsets)
                ? this[TiffTag.TileOffsets].GetValues<uint>().ToList()
                : this[TiffTag.StripOffsets].GetValues<uint>().ToList();

            var counts = HasField(TiffTag.TileByteCounts)
                ? this[TiffTag.TileByteCounts].GetValues<uint>().ToList()
                : this[TiffTag.StripByteCounts].GetValues<uint>().ToList();

            var pos = reader.BaseStream.Position;
            for (var i = 0; i < offsets.Count; ++i)
            {
                // if the byte counts array is shorter than the offsets array
                // (which should not happen, but better safe than sorry) assume 0 bytes.
                var count = counts.Count >= i ? counts[i] : 0;
                imageData.Add(reader.ReadNBytes(offsets[i], count, false));
            }
            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        private void ReadJpegData(TiffReader reader)
        {
            if (!HasField(TiffTag.JPEGInterchangeFormat) || jfifPointer == 0) return;

            if (!HasField(TiffTag.JPEGInterchangeFormatLength))
            {
                // bad ojpeg, no JFIF length given. assume that JFIF data extents from the JFIF offset
                // to the beginning of the next IFD (this may or may not be the case, but it's the best
                // we can do under the circumstances).
                if (NextIfdOffset > 0) jfifLength = NextIfdOffset - jfifPointer;
                else jfifLength = (uint)(reader.BaseStream.Length - jfifPointer);

                // be nice to future decoders, and add the JFIF length tag.
                fields[(ushort)TiffTag.JPEGInterchangeFormatLength] =
                    new LongTiffField((ushort)TiffTag.JPEGInterchangeFormatLength, jfifLength);
            }

            jfifData = reader.ReadNBytes(jfifPointer, jfifLength);
        }
    }
}