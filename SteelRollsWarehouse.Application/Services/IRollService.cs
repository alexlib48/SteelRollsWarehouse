using SteelRollsWarehouse.Domain.DTOs;
using SteelRollsWarehouse.Domain.Entities;

namespace SteelRollsWarehouse.Application.Services
{
    public interface IRollService
    {
        Task<RollDto> AddRollAsync(CreateRollRequest request);
        Task<RollDto?> DeleteRollAsync(int id);
        Task<IEnumerable<RollDto>> GetRollsAsync(RollFilter? filter = null);
        Task<StatisticsResponse> GetStatisticsAsync(StatisticsRequest request);
    }
}