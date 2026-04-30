using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Contracts
{
    // PROMPT v1.4: Baseline Prototype contract for profile cloning.
    public interface IRentalApplicationPrototype
    {
        Guid Id { get; set; }
        string ProfileName { get; set; }
        string ApplicantName { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
        string? Message { get; set; }
        DateTime CreatedAtUtc { get; set; }
        DateTime UpdatedAtUtc { get; set; }

        IRentalApplicationPrototype Clone();
        RentalApplication ToRentalApplication(Guid listingId);
    }
}
