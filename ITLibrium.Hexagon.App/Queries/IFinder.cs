using System.Threading.Tasks;

namespace ITLibrium.Hexagon.App.Queries
{
    public interface IFinder { }

    public interface IFinder<TResult> : IFinder
    {
        Task<TResult> FindAsync();
    }

    public interface IFinder<TResult, in TCriteria> : IFinder
    {
        Task<TResult> FindAsync(TCriteria criteria);
    }
}