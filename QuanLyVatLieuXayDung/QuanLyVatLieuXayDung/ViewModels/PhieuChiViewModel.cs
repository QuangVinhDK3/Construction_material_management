using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using QuanLyVatLieuXayDung.Views;


namespace QuanLyVatLieuXayDung.ViewModels
{
    public class PhieuChiViewModel : BaseViewModel
    {
        private ObservableCollection<PhieuChi> _DSPhieuChi;
        public ObservableCollection<PhieuChi> DSPhieuChi
        {
            get => _DSPhieuChi;
            set { _DSPhieuChi = value; OnPropertyChanged(); }
        }

        private ICollectionView _PhieuChiView;
        public ICollectionView PhieuChiView
        {
            get => _PhieuChiView;
            set { _PhieuChiView = value; OnPropertyChanged(); }
        }

        private string _SearchKeyword;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set { _SearchKeyword = value; OnPropertyChanged(); PhieuChiView?.Refresh(); }
        }

        private DateTime? _FilterTuNgay;
        public DateTime? FilterTuNgay
        {
            get => _FilterTuNgay;
            set { _FilterTuNgay = value; OnPropertyChanged(); PhieuChiView?.Refresh(); }
        }

        private DateTime? _FilterDenNgay;
        public DateTime? FilterDenNgay
        {
            get => _FilterDenNgay;
            set { _FilterDenNgay = value; OnPropertyChanged(); PhieuChiView?.Refresh(); }
        }

        private ObservableCollection<NhaCungCap> _DSNhaCungCap;
        public ObservableCollection<NhaCungCap> DSNhaCungCap
        {
            get => _DSNhaCungCap;
            set { _DSNhaCungCap = value; OnPropertyChanged(); }
        }

        private PhieuChi _SelectedPhieuChi;
        public PhieuChi SelectedPhieuChi
        {
            get => _SelectedPhieuChi;
            set
            {
                if (_SelectedPhieuChi != value)
                {
                    _SelectedPhieuChi = value;
                    OnPropertyChanged();
                    if (_SelectedPhieuChi != null)
                    {
                        IDNhaCungCap = _SelectedPhieuChi.IDNhaCungCap;
                        NgayChi = _SelectedPhieuChi.NgayChi;
                        SoTien = _SelectedPhieuChi.SoTien;
                        NoiDung = _SelectedPhieuChi.NoiDung;
                    }
                }
            }
        }

        private string _IDNhaCungCap;
        public string IDNhaCungCap
        {
            get => _IDNhaCungCap;
            set { _IDNhaCungCap = value; OnPropertyChanged(); }
        }

        private DateTime? _NgayChi;
        public DateTime? NgayChi
        {
            get => _NgayChi;
            set { _NgayChi = value; OnPropertyChanged(); }
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

        public PhieuChiViewModel()
        {
            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => SelectedPhieuChi != null, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => SelectedPhieuChi != null, (p) => Remove());
        }

        private void RefreshData()
        {
            DSNhaCungCap = new ObservableCollection<NhaCungCap>(DataProvider.Ins.DB.NhaCungCaps.ToList());
            
            var list = DataProvider.Ins.DB.PhieuChis.Include("NguoiDung").Include("NhaCungCap").ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSPhieuChi = new ObservableCollection<PhieuChi>(list);

            PhieuChiView = CollectionViewSource.GetDefaultView(DSPhieuChi);
            PhieuChiView.Filter = (obj) =>
            {
                var item = obj as PhieuChi;
                if (item == null) return false;
                
                bool matchKeyword = string.IsNullOrWhiteSpace(SearchKeyword) || 
                                    (item.ID != null && item.ID.ToLower().Contains(SearchKeyword.ToLower()));
                
                bool matchTuNgay = !FilterTuNgay.HasValue || (item.NgayChi >= FilterTuNgay.Value.Date);
                bool matchDenNgay = !FilterDenNgay.HasValue || (item.NgayChi <= FilterDenNgay.Value.Date.AddDays(1).AddTicks(-1));
                                   
                return matchKeyword && matchTuNgay && matchDenNgay;
            };
        }

        private void ClearFields()
        {
            IDNhaCungCap = null;
            NgayChi = DateTime.Now;
            SoTien = 0;
            NoiDung = string.Empty;
            SelectedPhieuChi = null;
        }

        private string AutoCreateID()
        {
            for (int i = 1; i <= 999; i++)
            {
                string testID = "PC" + i.ToString("D3");
                if (!DataProvider.Ins.DB.PhieuChis.Any(p => p.ID == testID))
                {
                    return testID;
                }
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Phiếu chi (PC999)!");
        }

        private void Add()
        {
            if (string.IsNullOrEmpty(IDNhaCungCap) || SoTien == null || SoTien <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp và nhập số tiền lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentUserId = null;
            var mainWin = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWin != null && mainWin.DataContext is MainWindowViewModel mainVM)
            {
                currentUserId = mainVM.CurrentNguoiDung?.ID;
            }

            string nextID = AutoCreateID();
            var pc = new PhieuChi()
            {
                ID = nextID,
                IDNhaCungCap = IDNhaCungCap,
                NgayChi = NgayChi ?? DateTime.Now,
                SoTien = SoTien,
                NoiDung = NoiDung,
                IDUser = currentUserId
            };

            DataProvider.Ins.DB.PhieuChis.Add(pc);
            DataProvider.Ins.DB.SaveChanges();

            RefreshData();
            ClearFields();
            MessageBox.Show("Thêm phiếu chi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Update()
        {
            if (SelectedPhieuChi == null) return;
            if (string.IsNullOrEmpty(IDNhaCungCap) || SoTien == null || SoTien <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp và nhập số tiền lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pc = DataProvider.Ins.DB.PhieuChis.SingleOrDefault(p => p.ID == SelectedPhieuChi.ID);
            if (pc != null)
            {
                pc.IDNhaCungCap = IDNhaCungCap;
                pc.NgayChi = NgayChi;
                pc.SoTien = SoTien;
                pc.NoiDung = NoiDung;

                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                ClearFields();
                MessageBox.Show("Cập nhật phiếu chi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Remove()
        {
            if (SelectedPhieuChi == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu chi này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataProvider.Ins.DB.PhieuChis.Remove(SelectedPhieuChi);
                DataProvider.Ins.DB.SaveChanges();

                RefreshData();
                ClearFields();
                MessageBox.Show("Xóa phiếu chi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
