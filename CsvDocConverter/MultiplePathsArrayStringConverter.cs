using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CsvDocConverter
{
    public class MultiplePathsArrayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) { return ""; }
            List<string> paths = (List<string>)value;
            return "\"" + string.Join("\" \"", paths.Select(p => p.Trim('"'))) + "\"";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paths = (string)value;
            if (string.IsNullOrEmpty(paths)) { return new List<string>(); }
            string delimiter = " ";
            return Regex.Split(paths, delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(p => p.Trim('"')).ToList();    //split paths (using quotes to allow the delimiter)
        }
    }
}
