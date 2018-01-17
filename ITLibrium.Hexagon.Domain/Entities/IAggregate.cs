using ITLibrium.Hexagon.Domain.Meta;

namespace ITLibrium.Hexagon.Domain.Entities
{
    [Aggregate]
    internal interface IAggregate<TAggregate> where TAggregate : IAggregate<TAggregate> { }
}