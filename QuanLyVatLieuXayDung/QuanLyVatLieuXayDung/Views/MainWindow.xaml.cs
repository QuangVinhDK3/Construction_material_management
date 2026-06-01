using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace QuanLyVatLieuXayDung.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnDashBroad.Click += BtnDashBroad_Click;
            using (var db = new QuanLyVatLieuXayDungEntities())
            {
                if (db.Database.Exists())
                {
                    System.Windows.MessageBox.Show("KẾT NỐI THÀNH CÔNG! Database đã sẵn sàng.");
                }
                else
                {
                    System.Windows.MessageBox.Show("KẾT NỐI THẤT BẠI! Kiểm tra lại SQL Server hoặc App.config.");
                }
            }
        }

        private void BtnDashBroad_Click(object sender, RoutedEventArgs e)
        {
           Main.Children.Clear();
            Main.Children.Add(new TrangChuView());
        }
    }
}
