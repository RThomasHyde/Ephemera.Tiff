using System;
using System.Collections.Generic;
using System.Linq;

namespace Ephemera.Tiff
{
    internal abstract class TiffFieldBase<TValue> : ITiffField
    {
        public ushort TagNum { get; protected set; }
        public ushort TypeNum { get; protected set; }
        protected List<TValue> Values { get; set; }
        public int Count => Values.Count;

        public TiffTag Tag
        {
            get
            {
                if (Enum.IsDefined(typeof(TiffTag), (int)TagNum))
                    return (TiffTag)TagNum;
                return TiffTag.Unknown;
            }
        }

        public TiffFieldType Type
        {
            get
            {
                if (Enum.IsDefined(typeof(TiffFieldType), (int)TypeNum))
                    return (TiffFieldType)TypeNum;
                return TiffFieldType.Unknown;
            }
        }

        public T GetValue<T>() where T : IComparable, IConvertible, IEquatable<T>
        {
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

        public virtual void SetValue<T>(T value, int index = 0) where T : IComparable, IConvertible, IEquatable<T>
        {
            if (value == null)
                throw new TiffException("Null values cannot be stored in a TIFF field.");
            var newValue = (TValue) Convert.ChangeType(value, typeof(TValue));
            if (index >= Count)
                Values.Add(newValue);
            else
                Values[index] = newValue;
        }

        public virtual void AddValue<T>(T value) where T : IComparable, IConvertible, IEquatable<T>
        {
            SetValue(value, Count);
        }

        public virtual bool DataExceeds4Bytes
        {
            get
            {
                switch (Type)
                {
                    case TiffFieldType.Unknown:
                        return false;
                    case TiffFieldType.Byte:
                        return Count > 4;
                    case TiffFieldType.ASCII:
                        return true;
                    case TiffFieldType.Short:
                        return Count > 2;
                    case TiffFieldType.Long:
                        return Count > 1;
                    case TiffFieldType.Rational:
                        return true;
                    case TiffFieldType.SByte:
                        return Count > 4;
                    case TiffFieldType.Undefined:
                        return false;
                    case TiffFieldType.SShort:
                        return Count > 2;
                    case TiffFieldType.SLong:
                        return Count > 1;
                    case TiffFieldType.SRational:
                        return true;
                    case TiffFieldType.Float:
                        return true;
                    case TiffFieldType.Double:
                        return true;
                    case TiffFieldType.IFD:
                        return Count > 1;
                    default:
                        return false;
                }
            }
        }
    }
}