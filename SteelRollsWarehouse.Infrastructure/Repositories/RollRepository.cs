using Microsoft.EntityFrameworkCore;
using SteelRollsWarehouse.Domain.DTOs;
using SteelRollsWarehouse.Domain.Entities;
using SteelRollsWarehouse.Infrastructure.Data;

namespace SteelRollsWarehouse.Infrastructure.Repositories
{
    public class RollRepository : IRollRepository
    {
        private readonly ApplicationDbContext _context;

        public RollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Roll> AddAsync(Roll roll)
        {
            _context.Rolls.Add(roll);
            await _context.SaveChangesAsync();
            return roll;
        }

        public async Task<Roll?> GetByIdAsync(int id)
        {
            return await _context.Rolls.FindAsync(id);
        }

        public async Task<Roll> UpdateAsync(Roll roll)
        {
            _context.Entry(roll).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return roll;
        }

        public async Task<IEnumerable<Roll>> GetAllAsync(RollFilter? filter = null)
        {
            var query = _context.Rolls.AsQueryable();

            if (filter != null)
            {
                if (filter.IdRange != null)
                {
                    if (filter.IdRange.From.HasValue)
                        query = query.Where(r => r.Id >= filter.IdRange.From.Value);
                    if (filter.IdRange.To.HasValue)
                        query = query.Where(r => r.Id <= filter.IdRange.To.Value);
                }

                if (filter.LengthRange != null)
                {
                    if (filter.LengthRange.From.HasValue)
                        query = query.Where(r => r.Length >= filter.LengthRange.From.Value);
                    if (filter.LengthRange.To.HasValue)
                        query = query.Where(r => r.Length <= filter.LengthRange.To.Value);
                }

                if (filter.WeightRange != null)
                {
                    if (filter.WeightRange.From.HasValue)
                        query = query.Where(r => r.Weight >= filter.WeightRange.From.Value);
                    if (filter.WeightRange.To.HasValue)
                        query = query.Where(r => r.Weight <= filter.WeightRange.To.Value);
                }

                if (filter.AddedDateRange != null)
                {
                    if (filter.AddedDateRange.From.HasValue)
                        query = query.Where(r => r.AddedDate >= filter.AddedDateRange.From.Value);
                    if (filter.AddedDateRange.To.HasValue)
                        query = query.Where(r => r.AddedDate <= filter.AddedDateRange.To.Value);
                }

                if (filter.DeletedDateRange != null)
                {
                    if (filter.DeletedDateRange.From.HasValue)
                        query = query.Where(r => r.DeletedDate >= filter.DeletedDateRange.From.Value);
                    if (filter.DeletedDateRange.To.HasValue)
                        query = query.Where(r => r.DeletedDate <= filter.DeletedDateRange.To.Value);
                }

                if (filter.IsDeleted.HasValue)
                {
                    query = filter.IsDeleted.Value
                        ? query.Where(r => r.DeletedDate != null)
                        : query.Where(r => r.DeletedDate == null);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<StatisticsResponse> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var rollsInPeriod = await _context.Rolls
                .Where(r => (r.AddedDate <= endDate && (!r.DeletedDate.HasValue || r.DeletedDate >= startDate)))
                .ToListAsync();

            var response = new StatisticsResponse
            {
                AddedCount = rollsInPeriod.Count(r => r.AddedDate >= startDate && r.AddedDate <= endDate),
                DeletedCount = rollsInPeriod.Count(r => r.DeletedDate.HasValue && 
                    r.DeletedDate.Value >= startDate && r.DeletedDate.Value <= endDate),
                AverageLength = rollsInPeriod.Any() ? rollsInPeriod.Average(r => r.Length) : 0,
                AverageWeight = rollsInPeriod.Any() ? rollsInPeriod.Average(r => r.Weight) : 0,
                MinLength = rollsInPeriod.Any() ? rollsInPeriod.Min(r => r.Length) : 0,
                MaxLength = rollsInPeriod.Any() ? rollsInPeriod.Max(r => r.Length) : 0,
                MinWeight = rollsInPeriod.Any() ? rollsInPeriod.Min(r => r.Weight) : 0,
                MaxWeight = rollsInPeriod.Any() ? rollsInPeriod.Max(r => r.Weight) : 0,
                TotalWeight = rollsInPeriod.Sum(r => r.Weight)
            };

            var deletedRolls = rollsInPeriod.Where(r => r.DeletedDate.HasValue).ToList();
            if (deletedRolls.Any())
            {
                var durations = deletedRolls
                    .Select(r => (r.DeletedDate!.Value - r.AddedDate).TotalDays)
                    .ToList();
                
                response.MinStorageDuration = durations.Min();
                response.MaxStorageDuration = durations.Max();
            }

            var dailyStats = await CalculateDailyStatistics(startDate, endDate);
            if (dailyStats.Any())
            {
                var maxRollsDay = dailyStats.OrderByDescending(d => d.RollsCount).First();
                var minRollsDay = dailyStats.OrderBy(d => d.RollsCount).First();
                var maxWeightDay = dailyStats.OrderByDescending(d => d.TotalWeight).First();
                var minWeightDay = dailyStats.OrderBy(d => d.TotalWeight).First();

                response.DayWithMaxRollsCount = maxRollsDay.Date;
                response.DayWithMinRollsCount = minRollsDay.Date;
                response.MaxRollsCount = maxRollsDay.RollsCount;
                response.MinRollsCount = minRollsDay.RollsCount;
                response.DayWithMaxTotalWeight = maxWeightDay.Date;
                response.DayWithMinTotalWeight = minWeightDay.Date;
                response.MaxDayTotalWeight = maxWeightDay.TotalWeight;
                response.MinDayTotalWeight = minWeightDay.TotalWeight;
            }

            return response;
        }

        private async Task<List<DailyStat>> CalculateDailyStatistics(DateTime startDate, DateTime endDate)
        {
            var dailyStats = new List<DailyStat>();
            var allRolls = await _context.Rolls.ToListAsync();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var rollsOnDate = allRolls
                    .Where(r => r.AddedDate.Date <= date && 
                              (!r.DeletedDate.HasValue || r.DeletedDate.Value.Date >= date))
                    .ToList();

                dailyStats.Add(new DailyStat
                {
                    Date = date,
                    RollsCount = rollsOnDate.Count,
                    TotalWeight = rollsOnDate.Sum(r => r.Weight)
                });
            }

            return dailyStats;
        }

        private class DailyStat
        {
            public DateTime Date { get; set; }
            public int RollsCount { get; set; }
            public double TotalWeight { get; set; }
        }
    }
}