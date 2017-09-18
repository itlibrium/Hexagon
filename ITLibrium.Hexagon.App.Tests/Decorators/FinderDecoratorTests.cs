using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Decorators;
using ITLibrium.Hexagon.App.Queries;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.App.Tests.Decorators
{
    public class FinderDecoratorTests
    {
        [Theory]
        [InlineData(ExecutionOrder.AfterDecorated)]
        [InlineData(ExecutionOrder.BeforeDecorated)]
        public void FinderWithoutCriteriaDecoratedCorrectly(ExecutionOrder executionOrder)
        {
            var fixture = new Fixture(executionOrder);
            fixture.FinderWithoutCriteria.FindAsync().Wait();
            fixture.AssertCorrectExecutionOrder();
        }

        [Theory]
        [InlineData(ExecutionOrder.AfterDecorated)]
        [InlineData(ExecutionOrder.BeforeDecorated)]
        public void FinderWithCriteriaDecoratedCorrectly(ExecutionOrder executionOrder)
        {
            var fixture = new Fixture(executionOrder);
            Result result = fixture.FinderWithCriteria.FindAsync(new Criteria()).Result;
            result.ShouldBe(fixture.TestResult);
            fixture.AssertCorrectExecutionOrder();
        }

        private class Fixture : DecoratorsFixtureBase
        {
            public Result TestResult { get; } = new Result();

            public IFinder<Result> FinderWithoutCriteria { get; }

            public IFinder<Result, Criteria> FinderWithCriteria { get; }

            public Fixture(ExecutionOrder executionOrder)
                : base(executionOrder)
            {
                FinderWithoutCriteria = new DecoratorWithoutCriteria(
                    new FinderWithoutCriteria(this), 
                    this,
                    executionOrder);

                FinderWithCriteria = new DecoratorWithCriteria(
                    new FinderWithCriteria(this),
                    this,
                    executionOrder);
            }
        }

        private class Result { }

        private class Criteria { }

        private class FinderWithoutCriteria : IFinder<Result>
        {
            private readonly Fixture _fixture;

            public FinderWithoutCriteria(Fixture fixture)
            {
                _fixture = fixture;
            }

            public Task<Result> FindAsync()
            {
                _fixture.RegisterMethodExecution(MethodType.Decoraded);
                return Task.FromResult(_fixture.TestResult);
            }
        }

        private class FinderWithCriteria : IFinder<Result, Criteria>
        {
            private readonly Fixture _fixture;

            public FinderWithCriteria(Fixture fixture)
            {
                _fixture = fixture;
            }

            public Task<Result> FindAsync(Criteria criteria)
            {
                _fixture.RegisterMethodExecution(MethodType.Decoraded);
                return Task.FromResult(_fixture.TestResult);
            }
        }

        private class DecoratorWithoutCriteria : FinderDecorator<Result>
        {
            private readonly Fixture _fixture;

            protected override ExecutionOrder ExecutionOrder { get; }

            public DecoratorWithoutCriteria(IFinder<Result> decorated, Fixture fixture, ExecutionOrder executionOrder)
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

        private class DecoratorWithCriteria : FinderDecorator<Result, Criteria>
        {
            private readonly Fixture _fixture;

            protected override ExecutionOrder ExecutionOrder { get; }

            public DecoratorWithCriteria(IFinder<Result, Criteria> decorated, Fixture fixture, ExecutionOrder executionOrder)
                : base(decorated)
            {
                ExecutionOrder = executionOrder;
                _fixture = fixture;
            }

            protected override Task DecorateAsync(Criteria criteria)
            {
                _fixture.RegisterMethodExecution(MethodType.Decorator);
                return Task.CompletedTask;
            }
        }
    }
}