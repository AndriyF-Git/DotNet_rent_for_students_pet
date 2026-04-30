using rent_for_students.Application.Common;
using rent_for_students.Application.Notifications;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Services;

namespace rent_for_students.Application.UseCases
{
    // PROMPT v1.3: Template Method + Prototype integration - ApplicationUseCaseMediator
    public class ApplicationUseCaseMediator : BaseUseCaseMediator, IApplicationUseCaseMediator
    {
        private readonly HousingService _housingService;
        private readonly IRentalApplicationRepository _rentalApplicationRepository;
        private readonly IRentalApplicationProfileRepository _profileRepository;

        public ApplicationUseCaseMediator(
            HousingService housingService,
            IRentalApplicationRepository rentalApplicationRepository,
            IRentalApplicationProfileRepository profileRepository,
            INotificationService notificationService)
            : base(notificationService)
        {
            _housingService = housingService ?? throw new ArgumentNullException(nameof(housingService));
            _rentalApplicationRepository = rentalApplicationRepository ?? throw new ArgumentNullException(nameof(rentalApplicationRepository));
            _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        }

        // PROMPT v1.2: Template Method - ApplyAsync operation
        public Task<Result<Guid>> ApplyAsync(Guid listingId, RentalApplication applicant, CancellationToken ct = default)
            => ExecuteApplyTemplateAsync(listingId, applicant, ct);

        // PROMPT v1.3: Prototype flow - create application from saved profile.
        public async Task<Result<Guid>> ApplyFromProfileAsync(Guid listingId, Guid profileId, CancellationToken ct = default)
        {
            if (profileId == Guid.Empty)
            {
                return Result<Guid>.Failure(ErrorCodes.ValidationError, "Profile id is invalid.");
            }

            IRentalApplicationPrototype? prototype = await _profileRepository.GetByIdAsync(profileId, ct);
            if (prototype is null)
            {
                return Result<Guid>.Failure(ErrorCodes.NotFound, "Application profile was not found.");
            }

            IRentalApplicationPrototype clone = prototype.Clone();
            RentalApplication applicant = clone.ToRentalApplication(listingId);
            return await ExecuteApplyTemplateAsync(listingId, applicant, ct);
        }

        // PROMPT v1.4: Prototype source management through baseline interface.
        public async Task<Result<Guid>> CreateProfileAsync(IRentalApplicationPrototype prototype, CancellationToken ct = default)
        {
            if (prototype is null)
            {
                return Result<Guid>.Failure(ErrorCodes.ValidationError, "Application profile is required.");
            }

            string? validationMessage = ValidateProfile(prototype);
            if (validationMessage is not null)
            {
                return Result<Guid>.Failure(ErrorCodes.ValidationError, validationMessage);
            }

            prototype.Id = prototype.Id == Guid.Empty ? Guid.NewGuid() : prototype.Id;
            prototype.CreatedAtUtc = prototype.CreatedAtUtc == default ? DateTime.UtcNow : prototype.CreatedAtUtc;
            prototype.UpdatedAtUtc = DateTime.UtcNow;

            await _profileRepository.AddAsync(prototype, ct);
            await _profileRepository.SaveChangesAsync(ct);
            await NotifyIfNeededAsync($"Application profile created: profileId={prototype.Id}, name={prototype.ProfileName}", ct);

            return Success(prototype.Id, "Application profile was created.");
        }

        // PROMPT v1.4: Prototype source management - list via interface.
        public async Task<Result<IReadOnlyList<IRentalApplicationPrototype>>> ListProfilesAsync(CancellationToken ct = default)
        {
            IReadOnlyList<IRentalApplicationPrototype> prototypes = await _profileRepository.ListAsync(ct);
            return Result<IReadOnlyList<IRentalApplicationPrototype>>.Success(prototypes);
        }

        public async Task<Result<IReadOnlyList<RentalApplication>>> ListByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            if (listingId == Guid.Empty)
            {
                return Result<IReadOnlyList<RentalApplication>>.Failure(ErrorCodes.ValidationError, "Listing id is invalid.");
            }

            HousingListing? listing = await _housingService.GetListingDetailsAsync(listingId, ct);
            if (listing is null)
            {
                return Result<IReadOnlyList<RentalApplication>>.Failure(ErrorCodes.NotFound, "Listing not found.");
            }

            IReadOnlyList<RentalApplication> applications = await _rentalApplicationRepository.ListByListingIdAsync(listingId, ct);
            return Result<IReadOnlyList<RentalApplication>>.Success(applications);
        }

        protected override async Task<OperationValidationResult> ValidateApplyAsync(
            Guid listingId,
            RentalApplication? applicant,
            CancellationToken ct)
        {
            if (listingId == Guid.Empty || applicant is null)
            {
                return OperationValidationResult.Invalid("Application payload is invalid.", ErrorCodes.ValidationError);
            }

            string? validationMessage = ValidateApplicant(applicant);
            if (validationMessage is not null)
            {
                return OperationValidationResult.Invalid(validationMessage, ErrorCodes.ValidationError);
            }

            HousingListing? listing = await _housingService.GetListingDetailsAsync(listingId, ct);
            if (listing is null || !listing.IsActive)
            {
                return OperationValidationResult.Invalid("Listing not found or not published.", ErrorCodes.ListingNotAvailable);
            }

            return Valid();
        }

        protected override Task OnBeforeApplyExecuteAsync(
            Guid listingId,
            RentalApplication applicant,
            CancellationToken ct)
        {
            applicant.Id = applicant.Id == Guid.Empty ? Guid.NewGuid() : applicant.Id;
            applicant.ListingId = listingId;
            applicant.CreatedAtUtc = applicant.CreatedAtUtc == default ? DateTime.UtcNow : applicant.CreatedAtUtc;
            applicant.Approve();

            return Task.CompletedTask;
        }

        protected override async Task<Guid> ExecuteApplyCoreAsync(
            Guid listingId,
            RentalApplication applicant,
            CancellationToken ct)
        {
            await _rentalApplicationRepository.AddAsync(applicant, ct);
            await _rentalApplicationRepository.SaveChangesAsync(ct);
            return applicant.Id;
        }

        protected override string? BuildApplyNotificationMessage(
            Guid listingId,
            RentalApplication applicant,
            Guid applicationId)
            => $"Rental application created: applicationId={applicationId}, listingId={listingId}, status={applicant.Status}";

        protected override string? BuildApplySuccessMessage(
            Guid listingId,
            RentalApplication applicant,
            Guid applicationId)
            => "Rental application was created.";

        private static string? ValidateApplicant(RentalApplication applicant)
        {
            if (string.IsNullOrWhiteSpace(applicant.ApplicantName))
            {
                return "Applicant name is required.";
            }

            if (string.IsNullOrWhiteSpace(applicant.Phone))
            {
                return "Phone is required.";
            }

            if (string.IsNullOrWhiteSpace(applicant.Email) || !applicant.Email.Contains('@'))
            {
                return "Valid email is required.";
            }

            return null;
        }

        private static string? ValidateProfile(IRentalApplicationPrototype prototype)
        {
            if (string.IsNullOrWhiteSpace(prototype.ProfileName) || prototype.ProfileName.Trim().Length < 2)
            {
                return "Profile name is required and must be at least 2 characters.";
            }

            if (string.IsNullOrWhiteSpace(prototype.ApplicantName))
            {
                return "Applicant name is required.";
            }

            if (string.IsNullOrWhiteSpace(prototype.Phone))
            {
                return "Phone is required.";
            }

            if (string.IsNullOrWhiteSpace(prototype.Email) || !prototype.Email.Contains('@'))
            {
                return "Valid email is required.";
            }

            return null;
        }
    }
}
