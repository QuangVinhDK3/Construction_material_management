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
        public ICommand QuanLyPhieuNhapCommand { get; set; }
        public ICommand NhapKhoCommand { get; set; }
        public ICommand XuatKhoCommand { get; set; }
        public ICommand QuanLyPhieuXuatCommand { get; set; }
        public ICommand QuanLyPhieuThuCommand { get; set; }
        public ICommand QuanLyPhieuChiCommand { get; set; }
        public ICommand DangXuatCommand { get; set; }
        public ICommand ThongTinCaNhanCommand { get; set; }
        public MainWindowViewModel()
        {
            IsAdmin = IsBanHang = IsKho = IsDoiTac = false ;
            InitCommands();
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
            InitCommands();

        }

        private void InitCommands()
        {
            // Tận dụng các biến IsAdmin, IsKho... để kiểm tra điều kiện bấm nút (CanExecute)
            LoaiVatLieuCommand = new RelayCommand<object>((p) => true, (p) => { var wd = new CRUDLoaiVatLieuView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            NhaCungCapCommand = new RelayCommand<object>((p) => IsDoiTac, (p) => { var wd = new CRUDNhaCungCapView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            KhachHangCommand = new RelayCommand<object>((p) => IsDoiTac, (p) => { var wd = new CRUDKhachHangView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            DoiTuongCommand = new RelayCommand<object>((p) => IsAdmin, (p) => { var wd = new CRUDDoiTuongView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });

            // Chỉ Admin mới được quản lý nhân viên
            NhanVienCommand = new RelayCommand<object>((p) => IsAdmin, (p) => { var wd = new CRUDNhanVienView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });

            // Chỉ kho hoặc Admin mới được nhập
            QuanLyPhieuNhapCommand = new RelayCommand<object>((p) => IsKho, (p) => { var wd = new QuanLyPhieuNhapView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            NhapKhoCommand = new RelayCommand<object>((p) => IsKho, (p) => { var wd = new CRUDPhieuNhapView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });

            // Bán hàng (hoặc Admin) mới được quản lý phiếu xuất và phiếu thu
            QuanLyPhieuXuatCommand = new RelayCommand<object>((p) => IsBanHang, (p) => { var wd = new QuanLyPhieuXuatView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            XuatKhoCommand = new RelayCommand<object>((p) => IsBanHang, (p) => { var wd = new CRUDPhieuXuatView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            QuanLyPhieuThuCommand = new RelayCommand<object>((p) => IsBanHang, (p) => { var wd = new QuanLyPhieuThuView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });
            
            // Kế toán chi trả hoặc Admin
            QuanLyPhieuChiCommand = new RelayCommand<object>((p) => IsKho || IsBanHang, (p) => { var wd = new QuanLyPhieuChiView(); wd.ShowDialog(); TrangChuViewModel.RefreshDashboardData?.Invoke(); });

            DangXuatCommand = new RelayCommand<object>((p) => true, (p) => DangXuat(p as Window));
            // Kích hoạt cập nhật lại giao diện tên người dùng trên Main khi tắt Form hồ sơ
            ThongTinCaNhanCommand = new RelayCommand<object>((p) => true, (p) => { var wd = new ThongTinNguoiDungView(CurrentNguoiDung); wd.ShowDialog(); OnPropertyChanged(nameof(CurrentNguoiDung)); });
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
