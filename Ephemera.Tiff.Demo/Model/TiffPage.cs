using System.Collections.Generic;

namespace Ephemera.Tiff.Demo.Model
{
    public class TiffPage
    {
        public string Name { get; set; }
        public List<Tag> Children { get; } = new List<Tag>();
    }
}