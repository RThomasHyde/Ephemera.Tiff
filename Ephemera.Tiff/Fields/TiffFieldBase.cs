using System;
using System.Collections.Generic;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    internal abstract class TiffFieldBase<TValue> : ITiffFieldInternal
    {
        public virtual bool IsComplex => false;
        public ushort TagNum { get; protected set; }
        public ushort TypeNum { get; protected set; }
        protected List<TValue> Values { get; set; }
        public virtual int Count => Values.Count;

        public TiffTag Tag
        {
            get
            {
                if (Enum.IsDefined(typeof(TiffTag), TagNum))
                    return (TiffTag)TagNum;
                return TiffTag.Unknown;
            }
        }

        public TiffFieldType Type
        {
            get
            {
                if (Enum.IsDefined(typeof(TiffFieldType), TypeNum))
                    return (TiffFieldType)TypeNum;
                return TiffFieldType.Unknown;
            }
        }

        public T GetValue<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
            if (Values == null) Values = new List<TValue>();
            try
            {
                return (T) Convert.ChangeType(Values[0], typeof(T));
            }
            catch (Exception e) when (e is InvalidCastException || e is OverflowException)
            {
                throw new TiffException(
                    $"The value of tag {TagNum} ({Tag}) could not be converted to type '{typeof(T).Name}'", e);
            }
        }

        public virtual IEnumerable<T> GetValues<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
            if (Values == null) Values = new List<TValue>();
            try
            {
                return Values.Select(item => Convert.ChangeType(item, typeof(T))).OfType<T>();
            }
            catch (Exception e) when (e is InvalidCastException || e is OverflowException)
            {
                throw new TiffException(
                    $"The values of tag {TagNum} ({Tag}) could not be converted to type '{typeof(T).Name}'", e);
            }
        }

        public void SetValue(object value, int index = 0)
        {
            if (Values == null) Values = new List<TValue>();
            TValue newValue = (TValue) Convert.ChangeType(value, typeof(TValue));
            if (index >= Count)
                Values.Add(newValue);
            else
                Values[index] = newValue;
        }

        public virtual void SetValue<T>(T value, int index = 0) where T : IComparable, IConvertible, IEquatable<T>
        {
            if (value == null)
                throw new TiffException("Null values cannot be stored in a TIFF field.");
            if (Values == null) Values = new List<TValue>();
            var newValue = (TValue) Convert.ChangeType(value, typeof(TValue));
            if (index >= Count)
                Values.Add(newValue);
            else
                Values[index] = newValue;
        }

        public void AddValue(object value)
        {
            if (Values == null) Values = new List<TValue>();
            SetValue(value, Count);
        }

        public virtual void AddValue<T>(T value) where T : IComparable, IConvertible, IEquatable<T>
        {
            if (Values == null) Values = new List<TValue>();
            SetValue(value, Count);
        }

        public uint Offset { get; set; }

        void ITiffFieldInternal.WriteEntry(TiffWriter writer)
        {
            writer.Write(TagNum);
            writer.Write(TypeNum);
            writer.Write(Count);
            WriteOffset(writer);
        }

        protected abstract void WriteOffset(TiffWriter writer);

        void ITiffFieldInternal.WriteData(TiffWriter writer)
        {
            throw new NotImplementedException();
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            throw new NotImplementedException();
        }
    }
}