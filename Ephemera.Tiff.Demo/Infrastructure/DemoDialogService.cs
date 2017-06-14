using Ephemera.Tiff.Demo.View;
using Ephemera.Tiff.Demo.ViewModel;
using Microsoft.Win32;

namespace Ephemera.Tiff.Demo
{
    internal sealed class DemoDialogService : IDemoDialogService
    {
        public void ShowViewStructureDemo()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "TIFF Files (*.tif, *.tiff)|*.tif;*.tiff",
                RestoreDirectory = true,
                Title = "Select a TIFF file whose structure you would like to view"
            };
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var view = new TiffStructureView {DataContext = new TiffStructureViewModel(ofd.FileName, this)};
                view.Show();
            }
        }

        public void ShowAppendInMemoryDemo()
        {
            
        }

        public void ShowAppendToFileDemo()
        {
            
        }

        public void ShowMergeFolderDemo()
        {
            
        }
    }
}