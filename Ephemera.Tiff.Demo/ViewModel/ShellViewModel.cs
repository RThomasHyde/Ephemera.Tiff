using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Ephemera.Tiff.Demo.Model;
using Microsoft.Win32;
using Stylet;
using Ephemera.Tiff;

namespace Ephemera.Tiff.Demo.ViewModel
{
    public class ShellViewModel : PropertyChangedBase, IViewAware
    {
        private string title = "TIFF Explorer";

        public ObservableCollection<TiffPage> Pages { get; } = new ObservableCollection<TiffPage>();

        public string Title
        {
            get => title;
            set => SetAndNotify(ref title, value);
        }

        public void OpenFile()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "TIFF Files (*.tif)|*.tif",
                Multiselect = false,
                RestoreDirectory = true,
                CheckFileExists = true
            };
            var ok = ofd.ShowDialog(View as Window);
            if (ok.HasValue && ok.Value)
            {
                LoadFile(ofd.FileName);
            }
        }

        public void DropFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadFile(files[0]);
            }
        }

        public void AttachView(UIElement view)
        {
            View = view;
        }

        public UIElement View { get; private set; }

        private void LoadFile(string fileName)
        {
            Pages.Clear();
            var tiff = new TiffDocument(fileName);
            Title = $"TIFF Explorer | {Path.GetFileName(fileName)}";
            int i = 1;
            foreach (var dir in tiff.Directories)
            {
                var page = new TiffPage { Name = $"Page {i++}" };
                foreach (var field in dir.Fields)
                {
                    var tag = new Tag
                    {
                        Name = field.Value.Tag.ToString(),
                        TagNum = field.Key,
                        Offset = field.Value.Offset,
                        Value = GetTagValue(field.Value)
                    };
                    page.Children.Add(tag);
                }
                Pages.Add(page);
            }
        }

        private string GetTagValue(ITiffField field)
        {
            var values = field.GetValues<string>();
            return string.Join(",", values);
        }
    }
}