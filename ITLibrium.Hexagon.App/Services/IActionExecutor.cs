using System;
using System.Threading.Tasks;

namespace ITLibrium.Hexagon.App.Services
{
    public interface IActionExecutor { }

    public interface IActionExecutor<out TService> : IActionExecutor
        where TService : IAppService
    {
        Task ExecuteAsync(Func<TService, Task> action);
        Task<TResult> ExecuteAsync<TResult>(Func<TService, Task<TResult>> action);
    }
}