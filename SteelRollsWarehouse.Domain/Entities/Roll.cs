namespace SteelRollsWarehouse.Domain.Entities
{
    public class Roll
    {
        public int Id { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted => DeletedDate.HasValue;
    }
}