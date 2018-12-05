using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;

namespace manageclientwpf
{
    public class MemberlistConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            ObservableCollection<ExtDevice> date = (ObservableCollection<ExtDevice>)value;
            return date.Count;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class BoolToIsCheckedConverter : IValueConverter
    {

        public object Convert(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            return (bool)value;
        }
 

        public object ConvertBack(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            bool? isChecked = (bool?)value;
            if (isChecked == null)
            {
                return false;
            }
            else
            {
                return isChecked == true;
            }
        }

    }

}
