using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyVatLieuXayDung.Models
{
    public partial class KhachHang
    {
        public int STT { get; set; }
        public double CongNoHienTai 
        { 
            get 
            {
                double totalNo = 0;
                if (PhieuXuats != null)
                {
                    // Công nợ từ các phiếu xuất (đã trừ khoản thanh toán ngay trên phiếu)
                    totalNo += PhieuXuats.Sum(px => px.ConNo);
                }
                
                if (this.PhieuThus != null)
                {
                    // Trừ đi tổng số tiền đã thu trong sổ quỹ
                    totalNo -= this.PhieuThus.Sum(pt => pt.SoTien ?? 0);
                }
                
                return totalNo;
            } 
        }
    }

    public partial class LoaiVatLieu
    {
        public int STT { get; set; }
    }

    public partial class NhaCungCap
    {
        public int STT { get; set; }
        public double CongNoHienTai
        {
            get
            {
                double totalNo = 0;
                if (PhieuNhaps != null)
                {
                    // Giả sử phiếu nhập không có cột SoTienDaThanhToan, toàn bộ là nợ
                    totalNo += PhieuNhaps.Sum(pn => pn.TongTien);
                }

                if (this.PhieuChis != null)
                {
                    // Trừ đi số tiền đã chi trả
                    totalNo -= this.PhieuChis.Sum(pc => pc.SoTien ?? 0);
                }

                return totalNo;
            }
        }
    }

    public partial class PhieuNhap
    {
        public int STT { get; set; }
        public double TongTien
        {
            get
            {
                if (ChiTietPhieuNhaps != null)
                {
                    return ChiTietPhieuNhaps.Sum(ct => (ct.Counts ?? 0) * (ct.PriceInput ?? 0));
                }
                return 0;
            }
        }
    }

    public partial class PhieuXuat
    {
        public int STT { get; set; }
        
        public double TongTien
        {
            get
            {
                double tongGiaTri = 0;
                if (ChiTietPhieuXuats != null && ChiTietPhieuXuats.Count > 0)
                {
                    tongGiaTri = ChiTietPhieuXuats.Sum(ct => (ct.Counts ?? 0) * (ct.Price ?? 0));
                }
                else
                {
                    tongGiaTri = Total ?? 0;
                }
                
                return tongGiaTri;
            }
        }

        public double ConNo
        {
            get
            {
                double chietKhau = this.ChietKhau ?? 0;
                return TongTien - chietKhau - (this.SoTienDaThanhToan ?? 0);
            }
        }
    }

    public partial class VatLieu
    {
        public int STT { get; set; }
        public int SoLuongTon { get; set; }
        public double GiaXuat { get; set; }
        public string TrangThai { get; set; }
        public int SoNgayUDong { get; set; }
    }

    public partial class NguoiDung
    {
        public int STT { get; set; }
        public bool? IsLocked { get; set; }
    }

    public partial class ChiTietPhieuNhap
    {
        public int STT { get; set; }
        public double ThanhTien
        {
            get
            {
                return (this.Counts ?? 0) * (this.PriceInput ?? 0);
            }
        }
    }

    public partial class ChiTietPhieuXuat
    {
        public int STT { get; set; }
        public double ThanhTien
        {
            get
            {
                return (this.Counts ?? 0) * (this.Price ?? 0);
            }
        }
    }

    public partial class PhieuThu
    {
        public int STT { get; set; }
    }

    public partial class PhieuChi
    {
        public int STT { get; set; }
    }
}
