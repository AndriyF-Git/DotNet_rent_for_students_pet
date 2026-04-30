using Microsoft.AspNetCore.Mvc;
using rent_for_students.Application.Commands;
using rent_for_students.Application.UseCases;
using rent_for_students.ViewModels;

namespace rent_for_students.Controllers
{
    // PROMPT v1.7: Reports controller — demand report via View + cursor SP
    public class ReportsController : Controller
    {
        private readonly CommandDispatcher _dispatcher;
        private readonly IListingUseCaseMediator _listingMediator;

        public ReportsController(
            CommandDispatcher dispatcher,
            IListingUseCaseMediator listingMediator)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _listingMediator = listingMediator ?? throw new ArgumentNullException(nameof(listingMediator));
        }

        [HttpGet]
        public async Task<IActionResult> DemandReport(CancellationToken ct)
        {
            var cmd = new GetListingDemandReportCommand(_listingMediator);
            var result = await _dispatcher.DispatchAsync(cmd, ct);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to load demand report.");
                return View(new ListingDemandReportViewModel());
            }

            return View(new ListingDemandReportViewModel { Rows = result.Value ?? [] });
        }
    }
}
