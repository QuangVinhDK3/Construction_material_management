using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class LoaiVatLieuViewModel:BaseViewModel
    {
		private ObservableCollection<LoaiVatLieu> _DSLoai;

		public ObservableCollection<LoaiVatLieu> DSLoai
		{
			get { return _DSLoai; }
			set 
			{
				if (_DSLoai != value)
				{
					_DSLoai = value;
					OnPropertyChanged(nameof(DSLoai));
				}
			}
		}
		private LoaiVatLieu _SelectedLoai;

		public LoaiVatLieu SelectedLoai
        {
			get { return _SelectedLoai; }
			set
			{
				if (_SelectedLoai != value)
				{
					_SelectedLoai = value;
					OnPropertyChanged(nameof(SelectedLoai));
				}
				if (_SelectedLoai != null)
				{
					DisplayName = SelectedLoai.DisplayName;
				}
			}
		}
        private string _DisplayName;

        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                if (_DisplayName != value)
                {
                    _DisplayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public LoaiVatLieuViewModel()
		{
			RefreshData();
            AddCommand = new RelayCommand<object>((p) => true, (p) => Add());
            RemoveCommand = new RelayCommand<object>((p) => true, (p) => Remove());
            UpdateCommand = new RelayCommand<object>((p) => true, (p) => Update());

        }
        private void RefreshData()
        {
            var list = DataProvider.Ins.DB.LoaiVatLieux.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].STT = i + 1;
            }
            DSLoai = new ObservableCollection<LoaiVatLieu>(list);
        }
        private string AutoCreateID()
        {
            for (int i = 1; i <= 999; i++)
            {
                string testID = "LVL" + i.ToString("D3");
                if (!DataProvider.Ins.DB.LoaiVatLieux.Any(p => p.ID == testID))
                {
                    return testID;
                }
            }
            throw new Exception("Hệ thống đã đạt giới hạn mã Loại vật liệu (LVL999)!");
        }
        public void Add()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin !!!", "Cảnh báo !");
                return;
            }
            var isExist = DataProvider.Ins.DB.LoaiVatLieux.Any(p => p.DisplayName == DisplayName);
            if (isExist)
            {
                MessageBox.Show("Tên loại vật liệu đã tồn tại, vui lòng nhập tên khác !!!", "Cảnh báo !");
                return;
            }
            string nextID = AutoCreateID();
            var loai = new LoaiVatLieu()
            {
                ID = nextID,
                DisplayName = DisplayName
            };

            DataProvider.Ins.DB.LoaiVatLieux.Add(loai);
            DataProvider.Ins.DB.SaveChanges();
            DSLoai.Add(loai);
            RefreshData();
            MessageBox.Show("Thêm thành công !!!", "Thông báo");
            DisplayName = string.Empty;

        }
        public void Remove()
        {
            if (SelectedLoai == null)
            {
                MessageBox.Show("Vui lòng chọn để xóa !!!", "Cảnh báo !");
                return;
            }
            DataProvider.Ins.DB.LoaiVatLieux.Remove(SelectedLoai);
            DataProvider.Ins.DB.SaveChanges();
            DSLoai.Remove(SelectedLoai);
            RefreshData();
            MessageBox.Show("Xóa thành công !!!", "Thông báo");
            DisplayName = string.Empty;
            SelectedLoai = null;
        }
        public void Update()
        {
            if (SelectedLoai == null)
            {
                MessageBox.Show("Vui lòng chọn để sửa !!!", "Cảnh báo !");
                return;
            }
            if (string.IsNullOrEmpty(DisplayName))
            {
                MessageBox.Show("Vui lòng nhập tên loại vật liệu !!!", "Cảnh báo !");
                return;
            }
            var isExist = DataProvider.Ins.DB.LoaiVatLieux.Any(p => p.DisplayName == DisplayName && p.ID != SelectedLoai.ID);
            if (isExist)
            {
                MessageBox.Show("Tên loại vật liệu đã tồn tại, vui lòng nhập tên khác !!!", "Cảnh báo !");
                return;
            }
            var loai = DataProvider.Ins.DB.LoaiVatLieux.Where(p => p.ID == SelectedLoai.ID).SingleOrDefault();
            if (loai != null)
            {
                loai.DisplayName = DisplayName;
                DataProvider.Ins.DB.SaveChanges();
                RefreshData();
                MessageBox.Show("Sửa thành công !!!", "Thông báo");
                DisplayName = string.Empty;
                SelectedLoai = null;
            }
        }
    }
}
