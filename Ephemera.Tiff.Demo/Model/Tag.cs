namespace Ephemera.Tiff.Demo.Model
{
    public class Tag
    {
        public string Name { get; set; }
        public ushort TagNum { get; set; }
        public uint Offset { get; set; }
        public string Value { get; set; }
    }
}