﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace KidsSignIn.Converters
{
    class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) return !(bool)value;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) return !(bool)value;
            return value;
        }
    }
}
