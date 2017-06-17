using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
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
                Offset = reader.ReadUInt32();
            }
        }

        public uint Offset { get; set; }

        void ITiffFieldInternal.WriteEntry(BinaryWriter writer)
        {
            // ignored.
        }

        void ITiffFieldInternal.WriteData(BinaryWriter writer)
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
                if (Enum.IsDefined(typeof(TiffTag), (int)TagNum))
                    return (TiffTag) TagNum;
                return TiffTag.Unknown;
            }
        }

        public TiffFieldType Type => TiffFieldType.Unknown;
        public int Count { get; }

        public T GetValue<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
            return default(T);
        }

        public IEnumerable<T> GetValues<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
            return new List<T>();
        }

        public void SetValue(object value, int index = 0)
        {
            
        }

        public void SetValue<T>(T value, int index = 0) where T : IComparable, IConvertible, IEquatable<T>
        {
            
        }

        public void AddValue(object value)
        {
            
        }

        public void AddValue<T>(T value) where T : IComparable, IConvertible, IEquatable<T>
        {
            
        }
    }
}