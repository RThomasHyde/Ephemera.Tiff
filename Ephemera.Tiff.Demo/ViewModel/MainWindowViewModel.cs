using System.Windows.Input;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class MainWindowViewModel
    {
        private readonly IDemoDialogService demoDialogService;

        public MainWindowViewModel(IDemoDialogService demoDialogService)
        {
            this.demoDialogService = demoDialogService;
            ViewTagsDemoCommand = new DelegateCommand(ExecuteViewTagsDemoCommand);
            AppendInMemoryDemoCommand = new DelegateCommand(ExecuteAppendInMemoryDemoCommand);
            AppendToFileDemoCommand = new DelegateCommand(ExecuteAppendToFileDemoCommand);
            MergeAllInFolderDemoCommand = new DelegateCommand(ExecuteMergeAllInFolderDemoCommand);
        }

        public ICommand ViewTagsDemoCommand { get; }
        public ICommand AppendInMemoryDemoCommand { get; }
        public ICommand AppendToFileDemoCommand { get; }
        public ICommand MergeAllInFolderDemoCommand { get; }

        private void ExecuteViewTagsDemoCommand(object o)
        {
            demoDialogService.ShowViewStructureDemo();
        }

        private void ExecuteAppendInMemoryDemoCommand(object o)
        {
            demoDialogService.ShowAppendInMemoryDemo();
        }

        private void ExecuteAppendToFileDemoCommand(object o)
        {
            demoDialogService.ShowAppendToFileDemo();
        }

        private void ExecuteMergeAllInFolderDemoCommand(object o)
        {
            demoDialogService.ShowMergeFolderDemo();
        }
    }
}