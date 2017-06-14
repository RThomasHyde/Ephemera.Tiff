using System.Collections.ObjectModel;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class TiffStructureViewModel : ViewModelBase
    {
        private readonly TiffDocument tiffDocument;
        private readonly IDemoDialogService demoDialogService;
        private TiffDirectoryViewModel selectedDirectory;
        private TiffFieldViewModel selectedField;

        public TiffStructureViewModel(string fileName, IDemoDialogService demoDialogService)
        {
            this.demoDialogService = demoDialogService;
            tiffDocument = new TiffDocument(fileName);
            int directoryNumber = 1;
            foreach (var directory in tiffDocument.Directories)
            {
                Directories.Add(new TiffDirectoryViewModel(directory, directoryNumber++));
            }
        }

        public ObservableCollection<TiffDirectoryViewModel> Directories { get; } = new ObservableCollection<TiffDirectoryViewModel>();

        public TiffDirectoryViewModel SelectedDirectory
        {
            get => selectedDirectory;
            set
            {
                selectedDirectory = value;
                OnPropertyChanged();
            }
        }

        public TiffFieldViewModel SelectedField
        {
            get => selectedField;
            set
            {
                selectedField = value;
                OnPropertyChanged();
            }
        }
    }
}