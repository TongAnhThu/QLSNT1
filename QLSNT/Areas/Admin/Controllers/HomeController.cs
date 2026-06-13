using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSNT.Data;
using QLSNT.Models;
using System;
using System.Linq;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard/Home page
        public IActionResult Index()
        {
            // ==========================================
            // 1. DASHBOARD TỔNG QUAN HỆ THỐNG
            // ==========================================
            ViewBag.TinhCuCount = _context.TinhCus.Count();
            ViewBag.HuyenCuCount = _context.HuyenCus.Count();
            ViewBag.XaCuCount = _context.XaCus.Count();

            ViewBag.TinhMoiCount = _context.TinhMois.Count();
            ViewBag.XaMoiCount = _context.XaMois.Count();

            ViewBag.SuKienCount = _context.SuKienHanhChinhs.Count();
            ViewBag.NguoiDanCount = _context.NguoiDans.Count();

            ViewBag.ThuongTruCount = _context.ThuongTrus.Count();
            ViewBag.TamTruCount = _context.TamTrus.Count();

            ViewBag.CuTruCount =
                _context.ThuongTrus.Count() +
                _context.TamTrus.Count();

            // ==========================================
            // 2. DASHBOARD DÂN CƯ (DEMOGRAPHICS)
            // ==========================================
            // Dân số theo Tỉnh mới (Từ bản ghi Thường trú)
            ViewBag.DanSoTheoTinhMoi = _context.ThuongTrus
                .Include(t => t.XaMoi)
                    .ThenInclude(x => x.TinhMoi)
                .Where(t => t.XaMoi != null && t.XaMoi.TinhMoi != null)
                .GroupBy(t => t.XaMoi.TinhMoi.TenTinhMoi)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToList();

            // Dân số theo Xã mới (Top 10 xã mới đông dân nhất)
            ViewBag.DanSoTheoXaMoi = _context.ThuongTrus
                .Include(t => t.XaMoi)
                .Where(t => t.XaMoi != null)
                .GroupBy(t => t.XaMoi.TenXaMoi)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .OrderByDescending(g => g.Value)
                .Take(10)
                .ToList();

            // Cơ cấu giới tính Nam / Nữ
            ViewBag.GioiTinhStats = _context.NguoiDans
                .GroupBy(n => n.GioiTinh ?? "Không xác định")
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToList();

            // Lịch sử biến động/thay đổi địa chỉ
            ViewBag.LichSuDiaChiCount = _context.LichSuDiaChis.Count();

            // ==========================================
            // 3. DASHBOARD SAU SÁP NHẬP (MAPPING & EVENTS)
            // ==========================================
            // Phân bổ số lượng Xã mới theo từng Tỉnh mới
            ViewBag.PhanBoXaMoiTheoTinh = _context.XaMois
                .Include(x => x.TinhMoi)
                .Where(x => x.TinhMoi != null)
                .GroupBy(x => x.TinhMoi.TenTinhMoi)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToList();

            // Tỷ lệ Đơn vị hành chính cấp xã Cũ đã được ánh xạ sang Xã mới
            int totalXaCu = _context.XaCus.Count();
            int mappedXaCu = _context.LssnXas
                .Select(l => l.MaXaCu)
                .Distinct()
                .Count();

            ViewBag.MappedXaCuCount = mappedXaCu;
            ViewBag.UnmappedXaCuCount = Math.Max(0, totalXaCu - mappedXaCu);
            ViewBag.TyleAnhXa = totalXaCu > 0 ? Math.Round((double)mappedXaCu / totalXaCu * 100, 1) : 0;

            // Danh sách ánh xạ sáp nhập chi tiết (phục vụ bảng tra cứu ánh xạ)
            ViewBag.AnhXaList = _context.LssnXas
                .Include(l => l.XaCu)
                    .ThenInclude(xc => xc.HuyenCu)
                        .ThenInclude(hc => hc.TinhCu)
                .Include(l => l.XaMoi)
                    .ThenInclude(xm => xm.TinhMoi)
                .Select(l => new {
                    TinhCu = l.XaCu.HuyenCu.TinhCu.TenTinhCu,
                    HuyenCu = l.XaCu.HuyenCu.TenHuyenCu,
                    XaCu = l.XaCu.TenXaCu,
                    TinhMoi = l.XaMoi.TinhMoi.TenTinhMoi,
                    XaMoi = l.XaMoi.TenXaMoi,
                    SoNghiDinh = l.LichSuSapNhap.SoNghiDinh
                })
                .ToList();

            // Danh sách các quyết định/sự kiện hành chính
            ViewBag.SuKienList = _context.SuKienHanhChinhs
                .OrderByDescending(s => s.NgayBanHanh)
                .Take(15)
                .ToList();

            return View();
        }

        // About page
        public IActionResult About()
        {
            ViewBag.Message = "Trang quản trị hệ thống quản lý thông tin hành chính sau sáp nhập.";
            return View();
        }

        // Error page
        public IActionResult Error()
        {
            return View();
        }
    }
}