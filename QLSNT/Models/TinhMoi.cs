using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSNT.Models
{
    public class TinhMoi
    {
        [Key]
        [Required]

        public int MaTinhMoi { get; set; }   // PK
        public string TenTinhMoi { get; set; } = default!;
        public double? DienTich { get; set; }
        public long? DanSo { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public int? SLXa { get; set; }
        public string? VungDiaLy { get; set; }
        public string? LoaiTinh { get; set; }
        public int? SLHuyen { get; set; }

        public ICollection<XaMoi> XaMois { get; set; } = new List<XaMoi>();
        public ICollection<LssnTinh> LssnTinhs { get; set; } = new List<LssnTinh>();
    }
}
