using QuanLyVatLieuXayDung.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyVatLieuXayDung.ViewModels
{
    public class TrangChuViewModel : BaseViewModel
    {
        private ObservableCollection<TonKho> _DSTonKho;

        public ObservableCollection<TonKho> DSTonKho
        {
            get { return _DSTonKho; }
            set
            {
                if (value != _DSTonKho)
                {
                    _DSTonKho = value;
                    OnPropertyChanged(nameof(DSTonKho));
                }
            }
        }

        public TrangChuViewModel()
        {
            LoadTonKhoData();
        }

        public void LoadTonKhoData()
        {
            DSTonKho = new ObservableCollection<TonKho>();

            var VatLieuList = DataProvider.Ins.DB.VatLieux;
            int j = 1;

            foreach (var i in VatLieuList)
            {
                var InputList = DataProvider.Ins.DB.ChiTietPhieuNhaps.Where(p => p.IDObject == i.ID);
                var OutputList = DataProvider.Ins.DB.ChiTietPhieuXuats.Where(p => p.IDObject == i.ID);

                int sumInput = 0;
                int sumOutput = 0;
                if (InputList != null && InputList.Any())
                {
                    sumInput = (int)InputList.Sum(p => p.Counts);
                }

                if (OutputList != null && OutputList.Any())
                {
                    sumOutput = (int)OutputList.Sum(p => p.Counts);
                }

                TonKho ton = new TonKho();
                ton.STT = j;
                ton.Count = sumInput - sumOutput;
                ton.VatLieu = i;
                ton.LoaiVatLieu = i.LoaiVatLieu;

                DSTonKho.Add(ton);
                j++;
            }
        }
    }
}