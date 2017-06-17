using System.IO;

namespace Ephemera.Tiff.Infrastructure
{
    internal interface ITiffFieldInternal : ITiffField
    {
        uint Offset { set; }
        void WriteEntry(BinaryWriter writer);
        void WriteData(BinaryWriter writer);
        ITiffFieldInternal Clone();
    }
}