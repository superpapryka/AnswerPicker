using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AnswerPicker.Converters
{
    public class AbsenceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Upewnijmy się, że wartość nie jest null
            if (value is bool isAbsent)
            {
                return isAbsent ? "Present" : "Absent";  // Jeśli student nieobecny, to napis na przycisku będzie "Absent"
            }
            return "Absent";  // Domyślnie zakładajmy, że jest "Absent"
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Nie musimy nic robić w ConvertBack, więc zwróć null
            return null;
        }
    }
}