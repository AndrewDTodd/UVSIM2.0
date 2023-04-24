using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim.ViewModel.Converters
{
    public class ByteArrayToWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)parameter switch
            {
                1 => (value as byte[])[0],
                2 => BitConverter.ToInt16(value as byte[]),
                <= 4 => BitConverter.ToInt32(value as byte[]),
                <= 8 => BitConverter.ToInt64(value as byte[]),
                _ => BitConverter.ToInt16(value as byte[]),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)parameter switch
            {
                1 => (byte)value,
                2 => BitConverter.GetBytes((Int16)value),
                <= 4 => BitConverter.GetBytes((Int32)value),
                <= 8 => BitConverter.GetBytes((Int64)value),
                _ => BitConverter.GetBytes((Int64)value),
            };
        }
    }
}
