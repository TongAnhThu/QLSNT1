using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSNT.Models
{
    public class ThuongTru
    {
        [Key]
        public int Id { get; set; } 
        public int MaXaMoi { get; set; }

        public string MaCCCD { get; set; } 

        public string? DiaChi { get; set; }
        public DateTime? NgayDangKy { get; set; }

        // Navigation
        [ForeignKey(nameof(MaXaMoi))]
        public XaMoi? XaMoi { get; set; }

        [ForeignKey(nameof(MaCCCD))]
        public NguoiDan? NguoiDan { get; set; }
    }
}