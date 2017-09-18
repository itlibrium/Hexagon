using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Queries;

namespace ITLibrium.Hexagon.App.Decorators
{
    public abstract class FinderDecorator<TResult> : IFinder<TResult>, IDecorator
    {
        protected abstract ExecutionOrder ExecutionOrder { get; }

        private readonly IFinder<TResult> _decorated;
        object IDecorator.Decorated => _decorated;

        protected FinderDecorator(IFinder<TResult> decorated)
        {
            _decorated = decorated;
        }

        public Task<TResult> FindAsync()
        {
            return DecoratorExctensions.Execute(ExecutionOrder, _decorated.FindAsync, DecorateAsync);
        }

        protected abstract Task DecorateAsync();
    }

    public abstract class FinderDecorator<TResult, TCriteria> : IFinder<TResult, TCriteria>, IDecorator
    {
        protected abstract ExecutionOrder ExecutionOrder { get; }

        private readonly IFinder<TResult, TCriteria> _decorated;
        object IDecorator.Decorated => _decorated;

        protected FinderDecorator(IFinder<TResult, TCriteria> decorated)
        {
            _decorated = decorated;
        }

        public Task<TResult> FindAsync(TCriteria criteria)
        {
            return DecoratorExctensions.Execute(criteria, ExecutionOrder, _decorated.FindAsync, DecorateAsync);
        }

        protected abstract Task DecorateAsync(TCriteria criteria);
    }
}