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
using QuanLyVatLieuXayDung.ViewModels;

namespace QuanLyVatLieuXayDung.Views
{
    /// <summary>
    /// Interaction logic for DangNhapView.xaml
    /// </summary>
    public partial class DangNhapView : Window
    {
        public DangNhapView()
        {
            InitializeComponent();
        }
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as DangNhapViewModel;
            if (viewModel != null)
            {
                // Đồng bộ trực tiếp chuỗi mật khẩu sang biến Password trong ViewModel
                viewModel.Password = txtPassword.Password;
            }
        }
    }
}
