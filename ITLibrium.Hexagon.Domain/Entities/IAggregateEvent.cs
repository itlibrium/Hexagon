namespace ITLibrium.Hexagon.Domain.Entities
{
    public interface IAggregateEvent<TAggregate> where TAggregate : IAggregate<TAggregate> { }
}