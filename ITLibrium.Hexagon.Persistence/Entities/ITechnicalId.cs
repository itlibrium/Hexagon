namespace ITLibrium.Hexagon.Persistence.Entities
{
    public interface ITechnicalId<out TValue>
    {
        TValue TechnicalValue { get; }
        bool IsNew { get; }
    }
}