using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class ControlBarViewModel : BaseViewModel
    {
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MaxCommand { get; set; }
        public ICommand MinCommand { get; set; }
        public ICommand MouseMoveCommand { get; set; }
        public ControlBarViewModel()
        {
            CloseWindowCommand = new RelayCommand<System.Windows.Controls.UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetElementParent(p);
                var w = window as Window;
                if (w != null)
                {
                    w.Close();
                }
            }
            );
            MaxCommand = new RelayCommand<System.Windows.Controls.UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetElementParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Maximized)
                        w.WindowState = WindowState.Maximized;
                    else
                        w.WindowState = WindowState.Normal;
                }
            }
            );
            MinCommand = new RelayCommand<System.Windows.Controls.UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetElementParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Minimized)
                        w.WindowState = WindowState.Minimized;
                    else
                        w.WindowState = WindowState.Maximized;
                }
            }
            );
            MouseMoveCommand = new RelayCommand<System.Windows.Controls.UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetElementParent(p);
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();
                }
            }
            );

        }
        FrameworkElement GetElementParent(System.Windows.Controls.UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
            {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
