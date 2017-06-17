using System.IO;

namespace Ephemera.Tiff.Infrastructure
{
    internal static class StreamUtils
    {
        public static void AlignToWordBoundary(this BinaryWriter writer)
        {
            writer.BaseStream.AlignToWordBoundary();
        }

        public static void AlignToWordBoundary(this Stream stream)
        {
            var wordAlignBytes = stream.Position % 4;
            if (wordAlignBytes > 0)
            {
                var padBytes = new byte[4 - wordAlignBytes];
                stream.Write(padBytes, 0, padBytes.Length);
            }
        }
    }
}