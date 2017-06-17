using System.Windows;
using Ephemera.Tiff.Demo.ViewModel;

namespace Ephemera.Tiff.Demo.View
{
    /// <summary>
    /// Interaction logic for TiffStructureView.xaml
    /// </summary>
    public partial class TiffStructureView : Window
    {
        public TiffStructureView()
        {
            InitializeComponent();
            DataContext = new TiffStructureViewModel(new DialogService());
        }
    }
}
