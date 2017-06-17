using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class TiffDirectoryViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;

        public TiffDirectoryViewModel(TiffDirectory directory, int number, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            Number = number;
            this.Directory = directory;
            foreach (var field in directory.Fields.Values)
                Fields.Add(new TiffFieldViewModel(field));
        }

        public int Number { get; }

        public ObservableCollection<TiffFieldViewModel> Fields { get; } = new ObservableCollection<TiffFieldViewModel>();

        public TiffDirectory Directory { get; }

        public void RemoveField(TiffFieldViewModel field)
        {
            try
            {
                Directory.RemoveField(field.Tag);
                Fields.Remove(field);
            }
            catch (Exception e)
            {
                dialogService.ShowMessage(e.Message, "Error");
            }
        }

        public void AddField(ushort tagNumber, TiffFieldType type, IList<object> values)
        {
            if (Directory.HasTag(tagNumber))
            {
                dialogService.ShowMessage("The specified field already exists, and cannot be overwritten.", "Error");
                return;
            }

            if (values.Count == 0)
            {
                dialogService.ShowMessage("Cannot add a field with no value(s).", "Error");
                return;
            }

            ITiffField field = null;
            try
            {
                switch (type)
                {
                    case TiffFieldType.Byte:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(byte))).OfType<byte>().ToList());
                        break;
                    case TiffFieldType.ASCII:
                        field = Directory.AddField(tagNumber, values.Select(x => x.ToString()).ToList());
                        break;
                    case TiffFieldType.Short:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(ushort))).OfType<ushort>().ToList());
                        break;
                    case TiffFieldType.Long:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(uint))).OfType<uint>().ToList());
                        break;
                    case TiffFieldType.Rational:
                    case TiffFieldType.SRational:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(decimal))).OfType<decimal>().ToList());
                        break;
                    case TiffFieldType.SByte:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(sbyte))).OfType<sbyte>().ToList());
                        break;
                    case TiffFieldType.SShort:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(short))).OfType<short>().ToList());
                        break;
                    case TiffFieldType.SLong:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(int))).OfType<int>().ToList());
                        break;
                    case TiffFieldType.Float:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(float))).OfType<float>().ToList());
                        break;
                    case TiffFieldType.Double:
                        field = Directory.AddField(tagNumber, values.Select(x => Convert.ChangeType(x, typeof(double))).OfType<double>().ToList());
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
            catch (Exception e)
            {
                dialogService.ShowMessage("The field could not be added because one or more values could not be converted to the specified data type.", "Error");
                return;
            }

            if (field == null) return;

            Fields.Add(new TiffFieldViewModel(field));
        }

        public void AddSubdirectory(TiffDirectory directory)
        {
            Directory.AddSubdirectory(directory);
            var subIfdField = Directory[TiffTag.SubIFDs];
            var subIfdVm = Fields.FirstOrDefault(x => x.Tag == TiffTag.SubIFDs);
            if (subIfdVm != null) Fields.Remove(subIfdVm);
            subIfdVm = new TiffFieldViewModel(subIfdField);
            Fields.Add(subIfdVm);
        }
    }
}