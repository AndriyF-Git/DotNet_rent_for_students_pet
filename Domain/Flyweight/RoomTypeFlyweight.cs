using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Flyweight
{
    // PROMPT v1.5: ConcreteFlyweight - immutable RoomType metadata object
    public class RoomTypeFlyweight : IRoomTypeFlyweight
    {
        public RoomType Type { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public int TypicalCapacity { get; }
        public string CssClass { get; }

        public RoomTypeFlyweight(
            RoomType type,
            string displayName,
            string description,
            int typicalCapacity,
            string cssClass)
        {
            Type = type;
            DisplayName = displayName;
            Description = description;
            TypicalCapacity = typicalCapacity;
            CssClass = cssClass;
        }
    }
}
