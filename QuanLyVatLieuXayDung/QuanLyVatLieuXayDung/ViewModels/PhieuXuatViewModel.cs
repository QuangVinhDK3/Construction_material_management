using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class PhieuXuatViewModel : BaseViewModel
    {
        #region Collections
        private ObservableCollection<ChiTietPhieuXuat> _DSCTPhieuXuat;
        public ObservableCollection<ChiTietPhieuXuat> DSCTPhieuXuat
        {
            get => _DSCTPhieuXuat;
            set { _DSCTPhieuXuat = value; OnPropertyChanged(); }
        }

        private ObservableCollection<VatLieu> _DSVatLieu;
        public ObservableCollection<VatLieu> DSVatLieu
        {
            get => _DSVatLieu;
            set { _DSVatLieu = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PhieuXuat> _DSPhieuXuat;
        public ObservableCollection<PhieuXuat> DSPhieuXuat
        {
            get => _DSPhieuXuat;
            set { _DSPhieuXuat = value; OnPropertyChanged(); }
        }

        private ObservableCollection<KhachHang> _DSKhachHang;
        public ObservableCollection<KhachHang> DSKhachHang
        {
            get => _DSKhachHang;
            set { _DSKhachHang = value; OnPropertyChanged(); }
        }
        #endregion

        #region Selected Items & Properties
        private ChiTietPhieuXuat _SelectedCTPX;
        public ChiTietPhieuXuat SelectedCTPX
        {
            get => _SelectedCTPX;
            set
            {
                _SelectedCTPX = value;
                OnPropertyChanged();
                if (_SelectedCTPX != null)
                {
                    IDOutput = _SelectedCTPX.IDOutput;
                    IDObject = _SelectedCTPX.IDObject;
                    Counts = _SelectedCTPX.Counts ?? 0;
                    Price = _SelectedCTPX.Price ?? 0;
                    var phieuXuat = DSPhieuXuat?.FirstOrDefault(p => p.ID == _SelectedCTPX.IDOutput);
                }
            }
        }

        private string _IDOutput;
        public string IDOutput { get => _IDOutput; set { _IDOutput = value; OnPropertyChanged(); } }

        private string _IDObject;
        public string IDObject 
        { 
            get => _IDObject; 
            set 
            { 
                if (_IDObject != value)
                {
                    _IDObject = value; 
                    OnPropertyChanged(); 
                    
                    if (!string.IsNullOrEmpty(_IDObject))
                    {
                        var latestInput = DataProvider.Ins.DB.ChiTietPhieuNhaps
                            .Where(c => c.IDObject == _IDObject)
                            .OrderByDescending(c => c.PhieuNhap.DateInput)
                            .FirstOrDefault();

                        if (latestInput != null && latestInput.PriceOutput.HasValue)
                        {
                            Price = latestInput.PriceOutput.Value;
                        }
                        else
                        {
                            Price = 0;
                        }
                    }
                }
            } 
        }

        private int _Counts;
        public int Counts 
        { 
            get => _Counts; 
            set 
            { 
                _Counts = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(ThanhTien));
            } 
        }

        private double _Price;
        public double Price 
        { 
            get => _Price; 
            set 
            { 
                _Price = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(ThanhTien));
            } 
        }

        public double ThanhTien => Counts * Price;
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        #endregion

        public PhieuXuatViewModel()
        {
            // Nạp dữ liệu lần đầu khi mở form bằng RefreshData
            DSPhieuXuat = new ObservableCollection<PhieuXuat>(DataProvider.Ins.DB.PhieuXuats.ToList());
            DSVatLieu = new ObservableCollection<VatLieu>(DataProvider.Ins.DB.VatLieux.ToList());
            DSKhachHang = new ObservableCollection<KhachHang>(DataProvider.Ins.DB.KhachHangs.ToList());
            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => SelectedCTPX != null, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => SelectedCTPX != null, (p) => Remove());
        }

        #region Helper Methods
        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.ChiTietPhieuXuats
                        .Include(c => c.VatLieu)
                        .Include(c => c.PhieuXuat)
                        .Include(c => c.PhieuXuat.KhachHang)
                        .ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSCTPhieuXuat = new ObservableCollection<ChiTietPhieuXuat>(list);
        }

        // HÀM XỬ LÝ LOGIC TỒN KHO
        private int LaySoLuongTonKho(string idVatLieu)
        {
            int tongNhap = DataProvider.Ins.DB.ChiTietPhieuNhaps
                            .Where(p => p.IDObject == idVatLieu)
                            .Sum(p => (int?)p.Counts) ?? 0;

            int tongXuat = DataProvider.Ins.DB.ChiTietPhieuXuats
                            .Where(p => p.IDObject == idVatLieu)
                            .Sum(p => (int?)p.Counts) ?? 0;

            return tongNhap - tongXuat;
        }

        private string AutoCreateID()
        {
            var existingIDs = DataProvider.Ins.DB.ChiTietPhieuXuats.Select(p => p.ID.Trim()).ToList();
            for (int i = 1; i <= 9999; i++)
            {
                string testID = "CTPX" + i.ToString("D3");
                if (!existingIDs.Contains(testID)) return testID;
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Chi tiết phiếu xuất!");
        }

        private void ClearFields()
        {
            SelectedCTPX = null;
            IDOutput = string.Empty;
            IDObject = string.Empty;
            Counts = 0;
            Price = 0;
        }
        #endregion

        #region CRUD Operations
        public void Add()
        {
            if (string.IsNullOrEmpty(IDOutput) || string.IsNullOrEmpty(IDObject))
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Mã phiếu xuất và Vật liệu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Counts <= 0)
            {
                MessageBox.Show("Số lượng xuất phải lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int tonKhoHienTai = LaySoLuongTonKho(IDObject);
            if (Counts > tonKhoHienTai)
            {
                MessageBox.Show($"Không đủ vật liệu để xuất!\nSố lượng tồn kho hiện tại chỉ còn: {tonKhoHienTai}", "Lỗi tồn kho", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var ctpx = new ChiTietPhieuXuat()
                {
                    ID = AutoCreateID(),
                    IDOutput = IDOutput,
                    IDObject = IDObject,
                    Counts = Counts,
                    Price = Price
                };

                DataProvider.Ins.DB.ChiTietPhieuXuats.Add(ctpx);
                
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Thêm chi tiết phiếu xuất thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(IDOutput) || string.IsNullOrEmpty(IDObject))
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var ctpx = DataProvider.Ins.DB.ChiTietPhieuXuats.SingleOrDefault(p => p.ID == SelectedCTPX.ID);
                if (ctpx != null)
                {
                    int tonKhoHienTai = LaySoLuongTonKho(IDObject);
                    int tonKhoThucTe = tonKhoHienTai + (ctpx.Counts ?? 0);

                    if (ctpx.IDObject != IDObject)
                    {
                        tonKhoThucTe = LaySoLuongTonKho(IDObject);
                    }

                    if (Counts > tonKhoThucTe)
                    {
                        MessageBox.Show($"Không thể cập nhật!\nSố lượng tồn kho tối đa có thể xuất là: {tonKhoThucTe}", "Lỗi tồn kho", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ctpx.IDOutput = IDOutput;
                    ctpx.IDObject = IDObject;
                    ctpx.Counts = Counts;
                    ctpx.Price = Price;

                    DataProvider.Ins.DB.SaveChanges();

                    MessageBox.Show("Cập nhật chi tiết phiếu xuất thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Remove()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa chi tiết phiếu xuất này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var ctpx = DataProvider.Ins.DB.ChiTietPhieuXuats.SingleOrDefault(p => p.ID == SelectedCTPX.ID);
                    if (ctpx != null)
                    {
                        DataProvider.Ins.DB.ChiTietPhieuXuats.Remove(ctpx);
                        DataProvider.Ins.DB.SaveChanges();

                        MessageBox.Show("Xóa thành công! Số lượng vật liệu đã được hoàn lại vào kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearFields();
                        RefreshData();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa do dữ liệu này đang bị ràng buộc ở nơi khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}