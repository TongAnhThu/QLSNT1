using QLSNT.Models;

namespace QLSNT.Areas.Admin.ViewModel
{
    public class BaoCaoThongKeVM
    {
        public int TongTinhMoi { get; set; }

        public int TongXaCu { get; set; }

        public int TongXaMoi { get; set; }

        public int TongNguoiDan { get; set; }

        public int SoDiaChiDaChuyenDoi { get; set; }

        public List<ThongKeDanTheoXaVM> DanTheoXa { get; set; }

        public List<DiaChiChuyenDoiVM> DiaChiChuyenDois { get; set; }
        public List<ThongKeDanTheoXaVM> DanTheoXaCu { get; set; }

        public List<XaCuDanhSachVM> DanhSachXaCu { get; set; }
        public List<ThongKeXaCuTheoTinhVM> ThongKeXaCuTheoTinh { get; set; } = new();
        public List<XaMoiVM> DanhSachXaMoi { get; set; }
    }
}
