namespace Ephemera.Tiff
{
    /// <summary>
    /// Enumeration of TIFF image compression types.
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// No compression
        /// </summary>
        None = 1,
        /// <summary>
        /// CCITT modified Huffman RLE
        /// </summary>
        CCITT_RLE = 2,
        /// <summary>
        /// CCITT Group 3 fax
        /// </summary>
        CCITT_T4 = 3,
        /// <summary>
        /// CCITT Group 4 fax
        /// </summary>
        CCITT_T6 = 4,
        /// <summary>
        /// Lempel-Ziv-Welch
        /// </summary>
        LZW = 5,
        /// <summary>
        /// Old-style JPEG
        /// </summary>
        OJPEG = 6,
        /// <summary>
        /// New-style JPEG
        /// </summary>
        JPEG = 7,
        /// <summary>
        /// Adobe deflate
        /// </summary>
        Deflate = 8,
        /// <summary>
        /// PackBits (Macintosh RLE)
        /// </summary>
        PackBits = 32773,
        /// <summary>
        /// Other (unknown) compression
        /// </summary>
        Other
    }
}