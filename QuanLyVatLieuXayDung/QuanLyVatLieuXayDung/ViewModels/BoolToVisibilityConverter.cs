using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace QuanLyVatLieuXayDung.ViewModels
{
    // Converter: chuyển bool → Visibility
    // true  → Visible  (hiện)
    // false → Collapsed (ẩn, không chiếm chỗ)
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            // ✅ C# 5: ép kiểu tường minh, không dùng pattern matching "is bool b"
            if (value is bool)
            {
                bool b = (bool)value;
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
