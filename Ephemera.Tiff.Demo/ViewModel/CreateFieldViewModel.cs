using System;
using System.Collections.Generic;

namespace Ephemera.Tiff.Demo.ViewModel
{
    internal sealed class CreateFieldViewModel : ViewModelBase
    {
        private ushort tagNumber = 65500;
        private TiffFieldType fieldType;
        private string values;

        public ushort TagNumber
        {
            get => tagNumber;
            set
            {
                tagNumber = value;
                OnPropertyChanged();
            }
        }

        public TiffFieldType FieldType
        {
            get => fieldType;
            set
            {
                fieldType = value;
                OnPropertyChanged();
            }
        }

        public string Values
        {
            get => values;
            set
            {
                values = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<object> GetValues()
        {
            return Values.Split(System.Environment.NewLine.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}