using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Ephemera.Tiff.Demo.View
{
    /// <summary>
    /// Interaction logic for CreateFieldView.xaml
    /// </summary>
    public partial class CreateFieldView : Window
    {
        public CreateFieldView()
        {
            InitializeComponent();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsAllowed(e.Text);
        }

        private bool IsAllowed(string value)
        {
            Regex regex = new Regex("[0-9]*");
            return regex.IsMatch(value);
        }

        private void OnAcceptButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
