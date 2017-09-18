using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;

namespace ITLibrium.Hexagon.App.Gates
{
    public interface IGatePolicy
    {
        void Check(IActionExecutor executor);
        void Check(ICommandHandler handler);
        void Check(IFinder finder);
    }
}