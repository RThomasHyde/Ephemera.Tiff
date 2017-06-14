using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff
{
    /// <summary>
    /// Represents a TIFF image file directory (IFD), which is basically a page within a TIFF document.
    /// </summary>
    public sealed class TiffDirectory
    {
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
            foreach (var tag in original.Tags)
            {
                Tags[tag.Key] = ((ITiffFieldInternal) tag.Value).Clone();
            }
            foreach (var array in original.imageData)
            {
                var newArray = new byte[array.Length];
                array.CopyTo(newArray, 0);
                imageData.Add(array);
            }
        }

        /// <summary>
        ///     Gets the TIFF tags associated with this directory.
        /// </summary>
        public SortedDictionary<ushort, ITiffField> Tags { get; } = new SortedDictionary<ushort, ITiffField>();

        /// <summary>
        ///     Gets the <see cref="ITiffField" /> at the specified index.
        /// </summary>
        /// <param name="index">The tag code.</param>
        public ITiffField this[ushort index] => Tags.ContainsKey(index) ? Tags[index] : null;

        /// <summary>
        ///     Gets the <see cref="ITiffField" /> at the specified index.
        /// </summary>
        /// <param name="index">The tag name.</param>
        public ITiffField this[TiffTag index] => HasTag(index) ? Tags[(ushort) index] : null;

        /// <summary>
        ///     Gets the horizontal DPI of the image.
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
        ///     Gets the vertical DPI of the image.
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
        ///     Gets the width of the image.
        /// </summary>
        public int Width
        {
            get
            {
                if (!HasTag(TiffTag.ImageWidth)) return 0;
                return (int)this[TiffTag.ImageWidth].GetValues<uint>().First();
            }
        }

        /// <summary>
        ///     Gets the height of the image.
        /// </summary>
        public int Height
        {
            get
            {
                if (!HasTag(TiffTag.ImageHeight)) return 0;
                return (int)this[TiffTag.ImageHeight].GetValues<uint>().First();
            }
        }

        /// <summary>
        ///     Gets the number of samples per pixel.
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
        ///     Gets the type of image compression.
        /// </summary>
        public CompressionType CompressionType
        {
            get
            {
                if (!HasTag(TiffTag.Compression)) return CompressionType.Other;
                return (CompressionType) this[TiffTag.Compression].GetValues<ushort>().First();
            }
        }

        /// <summary>
        ///     Gets the subdirectories, if any exist.
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
        ///     Adds a subdirectory.
        /// </summary>
        /// <param name="subDirectory">The subdirectory.</param>
        public void AddSubdirectory(TiffDirectory subDirectory)
        {
            if (!HasTag(TiffTag.SubIFDs))
                Tags[(ushort) TiffTag.SubIFDs] = new SubIfdTiffField(subDirectory);
            else
            {
                if (this[TiffTag.SubIFDs] is SubIfdTiffField tag)
                    tag.AddDirectory(subDirectory);
            }
        }

        /// <summary>
        /// Adds a new field to the directory, with the specified tag number and values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagNumber">The tag number.</param>
        /// <param name="values">The values.</param>
        /// <returns>An <see cref="ITiffField"></see> instance.</returns>
        /// <remarks>
        /// If you are adding a custom field, the TIFF 6 specification recommends using a
        /// tag number in the range of 65000-65535 in order to avoid collision with baseline
        /// and reserved tags. Be aware that there is still no guarantee of avoiding
        /// collisions using this range, so caution should be used. The best thing to do
        /// in order to avoid collision would be to choose an unallocated tag number in the 
        /// 32768-64999 range and contact the Adobe TIFF administrator to register it as a 
        /// private tag before using it in any TIFF files that might escape your private
        /// environment.
        /// </remarks>
        public ITiffField AddField<T>(ushort tagNumber, IEnumerable<T> values) where T : IComparable, IConvertible, IEquatable<T>
        {
            var field = TiffFieldFactory.CreateField(tagNumber, typeof(T));
            foreach (var value in values)
                field.AddValue(value);
            return field;
        }

        /// <summary>
        ///     Gets whether the directory contains the specified tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns><c>true</c> if the tag exists, else <c>false</c>.</returns>
        public bool HasTag(TiffTag tag)
        {
            return Tags.ContainsKey((ushort) tag);
        }

        internal DirectoryBlock Write(Stream s)
        {
            var writer = new BinaryWriter(s);

            // store the location of the pointer to this IFD
            var ifdPointerPos = s.Position;

            // advance by 4 bytes (the size of the IFD pointer)
            s.Seek(4, SeekOrigin.Current);

            // write out the image data and update the offsets in the appropriate tag
            var offsetsTag = HasTag(TiffTag.TileOffsets) ? this[TiffTag.TileOffsets] : this[TiffTag.StripOffsets];
            for (var i = 0; i < imageData.Count; ++i)
            {
                AlignToWordBoundary(writer);
                if (((ITiffFieldInternal) offsetsTag).DataExceeds4Bytes)
                    offsetsTag.SetValue((uint) s.Position, i);
                writer.Write(imageData[i], 0, imageData[i].Length);
            }

            // if there is jfif data, write it out and update the JPEGInterchageFormat tag
            if (HasTag(TiffTag.JPEGInterchangeFormat) && jfifData != null)
            {
                AlignToWordBoundary(writer);
                this[TiffTag.JPEGInterchangeFormat].SetValue((uint) s.Position);
                writer.Write(jfifData);
            }

            // write the tags' data, excluding unknown tags
            foreach (var tag in Tags)
            {
                if (tag.Value is UnknownTiffField) continue;
                AlignToWordBoundary(writer);
                ((ITiffFieldInternal) tag.Value).WriteData(s);
            }

            // store the position of the start of the IFD. An IFD must begin on a word 
            // boundary (according to the TIFF 6 spec), so first write any pad bytes required.
            AlignToWordBoundary(writer);
            var ifdPos = writer.BaseStream.Position;

            // write the tag count, excluding invalid tags (those with an unknown data type)
            var tagCount = (ushort) Tags.Count(x => !(x.Value is UnknownTiffField));
            writer.Write(tagCount);

            // write the tags, excluding invalid tags (those with an unknown data type)
            foreach (var tag in Tags)
            {
                if (tag.Value is UnknownTiffField) continue;
                ((ITiffFieldInternal) tag.Value).WriteTag(s);
            }

            return new DirectoryBlock(ifdPointerPos, ifdPos, s.Position);
        }

        private void AlignToWordBoundary(BinaryWriter writer)
        {
            var wordAlignBytes = writer.BaseStream.Position % 4;
            if (wordAlignBytes > 0)
            {
                var padBytes = new byte[4 - wordAlignBytes];
                writer.Write(padBytes);
            }
        }

        private void ReadDirectory(TiffReader reader)
        {
            var numTags = reader.ReadUInt16();
            for (var i = 0; i < numTags; ++i)
            {
                var tag = TiffFieldFactory.ReadField(reader);
                if (tag == null) continue;

                Tags[tag.TagNum] = tag;

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
                uint count = (counts.Count >= i) ? counts[i] : 0;
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
                Tags[(ushort) TiffTag.JPEGInterchangeFormatLength] =
                    new LongTiffField((ushort) TiffTag.JPEGInterchangeFormatLength, jfifLength);
            }

            jfifData = reader.ReadNBytes(jfifPointer, jfifLength);
        }
    }
}