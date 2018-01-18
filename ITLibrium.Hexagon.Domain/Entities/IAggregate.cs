using ITLibrium.Hexagon.Domain.Meta;

namespace ITLibrium.Hexagon.Domain.Entities
{
    [Aggregate]
    public interface IAggregate<TAggregate> where TAggregate : IAggregate<TAggregate> { }
}