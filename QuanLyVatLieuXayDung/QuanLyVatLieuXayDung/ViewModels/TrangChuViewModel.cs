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
using OfficeOpenXml;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;

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

        private ObservableCollection<PhieuXuat> _DanhSachNoQuaHan;
        public ObservableCollection<PhieuXuat> DanhSachNoQuaHan
        {
            get { return _DanhSachNoQuaHan; }
            set
            {
                if (value != _DanhSachNoQuaHan)
                {
                    _DanhSachNoQuaHan = value;
                    OnPropertyChanged(nameof(DanhSachNoQuaHan));
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

        private string _TongThuStr;
        public string TongThuStr
        {
            get => _TongThuStr;
            set { _TongThuStr = value; OnPropertyChanged(); }
        }

        private string _TongChiStr;
        public string TongChiStr
        {
            get => _TongChiStr;
            set { _TongChiStr = value; OnPropertyChanged(); }
        }

        private ObservableCollection<VatLieu> _CanhBaoTonKho;
        public ObservableCollection<VatLieu> CanhBaoTonKho
        {
            get => _CanhBaoTonKho;
            set { _CanhBaoTonKho = value; OnPropertyChanged(); }
        }

        private ObservableCollection<VatLieu> _HangUDong;
        public ObservableCollection<VatLieu> HangUDong
        {
            get => _HangUDong;
            set { _HangUDong = value; OnPropertyChanged(); }
        }

        public static Action RefreshDashboardData;

        public ICommand ExportExcelCommand { get; set; }

        public TrangChuViewModel()
        {
            RefreshDashboardData = () => 
            {
                LoadCongNoData();
                LoadTyLeLoaiVatLieuChartData();
                LoadStatistics();
                // We also call InitializeYears or LoadRevenueChartData if needed, but year data doesn't change as often.
                LoadRevenueChartData();
                LoadCanhBaoKho();
            };

            InitializeYears();
            LoadCongNoData();
            // LoadRevenueChartData() tự động được gọi khi SelectedYear thay đổi
            LoadTyLeLoaiVatLieuChartData();
            LoadStatistics();
            LoadCanhBaoKho();

            ExportExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportExcel());
        }

        private void ExportExcel()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "Excel Workbook|*.xlsx",
                    Title = "Lưu báo cáo Excel",
                    FileName = $"BaoCao_Dashboard_Năm_{SelectedYear}.xlsx"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        // Sheet 1: Doanh Thu
                        var wsDoanhThu = package.Workbook.Worksheets.Add("Doanh Thu");
                        wsDoanhThu.Cells[1, 1].Value = $"BÁO CÁO DOANH THU NĂM {SelectedYear}";
                        wsDoanhThu.Cells[1, 1, 1, 4].Merge = true;
                        wsDoanhThu.Cells[1, 1].Style.Font.Bold = true;
                        wsDoanhThu.Cells[1, 1].Style.Font.Size = 14;

                        wsDoanhThu.Cells[3, 1].Value = "Tháng";
                        wsDoanhThu.Cells[3, 2].Value = "Doanh thu (VNĐ)";
                        wsDoanhThu.Cells[3, 1, 3, 2].Style.Font.Bold = true;

                        if (DoanhThu != null && DoanhThu.Count > 0)
                        {
                            var values = DoanhThu[0].Values;
                            for (int i = 0; i < values.Count; i++)
                            {
                                wsDoanhThu.Cells[i + 4, 1].Value = $"Tháng {i + 1}";
                                wsDoanhThu.Cells[i + 4, 2].Value = values[i];
                                wsDoanhThu.Cells[i + 4, 2].Style.Numberformat.Format = "#,##0";
                            }
                        }
                        wsDoanhThu.Cells.AutoFitColumns();

                        // Sheet 2: Tồn Kho & Ứ Đọng
                        var wsTonKho = package.Workbook.Worksheets.Add("Tồn Kho");
                        wsTonKho.Cells[1, 1].Value = "CẢNH BÁO SẮP HẾT HÀNG";
                        wsTonKho.Cells[1, 1].Style.Font.Bold = true;
                        wsTonKho.Cells[2, 1].Value = "Tên vật liệu";
                        wsTonKho.Cells[2, 2].Value = "Tồn kho";
                        wsTonKho.Cells[2, 1, 2, 2].Style.Font.Bold = true;

                        int row = 3;
                        if (CanhBaoTonKho != null)
                        {
                            foreach (var item in CanhBaoTonKho)
                            {
                                wsTonKho.Cells[row, 1].Value = item.DisplayName;
                                wsTonKho.Cells[row, 2].Value = item.SoLuongTon;
                                row++;
                            }
                        }

                        row += 2;
                        wsTonKho.Cells[row, 1].Value = "HÀNG Ứ ĐỌNG TRONG KHO";
                        wsTonKho.Cells[row, 1].Style.Font.Bold = true;
                        wsTonKho.Cells[row + 1, 1].Value = "Tên vật liệu";
                        wsTonKho.Cells[row + 1, 2].Value = "Tồn kho";
                        wsTonKho.Cells[row + 1, 3].Value = "Số ngày ứ đọng";
                        wsTonKho.Cells[row + 1, 1, row + 1, 3].Style.Font.Bold = true;

                        row += 2;
                        if (HangUDong != null)
                        {
                            foreach (var item in HangUDong)
                            {
                                wsTonKho.Cells[row, 1].Value = item.DisplayName;
                                wsTonKho.Cells[row, 2].Value = item.SoLuongTon;
                                wsTonKho.Cells[row, 3].Value = item.SoNgayUDong;
                                row++;
                            }
                        }
                        wsTonKho.Cells.AutoFitColumns();

                        // Sheet 3: Công Nợ Khách Hàng
                        var wsCongNo = package.Workbook.Worksheets.Add("Công Nợ");
                        wsCongNo.Cells[1, 1].Value = "DANH SÁCH CÔNG NỢ KHÁCH HÀNG";
                        wsCongNo.Cells[1, 1, 1, 4].Merge = true;
                        wsCongNo.Cells[1, 1].Style.Font.Bold = true;
                        wsCongNo.Cells[1, 1].Style.Font.Size = 14;

                        wsCongNo.Cells[3, 1].Value = "Tên khách hàng";
                        wsCongNo.Cells[3, 2].Value = "Điện thoại";
                        wsCongNo.Cells[3, 3].Value = "Địa chỉ";
                        wsCongNo.Cells[3, 4].Value = "Tổng nợ (VNĐ)";
                        wsCongNo.Cells[3, 1, 3, 4].Style.Font.Bold = true;

                        row = 4;
                        if (CongNoKhachHang != null)
                        {
                            foreach (var item in CongNoKhachHang)
                            {
                                wsCongNo.Cells[row, 1].Value = item.DisplayName;
                                wsCongNo.Cells[row, 2].Value = item.Phone;
                                wsCongNo.Cells[row, 3].Value = item.Address;
                                wsCongNo.Cells[row, 4].Value = item.CongNoHienTai;
                                wsCongNo.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                                row++;
                            }
                        }
                        wsCongNo.Cells.AutoFitColumns();

                        // Sheet 4: Toàn Bộ Tồn Kho
                        var wsAllTonKho = package.Workbook.Worksheets.Add("Tồn Kho Tổng");
                        wsAllTonKho.Cells[1, 1].Value = "DANH SÁCH TẤT CẢ VẬT LIỆU TRONG KHO";
                        wsAllTonKho.Cells[1, 1, 1, 3].Merge = true;
                        wsAllTonKho.Cells[1, 1].Style.Font.Bold = true;
                        wsAllTonKho.Cells[1, 1].Style.Font.Size = 14;

                        wsAllTonKho.Cells[3, 1].Value = "Tên vật liệu";
                        wsAllTonKho.Cells[3, 2].Value = "Đơn vị tính";
                        wsAllTonKho.Cells[3, 3].Value = "Số lượng tồn";
                        wsAllTonKho.Cells[3, 1, 3, 3].Style.Font.Bold = true;

                        row = 4;
                        var allVatLieu = DataProvider.Ins.DB.VatLieux.ToList();
                        foreach (var item in allVatLieu)
                        {
                            wsAllTonKho.Cells[row, 1].Value = item.DisplayName;
                            wsAllTonKho.Cells[row, 2].Value = item.DonViTinh;
                            wsAllTonKho.Cells[row, 3].Value = item.SoLuongTon;
                            row++;
                        }
                        wsAllTonKho.Cells.AutoFitColumns();

                        // Sheet 5: Thu Chi
                        var wsThuChi = package.Workbook.Worksheets.Add("Thu Chi");
                        wsThuChi.Cells[1, 1].Value = $"BÁO CÁO THU CHI NĂM {SelectedYear}";
                        wsThuChi.Cells[1, 1, 1, 4].Merge = true;
                        wsThuChi.Cells[1, 1].Style.Font.Bold = true;
                        wsThuChi.Cells[1, 1].Style.Font.Size = 14;

                        wsThuChi.Cells[3, 1].Value = "PHIẾU THU";
                        wsThuChi.Cells[3, 1].Style.Font.Bold = true;
                        wsThuChi.Cells[4, 1].Value = "Mã phiếu";
                        wsThuChi.Cells[4, 2].Value = "Ngày tạo";
                        wsThuChi.Cells[4, 3].Value = "Số tiền (VNĐ)";
                        wsThuChi.Cells[4, 1, 4, 3].Style.Font.Bold = true;

                        row = 5;
                        var phieuThus = DataProvider.Ins.DB.PhieuThus.Where(x => x.NgayThu.HasValue && x.NgayThu.Value.Year == SelectedYear).ToList();
                        double tongThu = 0;
                        foreach (var item in phieuThus)
                        {
                            wsThuChi.Cells[row, 1].Value = item.ID;
                            wsThuChi.Cells[row, 2].Value = item.NgayThu.Value.ToString("dd/MM/yyyy");
                            wsThuChi.Cells[row, 3].Value = item.SoTien;
                            wsThuChi.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                            tongThu += item.SoTien ?? 0;
                            row++;
                        }
                        wsThuChi.Cells[row, 2].Value = "Tổng thu:";
                        wsThuChi.Cells[row, 2].Style.Font.Bold = true;
                        wsThuChi.Cells[row, 3].Value = tongThu;
                        wsThuChi.Cells[row, 3].Style.Font.Bold = true;
                        wsThuChi.Cells[row, 3].Style.Numberformat.Format = "#,##0";

                        row += 3;
                        wsThuChi.Cells[row, 1].Value = "PHIẾU CHI";
                        wsThuChi.Cells[row, 1].Style.Font.Bold = true;
                        wsThuChi.Cells[row + 1, 1].Value = "Mã phiếu";
                        wsThuChi.Cells[row + 1, 2].Value = "Ngày tạo";
                        wsThuChi.Cells[row + 1, 3].Value = "Số tiền (VNĐ)";
                        wsThuChi.Cells[row + 1, 1, row + 1, 3].Style.Font.Bold = true;

                        row += 2;
                        var phieuChis = DataProvider.Ins.DB.PhieuChis.Where(x => x.NgayChi.HasValue && x.NgayChi.Value.Year == SelectedYear).ToList();
                        double tongChi = 0;
                        foreach (var item in phieuChis)
                        {
                            wsThuChi.Cells[row, 1].Value = item.ID;
                            wsThuChi.Cells[row, 2].Value = item.NgayChi.Value.ToString("dd/MM/yyyy");
                            wsThuChi.Cells[row, 3].Value = item.SoTien;
                            wsThuChi.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                            tongChi += item.SoTien ?? 0;
                            row++;
                        }
                        wsThuChi.Cells[row, 2].Value = "Tổng chi:";
                        wsThuChi.Cells[row, 2].Style.Font.Bold = true;
                        wsThuChi.Cells[row, 3].Value = tongChi;
                        wsThuChi.Cells[row, 3].Style.Font.Bold = true;
                        wsThuChi.Cells[row, 3].Style.Numberformat.Format = "#,##0";

                        row += 2;
                        wsThuChi.Cells[row, 2].Value = "LỢI NHUẬN (Thu - Chi):";
                        wsThuChi.Cells[row, 2].Style.Font.Bold = true;
                        wsThuChi.Cells[row, 3].Value = tongThu - tongChi;
                        wsThuChi.Cells[row, 3].Style.Font.Bold = true;
                        wsThuChi.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                        wsThuChi.Cells.AutoFitColumns();

                        // Sheet 6: Công Nợ Nhà Cung Cấp
                        var wsCongNoNCC = package.Workbook.Worksheets.Add("Công Nợ NCC");
                        wsCongNoNCC.Cells[1, 1].Value = "DANH SÁCH CÔNG NỢ NHÀ CUNG CẤP";
                        wsCongNoNCC.Cells[1, 1, 1, 4].Merge = true;
                        wsCongNoNCC.Cells[1, 1].Style.Font.Bold = true;
                        wsCongNoNCC.Cells[1, 1].Style.Font.Size = 14;

                        wsCongNoNCC.Cells[3, 1].Value = "Tên nhà cung cấp";
                        wsCongNoNCC.Cells[3, 2].Value = "Điện thoại";
                        wsCongNoNCC.Cells[3, 3].Value = "Địa chỉ";
                        wsCongNoNCC.Cells[3, 4].Value = "Tổng nợ (VNĐ)";
                        wsCongNoNCC.Cells[3, 1, 3, 4].Style.Font.Bold = true;

                        row = 4;
                        var nccs = DataProvider.Ins.DB.NhaCungCaps.Include(x => x.PhieuNhaps).Include(x => x.PhieuChis).ToList();
                        var listNoNCC = nccs.Where(x => x.CongNoHienTai > 0).OrderByDescending(x => x.CongNoHienTai).ToList();
                        foreach (var item in listNoNCC)
                        {
                            wsCongNoNCC.Cells[row, 1].Value = item.DisplayName;
                            wsCongNoNCC.Cells[row, 2].Value = item.Phone;
                            wsCongNoNCC.Cells[row, 3].Value = item.Address;
                            wsCongNoNCC.Cells[row, 4].Value = item.CongNoHienTai;
                            wsCongNoNCC.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                            row++;
                        }
                        wsCongNoNCC.Cells.AutoFitColumns();

                        FileInfo fi = new FileInfo(sfd.FileName);
                        package.SaveAs(fi);
                        
                        MessageBoxResult result = MessageBox.Show("Xuất báo cáo thành công! Bạn có muốn mở file ngay không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                double doanhThuBanHang = allSalesDetails.Sum(x => x.Counts * (x.Price > 0 ? x.Price : x.FallbackPrice));
                
                // Người dùng muốn tính luôn phần thu (Sổ quỹ) và chi (Sổ quỹ) vào tổng doanh thu
                double tongPhieuThu = DataProvider.Ins.DB.PhieuThus.Sum(p => p.SoTien) ?? 0;
                double tongPhieuChi = DataProvider.Ins.DB.PhieuChis.Sum(p => p.SoTien) ?? 0;

                double tongDoanhThu = doanhThuBanHang + tongPhieuThu - tongPhieuChi;
                
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

                // Tính toán Nợ quá hạn (>30 ngày)
                var noQuaHanList = new List<PhieuXuat>();
                DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

                foreach (var kh in dsCongNo)
                {
                    // Lấy tổng số tiền đã thu từ khách hàng này bằng Phiếu Thu
                    var tongDaThu = DataProvider.Ins.DB.PhieuThus.Where(pt => pt.IDKhachHang == kh.ID).Sum(pt => pt.SoTien) ?? 0;

                    // Lấy các phiếu xuất có nợ của khách hàng này, sắp xếp từ cũ nhất đến mới nhất
                    var pxList = kh.PhieuXuats.Where(px => px.ConNo > 0).OrderBy(px => px.DateOutput).ToList();

                    foreach (var px in pxList)
                    {
                        double nợCủaPhiếuNày = px.ConNo;
                        if (tongDaThu > 0)
                        {
                            if (tongDaThu >= nợCủaPhiếuNày)
                            {
                                // Phiếu này đã được trả hết bằng Phiếu Thu
                                tongDaThu -= nợCủaPhiếuNày;
                                nợCủaPhiếuNày = 0;
                            }
                            else
                            {
                                // Phiếu này chỉ được trả 1 phần
                                nợCủaPhiếuNày -= tongDaThu;
                                tongDaThu = 0;
                            }
                        }

                        // Nếu sau khi cấn trừ bằng Phiếu Thu mà vẫn còn nợ, và phiếu này đã quá 30 ngày
                        if (nợCủaPhiếuNày > 0 && px.DateOutput <= thirtyDaysAgo)
                        {
                            // Ta tạm gán lại ConNo = số tiền còn nợ thực tế để hiển thị trên UI
                            var pxNoQuaHan = new PhieuXuat
                            {
                                ID = px.ID,
                                KhachHang = kh,
                                IDCustomer = px.IDCustomer,
                                DateOutput = px.DateOutput,
                                Status = px.Status,
                                Total = px.Total,
                                // Dùng thuộc tính SoTienDaThanhToan lưu trữ tạm ConNo (thực tế) để hiển thị
                                SoTienDaThanhToan = nợCủaPhiếuNày,
                                NguoiDung = px.NguoiDung
                            };
                            noQuaHanList.Add(pxNoQuaHan);
                        }
                    }
                }

                DanhSachNoQuaHan = new ObservableCollection<PhieuXuat>(noQuaHanList.OrderByDescending(p => p.SoTienDaThanhToan));
            }
            catch (Exception)
            {
                CongNoKhachHang = new ObservableCollection<KhachHang>();
            }
        }

        public void LoadCanhBaoKho()
        {
            try
            {
                var listVatLieu = DataProvider.Ins.DB.VatLieux.ToList();

                var tongNhapDict = DataProvider.Ins.DB.ChiTietPhieuNhaps
                    .GroupBy(c => c.IDObject)
                    .Select(g => new { IDObject = g.Key, TongNhap = g.Sum(c => c.Counts) })
                    .ToDictionary(x => x.IDObject.Trim(), x => x.TongNhap ?? 0);

                var tongXuatDict = DataProvider.Ins.DB.ChiTietPhieuXuats
                    .GroupBy(c => c.IDObject)
                    .Select(g => new { IDObject = g.Key, TongXuat = g.Sum(c => c.Counts) })
                    .ToDictionary(x => x.IDObject.Trim(), x => x.TongXuat ?? 0);

                var ngayXuatGiaoDichCuoiDict = DataProvider.Ins.DB.ChiTietPhieuXuats
                    .Where(c => c.PhieuXuat.DateOutput != null)
                    .GroupBy(c => c.IDObject)
                    .Select(g => new { IDObject = g.Key, LastDate = g.Max(c => c.PhieuXuat.DateOutput) })
                    .ToDictionary(x => x.IDObject.Trim(), x => x.LastDate);
                
                var ngayNhapGiaoDichCuoiDict = DataProvider.Ins.DB.ChiTietPhieuNhaps
                    .Where(c => c.PhieuNhap.DateInput != null)
                    .GroupBy(c => c.IDObject)
                    .Select(g => new { IDObject = g.Key, LastDate = g.Max(c => c.PhieuNhap.DateInput) })
                    .ToDictionary(x => x.IDObject.Trim(), x => x.LastDate);

                var dsSapHet = new List<VatLieu>();
                var dsUDong = new List<VatLieu>();

                DateTime now = DateTime.Now;

                foreach (var vl in listVatLieu)
                {
                    string id = vl.ID.Trim();
                    int tongNhap = tongNhapDict.ContainsKey(id) ? tongNhapDict[id] : 0;
                    int tongXuat = tongXuatDict.ContainsKey(id) ? tongXuatDict[id] : 0;
                    vl.SoLuongTon = tongNhap - tongXuat;

                    // Lọc hàng sắp hết
                    if (vl.SoLuongTon <= 10)
                    {
                        dsSapHet.Add(vl);
                    }

                    // Lọc hàng ứ đọng (Tồn kho > 0 và quá 3 tháng chưa bán được)
                    if (vl.SoLuongTon > 0)
                    {
                        DateTime? lastDate = null;
                        if (ngayXuatGiaoDichCuoiDict.ContainsKey(id))
                        {
                            lastDate = ngayXuatGiaoDichCuoiDict[id];
                        }
                        else if (ngayNhapGiaoDichCuoiDict.ContainsKey(id))
                        {
                            lastDate = ngayNhapGiaoDichCuoiDict[id];
                        }

                        if (lastDate.HasValue)
                        {
                            TimeSpan diff = now - lastDate.Value;
                            if (diff.TotalDays > 90)
                            {
                                vl.SoNgayUDong = (int)diff.TotalDays;
                                dsUDong.Add(vl);
                            }
                        }
                    }
                }

                dsSapHet = dsSapHet.OrderBy(x => x.SoLuongTon).ToList();
                for (int i = 0; i < dsSapHet.Count; i++) dsSapHet[i].STT = i + 1;
                CanhBaoTonKho = new ObservableCollection<VatLieu>(dsSapHet);

                dsUDong = dsUDong.OrderByDescending(x => x.SoNgayUDong).ToList();
                for (int i = 0; i < dsUDong.Count; i++) dsUDong[i].STT = i + 1;
                HangUDong = new ObservableCollection<VatLieu>(dsUDong);
            }
            catch (Exception ex)
            {
                CanhBaoTonKho = new ObservableCollection<VatLieu>();
                HangUDong = new ObservableCollection<VatLieu>();
            }
        }

        public void LoadRevenueChartData()
        {
            try
            {
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

                var phieuThuList = DataProvider.Ins.DB.PhieuThus
                    .Where(x => x.NgayThu != null && x.NgayThu.Value.Year == SelectedYear)
                    .Select(x => new { Date = x.NgayThu, SoTien = x.SoTien ?? 0 })
                    .ToList();

                var phieuChiList = DataProvider.Ins.DB.PhieuChis
                    .Where(x => x.NgayChi != null && x.NgayChi.Value.Year == SelectedYear)
                    .Select(x => new { Date = x.NgayChi, SoTien = x.SoTien ?? 0 })
                    .ToList();

                var monthLabels = new List<string>();
                var revenueValues = new ChartValues<double>();

                for (int i = 1; i <= 12; i++)
                {
                    monthLabels.Add("T" + i);

                    double doanhThuBanHang = salesDetails
                        .Where(x => x.Date != null && x.Date.Value.Month == i)
                        .Sum(x => x.Counts * (x.Price > 0 ? x.Price : x.FallbackPrice));

                    double tongPhieuThu = phieuThuList
                        .Where(x => x.Date != null && x.Date.Value.Month == i)
                        .Sum(x => x.SoTien);

                    double tongPhieuChi = phieuChiList
                        .Where(x => x.Date != null && x.Date.Value.Month == i)
                        .Sum(x => x.SoTien);

                    double monthlyRevenue = doanhThuBanHang + tongPhieuThu - tongPhieuChi;

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