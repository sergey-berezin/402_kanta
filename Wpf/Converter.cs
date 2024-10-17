using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wpf
{
    public class SelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selection)
            {
                return selection.ToString();
            }
            return Binding.DoNothing;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number;
            bool isValid = int.TryParse((string)value, out number);

            return isValid ? number : Binding.DoNothing;
        }
    }

    public class MutationRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double mutationrate)
            {
                return mutationrate.ToString();
            }
            return Binding.DoNothing;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double number;
            bool isValid = double.TryParse((string)value, out number);

            return isValid ? number : Binding.DoNothing;
        }
    }

    public class PopulationCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int populationcount)
            {
                return populationcount.ToString();
            }
            return Binding.DoNothing;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number;
            bool isValid = int.TryParse((string)value, out number);

            return isValid ? number : Binding.DoNothing;
        }
    }
    public class CitiesCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int citiescount)
            {
                return citiescount.ToString();
            }
            return Binding.DoNothing;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number;
            bool isValid = int.TryParse((string)value, out number);

            return isValid ? number : Binding.DoNothing;
        }
    }
}
