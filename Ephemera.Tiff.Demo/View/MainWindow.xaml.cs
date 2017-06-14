using System.Windows;
using Ephemera.Tiff.Demo.ViewModel;

namespace Ephemera.Tiff.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new DemoDialogService());
        }
    }
}
