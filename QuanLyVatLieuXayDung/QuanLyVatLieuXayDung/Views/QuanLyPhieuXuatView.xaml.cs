using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Data.Entity;
using QuanLyVatLieuXayDung.ViewModels;
using QuanLyVatLieuXayDung.Models;

namespace QuanLyVatLieuXayDung.Views
{
    public partial class QuanLyPhieuXuatView : Window
    {
        public QuanLyPhieuXuatView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = ((ListViewItem)sender).Content as PhieuXuat;

            if (selectedItem != null)
            {
                var viewModel = DataContext as QuanLyPhieuXuatViewModel;
                if (viewModel != null)
                {
                    viewModel.SelectedPX = selectedItem;
                }
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as QuanLyPhieuXuatViewModel;
            if (vm == null || vm.SelectedPX == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu xuất (hóa đơn) trong danh sách để in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phieuXuat = DataProvider.Ins.DB.PhieuXuats
                .Include("ChiTietPhieuXuats.VatLieu")
                .Include("KhachHang")
                .Include("NguoiDung")
                .FirstOrDefault(p => p.ID == vm.SelectedPX.ID);
            if (phieuXuat == null) return;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CreateInvoiceDocument(phieuXuat);
                doc.Name = "FlowDoc";
                
                // Cấu hình kích thước trang in để WPF biết cách dàn trang
                doc.PageHeight = printDialog.PrintableAreaHeight > 0 ? printDialog.PrintableAreaHeight : 1122; // A4 Height
                doc.PageWidth = printDialog.PrintableAreaWidth > 0 ? printDialog.PrintableAreaWidth : 793;   // A4 Width
                doc.ColumnWidth = doc.PageWidth;

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Hóa Đơn - " + phieuXuat.ID);
            }
        }

        private FlowDocument CreateInvoiceDocument(PhieuXuat px)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new FontFamily("Arial");

            Paragraph pHeader = new Paragraph(new Run("CỬA HÀNG VẬT LIỆU XÂY DỰNG 3V"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            doc.Blocks.Add(pHeader);

            Paragraph pTitle = new Paragraph(new Run("HÓA ĐƠN BÁN HÀNG"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(pTitle);

            Paragraph pInfo = new Paragraph();
            pInfo.Inlines.Add(new Run($"Mã Hóa Đơn: {px.ID}\n") { FontWeight = FontWeights.Bold });
            pInfo.Inlines.Add(new Run($"Ngày: {px.DateOutput:dd/MM/yyyy HH:mm}\n"));
            pInfo.Inlines.Add(new Run($"Khách hàng: {px.KhachHang?.DisplayName}\n"));
            pInfo.Inlines.Add(new Run($"Địa chỉ: {px.KhachHang?.Address}\n"));
            pInfo.Margin = new Thickness(0, 0, 0, 20);
            doc.Blocks.Add(pInfo);

            Table table = new Table();
            table.CellSpacing = 0;
            table.Columns.Add(new TableColumn() { Width = new GridLength(50) }); 
            table.Columns.Add(new TableColumn() { Width = new GridLength(200) }); 
            table.Columns.Add(new TableColumn() { Width = new GridLength(80) }); 
            table.Columns.Add(new TableColumn() { Width = new GridLength(120) }); 
            table.Columns.Add(new TableColumn() { Width = new GridLength(150) }); 

            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow();
            headerRow.Background = Brushes.LightGray;
            headerRow.FontWeight = FontWeights.Bold;

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("STT"))) { Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Tên vật liệu"))) { Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("SL"))) { Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Đơn giá"))) { Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Thành tiền"))) { Padding = new Thickness(5) });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            TableRowGroup dataGroup = new TableRowGroup();
            int stt = 1;
            double tongTien = 0;

            if (px.ChiTietPhieuXuats != null)
            {
                foreach (var item in px.ChiTietPhieuXuats)
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(stt.ToString()))) { Padding = new Thickness(5) });
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.VatLieu?.DisplayName))) { Padding = new Thickness(5) });
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.Counts.ToString()))) { Padding = new Thickness(5) });
                    row.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("{0:N0}", item.Price)))) { Padding = new Thickness(5) });
                    
                    double thanhTienItem = (item.Counts ?? 0) * (item.Price ?? 0);
                    tongTien += thanhTienItem;
                    
                    row.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("{0:N0}", thanhTienItem)))) { Padding = new Thickness(5) });
                    
                    dataGroup.Rows.Add(row);
                    stt++;
                }
            }
            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Paragraph pTotal = new Paragraph();
            pTotal.Inlines.Add(new Run($"Tổng cộng: {tongTien:N0} VNĐ\n"));
            if ((px.ChietKhau ?? 0) > 0)
            {
                pTotal.Inlines.Add(new Run($"Chiết khấu: -{px.ChietKhau:N0} VNĐ\n") { Foreground = Brushes.OrangeRed });
            }
            double thucThu = tongTien - (px.ChietKhau ?? 0);
            pTotal.Inlines.Add(new Run($"Thành tiền (Đã trừ CK): {thucThu:N0} VNĐ\n") { FontWeight = FontWeights.Bold });
            pTotal.Inlines.Add(new Run($"Khách đã trả: {px.SoTienDaThanhToan:N0} VNĐ\n"));
            pTotal.Inlines.Add(new Run($"Còn nợ: {thucThu - (px.SoTienDaThanhToan ?? 0):N0} VNĐ\n") { FontWeight = FontWeights.Bold, Foreground = Brushes.Red });
            pTotal.TextAlignment = TextAlignment.Right;
            pTotal.Margin = new Thickness(0, 20, 0, 0);
            doc.Blocks.Add(pTotal);

            Table footerTable = new Table();
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            TableRowGroup footerGroup = new TableRowGroup();
            TableRow footerRow = new TableRow();
            
            Paragraph pKys = new Paragraph(new Run("Người mua hàng\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            Paragraph pBan = new Paragraph(new Run("Người bán hàng\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            
            footerRow.Cells.Add(new TableCell(pKys));
            footerRow.Cells.Add(new TableCell(pBan));
            footerGroup.Rows.Add(footerRow);
            footerTable.RowGroups.Add(footerGroup);
            footerTable.Margin = new Thickness(0, 30, 0, 0);
            
            doc.Blocks.Add(footerTable);

            return doc;
        }
    }
}
