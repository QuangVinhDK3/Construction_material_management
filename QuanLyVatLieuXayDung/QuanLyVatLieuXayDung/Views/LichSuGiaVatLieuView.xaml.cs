using System.Linq;
using System.Windows;
using QuanLyVatLieuXayDung.Models;

namespace QuanLyVatLieuXayDung.Views
{
    public partial class LichSuGiaVatLieuView : Window
    {
        public LichSuGiaVatLieuView(string idVatLieu, string tenVatLieu)
        {
            InitializeComponent();
            txtTitle.Text = $"Lịch sử giá của vật liệu: {tenVatLieu}";

            using (var db = new QuanLyVatLieuXayDungEntities())
            {
                var history = db.LichSuGiaVatLieux
                    .Where(x => x.IDVatLieu == idVatLieu)
                    .OrderByDescending(x => x.NgayThayDoi)
                    .ToList();
                
                lvLichSuGia.ItemsSource = history;
            }
        }
    }
}
