using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;
using QuanLyVatLieuXayDung.Views; // For opening CRUDPhieuXuatView

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class QuanLyPhieuXuatViewModel : BaseViewModel
    {
        #region Collections
        private ObservableCollection<PhieuXuat> _DSPhieuXuat;
        public ObservableCollection<PhieuXuat> DSPhieuXuat
        {
            get => _DSPhieuXuat;
            set { _DSPhieuXuat = value; OnPropertyChanged(nameof(DSPhieuXuat)); }
        }

        private ObservableCollection<KhachHang> _DSKhachHang;
        public ObservableCollection<KhachHang> DSKhachHang
        {
            get => _DSKhachHang;
            set { _DSKhachHang = value; OnPropertyChanged(nameof(DSKhachHang)); }
        }

        private ObservableCollection<NguoiDung> _DSNguoiDung;
        public ObservableCollection<NguoiDung> DSNguoiDung
        {
            get => _DSNguoiDung;
            set { _DSNguoiDung = value; OnPropertyChanged(nameof(DSNguoiDung)); }
        }
        #endregion

        #region Selected Item
        private PhieuXuat _SelectedPX;
        public PhieuXuat SelectedPX
        {
            get => _SelectedPX;
            set
            {
                if (_SelectedPX != value)
                {
                    _SelectedPX = value;
                    OnPropertyChanged(nameof(SelectedPX));
                    if (_SelectedPX != null)
                    {
                        IDPhieuXuat = _SelectedPX.ID;
                        DateOutput = _SelectedPX.DateOutput;
                        IDKhachHang = _SelectedPX.IDCustomer;
                        SelectedKH = _SelectedPX.KhachHang;
                        Status = _SelectedPX.Status;
                        
                        IDUser = _SelectedPX.IDUser;
                        SelectedND = _SelectedPX.NguoiDung;
                        SoTienDaThanhToan = _SelectedPX.SoTienDaThanhToan;
                        PhuongThucThanhToan = _SelectedPX.PhuongThucThanhToan;
                        ChietKhau = _SelectedPX.ChietKhau;
                    }
                }
            }
        }

        private KhachHang _SelectedKH;
        public KhachHang SelectedKH
        {
            get => _SelectedKH;
            set { _SelectedKH = value; OnPropertyChanged(nameof(SelectedKH)); }
        }

        private NguoiDung _SelectedND;
        public NguoiDung SelectedND
        {
            get => _SelectedND;
            set { _SelectedND = value; OnPropertyChanged(nameof(SelectedND)); }
        }
        #endregion

        #region Properties
        private string _IDPhieuXuat;
        public string IDPhieuXuat
        {
            get => _IDPhieuXuat;
            set { _IDPhieuXuat = value; OnPropertyChanged(nameof(IDPhieuXuat)); }
        }

        private string _IDKhachHang;
        public string IDKhachHang
        {
            get => _IDKhachHang;
            set { _IDKhachHang = value; OnPropertyChanged(nameof(IDKhachHang)); }
        }

        private DateTime? _DateOutput;
        public DateTime? DateOutput
        {
            get => _DateOutput;
            set { _DateOutput = value; OnPropertyChanged(nameof(DateOutput)); }
        }
        
        private string _Status;
        public string Status
        {
            get => _Status;
            set { _Status = value; OnPropertyChanged(nameof(Status)); }
        }

        private string _IDUser;
        public string IDUser
        {
            get => _IDUser;
            set { _IDUser = value; OnPropertyChanged(nameof(IDUser)); }
        }

        private double? _SoTienDaThanhToan;
        public double? SoTienDaThanhToan
        {
            get => _SoTienDaThanhToan;
            set { _SoTienDaThanhToan = value; OnPropertyChanged(nameof(SoTienDaThanhToan)); }
        }

        private string _PhuongThucThanhToan;
        public string PhuongThucThanhToan
        {
            get => _PhuongThucThanhToan;
            set { _PhuongThucThanhToan = value; OnPropertyChanged(nameof(PhuongThucThanhToan)); }
        }

        private double? _ChietKhau;
        public double? ChietKhau
        {
            get => _ChietKhau;
            set { _ChietKhau = value; OnPropertyChanged(nameof(ChietKhau)); }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand OpenDetailsCommand { get; set; }
        #endregion

        public QuanLyPhieuXuatViewModel()
        {
            SoTienDaThanhToan = 0;
            DSKhachHang = new ObservableCollection<KhachHang>(DataProvider.Ins.DB.KhachHangs.ToList());
            DSNguoiDung = new ObservableCollection<NguoiDung>(DataProvider.Ins.DB.NguoiDungs.ToList());
            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => SelectedPX != null, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => SelectedPX != null, (p) => Remove());

            OpenDetailsCommand = new RelayCommand<object>((p) => true, (p) => OpenDetails());
        }

        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.PhieuXuats
                .Include(p => p.ChiTietPhieuXuats)
                .Include(p => p.KhachHang)
                .Include(p => p.NguoiDung)
                .AsNoTracking()
                .ToList();
            DSPhieuXuat = new ObservableCollection<PhieuXuat>(list);
        }

        private void ClearFields()
        {
            IDPhieuXuat = string.Empty;
            SelectedKH = null;
            IDKhachHang = null;
            DateOutput = null;
            Status = string.Empty;
            IDUser = null;
            SelectedND = null;
            SoTienDaThanhToan = 0;
            PhuongThucThanhToan = string.Empty;
            SelectedPX = null;
        }

        public void Add()
        {
            if (string.IsNullOrEmpty(IDPhieuXuat))
            {
                MessageBox.Show("Vui lòng nhập Mã phiếu xuất!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var isExist = DataProvider.Ins.DB.PhieuXuats.Any(p => p.ID == IDPhieuXuat);
            if (isExist)
            {
                MessageBox.Show("Mã phiếu xuất đã tồn tại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var px = new PhieuXuat()
                {
                    ID = IDPhieuXuat,
                    IDCustomer = IDKhachHang,
                    DateOutput = DateOutput ?? DateTime.Now,
                    Status = Status,
                    IDUser = SelectedND?.ID,
                    SoTienDaThanhToan = SoTienDaThanhToan,
                    PhuongThucThanhToan = PhuongThucThanhToan,
                    ChietKhau = ChietKhau
                };

                DataProvider.Ins.DB.PhieuXuats.Add(px);
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Update()
        {
            if (SelectedPX == null) return;

            try
            {
                var px = DataProvider.Ins.DB.PhieuXuats.SingleOrDefault(p => p.ID == SelectedPX.ID);
                if (px != null)
                {
                    px.IDCustomer = IDKhachHang;
                    px.DateOutput = DateOutput;
                    px.Status = Status;
                    px.IDUser = SelectedND?.ID;
                    px.SoTienDaThanhToan = SoTienDaThanhToan;
                    px.PhuongThucThanhToan = PhuongThucThanhToan;
                    px.ChietKhau = ChietKhau;

                    DataProvider.Ins.DB.SaveChanges();

                    MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Remove()
        {
            if (SelectedPX == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu xuất {SelectedPX.ID}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var px = DataProvider.Ins.DB.PhieuXuats.SingleOrDefault(p => p.ID == SelectedPX.ID);
                    if (px != null)
                    {
                        DataProvider.Ins.DB.PhieuXuats.Remove(px);
                        DataProvider.Ins.DB.SaveChanges();

                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearFields();
                        RefreshData();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa phiếu xuất này vì đã có chi tiết vật liệu bên trong!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OpenDetails()
        {
            if (SelectedPX != null)
            {
                var wd = new CRUDPhieuXuatView();
                
                if (wd.DataContext is PhieuXuatViewModel vm)
                {
                    vm.IDOutput = SelectedPX.ID;
                    // Find it in DS to trigger SelectedCTPX changes maybe, but setting IDOutput is key.
                }

                wd.ShowDialog();
            }
        }
    }
}
