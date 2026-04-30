using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Services;
using rent_for_students.Tests.TestDoubles;

namespace rent_for_students.Tests
{
    public class ApplicationUseCaseMediatorTests
    {
        [Fact]
        public async Task ApplyAsync_ActiveListing_CreatesApprovedApplication()
        {
            var housingRepo = new InMemoryHousingRepository();
            var appRepo = new InMemoryRentalApplicationRepository();
            var profileRepo = new InMemoryRentalApplicationProfileRepository();
            var notifications = new TestNotificationService();
            var housingService = new HousingService(housingRepo);

            var listing = new HousingListing
            {
                Id = Guid.NewGuid(),
                Title = "Listing A",
                City = "Kyiv",
                PricePerMonth = 1000,
                RoomType = RoomType.Studio,
                AreaSqm = 30,
                IsActive = true
            };
            await housingRepo.AddAsync(listing);

            var sut = new ApplicationUseCaseMediator(housingService, appRepo, profileRepo, notifications);

            var applicant = new RentalApplication
            {
                ApplicantName = "John Doe",
                Phone = "+380501234567",
                Email = "john@example.com"
            };

            var result = await sut.ApplyAsync(listing.Id, applicant);

            Assert.True(result.IsSuccess);
            var applicationId = result.Value;
            Assert.NotEqual(Guid.Empty, applicationId);

            var saved = await appRepo.GetByIdAsync(applicationId);
            Assert.NotNull(saved);
            Assert.Equal(ApplicationStatus.Approved, saved!.Status);
            Assert.Contains(notifications.Messages, m => m.Contains("Rental application created"));
        }

        [Fact]
        public async Task ApplyAsync_InactiveListing_ReturnsListingNotAvailable()
        {
            var housingRepo = new InMemoryHousingRepository();
            var appRepo = new InMemoryRentalApplicationRepository();
            var profileRepo = new InMemoryRentalApplicationProfileRepository();
            var notifications = new TestNotificationService();
            var housingService = new HousingService(housingRepo);

            var listing = new HousingListing
            {
                Id = Guid.NewGuid(),
                Title = "Listing B",
                City = "Lviv",
                PricePerMonth = 850,
                RoomType = RoomType.OneBedroom,
                AreaSqm = 35,
                IsActive = false
            };
            await housingRepo.AddAsync(listing);

            var sut = new ApplicationUseCaseMediator(housingService, appRepo, profileRepo, notifications);

            var applicant = new RentalApplication
            {
                ApplicantName = "Jane Doe",
                Phone = "+380509999999",
                Email = "jane@example.com"
            };

            var result = await sut.ApplyAsync(listing.Id, applicant);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCodes.ListingNotAvailable, result.ErrorCode);
        }

        [Fact]
        public async Task ApplyFromProfileAsync_ExistingProfile_CreatesApprovedApplication()
        {
            var housingRepo = new InMemoryHousingRepository();
            var appRepo = new InMemoryRentalApplicationRepository();
            var profileRepo = new InMemoryRentalApplicationProfileRepository();
            var notifications = new TestNotificationService();
            var housingService = new HousingService(housingRepo);

            var listing = new HousingListing
            {
                Id = Guid.NewGuid(),
                Title = "Listing C",
                City = "Odesa",
                PricePerMonth = 950,
                RoomType = RoomType.Studio,
                AreaSqm = 24,
                IsActive = true
            };
            await housingRepo.AddAsync(listing);

            var profile = new RentalApplicationProfile
            {
                Id = Guid.NewGuid(),
                ProfileName = "Default Student",
                ApplicantName = "Profile User",
                Phone = "+380501110000",
                Email = "profile@example.com",
                Message = "I can move in next month."
            };
            await profileRepo.AddAsync(profile);

            var sut = new ApplicationUseCaseMediator(housingService, appRepo, profileRepo, notifications);
            var result = await sut.ApplyFromProfileAsync(listing.Id, profile.Id);

            Assert.True(result.IsSuccess);
            var created = await appRepo.GetByIdAsync(result.Value);
            Assert.NotNull(created);
            Assert.Equal(ApplicationStatus.Approved, created!.Status);
            Assert.Equal(profile.ApplicantName, created.ApplicantName);
            Assert.Equal(profile.Phone, created.Phone);
            Assert.Equal(profile.Email, created.Email);
        }

        [Fact]
        public async Task CreateProfileAsync_ValidProfile_PersistsProfile()
        {
            var housingRepo = new InMemoryHousingRepository();
            var appRepo = new InMemoryRentalApplicationRepository();
            var profileRepo = new InMemoryRentalApplicationProfileRepository();
            var notifications = new TestNotificationService();
            var housingService = new HousingService(housingRepo);
            var sut = new ApplicationUseCaseMediator(housingService, appRepo, profileRepo, notifications);

            var profile = new RentalApplicationProfile
            {
                ProfileName = "Quick Apply",
                ApplicantName = "User Name",
                Phone = "+380501231231",
                Email = "user@example.com",
                Message = "Saved profile."
            };

            var createResult = await sut.CreateProfileAsync(profile);

            Assert.True(createResult.IsSuccess);
            Assert.NotEqual(Guid.Empty, createResult.Value);

            var listResult = await sut.ListProfilesAsync();
            Assert.True(listResult.IsSuccess);
            Assert.Contains(listResult.Value!, x => x.Id == createResult.Value && x.ProfileName == "Quick Apply");
        }

        [Fact]
        public void RentalApplicationProfile_Clone_CopiesPrototypeFields()
        {
            var prototype = new RentalApplicationProfile
            {
                Id = Guid.NewGuid(),
                ProfileName = "Baseline Profile",
                ApplicantName = "Student User",
                Phone = "+380501231231",
                Email = "student@example.com",
                Message = "Clone me.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                UpdatedAtUtc = DateTime.UtcNow
            };

            var clone = prototype.Clone();

            Assert.NotNull(clone);
            Assert.NotSame(prototype, clone);
            Assert.Equal(prototype.Id, clone.Id);
            Assert.Equal(prototype.ProfileName, clone.ProfileName);
            Assert.Equal(prototype.ApplicantName, clone.ApplicantName);
            Assert.Equal(prototype.Phone, clone.Phone);
            Assert.Equal(prototype.Email, clone.Email);
            Assert.Equal(prototype.Message, clone.Message);
        }
    }
}
