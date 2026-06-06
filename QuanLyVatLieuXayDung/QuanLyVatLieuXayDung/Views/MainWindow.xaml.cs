using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuanLyVatLieuXayDung.Models;
using QuanLyVatLieuXayDung.ViewModels;

namespace QuanLyVatLieuXayDung.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitEvents();
        }

        // 2. CONSTRUCTOR MỚI: Nhận tham số NguoiDung truyền từ màn hình Đăng Nhập
        public MainWindow(NguoiDung taiKhoanDangNhap)
        {
            InitializeComponent();
            InitEvents();

            // Ép DataContext của MainWindow nhận ViewModel có chứa thông tin user vừa đăng nhập
            // Việc này giúp kích hoạt tầng phân quyền (IsAdmin, IsKho...) trong MainWindowViewModel(NguoiDung account)
            this.DataContext = new MainWindowViewModel(taiKhoanDangNhap);
        }

        // Tách hàm xử lý sự kiện dùng chung cho cả 2 Constructor
        private void InitEvents()
        {
            btnDashBroad.Click += BtnDashBroad_Click;

            // Kiểm tra kết nối DB (Code cũ của bạn)
            using (var db = new QuanLyVatLieuXayDungEntities())
            {
                if (db.Database.Exists())
                {
                    System.Windows.MessageBox.Show("KẾT NỐI THÀNH CÔNG! Database đã sẵn sàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("KẾT NỐI THẤT BẠI! Kiểm tra lại SQL Server hoặc App.config.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDashBroad_Click(object sender, RoutedEventArgs e)
        {
            Main.Children.Clear();
            Main.Children.Add(new TrangChuView());
        }
    }
}
