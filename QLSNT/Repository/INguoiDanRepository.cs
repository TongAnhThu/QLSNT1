using QLSNT.Models;

namespace QLSNT.Repositories
{
    public interface INguoiDanRepository
    {
        Task<IEnumerable<NguoiDan>> GetAllAsync();   
        Task<IEnumerable<NguoiDan>> SearchAsync(string? keyword);
        Task<NguoiDan?> GetByIdAsync(string maCccd);
        Task AddAsync(NguoiDan entity);
        Task UpdateAsync(NguoiDan entity);

        Task DeleteAsync(string maCccd);
        Task<NguoiDan?> GetByIdentityUserIdAsync(string identityUserId);
        Task<bool> ExistsAsync(string maCCCD);
    }
}
