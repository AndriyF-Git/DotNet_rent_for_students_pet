using rent_for_students.Domain.Entities;
using rent_for_students.Mapping;
using rent_for_students.ViewModels;

namespace rent_for_students.Tests
{
    public class ListingMapperTests
    {
        // ── ToCriteria ──────────────────────────────────────────────

        [Fact]
        public void ToCriteria_MapsAllFields()
        {
            ListingFilterViewModel vm = new()
            {
                City = "Kyiv",
                MinPricePerMonth = 1000,
                MaxPricePerMonth = 5000,
                RoomType = RoomType.TwoBedroom,
                OnlyActive = false,
                Page = 2,
                PageSize = 50
            };

            var criteria = ListingMapper.ToCriteria(vm);

            Assert.Equal("Kyiv", criteria.City);
            Assert.Equal(1000, criteria.MinPricePerMonth);
            Assert.Equal(5000, criteria.MaxPricePerMonth);
            Assert.Equal(RoomType.TwoBedroom, criteria.RoomType);
            Assert.False(criteria.OnlyActive);
            Assert.Equal(2, criteria.Page);
            Assert.Equal(50, criteria.PageSize);
        }

        [Fact]
        public void ToCriteria_TrimsCity()
        {
            ListingFilterViewModel vm = new() { City = "  Lviv  " };

            var criteria = ListingMapper.ToCriteria(vm);

            Assert.Equal("Lviv", criteria.City);
        }

        [Fact]
        public void ToCriteria_SetsNullCity_WhenWhitespace()
        {
            ListingFilterViewModel vm = new() { City = "   " };

            var criteria = ListingMapper.ToCriteria(vm);

            Assert.Null(criteria.City);
        }

        [Fact]
        public void ToCriteria_SetsNullCity_WhenNull()
        {
            ListingFilterViewModel vm = new() { City = null };

            var criteria = ListingMapper.ToCriteria(vm);

            Assert.Null(criteria.City);
        }

        [Fact]
        public void ToCriteria_PreservesNullOptionalFields()
        {
            ListingFilterViewModel vm = new()
            {
                MinPricePerMonth = null,
                MaxPricePerMonth = null,
                RoomType = null
            };

            var criteria = ListingMapper.ToCriteria(vm);

            Assert.Null(criteria.MinPricePerMonth);
            Assert.Null(criteria.MaxPricePerMonth);
            Assert.Null(criteria.RoomType);
        }

        // ── ToEntity ────────────────────────────────────────────────

        [Fact]
        public void ToEntity_MapsAllFields()
        {
            ListingCreateViewModel vm = new()
            {
                Title = "Cozy Studio",
                Description = "Near university",
                City = "Odesa",
                District = "Arcadia",
                PricePerMonth = 4500,
                RoomType = RoomType.Studio,
                AreaSqm = 25
            };

            HousingListing entity = ListingMapper.ToEntity(vm);

            Assert.Equal("Cozy Studio", entity.Title);
            Assert.Equal("Near university", entity.Description);
            Assert.Equal("Odesa", entity.City);
            Assert.Equal("Arcadia", entity.District);
            Assert.Equal(4500, entity.PricePerMonth);
            Assert.Equal(RoomType.Studio, entity.RoomType);
            Assert.Equal(25, entity.AreaSqm);
        }

        [Fact]
        public void ToEntity_TrimsStringFields()
        {
            ListingCreateViewModel vm = new()
            {
                Title = "  Apartment  ",
                City = " Kyiv ",
                Description = "  Desc  ",
                District = "  Center  ",
                PricePerMonth = 3000,
                RoomType = RoomType.OneBedroom,
                AreaSqm = 35
            };

            HousingListing entity = ListingMapper.ToEntity(vm);

            Assert.Equal("Apartment", entity.Title);
            Assert.Equal("Kyiv", entity.City);
            Assert.Equal("Desc", entity.Description);
            Assert.Equal("Center", entity.District);
        }

        [Fact]
        public void ToEntity_SetsNullDescription_WhenWhitespace()
        {
            ListingCreateViewModel vm = new()
            {
                Title = "Apt",
                City = "Kyiv",
                Description = "   ",
                PricePerMonth = 3000,
                RoomType = RoomType.OneBedroom,
                AreaSqm = 30
            };

            HousingListing entity = ListingMapper.ToEntity(vm);

            Assert.Null(entity.Description);
        }

        [Fact]
        public void ToEntity_SetsNullDistrict_WhenWhitespace()
        {
            ListingCreateViewModel vm = new()
            {
                Title = "Apt",
                City = "Kyiv",
                District = "  ",
                PricePerMonth = 3000,
                RoomType = RoomType.OneBedroom,
                AreaSqm = 30
            };

            HousingListing entity = ListingMapper.ToEntity(vm);

            Assert.Null(entity.District);
        }

        // ── ToCreateViewModel ───────────────────────────────────────

        [Fact]
        public void ToCreateViewModel_MapsAllFields()
        {
            HousingListing entity = new()
            {
                Title = "Big Apartment",
                Description = "Great view",
                City = "Kharkiv",
                District = "Center",
                PricePerMonth = 6000,
                RoomType = RoomType.ThreeBedroom,
                AreaSqm = 80
            };

            ListingCreateViewModel vm = ListingMapper.ToCreateViewModel(entity);

            Assert.Equal("Big Apartment", vm.Title);
            Assert.Equal("Great view", vm.Description);
            Assert.Equal("Kharkiv", vm.City);
            Assert.Equal("Center", vm.District);
            Assert.Equal(6000, vm.PricePerMonth);
            Assert.Equal(RoomType.ThreeBedroom, vm.RoomType);
            Assert.Equal(80, vm.AreaSqm);
        }

        [Fact]
        public void ToCreateViewModel_PreservesNullOptionalFields()
        {
            HousingListing entity = new()
            {
                Title = "Apt",
                City = "Kyiv",
                Description = null,
                District = null,
                PricePerMonth = 3000,
                RoomType = RoomType.Studio,
                AreaSqm = 20
            };

            ListingCreateViewModel vm = ListingMapper.ToCreateViewModel(entity);

            Assert.Null(vm.Description);
            Assert.Null(vm.District);
        }

        // ── Roundtrip ───────────────────────────────────────────────

        [Fact]
        public void Roundtrip_EntityToViewModelAndBack_PreservesData()
        {
            HousingListing original = new()
            {
                Title = "Roundtrip Test",
                Description = "Should survive",
                City = "Dnipro",
                District = "South",
                PricePerMonth = 3500,
                RoomType = RoomType.TwoBedroom,
                AreaSqm = 55
            };

            ListingCreateViewModel vm = ListingMapper.ToCreateViewModel(original);
            HousingListing reconstructed = ListingMapper.ToEntity(vm);

            Assert.Equal(original.Title, reconstructed.Title);
            Assert.Equal(original.Description, reconstructed.Description);
            Assert.Equal(original.City, reconstructed.City);
            Assert.Equal(original.District, reconstructed.District);
            Assert.Equal(original.PricePerMonth, reconstructed.PricePerMonth);
            Assert.Equal(original.RoomType, reconstructed.RoomType);
            Assert.Equal(original.AreaSqm, reconstructed.AreaSqm);
        }
    }
}
