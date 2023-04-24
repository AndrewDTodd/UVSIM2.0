using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UVSim.ViewModel.Converters
{
    public class CollectionArrayToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*
            ConcurrentBag<byte[]> bytesCollection = new(value as Collection<byte[]>);
            ConcurrentBag<Int64> wordList = new();
            */

            Collection<byte[]> bytesCollection = value as Collection<byte[]>;
            List<Int64> wordList = new(bytesCollection.Count);
            /*
            try
            {
                Parallel.ForEach(bytesCollection.AsParallel().AsOrdered(), (bytes) =>
                {
                    Int64 word = (int)parameter switch
                    {
                        1 => bytes[0],
                        2 => BitConverter.ToInt16(bytes, 0),
                        <= 4 => BitConverter.ToInt32(bytes, 0),
                        <= 8 => BitConverter.ToInt64(bytes, 0),
                        _ => 0
                    };

                    wordList.Add(word);
                });
            }
            catch
            {

            }
            */
           
            foreach (byte[] bytes in bytesCollection) 
            {
                Int64 word = (int)parameter switch
                {
                    1 => bytes[0],
                    2 => BitConverter.ToInt16(bytes, 0),
                    <= 4 => BitConverter.ToInt32(bytes, 0),
                    <= 8 => BitConverter.ToInt64(bytes, 0),
                    _ => 0
                };

                wordList.Add(word);
            }
            

            return wordList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*
            return (int)parameter switch
            {
                1 => BitConverter.GetBytes((Int64)value)[0],
                2 => BitConverter.GetBytes((Int64)value)[..2],
                <= 4 => BitConverter.GetBytes((Int64)value)[..4],
                <= 8 => BitConverter.GetBytes((Int64)value),
                _ => BitConverter.GetBytes((Int64)value),
            };
            */
            return null;
        }
    }
}
