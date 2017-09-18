using System;
using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Services;

namespace ITLibrium.Hexagon.App.Decorators
{
    public abstract class ActionExecutorDecorator<TService> : IActionExecutor<TService>, IDecorator
        where TService : IAppService
    {
        protected abstract ExecutionOrder ExecutionOrder { get; }

        private readonly IActionExecutor<TService> _decorated;
        object IDecorator.Decorated => _decorated;

        protected ActionExecutorDecorator(IActionExecutor<TService> decorated)
        {
            _decorated = decorated;
        }

        public Task ExecuteAsync(Func<TService, Task> action)
        {
            return DecoratorExctensions.Execute(ExecutionOrder, () => _decorated.ExecuteAsync(action), DecorateAsync);
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<TService, Task<TResult>> action)
        {
            return DecoratorExctensions.Execute<TResult>(ExecutionOrder, () => _decorated.ExecuteAsync(action), DecorateAsync);
        }

        protected abstract Task DecorateAsync();
    }
}