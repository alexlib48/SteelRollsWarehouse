using Microsoft.Extensions.Logging;
using Moq;
using SteelRollsWarehouse.Application.Services;
using SteelRollsWarehouse.Domain.DTOs;
using SteelRollsWarehouse.Domain.Entities;
using SteelRollsWarehouse.Infrastructure.Repositories;

namespace SteelRollsWarehouse.Tests.Services
{
    public class RollServiceTests
    {
        private readonly Mock<IRollRepository> _mockRepository;
        private readonly Mock<ILogger<RollService>> _mockLogger;
        private readonly RollService _service;

        public RollServiceTests()
        {
            _mockRepository = new Mock<IRollRepository>();
            _mockLogger = new Mock<ILogger<RollService>>();
            _service = new RollService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddRollAsync_ValidRequest_ReturnsRollDto()
        {
            var request = new CreateRollRequest { Length = 10.5, Weight = 250.0 };
            var roll = new Roll { Id = 1, Length = 10.5, Weight = 250.0, AddedDate = DateTime.UtcNow };
            
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Roll>()))
                .ReturnsAsync(roll);

            var result = await _service.AddRollAsync(request);

            Assert.NotNull(result);
            Assert.Equal(roll.Id, result.Id);
            Assert.Equal(roll.Length, result.Length);
            Assert.Equal(roll.Weight, result.Weight);
        }

        [Fact]
        public async Task DeleteRollAsync_ExistingRoll_ReturnsDeletedRoll()
        {
            var roll = new Roll { Id = 1, Length = 10.5, Weight = 250.0, AddedDate = DateTime.UtcNow.AddDays(-1) };
            var deletedRoll = new Roll { Id = 1, Length = 10.5, Weight = 250.0, AddedDate = roll.AddedDate, DeletedDate = DateTime.UtcNow };
            
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(roll);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Roll>()))
                .ReturnsAsync(deletedRoll);

            var result = await _service.DeleteRollAsync(1);

            Assert.NotNull(result);
            Assert.NotNull(result.DeletedDate);
        }

        [Fact]
        public async Task DeleteRollAsync_NonExistingRoll_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Roll?)null);

            var result = await _service.DeleteRollAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRollsAsync_WithFilter_ReturnsFilteredRolls()
        {
            var rolls = new List<Roll>
            {
                new Roll { Id = 1, Length = 10, Weight = 200 },
                new Roll { Id = 2, Length = 15, Weight = 300 }
            };
            
            var filter = new RollFilter
            {
                LengthRange = new RangeFilter<double> { From = 5, To = 12 }
            };

            _mockRepository.Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync(rolls.Where(r => r.Length >= 5 && r.Length <= 12).ToList());

            var result = await _service.GetRollsAsync(filter);

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetStatisticsAsync_ValidPeriod_ReturnsStatistics()
        {
            var request = new StatisticsRequest
            {
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow
            };

            var expectedStats = new StatisticsResponse
            {
                AddedCount = 5,
                DeletedCount = 2,
                AverageLength = 12.5,
                AverageWeight = 275.0
            };

            _mockRepository.Setup(r => r.GetStatisticsAsync(request.StartDate, request.EndDate))
                .ReturnsAsync(expectedStats);

            var result = await _service.GetStatisticsAsync(request);

            Assert.NotNull(result);
            Assert.Equal(5, result.AddedCount);
            Assert.Equal(2, result.DeletedCount);
            Assert.Equal(12.5, result.AverageLength);
        }
    }
}