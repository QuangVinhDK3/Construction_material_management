using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;
using QuanLyVatLieuXayDung.Views; // For opening CRUDPhieuNhapView

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class QuanLyPhieuNhapViewModel : BaseViewModel
    {
        #region Collections
        private ObservableCollection<PhieuNhap> _DSPhieuNhap;
        public ObservableCollection<PhieuNhap> DSPhieuNhap
        {
            get => _DSPhieuNhap;
            set { _DSPhieuNhap = value; OnPropertyChanged(nameof(DSPhieuNhap)); }
        }

        private ObservableCollection<NhaCungCap> _DSNhaCC;
        public ObservableCollection<NhaCungCap> DSNhaCC
        {
            get => _DSNhaCC;
            set { _DSNhaCC = value; OnPropertyChanged(nameof(DSNhaCC)); }
        }
        #endregion

        #region Selected Item
        private PhieuNhap _SelectedPN;
        public PhieuNhap SelectedPN
        {
            get => _SelectedPN;
            set
            {
                if (_SelectedPN != value)
                {
                    _SelectedPN = value;
                    OnPropertyChanged(nameof(SelectedPN));
                    if (_SelectedPN != null)
                    {
                        IDPhieuNhap = _SelectedPN.ID;
                        DateInput = _SelectedPN.DateInput;
                        Status = _SelectedPN.Status;
                        IDNhaCC = _SelectedPN.IDNhaCungCap;
                        SelectedNCC = _SelectedPN.NhaCungCap;
                    }
                }
            }
        }

        private NhaCungCap _SelectedNCC;
        public NhaCungCap SelectedNCC
        {
            get => _SelectedNCC;
            set { _SelectedNCC = value; OnPropertyChanged(nameof(SelectedNCC)); }
        }
        #endregion

        #region Properties
        private string _IDPhieuNhap;
        public string IDPhieuNhap
        {
            get => _IDPhieuNhap;
            set { _IDPhieuNhap = value; OnPropertyChanged(nameof(IDPhieuNhap)); }
        }

        private string _IDNhaCC;
        public string IDNhaCC
        {
            get => _IDNhaCC;
            set { _IDNhaCC = value; OnPropertyChanged(nameof(IDNhaCC)); }
        }

        private DateTime? _DateInput;
        public DateTime? DateInput
        {
            get => _DateInput;
            set { _DateInput = value; OnPropertyChanged(nameof(DateInput)); }
        }

        private string _Status;
        public string Status
        {
            get => _Status;
            set { _Status = value; OnPropertyChanged(nameof(Status)); }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand OpenDetailsCommand { get; set; }
        #endregion

        public QuanLyPhieuNhapViewModel()
        {
            DSNhaCC = new ObservableCollection<NhaCungCap>(DataProvider.Ins.DB.NhaCungCaps.ToList());
            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => SelectedPN != null, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => SelectedPN != null, (p) => Remove());

            OpenDetailsCommand = new RelayCommand<object>((p) => true, (p) => OpenDetails());
        }

        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .Include(p => p.NhaCungCap)
                .AsNoTracking()
                .ToList();
            DSPhieuNhap = new ObservableCollection<PhieuNhap>(list);
        }

        private void ClearFields()
        {
            IDPhieuNhap = string.Empty;
            IDNhaCC = null;
            SelectedNCC = null;
            DateInput = null;
            Status = string.Empty;
            SelectedPN = null;
        }

        public void Add()
        {
            if (string.IsNullOrEmpty(IDPhieuNhap))
            {
                MessageBox.Show("Vui lòng nhập Mã phiếu nhập!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var isExist = DataProvider.Ins.DB.PhieuNhaps.Any(p => p.ID == IDPhieuNhap);
            if (isExist)
            {
                MessageBox.Show("Mã phiếu nhập đã tồn tại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var pn = new PhieuNhap()
                {
                    ID = IDPhieuNhap,
                    IDNhaCungCap = IDNhaCC,
                    DateInput = DateInput ?? DateTime.Now,
                    Status = Status
                };

                DataProvider.Ins.DB.PhieuNhaps.Add(pn);
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
            if (SelectedPN == null) return;

            try
            {
                var pn = DataProvider.Ins.DB.PhieuNhaps.SingleOrDefault(p => p.ID == SelectedPN.ID);
                if (pn != null)
                {
                    pn.IDNhaCungCap = IDNhaCC;
                    pn.DateInput = DateInput;
                    pn.Status = Status;

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
            if (SelectedPN == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu nhập {SelectedPN.ID}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var pn = DataProvider.Ins.DB.PhieuNhaps.SingleOrDefault(p => p.ID == SelectedPN.ID);
                    if (pn != null)
                    {
                        DataProvider.Ins.DB.PhieuNhaps.Remove(pn);
                        DataProvider.Ins.DB.SaveChanges();

                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearFields();
                        RefreshData();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa phiếu nhập này vì đã có chi tiết vật liệu bên trong!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OpenDetails()
        {
            if (SelectedPN != null)
            {
                // To keep it simple, we just open the CRUDPhieuNhapView.
                // In a real scenario, we might want to pass the selected IDPhieuNhap to filter the details.
                var wd = new CRUDPhieuNhapView();
                
                // If the user wants the opened window to automatically select the parent PhieuNhap,
                // we can access the DataContext.
                if (wd.DataContext is PhieuNhapViewModel vm)
                {
                    // Select the matching PhieuNhap in the combobox of CRUDPhieuNhapView
                    vm.SelectedPN = vm.DSPhieuNhap.FirstOrDefault(p => p.ID == SelectedPN.ID);
                    vm.IDPhieuNhap = SelectedPN.ID;
                }

                wd.ShowDialog();
            }
        }
    }
}
