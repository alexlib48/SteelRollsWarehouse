using SteelRollsWarehouse.Domain.DTOs;
using SteelRollsWarehouse.Domain.Entities;

namespace SteelRollsWarehouse.Infrastructure.Repositories
{
    public interface IRollRepository
    {
        Task<Roll> AddAsync(Roll roll);
        Task<Roll?> GetByIdAsync(int id);
        Task<Roll> UpdateAsync(Roll roll);
        Task<IEnumerable<Roll>> GetAllAsync(RollFilter? filter = null);
        Task<StatisticsResponse> GetStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}