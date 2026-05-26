using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLSNT.Areas.Identity.Pages.Account;
using QLSNT.Data;
using QLSNT.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Đăng ký DbContext
// =======================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================
// Đăng ký Identity
// =======================
// Nếu bạn cần Role thì giữ AddIdentity<IdentityUser, IdentityRole>
// Nếu chỉ cần Login/Register thì dùng AddDefaultIdentity<IdentityUser>
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// =======================
// Đăng ký Repositories
// =======================
builder.Services.AddScoped<ITinhCuRepository, EFTinhCuRepository>();
builder.Services.AddScoped<IHuyenCuRepository, EFHuyenCuRepository>();
builder.Services.AddScoped<IXaCuRepository, EFXaCuRepository>();
builder.Services.AddScoped<IXaMoiRepository, EFXaMoiRepository>();
builder.Services.AddScoped<ITinhMoiRepository, EFTinhMoiRepository>();
builder.Services.AddScoped<ISuKienHanhChinhRepository, EFSuKienHanhChinhRepository>();
builder.Services.AddScoped<ILichSuSapNhapRepository, EFLichSuSapNhapRepository>();
builder.Services.AddScoped<ILssnTinhRepository, EFLssnTinhRepository>();
builder.Services.AddScoped<ILssnXaRepository, EFLssnXaRepository>();
builder.Services.AddScoped<INguoiDanRepository, EFNguoiDanRepository>();
builder.Services.AddScoped<IThuongTruRepository, EFThuongTruRepository>();
builder.Services.AddScoped<ITamTruRepository, EFTamTruRepository>();
builder.Services.AddScoped<ILichSuDiaChiRepository, EFLichSuDiaChiRepository>();
builder.Services.AddScoped<ITrinhDoVanHoaRepository, EFTrinhDoVanHoaRepository>();
builder.Services.AddScoped<IDanTocRepository, EFDanTocRepository>();
builder.Services.AddScoped<ITonGiaoRepository, EFTonGiaoRepository>();
builder.Services.AddScoped<IQuanHeChuHoRepository, EFQuanHeChuHoRepository>();

// =======================
// Razor Pages + MVC
// =======================
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// =======================
// Pipeline
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // Bắt buộc để Identity hoạt động
app.UseAuthorization();

// Route cho Area (đặt trước default route)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Razor Pages (Identity UI)
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
}

app.Run();
