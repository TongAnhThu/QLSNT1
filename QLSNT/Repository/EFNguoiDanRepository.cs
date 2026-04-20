using Microsoft.EntityFrameworkCore;
using QLSNT.Data;
using QLSNT.Models;

namespace QLSNT.Repositories
{
    public class EFNguoiDanRepository : INguoiDanRepository
    {
        private readonly ApplicationDbContext _context;

        public EFNguoiDanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NguoiDan>> GetAllAsync()
        {
            return await _context.NguoiDans
                // Include nếu bạn có navigation (tùy model của bạn)
                .Include(n => n.DanToc)
                .Include(n => n.TonGiao)
                .Include(n => n.TrinhDoVanHoa)
                .Include(n => n.QuanHeChuHo)
                .AsNoTracking()
                .OrderBy(n => n.MaCCCD)
                .ToListAsync();
        }

        public async Task<IEnumerable<NguoiDan>> SearchAsync(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllAsync();
            }

            keyword = keyword.Trim();

            var query = _context.NguoiDans
                .Include(n => n.DanToc)
                .Include(n => n.TonGiao)
                .Include(n => n.TrinhDoVanHoa)
                .Include(n => n.QuanHeChuHo)
                .AsQueryable();

            return await query
                .Where(n =>
                    EF.Functions.Like(n.MaCCCD, $"%{keyword}%") ||
                    EF.Functions.Like(
                        EF.Functions.Collate(n.HoTen, "SQL_Latin1_General_CP1_CI_AI"),
                        $"%{keyword}%"
                    )
                )
                .AsNoTracking()
                .OrderBy(n => n.HoTen)
                .ToListAsync();
        }

        public async Task<NguoiDan?> GetByIdAsync(string maCccd)
        {
            if (string.IsNullOrWhiteSpace(maCccd))
                return null;

            maCccd = maCccd.Trim();

            return await _context.NguoiDans
                .Include(n => n.DanToc)
                .Include(n => n.TonGiao)
                .Include(n => n.TrinhDoVanHoa)
                .Include(n => n.QuanHeChuHo)
                .FirstOrDefaultAsync(n => n.MaCCCD == maCccd);
        }

        public async Task AddAsync(NguoiDan entity)
        {
            await _context.NguoiDans.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NguoiDan entity)
        {
            _context.NguoiDans.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string maCccd)
        {
            if (string.IsNullOrWhiteSpace(maCccd))
                return;

            maCccd = maCccd.Trim();

            var existing = await _context.NguoiDans
                .FirstOrDefaultAsync(n => n.MaCCCD == maCccd);

            if (existing != null)
            {
                _context.NguoiDans.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<NguoiDan?> GetByIdentityUserIdAsync(string cccd)
        {
            return await _context.NguoiDans
        .FirstOrDefaultAsync(nd => nd.MaCCCD == cccd);
        }
        public async Task<bool> ExistsAsync(string maCCCD)
        {
            return await _context.NguoiDans.AnyAsync(x => x.MaCCCD == maCCCD);
        }

    }
}
