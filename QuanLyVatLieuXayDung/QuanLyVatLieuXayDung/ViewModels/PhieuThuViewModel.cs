using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class PhieuThuViewModel : BaseViewModel
    {
        private ObservableCollection<PhieuThu> _DSPhieuThu;
        public ObservableCollection<PhieuThu> DSPhieuThu
        {
            get => _DSPhieuThu;
            set { _DSPhieuThu = value; OnPropertyChanged(); }
        }

        private ObservableCollection<KhachHang> _DSKhachHang;
        public ObservableCollection<KhachHang> DSKhachHang
        {
            get => _DSKhachHang;
            set { _DSKhachHang = value; OnPropertyChanged(); }
        }

        private PhieuThu _SelectedPhieuThu;
        public PhieuThu SelectedPhieuThu
        {
            get => _SelectedPhieuThu;
            set
            {
                if (_SelectedPhieuThu != value)
                {
                    _SelectedPhieuThu = value;
                    OnPropertyChanged();
                    if (_SelectedPhieuThu != null)
                    {
                        IDKhachHang = _SelectedPhieuThu.IDKhachHang;
                        NgayThu = _SelectedPhieuThu.NgayThu;
                        SoTien = _SelectedPhieuThu.SoTien;
                        NoiDung = _SelectedPhieuThu.NoiDung;
                    }
                }
            }
        }

        private string _IDKhachHang;
        public string IDKhachHang
        {
            get => _IDKhachHang;
            set { _IDKhachHang = value; OnPropertyChanged(); }
        }

        private DateTime? _NgayThu;
        public DateTime? NgayThu
        {
            get => _NgayThu;
            set { _NgayThu = value; OnPropertyChanged(); }
        }

        private double? _SoTien;
        public double? SoTien
        {
            get => _SoTien;
            set { _SoTien = value; OnPropertyChanged(); }
        }

        private string _NoiDung;
        public string NoiDung
        {
            get => _NoiDung;
            set { _NoiDung = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public PhieuThuViewModel()
        {
            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => SelectedPhieuThu != null, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => SelectedPhieuThu != null, (p) => Remove());
        }

        private void RefreshData()
        {
            DSKhachHang = new ObservableCollection<KhachHang>(DataProvider.Ins.DB.KhachHangs.ToList());
            
            var list = DataProvider.Ins.DB.PhieuThus.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSPhieuThu = new ObservableCollection<PhieuThu>(list);
        }

        private void ClearFields()
        {
            IDKhachHang = null;
            NgayThu = DateTime.Now;
            SoTien = 0;
            NoiDung = string.Empty;
            SelectedPhieuThu = null;
        }

        private string AutoCreateID()
        {
            for (int i = 1; i <= 999; i++)
            {
                string testID = "PT" + i.ToString("D3");
                if (!DataProvider.Ins.DB.PhieuThus.Any(p => p.ID == testID))
                {
                    return testID;
                }
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Phiếu thu (PT999)!");
        }

        private void Add()
        {
            if (string.IsNullOrEmpty(IDKhachHang) || SoTien == null || SoTien <= 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng và nhập số tiền lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentUserId = null;
            if (Application.Current.MainWindow != null && Application.Current.MainWindow.DataContext is MainWindowViewModel mainVM)
            {
                currentUserId = mainVM.CurrentNguoiDung?.ID;
            }

            string nextID = AutoCreateID();
            var pt = new PhieuThu()
            {
                ID = nextID,
                IDKhachHang = IDKhachHang,
                NgayThu = NgayThu ?? DateTime.Now,
                SoTien = SoTien,
                NoiDung = NoiDung,
                IDUser = currentUserId
            };

            DataProvider.Ins.DB.PhieuThus.Add(pt);
            DataProvider.Ins.DB.SaveChanges();

            RefreshData();
            ClearFields();
            MessageBox.Show("Thêm phiếu thu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Update()
        {
            if (SelectedPhieuThu == null) return;
            if (string.IsNullOrEmpty(IDKhachHang) || SoTien == null || SoTien <= 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng và nhập số tiền lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pt = DataProvider.Ins.DB.PhieuThus.SingleOrDefault(p => p.ID == SelectedPhieuThu.ID);
            if (pt != null)
            {
                pt.IDKhachHang = IDKhachHang;
                pt.NgayThu = NgayThu;
                pt.SoTien = SoTien;
                pt.NoiDung = NoiDung;

                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                ClearFields();
                MessageBox.Show("Cập nhật phiếu thu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Remove()
        {
            if (SelectedPhieuThu == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu thu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataProvider.Ins.DB.PhieuThus.Remove(SelectedPhieuThu);
                DataProvider.Ins.DB.SaveChanges();

                RefreshData();
                ClearFields();
                MessageBox.Show("Xóa phiếu thu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
