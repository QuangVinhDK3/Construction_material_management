using QuanLyVatLieuXayDung.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;
using System.Windows;
using System.Collections.ObjectModel;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        

        public bool IsLoaded = false;
        public ICommand LoaiVatLieuCommand { get; set; }
        public ICommand NhaCungCapCommand { get; set; }
        public ICommand KhachHangCommand { get; set; }
        public ICommand DoiTuongCommand { get; set; }
        public ICommand NhanVienCommand { get; set; }
        public ICommand NhapKhoCommand { get; set; }
        public ICommand XuatKhoCommand { get; set; }
        public ICommand DangXuatCommand { get; set; }
        public MainWindowViewModel()
        {
            LoaiVatLieuCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDLoaiVatLieuView wd = new CRUDLoaiVatLieuView(); wd.ShowDialog(); });
            NhaCungCapCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDNhaCungCapView wd = new CRUDNhaCungCapView(); wd.ShowDialog(); });
            KhachHangCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDKhachHangView wd = new CRUDKhachHangView(); wd.ShowDialog(); });
            DoiTuongCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDDoiTuongView wd = new CRUDDoiTuongView(); wd.ShowDialog(); });
            NhanVienCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDNhanVienView wd = new CRUDNhanVienView(); wd.ShowDialog(); });
            NhapKhoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDPhieuNhapView wd = new CRUDPhieuNhapView(); wd.ShowDialog(); });
            XuatKhoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDPhieuXuatView wd = new CRUDPhieuXuatView(); wd.ShowDialog(); });
            DangXuatCommand = new RelayCommand<object>((p) => { return true; }, (p) => { DangXuat(p as Window); });
            IsAdmin = IsBanHang = IsKho = IsDoiTac = true;

        }
        // Thuộc tính lưu trữ thông tin người dùng đang đăng nhập
        private NguoiDung _currentNguoiDung;
        public NguoiDung CurrentNguoiDung
        {
            get => _currentNguoiDung;
            set { _currentNguoiDung = value; OnPropertyChanged(nameof(CurrentNguoiDung)); }
        }

        // Các thuộc tính phân quyền (Dùng để Binding ẩn/hiện sang XAML)
        private bool _isAdmin;
        public bool IsAdmin { get => _isAdmin; set { _isAdmin = value; OnPropertyChanged(nameof(IsAdmin)); } }

        private bool _isBanHang;
        public bool IsBanHang { get => _isBanHang; set { _isBanHang = value; OnPropertyChanged(nameof(IsBanHang)); } }

        private bool _isKho;
        public bool IsKho { get => _isKho; set { _isKho = value; OnPropertyChanged(nameof(IsKho)); } }

        private bool _isDoiTac;
        public bool IsDoiTac { get => _isDoiTac; set { _isDoiTac = value; OnPropertyChanged(nameof(IsDoiTac)); } }

        public MainWindowViewModel(NguoiDung account)
        {
            if (account != null)
            {
                // 1. Lưu lại thông tin người dùng để hiển thị Tên/Avatar lên góc màn hình
                CurrentNguoiDung = account;

                // 2. Xử lý logic phân quyền dựa trên IDRole từ Database
                string role = account.IDRole.Trim().ToUpper();

                IsAdmin = (role == "AM");
                IsBanHang = (role == "BH" || role == "AM");
                IsKho = (role == "TK" || role == "AM");
                IsDoiTac = (role == "BH" || role == "AM");
            }

            LoaiVatLieuCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDLoaiVatLieuView wd = new CRUDLoaiVatLieuView(); wd.ShowDialog(); });
            NhaCungCapCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDNhaCungCapView wd = new CRUDNhaCungCapView(); wd.ShowDialog(); });
            KhachHangCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDKhachHangView wd = new CRUDKhachHangView(); wd.ShowDialog(); });
            DoiTuongCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDDoiTuongView wd = new CRUDDoiTuongView(); wd.ShowDialog(); });
            NhanVienCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDNhanVienView wd = new CRUDNhanVienView(); wd.ShowDialog(); });
            NhapKhoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDPhieuNhapView wd = new CRUDPhieuNhapView(); wd.ShowDialog(); });
            XuatKhoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDPhieuXuatView wd = new CRUDPhieuXuatView(); wd.ShowDialog(); });
            DangXuatCommand = new RelayCommand<object>((p) => { return true; }, (p) => { DangXuat(p as Window); });
        }

        private void DangXuat(Window mainWindow)
        {
            // 1. Khởi chạy và hiển thị màn hình đăng nhập mới
            DangNhapView login = new DangNhapView();
            login.Show();

            // 2. Tiến hành đóng màn hình chính
            if (mainWindow != null)
            {
                mainWindow.Close();
            }
            else
            {
                // Phương án dự phòng: Nếu ép kiểu Window bị null, quét toàn bộ hệ thống để tìm MainWindow và đóng
                Window mainWin = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWin != null)
                {
                    mainWin.Close();
                }
            }
        }
    }


}
