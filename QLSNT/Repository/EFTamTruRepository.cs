using Microsoft.EntityFrameworkCore;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Repositories
{
    public class EFTamTruRepository : ITamTruRepository
    {
        private readonly ApplicationDbContext _db;

        public EFTamTruRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TamTru>> GetAllAsync()
        {
            return await _db.TamTrus
                .Include(t => t.XaMoi)
                .Include(t => t.NguoiDan)
                .OrderBy(t => t.MaXaMoi)
                .ThenBy(t => t.MaCCCD)
                .ToListAsync();
        }

        public async Task<List<TamTru>> SearchAsync(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllAsync();
            }

            keyword = keyword.Trim();

            var query = _db.TamTrus
                .Include(t => t.XaMoi)
                .Include(t => t.NguoiDan)
                .AsQueryable();

            // Sử dụng ToString() để tìm kiếm với kiểu int (MaXaMoi, MaCCCD)
            return await query
                .Where(t =>
                    EF.Functions.Like(t.MaXaMoi.ToString(), $"%{keyword}%") ||
                    EF.Functions.Like(t.MaCCCD.ToString(), $"%{keyword}%") ||
                    (t.DiaChi != null && EF.Functions.Like(t.DiaChi, $"%{keyword}%")) ||
                    (t.NoiDungDeNghi != null && EF.Functions.Like(t.NoiDungDeNghi, $"%{keyword}%"))
                )
                .OrderBy(t => t.MaXaMoi)
                .ThenBy(t => t.MaCCCD)
                .ToListAsync();
        }

        public async Task<TamTru?> GetByIdAsync(int maXaMoi, string maCCCD)
        {
            return await _db.TamTrus
                .Include(t => t.XaMoi)
                .Include(t => t.NguoiDan)
                .FirstOrDefaultAsync(t => t.MaXaMoi == maXaMoi && t.MaCCCD == maCCCD);
        }

        public async Task AddAsync(TamTru entity)
        {
            _db.TamTrus.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TamTru entity)
        {
            _db.TamTrus.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maXaMoi, string maCCCD)
        {
            var entity = await _db.TamTrus
                .FirstOrDefaultAsync(t => t.MaXaMoi == maXaMoi && t.MaCCCD == maCCCD);

            if (entity != null)
            {
                _db.TamTrus.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<TamTru>> GetTamTruHieuLucByNguoiDanIdAsync(string maNguoiDan)
        {
            var today = DateTime.Today;

            return await _db.TamTrus
                .Include(t => t.XaMoi)
                .ThenInclude(x => x.TinhMoi)
                .Include(t => t.NguoiDan)
                .Where(t =>
                    t.MaCCCD == maNguoiDan &&
                    t.NgayDangKy <= today)
                .OrderByDescending(t => t.NgayDangKy)
                .ToListAsync();
        }
        public async Task<List<TamTru>> GetByNguoiDanAsync(string maCCCD)
        {
            return await _db.TamTrus
           
                .Include(t => t.XaMoi)
                .Include(t => t.NguoiDan)
                .Where(t => t.MaCCCD == maCCCD)
                .ToListAsync();
        }

    }
}
