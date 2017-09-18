using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Decorators;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.App.Tests.Decorators
{
    public class CommandHandlerDecoratorTests
    {
        [Theory]
        [InlineData(ExecutionOrder.AfterDecorated)]
        [InlineData(ExecutionOrder.BeforeDecorated)]
        public void VoidHandlerDecoratedCorrectly(ExecutionOrder executionOrder)
        {
            var fixture = new Fixture(executionOrder);
            fixture.VoidHandler.HandleAsync(new VoidCommand()).Wait();
            fixture.AssertCorrectExecutionOrder();
        }

        [Theory]
        [InlineData(ExecutionOrder.AfterDecorated)]
        [InlineData(ExecutionOrder.BeforeDecorated)]
        public void ResultHandlerDecoratedCorrectly(ExecutionOrder executionOrder)
        {
            var fixture = new Fixture(executionOrder);
            int result = fixture.ResultHandler.HandleAsync(new ResultCommand()).Result;
            result.ShouldBe(fixture.TestResult);
            fixture.AssertCorrectExecutionOrder();
        }

        private class Fixture : DecoratorsFixtureBase
        {
            public ICommandHandler<VoidCommand> VoidHandler { get; }

            public ICommandHandler<ResultCommand, int> ResultHandler { get; }

            public int TestResult { get; } = 5;

            public Fixture(ExecutionOrder executionOrder)
                : base(executionOrder)
            {
                VoidHandler = new VoidDecorator(
                    new VoidCommandHandler(this),
                    this,
                    executionOrder);

                ResultHandler = new ResultDecorator(
                    new ResultCommandHandler(this),
                    this,
                    executionOrder);
            }
        }

        private class VoidCommand : ICommand { }

        private class ResultCommand : ICommand<int> { }

        private class VoidCommandHandler : ICommandHandler<VoidCommand>
        {
            private readonly Fixture _fixture;

            public VoidCommandHandler(Fixture fixture)
            {
                _fixture = fixture;
            }

            public Task HandleAsync(VoidCommand command)
            {
                _fixture.RegisterMethodExecution(MethodType.Decoraded);
                return Task.CompletedTask;
            }
        }

        private class ResultCommandHandler : ICommandHandler<ResultCommand, int>
        {
            private readonly Fixture _fixture;

            public ResultCommandHandler(Fixture fixture)
            {
                _fixture = fixture;
            }

            public Task<int> HandleAsync(ResultCommand command)
            {
                _fixture.RegisterMethodExecution(MethodType.Decoraded);
                return Task.FromResult<int>(_fixture.TestResult);
            }
        }

        private class VoidDecorator : CommandHandlerDecorator<VoidCommand>
        {
            private readonly Fixture _fixture;

            protected override ExecutionOrder ExecutionOrder { get; }

            public VoidDecorator(ICommandHandler<VoidCommand> decorated, Fixture fixture, ExecutionOrder executionOrder)
                : base(decorated)
            {
                ExecutionOrder = executionOrder;
                _fixture = fixture;
            }

            protected override Task DecorateAsync(VoidCommand command)
            {
                _fixture.RegisterMethodExecution(MethodType.Decorator);
                return Task.CompletedTask;
            }
        }

        private class ResultDecorator : CommandHandlerDecorator<ResultCommand, int>
        {
            private readonly Fixture _fixture;

            protected override ExecutionOrder ExecutionOrder { get; }

            public ResultDecorator(ICommandHandler<ResultCommand, int> decorated, Fixture fixture, ExecutionOrder executionOrder)
                : base(decorated)
            {
                ExecutionOrder = executionOrder;
                _fixture = fixture;
            }

            protected override Task DecorateAsync(ResultCommand command)
            {
                _fixture.RegisterMethodExecution(MethodType.Decorator);
                return Task.CompletedTask;
            }
        }
    }
}