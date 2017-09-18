using System;
using System.Threading.Tasks;

namespace ITLibrium.Hexagon.App.Services
{
    public class ActionExecutor<TService> : IActionExecutor<TService> 
        where TService : IAppService
    {
        private readonly TService _service;

        public ActionExecutor(TService service)
        {
            _service = service;
        }

        public Task ExecuteAsync(Func<TService, Task> action)
        {
            return action(_service);
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<TService, Task<TResult>> action)
        {
            return action(_service);
        }
    }
}