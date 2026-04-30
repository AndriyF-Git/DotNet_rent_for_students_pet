using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Flyweight
{
    // PROMPT v1.5: Flyweight Factory - caches and returns shared RoomType flyweights
    public class RoomTypeFlyweightFactory
    {
        private readonly Dictionary<RoomType, IRoomTypeFlyweight> _cache = new Dictionary<RoomType, IRoomTypeFlyweight>();

        public RoomTypeFlyweightFactory()
        {
            _cache[RoomType.Studio] = new RoomTypeFlyweight(
                RoomType.Studio,
                "Studio",
                "Compact housing for one person, combining living and sleeping areas",
                1,
                "room-studio");

            _cache[RoomType.OneBedroom] = new RoomTypeFlyweight(
                RoomType.OneBedroom,
                "1-Bedroom",
                "Separate bedroom apartment, suitable for one or two residents",
                2,
                "room-1bd");

            _cache[RoomType.TwoBedroom] = new RoomTypeFlyweight(
                RoomType.TwoBedroom,
                "2-Bedroom",
                "Two-bedroom apartment, ideal for sharing between students",
                3,
                "room-2bd");

            _cache[RoomType.ThreeBedroom] = new RoomTypeFlyweight(
                RoomType.ThreeBedroom,
                "3-Bedroom",
                "Spacious three-bedroom apartment for a group of students",
                4,
                "room-3bd");
        }

        public IRoomTypeFlyweight GetFlyweight(RoomType type)
        {
            if (_cache.TryGetValue(type, out IRoomTypeFlyweight? flyweight))
            {
                return flyweight;
            }

            throw new ArgumentException($"No flyweight registered for RoomType '{type}'.", nameof(type));
        }

        public IReadOnlyDictionary<RoomType, IRoomTypeFlyweight> GetAll()
        {
            return _cache;
        }
    }
}
