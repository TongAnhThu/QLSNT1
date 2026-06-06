using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSNT.Areas.Admin.ViewModel;

using QLSNT.Data;
using QLSNT.ViewModels;


namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? maTinh,int? maHuyen, int? maXa)
        {
            var model = new BaoCaoThongKeVM();
           
            model.TongTinhMoi = _context.TinhMois.Count();

            model.TongXaCu = _context.LssnXas.Count();

            model.TongXaMoi = _context.XaMois.Count();

            model.TongNguoiDan = _context.NguoiDans.Count();

            model.SoDiaChiDaChuyenDoi =
                _context.NguoiDans.Count();

            ////ViewBag dữ liệu cho dropdown lọc

            ViewBag.TinhCus = _context.TinhCus
            .OrderBy(x => x.TenTinhCu)
            .ToList();
            ViewBag.TinhMois = _context.TinhMois.ToList();


            //// Thống kê dân số theo xã mới Tab 2
            model.DanTheoXa = _context.ThuongTrus
             .Include(x => x.XaMoi)
             .GroupBy(x => x.XaMoi!.TenXaMoi)
             .Select(g => new ThongKeDanTheoXaVM
                 {
                     TenXa = g.Key,
                     SoLuongDan = g.Count()
                 })
             .OrderByDescending(x => x.SoLuongDan)
             .ToList();

         model.DiaChiChuyenDois = _context.ThuongTrus
         .Include(x => x.NguoiDan)
         .Include(x => x.XaMoi)
             .ThenInclude(x => x.TinhMoi)
             .Select(x => new DiaChiChuyenDoiVM
             {
                 CCCD = x.MaCCCD,
                 HoTen = x.NguoiDan!.HoTen,

                 DiaChiMoi = x.DiaChi ?? "",

                 TenXaMoi = x.XaMoi!.TenXaMoi,

                 TenTinhMoi = x.XaMoi.TinhMoi!.TenTinhMoi
             })
         .ToList();


            // Thống kê dân số theo xã cũ Tab 1
            var xaQuery = _context.XaCus
    .Include(x => x.HuyenCu)
    .ThenInclude(h => h.TinhCu)
    .AsQueryable();

            if (maTinh.HasValue)
            {
                xaQuery = xaQuery.Where(x =>
                    x.HuyenCu.MaTinhCu == maTinh);
            }

            if (maHuyen.HasValue)
            {
                xaQuery = xaQuery.Where(x =>
                    x.MaHuyenCu == maHuyen);
            }

            if (maXa.HasValue)
            {
                xaQuery = xaQuery.Where(x =>
                    x.MaXaCu == maXa);
            }

            model.DanhSachXaCu = xaQuery
                .Select(x => new XaCuDanhSachVM
                {
                    MaXaCu = x.MaXaCu,
                    TenXaCu = x.TenXaCu,
                    TenHuyenCu = x.HuyenCu.TenHuyenCu,
                    TenTinhCu = x.HuyenCu.TinhCu.TenTinhCu
                })
                .ToList();



            model.ThongKeXaCuTheoTinh = _context.XaCus
    .Include(x => x.HuyenCu)
    .ThenInclude(x => x.TinhCu)
    .GroupBy(x => x.HuyenCu.TinhCu.TenTinhCu)
    .Select(g => new ThongKeXaCuTheoTinhVM
    {
        TenTinhCu = g.Key,
        SoHuyen = g.Select(x => x.HuyenCu.MaHuyenCu).Distinct().Count(),
        SoXa = g.Count()
    })
    .OrderByDescending(x => x.SoXa)
    .ToList();



            

            model.DanhSachXaMoi = _context.XaMois
    .Select(x => new XaMoiVM
    {
        TenXaMoi = x.TenXaMoi,
        TenTinhMoi = x.TinhMoi.TenTinhMoi
    })
    .ToList();



            return View(model);
        }

        [HttpGet]
        public JsonResult GetHuyenByTinh(int maTinh)
        {
            var data = _context.HuyenCus
                .Where(x => x.MaTinhCu == maTinh)
                .Select(x => new
                {
                    id = x.MaHuyenCu,
                    text = x.TenHuyenCu
                })
                .ToList();

            return Json(data);
        }
        [HttpGet]
        public JsonResult GetXaByHuyen(int maHuyen)
        {
            var data = _context.XaCus
                .Where(x => x.MaHuyenCu == maHuyen)
                .Select(x => new
                {
                    id = x.MaXaCu,
                    text = x.TenXaCu
                })
                .ToList();

            return Json(data);
        }
        
        
        
        [HttpGet]
        public IActionResult KetQuaTraCuu(
    int? maTinhCu,
    int? maHuyenCu,
    int? maXaCu)
        {
            var data = _context.XaCus
                .Include(x => x.HuyenCu)
                .ThenInclude(x => x.TinhCu)
                .AsQueryable();

            if (maTinhCu.HasValue)
            {
                data = data.Where(x =>
                    x.HuyenCu.MaTinhCu == maTinhCu);
            }

            if (maHuyenCu.HasValue)
            {
                data = data.Where(x =>
                    x.MaHuyenCu == maHuyenCu);
            }

            if (maXaCu.HasValue)
            {
                data = data.Where(x =>
                    x.MaXaCu == maXaCu);
            }

            var ketQua = data
                .Select(x => new XaCuDanhSachVM
                {
                    MaXaCu = x.MaXaCu,
                    TenXaCu = x.TenXaCu,
                    TenHuyenCu = x.HuyenCu.TenHuyenCu,
                    TenTinhCu = x.HuyenCu.TinhCu.TenTinhCu
                })
                .ToList();

            return View(ketQua);
        }
        public JsonResult GetXaMoiByTinh(int maTinh)
        {
            var data = _context.XaMois
                .Where(x => x.MaTinh == maTinh)
                .Select(x => new
                {
                    id = x.MaXaMoi,
                    text = x.TenXaMoi
                })
                .ToList();

            return Json(data);
        }
    }

}

