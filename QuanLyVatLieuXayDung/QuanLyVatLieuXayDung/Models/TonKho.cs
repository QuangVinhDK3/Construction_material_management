using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyVatLieuXayDung.Models
{
    public class TonKho
    {
        public VatLieu VatLieu { get; set; }
        public int STT { get; set; }
        public int Count { get; set; }
        public LoaiVatLieu LoaiVatLieu { get; set; }
    }
}
