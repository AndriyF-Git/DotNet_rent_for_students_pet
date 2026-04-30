using rent_for_students.Application.Common;
using rent_for_students.Application.Notifications;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.UseCases
{
    /// <summary>
    /// PROMPT v1.2: Classic Template Method integration - BaseUseCaseMediator
    /// 
    /// Base class that contains algorithm skeletons for common mediator flows:
    /// Validate -> Execute -> Notify -> Return Result.
    /// Concrete mediators override required steps and optional hooks.
    /// </summary>
    public abstract class BaseUseCaseMediator
    {
        protected enum ListingCreateFlow
        {
            Create,
            CreateDraft
        }

        protected readonly INotificationService _notificationService;

        protected BaseUseCaseMediator(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        protected Result<T> ValidationFailure<T>(OperationValidationResult validation)
        {
            if (validation.IsValid)
            {
                throw new ArgumentException("Validation result must be invalid.", nameof(validation));
            }

            string errorCode = string.IsNullOrWhiteSpace(validation.ErrorCode)
                ? ErrorCodes.ValidationError
                : validation.ErrorCode;
            string errorMessage = string.IsNullOrWhiteSpace(validation.ErrorMessage)
                ? "Validation failed."
                : validation.ErrorMessage;

            return Result<T>.Failure(errorCode, errorMessage);
        }

        protected Result<T> Success<T>(T value, string? message = null)
            => Result<T>.Success(value, message);

        protected async Task NotifyIfNeededAsync(string? notificationMessage, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(notificationMessage))
            {
                return;
            }

            await _notificationService.NotifyAsync(notificationMessage, ct);
        }

        protected static OperationValidationResult Valid()
            => OperationValidationResult.Valid();

        // PROMPT v1.2: Template Method for listing create/create draft flows.
        protected async Task<Result<Guid>> ExecuteListingCreateTemplateAsync(
            HousingListing? listing,
            ListingCreateFlow flow,
            CancellationToken ct = default)
        {
            OperationValidationResult validation = await ValidateListingCreateAsync(listing, flow, ct);
            if (!validation.IsValid)
            {
                return ValidationFailure<Guid>(validation);
            }

            HousingListing safeListing = listing!;
            await OnBeforeListingCreateExecuteAsync(safeListing, flow, ct);

            Guid createdId = await ExecuteListingCreateCoreAsync(safeListing, flow, ct);

            await OnAfterListingCreateExecuteAsync(safeListing, createdId, flow, ct);
            await NotifyIfNeededAsync(BuildListingCreateNotificationMessage(safeListing, createdId, flow), ct);

            return Success(createdId, BuildListingCreateSuccessMessage(safeListing, createdId, flow));
        }

        // PROMPT v1.2: Template Method for rental application apply flow.
        protected async Task<Result<Guid>> ExecuteApplyTemplateAsync(
            Guid listingId,
            RentalApplication? applicant,
            CancellationToken ct = default)
        {
            OperationValidationResult validation = await ValidateApplyAsync(listingId, applicant, ct);
            if (!validation.IsValid)
            {
                return ValidationFailure<Guid>(validation);
            }

            RentalApplication safeApplicant = applicant!;
            await OnBeforeApplyExecuteAsync(listingId, safeApplicant, ct);

            Guid applicationId = await ExecuteApplyCoreAsync(listingId, safeApplicant, ct);

            await OnAfterApplyExecuteAsync(listingId, safeApplicant, applicationId, ct);
            await NotifyIfNeededAsync(BuildApplyNotificationMessage(listingId, safeApplicant, applicationId), ct);

            return Success(applicationId, BuildApplySuccessMessage(listingId, safeApplicant, applicationId));
        }

        // Required and optional steps for listing create template.
        protected virtual Task<OperationValidationResult> ValidateListingCreateAsync(
            HousingListing? listing,
            ListingCreateFlow flow,
            CancellationToken ct)
            => throw new NotSupportedException("Listing create template is not supported in this mediator.");

        protected virtual Task OnBeforeListingCreateExecuteAsync(
            HousingListing listing,
            ListingCreateFlow flow,
            CancellationToken ct)
            => Task.CompletedTask;

        protected virtual Task<Guid> ExecuteListingCreateCoreAsync(
            HousingListing listing,
            ListingCreateFlow flow,
            CancellationToken ct)
            => throw new NotSupportedException("Listing create template is not supported in this mediator.");

        protected virtual Task OnAfterListingCreateExecuteAsync(
            HousingListing listing,
            Guid createdId,
            ListingCreateFlow flow,
            CancellationToken ct)
            => Task.CompletedTask;

        protected virtual string? BuildListingCreateNotificationMessage(
            HousingListing listing,
            Guid createdId,
            ListingCreateFlow flow)
            => null;

        protected virtual string? BuildListingCreateSuccessMessage(
            HousingListing listing,
            Guid createdId,
            ListingCreateFlow flow)
            => null;

        // Required and optional steps for apply template.
        protected virtual Task<OperationValidationResult> ValidateApplyAsync(
            Guid listingId,
            RentalApplication? applicant,
            CancellationToken ct)
            => throw new NotSupportedException("Apply template is not supported in this mediator.");

        protected virtual Task OnBeforeApplyExecuteAsync(
            Guid listingId,
            RentalApplication applicant,
            CancellationToken ct)
            => Task.CompletedTask;

        protected virtual Task<Guid> ExecuteApplyCoreAsync(
            Guid listingId,
            RentalApplication applicant,
            CancellationToken ct)
            => throw new NotSupportedException("Apply template is not supported in this mediator.");

        protected virtual Task OnAfterApplyExecuteAsync(
            Guid listingId,
            RentalApplication applicant,
            Guid applicationId,
            CancellationToken ct)
            => Task.CompletedTask;

        protected virtual string? BuildApplyNotificationMessage(
            Guid listingId,
            RentalApplication applicant,
            Guid applicationId)
            => null;

        protected virtual string? BuildApplySuccessMessage(
            Guid listingId,
            RentalApplication applicant,
            Guid applicationId)
            => null;
    }
}
