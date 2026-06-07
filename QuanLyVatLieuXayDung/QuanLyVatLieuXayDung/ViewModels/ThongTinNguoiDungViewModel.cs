using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class ThongTinNguoiDungViewModel : BaseViewModel
    {
        // Giữ đối tượng gốc phục vụ lưu DB
        private NguoiDung _userGoc;

        #region Properties Binding
        private string _id;
        public string ID { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }

        private string _userName;
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(); } }

        private string _roleName;
        public string RoleName { get => _roleName; set { _roleName = value; OnPropertyChanged(); } }

        // Đổi mật khẩu
        private bool _isChangePasswordVisible;
        public bool IsChangePasswordVisible
        {
            get => _isChangePasswordVisible;
            set { _isChangePasswordVisible = value; OnPropertyChanged(); }
        }

        private string _currentPasswordInput;
        public string CurrentPasswordInput { get => _currentPasswordInput; set { _currentPasswordInput = value; OnPropertyChanged(); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public ICommand UpdateInfoCommand { get; set; }
        public ICommand TogglePasswordPanelCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion

        public ThongTinNguoiDungViewModel(NguoiDung user)
        {
            if (user == null) return;
            _userGoc = user;

            // Đổ dữ liệu từ user đăng nhập vào thuộc tính hiển thị
            ID = user.ID;
            DisplayName = user.DisplayName;
            UserName = user.UserName;

            // Lấy tên chức vụ hiển thị dạng văn bản chữ
            var roleObj = DataProvider.Ins.DB.NguoiDungRoles.FirstOrDefault(r => r.ID == user.IDRole);
            RoleName = roleObj != null ? roleObj.DisplayName : user.IDRole;

            IsChangePasswordVisible = false; // Mặc định ẩn vùng đổi mật khẩu

            // Khởi tạo các Command
            TogglePasswordPanelCommand = new RelayCommand<object>((p) => true, (p) => {
                IsChangePasswordVisible = !IsChangePasswordVisible;
            });

            CancelCommand = new RelayCommand<Window>((p) => true, (p) => {
                if (p != null) p.Close();
            });

            UpdateInfoCommand = new RelayCommand<Window>((p) => true, (p) => LuuThayDoi(p));
        }

        private void LuuThayDoi(Window currentWindow)
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Tên hiển thị không được để trống!", "Nhắc nhở", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Tìm kiếm dòng dữ liệu trong DB để sửa đổi chính xác
                var dbUser = DataProvider.Ins.DB.NguoiDungs.SingleOrDefault(x => x.ID == _userGoc.ID);
                if (dbUser != null)
                {
                    dbUser.DisplayName = DisplayName;

                    // Nếu người dùng chọn mở rộng và muốn đổi mật khẩu
                    if (IsChangePasswordVisible)
                    {
                        if (string.IsNullOrEmpty(CurrentPasswordInput) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                        {
                            MessageBox.Show("Vui lòng điền đầy đủ các ô mật khẩu!", "Nhắc nhở", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (CurrentPasswordInput != dbUser.Password)
                        {
                            MessageBox.Show("Mật khẩu hiện tại không chính xác!", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (NewPassword != ConfirmPassword)
                        {
                            MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không trùng nhau!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Đủ điều kiện thì gán mật khẩu mới
                        dbUser.Password = NewPassword;
                    }

                    DataProvider.Ins.DB.SaveChanges();

                    // Đồng bộ lại dữ liệu hiển thị tức thời ra màn hình chính (MainWindow)
                    _userGoc.DisplayName = DisplayName;

                    MessageBox.Show("Cập nhật thông tin tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (currentWindow != null) currentWindow.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gặp lỗi khi lưu thông tin: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
