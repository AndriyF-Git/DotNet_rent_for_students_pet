using rent_for_students.Domain.Entities;

namespace rent_for_students.ViewModels
{
    public class ListingListViewModel
    {
        public ListingFilterViewModel Filter { get; set; } = new();
        public IReadOnlyList<HousingListing> Listings { get; set; } = Array.Empty<HousingListing>();
    }
}
