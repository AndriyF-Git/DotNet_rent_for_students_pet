using rent_for_students.Application.Common;
using rent_for_students.Application.Notifications;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Reports;
using rent_for_students.Domain.Requests;
using rent_for_students.Domain.Services;

namespace rent_for_students.Application.UseCases
{
    // PROMPT v1.2: Template Method integration - ListingUseCaseMediator
    public class ListingUseCaseMediator : BaseUseCaseMediator, IListingUseCaseMediator
    {
        private readonly HousingService _housingService;
        private readonly IListingReportRepository _reportRepository;

        public ListingUseCaseMediator(
            HousingService housingService,
            INotificationService notificationService,
            IListingReportRepository reportRepository)
            : base(notificationService)
        {
            _housingService = housingService ?? throw new ArgumentNullException(nameof(housingService));
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        }

        public async Task<Result<IReadOnlyList<HousingListing>>> SearchListingsAsync(ListingSearchCriteria criteria, CancellationToken ct = default)
        {
            if (criteria is null)
            {
                return Result<IReadOnlyList<HousingListing>>.Failure(ErrorCodes.ValidationError, "Search criteria is required.");
            }

            IReadOnlyList<HousingListing> listings = await _housingService.SearchListingsAsync(criteria, ct);
            return Result<IReadOnlyList<HousingListing>>.Success(listings);
        }

        public async Task<Result<HousingListing>> GetDetailsAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
            {
                return Result<HousingListing>.Failure(ErrorCodes.ValidationError, "Listing id is invalid.");
            }

            HousingListing? listing = await _housingService.GetListingDetailsAsync(id, ct);
            if (listing is null)
            {
                return Result<HousingListing>.Failure(ErrorCodes.NotFound, "Listing not found.");
            }

            return Result<HousingListing>.Success(listing);
        }

        // PROMPT v1.2: Template Method - listing create flow
        public Task<Result<Guid>> CreateAsync(HousingListing listing, CancellationToken ct = default)
            => ExecuteListingCreateTemplateAsync(listing, ListingCreateFlow.Create, ct);

        // PROMPT v1.2: Template Method - listing draft create flow
        public Task<Result<Guid>> CreateDraftAsync(HousingListing draft, CancellationToken ct = default)
            => ExecuteListingCreateTemplateAsync(draft, ListingCreateFlow.CreateDraft, ct);

        public async Task<Result<bool>> UpdateDraftAsync(Guid id, HousingListing draft, CancellationToken ct = default)
        {
            if (id == Guid.Empty || draft is null)
            {
                return Result<bool>.Failure(ErrorCodes.ValidationError, "Draft update payload is invalid.");
            }

            string? validationMessage = ValidateListing(draft);
            if (validationMessage is not null)
            {
                return Result<bool>.Failure(ErrorCodes.ValidationError, validationMessage);
            }

            bool updated = await _housingService.UpdateListingDraftAsync(id, draft, ct);
            if (!updated)
            {
                return Result<bool>.Failure(ErrorCodes.NotFoundOrNotDraft, "Draft listing was not found or is already published.");
            }

            await _notificationService.NotifyAsync($"Draft updated: {id}", ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> PublishAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
            {
                return Result<bool>.Failure(ErrorCodes.ValidationError, "Listing id is invalid.");
            }

            bool published = await _housingService.PublishListingAsync(id, ct);
            if (!published)
            {
                return Result<bool>.Failure(ErrorCodes.NotFound, "Listing was not found.");
            }

            await _notificationService.NotifyAsync($"Listing published: {id}", ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, HousingListing listing, CancellationToken ct = default)
        {
            if (id == Guid.Empty || listing is null)
            {
                return Result<bool>.Failure(ErrorCodes.ValidationError, "Listing update payload is invalid.");
            }

            string? validationMessage = ValidateListing(listing);
            if (validationMessage is not null)
            {
                return Result<bool>.Failure(ErrorCodes.ValidationError, validationMessage);
            }

            bool updated = await _housingService.UpdateListingAsync(id, listing, ct);
            if (!updated)
            {
                return Result<bool>.Failure(ErrorCodes.NotFound, "Listing was not found.");
            }

            await _notificationService.NotifyAsync($"Listing updated: {id}", ct);
            return Result<bool>.Success(true);
        }

        protected override Task<OperationValidationResult> ValidateListingCreateAsync(
            HousingListing? listing,
            ListingCreateFlow flow,
            CancellationToken ct)
        {
            if (listing is null)
            {
                string missingMessage = flow == ListingCreateFlow.Create
                    ? "Listing is required."
                    : "Draft is required.";

                return Task.FromResult(OperationValidationResult.Invalid(missingMessage, ErrorCodes.ValidationError));
            }

            string? validationMessage = ValidateListing(listing);
            if (validationMessage is not null)
            {
                return Task.FromResult(OperationValidationResult.Invalid(validationMessage, ErrorCodes.ValidationError));
            }

            return Task.FromResult(Valid());
        }

        protected override Task<Guid> ExecuteListingCreateCoreAsync(
            HousingListing listing,
            ListingCreateFlow flow,
            CancellationToken ct)
        {
            return flow switch
            {
                ListingCreateFlow.Create => _housingService.CreateListingAsync(listing, ct),
                ListingCreateFlow.CreateDraft => _housingService.CreateListingDraftAsync(listing, ct),
                _ => throw new NotSupportedException($"Unsupported listing create flow: {flow}.")
            };
        }

        protected override string? BuildListingCreateNotificationMessage(
            HousingListing listing,
            Guid createdId,
            ListingCreateFlow flow)
        {
            return flow switch
            {
                ListingCreateFlow.Create => $"Listing created: {createdId}",
                ListingCreateFlow.CreateDraft => $"Draft created: {createdId}",
                _ => null
            };
        }

        // PROMPT v1.7: Demand report via View + cursor SP
        public async Task<Result<IReadOnlyList<ListingDemandReportRow>>> GetDemandReportAsync(CancellationToken ct = default)
        {
            IReadOnlyList<ListingDemandReportRow> rows = await _reportRepository.GetDemandReportAsync(ct);
            return Result<IReadOnlyList<ListingDemandReportRow>>.Success(rows);
        }

        private static string? ValidateListing(HousingListing listing)
        {
            if (string.IsNullOrWhiteSpace(listing.Title) || listing.Title.Trim().Length < 2)
            {
                return "Title is required and must be at least 2 characters.";
            }

            if (string.IsNullOrWhiteSpace(listing.City))
            {
                return "City is required.";
            }

            if (listing.PricePerMonth <= 0)
            {
                return "Price per month must be greater than 0.";
            }

            if (listing.AreaSqm < 10)
            {
                return "Area must be at least 10 sqm.";
            }

            return null;
        }
    }
}
