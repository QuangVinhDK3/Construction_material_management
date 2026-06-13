using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;


namespace QuanLyVatLieuXayDung.ViewModels
{
    public class NhaCungCapViewModel : BaseViewModel
    {
        #region Collections & Selected Itemprivate 
        private ObservableCollection<NhaCungCap> _DSNhaCC;
        public ObservableCollection<NhaCungCap> DSNhaCC
        {
            get { return _DSNhaCC; }
            set
            {
                if (_DSNhaCC != value)
                {
                    _DSNhaCC = value;
                    OnPropertyChanged(nameof(DSNhaCC));
                }
            }
        }
        
        private ICollectionView _NhaCungCapView;
        public ICollectionView NhaCungCapView
        {
            get => _NhaCungCapView;
            set { _NhaCungCapView = value; OnPropertyChanged(nameof(NhaCungCapView)); }
        }

        private string _SearchKeyword;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set { _SearchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); NhaCungCapView?.Refresh(); }
        }

        private DateTime? _FilterTuNgay;
        public DateTime? FilterTuNgay
        {
            get => _FilterTuNgay;
            set { _FilterTuNgay = value; OnPropertyChanged(nameof(FilterTuNgay)); NhaCungCapView?.Refresh(); }
        }

        private DateTime? _FilterDenNgay;
        public DateTime? FilterDenNgay
        {
            get => _FilterDenNgay;
            set { _FilterDenNgay = value; OnPropertyChanged(nameof(FilterDenNgay)); NhaCungCapView?.Refresh(); }
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
                if (_SelectedNCC != null)
                {
                    DisplayName = SelectedNCC.DisplayName;
                    Phone = SelectedNCC.Phone;
                    Email = SelectedNCC.Email;
                    Address = SelectedNCC.Address;
                    ContractDate = SelectedNCC.ContractDate;
                    MoreInfo = SelectedNCC.MoreInfo;
                }
            }
        }
        #endregion

        #region Properties (Đã bổ sung đầy đủ để không bị lỗi biên dịch)
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { if (_DisplayName != value) { _DisplayName = value; OnPropertyChanged(nameof(DisplayName)); } }
        }

        private string _Phone;
        public string Phone
        {
            get { return _Phone; }
            set { if (_Phone != value) { _Phone = value; OnPropertyChanged(nameof(Phone)); } }
        }

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set { if (_Email != value) { _Email = value; OnPropertyChanged(nameof(Email)); } }
        }

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { if (_Address != value) { _Address = value; OnPropertyChanged(nameof(Address)); } }
        }

        private DateTime? _ContractDate;
        public DateTime? ContractDate
        {
            get { return _ContractDate; }
            set { if (_ContractDate != value) { _ContractDate = value; OnPropertyChanged(nameof(ContractDate)); } }
        }

        private string _MoreInfo;
        public string MoreInfo
        {
            get { return _MoreInfo; }
            set { if (_MoreInfo != value) { _MoreInfo = value; OnPropertyChanged(nameof(MoreInfo)); } }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        #endregion

        public NhaCungCapViewModel()
        {
            RefreshData();
            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => true, (p) => Remove());
        }

        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.NhaCungCaps.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSNhaCC = new ObservableCollection<NhaCungCap>(list);

            NhaCungCapView = CollectionViewSource.GetDefaultView(DSNhaCC);
            NhaCungCapView.Filter = (obj) =>
            {
                var item = obj as NhaCungCap;
                if (item == null) return false;
                
                bool matchKeyword = string.IsNullOrWhiteSpace(SearchKeyword) || 
                                    (item.ID != null && item.ID.ToLower().Contains(SearchKeyword.ToLower())) ||
                                    (item.DisplayName != null && item.DisplayName.ToLower().Contains(SearchKeyword.ToLower()));
                
                bool matchTuNgay = !FilterTuNgay.HasValue || (item.ContractDate >= FilterTuNgay.Value.Date);
                bool matchDenNgay = !FilterDenNgay.HasValue || (item.ContractDate <= FilterDenNgay.Value.Date.AddDays(1).AddTicks(-1));
                                   
                return matchKeyword && matchTuNgay && matchDenNgay;
            };
        }

        private string AutoCreateID()
        {
            for (int i = 1; i <= 999; i++)
            {
                // Đổi thành "D3" để ra mã dạng NCC001 đến NCC999
                string testID = "NCC" + i.ToString("D3");

                // SỬA: Check bảng Nhà Cung Cấp thay vì Khách Hàng
                if (!DataProvider.Ins.DB.NhaCungCaps.Any(p => p.ID == testID))
                {
                    return testID;
                }
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Nhà cung cấp (NCC999)!");
        }

        private void ClearFields()
        {
            DisplayName = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            Address = string.Empty;
            ContractDate = null;
            MoreInfo = string.Empty;
        }

        public void Add()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp !!!", "Cảnh báo !");
                return;
            }

            // SỬA: Check trùng Email của Nhà Cung Cấp
            var isExist = DataProvider.Ins.DB.NhaCungCaps.Any(p => p.Email == Email);
            if (isExist && !string.IsNullOrEmpty(Email))
            {
                MessageBox.Show("Email nhà cung cấp đã tồn tại, vui lòng nhập email khác !!!", "Cảnh báo !");
                return;
            }

            string nextID = AutoCreateID();
            var ncc = new NhaCungCap()
            {
                ID = nextID,
                DisplayName = DisplayName,
                Phone = Phone,
                Email = Email,
                MoreInfo = MoreInfo,
                Address = Address,
                ContractDate = ContractDate
            };

            DataProvider.Ins.DB.NhaCungCaps.Add(ncc);
            DataProvider.Ins.DB.SaveChanges();

            RefreshData();
            MessageBox.Show("Thêm thành công !!!", "Thông báo");

            ClearFields();
        }

        public void Remove()
        {
            if (SelectedNCC == null)
            {
                MessageBox.Show("Vui lòng chọn để xóa !!!", "Cảnh báo !");
                return;
            }

            // Thêm bước xác nhận trước khi xóa cho an toàn UX
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhà cung cấp {SelectedNCC.DisplayName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataProvider.Ins.DB.NhaCungCaps.Remove(SelectedNCC);
                DataProvider.Ins.DB.SaveChanges();

                RefreshData();
                MessageBox.Show("Xóa thành công !!!", "Thông báo");

                ClearFields();
                SelectedNCC = null;
            }
        }

        public void Update()
        {
            if (SelectedNCC == null)
            {
                MessageBox.Show("Vui lòng chọn để sửa !!!", "Cảnh báo !");
                return;
            }
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp !!!", "Cảnh báo !");
                return;
            }

            var isExist = DataProvider.Ins.DB.NhaCungCaps.Any(p => p.Email == Email && p.ID != SelectedNCC.ID);
            if (isExist && !string.IsNullOrEmpty(Email))
            {
                MessageBox.Show("Email nhà cung cấp đã tồn tại, vui lòng nhập email khác !!!", "Cảnh báo !");
                return;
            }

            // SỬA: Đổi tên biến kh -> ncc cho đúng ngữ nghĩa
            var ncc = DataProvider.Ins.DB.NhaCungCaps.Where(p => p.ID == SelectedNCC.ID).SingleOrDefault();
            if (ncc != null)
            {
                ncc.DisplayName = DisplayName;
                ncc.Phone = Phone;
                ncc.Email = Email;
                ncc.Address = Address;
                ncc.ContractDate = ContractDate;
                ncc.MoreInfo = MoreInfo;

                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                MessageBox.Show("Sửa thành công !!!", "Thông báo");

                ClearFields();
                SelectedNCC = null; // SỬA: Từ SelectedKH thành SelectedNCC
            }
        }
    }
}