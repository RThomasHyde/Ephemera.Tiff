namespace Ephemera.Tiff.Infrastructure
{
    internal sealed class DirectoryBlock
    {
        public long NextIFDPosition { get; }
        public long IFDPosition { get; }

        public DirectoryBlock(long position, long nextPosition)
        {
            IFDPosition = position;
            NextIFDPosition = nextPosition;
        }
    }
}