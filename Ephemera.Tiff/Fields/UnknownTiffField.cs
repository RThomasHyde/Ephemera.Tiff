using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ephemera.Tiff
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal sealed class UnknownTiffField : ITiffFieldInternal
    {
        public UnknownTiffField(ushort num, ushort type, TiffReader reader = null)
        {
            TagNum = num;
            TypeNum = type;
            if (reader != null)
            {
                Count = (int)reader.ReadUInt32();
                ((ITiffFieldInternal) this).Offset = reader.ReadUInt32();
            }
        }

        uint ITiffFieldInternal.Offset { get; set; }

        public bool DataExceeds4Bytes => false;

        void ITiffFieldInternal.WriteTag(Stream s)
        {
            // ignored.
        }

        void ITiffFieldInternal.WriteData(Stream s)
        {
            // ignored.
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new UnknownTiffField(TagNum, TypeNum);
        }

        public ushort TagNum { get; }
        public ushort TypeNum { get; }

        public TiffTag Tag
        {
            get
            {
                if (Enum.IsDefined(typeof(TiffTag), TagNum))
                    return (TiffTag) TagNum;
                return TiffTag.Unknown;
            }
        }

        public TiffFieldType Type => TiffFieldType.Unknown;
        public int Count { get; private set; }

        public T GetValue<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
            return default(T);
        }

        public IEnumerable<T> GetValues<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
            return new List<T>();
        }

        public void SetValue<T>(T value, int index = 0) where T : IComparable, IConvertible, IEquatable<T>
        {
            
        }

        public void AddValue<T>(T value) where T : IComparable, IConvertible, IEquatable<T>
        {
            
        }
    }
}