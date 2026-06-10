using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // Bổ sung thư viện này cho MessageBox
using System.Windows.Input;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class KhachHangViewModel : BaseViewModel
    {
        private ObservableCollection<KhachHang> _DSKhach;
        public ObservableCollection<KhachHang> DSKhach
        {
            get { return _DSKhach; }
            set { if (_DSKhach != value) { _DSKhach = value; OnPropertyChanged(nameof(DSKhach)); } }
        }

        private KhachHang _SelectedKH;
        public KhachHang SelectedKH
        {
            get { return _SelectedKH; }
            set
            {
                if (_SelectedKH != value)
                {
                    _SelectedKH = value;
                    OnPropertyChanged(nameof(SelectedKH));
                }
                if (_SelectedKH != null)
                {
                    DisplayName = SelectedKH.DisplayName;
                    Phone = SelectedKH.Phone;
                    Email = SelectedKH.Email;
                    MoreInfo = SelectedKH.MoreInfo;
                    Address = SelectedKH.Address;
                    ContractDate = SelectedKH.ContractDate;
                }
            }
        }

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

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { if (_Address != value) { _Address = value; OnPropertyChanged(nameof(Address)); } }
        }

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set { if (_Email != value) { _Email = value; OnPropertyChanged(nameof(Email)); } }
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

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public KhachHangViewModel()
        {
            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => true, (p) => Remove());
        }

        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.KhachHangs
                .Include("PhieuXuats.ChiTietPhieuXuats")
                .ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSKhach = new ObservableCollection<KhachHang>(list);
        }

        private string AutoCreateID()
        {
            for (int i = 1; i <= 999; i++)
            {
                string testID = "KH" + i.ToString("D3");
                if (!DataProvider.Ins.DB.KhachHangs.Any(p => p.ID == testID))
                {
                    return testID;
                }
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Loại vật liệu (LVL999)!");
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
                MessageBox.Show("Vui lòng nhập tên khách hàng !!!", "Cảnh báo !");
                return;
            }
            var isExist = DataProvider.Ins.DB.KhachHangs.Any(p => p.DisplayName == DisplayName);
            if (isExist)
            {
                MessageBox.Show("Tên khách hàng đã tồn tại, vui lòng nhập tên khác !!!", "Cảnh báo !");
                return;
            }

            string nextID = AutoCreateID();
            var kh = new KhachHang()
            {
                ID = nextID,
                DisplayName = DisplayName,
                Phone = Phone,
                Email = Email,
                MoreInfo=MoreInfo,
                Address = Address,
                ContractDate = ContractDate
            };

            DataProvider.Ins.DB.KhachHangs.Add(kh);
            DataProvider.Ins.DB.SaveChanges();

            RefreshData();
            MessageBox.Show("Thêm thành công !!!", "Thông báo");

            ClearFields();
        }

        public void Remove()
        {
            if (SelectedKH == null)
            {
                MessageBox.Show("Vui lòng chọn để xóa !!!", "Cảnh báo !");
                return;
            }

            DataProvider.Ins.DB.KhachHangs.Remove(SelectedKH);
            DataProvider.Ins.DB.SaveChanges();

            RefreshData();
            MessageBox.Show("Xóa thành công !!!", "Thông báo");

            ClearFields();
            SelectedKH = null;
        }

        public void Update()
        {
            if (SelectedKH == null)
            {
                MessageBox.Show("Vui lòng chọn để sửa !!!", "Cảnh báo !");
                return;
            }
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng !!!", "Cảnh báo !");
                return;
            }

            var isExist = DataProvider.Ins.DB.KhachHangs.Any(p => p.DisplayName == DisplayName && p.ID != SelectedKH.ID);
            if (isExist)
            {
                MessageBox.Show("Tên khách hàng đã tồn tại, vui lòng nhập tên khác !!!", "Cảnh báo !");
                return;
            }

            var kh = DataProvider.Ins.DB.KhachHangs.Where(p => p.ID == SelectedKH.ID).SingleOrDefault();
            if (kh != null)
            {
                kh.DisplayName = DisplayName;
                kh.Phone = Phone;
                kh.Email = Email;
                kh.Address = Address;
                kh.ContractDate = ContractDate;
                kh.MoreInfo = MoreInfo;
                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                MessageBox.Show("Sửa thành công !!!", "Thông báo");

                ClearFields();
                SelectedKH = null;
            }
        }
    }
}