using System.Threading.Tasks;

namespace ITLibrium.Hexagon.App.Commands
{
    public interface ICommandHandler { }

    public interface ICommandHandler<in TCommand> : ICommandHandler
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler
        where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}