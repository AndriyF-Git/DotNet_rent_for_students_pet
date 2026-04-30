using System.ComponentModel.DataAnnotations;
using rent_for_students.Domain.Contracts;

namespace rent_for_students.Domain.Entities
{
    // PROMPT v1.4: Baseline Prototype - concrete object clones itself.
    public class RentalApplicationProfile : IRentalApplicationPrototype
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(80, MinimumLength = 2)]
        public string ProfileName { get; set; } = string.Empty;

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string ApplicantName { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Message { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public RentalApplicationProfile()
        {
        }

        private RentalApplicationProfile(RentalApplicationProfile prototype)
        {
            Id = prototype.Id;
            ProfileName = prototype.ProfileName;
            ApplicantName = prototype.ApplicantName;
            Phone = prototype.Phone;
            Email = prototype.Email;
            Message = prototype.Message;
            CreatedAtUtc = prototype.CreatedAtUtc;
            UpdatedAtUtc = prototype.UpdatedAtUtc;
        }

        public IRentalApplicationPrototype Clone()
            => new RentalApplicationProfile(this);

        public RentalApplication ToRentalApplication(Guid listingId)
        {
            return new RentalApplication
            {
                Id = Guid.NewGuid(),
                ListingId = listingId,
                ApplicantName = ApplicantName,
                Phone = Phone,
                Email = Email,
                Message = Message,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        // PROMPT v1.4: Compatibility wrapper for existing v1.3 call sites.
        public RentalApplication CreateApplicationPrototype(Guid listingId)
            => ToRentalApplication(listingId);
    }
}
