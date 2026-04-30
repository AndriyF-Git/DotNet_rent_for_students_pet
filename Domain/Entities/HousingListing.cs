using System.ComponentModel.DataAnnotations;

namespace rent_for_students.Domain.Entities
{
    public class HousingListing
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(120)]
        public string City { get; set; } = string.Empty;

        [StringLength(120)]
        public string? District { get; set; }

        [Range(1, 1_000_000)]
        public decimal PricePerMonth { get; set; }

        public RoomType RoomType { get; set; }

        [Range(10, 10_000)]
        public int AreaSqm { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
