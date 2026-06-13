using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity;
using System.Linq;
namespace QuanLyVatLieuXayDung.Views
{
    /// <summary>
    /// Interaction logic for QuanLyPhieuThuView.xaml
    /// </summary>
    public partial class QuanLyPhieuThuView : Window
    {
        public QuanLyPhieuThuView()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as QuanLyVatLieuXayDung.ViewModels.PhieuThuViewModel;
            if (vm == null || vm.SelectedPhieuThu == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu thu trong danh sách để in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phieuThu = QuanLyVatLieuXayDung.Models.DataProvider.Ins.DB.PhieuThus
                .Include(p => p.KhachHang)
                .Include(p => p.NguoiDung)
                .FirstOrDefault(p => p.ID == vm.SelectedPhieuThu.ID);
            if (phieuThu == null) return;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CreatePhieuThuDocument(phieuThu);
                doc.Name = "FlowDoc";
                
                doc.PageHeight = printDialog.PrintableAreaHeight > 0 ? printDialog.PrintableAreaHeight : 1122; // A4
                doc.PageWidth = printDialog.PrintableAreaWidth > 0 ? printDialog.PrintableAreaWidth : 793;   // A4
                doc.ColumnWidth = doc.PageWidth;

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Phiếu Thu - " + phieuThu.ID);
            }
        }

        private FlowDocument CreatePhieuThuDocument(QuanLyVatLieuXayDung.Models.PhieuThu pt)
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

            Paragraph pTitle = new Paragraph(new Run("PHIẾU THU TIỀN"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            };
            doc.Blocks.Add(pTitle);

            Paragraph pInfo = new Paragraph();
            pInfo.LineHeight = 30;
            pInfo.Inlines.Add(new Run($"Mã phiếu: {pt.ID}\n") { FontWeight = FontWeights.Bold });
            pInfo.Inlines.Add(new Run($"Ngày thu: {pt.NgayThu:dd/MM/yyyy HH:mm}\n"));
            pInfo.Inlines.Add(new Run($"Họ tên người nộp tiền (Khách hàng): {pt.KhachHang?.DisplayName}\n"));
            pInfo.Inlines.Add(new Run($"Địa chỉ: {pt.KhachHang?.Address}\n"));
            pInfo.Inlines.Add(new Run($"Lý do nộp: {pt.NoiDung}\n"));
            pInfo.Inlines.Add(new Run($"Số tiền thu: {pt.SoTien:N0} VNĐ\n") { FontWeight = FontWeights.Bold, FontSize = 16 });
            pInfo.Margin = new Thickness(0, 0, 0, 30);
            doc.Blocks.Add(pInfo);

            Table footerTable = new Table();
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            TableRowGroup footerGroup = new TableRowGroup();
            TableRow footerRow = new TableRow();
            
            Paragraph pKys = new Paragraph(new Run("Người nộp tiền\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            Paragraph pBan = new Paragraph(new Run("Người thu tiền\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            
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
