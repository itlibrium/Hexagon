using System.Collections.Generic;
using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Decorators;
using ITLibrium.Hexagon.App.Gates;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;
using ITLibrium.Hexagon.App.SimpleInjector.Gates;
using ITLibrium.Hexagon.App.SimpleInjector.Registration;
using Shouldly;
using SimpleInjector;
using Xunit;

namespace ITLibrium.Hexagon.App.SimpleInjector.Tests.Registration
{
    public class AppRegistrationPolicyTests
    {
        [Fact]
        public void GateResolvedCorrectly()
        {
            IGate gate = CreateGate();
            gate.ShouldBeOfType<SimpleInjectorGate>();
        }
        
        [Fact]
        public void ServiceResolvedCorrectly()
        {
            IGate gate = CreateGate();
            gate.GetService<IServiceA>().ShouldBeOfType<ActionExecutor<IServiceA>>();
        }
        
        [Fact]
        public void VoidHandlerResolvedCorrectly()
        {
            IGate gate = CreateGate();
            gate.GetHandlerFor<VoidCommand>().ShouldBeOfType<VoidHandler>();
        }
        
        [Fact]
        public void ResultHandlerResolvedCorrectly()
        {
            IGate gate = CreateGate();
            gate.GetHandlerFor<ResultCommand, int>().ShouldBeOfType<ResultHandler>();
        }
        
        [Fact]
        public void FinderWithoutCriteriaResolvedCorrectly()
        {
            IGate gate = CreateGate();
            gate.GetFinderFor<Result>().ShouldBeOfType<FinderWithoutCriteria>();
        }
        
        [Fact]
        public void FinderWithCriteriaResolvedCorrectly()
        {
            IGate gate = CreateGate();
            gate.GetFinderFor<Result, Criteria>().ShouldBeOfType<FinderWithCriteria>();
        }
        
        [Fact]
        public void DecoratedServiceResolvedCorrectly()
        {
            IGate gate = CreateGate(new DecoratorInfo(typeof(ServiceDecorator<>), Lifestyle.Singleton, null));
            gate.GetService<IServiceA>().ShouldBeOfType<ServiceDecorator<IServiceA>>();
        }
        
        [Fact]
        public void DecoratedVoidHandlerResolvedCorrectly()
        {
            IGate gate = CreateGate(new DecoratorInfo(typeof(VoidHandlerDecorator<>), Lifestyle.Singleton, null));
            gate.GetHandlerFor<VoidCommand>().ShouldBeOfType<VoidHandlerDecorator<VoidCommand>>();
        }
        
        [Fact]
        public void DecoratedResultHandlerResolvedCorrectly()
        {
            IGate gate = CreateGate(new DecoratorInfo(typeof(ResultHandlerDecorator<,>), Lifestyle.Singleton, null));
            gate.GetHandlerFor<ResultCommand, int>().ShouldBeOfType<ResultHandlerDecorator<ResultCommand, int>>();
        }
        
        [Fact]
        public void DecoratedFinderWithoutCriteriaResolvedCorrectly()
        {
            IGate gate = CreateGate(new DecoratorInfo(typeof(FinderWithoutCriteriaDecorator<>), Lifestyle.Singleton, null));
            gate.GetFinderFor<Result>().ShouldBeOfType<FinderWithoutCriteriaDecorator<Result>>();
        }
        
        [Fact]
        public void DecoratedFinderWithCriteriaResolvedCorrectly()
        {
            IGate gate = CreateGate(new DecoratorInfo(typeof(FinderWithCriteriaDecorator<,>), Lifestyle.Singleton, null));
            gate.GetFinderFor<Result, Criteria>().ShouldBeOfType<FinderWithCriteriaDecorator<Result, Criteria>>();
        }
        
        [Fact]
        public void NestedDecorationsResolvedCorrectly()
        {
            IGate gate = CreateGate(
                new DecoratorInfo(typeof(ServiceDecorator<>), Lifestyle.Singleton, null),
                new DecoratorInfo(typeof(OuterServiceDecorator<>), Lifestyle.Singleton, null));
            
            IDecorator outerDecorator = (IDecorator)gate.GetService<IServiceA>();
            outerDecorator.ShouldBeOfType<OuterServiceDecorator<IServiceA>>();
            IDecorator decorator = (IDecorator) outerDecorator.Decorated;
            decorator.ShouldBeOfType<ServiceDecorator<IServiceA>>();
            decorator.Decorated.ShouldBeOfType<ActionExecutor<IServiceA>>();
        }
        
        private static IGate CreateGate(params IDecoratorInfo[] decoratorsInfo)
        {
            var policy = new AppRegistrationPolicy();
            var container = new Container();
            policy.Register(container, Lifestyle.Singleton,
                new[]{typeof(AppRegistrationPolicyTests).Assembly}, 
                decoratorsInfo);
            container.Verify();
            
            return container.GetInstance<IGate>();
        }
        
        private interface IServiceA : IAppService { }
        private class ServiceA : IServiceA { }
        
        private interface IGenericService<T> : IAppService { }
        private class GenericService<T> : IGenericService<T> { }
        
        private class EntityA { }
        
        private class VoidCommand : ICommand { }
        private interface IVoidHandler : ICommandHandler<VoidCommand> { }
        private class VoidHandler : IVoidHandler
        {
            public Task HandleAsync(VoidCommand command) => Task.CompletedTask;
        }
        
        private class ResultCommand : ICommand<int> { }
        private interface IResultHandler : ICommandHandler<ResultCommand, int> { }
        private class ResultHandler : IResultHandler
        {
            public Task<int> HandleAsync(ResultCommand command) => Task.FromResult(5);
        }
        
        private interface IFinderWithoutCriteria : IFinder<Result> { }
        private class FinderWithoutCriteria : IFinderWithoutCriteria
        {
            public Task<Result> FindAsync() => Task.FromResult(new Result());
        }
        private interface IFinderWithCriteria : IFinder<Result, Criteria> { }
        private class FinderWithCriteria : IFinderWithCriteria
        {
            public Task<Result> FindAsync(Criteria criteria) => Task.FromResult(new Result());
        }
        private class Result { }
        private class Criteria { }
        
        private class ServiceDecorator<TService> : ActionExecutorDecorator<TService>
            where TService : IAppService
        {
            public ServiceDecorator(IActionExecutor<TService> decorated) : base(decorated) { }

            protected override ExecutionOrder ExecutionOrder => ExecutionOrder.BeforeDecorated;
            
            protected override Task DecorateAsync() => Task.CompletedTask;
        }
        
        private class OuterServiceDecorator<TService> : ActionExecutorDecorator<TService>
            where TService : IAppService
        {
            public OuterServiceDecorator(IActionExecutor<TService> decorated) : base(decorated) { }

            protected override ExecutionOrder ExecutionOrder => ExecutionOrder.BeforeDecorated;
            
            protected override Task DecorateAsync() => Task.CompletedTask;
        }
        
        private class VoidHandlerDecorator<TCommand> : CommandHandlerDecorator<TCommand> 
            where TCommand : ICommand
        {
            public VoidHandlerDecorator(ICommandHandler<TCommand> decorated) : base(decorated) { }

            protected override ExecutionOrder ExecutionOrder => ExecutionOrder.BeforeDecorated;
            
            protected override Task DecorateAsync(TCommand command) => Task.CompletedTask;
        }
        
        private class ResultHandlerDecorator<TCommand, TResult> : CommandHandlerDecorator<TCommand, TResult> 
            where TCommand : ICommand<TResult>
        {
            public ResultHandlerDecorator(ICommandHandler<TCommand, TResult> decorated) : base(decorated) { }

            protected override ExecutionOrder ExecutionOrder => ExecutionOrder.BeforeDecorated;
            
            protected override Task DecorateAsync(TCommand command) => Task.CompletedTask;
        }
        
        private class FinderWithoutCriteriaDecorator<TResult> : FinderDecorator<TResult>
        {
            public FinderWithoutCriteriaDecorator(IFinder<TResult> decorated) : base(decorated) { }

            protected override ExecutionOrder ExecutionOrder => ExecutionOrder.BeforeDecorated;
            
            protected override Task DecorateAsync()=> Task.CompletedTask;
        }
        
        private class FinderWithCriteriaDecorator<TResult, TCriteria> : FinderDecorator<TResult, TCriteria>
        {
            public FinderWithCriteriaDecorator(IFinder<TResult, TCriteria> decorated) : base(decorated) { }

            protected override ExecutionOrder ExecutionOrder => ExecutionOrder.BeforeDecorated;

            protected override Task DecorateAsync(TCriteria criteria) => Task.CompletedTask;
        }
    }
}