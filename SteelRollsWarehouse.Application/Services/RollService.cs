using SteelRollsWarehouse.Domain.DTOs;
using SteelRollsWarehouse.Domain.Entities;
using SteelRollsWarehouse.Infrastructure.Repositories;

namespace SteelRollsWarehouse.Application.Services
{
    public class RollService : IRollService
    {
        private readonly IRollRepository _repository;
        private readonly ILogger<RollService> _logger;

        public RollService(IRollRepository repository, ILogger<RollService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<RollDto> AddRollAsync(CreateRollRequest request)
        {
            var roll = new Roll
            {
                Length = request.Length,
                Weight = request.Weight,
                AddedDate = DateTime.UtcNow
            };

            var addedRoll = await _repository.AddAsync(roll);
            return MapToDto(addedRoll);
        }

        public async Task<RollDto?> DeleteRollAsync(int id)
        {
            var roll = await _repository.GetByIdAsync(id);
            
            if (roll == null)
                return null;

            if (roll.IsDeleted)
                throw new InvalidOperationException($"Roll with id {id} is already deleted");

            roll.DeletedDate = DateTime.UtcNow;
            var updatedRoll = await _repository.UpdateAsync(roll);
            
            return MapToDto(updatedRoll);
        }

        public async Task<IEnumerable<RollDto>> GetRollsAsync(RollFilter? filter = null)
        {
            var rolls = await _repository.GetAllAsync(filter);
            return rolls.Select(MapToDto);
        }

        public async Task<StatisticsResponse> GetStatisticsAsync(StatisticsRequest request)
        {
            if (request.StartDate > request.EndDate)
                throw new ArgumentException("Start date must be less than or equal to end date");

            return await _repository.GetStatisticsAsync(request.StartDate, request.EndDate);
        }

        private RollDto MapToDto(Roll roll)
        {
            return new RollDto
            {
                Id = roll.Id,
                Length = roll.Length,
                Weight = roll.Weight,
                AddedDate = roll.AddedDate,
                DeletedDate = roll.DeletedDate
            };
        }
    }
}