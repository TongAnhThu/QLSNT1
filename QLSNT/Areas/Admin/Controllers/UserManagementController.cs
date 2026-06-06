using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSNT.Areas.Admin.ViewModel;
using QLSNT.Data;
using QLSNT.Models;
using System.Security.Claims;

namespace QLSNT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public UserManagementController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        #region Danh sách tài khoản

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserManagementVM>();

            foreach (var user in users)
            {
                var nguoiDan = await _context.NguoiDans
                    .FirstOrDefaultAsync(x => x.MaCCCD == user.UserName);

                var role = (await _userManager.GetRolesAsync(user))
                    .FirstOrDefault();

                model.Add(new UserManagementVM
                {
                    UserId = user.Id,
                    CCCD = user.UserName,
                    HoTen = nguoiDan?.HoTen ?? "",
                    Role = role ?? "",
                    IsLocked = user.LockoutEnd != null &&
                               user.LockoutEnd > DateTimeOffset.Now
                });
            }

            return View(model);
        }

        #endregion

        #region Chi tiết tài khoản

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            ViewBag.Roles = await _userManager.GetRolesAsync(user);

            ViewBag.AllRoles = _roleManager.Roles.ToList();

            ViewBag.LichSuHoatDong = await _context.NhatKyHoatDongs
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.ThoiGian)
                .Take(50)
                .ToListAsync();

            return View(user);
        }

        #endregion

        #region Khóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.AddYears(100));

            await GhiNhatKy(
                user.Id,
                "Khóa tài khoản",
                $"Tài khoản {user.UserName} bị khóa bởi Admin");

            TempData["Success"] = "Đã khóa tài khoản.";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Mở khóa tài khoản

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);

            await GhiNhatKy(
                user.Id,
                "Mở khóa tài khoản",
                $"Tài khoản {user.UserName} được mở khóa bởi Admin");

            TempData["Success"] = "Đã mở khóa tài khoản.";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Reset mật khẩu

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            string defaultPassword = "123456@Aa";

            IdentityResult result;

            if (!await _userManager.HasPasswordAsync(user))
            {
                result = await _userManager.AddPasswordAsync(
                    user,
                    defaultPassword);
            }
            else
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(user);

                result =
                    await _userManager.ResetPasswordAsync(
                        user,
                        token,
                        defaultPassword);
            }

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    string.Join(", ",
                    result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Index));
            }

            await GhiNhatKy(
    user.Id,
    "Reset mật khẩu",
    $"Mật khẩu được đặt lại bởi Admin");
            TempData["Success"] =
                $"Đã reset mật khẩu về: {defaultPassword}";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Xóa tài khoản

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId =
                _userManager.GetUserId(User);

            if (currentUserId == id)
            {
                TempData["Error"] =
                    "Không thể xóa tài khoản đang đăng nhập.";

                return RedirectToAction(nameof(Index));
            }

            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var result =
                await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    string.Join(", ",
                    result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Index));
            }
            if (result.Succeeded)
            {
                await GhiNhatKy(
                    user.Id,
                    "Xóa tài khoản",
                    $"Tài khoản {user.UserName} đã bị xóa");
            }

            TempData["Success"] =
                "Đã xóa tài khoản thành công.";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Gán quyền

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(
            string userId,
            string roleName)
        {
            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["Error"] =
                    "Role không tồn tại.";

                return RedirectToAction(nameof(Index));
            }

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(
                user,
                currentRoles);

            await _userManager.AddToRoleAsync(
                user,
                roleName);


            await GhiNhatKy(
    user.Id,
    "Cập nhật quyền",
    $"Được gán quyền {roleName}");


            TempData["Success"] =
                "Cập nhật quyền thành công.";

            return RedirectToAction(
                nameof(Details),
                new { id = userId });
        }

        #endregion

        #region Danh sách Role

        public IActionResult Roles()
        {
            var roles =
                _roleManager.Roles.ToList();

            return View(roles);
        }

        #endregion

        #region Tạo Role

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["Error"] =
                    "Tên quyền không được để trống.";

                return RedirectToAction(nameof(Roles));
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["Error"] =
                    "Role đã tồn tại.";

                return RedirectToAction(nameof(Roles));
            }

            var result =
                await _roleManager.CreateAsync(
                    new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    string.Join(", ",
                    result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Roles));
            }

            TempData["Success"] =
                "Tạo role thành công.";

            return RedirectToAction(nameof(Roles));
        }

        #endregion

        #region Xóa Role

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role =
                await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            var result =
                await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    string.Join(", ",
                    result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Roles));
            }

            TempData["Success"] =
                "Đã xóa role.";

            return RedirectToAction(nameof(Roles));
        }

        #endregion


        private async Task GhiNhatKy(
    string userId,
    string hanhDong,
    string moTa)
        {
            var log = new NhatKyHoatDong
            {
                UserId = userId,
                HanhDong = hanhDong,
                MoTa = moTa,
                ThoiGian = DateTime.Now,
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _context.NhatKyHoatDongs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> LichSuHoatDong(string id)
        {
            var logs = await _context.NhatKyHoatDongs
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.ThoiGian)
                .ToListAsync();

            return View(logs);
        }
    }
}
