using System.IO;

namespace Ephemera.Tiff.Infrastructure
{
    internal interface ITiffFieldInternal : ITiffField
    {
        void WriteEntry(TiffWriter writer);
        void WriteData(TiffWriter writer);
        ITiffFieldInternal Clone();
    }
}