using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Requests;
using rent_for_students.Domain.Services;
using rent_for_students.Tests.TestDoubles;

namespace rent_for_students.Tests
{
    public class HousingServiceTests
    {
        private readonly InMemoryHousingRepository _repository;
        private readonly HousingService _sut;

        public HousingServiceTests()
        {
            _repository = new InMemoryHousingRepository();
            _sut = new HousingService(_repository);
        }

        private static HousingListing CreateValidListing(
            string title = "Test Apartment",
            string city = "Kyiv",
            decimal price = 5000,
            RoomType roomType = RoomType.OneBedroom,
            int area = 40)
        {
            return new HousingListing
            {
                Title = title,
                City = city,
                PricePerMonth = price,
                RoomType = roomType,
                AreaSqm = area
            };
        }

        // ── CreateListingAsync ──────────────────────────────────────

        [Fact]
        public async Task CreateListingAsync_AssignsNewId_WhenIdIsEmpty()
        {
            HousingListing listing = CreateValidListing();
            listing.Id = Guid.Empty;

            Guid resultId = await _sut.CreateListingAsync(listing);

            Assert.NotEqual(Guid.Empty, resultId);
        }

        [Fact]
        public async Task CreateListingAsync_PreservesExistingId()
        {
            Guid existingId = Guid.NewGuid();
            HousingListing listing = CreateValidListing();
            listing.Id = existingId;

            Guid resultId = await _sut.CreateListingAsync(listing);

            Assert.Equal(existingId, resultId);
        }

        [Fact]
        public async Task CreateListingAsync_SetsIsActiveTrue()
        {
            HousingListing listing = CreateValidListing();
            listing.IsActive = false;

            Guid id = await _sut.CreateListingAsync(listing);

            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.True(stored.IsActive);
        }

        [Fact]
        public async Task CreateListingAsync_SetsCreatedAtUtc_WhenDefault()
        {
            HousingListing listing = CreateValidListing();
            listing.CreatedAtUtc = default;

            DateTime before = DateTime.UtcNow;
            await _sut.CreateListingAsync(listing);
            DateTime after = DateTime.UtcNow;

            Assert.InRange(listing.CreatedAtUtc, before, after);
        }

        [Fact]
        public async Task CreateListingAsync_PreservesCreatedAtUtc_WhenAlreadySet()
        {
            DateTime customDate = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
            HousingListing listing = CreateValidListing();
            listing.CreatedAtUtc = customDate;

            await _sut.CreateListingAsync(listing);

            Assert.Equal(customDate, listing.CreatedAtUtc);
        }

        [Fact]
        public async Task CreateListingAsync_PersistsToRepository()
        {
            HousingListing listing = CreateValidListing();

            Guid id = await _sut.CreateListingAsync(listing);

            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.Equal("Test Apartment", stored.Title);
        }

        // ── CreateListingDraftAsync ─────────────────────────────────

        [Fact]
        public async Task CreateListingDraftAsync_SetsIsActiveFalse()
        {
            HousingListing draft = CreateValidListing();
            draft.IsActive = true;

            Guid id = await _sut.CreateListingDraftAsync(draft);

            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.False(stored.IsActive);
        }

        [Fact]
        public async Task CreateListingDraftAsync_AssignsNewId_WhenIdIsEmpty()
        {
            HousingListing draft = CreateValidListing();
            draft.Id = Guid.Empty;

            Guid resultId = await _sut.CreateListingDraftAsync(draft);

            Assert.NotEqual(Guid.Empty, resultId);
        }

        // ── UpdateListingDraftAsync ─────────────────────────────────

        [Fact]
        public async Task UpdateListingDraftAsync_UpdatesFields_WhenDraftExists()
        {
            HousingListing draft = CreateValidListing(title: "Old Title");
            Guid id = await _sut.CreateListingDraftAsync(draft);

            HousingListing updated = CreateValidListing(title: "New Title", city: "Lviv", price: 7000);
            bool result = await _sut.UpdateListingDraftAsync(id, updated);

            Assert.True(result);
            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.Equal("New Title", stored.Title);
            Assert.Equal("Lviv", stored.City);
            Assert.Equal(7000, stored.PricePerMonth);
        }

        [Fact]
        public async Task UpdateListingDraftAsync_ReturnsFalse_WhenNotFound()
        {
            bool result = await _sut.UpdateListingDraftAsync(Guid.NewGuid(), CreateValidListing());

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateListingDraftAsync_ReturnsFalse_WhenListingIsActive()
        {
            HousingListing listing = CreateValidListing();
            Guid id = await _sut.CreateListingAsync(listing);

            bool result = await _sut.UpdateListingDraftAsync(id, CreateValidListing(title: "Updated"));

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateListingDraftAsync_KeepsIsActiveFalse()
        {
            HousingListing draft = CreateValidListing();
            Guid id = await _sut.CreateListingDraftAsync(draft);

            HousingListing updated = CreateValidListing();
            await _sut.UpdateListingDraftAsync(id, updated);

            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.False(stored.IsActive);
        }

        // ── PublishListingAsync ──────────────────────────────────────

        [Fact]
        public async Task PublishListingAsync_SetsIsActiveTrue()
        {
            HousingListing draft = CreateValidListing();
            Guid id = await _sut.CreateListingDraftAsync(draft);

            bool result = await _sut.PublishListingAsync(id);

            Assert.True(result);
            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.True(stored.IsActive);
        }

        [Fact]
        public async Task PublishListingAsync_UpdatesCreatedAtUtc()
        {
            HousingListing draft = CreateValidListing();
            Guid id = await _sut.CreateListingDraftAsync(draft);

            DateTime before = DateTime.UtcNow;
            await _sut.PublishListingAsync(id);
            DateTime after = DateTime.UtcNow;

            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.InRange(stored.CreatedAtUtc, before, after);
        }

        [Fact]
        public async Task PublishListingAsync_ReturnsFalse_WhenNotFound()
        {
            bool result = await _sut.PublishListingAsync(Guid.NewGuid());

            Assert.False(result);
        }

        // ── UpdateListingAsync ──────────────────────────────────────

        [Fact]
        public async Task UpdateListingAsync_UpdatesFields_WhenExists()
        {
            HousingListing listing = CreateValidListing(title: "Original");
            Guid id = await _sut.CreateListingAsync(listing);

            HousingListing updated = CreateValidListing(title: "Modified", city: "Odesa");
            bool result = await _sut.UpdateListingAsync(id, updated);

            Assert.True(result);
            HousingListing? stored = await _repository.GetByIdAsync(id);
            Assert.NotNull(stored);
            Assert.Equal("Modified", stored.Title);
            Assert.Equal("Odesa", stored.City);
        }

        [Fact]
        public async Task UpdateListingAsync_ReturnsFalse_WhenNotFound()
        {
            bool result = await _sut.UpdateListingAsync(Guid.NewGuid(), CreateValidListing());

            Assert.False(result);
        }

        // ── SearchListingsAsync ─────────────────────────────────────

        [Fact]
        public async Task SearchListingsAsync_NormalizesPage_WhenZeroOrNegative()
        {
            ListingSearchCriteria criteria = new ListingSearchCriteria { Page = 0, OnlyActive = false };

            await _sut.SearchListingsAsync(criteria);

            Assert.Equal(1, criteria.Page);
        }

        [Fact]
        public async Task SearchListingsAsync_NormalizesPageSize_WhenOutOfRange()
        {
            ListingSearchCriteria tooSmall = new ListingSearchCriteria { PageSize = 0, OnlyActive = false };
            ListingSearchCriteria tooLarge = new ListingSearchCriteria { PageSize = 500, OnlyActive = false };

            await _sut.SearchListingsAsync(tooSmall);
            await _sut.SearchListingsAsync(tooLarge);

            Assert.Equal(20, tooSmall.PageSize);
            Assert.Equal(20, tooLarge.PageSize);
        }

        [Fact]
        public async Task SearchListingsAsync_ReturnsMatchingListings()
        {
            await _sut.CreateListingAsync(CreateValidListing(city: "Kyiv"));
            await _sut.CreateListingAsync(CreateValidListing(city: "Lviv"));

            ListingSearchCriteria criteria = new ListingSearchCriteria { City = "Kyiv" };
            IReadOnlyList<HousingListing> results = await _sut.SearchListingsAsync(criteria);

            Assert.Single(results);
            Assert.Equal("Kyiv", results[0].City);
        }

        // ── GetListingDetailsAsync ──────────────────────────────────

        [Fact]
        public async Task GetListingDetailsAsync_ReturnsListing_WhenExists()
        {
            HousingListing listing = CreateValidListing();
            Guid id = await _sut.CreateListingAsync(listing);

            HousingListing? result = await _sut.GetListingDetailsAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetListingDetailsAsync_ReturnsNull_WhenNotFound()
        {
            HousingListing? result = await _sut.GetListingDetailsAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}
