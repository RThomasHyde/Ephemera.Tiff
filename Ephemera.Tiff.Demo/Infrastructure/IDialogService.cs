using Ephemera.Tiff.Demo.ViewModel;

namespace Ephemera.Tiff.Demo
{
    internal interface IDialogService
    {
        CreateFieldViewModel ShowCreateFieldDialog(TiffDirectoryViewModel directoryViewModel);
        void ShowMessage(string message, string caption);
        bool ShowYesNo(string message, string caption);
        string GetSaveFile();
        string GetOpenFile();
    }
}