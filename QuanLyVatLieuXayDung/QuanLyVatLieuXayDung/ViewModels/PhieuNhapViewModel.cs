using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; // ĐÃ BỔ SUNG THƯ VIỆN NÀY ĐỂ DÙNG ICOMMAND
using QuanLyVatLieuXayDung.Models;
using System.ComponentModel;
using System.Windows.Data;


namespace QuanLyVatLieuXayDung.ViewModels
{
    public class PhieuNhapViewModel : BaseViewModel
    {
        private ObservableCollection<ChiTietPhieuNhap> _DSCTPhieuNhap;
        public ObservableCollection<ChiTietPhieuNhap> DSCTPhieuNhap
        {
            get => _DSCTPhieuNhap;
            set { _DSCTPhieuNhap = value; OnPropertyChanged(); }
        }

        private ICollectionView _CTPhieuNhapView;
        public ICollectionView CTPhieuNhapView
        {
            get => _CTPhieuNhapView;
            set { _CTPhieuNhapView = value; OnPropertyChanged(); }
        }

        private string _SearchKeyword;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set { _SearchKeyword = value; OnPropertyChanged(); CTPhieuNhapView?.Refresh(); }
        }

        private ObservableCollection<VatLieu> _DSVatLieu;
        public ObservableCollection<VatLieu> DSVatLieu
        {
            get => _DSVatLieu;
            set { _DSVatLieu = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PhieuNhap> _DSPhieuNhap;
        public ObservableCollection<PhieuNhap> DSPhieuNhap
        {
            get => _DSPhieuNhap;
            set { _DSPhieuNhap = value; OnPropertyChanged(); }
        }

        private ChiTietPhieuNhap _SelectedCTPN;
        public ChiTietPhieuNhap SelectedCTPN
        {
            get => _SelectedCTPN;
            set
            {
                _SelectedCTPN = value;
                OnPropertyChanged();
                if (_SelectedCTPN != null)
                {
                    IDPhieuNhap = SelectedCTPN.IDINput;
                    IDVatLieu = SelectedCTPN.IDObject;
                    Count = SelectedCTPN.Counts ?? 0;
                    PriceInput = SelectedCTPN.PriceInput ?? 0;
                    PriceOutput = SelectedCTPN.PriceOutput ?? 0;
                    var phieuNhap = DSPhieuNhap?.FirstOrDefault(p => p.ID == SelectedCTPN.IDINput);
                    if (phieuNhap != null)
                    {
                        DateInput = phieuNhap.DateInput;
                    }
                }
            }
        }

        private PhieuNhap _SelectedPN;
        public PhieuNhap SelectedPN
        {
            get => _SelectedPN;
            set { _SelectedPN = value; OnPropertyChanged(); }
        }

        private VatLieu _SelectedVL;
        public VatLieu SelectedVL
        {
            get => _SelectedVL;
            set { _SelectedVL = value; OnPropertyChanged(); }
        }

        private DateTime? _DateInput;
        public DateTime? DateInput
        {
            get => _DateInput;
            set { _DateInput = value; OnPropertyChanged(); }
        }

        private string _IDPhieuNhap;
        public string IDPhieuNhap
        {
            get => _IDPhieuNhap;
            set { _IDPhieuNhap = value; OnPropertyChanged(); }
        }

        private string _IDVatLieu;
        public string IDVatLieu
        {
            get => _IDVatLieu;
            set { _IDVatLieu = value; OnPropertyChanged(); }
        }

        private int _Count;
        public int Count
        {
            get => _Count;
            set { _Count = value; OnPropertyChanged(); }
        }

        private double _PriceInput;
        public double PriceInput
        {
            get => _PriceInput;
            set { _PriceInput = value; OnPropertyChanged(); }
        }

        private double _PriceOutput;
        public double PriceOutput
        {
            get => _PriceOutput;
            set { _PriceOutput = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public PhieuNhapViewModel()
        {
            DSPhieuNhap = new ObservableCollection<PhieuNhap>(DataProvider.Ins.DB.PhieuNhaps.ToList());
            DSVatLieu = new ObservableCollection<VatLieu>(DataProvider.Ins.DB.VatLieux.ToList());

            RefreshData();

            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            UpdateCommand = new RelayCommand<object>((p) => SelectedCTPN != null, (p) => Update());
            RemoveCommand = new RelayCommand<object>((p) => SelectedCTPN != null, (p) => Remove());
        }

        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.ChiTietPhieuNhaps
                        .Include("VatLieu")
                        .Include("PhieuNhap")
                        .ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSCTPhieuNhap = new ObservableCollection<ChiTietPhieuNhap>(list);

            CTPhieuNhapView = CollectionViewSource.GetDefaultView(DSCTPhieuNhap);
            CTPhieuNhapView.Filter = (obj) =>
            {
                var item = obj as ChiTietPhieuNhap;
                if (item == null) return false;
                if (string.IsNullOrWhiteSpace(SearchKeyword)) return true;
                string keyword = SearchKeyword.ToLower();
                return (item.ID != null && item.ID.ToLower().Contains(keyword)) ||
                       (item.IDINput != null && item.IDINput.ToLower().Contains(keyword));
            };
        }

        private string AutoCreateID()
        {
            var existingIDs = DataProvider.Ins.DB.ChiTietPhieuNhaps.Select(p => p.ID.Trim()).ToList();

            for (int i = 1; i <= 9999; i++)
            {
                string testID = "CTPN" + i.ToString("D3");
                if (!existingIDs.Contains(testID))
                {
                    return testID;
                }
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Chi tiết phiếu nhập!");
        }

        private void ClearFields()
        {
            SelectedPN = null;
            SelectedVL = null;
            IDPhieuNhap = string.Empty;
            IDVatLieu = string.Empty;
            Count = 0;
            PriceInput = 0;
            PriceOutput = 0;
            SelectedCTPN = null;
            DateInput = null;
        }

        #region CRUD Operations
        public void Add()
        {
            if (string.IsNullOrEmpty(IDPhieuNhap) || string.IsNullOrEmpty(IDVatLieu))
            {
                MessageBox.Show("Vui lòng chọn Mã phiếu nhập và Mã vật liệu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string nextID = AutoCreateID();
                var ctpn = new ChiTietPhieuNhap()
                {
                    ID = nextID,
                    IDINput = IDPhieuNhap,
                    IDObject = IDVatLieu,
                    Counts = Count,
                    PriceInput = PriceInput,
                    PriceOutput = PriceOutput
                };

                DataProvider.Ins.DB.ChiTietPhieuNhaps.Add(ctpn);

                // Lưu lại Lịch sử giá
                if (PriceOutput > 0)
                {
                    // Lấy giá cũ (giá lớn nhất trước đó của vật liệu này)
                    double giaCu = DataProvider.Ins.DB.ChiTietPhieuNhaps
                        .Where(c => c.IDObject == IDVatLieu && c.PriceOutput.HasValue)
                        .Select(c => c.PriceOutput.Value)
                        .DefaultIfEmpty(0)
                        .Max();

                    if (giaCu != PriceOutput)
                    {
                        var lichSu = new LichSuGiaVatLieu()
                        {
                            IDVatLieu = IDVatLieu,
                            GiaCu = giaCu,
                            GiaMoi = (double)PriceOutput,
                            NgayThayDoi = DateTime.Now,
                            NguoiThayDoi = "System"
                        };
                        DataProvider.Ins.DB.LichSuGiaVatLieux.Add(lichSu);
                    }
                }
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Thêm chi tiết phiếu nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                RefreshData();
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.InnerException?.Message 
                            ?? ex.InnerException?.Message 
                            ?? ex.Message;
                MessageBox.Show("Chi tiết lỗi: " + innerMsg, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(IDPhieuNhap) || string.IsNullOrEmpty(IDVatLieu))
            {
                MessageBox.Show("Vui lòng chọn Mã phiếu nhập và Mã vật liệu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var ctpn = DataProvider.Ins.DB.ChiTietPhieuNhaps.SingleOrDefault(p => p.ID == SelectedCTPN.ID);
                if (ctpn != null)
                {
                    // Tính giá cũ để so sánh
                    double giaCu = DataProvider.Ins.DB.ChiTietPhieuNhaps
                        .Where(c => c.IDObject == IDVatLieu && c.PriceOutput.HasValue && c.ID != ctpn.ID)
                        .Select(c => c.PriceOutput.Value)
                        .DefaultIfEmpty(0)
                        .Max();

                    ctpn.IDINput = IDPhieuNhap;
                    ctpn.IDObject = IDVatLieu;
                    ctpn.Counts = Count;
                    ctpn.PriceInput = PriceInput;
                    ctpn.PriceOutput = PriceOutput;

                    // Lưu lại Lịch sử giá nếu có sửa PriceOutput
                    if (PriceOutput > 0 && giaCu != PriceOutput)
                    {
                        var lichSu = new LichSuGiaVatLieu()
                        {
                            IDVatLieu = IDVatLieu,
                            GiaCu = giaCu,
                            GiaMoi = (double)PriceOutput,
                            NgayThayDoi = DateTime.Now,
                            NguoiThayDoi = "System"
                        };
                        DataProvider.Ins.DB.LichSuGiaVatLieux.Add(lichSu);
                    }

                    DataProvider.Ins.DB.SaveChanges();

                    MessageBox.Show("Cập nhật chi tiết phiếu nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Remove()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa chi tiết phiếu nhập này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var ctpn = DataProvider.Ins.DB.ChiTietPhieuNhaps.SingleOrDefault(p => p.ID == SelectedCTPN.ID);
                    if (ctpn != null)
                    {
                        // Remove from parent collections to prevent EF from trying to UPDATE FK to NULL
                        if (ctpn.PhieuNhap != null) ctpn.PhieuNhap.ChiTietPhieuNhaps.Remove(ctpn);
                        if (ctpn.VatLieu != null) ctpn.VatLieu.ChiTietPhieuNhaps.Remove(ctpn);

                        DataProvider.Ins.DB.ChiTietPhieuNhaps.Remove(ctpn);
                        DataProvider.Ins.DB.SaveChanges();

                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearFields();
                        RefreshData();
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.PropertyName + ": " + x.ErrorMessage);
                    string fullErrorMessage = string.Join("\n", errorMessages);
                    MessageBox.Show("Lỗi xác thực dữ liệu:\n" + fullErrorMessage, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                    MessageBox.Show("Lỗi chi tiết: " + msg, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}