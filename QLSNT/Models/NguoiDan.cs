using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSNT.Models
{
    public class NguoiDan
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string MaCCCD { get; set; } = default!;   // PK
        public string HoTen { get; set; } = default!;
        public string? HoTenKhongDau { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? NoiSinh { get; set; }
        public string? NgheNghiep { get; set; }
        public string? NoiLamViec { get; set; }
        public string? TinhTrangHonNhan { get; set; }
        public string? TrangThaiCongDan { get; set; }

        public string? MaQHCH { get; set; }
        public string? MaTonGiao { get; set; }
        public int? MaDanToc { get; set; }
        public string? MaTDVH { get; set; }

        [ForeignKey(nameof(MaQHCH))]
        public QuanHeChuHo? QuanHeChuHo { get; set; }
        [ForeignKey(nameof(MaTonGiao))]
        public TonGiao? TonGiao { get; set; }
        [ForeignKey(nameof(MaDanToc))]
        public DanToc? DanToc { get; set; }
        [ForeignKey(nameof(MaTDVH))]
        public TrinhDoVanHoa? TrinhDoVanHoa { get; set; }

        public ICollection<ThuongTru> ThuongTrus { get; set; } = new List<ThuongTru>();
        public ICollection<TamTru> TamTrus { get; set; } = new List<TamTru>();
        public ICollection<LichSuDiaChi> LichSuDiaChis { get; set; } = new List<LichSuDiaChi>();
    }
}
