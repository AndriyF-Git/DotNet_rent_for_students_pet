using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Services;
using rent_for_students.Tests.TestDoubles;

namespace rent_for_students.Tests
{
    public class ListingUseCaseMediatorTests
    {
        [Fact]
        public async Task CreateDraftAndPublish_SetsFlagsAndTimestamp()
        {
            var housingRepo = new InMemoryHousingRepository();
            var notifications = new TestNotificationService();
            var housingService = new HousingService(housingRepo);
            var sut = new ListingUseCaseMediator(housingService, notifications, new StubListingReportRepository());

            var draft = new HousingListing
            {
                Title = "Draft",
                City = "Odesa",
                PricePerMonth = 600,
                RoomType = RoomType.Studio,
                AreaSqm = 22
            };

            var created = await sut.CreateDraftAsync(draft);
            Assert.True(created.IsSuccess);
            var listingId = created.Value;
            Assert.NotEqual(Guid.Empty, listingId);

            var detailsBefore = await sut.GetDetailsAsync(listingId);
            Assert.True(detailsBefore.IsSuccess);
            Assert.False(detailsBefore.Value!.IsActive);
            var beforePublishTimestamp = detailsBefore.Value.CreatedAtUtc;

            await Task.Delay(5);

            var published = await sut.PublishAsync(listingId);
            Assert.True(published.IsSuccess);

            var detailsAfter = await sut.GetDetailsAsync(listingId);
            Assert.True(detailsAfter.IsSuccess);
            Assert.True(detailsAfter.Value!.IsActive);
            Assert.True(detailsAfter.Value.CreatedAtUtc >= beforePublishTimestamp);
        }

        [Fact]
        public async Task CreateAsync_InvalidListing_ReturnsValidationError()
        {
            var housingRepo = new InMemoryHousingRepository();
            var notifications = new TestNotificationService();
            var housingService = new HousingService(housingRepo);
            var sut = new ListingUseCaseMediator(housingService, notifications, new StubListingReportRepository());

            var invalid = new HousingListing
            {
                Title = "",
                City = "",
                PricePerMonth = 0,
                RoomType = RoomType.Studio,
                AreaSqm = 1
            };

            var result = await sut.CreateAsync(invalid);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCodes.ValidationError, result.ErrorCode);
        }
    }
}
