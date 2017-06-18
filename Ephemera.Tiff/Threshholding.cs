using System.Diagnostics.CodeAnalysis;
#pragma warning disable 1591

namespace Ephemera.Tiff
{
    /// <summary>
    /// Values for the Threshholding field.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum Threshholding : ushort
    {
        Unknown = 0,
        Bilevel = 1,
        Halftone = 2,
        ErrorDiffuse = 3
    }
}