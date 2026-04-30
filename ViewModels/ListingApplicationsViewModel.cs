using rent_for_students.Domain.Entities;

namespace rent_for_students.ViewModels
{
    public class ListingApplicationsViewModel
    {
        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public IReadOnlyList<RentalApplication> Applications { get; set; } = Array.Empty<RentalApplication>();
    }
}
