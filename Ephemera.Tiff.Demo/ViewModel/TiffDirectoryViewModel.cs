using System.Collections.ObjectModel;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class TiffDirectoryViewModel : ViewModelBase
    {
        private readonly TiffDirectory directory;

        public TiffDirectoryViewModel(TiffDirectory directory, int number)
        {
            Number = number;
            this.directory = directory;
            foreach (var field in directory.Tags.Values)
                Fields.Add(new TiffFieldViewModel(field));
        }

        public int Number { get; }
        public int Width => directory.Width;
        public int Height => directory.Height;
        public double DpiX => directory.DpiX;
        public double DpiY => directory.DpiY;

        public ObservableCollection<TiffFieldViewModel> Fields { get; } = new ObservableCollection<TiffFieldViewModel>();
    }
}