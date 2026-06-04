using QLSNT.Models;

namespace QLSNT.Areas.User.ViewModel
{
    public class HoSoCaNhanViewModel
    {
        public NguoiDan? NguoiDan { get; set; }

        // Thường trú hiện tại (nếu có)
        public ThuongTru? ThuongTruHienTai { get; set; }

        // Danh sách các bản ghi tạm trú còn hiệu lực
        public IEnumerable<TamTru> DanhSachTamTru { get; set; } = new List<TamTru>();
        public string? DiaChiMoi { get; set; }

        // Tổng số nơi tạm trú
        public int SoLuongTamTru { get; set; }
    }
}
