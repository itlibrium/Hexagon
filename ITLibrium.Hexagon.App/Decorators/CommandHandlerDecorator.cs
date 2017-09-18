using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Commands;

namespace ITLibrium.Hexagon.App.Decorators
{
    public abstract class CommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>, IDecorator
        where TCommand : ICommand
    {
        protected abstract ExecutionOrder ExecutionOrder { get; }

        private readonly ICommandHandler<TCommand> _decorated;
        object IDecorator.Decorated => _decorated;
        
        protected CommandHandlerDecorator(ICommandHandler<TCommand> decorated)
        {
            _decorated = decorated;
        }

        public Task HandleAsync(TCommand command)
        {
            return DecoratorExctensions.Execute(command, ExecutionOrder, _decorated.HandleAsync, DecorateAsync);
        }

        protected abstract Task DecorateAsync(TCommand command);
    }

    public abstract class CommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IDecorator
        where TCommand : ICommand<TResult>
    {
        protected abstract ExecutionOrder ExecutionOrder { get; }

        private readonly ICommandHandler<TCommand, TResult> _decorated;
        object IDecorator.Decorated => _decorated;

        protected CommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated)
        {
            _decorated = decorated;
        }

        public Task<TResult> HandleAsync(TCommand command)
        {
            return DecoratorExctensions.Execute(command, ExecutionOrder, _decorated.HandleAsync, DecorateAsync);
        }

        protected abstract Task DecorateAsync(TCommand command);
    }
}