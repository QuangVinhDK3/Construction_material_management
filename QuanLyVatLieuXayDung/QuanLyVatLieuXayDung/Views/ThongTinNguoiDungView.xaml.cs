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
using QuanLyVatLieuXayDung.Models;
using QuanLyVatLieuXayDung.ViewModels;

namespace QuanLyVatLieuXayDung.Views
{
    /// <summary>
    /// Interaction logic for ThongTinNguoiDungView.xaml
    /// </summary>
    public partial class ThongTinNguoiDungView : Window
    {
        public ThongTinNguoiDungView()
        {
            InitializeComponent();
        }
        public ThongTinNguoiDungView(NguoiDung user)
        {
            InitializeComponent();
            this.DataContext = new ThongTinNguoiDungViewModel(user);
        }
    }
}
