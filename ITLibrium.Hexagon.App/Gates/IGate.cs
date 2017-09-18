using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;

namespace ITLibrium.Hexagon.App.Gates
{
    public interface IGate
    {
        IActionExecutor<TService> GetService<TService>() where TService : IAppService;
        ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : ICommand;
        ICommandHandler<TCommand, TResult> GetHandlerFor<TCommand, TResult>() where TCommand : ICommand<TResult>;
        IFinder<TResult> GetFinderFor<TResult>();
        IFinder<TResult, TCriteria> GetFinderFor<TResult, TCriteria>();
    }
}