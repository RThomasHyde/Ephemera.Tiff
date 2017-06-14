using System.IO;

namespace Ephemera.Tiff
{
    internal interface ITiffFieldInternal : ITiffField
    {
        uint Offset { get; set; }
        bool DataExceeds4Bytes { get; }
        void WriteTag(Stream s);
        void WriteData(Stream s);
        ITiffFieldInternal Clone();
    }
}