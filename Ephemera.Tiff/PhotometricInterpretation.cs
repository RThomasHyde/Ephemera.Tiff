using System.Diagnostics.CodeAnalysis;
#pragma warning disable 1591

namespace Ephemera.Tiff
{
    /// <summary>
    /// Values for the PhotometricInterpretation tag.
    /// </summary>
    /// <remarks>
    /// Enum names and values were taken from the <see href="http://www.awaresystems.be/imaging/tiff/tifftags/photometricinterpretation.html">Aware Systems website</see>.
    /// </remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum PhotometricInterpretation : ushort
    {
        WhiteIsZero = 0,
        BlackIsZero = 1,
        RGB = 2,
        PaletteColor = 3,
        TransparencyMask = 4,
        Separated = 5,
        YCbCr = 6,
        CIELab = 8,
        ICCLab = 9,
        ITULab = 10,
        CFA = 32803,
        LOGLUV = 32844,
        LinearRaw = 34892,
        Unknown = 65535
    }
}