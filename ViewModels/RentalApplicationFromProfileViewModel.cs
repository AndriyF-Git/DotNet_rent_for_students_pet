using System.ComponentModel.DataAnnotations;

namespace rent_for_students.ViewModels
{
    public class RentalApplicationFromProfileViewModel
    {
        [Required]
        public Guid ListingId { get; set; }

        [Required]
        public Guid ProfileId { get; set; }
    }
}
