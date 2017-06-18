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
        private readonly List<byte[]> imageData = new List<byte[]>();
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
                fields[tag.Key] = ((ITiffFieldInternal) tag.Value).Clone();
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
        public ITiffField this[TiffTag tag] => HasTag(tag) ? Fields[(ushort) tag] : null;

        /// <summary>
        ///     Gets the value of the XResolution field.
        /// </summary>
        public double DpiX
        {
            get
            {
                if (!HasTag(TiffTag.XResolution)) return 200d;
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
                if (!HasTag(TiffTag.YResolution)) return 200d;
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
                if (!HasTag(TiffTag.ImageWidth)) return 0;
                return (int) this[TiffTag.ImageWidth].GetValues<uint>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the ImageLength field.
        /// </summary>
        public int Height
        {
            get
            {
                if (!HasTag(TiffTag.ImageLength)) return 0;
                return (int) this[TiffTag.ImageLength].GetValues<uint>().First();
            }
        }

        /// <summary>
        ///     Gets the value of the SamplesPerPixel field.
        /// </summary>
        public int SamplesPerPixel
        {
            get
            {
                if (!HasTag(TiffTag.SamplesPerPixel)) return 1;
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
                if (!HasTag(TiffTag.Compression)) return CompressionType.Unknown;
                return (CompressionType) this[TiffTag.Compression].GetValues<ushort>().First();
            }
        }

        /// <summary>
        /// Gets the value of the PhotometricInterpretation field.
        /// </summary>
        public PhotometricInterpretation PhotometricInterpretation
        {
            get
            {
                if (!HasTag(TiffTag.PhotometricInterpretation)) return PhotometricInterpretation.Unknown;
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
                if (!HasTag(TiffTag.Thresholding)) return Threshholding.Unknown;
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
                if (!HasTag(TiffTag.FillOrder)) return FillOrder.Unknown;
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
                if (!HasTag(TiffTag.SubIFDs)) return new List<TiffDirectory>();
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
            if (!HasTag(TiffTag.SubIFDs))
                fields[(ushort) TiffTag.SubIFDs] = new SubIfdTiffField(subDirectory);
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
        public bool HasTag(TiffTag tag)
        {
            return Fields.ContainsKey((ushort) tag);
        }

        /// <summary>
        ///     Gets whether the directory has a field with the specified tag number.
        /// </summary>
        /// <param name="tagNumber"></param>
        /// <returns><c>true</c> if the directory has the field, else <c>false</c>.</returns>
        public bool HasTag(ushort tagNumber)
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
            if (!HasTag(tagNumber)) return;
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
            RemoveField((ushort) tag);
        }

        internal DirectoryBlock Write(TiffWriter writer)
        {
            // write out the image data and update the offsets in the appropriate tag
            var offsetsTag = HasTag(TiffTag.TileOffsets) ? this[TiffTag.TileOffsets] : this[TiffTag.StripOffsets];
            for (var i = 0; i < imageData.Count; ++i)
            {
                writer.AlignToWordBoundary();
                offsetsTag.SetValue((uint) writer.Position, i);
                writer.Write(imageData[i], 0, imageData[i].Length);
            }

            // if there is jfif data, write it out and update the JPEGInterchageFormat tag
            if (HasTag(TiffTag.JPEGInterchangeFormat) && jfifData != null)
            {
                writer.AlignToWordBoundary();
                this[TiffTag.JPEGInterchangeFormat].SetValue((uint) writer.Position);
                writer.Write(jfifData);
            }

            // write the tags' data, excluding unknown tags
            foreach (var tag in Fields)
            {
                writer.AlignToWordBoundary();
                ((ITiffFieldInternal) tag.Value).WriteData(writer);
            }

            // store the position of the start of the IFD
            writer.AlignToWordBoundary();
            var ifdPos = writer.Position;

            writer.Write((ushort)Fields.Count);

            foreach (var field in Fields)
            {
                ((ITiffFieldInternal) field.Value).WriteEntry(writer);
            }

            return new DirectoryBlock(ifdPos, writer.BaseStream.Position);
        }

        private void ReadDirectory(TiffReader reader)
        {
            var numTags = reader.ReadUInt16();
            for (var i = 0; i < numTags; ++i)
            {
                var tag = TiffFieldFactory.ReadField(reader);
                if (tag == null) continue;

                fields[tag.TagNum] = tag;

                if (tag.Tag == TiffTag.JPEGInterchangeFormat)
                    jfifPointer = tag.GetValues<uint>().First();

                if (tag.Tag == TiffTag.JPEGInterchangeFormatLength)
                    jfifLength = tag.GetValues<uint>().First();
            }

            NextIfdOffset = reader.ReadUInt32();

            ReadJpegData(reader);
            ReadImageData(reader);
        }

        private void ReadImageData(TiffReader reader)
        {
            var offsets = HasTag(TiffTag.TileOffsets)
                ? this[TiffTag.TileOffsets].GetValues<uint>().ToList()
                : this[TiffTag.StripOffsets].GetValues<uint>().ToList();

            var counts = HasTag(TiffTag.TileByteCounts)
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
            if (!HasTag(TiffTag.JPEGInterchangeFormat)) return;

            if (!HasTag(TiffTag.JPEGInterchangeFormatLength))
            {
                // bad ojpeg, no JFIF length given. assume that JFIF data extents from the JFIF offset
                // to the beginning of the next IFD (this may or may not be the case, but it's the best
                // we can do given the circumstances.
                if (NextIfdOffset > 0) jfifLength = NextIfdOffset - jfifPointer;
                else jfifLength = (uint) (reader.BaseStream.Length - jfifPointer);

                // be nice to future decoders, and add the JFIF length tag.
                fields[(ushort) TiffTag.JPEGInterchangeFormatLength] =
                    new LongTiffField((ushort) TiffTag.JPEGInterchangeFormatLength, jfifLength);
            }

            jfifData = reader.ReadNBytes(jfifPointer, jfifLength);
        }
    }
}