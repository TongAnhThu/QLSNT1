using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSNT.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanTocs",
                columns: table => new
                {
                    MaDanToc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanToc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenKhac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanTocs", x => x.MaDanToc);
                });

            migrationBuilder.CreateTable(
                name: "QuanHeChuHos",
                columns: table => new
                {
                    MaQHCH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenQHCH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanHeChuHos", x => x.MaQHCH);
                });

            migrationBuilder.CreateTable(
                name: "SuKienHanhChinhs",
                columns: table => new
                {
                    SoNghiDinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenSK = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanCuPhapLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoQuanBanHanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBanHanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiTao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuKienHanhChinhs", x => x.SoNghiDinh);
                });

            migrationBuilder.CreateTable(
                name: "TinhCus",
                columns: table => new
                {
                    MaTinhCu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTinhCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DienTich = table.Column<double>(type: "float", nullable: true),
                    DanSo = table.Column<long>(type: "bigint", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLHuyen = table.Column<int>(type: "int", nullable: true),
                    SLXa = table.Column<int>(type: "int", nullable: true),
                    VungDiaLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhCus", x => x.MaTinhCu);
                });

            migrationBuilder.CreateTable(
                name: "TinhMois",
                columns: table => new
                {
                    MaTinhMoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTinhMoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DienTich = table.Column<double>(type: "float", nullable: true),
                    DanSo = table.Column<long>(type: "bigint", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLXa = table.Column<int>(type: "int", nullable: true),
                    VungDiaLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLHuyen = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhMois", x => x.MaTinhMoi);
                });

            migrationBuilder.CreateTable(
                name: "TonGiaos",
                columns: table => new
                {
                    MaTonGiao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenTonGiao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenKhac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TonGiaos", x => x.MaTonGiao);
                });

            migrationBuilder.CreateTable(
                name: "TrinhDoVanHoas",
                columns: table => new
                {
                    MaTDVH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenTDVH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrinhDoVanHoas", x => x.MaTDVH);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuSapNhaps",
                columns: table => new
                {
                    MaLSSN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NguoiTao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoNghiDinh = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuSapNhaps", x => x.MaLSSN);
                    table.ForeignKey(
                        name: "FK_LichSuSapNhaps_SuKienHanhChinhs_SoNghiDinh",
                        column: x => x.SoNghiDinh,
                        principalTable: "SuKienHanhChinhs",
                        principalColumn: "SoNghiDinh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HuyenCus",
                columns: table => new
                {
                    MaHuyenCu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHuyenCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiHuyen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanSo = table.Column<long>(type: "bigint", nullable: true),
                    DienTich = table.Column<double>(type: "float", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLXa = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaTinhCu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuyenCus", x => x.MaHuyenCu);
                    table.ForeignKey(
                        name: "FK_HuyenCus_TinhCus_MaTinhCu",
                        column: x => x.MaTinhCu,
                        principalTable: "TinhCus",
                        principalColumn: "MaTinhCu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XaMois",
                columns: table => new
                {
                    MaXaMoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenXaMoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiXa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLAp = table.Column<int>(type: "int", nullable: true),
                    DanSo = table.Column<long>(type: "bigint", nullable: true),
                    DienTich = table.Column<double>(type: "float", nullable: true),
                    MaTinh = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XaMois", x => x.MaXaMoi);
                    table.ForeignKey(
                        name: "FK_XaMois_TinhMois_MaTinh",
                        column: x => x.MaTinh,
                        principalTable: "TinhMois",
                        principalColumn: "MaTinhMoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDans",
                columns: table => new
                {
                    MaCCCD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoTenKhongDau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiSinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgheNghiep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiLamViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhTrangHonNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiCongDan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaQHCH = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    MaTonGiao = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    MaDanToc = table.Column<int>(type: "int", nullable: true),
                    MaTDVH = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDans", x => x.MaCCCD);
                    table.ForeignKey(
                        name: "FK_NguoiDans_DanTocs_MaDanToc",
                        column: x => x.MaDanToc,
                        principalTable: "DanTocs",
                        principalColumn: "MaDanToc");
                    table.ForeignKey(
                        name: "FK_NguoiDans_QuanHeChuHos_MaQHCH",
                        column: x => x.MaQHCH,
                        principalTable: "QuanHeChuHos",
                        principalColumn: "MaQHCH");
                    table.ForeignKey(
                        name: "FK_NguoiDans_TonGiaos_MaTonGiao",
                        column: x => x.MaTonGiao,
                        principalTable: "TonGiaos",
                        principalColumn: "MaTonGiao");
                    table.ForeignKey(
                        name: "FK_NguoiDans_TrinhDoVanHoas_MaTDVH",
                        column: x => x.MaTDVH,
                        principalTable: "TrinhDoVanHoas",
                        principalColumn: "MaTDVH");
                });

            migrationBuilder.CreateTable(
                name: "LssnTinhs",
                columns: table => new
                {
                    MaLSSN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaTinhCu = table.Column<int>(type: "int", nullable: false),
                    MaTinhMoi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LssnTinhs", x => x.MaLSSN);
                    table.ForeignKey(
                        name: "FK_LssnTinhs_LichSuSapNhaps_MaLSSN",
                        column: x => x.MaLSSN,
                        principalTable: "LichSuSapNhaps",
                        principalColumn: "MaLSSN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LssnTinhs_TinhCus_MaTinhCu",
                        column: x => x.MaTinhCu,
                        principalTable: "TinhCus",
                        principalColumn: "MaTinhCu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LssnTinhs_TinhMois_MaTinhMoi",
                        column: x => x.MaTinhMoi,
                        principalTable: "TinhMois",
                        principalColumn: "MaTinhMoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XaCus",
                columns: table => new
                {
                    MaXaCu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenXaCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiXa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SLAp = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanSo = table.Column<long>(type: "bigint", nullable: true),
                    DienTich = table.Column<double>(type: "float", nullable: true),
                    MaHuyenCu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XaCus", x => x.MaXaCu);
                    table.ForeignKey(
                        name: "FK_XaCus_HuyenCus_MaHuyenCu",
                        column: x => x.MaHuyenCu,
                        principalTable: "HuyenCus",
                        principalColumn: "MaHuyenCu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuDiaChis",
                columns: table => new
                {
                    MaLichSuCuTru = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LoaiThayDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoQuyetDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDoThayDoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiaChiCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiMoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NguoiCapNhat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiTao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaCCCD = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    MaXaMoi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuDiaChis", x => x.MaLichSuCuTru);
                    table.ForeignKey(
                        name: "FK_LichSuDiaChis_NguoiDans_MaCCCD",
                        column: x => x.MaCCCD,
                        principalTable: "NguoiDans",
                        principalColumn: "MaCCCD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichSuDiaChis_XaMois_MaXaMoi",
                        column: x => x.MaXaMoi,
                        principalTable: "XaMois",
                        principalColumn: "MaXaMoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TamTrus",
                columns: table => new
                {
                    MaXaMoi = table.Column<int>(type: "int", nullable: false),
                    MaCCCD = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiHan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungDeNghi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TamTrus", x => new { x.MaXaMoi, x.MaCCCD });
                    table.ForeignKey(
                        name: "FK_TamTrus_NguoiDans_MaCCCD",
                        column: x => x.MaCCCD,
                        principalTable: "NguoiDans",
                        principalColumn: "MaCCCD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TamTrus_XaMois_MaXaMoi",
                        column: x => x.MaXaMoi,
                        principalTable: "XaMois",
                        principalColumn: "MaXaMoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThuongTrus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaXaMoi = table.Column<int>(type: "int", nullable: false),
                    MaCCCD = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuongTrus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThuongTrus_NguoiDans_MaCCCD",
                        column: x => x.MaCCCD,
                        principalTable: "NguoiDans",
                        principalColumn: "MaCCCD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThuongTrus_XaMois_MaXaMoi",
                        column: x => x.MaXaMoi,
                        principalTable: "XaMois",
                        principalColumn: "MaXaMoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LssnXas",
                columns: table => new
                {
                    MaLSSN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaXaMoi = table.Column<int>(type: "int", nullable: false),
                    MaXaCu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LssnXas", x => x.MaLSSN);
                    table.ForeignKey(
                        name: "FK_LssnXas_LichSuSapNhaps_MaLSSN",
                        column: x => x.MaLSSN,
                        principalTable: "LichSuSapNhaps",
                        principalColumn: "MaLSSN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LssnXas_XaCus_MaXaCu",
                        column: x => x.MaXaCu,
                        principalTable: "XaCus",
                        principalColumn: "MaXaCu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LssnXas_XaMois_MaXaMoi",
                        column: x => x.MaXaMoi,
                        principalTable: "XaMois",
                        principalColumn: "MaXaMoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HuyenCus_MaTinhCu",
                table: "HuyenCus",
                column: "MaTinhCu");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiaChis_MaCCCD",
                table: "LichSuDiaChis",
                column: "MaCCCD");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiaChis_MaXaMoi",
                table: "LichSuDiaChis",
                column: "MaXaMoi");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuSapNhaps_SoNghiDinh",
                table: "LichSuSapNhaps",
                column: "SoNghiDinh");

            migrationBuilder.CreateIndex(
                name: "IX_LssnTinhs_MaTinhCu",
                table: "LssnTinhs",
                column: "MaTinhCu");

            migrationBuilder.CreateIndex(
                name: "IX_LssnTinhs_MaTinhMoi",
                table: "LssnTinhs",
                column: "MaTinhMoi");

            migrationBuilder.CreateIndex(
                name: "IX_LssnXas_MaXaCu",
                table: "LssnXas",
                column: "MaXaCu");

            migrationBuilder.CreateIndex(
                name: "IX_LssnXas_MaXaMoi",
                table: "LssnXas",
                column: "MaXaMoi");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDans_MaDanToc",
                table: "NguoiDans",
                column: "MaDanToc");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDans_MaQHCH",
                table: "NguoiDans",
                column: "MaQHCH");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDans_MaTDVH",
                table: "NguoiDans",
                column: "MaTDVH");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDans_MaTonGiao",
                table: "NguoiDans",
                column: "MaTonGiao");

            migrationBuilder.CreateIndex(
                name: "IX_TamTrus_MaCCCD",
                table: "TamTrus",
                column: "MaCCCD");

            migrationBuilder.CreateIndex(
                name: "IX_ThuongTrus_MaCCCD",
                table: "ThuongTrus",
                column: "MaCCCD");

            migrationBuilder.CreateIndex(
                name: "IX_ThuongTrus_MaXaMoi",
                table: "ThuongTrus",
                column: "MaXaMoi");

            migrationBuilder.CreateIndex(
                name: "IX_XaCus_MaHuyenCu",
                table: "XaCus",
                column: "MaHuyenCu");

            migrationBuilder.CreateIndex(
                name: "IX_XaMois_MaTinh",
                table: "XaMois",
                column: "MaTinh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "LichSuDiaChis");

            migrationBuilder.DropTable(
                name: "LssnTinhs");

            migrationBuilder.DropTable(
                name: "LssnXas");

            migrationBuilder.DropTable(
                name: "TamTrus");

            migrationBuilder.DropTable(
                name: "ThuongTrus");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "LichSuSapNhaps");

            migrationBuilder.DropTable(
                name: "XaCus");

            migrationBuilder.DropTable(
                name: "NguoiDans");

            migrationBuilder.DropTable(
                name: "XaMois");

            migrationBuilder.DropTable(
                name: "SuKienHanhChinhs");

            migrationBuilder.DropTable(
                name: "HuyenCus");

            migrationBuilder.DropTable(
                name: "DanTocs");

            migrationBuilder.DropTable(
                name: "QuanHeChuHos");

            migrationBuilder.DropTable(
                name: "TonGiaos");

            migrationBuilder.DropTable(
                name: "TrinhDoVanHoas");

            migrationBuilder.DropTable(
                name: "TinhMois");

            migrationBuilder.DropTable(
                name: "TinhCus");
        }
    }
}
