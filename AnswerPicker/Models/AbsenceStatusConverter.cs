using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AnswerPicker.Converters
{
    public class AbsenceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAbsent)
            {
                return isAbsent ? "Present" : "Absent";
            }
            return "Absent";  // Domyślnie zakładajmy, że jest "Absent"
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
