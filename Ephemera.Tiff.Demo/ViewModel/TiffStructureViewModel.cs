using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class TiffStructureViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private TiffDirectoryViewModel selectedDirectoryViewModel;
        private TiffFieldViewModel selectedFieldViewModel;
        private TiffDocument tiffDocument;
        private string title;

        public TiffStructureViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            LoadFileCommand = new DelegateCommand(ExecuteLoadFileCommand);
            OpenInDefaultAppCommand = new DelegateCommand(ExecuteOpenInDefaultAppCommand, CanExecuteOpenInDefaultAppCommand);
            DeleteDirectoryCommand = new DelegateCommand(ExecuteDeleteDirectoryCommand, CanExecuteDeleteDirectoryCommand);
            DeleteFieldCommand = new DelegateCommand(ExecuteDeleteFieldCommand, CanExecuteDeleteFieldCommand);
            AddFieldCommand = new DelegateCommand(ExecuteAddFieldCommand, CanExecuteAddFieldCommand);
            AppendFromDiskCommand = new DelegateCommand(ExecuteMergeFileCommand, CanExecuteAppendFromDiskCommand);
            SaveFileCommand = new DelegateCommand(ExecuteSaveFileCommand, CanExecuteSaveFileCommand);
            AppendToFileCommand = new DelegateCommand(ExecuteAppendToFileCommand, CanExecuteAppendToFileCommand);
            AddSubdirectoryCommand = new DelegateCommand(ExecuteAddSubdirectoryCommand, CanExecuteAddSubdirectoryCommand);
        }

        private bool CanExecuteAppendToFileCommand(object o)
        {
            return tiffDocument != null;
        }

        private bool CanExecuteSaveFileCommand(object o)
        {
            return tiffDocument != null;
        }

        private bool CanExecuteOpenInDefaultAppCommand(object o)
        {
            return tiffDocument != null;
        }

        private bool CanExecuteAppendFromDiskCommand(object o)
        {
            return tiffDocument != null;
        }

        public DelegateCommand LoadFileCommand { get; }
        public DelegateCommand OpenInDefaultAppCommand { get; }
        public DelegateCommand DeleteDirectoryCommand { get; }
        public DelegateCommand DeleteFieldCommand { get; }
        public DelegateCommand AddFieldCommand { get; }
        public DelegateCommand AppendFromDiskCommand { get; }
        public DelegateCommand SaveFileCommand { get; }
        public DelegateCommand AppendToFileCommand { get; }
        public DelegateCommand AddSubdirectoryCommand { get; }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TiffDirectoryViewModel> Directories { get; } =
            new ObservableCollection<TiffDirectoryViewModel>();

        public TiffDirectoryViewModel SelectedDirectoryViewModel
        {
            get => selectedDirectoryViewModel;
            set
            {
                selectedDirectoryViewModel = value;
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        public TiffFieldViewModel SelectedFieldViewModel
        {
            get => selectedFieldViewModel;
            set
            {
                selectedFieldViewModel = value;
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        private bool CanExecuteAddFieldCommand(object o)
        {
            return SelectedDirectoryViewModel != null;
        }

        private bool CanExecuteAddSubdirectoryCommand(object o)
        {
            return SelectedDirectoryViewModel != null;
        }

        private bool CanExecuteDeleteDirectoryCommand(object o)
        {
            return SelectedDirectoryViewModel != null;
        }

        private bool CanExecuteDeleteFieldCommand(object o)
        {
            return SelectedDirectoryViewModel != null && SelectedFieldViewModel != null;
        }

        private void ExecuteAddFieldCommand(object o)
        {
            var fieldVm = dialogService.ShowCreateFieldDialog(SelectedDirectoryViewModel);
            if (fieldVm == null) return;
            var values = fieldVm.GetValues().ToList();

            var directory = SelectedDirectoryViewModel.Directory;
            if (directory.HasField(fieldVm.TagNumber))
            {
                dialogService.ShowMessage("The specified field already exists, and cannot be overwritten.", "Error");
                return;
            }

            if (values.Count == 0)
            {
                dialogService.ShowMessage("Cannot add a field with no value(s).", "Error");
                return;
            }

            try
            {
                switch (fieldVm.FieldType)
                {
                    case TiffFieldType.Byte:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(byte))).OfType<byte>().ToList());
                        break;
                    case TiffFieldType.ASCII:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => x.ToString()).ToList());
                        break;
                    case TiffFieldType.Short:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(ushort))).OfType<ushort>().ToList());
                        break;
                    case TiffFieldType.Long:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(uint))).OfType<uint>().ToList());
                        break;
                    case TiffFieldType.Rational:
                    case TiffFieldType.SRational:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(decimal))).OfType<decimal>().ToList());
                        break;
                    case TiffFieldType.SByte:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(sbyte))).OfType<sbyte>().ToList());
                        break;
                    case TiffFieldType.SShort:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(short))).OfType<short>().ToList());
                        break;
                    case TiffFieldType.SLong:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(int))).OfType<int>().ToList());
                        break;
                    case TiffFieldType.Float:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(float))).OfType<float>().ToList());
                        break;
                    case TiffFieldType.Double:
                        directory.AddField(fieldVm.TagNumber, values.Select(x => Convert.ChangeType(x, typeof(double))).OfType<double>().ToList());
                        break;
                    case TiffFieldType.IFD:
                        dialogService.ShowMessage("Adding fields of type IFD directly is not supported.", "Error");
                        break;
                    case TiffFieldType.Undefined:
                        dialogService.ShowMessage("Adding fields of type Undefined is not supported.", "Error");
                        break;
                    case TiffFieldType.Unknown:
                        dialogService.ShowMessage("Adding fields of type Unknown is not supported.", "Error");
                        break;
                }
            }
            catch (Exception)
            {
                dialogService.ShowMessage("The field could not be added because one or more values could not be converted to the specified data type.", "Error");
                return;
            }
            LoadDocument(tiffDocument);
        }

        private void ExecuteAddSubdirectoryCommand(object o)
        {
            try
            {
                var fileName = dialogService.GetOpenFile();
                if (string.IsNullOrWhiteSpace(fileName)) return;

                var selectedDirectory = SelectedDirectoryViewModel.Directory;
                var doc = new TiffDocument(fileName);
                selectedDirectory.AddSubdirectory(doc.Directories.First());
                LoadDocument(tiffDocument);
            }
            catch (TiffException e)
            {
                dialogService.ShowMessage(e.Message, "Error");
            }
        }

        private void ExecuteAppendToFileCommand(object o)
        {
            try
            {
                var fileName = dialogService.GetOpenFile();
                if (string.IsNullOrWhiteSpace(fileName)) return;
                tiffDocument.WriteAppend(fileName);
                var result = dialogService.ShowYesNo(
                    "Document successfully appended to the selected file. Would you like to open the file in the default app?",
                    "Success");
                if (result)
                    Process.Start(fileName);
            }
            catch (TiffException e)
            {
                dialogService.ShowMessage(e.Message, "Error");
            }
        }

        private void ExecuteDeleteDirectoryCommand(object o)
        {
            if (SelectedDirectoryViewModel == null) return;
            if (Directories.Count == 1)
            {
                dialogService.ShowMessage("Cannot delete the only directory in the TIFF!", "Error");
                return;
            }
            if (!dialogService.ShowYesNo("Are you sure you want to delete this directory?",
                "Delete Directory")) return;
            var toDelete = SelectedDirectoryViewModel.Directory;
            tiffDocument.RemoveDirectory(toDelete);
            LoadDocument(tiffDocument);
        }

        private void ExecuteDeleteFieldCommand(object o)
        {
            var tiffDirectory = SelectedDirectoryViewModel.Directory;
            tiffDirectory.RemoveField(SelectedFieldViewModel.TagNum);
            LoadDocument(tiffDocument);
        }

        private void ExecuteLoadFileCommand(object o)
        {
            var fileName = dialogService.GetOpenFile();
            LoadFile(fileName);
        }

        private void ExecuteMergeFileCommand(object o)
        {
            var fileName = dialogService.GetOpenFile();
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var doc = new TiffDocument(fileName);
            tiffDocument.Append(doc);
            LoadDocument(tiffDocument);
        }

        private void ExecuteOpenInDefaultAppCommand(object o)
        {
            try
            {
                var tempFile = Path.GetTempFileName();
                File.Delete(tempFile);
                tiffDocument.Write(tempFile + ".tif");
                Process.Start(tempFile + ".tif");
            }
            catch (Exception)
            {
                dialogService.ShowMessage("The file could not be launched in the default application.", "Error");
            }
        }

        private void ExecuteSaveFileCommand(object o)
        {
            var fileName = dialogService.GetSaveFile();
            if (string.IsNullOrWhiteSpace(fileName)) return;
            try
            {
                tiffDocument.Write(fileName);
                var result = dialogService.ShowYesNo(
                    "File saved successfully. Would you like to open the file in the default app?", "Success");
                if (result)
                    Process.Start(fileName);
            }
            catch (Exception)
            {
                dialogService.ShowMessage("Oops! File save failed.", "Error");
            }
        }

        private void LoadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            LoadDocument(new TiffDocument(fileName));
            Title = $"TIFF Structure View - {Path.GetFileName(fileName)}";
            UpdateCommands();
        }

        private void LoadDocument(TiffDocument document)
        {
            tiffDocument = document;
            PopulateDirectories();
            SelectedDirectoryViewModel = Directories.FirstOrDefault();
        }

        private void UpdateCommands()
        {
            OpenInDefaultAppCommand.RaiseCanExecuteChanged();
            DeleteDirectoryCommand.RaiseCanExecuteChanged();
            DeleteFieldCommand.RaiseCanExecuteChanged();
            AddFieldCommand.RaiseCanExecuteChanged();
            AppendFromDiskCommand.RaiseCanExecuteChanged();
            SaveFileCommand.RaiseCanExecuteChanged();
            AppendToFileCommand.RaiseCanExecuteChanged();
            AddSubdirectoryCommand.RaiseCanExecuteChanged();
        }

        private void PopulateDirectories()
        {
            Directories.Clear();
            var directoryNumber = 1;
            foreach (var directory in tiffDocument.Directories)
            {
                Directories.Add(new TiffDirectoryViewModel(directory, directoryNumber++, dialogService));
            }
        }
    }
}