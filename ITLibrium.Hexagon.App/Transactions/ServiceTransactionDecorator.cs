using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Decorators;
using ITLibrium.Hexagon.App.Services;

namespace ITLibrium.Hexagon.App.Transactions
{
    public class ServiceTransactionDecorator<TService> : ActionExecutorDecorator<TService> 
        where TService : IAppService
    {
        protected override ExecutionOrder ExecutionOrder => ExecutionOrder.AfterDecorated;
        
        public ServiceTransactionDecorator(IActionExecutor<TService> decorated) : base(decorated) { }

        protected override Task DecorateAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}