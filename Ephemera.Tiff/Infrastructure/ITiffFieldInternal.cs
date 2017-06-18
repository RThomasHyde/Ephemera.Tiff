using System.IO;

namespace Ephemera.Tiff.Infrastructure
{
    internal interface ITiffFieldInternal : ITiffField
    {
        uint Offset { set; }
        void WriteEntry(TiffWriter writer);
        void WriteData(TiffWriter writer);
        ITiffFieldInternal Clone();
    }
}