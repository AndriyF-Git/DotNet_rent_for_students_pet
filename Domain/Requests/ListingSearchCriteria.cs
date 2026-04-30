using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Requests
{
    public sealed class ListingSearchCriteria
    {
        public string? City { get; set; }
        public decimal? MinPricePerMonth { get; set; }
        public decimal? MaxPricePerMonth { get; set; }
        public RoomType? RoomType { get; set; }
        public bool? OnlyActive { get; set; } = true;

        // simple paging (на майбутнє, щоб легко розширювати)
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
