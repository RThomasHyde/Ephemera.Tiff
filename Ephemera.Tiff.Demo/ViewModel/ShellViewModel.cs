using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ephemera.Tiff.Demo.Model;
using Microsoft.Win32;
using Stylet;

namespace Ephemera.Tiff.Demo.ViewModel
{
    public class ShellViewModel : PropertyChangedBase, IViewAware
    {
        private string title = "TIFF Explorer";
        private bool canSaveFile;
        private bool isOptionsFlyoutOpen;
        private TiffDocument tiff;
        private TiffOptions options = TiffOptions.None;

        public ObservableCollection<TiffPage> Pages { get; } = new ObservableCollection<TiffPage>();

        public string Title
        {
            get => title;
            set => SetAndNotify(ref title, value);
        }

        public bool CanSaveFile
        {
            get => canSaveFile;
            set => SetAndNotify(ref canSaveFile, value);
        }

        public bool IsOptionsFlyoutOpen
        {
            get => isOptionsFlyoutOpen;
            set => SetAndNotify(ref isOptionsFlyoutOpen, value);
        }

        public async Task OpenFile()
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
                await Task.Run(() => LoadFile(ofd.FileName));
            }
        }

        public void SaveFile()
        {
            if (tiff == null) return;
            var sfd = new SaveFileDialog
            {
                Filter = "TIFF Files (*.tif)|*.tif",
                AddExtension = true,
                DefaultExt = ".tif",
                RestoreDirectory = true,
                OverwritePrompt = true
            };
            var ok = sfd.ShowDialog(View as Window);
            if (ok.HasValue && ok.Value)
            {
                tiff.Write(sfd.FileName, options);
            }
        }

        public void OpenOptionsFlyout()
        {
            IsOptionsFlyoutOpen = true;
        }

        public void EnableStripTags()
        {
            options |= TiffOptions.StripUnknownTags;
        }

        public void DisableStripTags()
        {
            options &= ~TiffOptions.StripUnknownTags;
        }

        public void EnableConvertOjpeg()
        {
            options |= TiffOptions.ConvertOJPEGToJPEG;
        }

        public void DisableConvertOjpeg()
        {
            options &= ~TiffOptions.ConvertOJPEGToJPEG;
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
            Pages.ClearOnUI();
            var fileSize = new FileInfo(fileName).Length;
            tiff = new TiffDocument(fileName);
            Title = $"TIFF Explorer | {Path.GetFileName(fileName)} ({fileSize})";
            CanSaveFile = true;
            int i = 1;
            foreach (var dir in tiff.Directories)
            {
                var page = new TiffPage { Name = $"Page {i++}" };
                foreach (var field in dir.Fields)
                {
                    var tag = new Tag
                    {
                        Name = $"{field.Key} | {field.Value.Tag}",
                        Type = field.Value.Type.ToString(),
                        Count = field.Value.Count,
                        Value = GetTagValue(field.Value),
                        Offset = GetTagOffset(field.Value)
                    };
                    page.Children.Add(tag);
                }
                Pages.AddOnUI(page);
            }
        }

        private string GetTagOffset(ITiffField field)
        {
            switch (field.Type)
            {
                case TiffFieldType.Rational:
                case TiffFieldType.SRational:
                    return "";
                default:
                    var values = field.GetValues<string>().ToArray();
                    if (values.Length == 1 && field.Offset.ToString() == values[0])
                        return "";
                    return field.Offset.ToString();
            }
        }

        private string GetTagValue(ITiffField field)
        {
            switch (field.Tag)
            {
                case TiffTag.Compression:
                    return ((CompressionType) field.GetValue<ushort>()).ToString();
                case TiffTag.XResolution:
                case TiffTag.YResolution:
                    return field.GetValue<double>().ToString("F2");
                case TiffTag.FillOrder:
                    return ((FillOrder) field.GetValue<ushort>()).ToString();
                case TiffTag.PhotometricInterpretation:
                    return ((PhotometricInterpretation) field.GetValue<ushort>()).ToString();
                case TiffTag.Thresholding:
                    return ((Threshholding) field.GetValue<ushort>()).ToString();
                case TiffTag.ResolutionUnit:
                    return ((ResolutionUnit) field.GetValue<ushort>()).ToString();
                case TiffTag.Orientation:
                    return ((Orientation) field.GetValue<ushort>()).ToString();
                case TiffTag.PlanarConfiguration:
                    return ((PlanarConfiguration) field.GetValue<ushort>()).ToString();
                case TiffTag.NewSubfileType:
                    return ((NewSubfileType) field.GetValue<uint>()).ToString();
                default:
                    switch (field.Type)
                    {
                        case TiffFieldType.Byte:
                            var bytes = field.GetValues<byte>().ToArray();
                            return ByteArrayToString(bytes);
                        case TiffFieldType.SByte:
                            var sbytes = field.GetValues<sbyte>().ToArray();
                            return SByteArrayToString(sbytes);
                        case TiffFieldType.Rational:
                        case TiffFieldType.SRational:
                        case TiffFieldType.Float:
                        case TiffFieldType.Double:
                            var doubles = field.GetValues<double>();
                            return DoublesToString(doubles);
                        case TiffFieldType.IFD:
                            return "<subfile>";
                        default:
                            var values = field.GetValues<string>();
                            return string.Join(",", values);
                    }
            }
        }

        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private string SByteArrayToString(sbyte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (sbyte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private string DoublesToString(IEnumerable<double> doubles)
        {
            var builder = new StringBuilder();
            foreach (var dbl in doubles)
            {
                builder.Append($"{dbl:F2},");
            }
            builder.Length -= 1; //strip trailing comma
            return builder.ToString();
        }
    }
}