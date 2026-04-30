using rent_for_students.Domain.Requests;
using rent_for_students.Domain.Entities;
using rent_for_students.ViewModels;

namespace rent_for_students.Mapping
{
    public static class ListingMapper
    {
        public static ListingSearchCriteria ToCriteria(ListingFilterViewModel vm)
        {
            return new ListingSearchCriteria
            {
                City = string.IsNullOrWhiteSpace(vm.City) ? null : vm.City.Trim(),
                MinPricePerMonth = vm.MinPricePerMonth,
                MaxPricePerMonth = vm.MaxPricePerMonth,
                RoomType = vm.RoomType,
                OnlyActive = vm.OnlyActive,
                Page = vm.Page,
                PageSize = vm.PageSize
            };
        }

        public static HousingListing ToEntity(ListingCreateViewModel vm)
        {
            return new HousingListing
            {
                Title = vm.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim(),
                City = vm.City.Trim(),
                District = string.IsNullOrWhiteSpace(vm.District) ? null : vm.District.Trim(),
                PricePerMonth = vm.PricePerMonth,
                RoomType = vm.RoomType,
                AreaSqm = vm.AreaSqm
            };
        }

        public static ListingCreateViewModel ToCreateViewModel(HousingListing entity)
        {
            return new ListingCreateViewModel
            {
                Title = entity.Title,
                Description = entity.Description,
                City = entity.City,
                District = entity.District,
                PricePerMonth = entity.PricePerMonth,
                RoomType = entity.RoomType,
                AreaSqm = entity.AreaSqm
            };
        }
    }
}
