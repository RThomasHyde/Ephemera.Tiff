using System;
using System.Collections.Generic;

namespace Ephemera.Tiff
{
    public interface ITiffField
    {
        /// <summary>
        /// Gets the tag number.
        /// </summary>
        ushort TagNum { get; }

        /// <summary>
        /// Gets the tag name.
        /// </summary>
        TiffTag Tag { get; }

        /// <summary>
        /// Gets the number which represents the data type.
        /// </summary>
        ushort TypeNum { get; }

        /// <summary>
        /// Gets the data type.
        /// </summary>
        TiffFieldType Type { get; }

        /// <summary>
        /// Gets the count of values stored in the tag.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the value of the tag. If there are multiple values, gets the first one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetValue<T>() where T : IComparable, IConvertible, IEquatable<T>;

        /// <summary>
        /// Gets the values stored in the tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        IEnumerable<T> GetValues<T>() where T : IComparable, IConvertible, IEquatable<T>;

        /// <summary>
        /// Overwrites the value at the specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="index">The index, which is optional with a default of zero.</param>
        void SetValue<T>(T value, int index = 0) where T : IComparable, IConvertible, IEquatable<T>;

        /// <summary>
        /// Adds a value to the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        void AddValue<T>(T value) where T : IComparable, IConvertible, IEquatable<T>;
    }
}