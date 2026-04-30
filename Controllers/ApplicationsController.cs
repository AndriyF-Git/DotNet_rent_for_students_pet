using Microsoft.AspNetCore.Mvc;
using rent_for_students.Application.Common;
using rent_for_students.Application.Commands;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.ViewModels;

namespace rent_for_students.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly CommandDispatcher _dispatcher;
        private readonly IListingUseCaseMediator _listingMediator;
        private readonly IApplicationUseCaseMediator _applicationMediator;

        public ApplicationsController(
            CommandDispatcher dispatcher,
            IListingUseCaseMediator listingMediator,
            IApplicationUseCaseMediator applicationMediator)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _listingMediator = listingMediator ?? throw new ArgumentNullException(nameof(listingMediator));
            _applicationMediator = applicationMediator ?? throw new ArgumentNullException(nameof(applicationMediator));
        }

        [HttpGet]
        public async Task<IActionResult> Apply(Guid listingId, CancellationToken ct)
        {
            if (listingId == Guid.Empty) return BadRequest();

            var detailsCmd = new GetListingDetailsCommand(_listingMediator, listingId);
            var detailsResult = await _dispatcher.DispatchAsync(detailsCmd, ct);
            if (!detailsResult.IsSuccess || detailsResult.Value is null || !detailsResult.Value.IsActive)
            {
                return NotFound();
            }

            ViewData["ListingTitle"] = detailsResult.Value.Title;
            await PopulateProfileViewDataAsync(ct);
            return View(new RentalApplicationCreateViewModel { ListingId = listingId });
        }

        [HttpGet]
        public async Task<IActionResult> List(Guid listingId, CancellationToken ct)
        {
            if (listingId == Guid.Empty) return BadRequest();

            var detailsCmd = new GetListingDetailsCommand(_listingMediator, listingId);
            var detailsResult = await _dispatcher.DispatchAsync(detailsCmd, ct);
            if (!detailsResult.IsSuccess || detailsResult.Value is null)
            {
                return NotFound();
            }

            var cmd = new GetListingApplicationsCommand(_applicationMediator, listingId);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return View(new ListingApplicationsViewModel
            {
                ListingId = listingId,
                ListingTitle = detailsResult.Value.Title,
                Applications = result.Value ?? Array.Empty<RentalApplication>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(RentalApplicationCreateViewModel model, CancellationToken ct)
        {
            await FillListingTitleAsync(model.ListingId, ct);
            await PopulateProfileViewDataAsync(ct);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.SaveAsProfile && string.IsNullOrWhiteSpace(model.ProfileName))
            {
                ModelState.AddModelError(nameof(model.ProfileName), "Profile name is required when saving profile.");
                return View(model);
            }

            var applicant = new RentalApplication
            {
                ApplicantName = model.ApplicantName.Trim(),
                Phone = model.Phone.Trim(),
                Email = model.Email.Trim(),
                Message = string.IsNullOrWhiteSpace(model.Message) ? null : model.Message.Trim()
            };

            if (model.SaveAsProfile)
            {
                var profile = new RentalApplicationProfile
                {
                    ProfileName = model.ProfileName!.Trim(),
                    ApplicantName = applicant.ApplicantName,
                    Phone = applicant.Phone,
                    Email = applicant.Email,
                    Message = applicant.Message
                };

                var saveProfileCmd = new CreateRentalApplicationProfileCommand(_applicationMediator, profile);
                var saveProfileResult = await _dispatcher.DispatchAsync(saveProfileCmd, ct);
                if (!saveProfileResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, saveProfileResult.Message ?? "Failed to save profile.");
                    return View(model);
                }
            }

            var cmd = new CreateRentalApplicationCommand(_applicationMediator, model.ListingId, applicant);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess)
            {
                if (result.ErrorCode == ErrorCodes.ListingNotAvailable)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to submit application.");
                return View(model);
            }

            TempData["ListingMessage"] = "Application submitted. Status: Approved.";
            return RedirectToAction("Details", "Listings", new { id = model.ListingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyFromProfile(RentalApplicationFromProfileViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid || model.ListingId == Guid.Empty || model.ProfileId == Guid.Empty)
            {
                return BadRequest();
            }

            var cmd = new CreateRentalApplicationFromProfileCommand(_applicationMediator, model.ListingId, model.ProfileId);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess)
            {
                if (result.ErrorCode == ErrorCodes.ListingNotAvailable || result.ErrorCode == ErrorCodes.NotFound)
                {
                    return NotFound();
                }

                TempData["ListingError"] = result.Message ?? "Failed to submit application from profile.";
                return RedirectToAction(nameof(Apply), new { listingId = model.ListingId });
            }

            TempData["ListingMessage"] = "Application from saved profile submitted. Status: Approved.";
            return RedirectToAction("Details", "Listings", new { id = model.ListingId });
        }

        private async Task FillListingTitleAsync(Guid listingId, CancellationToken ct)
        {
            if (listingId == Guid.Empty)
            {
                return;
            }

            var detailsCmd = new GetListingDetailsCommand(_listingMediator, listingId);
            var detailsResult = await _dispatcher.DispatchAsync(detailsCmd, ct);
            if (detailsResult.IsSuccess && detailsResult.Value is not null)
            {
                ViewData["ListingTitle"] = detailsResult.Value.Title;
            }
        }

        private async Task PopulateProfileViewDataAsync(CancellationToken ct)
        {
            var cmd = new GetRentalApplicationProfilesCommand(_applicationMediator);
            var profilesResult = await _dispatcher.DispatchAsync(cmd, ct);
            ViewData["ApplicationProfiles"] = profilesResult.IsSuccess
                ? profilesResult.Value ?? Array.Empty<IRentalApplicationPrototype>()
                : Array.Empty<IRentalApplicationPrototype>();
        }
    }
}
