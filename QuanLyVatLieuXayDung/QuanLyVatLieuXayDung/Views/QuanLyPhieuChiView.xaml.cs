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
    /// Interaction logic for QuanLyPhieuChiView.xaml
    /// </summary>
    public partial class QuanLyPhieuChiView : Window
    {
        public QuanLyPhieuChiView()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as QuanLyVatLieuXayDung.ViewModels.PhieuChiViewModel;
            if (vm == null || vm.SelectedPhieuChi == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu chi trong danh sách để in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phieuChi = QuanLyVatLieuXayDung.Models.DataProvider.Ins.DB.PhieuChis
                .Include(p => p.NhaCungCap)
                .Include(p => p.NguoiDung)
                .FirstOrDefault(p => p.ID == vm.SelectedPhieuChi.ID);
            if (phieuChi == null) return;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CreatePhieuChiDocument(phieuChi);
                doc.Name = "FlowDoc";
                
                doc.PageHeight = printDialog.PrintableAreaHeight > 0 ? printDialog.PrintableAreaHeight : 1122; // A4
                doc.PageWidth = printDialog.PrintableAreaWidth > 0 ? printDialog.PrintableAreaWidth : 793;   // A4
                doc.ColumnWidth = doc.PageWidth;

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Phiếu Chi - " + phieuChi.ID);
            }
        }

        private FlowDocument CreatePhieuChiDocument(QuanLyVatLieuXayDung.Models.PhieuChi pc)
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

            Paragraph pTitle = new Paragraph(new Run("PHIẾU CHI TIỀN"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            };
            doc.Blocks.Add(pTitle);

            Paragraph pInfo = new Paragraph();
            pInfo.LineHeight = 30;
            pInfo.Inlines.Add(new Run($"Mã phiếu: {pc.ID}\n") { FontWeight = FontWeights.Bold });
            pInfo.Inlines.Add(new Run($"Ngày chi: {pc.NgayChi:dd/MM/yyyy HH:mm}\n"));
            pInfo.Inlines.Add(new Run($"Người/Đơn vị nhận tiền: {pc.NhaCungCap?.DisplayName}\n"));
            pInfo.Inlines.Add(new Run($"Địa chỉ: {pc.NhaCungCap?.Address}\n"));
            pInfo.Inlines.Add(new Run($"Lý do chi: {pc.NoiDung}\n"));
            pInfo.Inlines.Add(new Run($"Số tiền chi: {pc.SoTien:N0} VNĐ\n") { FontWeight = FontWeights.Bold, FontSize = 16 });
            pInfo.Margin = new Thickness(0, 0, 0, 30);
            doc.Blocks.Add(pInfo);

            Table footerTable = new Table();
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            TableRowGroup footerGroup = new TableRowGroup();
            TableRow footerRow = new TableRow();
            
            Paragraph pKys = new Paragraph(new Run("Người nhận tiền\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            Paragraph pBan = new Paragraph(new Run("Người chi tiền\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            
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
