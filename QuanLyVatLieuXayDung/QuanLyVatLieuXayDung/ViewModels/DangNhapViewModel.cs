using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;
using QuanLyVatLieuXayDung.Views;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class DangNhapViewModel:BaseViewModel
    {

        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public ICommand LoginCommand { get; set; }

        public DangNhapViewModel()
        {

            LoginCommand = new RelayCommand<Window>(
                (p) => {
                    return true;
                },
                (p) => {
                    Login(p );
                }
            );
        }

        

        private void Login(Window loginWindow)
        {
            // 1. Kiểm tra trống dữ liệu ô nhập vào trước
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("Vui lòng nhập Tên đăng nhập!", "Nhắc nhở", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu!", "Nhắc nhở", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 2. ÉP BUỘC QUÉT TRỰC TIẾP TỪ DATABASE KHI BẤM NÚT (Không dùng danh sách quét từ trước nữa)
                // Việc này đảm bảo kết nối luôn luôn mới và chính xác nhất
                var taiKhoan = DataProvider.Ins.DB.NguoiDungs.FirstOrDefault(x => x.UserName == UserName && x.Password == Password);

                if (taiKhoan != null)
                {
                    // Tải danh sách quyền từ DB về để gán cho người dùng
                    var danhSachQuyen = DataProvider.Ins.DB.NguoiDungRoles.ToList();

                    if (danhSachQuyen != null)
                    {
                        taiKhoan.NguoiDungRole = danhSachQuyen.FirstOrDefault(r => r.ID.Trim() == taiKhoan.IDRole.Trim());
                    }

                    MessageBox.Show($"Đăng nhập thành công! Xin chào {taiKhoan.DisplayName}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở màn hình chính và bàn giao dữ liệu
                    MainWindow mainScreen = new MainWindow(taiKhoan);
                    mainScreen.Show();

                    // Đóng màn hình đăng nhập
                    if (loginWindow != null)
                    {
                        loginWindow.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Nếu thực sự chưa kết nối được Database, nó sẽ nhảy vào đây và báo chính xác lỗi gì cho bạn biết
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu khi đăng nhập: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
