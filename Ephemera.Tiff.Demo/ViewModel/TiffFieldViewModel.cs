using System.Collections.ObjectModel;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class TiffFieldViewModel : ViewModelBase
    {
        private readonly ITiffField field;

        public TiffFieldViewModel(ITiffField field)
        {
            this.field = field;
            var values = field.GetValues<string>();
            foreach (var value in values) 
                Values.Add(value);
        }

        public TiffTag Tag => field.Tag;

        public TiffFieldType Type => field.Type;

        public int Count => field.Count;

        public ObservableCollection<string> Values { get; } = new ObservableCollection<string>();
    }
}