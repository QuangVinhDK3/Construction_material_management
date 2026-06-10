using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyVatLieuXayDung.ViewModels
{
    /// <summary>
    /// Converter: chuyển chuỗi TrangThai sang màu nền tương ứng để hiển thị badge màu.
    /// "Còn hàng"  → Xanh lá (#388E3C)
    /// "Sắp hết"   → Cam (#F57C00)
    /// "Hết hàng"  → Đỏ (#D32F2F)
    /// </summary>
    public class TrangThaiColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string trangThai)
            {
                switch (trangThai)
                {
                    case "Còn hàng":
                        return new SolidColorBrush(Color.FromRgb(0x38, 0x8E, 0x3C));
                    case "Sắp hết":
                        return new SolidColorBrush(Color.FromRgb(0xF5, 0x7C, 0x00));
                    case "Hết hàng":
                        return new SolidColorBrush(Color.FromRgb(0xD3, 0x2F, 0x2F));
                    default:
                        return new SolidColorBrush(Color.FromRgb(0x78, 0x90, 0x9C));
                }
            }
            return new SolidColorBrush(Color.FromRgb(0x78, 0x90, 0x9C));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
