using Microsoft.AspNetCore.Mvc;
using rent_for_students.Application.Commands;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Flyweight;
using rent_for_students.Mapping;
using rent_for_students.ViewModels;

namespace rent_for_students.Controllers
{
    public class ListingsController : Controller
    {
        private readonly CommandDispatcher _dispatcher;
        private readonly IListingUseCaseMediator _listingMediator;
        private readonly RoomTypeFlyweightFactory _roomTypeFactory;

        public ListingsController(
            CommandDispatcher dispatcher,
            IListingUseCaseMediator listingMediator,
            RoomTypeFlyweightFactory roomTypeFactory)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _listingMediator = listingMediator ?? throw new ArgumentNullException(nameof(listingMediator));
            _roomTypeFactory = roomTypeFactory ?? throw new ArgumentNullException(nameof(roomTypeFactory));
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ListingFilterViewModel filter, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var fallbackVm = new ListingListViewModel { Filter = new ListingFilterViewModel() };
                return View(fallbackVm);
            }

            var criteria = ListingMapper.ToCriteria(filter);

            var cmd = new SearchListingsCommand(_listingMediator, criteria);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to load listings.");
            }

            var vm = new ListingListViewModel
            {
                Filter = filter,
                Listings = result.Value ?? Array.Empty<HousingListing>()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();

            var cmd = new GetListingDetailsCommand(_listingMediator, id);
            var result = await _dispatcher.DispatchAsync(cmd, ct);

            if (!result.IsSuccess || result.Value is null) return NotFound();

            return View(new ListingDetailsViewModel { Listing = result.Value });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ListingCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingCreateViewModel form, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            if (form.MinPriceGreaterThanMax())
            {
                ModelState.AddModelError(string.Empty, "Invalid price values.");
                return View(form);
            }

            var entity = ListingMapper.ToEntity(form);

            var cmd = new CreateListingCommand(_listingMediator, entity);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess || result.Value == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create listing.");
                return View(form);
            }

            return RedirectToAction(nameof(Details), new { id = result.Value });
        }

        [HttpGet]
        public IActionResult CreateDraft()
        {
            return View("Create", new ListingCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDraft(ListingCreateViewModel form, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", form);
            }

            var draft = ListingMapper.ToEntity(form);
            var cmd = new CreateListingDraftCommand(_listingMediator, draft);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess || result.Value == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create draft.");
                return View("Create", form);
            }

            return RedirectToAction(nameof(Details), new { id = result.Value });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();

            var detailsCmd = new GetListingDetailsCommand(_listingMediator, id);
            var detailsResult = await _dispatcher.DispatchAsync(detailsCmd, ct);
            if (!detailsResult.IsSuccess || detailsResult.Value is null)
            {
                return NotFound();
            }

            ViewData["IsDraft"] = !detailsResult.Value.IsActive;
            ViewData["ListingId"] = id;

            return View(ListingMapper.ToCreateViewModel(detailsResult.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ListingCreateViewModel form, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var detailsCmd = new GetListingDetailsCommand(_listingMediator, id);
            var detailsResult = await _dispatcher.DispatchAsync(detailsCmd, ct);
            if (!detailsResult.IsSuccess || detailsResult.Value is null)
            {
                return NotFound();
            }

            var listing = ListingMapper.ToEntity(form);

            ICommand<bool> updateCmd = detailsResult.Value.IsActive
                ? new UpdateListingCommand(_listingMediator, id, listing)
                : new UpdateListingDraftCommand(_listingMediator, id, listing);

            var updateResult = await _dispatcher.DispatchAsync(updateCmd, ct);
            if (!updateResult.IsSuccess || updateResult.Value != true)
            {
                ModelState.AddModelError(string.Empty, updateResult.Message ?? "Failed to update listing.");
                ViewData["IsDraft"] = !detailsResult.Value.IsActive;
                ViewData["ListingId"] = id;
                return View(form);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();

            var cmd = new PublishListingCommand(_listingMediator, id);
            var result = await _dispatcher.DispatchAsync(cmd, ct);
            if (!result.IsSuccess || result.Value != true)
            {
                TempData["ListingError"] = result.Message ?? "Failed to publish listing.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }

    internal static class ListingCreateVmExtensions
    {
        public static bool MinPriceGreaterThanMax(this ListingCreateViewModel vm)
        {
            return false;
        }
    }
}
