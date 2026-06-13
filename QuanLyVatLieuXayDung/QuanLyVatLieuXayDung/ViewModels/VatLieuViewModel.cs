using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using QuanLyVatLieuXayDung.Models;
using System.Windows.Input; // Thêm để sử dụng ICommand
using System.Windows;       // Thêm để sử dụng MessageBox
using System.Data.Entity;   // Thêm để sử dụng Include()
using System.ComponentModel;
using System.Windows.Data;


namespace QuanLyVatLieuXayDung.ViewModels
{
    public class VatLieuViewModel : BaseViewModel
    {
        #region Collections
        private ObservableCollection<VatLieu> _DSVatLieu;
        public ObservableCollection<VatLieu> DSVatLieu
        {
            get { return _DSVatLieu; }
            set { if (_DSVatLieu != value) { _DSVatLieu = value; OnPropertyChanged(nameof(DSVatLieu)); } }
        }

        private ObservableCollection<LoaiVatLieu> _DSLoaiVL;
        public ObservableCollection<LoaiVatLieu> DSLoaiVL
        {
            get { return _DSLoaiVL; }
            set { if (_DSLoaiVL != value) { _DSLoaiVL = value; OnPropertyChanged(nameof(DSLoaiVL)); } }
        }

        private ICollectionView _VatLieuView;
        public ICollectionView VatLieuView
        {
            get => _VatLieuView;
            set { _VatLieuView = value; OnPropertyChanged(nameof(VatLieuView)); }
        }

        private string _SearchKeyword;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set 
            { 
                _SearchKeyword = value; 
                OnPropertyChanged(nameof(SearchKeyword)); 
                VatLieuView?.Refresh(); 
            }
        }

        #endregion

        #region Selected Item
        private VatLieu _SelectedVL;
        public VatLieu SelectedVL
        {
            get { return _SelectedVL; }
            set
            {
                if (_SelectedVL != value)
                {
                    _SelectedVL = value;
                    OnPropertyChanged(nameof(SelectedVL));
                }
                if (_SelectedVL != null)
                {
                    IDVatLieu = SelectedVL.ID;
                    DisplayName = SelectedVL.DisplayName;
                    IDLoaiVL = _SelectedVL.IDLoaiVatLieu;
                    SelectedLoaiVL = _SelectedVL.LoaiVatLieu;
                    QRCode = _SelectedVL.QRCode;
                    Barcode = _SelectedVL.Barcode;
                    DonViTinh = _SelectedVL.DonViTinh;
                    GiaXuat = _SelectedVL.GiaXuat;
                }
            }
        }
        private LoaiVatLieu _SelectedLoaiVL;
        public LoaiVatLieu SelectedLoaiVL
        {
            get { return _SelectedLoaiVL; }
            set
            {
                if (_SelectedLoaiVL != value)
                {
                    _SelectedLoaiVL = value;
                    OnPropertyChanged(nameof(SelectedLoaiVL));
                }
            }
        }
        #endregion

        #region Properties
        private string _IDVatLieu;
        public string IDVatLieu
        {
            get { return _IDVatLieu; }
            set { if (_IDVatLieu != value) { _IDVatLieu = value; OnPropertyChanged(nameof(IDVatLieu)); } }
        }

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { if (_DisplayName != value) { _DisplayName = value; OnPropertyChanged(nameof(DisplayName)); } }
        }

        private string _IDLoaiVL;
        public string IDLoaiVL
        {
            get { return _IDLoaiVL; }
            set { if (_IDLoaiVL != value) { _IDLoaiVL = value; OnPropertyChanged(nameof(IDLoaiVL)); } }
        }


        private string _QRCode;
        public string QRCode
        {
            get { return _QRCode; }
            set { if (_QRCode != value) { _QRCode = value; OnPropertyChanged(nameof(QRCode)); } }
        }

        private string _Barcode;
        public string Barcode
        {
            get { return _Barcode; }
            set { if (_Barcode != value) { _Barcode = value; OnPropertyChanged(nameof(Barcode)); } }
        }

        private string _DonViTinh;
        public string DonViTinh
        {
            get { return _DonViTinh; }
            set { if (_DonViTinh != value) { _DonViTinh = value; OnPropertyChanged(nameof(DonViTinh)); } }
        }

        private double _GiaXuat;
        public double GiaXuat
        {
            get { return _GiaXuat; }
            set { if (_GiaXuat != value) { _GiaXuat = value; OnPropertyChanged(nameof(GiaXuat)); } }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand ViewPriceHistoryCommand { get; set; }
        #endregion

        public VatLieuViewModel()
        {
            RefreshData();
            DSLoaiVL = new ObservableCollection<LoaiVatLieu>(DataProvider.Ins.DB.LoaiVatLieux.ToList());

            // ĐÃ BỔ SUNG: Khởi tạo Command để các nút bấm trên XAML hoạt động được
            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => true, (p) => Remove());
            ViewPriceHistoryCommand = new RelayCommand<object>((p) => SelectedVL != null, (p) => ViewPriceHistory());
        }

        private void RefreshData()
        {
            // Dùng AsNoTracking để tránh lỗi đếm kép do DbContext singleton
            var list = DataProvider.Ins.DB.VatLieux
                .AsNoTracking()
                .Include(v => v.LoaiVatLieu)
                .ToList();

            // Lấy tổng nhập và xuất bằng truy vấn trực tiếp (không qua navigation)
            var tongNhapDict = DataProvider.Ins.DB.ChiTietPhieuNhaps
                .AsNoTracking()
                .GroupBy(c => c.IDObject)
                .Select(g => new { IDObject = g.Key, Total = g.Sum(c => (int?)c.Counts ?? 0) })
                .ToDictionary(x => x.IDObject.Trim(), x => x.Total);

            var tongXuatDict = DataProvider.Ins.DB.ChiTietPhieuXuats
                .AsNoTracking()
                .GroupBy(c => c.IDObject)
                .Select(g => new { IDObject = g.Key, Total = g.Sum(c => (int?)c.Counts ?? 0) })
                .ToDictionary(x => x.IDObject.Trim(), x => x.Total);

            var giaXuatDict = DataProvider.Ins.DB.ChiTietPhieuNhaps
                .AsNoTracking()
                .Where(c => c.PriceOutput.HasValue)
                .GroupBy(c => c.IDObject)
                .Select(g => new { IDObject = g.Key, MaxPrice = g.Max(c => c.PriceOutput) })
                .ToDictionary(x => x.IDObject.Trim(), x => x.MaxPrice ?? 0);

            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;

                string id = list[i].ID.Trim();
                int tongNhap = tongNhapDict.ContainsKey(id) ? tongNhapDict[id] : 0;
                int tongXuat = tongXuatDict.ContainsKey(id) ? tongXuatDict[id] : 0;
                list[i].SoLuongTon = tongNhap - tongXuat;
                list[i].GiaXuat = giaXuatDict.ContainsKey(id) ? giaXuatDict[id] : 0;

                // Xác định trạng thái
                if (list[i].SoLuongTon <= 0)
                    list[i].TrangThai = "Hết hàng";
                else if (list[i].SoLuongTon <= 10)
                    list[i].TrangThai = "Sắp hết";
                else
                    list[i].TrangThai = "Còn hàng";
            }
            DSVatLieu = new ObservableCollection<VatLieu>(list);

            VatLieuView = CollectionViewSource.GetDefaultView(DSVatLieu);
            VatLieuView.Filter = (obj) =>
            {
                if (string.IsNullOrWhiteSpace(SearchKeyword)) return true;
                var item = obj as VatLieu;
                if (item == null) return false;
                string keyword = SearchKeyword.ToLower();
                return (item.DisplayName != null && item.DisplayName.ToLower().Contains(keyword)) ||
                       (item.LoaiVatLieu != null && item.LoaiVatLieu.DisplayName != null && item.LoaiVatLieu.DisplayName.ToLower().Contains(keyword));
            };
        }


        // ĐÃ SỬA: Dọn dẹp chính xác các trường nhập liệu của Vật liệu
        private void ClearFields()
        {
            IDVatLieu = string.Empty;
            DisplayName = string.Empty;
            IDLoaiVL = null;
            QRCode = string.Empty;
            Barcode = string.Empty;
            DonViTinh = string.Empty;
            GiaXuat = 0;
        }

        public void Add()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập tên vật liệu !!!", "Cảnh báo !");
                return;
            }
            if (string.IsNullOrEmpty(IDVatLieu))
            {
                MessageBox.Show("Vui lòng nhập mã vật liệu !!!", "Cảnh báo !");
                return;
            }

            var isExist = DataProvider.Ins.DB.VatLieux.Any(p => p.ID == IDVatLieu);
            if (isExist)
            {
                MessageBox.Show("Mã vật liệu đã tồn tại, vui lòng nhập mã khác !!!", "Cảnh báo !");
                return;
            }

            var vl = new VatLieu()
            {
                ID = IDVatLieu,
                DisplayName = DisplayName,
                IDLoaiVatLieu = IDLoaiVL,
                Barcode = Barcode,
                QRCode = QRCode,
                DonViTinh = DonViTinh,
            };

            DataProvider.Ins.DB.VatLieux.Add(vl);

            DataProvider.Ins.DB.SaveChanges();

            RefreshData();
            MessageBox.Show("Thêm vật liệu thành công !!!", "Thông báo");

            ClearFields();
        }

        public void Remove()
        {
            if (SelectedVL == null)
            {
                MessageBox.Show("Vui lòng chọn vật liệu để xóa !!!", "Cảnh báo !");
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa vật liệu {SelectedVL.DisplayName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataProvider.Ins.DB.VatLieux.Remove(SelectedVL);
                    DataProvider.Ins.DB.SaveChanges();

                    RefreshData();
                    MessageBox.Show("Xóa thành công !!!", "Thông báo");

                    ClearFields();
                    SelectedVL = null;
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa vật liệu này vì đã có phát sinh giao dịch ở các chứng từ kho!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void Update()
        {
            if (SelectedVL == null)
            {
                MessageBox.Show("Vui lòng chọn vật liệu để sửa !!!", "Cảnh báo !");
                return;
            }
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập tên vật liệu !!!", "Cảnh báo !");
                return;
            }

            // Kiểm tra xem có trùng tên với một vật liệu KHÁC mặt hàng đang sửa không
            var isExist = DataProvider.Ins.DB.VatLieux.Any(p => p.DisplayName == DisplayName && p.ID != SelectedVL.ID);
            if (isExist)
            {
                MessageBox.Show("Tên vật liệu này đã tồn tại ở mặt hàng khác, vui lòng kiểm tra lại !!!", "Cảnh báo !");
                return;
            }

            var vl = DataProvider.Ins.DB.VatLieux.Where(p => p.ID == SelectedVL.ID).SingleOrDefault();
            if (vl != null) // ĐÃ SỬA: Sửa từ kh -> vl để tránh lỗi biên dịch
            {
                vl.DisplayName = DisplayName;
                vl.IDLoaiVatLieu = IDLoaiVL;
                vl.Barcode = Barcode;
                vl.QRCode = QRCode;
                vl.DonViTinh = DonViTinh;

                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                MessageBox.Show("Sửa thành công !!!", "Thông báo");

                ClearFields();
                SelectedVL = null;
            }
        }

        public void ViewPriceHistory()
        {
            if (SelectedVL == null) return;
            
            var view = new QuanLyVatLieuXayDung.Views.LichSuGiaVatLieuView(SelectedVL.ID, SelectedVL.DisplayName);
            view.ShowDialog();
        }
    }
}