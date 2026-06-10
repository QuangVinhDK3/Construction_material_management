using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class TrangChuViewModel : BaseViewModel
    {
        private ObservableCollection<int> _ListYears;
        public ObservableCollection<int> ListYears
        {
            get => _ListYears;
            set { _ListYears = value; OnPropertyChanged(nameof(ListYears)); }
        }

        private int _SelectedYear;
        public int SelectedYear
        {
            get => _SelectedYear;
            set
            {
                if (_SelectedYear != value)
                {
                    _SelectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                    LoadRevenueChartData(); // Tải lại biểu đồ khi đổi năm
                }
            }
        }

        private ObservableCollection<KhachHang> _CongNoKhachHang;
        public ObservableCollection<KhachHang> CongNoKhachHang
        {
            get { return _CongNoKhachHang; }
            set
            {
                if (value != _CongNoKhachHang)
                {
                    _CongNoKhachHang = value;
                    OnPropertyChanged(nameof(CongNoKhachHang));
                }
            }
        }

        private SeriesCollection _DoanhThu;
        public SeriesCollection DoanhThu
        {
            get { return _DoanhThu; }
            set
            {
                if (_DoanhThu != value)
                {
                    _DoanhThu = value;
                    OnPropertyChanged(nameof(DoanhThu));
                }
            }
        }

        private List<string> _Labels;
        public List<string> Labels
        {
            get { return _Labels; }
            set
            {
                if (_Labels != value)
                {
                    _Labels = value;
                    OnPropertyChanged(nameof(Labels));
                }
            }
        }

        private Func<double, string> _Formatter;
        public Func<double, string> Formatter
        {
            get { return _Formatter; }
            set
            {
                if (_Formatter != value)
                {
                    _Formatter = value;
                    OnPropertyChanged(nameof(Formatter));
                }
            }
        }

        private SeriesCollection _TyLeLoaiVatLieu;
        public SeriesCollection TyLeLoaiVatLieu
        {
            get { return _TyLeLoaiVatLieu; }
            set
            {
                if (_TyLeLoaiVatLieu != value)
                {
                    _TyLeLoaiVatLieu = value;
                    OnPropertyChanged(nameof(TyLeLoaiVatLieu));
                }
            }
        }

        private int _SoNhanVien;
        public int SoNhanVien
        {
            get => _SoNhanVien;
            set { _SoNhanVien = value; OnPropertyChanged(); }
        }

        private int _SoKhachHang;
        public int SoKhachHang
        {
            get => _SoKhachHang;
            set { _SoKhachHang = value; OnPropertyChanged(); }
        }

        private int _SoLoaiVatLieu;
        public int SoLoaiVatLieu
        {
            get => _SoLoaiVatLieu;
            set { _SoLoaiVatLieu = value; OnPropertyChanged(); }
        }

        private int _SoHoaDon;
        public int SoHoaDon
        {
            get => _SoHoaDon;
            set { _SoHoaDon = value; OnPropertyChanged(); }
        }

        private string _TongDoanhThuStr;
        public string TongDoanhThuStr
        {
            get => _TongDoanhThuStr;
            set { _TongDoanhThuStr = value; OnPropertyChanged(); }
        }

        public TrangChuViewModel()
        {
            InitializeYears();
            LoadCongNoData();
            // LoadRevenueChartData() tự động được gọi khi SelectedYear thay đổi
            LoadTyLeLoaiVatLieuChartData();
            LoadStatistics();
        }

        private void InitializeYears()
        {
            try
            {
                var years = DataProvider.Ins.DB.PhieuXuats
                            .Where(x => x.DateOutput != null)
                            .Select(x => x.DateOutput.Value.Year)
                            .Distinct()
                            .OrderByDescending(y => y)
                            .ToList();
                if (years.Count == 0) years.Add(DateTime.Now.Year);
                
                ListYears = new ObservableCollection<int>(years);
                _SelectedYear = ListYears.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedYear));
                LoadRevenueChartData();
            }
            catch
            {
                ListYears = new ObservableCollection<int> { DateTime.Now.Year };
                _SelectedYear = DateTime.Now.Year;
                OnPropertyChanged(nameof(SelectedYear));
                LoadRevenueChartData();
            }
        }

        public void LoadStatistics()
        {
            try
            {
                SoNhanVien = DataProvider.Ins.DB.NguoiDungs.Count();
                SoKhachHang = DataProvider.Ins.DB.KhachHangs.Count();
                SoLoaiVatLieu = DataProvider.Ins.DB.LoaiVatLieux.Count();
                SoHoaDon = DataProvider.Ins.DB.PhieuXuats.Count();

                var allSalesDetails = DataProvider.Ins.DB.ChiTietPhieuXuats
                    .Where(x => x.PhieuXuat.Status != "Chưa xuất")
                    .Select(x => new
                    {
                        Counts = x.Counts ?? 0,
                        Price = x.Price ?? 0,
                        FallbackPrice = x.VatLieu.ChiTietPhieuNhaps
                            .OrderByDescending(n => n.PhieuNhap.DateInput)
                            .Select(n => n.PriceOutput)
                            .FirstOrDefault() ?? 0
                    })
                    .ToList();

                double tongDoanhThu = allSalesDetails.Sum(x => x.Counts * (x.Price > 0 ? x.Price : x.FallbackPrice));
                
                if (tongDoanhThu >= 1000000000) TongDoanhThuStr = (tongDoanhThu / 1000000000D).ToString("0.##") + " Tỷ";
                else if (tongDoanhThu >= 1000000) TongDoanhThuStr = (tongDoanhThu / 1000000D).ToString("0.##") + " Tr";
                else if (tongDoanhThu >= 1000) TongDoanhThuStr = (tongDoanhThu / 1000D).ToString("0.##") + " K";
                else TongDoanhThuStr = tongDoanhThu.ToString("N0") + " đ";
            }
            catch (Exception)
            {
                SoNhanVien = 0;
                SoKhachHang = 0;
                SoLoaiVatLieu = 0;
                SoHoaDon = 0;
                TongDoanhThuStr = "0 đ";
            }
        }

        public void LoadCongNoData()
        {
            try
            {
                var list = DataProvider.Ins.DB.KhachHangs
                    .Include("PhieuXuats.ChiTietPhieuXuats")
                    .ToList();
                
                // Chỉ lấy những khách hàng có công nợ > 0 và sắp xếp giảm dần
                var dsCongNo = list.Where(k => k.CongNoHienTai > 0)
                                   .OrderByDescending(k => k.CongNoHienTai)
                                   .ToList();

                for (int i = 0; i < dsCongNo.Count; i++)
                {
                    dsCongNo[i].STT = i + 1;
                }

                CongNoKhachHang = new ObservableCollection<KhachHang>(dsCongNo);
            }
            catch (Exception)
            {
                CongNoKhachHang = new ObservableCollection<KhachHang>();
            }
        }

        public void LoadRevenueChartData()
        {
            try
            {
                // Lấy chi tiết xuất kho trong năm được chọn để tối ưu hiệu suất
                var salesDetails = DataProvider.Ins.DB.ChiTietPhieuXuats
                    .Where(x => x.PhieuXuat.DateOutput != null && x.PhieuXuat.DateOutput.Value.Year == SelectedYear && x.PhieuXuat.Status != "Chưa xuất")
                    .Select(x => new
                    {
                        Date = x.PhieuXuat.DateOutput,
                        Counts = x.Counts ?? 0,
                        Price = x.Price ?? 0,
                        FallbackPrice = x.VatLieu.ChiTietPhieuNhaps
                            .OrderByDescending(n => n.PhieuNhap.DateInput)
                            .Select(n => n.PriceOutput)
                            .FirstOrDefault() ?? 0
                    })
                    .ToList();

                var monthLabels = new List<string>();
                var revenueValues = new ChartValues<double>();

                for (int i = 1; i <= 12; i++)
                {
                    monthLabels.Add("T" + i);

                    double monthlyRevenue = salesDetails
                        .Where(x => x.Date != null && x.Date.Value.Month == i)
                        .Sum(x => x.Counts * (x.Price > 0 ? x.Price : x.FallbackPrice));

                    revenueValues.Add(monthlyRevenue);
                }

                Labels = monthLabels;
                Formatter = value => 
                {
                    if (value >= 1000000000) return (value / 1000000000D).ToString("0.##") + " Tỷ";
                    if (value >= 1000000) return (value / 1000000D).ToString("0.##") + " Tr";
                    if (value >= 1000) return (value / 1000D).ToString("0.##") + " K";
                    return value.ToString("N0") + " đ";
                };

                DoanhThu = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Doanh thu",
                        Values = revenueValues,
                        DataLabels = true,
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 143, 0)),
                        MaxColumnWidth = 40,
                        LabelPoint = point => 
                        {
                            if (point.Y >= 1000000000) return (point.Y / 1000000000D).ToString("0.##") + " Tỷ";
                            if (point.Y >= 1000000) return (point.Y / 1000000D).ToString("0.##") + " Tr";
                            if (point.Y >= 1000) return (point.Y / 1000D).ToString("0.##") + " K";
                            return point.Y.ToString("N0") + " đ";
                        },
                        Foreground = System.Windows.Media.Brushes.White
                    }
                };
            }
            catch (Exception)
            {
                Labels = new List<string>();
                DoanhThu = new SeriesCollection();
                Formatter = value => value.ToString("N0") + " VNĐ";
            }
        }

        public void LoadTyLeLoaiVatLieuChartData()
        {
            try
            {
                // Nhóm vật liệu theo loại vật liệu để vẽ biểu đồ tròn
                var categoryData = DataProvider.Ins.DB.VatLieux
                    .Include("LoaiVatLieu")
                    .GroupBy(v => v.LoaiVatLieu.DisplayName)
                    .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                    .ToList();

                TyLeLoaiVatLieu = new SeriesCollection();
                foreach (var item in categoryData)
                {
                    TyLeLoaiVatLieu.Add(new PieSeries
                    {
                        Title = item.CategoryName,
                        Values = new ChartValues<int> { item.Count },
                        DataLabels = true,
                        LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
                    });
                }
            }
            catch (Exception)
            {
                TyLeLoaiVatLieu = new SeriesCollection();
            }
        }
    }
}