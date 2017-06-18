using System.Diagnostics.CodeAnalysis;
#pragma warning disable 1591

namespace Ephemera.Tiff
{
    /// <summary>
    /// Enumeration of TIFF image compression types.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum CompressionType : ushort
    {
        Unknown = 0,
        None = 1,
        CCITTRLE = 2,
        CCITTFAX3 = 3,
        CCITT_T4 = 3,
        CCITTFAX4 = 4,
        CCITT_T6 = 4,
        LZW = 5,
        OJPEG = 6,
        JPEG = 7,
        ADOBE_DEFLATE = 8,
        NEXT = 32766,
        CCITTRLEW = 32771,
        PACKBITS = 32773,
        THUNDERSCAN = 32809,
        IT8CTPAD = 32895,
        IT8LW = 32896,
        IT8MP = 32897,
        IT8BL = 32898,
        PIXARFILM = 32908,
        PIXARLOG = 32909,
        DEFLATE = 32946,
        DCS = 32947,
        JBIG = 34661,
        SGILOG = 34676,
        SGILOG24 = 34677,
        JP2000 = 34712,
    }
}