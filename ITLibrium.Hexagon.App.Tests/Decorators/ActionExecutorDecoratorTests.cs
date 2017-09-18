using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Decorators;
using ITLibrium.Hexagon.App.Services;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.App.Tests.Decorators
{
    public class ActionExecutorDecoratorTests
    {
        [Theory]
        [InlineData(ExecutionOrder.AfterDecorated)]
        [InlineData(ExecutionOrder.BeforeDecorated)]
        public void VoidActionDecoratedCorrectly(ExecutionOrder executionOrder)
        {
            var fixture = new Fixture(executionOrder);
            fixture.ActionExecutor.ExecuteAsync(s => s.Do()).Wait();
            fixture.AssertCorrectExecutionOrder();
        }

        [Theory]
        [InlineData(ExecutionOrder.AfterDecorated)]
        [InlineData(ExecutionOrder.BeforeDecorated)]
        public void ReturnActionDecoratedCorrectly(ExecutionOrder executionOrder)
        {
            var fixture = new Fixture(executionOrder);
            int result = fixture.ActionExecutor.ExecuteAsync(s => s.Get()).Result;
            result.ShouldBe(fixture.TestResult);
            fixture.AssertCorrectExecutionOrder();
        }

        private class Fixture : DecoratorsFixtureBase
        {
            public int TestResult { get; } = 5;

            public IActionExecutor<TestService> ActionExecutor { get; }

            public Fixture(ExecutionOrder executionOrder)
                : base(executionOrder)
            {
                ActionExecutor = new Decorator(
                    new ActionExecutor<TestService>(new TestService(this)),
                    this,
                    executionOrder);
            }
        }

        private class TestService : IAppService
        {
            private readonly Fixture _fixture;

            public TestService(Fixture fixture)
            {
                _fixture = fixture;
            }

            public Task Do()
            {
                _fixture.RegisterMethodExecution(MethodType.Decoraded);
                return Task.CompletedTask;
            }

            public Task<int> Get()
            {
                _fixture.RegisterMethodExecution(MethodType.Decoraded);
                return Task.FromResult(_fixture.TestResult);
            }
        }

        private class Decorator : ActionExecutorDecorator<TestService>
        {
            private readonly Fixture _fixture;

            protected override ExecutionOrder ExecutionOrder { get; }

            public Decorator(IActionExecutor<TestService> decorated, Fixture fixture, ExecutionOrder executionOrder)
                : base(decorated)
            {
                ExecutionOrder = executionOrder;
                _fixture = fixture;
            }

            protected override Task DecorateAsync()
            {
                _fixture.RegisterMethodExecution(MethodType.Decorator);
                return Task.CompletedTask;
            }
        }
    }
}