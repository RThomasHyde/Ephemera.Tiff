namespace Ephemera.Tiff.Infrastructure
{
    internal sealed class DirectoryBlock
    {
        public long IFDPointerPosition { get; }
        public long NextIFDPosition { get; }
        public long IFDPosition { get; }

        public DirectoryBlock(long pointer, long position, long nextPosition)
        {
            IFDPointerPosition = pointer;
            IFDPosition = position;
            NextIFDPosition = nextPosition;
        }
    }
}