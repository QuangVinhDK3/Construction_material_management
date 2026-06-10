using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyVatLieuXayDung.Models;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class NhanVienViewModel : BaseViewModel
    {
        #region Collections
        private ObservableCollection<NguoiDung> _listNguoiDung;
        public ObservableCollection<NguoiDung> ListNguoiDung
        {
            get => _listNguoiDung;
            set { _listNguoiDung = value; OnPropertyChanged(); }
        }

        private ObservableCollection<NguoiDungRole> _role;
        public ObservableCollection<NguoiDungRole> Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }
        #endregion

        #region Properties & Selected Item
        private NguoiDung _selectedNguoiDung;
        public NguoiDung SelectedNguoiDung
        {
            get => _selectedNguoiDung;
            set
            {
                _selectedNguoiDung = value;
                OnPropertyChanged();
                if (_selectedNguoiDung != null)
                {
                    UserName = _selectedNguoiDung.UserName;
                    DisplayName = _selectedNguoiDung.DisplayName;
                    Password = _selectedNguoiDung.Password;
                    SelectedRoleID = _selectedNguoiDung.IDRole;
                }
            }
        }

        private string _userName;
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(); } }

        private string _displayName;
        public string DisplayName { get => _displayName; set { _displayName = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private string _selectedRoleID;
        public string SelectedRoleID { get => _selectedRoleID; set { _selectedRoleID = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand ResetPasswordCommand { get; set; }
        public ICommand LockAccountCommand { get; set; }
        public ICommand UnlockAccountCommand { get; set; }
        #endregion

        public NhanVienViewModel()
        {
            // 1. Tải dữ liệu ban đầu từ Database
            LoadData();

            // 2. Khởi tạo các Lệnh điều khiển (Commands)
            AddCommand = new RelayCommand<object>((p) => true, (p) => AddNhanVien());
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => UpdateNhanVien());
            RemoveCommand = new RelayCommand<object>((p) => true, (p) => RemoveNhanVien());
            ResetPasswordCommand = new RelayCommand<object>((p) => true, (p) => ResetPassword());
            LockAccountCommand = new RelayCommand<object>((p) => true, (p) => SetLockStatus(true));
            UnlockAccountCommand = new RelayCommand<object>((p) => true, (p) => SetLockStatus(false));
        }

        #region Helper Methods
        private void LoadData()
        {
            try
            {
                // Tải danh sách quyền hệ thống để nạp vào ComboBox
                Role = new ObservableCollection<NguoiDungRole>(DataProvider.Ins.DB.NguoiDungRoles.ToList());

                // Tải danh sách người dùng bao gồm cả thực thể Role liên kết để hiển thị tên quyền
                var list = DataProvider.Ins.DB.NguoiDungs.ToList();
                foreach (var user in list)
                {
                    if (user.NguoiDungRole == null)
                    {
                        user.NguoiDungRole = Role.FirstOrDefault(r => r.ID.Trim() == user.IDRole.Trim());
                    }
                }
                ListNguoiDung = new ObservableCollection<NguoiDung>(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu nhân viên: " + ex.Message, "Hệ thống lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string AutoCreateIDUser()
        {
            var existingIDs = DataProvider.Ins.DB.NguoiDungs.Select(p => p.ID.Trim()).ToList();
            for (int i = 1; i <= 999; i++)
            {
                string testID = "NV" + i.ToString("D3"); // Sinh mã dạng NV001, NV002...
                if (!existingIDs.Contains(testID)) return testID;
            }
            throw new Exception("Hệ thống đã đạt số lượng nhân viên tối đa!");
        }

        private void ClearFields()
        {
            SelectedNguoiDung = null;
            UserName = string.Empty;
            DisplayName = string.Empty;
            Password = string.Empty;
            SelectedRoleID = null;
        }
        #endregion

        #region CRUD & Business Operations

        // 1. CHỨC NĂNG THÊM MỚI TÀI KHOẢN
        private void AddNhanVien()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(SelectedRoleID) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập, Tên hiển thị, Mật khẩu và Chức vụ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra trùng tên đăng nhập trong Database
            var checkUsername = DataProvider.Ins.DB.NguoiDungs.Any(x => x.UserName.Trim().ToLower() == UserName.Trim().ToLower());
            if (checkUsername)
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại trên hệ thống!", "Trùng dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newNhanVien = new NguoiDung()
                {
                    ID = AutoCreateIDUser(),
                    UserName = UserName.Trim(),
                    DisplayName = DisplayName.Trim(),
                    IDRole = SelectedRoleID,
                    Password = Password.Trim(),
                    IsLocked = false   // Mặc định tài khoản được phép hoạt động
                };

                DataProvider.Ins.DB.NguoiDungs.Add(newNhanVien);
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show($"Thêm tài khoản nhân viên thành công!\nMã số: {newNhanVien.ID}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 2. CHỨC NĂNG SỬA THÔNG TIN
        private void UpdateNhanVien()
        {
            if (SelectedNguoiDung == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Mật khẩu không được để trống!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var nhanvien = DataProvider.Ins.DB.NguoiDungs.SingleOrDefault(p => p.ID == SelectedNguoiDung.ID);
                if (nhanvien != null)
                {
                    nhanvien.DisplayName = DisplayName.Trim();
                    nhanvien.Password = Password.Trim();
                    nhanvien.IDRole = SelectedRoleID;

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 3. CHỨC NĂNG XÓA BỎ TÀI KHOẢN
        private void RemoveNhanVien()
        {
            if (SelectedNguoiDung == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên muốn xóa bỏ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn tài khoản {SelectedNguoiDung.UserName}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var nhanvien = DataProvider.Ins.DB.NguoiDungs.SingleOrDefault(p => p.ID == SelectedNguoiDung.ID);
                    if (nhanvien != null)
                    {
                        DataProvider.Ins.DB.NguoiDungs.Remove(nhanvien);
                        DataProvider.Ins.DB.SaveChanges();

                        MessageBox.Show("Đã xóa tài khoản khỏi hệ thống thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearFields();
                        LoadData();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa do tài khoản này đã có lịch sử tạo Hóa đơn/Phiếu nhập kho để ràng buộc dữ liệu!", "Lỗi ràng buộc", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 4. CHỨC NĂNG RESET MẬT KHẨU VỀ 123
        private void ResetPassword()
        {
            if (SelectedNguoiDung == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần đặt lại mật khẩu!", "Nhắc nhở", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn muốn đặt lại mật khẩu của tài khoản '{SelectedNguoiDung.UserName}' về mặc định là '123'?", "Xác nhận reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var nhanvien = DataProvider.Ins.DB.NguoiDungs.SingleOrDefault(p => p.ID == SelectedNguoiDung.ID);
                    if (nhanvien != null)
                    {
                        nhanvien.Password = "123"; // Reset về pass mặc định
                        DataProvider.Ins.DB.SaveChanges();

                        MessageBox.Show($"Đặt lại mật khẩu cho tài khoản {nhanvien.UserName} thành công! Mật khẩu mới là: 123", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearFields();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi reset mật khẩu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 5. CHỨC NĂNG KHÓA / MỞ KHÓA TÀI KHOẢN (DÙNG CHUNG CHO CẢ 2 NÚT KHỐI TAB 2)
        private void SetLockStatus(bool shouldLock)
        {
            if (SelectedNguoiDung == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản từ danh sách để thực hiện thay đổi trạng thái!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string hanhDongText = shouldLock ? "KHÓA" : "MỞ KHÓA";

            try
            {
                var nhanvien = DataProvider.Ins.DB.NguoiDungs.SingleOrDefault(p => p.ID == SelectedNguoiDung.ID);
                if (nhanvien != null)
                {
                    // Thực hiện thay đổi trạng thái IsLocked trong DB
                    nhanvien.IsLocked = shouldLock;
                    DataProvider.Ins.DB.SaveChanges();

                    MessageBox.Show($"Đã thực hiện {hanhDongText} tài khoản '{nhanvien.UserName}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Lưu lại dòng đang chọn để không bị mất focus danh sách ở Tab 2
                    var currentSelectedID = nhanvien.ID;
                    LoadData();
                    SelectedNguoiDung = ListNguoiDung.FirstOrDefault(x => x.ID == currentSelectedID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gặp lỗi khi thực hiện {hanhDongText}: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}