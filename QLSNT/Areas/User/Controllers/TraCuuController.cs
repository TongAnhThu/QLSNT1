using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSNT.Areas.User.ViewModel;
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
            if (string.IsNullOrWhiteSpace(diaChiCu))
            {
                ViewBag.Message = "Vui lòng nhập địa chỉ";

                return View("Index");
            }

            // Chuẩn hóa dữ liệu nhập
            string tenXaNhap = RemoveVietnameseTone(
                                    diaChiCu
                                        .ToLower()
                                        .Replace("xã", "")
                                        .Replace("phường", "")
                                        .Trim()
                                );

            // =====================================
            // TÌM TẤT CẢ XÃ CŨ TRÙNG TÊN
            // =====================================

            var dsXaCu = _context.XaCus
                .Include(x => x.HuyenCu)
                .ToList()
                .Where(x =>
                    RemoveVietnameseTone(
                        x.TenXaCu.ToLower()
                    )
                    .Replace("xã", "")
                    .Replace("phường", "")
                    .Trim()
                    .Contains(tenXaNhap)
                )
                .ToList();

            if (!dsXaCu.Any())
            {
                ViewBag.Message = "Không tìm thấy xã cũ";

                return View("Index");
            }

            // =====================================
            // DANH SÁCH KẾT QUẢ
            // =====================================

            List<KetQuaXaMoiViewModel> dsKetQua =
                new List<KetQuaXaMoiViewModel>();

            foreach (var xaCu in dsXaCu)
            {
                var huyenCu = _context.HuyenCus
                    .FirstOrDefault(x =>
                        x.MaHuyenCu == xaCu.MaHuyenCu);

                var tinhCu = _context.TinhCus
                    .FirstOrDefault(x =>
                        x.MaTinhCu == huyenCu.MaTinhCu);

                var lssnXa = _context.LssnXas
                    .Where(x =>
                        x.MaXaCu == xaCu.MaXaCu)
                    .Include(x => x.XaMoi)
                    .ToList();

                foreach (var item in lssnXa)
                {
                    var tinhMoi = _context.TinhMois
                        .FirstOrDefault(x =>
                            x.MaTinhMoi ==
                            item.XaMoi.MaTinh);

                    dsKetQua.Add(
                        new KetQuaXaMoiViewModel
                        {
                            TenXaCu = xaCu.TenXaCu,

                            TenHuyenCu = huyenCu != null
                                ? huyenCu.TenHuyenCu
                                : "",

                            TenTinhCu = tinhCu != null
                                ? tinhCu.TenTinhCu
                                : "",

                            TenXaMoi = item.XaMoi.TenXaMoi,

                            TenTinhMoi = tinhMoi != null
                                ? tinhMoi.TenTinhMoi
                                : ""
                        });
                }
            }

            var result = new DiaChiMoiViewModel
            {
                DiaChiCu = diaChiCu,
                DsXaMoi = dsKetQua
            };

            return View("Index", result);
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