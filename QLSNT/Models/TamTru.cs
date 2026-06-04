using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace QLSNT.Models
{
    [PrimaryKey(nameof(MaXaMoi), nameof(MaCCCD))]
    public class TamTru
    {
        // Composite key: MaXaMoi + MaCCCD
        public int MaXaMoi { get; set; } = default!;
        public string MaCCCD { get; set; } = default!;

        public string? DiaChi { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public string? ThoiHan { get; set; }
        public string? NoiDungDeNghi { get; set; }

        [ForeignKey(nameof(MaXaMoi))]
        public XaMoi? XaMoi { get; set; } = default!;

        [ForeignKey(nameof(MaCCCD))]
        public NguoiDan? NguoiDan { get; set; } = default!;

        [NotMapped]
        public int MaTinhMoi { get; set; }

        // Tính ngày hết hạn tạm trú
        [NotMapped]
        public DateTime? DenNgay
        {
            get
            {
                if (!NgayDangKy.HasValue || string.IsNullOrWhiteSpace(ThoiHan))
                    return null;

                // Lấy số từ chuỗi "12 tháng", "6 tháng",...
                var match = Regex.Match(ThoiHan, @"\d+");

                if (match.Success &&
                    int.TryParse(match.Value, out int soThang))
                {
                    return NgayDangKy.Value.AddMonths(soThang);
                }

                return null;
            }
        }
    }
}