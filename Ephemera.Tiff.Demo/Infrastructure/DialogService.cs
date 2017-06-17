using System.Windows;
using Ephemera.Tiff.Demo.View;
using Ephemera.Tiff.Demo.ViewModel;
using Microsoft.Win32;

namespace Ephemera.Tiff.Demo
{
    internal sealed class DialogService : IDialogService
    {
        public CreateFieldViewModel ShowCreateFieldDialog(TiffDirectoryViewModel directoryViewModel)
        {
            var viewModel = new CreateFieldViewModel();
            var view = new CreateFieldView {DataContext = viewModel};
            var result = view.ShowDialog();
            if (!result.HasValue || !result.Value) return null;
            return viewModel;
        }

        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(Application.Current.MainWindow, message, caption);
        }

        public bool ShowYesNo(string message, string caption)
        {
            var result = MessageBox.Show(Application.Current.MainWindow, message, caption, 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public string GetSaveFile()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "TIFF Files (*.tif, *.tiff)|*.tif;*.tiff",
                DefaultExt = ".tif",
                RestoreDirectory = true
            };
            bool? result = dlg.ShowDialog();
            if (!result.HasValue) return null;
            return dlg.FileName;
        }

        public string GetOpenFile()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "TIFF Files (*.tif, *.tiff)|*.tif;*.tiff",
                RestoreDirectory = true,
                Multiselect = false
            };
            bool? result = dlg.ShowDialog();
            if (!result.HasValue) return null;
            return dlg.FileName;
        }
    }
}