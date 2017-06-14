namespace Ephemera.Tiff
{
    /// <summary>
    /// Enumeration of TIFF tag data types.
    /// </summary>
    public enum TiffFieldType
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