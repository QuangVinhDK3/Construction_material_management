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
        public MainWindowViewModel()
        {
            LoaiVatLieuCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDLoaiVatLieuView wd = new CRUDLoaiVatLieuView(); wd.ShowDialog(); });
            NhaCungCapCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDNhaCungCapView wd = new CRUDNhaCungCapView(); wd.ShowDialog(); });
            KhachHangCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDKhachHangView wd = new CRUDKhachHangView(); wd.ShowDialog(); });
            DoiTuongCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDDoiTuongView wd = new CRUDDoiTuongView(); wd.ShowDialog(); });
            NhanVienCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDNhanVienView wd = new CRUDNhanVienView(); wd.ShowDialog(); });
            NhapKhoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDPhieuNhapView wd = new CRUDPhieuNhapView(); wd.ShowDialog(); });
            XuatKhoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CRUDPhieuXuatView wd = new CRUDPhieuXuatView(); wd.ShowDialog(); });
            
        }
    }
}
