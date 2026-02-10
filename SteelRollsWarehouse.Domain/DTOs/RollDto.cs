namespace SteelRollsWarehouse.Domain.DTOs
{
    public class RollDto
    {
        public int Id { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

    public class CreateRollRequest
    {
        [Required(ErrorMessage = "Length is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        public double Length { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public double Weight { get; set; }
    }

    public class RangeFilter<T>
    {
        public T? From { get; set; }
        public T? To { get; set; }
    }

    public class RollFilter
    {
        public RangeFilter<int>? IdRange { get; set; }
        public RangeFilter<double>? LengthRange { get; set; }
        public RangeFilter<double>? WeightRange { get; set; }
        public RangeFilter<DateTime>? AddedDateRange { get; set; }
        public RangeFilter<DateTime>? DeletedDateRange { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class StatisticsRequest
    {
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
    }

    public class StatisticsResponse
    {
        public int AddedCount { get; set; }
        public int DeletedCount { get; set; }
        public double AverageLength { get; set; }
        public double AverageWeight { get; set; }
        public double MinLength { get; set; }
        public double MaxLength { get; set; }
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }
        public double TotalWeight { get; set; }
        public double MinStorageDuration { get; set; }
        public double MaxStorageDuration { get; set; }
        
        public DateTime? DayWithMinRollsCount { get; set; }
        public DateTime? DayWithMaxRollsCount { get; set; }
        public DateTime? DayWithMinTotalWeight { get; set; }
        public DateTime? DayWithMaxTotalWeight { get; set; }
        public int MinRollsCount { get; set; }
        public int MaxRollsCount { get; set; }
        public double MinDayTotalWeight { get; set; }
        public double MaxDayTotalWeight { get; set; }
    }
}