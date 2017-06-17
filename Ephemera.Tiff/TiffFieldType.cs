using System.Diagnostics.CodeAnalysis;

namespace Ephemera.Tiff
{
    /// <summary>
    /// Enumeration of TIFF field data types.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum TiffFieldType : ushort
    {
        Unknown = 0,
        Byte = 1,
        ASCII = 2,
        Short = 3,
        Long = 4,
        Rational = 5,
        SByte = 6,
        Undefined = 7,
        SShort = 8,
        SLong = 9,
        SRational = 10,
        Float = 11,
        Double = 12,
        IFD = 13,
    }
}