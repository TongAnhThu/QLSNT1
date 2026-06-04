using Microsoft.EntityFrameworkCore;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Repositories
{
    public class EFThuongTruRepository : IThuongTruRepository
    {
        private readonly ApplicationDbContext _context;

        public EFThuongTruRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThuongTru>> GetAllAsync()
            => await _context.ThuongTrus.Include(x => x.XaMoi).Include(x => x.NguoiDan).ToListAsync();

        public async Task<ThuongTru?> GetByCCCDAsync(string maCCCD)
            => await _context.ThuongTrus.FirstOrDefaultAsync(x => x.MaCCCD == maCCCD);

        public async Task<IEnumerable<ThuongTru>> SearchAsync(string? keyword)
            => await _context.ThuongTrus
                .Where(x => string.IsNullOrEmpty(keyword)
                         || x.MaCCCD.Contains(keyword)
                         || x.DiaChi!.Contains(keyword))
                .ToListAsync();

        public async Task<ThuongTru?> GetByIdAsync(int maXaMoi, string maCccd)
            => await _context.ThuongTrus.FindAsync(maXaMoi, maCccd);

        public async Task<IEnumerable<ThuongTru>> GetByXaMoiAsync(int maXaMoi)
            => await _context.ThuongTrus.Where(x => x.MaXaMoi == maXaMoi).ToListAsync();

        public async Task<IEnumerable<ThuongTru>> GetByNguoiDanAsync(string maCccd)
            => await _context.ThuongTrus.Where(x => x.MaCCCD == maCccd).ToListAsync();

        public async Task AddAsync(ThuongTru entity)
        {
            _context.ThuongTrus.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThuongTru entity)
        {
            // Chỉ update các field không phải khóa
            var existing = await _context.ThuongTrus.FindAsync(entity.MaXaMoi, entity.MaCCCD);
            if (existing != null)
            {
                existing.DiaChi = entity.DiaChi;
                existing.NgayDangKy = entity.NgayDangKy;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int maXaMoi, string maCccd)
        {
            var existing = await _context.ThuongTrus.FindAsync(maXaMoi, maCccd);
            if (existing != null)
            {
                _context.ThuongTrus.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ThuongTru?> GetThuongTruHienTaiByNguoiDanIdAsync(string maNguoiDan)
        {
            return await _context.ThuongTrus
            .Include(x => x.XaMoi)
            .ThenInclude(x => x.TinhMoi)
            .Include(x => x.NguoiDan)
            .Where(x => x.MaCCCD == maNguoiDan)
            .OrderByDescending(x => x.NgayDangKy)
            .FirstOrDefaultAsync();
        }

    }
}
