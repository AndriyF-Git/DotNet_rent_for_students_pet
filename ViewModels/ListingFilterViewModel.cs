using System.ComponentModel.DataAnnotations;
using rent_for_students.Domain.Entities;

namespace rent_for_students.ViewModels
{
    public class ListingFilterViewModel
    {
        [StringLength(120)]
        public string? City { get; set; }

        [Range(0, 1_000_000)]
        public decimal? MinPricePerMonth { get; set; }

        [Range(0, 1_000_000)]
        public decimal? MaxPricePerMonth { get; set; }

        public RoomType? RoomType { get; set; }

        public bool OnlyActive { get; set; } = true;

        [Range(1, 10_000)]
        public int Page { get; set; } = 1;

        [Range(1, 200)]
        public int PageSize { get; set; } = 20;
    }
}
