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
        private ObservableCollection<TonKho> _DSTonKho;
        public ObservableCollection<TonKho> DSTonKho
        {
            get { return _DSTonKho; }
            set
            {
                if (value != _DSTonKho)
                {
                    _DSTonKho = value;
                    OnPropertyChanged(nameof(DSTonKho));
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

        private SeriesCollection _ChucVu;
        public SeriesCollection ChucVu
        {
            get { return _ChucVu; }
            set
            {
                if (_ChucVu != value)
                {
                    _ChucVu = value;
                    OnPropertyChanged(nameof(ChucVu));
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

        public TrangChuViewModel()
        {
            LoadTonKhoData();
            LoadRevenueChartData();
            LoadRolesChartData();
            LoadStatistics();
        }

        public void LoadStatistics()
        {
            try
            {
                SoNhanVien = DataProvider.Ins.DB.NguoiDungs.Count();
                SoKhachHang = DataProvider.Ins.DB.KhachHangs.Count();
                SoLoaiVatLieu = DataProvider.Ins.DB.LoaiVatLieux.Count();
                SoHoaDon = DataProvider.Ins.DB.PhieuXuats.Count();
            }
            catch (Exception)
            {
                SoNhanVien = 0;
                SoKhachHang = 0;
                SoLoaiVatLieu = 0;
                SoHoaDon = 0;
            }
        }

        public void LoadTonKhoData()
        {
            try
            {
                DSTonKho = new ObservableCollection<TonKho>();

                // Lấy danh sách Vật Liệu cùng Loại Vật Liệu trực tiếp từ database
                var VatLieuList = DataProvider.Ins.DB.VatLieux
                    .Include("LoaiVatLieu")
                    .ToList();

                // Tính tổng nhập theo từng vật liệu từ database
                var inputCounts = DataProvider.Ins.DB.ChiTietPhieuNhaps
                    .GroupBy(p => p.IDObject)
                    .Select(g => new { IDObject = g.Key, TotalInput = g.Sum(p => (int?)p.Counts) ?? 0 })
                    .ToDictionary(x => x.IDObject.Trim(), x => x.TotalInput);

                // Tính tổng xuất theo từng vật liệu từ database
                var outputCounts = DataProvider.Ins.DB.ChiTietPhieuXuats
                    .GroupBy(p => p.IDObject)
                    .Select(g => new { IDObject = g.Key, TotalOutput = g.Sum(p => (int?)p.Counts) ?? 0 })
                    .ToDictionary(x => x.IDObject.Trim(), x => x.TotalOutput);

                int j = 1;
                foreach (var i in VatLieuList)
                {
                    string key = i.ID.Trim();
                    int sumInput = inputCounts.ContainsKey(key) ? inputCounts[key] : 0;
                    int sumOutput = outputCounts.ContainsKey(key) ? outputCounts[key] : 0;

                    TonKho ton = new TonKho
                    {
                        STT = j,
                        Count = sumInput - sumOutput,
                        VatLieu = i,
                        LoaiVatLieu = i.LoaiVatLieu
                    };

                    DSTonKho.Add(ton);
                    j++;
                }
            }
            catch (Exception)
            {
                DSTonKho = new ObservableCollection<TonKho>();
            }
        }

        public void LoadRevenueChartData()
        {
            try
            {
                // Lấy chi tiết xuất kho phục vụ tính doanh thu
                var salesDetails = DataProvider.Ins.DB.ChiTietPhieuXuats
                    .Select(x => new
                    {
                        Date = x.PhieuXuat.DateOutput,
                        Counts = x.Counts ?? 0,
                        Price = x.Price ?? 0,
                        // Nếu giá xuất bằng 0 hoặc null, lấy giá xuất định mức (PriceOutput) từ chi tiết nhập gần nhất
                        FallbackPrice = x.VatLieu.ChiTietPhieuNhaps
                            .OrderByDescending(n => n.PhieuNhap.DateInput)
                            .Select(n => n.PriceOutput)
                            .FirstOrDefault() ?? 0
                    })
                    .ToList();

                var latestSale = DataProvider.Ins.DB.PhieuXuats.Max(p => p.DateOutput);
                DateTime endDate = latestSale ?? DateTime.Now;
                DateTime startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-11);

                var monthLabels = new List<string>();
                var revenueValues = new ChartValues<double>();

                for (int i = 0; i < 12; i++)
                {
                    var date = startDate.AddMonths(i);
                    monthLabels.Add(date.ToString("MM/yyyy"));

                    double monthlyRevenue = salesDetails
                        .Where(x => x.Date != null && x.Date.Value.Year == date.Year && x.Date.Value.Month == date.Month)
                        .Sum(x => x.Counts * (x.Price > 0 ? x.Price : x.FallbackPrice));

                    revenueValues.Add(monthlyRevenue);
                }

                Labels = monthLabels;
                Formatter = value => value.ToString("N0") + " VNĐ";

                DoanhThu = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Doanh thu",
                        Values = revenueValues,
                        DataLabels = true,
                        LabelPoint = point => point.Y.ToString("N0") + " VNĐ",
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

        public void LoadRolesChartData()
        {
            try
            {
                // Nhóm tài khoản nhân viên theo vai trò để vẽ biểu đồ tròn
                var rolesData = DataProvider.Ins.DB.NguoiDungs
                    .Include("NguoiDungRole")
                    .GroupBy(u => u.NguoiDungRole.DisplayName)
                    .Select(g => new { RoleName = g.Key, Count = g.Count() })
                    .ToList();

                ChucVu = new SeriesCollection();
                foreach (var item in rolesData)
                {
                    ChucVu.Add(new PieSeries
                    {
                        Title = item.RoleName,
                        Values = new ChartValues<int> { item.Count },
                        DataLabels = true,
                        LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
                    });
                }
            }
            catch (Exception)
            {
                ChucVu = new SeriesCollection();
            }
        }
    }
}