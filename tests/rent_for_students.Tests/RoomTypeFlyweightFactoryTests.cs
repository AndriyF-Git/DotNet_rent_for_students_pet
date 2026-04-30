using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Flyweight;

namespace rent_for_students.Tests
{
    public class RoomTypeFlyweightFactoryTests
    {
        private readonly RoomTypeFlyweightFactory _sut = new();

        // ── GetFlyweight ────────────────────────────────────────────

        [Theory]
        [InlineData(RoomType.Studio, "Studio")]
        [InlineData(RoomType.OneBedroom, "1-Bedroom")]
        [InlineData(RoomType.TwoBedroom, "2-Bedroom")]
        [InlineData(RoomType.ThreeBedroom, "3-Bedroom")]
        public void GetFlyweight_ReturnsCorrectDisplayName(RoomType type, string expectedName)
        {
            IRoomTypeFlyweight flyweight = _sut.GetFlyweight(type);

            Assert.Equal(expectedName, flyweight.DisplayName);
        }

        [Theory]
        [InlineData(RoomType.Studio, 1)]
        [InlineData(RoomType.OneBedroom, 2)]
        [InlineData(RoomType.TwoBedroom, 3)]
        [InlineData(RoomType.ThreeBedroom, 4)]
        public void GetFlyweight_ReturnsCorrectCapacity(RoomType type, int expectedCapacity)
        {
            IRoomTypeFlyweight flyweight = _sut.GetFlyweight(type);

            Assert.Equal(expectedCapacity, flyweight.TypicalCapacity);
        }

        [Theory]
        [InlineData(RoomType.Studio, "room-studio")]
        [InlineData(RoomType.OneBedroom, "room-1bd")]
        [InlineData(RoomType.TwoBedroom, "room-2bd")]
        [InlineData(RoomType.ThreeBedroom, "room-3bd")]
        public void GetFlyweight_ReturnsCorrectCssClass(RoomType type, string expectedCss)
        {
            IRoomTypeFlyweight flyweight = _sut.GetFlyweight(type);

            Assert.Equal(expectedCss, flyweight.CssClass);
        }

        [Fact]
        public void GetFlyweight_ReturnsSameInstance_ForSameType()
        {
            IRoomTypeFlyweight first = _sut.GetFlyweight(RoomType.Studio);
            IRoomTypeFlyweight second = _sut.GetFlyweight(RoomType.Studio);

            Assert.Same(first, second);
        }

        [Fact]
        public void GetFlyweight_ReturnsDifferentInstances_ForDifferentTypes()
        {
            IRoomTypeFlyweight studio = _sut.GetFlyweight(RoomType.Studio);
            IRoomTypeFlyweight oneBed = _sut.GetFlyweight(RoomType.OneBedroom);

            Assert.NotSame(studio, oneBed);
        }

        [Fact]
        public void GetFlyweight_ThrowsArgumentException_ForUnregisteredType()
        {
            RoomType unknownType = (RoomType)999;

            Assert.Throws<ArgumentException>(() => _sut.GetFlyweight(unknownType));
        }

        [Fact]
        public void GetFlyweight_TypePropertyMatchesRequestedType()
        {
            foreach (RoomType type in Enum.GetValues<RoomType>())
            {
                IRoomTypeFlyweight flyweight = _sut.GetFlyweight(type);
                Assert.Equal(type, flyweight.Type);
            }
        }

        // ── GetAll ──────────────────────────────────────────────────

        [Fact]
        public void GetAll_ReturnsAllRegisteredRoomTypes()
        {
            IReadOnlyDictionary<RoomType, IRoomTypeFlyweight> all = _sut.GetAll();

            Assert.Equal(4, all.Count);
            Assert.Contains(RoomType.Studio, all.Keys);
            Assert.Contains(RoomType.OneBedroom, all.Keys);
            Assert.Contains(RoomType.TwoBedroom, all.Keys);
            Assert.Contains(RoomType.ThreeBedroom, all.Keys);
        }

        [Fact]
        public void GetAll_ReturnsSameInstances_AsGetFlyweight()
        {
            IReadOnlyDictionary<RoomType, IRoomTypeFlyweight> all = _sut.GetAll();

            foreach (RoomType type in Enum.GetValues<RoomType>())
            {
                Assert.Same(all[type], _sut.GetFlyweight(type));
            }
        }

        [Fact]
        public void GetFlyweight_DescriptionIsNotEmpty()
        {
            foreach (RoomType type in Enum.GetValues<RoomType>())
            {
                IRoomTypeFlyweight flyweight = _sut.GetFlyweight(type);
                Assert.False(string.IsNullOrWhiteSpace(flyweight.Description));
            }
        }
    }
}
