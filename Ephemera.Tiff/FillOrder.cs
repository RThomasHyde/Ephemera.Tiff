using System.Diagnostics.CodeAnalysis;
#pragma warning disable 1591

namespace Ephemera.Tiff
{
    /// <summary>
    /// Values for the FillOrder field.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum FillOrder : ushort
    {
        Unknown = 0,
        MSB2LSB = 1,
        LSB2MSB = 2
    }
}