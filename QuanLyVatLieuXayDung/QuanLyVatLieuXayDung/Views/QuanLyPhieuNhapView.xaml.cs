using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Linq;
using System.Data.Entity;
using QuanLyVatLieuXayDung.ViewModels;
using QuanLyVatLieuXayDung.Models;
namespace QuanLyVatLieuXayDung.Views
{
    public partial class QuanLyPhieuNhapView : Window
    {
        public QuanLyPhieuNhapView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = ((ListViewItem)sender).Content as PhieuNhap;

            if (selectedItem != null)
            {
                var viewModel = DataContext as QuanLyPhieuNhapViewModel;
                if (viewModel != null)
                {
                    viewModel.SelectedPN = selectedItem;
                }
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as QuanLyPhieuNhapViewModel;
            if (vm == null || vm.SelectedPN == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập trong danh sách để in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phieuNhap = DataProvider.Ins.DB.PhieuNhaps
                .Include("ChiTietPhieuNhaps.VatLieu")
                .Include("NhaCungCap")
                .FirstOrDefault(p => p.ID == vm.SelectedPN.ID);
            if (phieuNhap == null) return;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CreateInvoiceDocument(phieuNhap);
                doc.Name = "FlowDoc";
                
                doc.PageHeight = printDialog.PrintableAreaHeight > 0 ? printDialog.PrintableAreaHeight : 1122; 
                doc.PageWidth = printDialog.PrintableAreaWidth > 0 ? printDialog.PrintableAreaWidth : 793;   
                doc.ColumnWidth = doc.PageWidth;

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Phiếu Nhập - " + phieuNhap.ID);
            }
        }

        private FlowDocument CreateInvoiceDocument(PhieuNhap pn)
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

            Paragraph pTitle = new Paragraph(new Run("PHIẾU NHẬP KHO"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(pTitle);

            Paragraph pInfo = new Paragraph();
            pInfo.Inlines.Add(new Run($"Mã Phiếu Nhập: {pn.ID}\n") { FontWeight = FontWeights.Bold });
            pInfo.Inlines.Add(new Run($"Ngày: {pn.DateInput:dd/MM/yyyy HH:mm}\n"));
            pInfo.Inlines.Add(new Run($"Nhà cung cấp: {pn.NhaCungCap?.DisplayName}\n"));
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
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Giá nhập"))) { Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Thành tiền"))) { Padding = new Thickness(5) });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            TableRowGroup dataGroup = new TableRowGroup();
            int stt = 1;
            double tongTien = 0;

            if (pn.ChiTietPhieuNhaps != null)
            {
                foreach (var item in pn.ChiTietPhieuNhaps)
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(stt.ToString()))) { Padding = new Thickness(5) });
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.VatLieu?.DisplayName))) { Padding = new Thickness(5) });
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.Counts.ToString()))) { Padding = new Thickness(5) });
                    row.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("{0:N0}", item.PriceInput)))) { Padding = new Thickness(5) });
                    
                    double thanhTienItem = (item.Counts ?? 0) * (item.PriceInput ?? 0);
                    tongTien += thanhTienItem;
                    
                    row.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("{0:N0}", thanhTienItem)))) { Padding = new Thickness(5) });
                    
                    dataGroup.Rows.Add(row);
                    stt++;
                }
            }
            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Paragraph pTotal = new Paragraph();
            pTotal.Inlines.Add(new Run($"Tổng tiền nhập: {tongTien:N0} VNĐ\n") { FontWeight = FontWeights.Bold, Foreground = Brushes.Red });
            pTotal.TextAlignment = TextAlignment.Right;
            pTotal.Margin = new Thickness(0, 20, 0, 0);
            doc.Blocks.Add(pTotal);

            Table footerTable = new Table();
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            footerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            TableRowGroup footerGroup = new TableRowGroup();
            TableRow footerRow = new TableRow();
            
            Paragraph pKys = new Paragraph(new Run("Người giao hàng\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            Paragraph pBan = new Paragraph(new Run("Người lập phiếu\n(Ký, ghi rõ họ tên)")) { TextAlignment = TextAlignment.Center };
            
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
