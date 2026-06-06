using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using QuanLyVatLieuXayDung.Models;
using System.Windows.Input; // Thêm để sử dụng ICommand
using System.Windows;       // Thêm để sử dụng MessageBox

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

        private ObservableCollection<NhaCungCap> _DSNhaCC;
        public ObservableCollection<NhaCungCap> DSNhaCC
        {
            get { return _DSNhaCC; }
            set { if (_DSNhaCC != value) { _DSNhaCC = value; OnPropertyChanged(nameof(DSNhaCC)); } }
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
                    IDLoaiVL = SelectedVL.IDLoaiVatLieu;
                    IDNhaCC = SelectedVL.IDNhaCungCap;
                    QRCode = SelectedVL.QRCode;
                    Barcode = SelectedVL.Barcode;
                    SelectedLoaiVL = SelectedVL.LoaiVatLieu;
                    SelectedNCC=SelectedVL.NhaCungCap;
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
        private NhaCungCap _SelectedNCC;
        public NhaCungCap SelectedNCC
        {
            get { return _SelectedNCC; }
            set
            {
                if (_SelectedNCC != value)
                {
                    _SelectedNCC = value;
                    OnPropertyChanged(nameof(SelectedNCC));
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

        private string _IDNhaCC;
        public string IDNhaCC
        {
            get { return _IDNhaCC; }
            set { if (_IDNhaCC != value) { _IDNhaCC = value; OnPropertyChanged(nameof(IDNhaCC)); } }
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
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        #endregion

        public VatLieuViewModel()
        {
            RefreshData();
            DSLoaiVL = new ObservableCollection<LoaiVatLieu>(DataProvider.Ins.DB.LoaiVatLieux.ToList());
            DSNhaCC = new ObservableCollection<NhaCungCap>(DataProvider.Ins.DB.NhaCungCaps.ToList());

            // ĐÃ BỔ SUNG: Khởi tạo Command để các nút bấm trên XAML hoạt động được
            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => true, (p) => Remove());
        }

        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.VatLieux.ToList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    list[i].STT = i + 1;
            //}
            DSVatLieu = new ObservableCollection<VatLieu>(list);
        }

        // ĐÃ SỬA: Dọn dẹp chính xác các trường nhập liệu của Vật liệu
        private void ClearFields()
        {
            IDVatLieu = string.Empty;
            DisplayName = string.Empty;
            IDLoaiVL = null;
            IDNhaCC = null;
            QRCode = string.Empty;
            Barcode = string.Empty;
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
                IDNhaCungCap = IDNhaCC,
                Barcode = Barcode,
                QRCode = QRCode,
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
                vl.IDNhaCungCap = IDNhaCC;
                vl.Barcode = Barcode;
                vl.QRCode = QRCode;

                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                MessageBox.Show("Sửa thành công !!!", "Thông báo");

                ClearFields();
                SelectedVL = null;
            }
        }
    }
}