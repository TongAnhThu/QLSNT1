using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]

    public class TraCuuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TraCuuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================
        // TRANG TRA CỨU
        // =====================================

        public IActionResult Index()
        {
            return View();
        }

        // =====================================
        // TÌM KIẾM
        // =====================================

        [HttpPost]
        public IActionResult TimKiem(string diaChiCu)
        {
            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(diaChiCu))
            {
                ViewBag.Message =
                    "Vui lòng nhập địa chỉ";

                return View("Index");
            }

            // Chuẩn hóa chuỗi
            string keyword =
                RemoveVietnameseTone(
                    diaChiCu.ToLower().Trim()
                );

            // =====================================
            // TÁCH ĐỊA CHỈ
            // =====================================

            string[] parts = diaChiCu.Split(',');

            string tenXaNhap = "";

            if (parts.Length > 0)
            {
                tenXaNhap = parts[0];
            }

            // Chuẩn hóa tên xã nhập vào
            tenXaNhap = RemoveVietnameseTone(
                            tenXaNhap
                                .ToLower()
                                .Replace("xã", "")
                                .Replace("phường", "")
                                .Trim()
                        );

            // =====================================
            // TÌM XÃ CŨ
            // =====================================

            var xaCu = _context.XaCus

                .ToList()

                .FirstOrDefault(x =>

                    RemoveVietnameseTone(
                        x.TenXaCu.ToLower()
                    )

                    .Replace("xã", "")
                    .Replace("phường", "")
                    .Trim()

                    .Contains(tenXaNhap)
                );

            // Không tìm thấy xã cũ
            if (xaCu == null)
            {
                ViewBag.Message =
                    "Không tìm thấy xã cũ";

                return View("Index");
            }

            // =====================================
            // LẤY HUYỆN CŨ
            // =====================================

            var huyenCu = _context.HuyenCus
                .FirstOrDefault(x =>
                    x.MaHuyenCu == xaCu.MaHuyenCu
                );

            // =====================================
            // LẤY TỈNH CŨ
            // =====================================

            var tinhCu = _context.TinhCus
                .FirstOrDefault(x =>
                    x.MaTinhCu == xaCu.HuyenCu.MaTinhCu
                );

            // =====================================
            // TRA BẢNG LSSN_XA
            // =====================================

            var lssnXa = _context.LssnXas
     .FirstOrDefault(x =>
         x.MaXaCu == xaCu.MaXaCu
     );
            // Không có dữ liệu sáp nhập
            if (lssnXa == null)
            {
                ViewBag.Message =
                    "Không tìm thấy dữ liệu sáp nhập";

                return View("Index");
            }

            // =====================================
            // LẤY XÃ MỚI
            // =====================================

            var xaMoi = _context.XaMois
                .FirstOrDefault(x =>
                    x.MaXaMoi == lssnXa.MaXaMoi
                );

            // Không có xã mới
            if (xaMoi == null)
            {
                ViewBag.Message =
                    "Không tìm thấy xã mới";

                return View("Index");
            }

            // =====================================
            // LẤY HUYỆN MỚI
            // =====================================

            

            // =====================================
            // LẤY TỈNH MỚI
            // =====================================

            var tinhMoi = _context.TinhMois
                .FirstOrDefault(x =>
                    x.MaTinhMoi == xaMoi.MaTinh
                );

            // =====================================
            // VIEWMODEL
            // =====================================

            var result = new DiaChiMoiViewModel
            {
                DiaChiCu = diaChiCu,

                // CŨ
                TenXaCu = xaCu.TenXaCu,

                TenHuyenCu = huyenCu != null
                    ? huyenCu.TenHuyenCu
                    : "",

                TenTinhnCu = tinhCu != null
                    ? tinhCu.TenTinhCu
                    : "",

                // MỚI
                TenXaMoi = xaMoi.TenXaMoi,

                TenTinhMoi = tinhMoi != null
                    ? tinhMoi.TenTinhMoi
                    : ""
            };

            return View("KetQua", result);
        }

        // =====================================
        // HÀM BỎ DẤU TIẾNG VIỆT
        // =====================================

        public static string RemoveVietnameseTone(string text)
        {
            string[] arr1 = new string[]
            {
                "á","à","ả","ã","ạ",
                "ă","ắ","ằ","ẳ","ẵ","ặ",
                "â","ấ","ầ","ẩ","ẫ","ậ",

                "đ",

                "é","è","ẻ","ẽ","ẹ",
                "ê","ế","ề","ể","ễ","ệ",

                "í","ì","ỉ","ĩ","ị",

                "ó","ò","ỏ","õ","ọ",
                "ô","ố","ồ","ổ","ỗ","ộ",
                "ơ","ớ","ờ","ở","ỡ","ợ",

                "ú","ù","ủ","ũ","ụ",
                "ư","ứ","ừ","ử","ữ","ự",

                "ý","ỳ","ỷ","ỹ","ỵ"
            };

            string[] arr2 = new string[]
            {
                "a","a","a","a","a",
                "a","a","a","a","a","a",
                "a","a","a","a","a","a",

                "d",

                "e","e","e","e","e",
                "e","e","e","e","e","e",

                "i","i","i","i","i",

                "o","o","o","o","o",
                "o","o","o","o","o","o",
                "o","o","o","o","o","o",

                "u","u","u","u","u",
                "u","u","u","u","u","u",

                "y","y","y","y","y"
            };

            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
            }

            return text;
        }
    }
}