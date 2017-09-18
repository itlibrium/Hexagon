using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Gates;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;
using Moq;
using Shouldly;
using Xunit;

namespace ITLibrium.Hexagon.App.Tests.Gates
{
    public class GateTests
    {
        [Fact]
        public void VoidActionDispatchedCorrectly()
        {
            var fixture = new Fixture();
            fixture.CreateGate().GetService<ITestServiceA>().ExecuteAsync(s => s.DoAsync(5)).Wait();

            fixture.AppServiceMockA.Verify(s => s.DoAsync(5), Times.Once);
        }

        [Fact]
        public void ResultActionDispatchedCorrectly()
        {
            var fixture = new Fixture();
            string result = fixture.CreateGate().GetService<ITestServiceA>().ExecuteAsync(s => s.GetAsync(fixture.TestInput)).Result;

            result.ShouldBe(fixture.TestResult);
            fixture.AppServiceMockA.Verify(s => s.GetAsync(5), Times.Once);
        }

        [Fact]
        public void VoidActionFromDecoratedServiceDispatchedCorrectly()
        {
            var fixture = new Fixture();
            fixture.CreateGate().GetService<ITestServiceC>().ExecuteAsync(s => s.DoAsync(5)).Wait();

            fixture.AppServiceMockC.Verify(s => s.DoAsync(5), Times.Once);
            fixture.AppServiceDecorator.Verify(d => d.Action(), Times.Once);
        }

        [Fact]
        public void ResultActionFromDecoratedServiceDispatchedCorrectly()
        {
            var fixture = new Fixture();
            string result = fixture.CreateGate().GetService<ITestServiceC>().ExecuteAsync(s => s.GetAsync(fixture.TestInput)).Result;

            result.ShouldBe(fixture.TestResult);
            fixture.AppServiceMockC.Verify(s => s.GetAsync(5), Times.Once);
            fixture.AppServiceDecorator.Verify(d => d.Action(), Times.Once);
        }

        [Fact]
        public void VoidCommandDispatchedCorrectly()
        {
            var fixture = new Fixture();
            var command = new TestCommandA();
            fixture.CreateGate().GetHandlerFor<TestCommandA>().HandleAsync(command).Wait();

            fixture.HandlerAandBMock.Verify(s => s.HandleAsync(command), Times.Once);
        }

        [Fact]
        public void ResultCommandDispatchedCorrectly()
        {
            var fixture = new Fixture();
            var command = new TestCommandB();
            string result = fixture.CreateGate().GetHandlerFor<TestCommandB, string>().HandleAsync(command).Result;

            result.ShouldBe(fixture.TestResult);
            fixture.HandlerAandBMock.Verify(s => s.HandleAsync(command), Times.Once);
        }

        [Fact]
        public void CommandForDecoratedHandlerDispatchedCorrectly()
        {
            var fixture = new Fixture();
            var command = new TestCommandC();
            fixture.CreateGate().GetHandlerFor<TestCommandC>().HandleAsync(command).Wait();

            fixture.HandlerDecoratorMock.Verify(d => d.Action(), Times.Once);
            fixture.HandlerCMock.Verify(s => s.HandleAsync(command), Times.Once);
        }

        [Fact]
        public void FinderWithoutCriteriaDispatchedCorrectly()
        {
            var fixture = new Fixture();
            TestResultA result = fixture.CreateGate().GetFinderFor<TestResultA>().FindAsync().Result;

            result.ShouldBe(fixture.TestResultA);
            fixture.FinderAandBMock.Verify(s => s.FindAsync(), Times.Once);
        }

        [Fact]
        public void FinderWithCriteriaDispatchedCorrectly()
        {
            var fixture = new Fixture();
            TestResultB result = fixture.CreateGate().GetFinderFor<TestResultB, int>().FindAsync(5).Result;

            result.ShouldBe(fixture.TestResultB);
            fixture.FinderAandBMock.Verify(s => s.FindAsync(5), Times.Once);
        }

        [Fact]
        public void DecoratedFinderDispatchedCorrectly()
        {
            var fixture = new Fixture();
            TestResultC result = fixture.CreateGate().GetFinderFor<TestResultC>().FindAsync().Result;

            result.ShouldBe(fixture.TestResultC);
            fixture.FinderDecoratorMock.Verify(d => d.Action(), Times.Once);
            fixture.FinderCMock.Verify(s => s.FindAsync(), Times.Once);
        }

        [Fact]
        public void AllGatePoliciesChecked()
        {
            var fixture = new Fixture();
            var policyMocks = new[] { new Mock<IGatePolicy>(), new Mock<IGatePolicy>() };
            fixture.CreateGate(policyMocks.Select(m => m.Object).ToList());

            foreach (IActionExecutor executor in fixture.Executors)
                foreach (Mock<IGatePolicy> policyMock in policyMocks)
                    policyMock.Verify(p => p.Check(executor), Times.Once);

            foreach (ICommandHandler handler in fixture.Handlers)
                foreach (Mock<IGatePolicy> policyMock in policyMocks)
                    policyMock.Verify(p => p.Check(handler), Times.Once);

            foreach (IFinder finder in fixture.Finders)
                foreach (Mock<IGatePolicy> policyMock in policyMocks)
                    policyMock.Verify(p => p.Check(finder), Times.Once);
        }

        private class Fixture
        {
            public Mock<ITestServiceA> AppServiceMockA { get; }
            public Mock<ITestServiceB> AppServiceMockB { get; }
            public Mock<ITestServiceC> AppServiceMockC { get; }
            public Mock<ServiceDecorator> AppServiceDecorator { get; }
            public IEnumerable<IActionExecutor> Executors { get; }

            public Mock<TestHandlerAandB> HandlerAandBMock { get; }
            public Mock<TestHandlerC> HandlerCMock { get; }
            public Mock<HandlerDecorator> HandlerDecoratorMock { get; }
            public IEnumerable<ICommandHandler> Handlers { get; }

            public Mock<TestFinderAandB> FinderAandBMock { get; }
            public Mock<TestFinderC> FinderCMock { get; }
            public Mock<FinderDecorator> FinderDecoratorMock { get; }
            public IEnumerable<IFinder> Finders { get; }

            public Gate CreateGate() => CreateGate(new IGatePolicy[0]);

            public Gate CreateGate(IReadOnlyCollection<IGatePolicy> policies)
            {
                return new Gate(Executors, Handlers, Finders, policies);
            }

            public int TestInput { get; } = 5;
            public string TestResult { get; } = "test";

            public TestResultA TestResultA { get; } = new TestResultA();
            public TestResultB TestResultB { get; } = new TestResultB();
            public TestResultC TestResultC { get; } = new TestResultC();

            public Fixture()
            {
                AppServiceMockA = new Mock<ITestServiceA>();
                AppServiceMockA.Setup(s => s.DoAsync(TestInput)).Returns(Task.CompletedTask);
                AppServiceMockA.Setup(s => s.GetAsync(TestInput)).Returns(Task.FromResult(TestResult));
                AppServiceMockB = new Mock<ITestServiceB>();
                AppServiceMockB.Setup(s => s.GetAsync(TestInput)).Returns(Task.FromResult(TestResult));
                AppServiceMockC = new Mock<ITestServiceC>();
                AppServiceMockC.Setup(s => s.DoAsync(TestInput)).Returns(Task.CompletedTask);
                AppServiceMockC.Setup(s => s.GetAsync(TestInput)).Returns(Task.FromResult(TestResult));
                AppServiceDecorator = new Mock<ServiceDecorator>(new ActionExecutor<ITestServiceC>(AppServiceMockC.Object)) { CallBase = true };
                Executors = new IActionExecutor[]
                {
                    new ActionExecutor<ITestServiceA>(AppServiceMockA.Object),
                    new ActionExecutor<ITestServiceB>(AppServiceMockB.Object),
                    AppServiceDecorator.Object
                };

                HandlerAandBMock = new Mock<TestHandlerAandB>();
                HandlerAandBMock.Setup(h => h.HandleAsync(It.IsAny<TestCommandB>())).Returns(Task.FromResult(TestResult));
                HandlerCMock = new Mock<TestHandlerC>();
                HandlerDecoratorMock = new Mock<HandlerDecorator>(HandlerCMock.Object) { CallBase = true };
                Handlers = new ICommandHandler[] { HandlerAandBMock.Object, HandlerDecoratorMock.Object };

                FinderAandBMock = new Mock<TestFinderAandB>();
                FinderAandBMock.Setup(f => f.FindAsync()).Returns(Task.FromResult(TestResultA));
                FinderAandBMock.Setup(f => f.FindAsync(TestInput)).Returns(Task.FromResult(TestResultB));
                FinderCMock = new Mock<TestFinderC>();
                FinderCMock.Setup(f => f.FindAsync()).Returns(Task.FromResult(TestResultC));
                FinderDecoratorMock = new Mock<FinderDecorator>(FinderCMock.Object) { CallBase = true };
                Finders = new IFinder[] { FinderAandBMock.Object, FinderDecoratorMock.Object };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface ITestServiceA : IAppService
        {
            Task DoAsync(int i);
            Task<string> GetAsync(int i);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public interface ITestServiceB : IAppService
        {
            Task<string> GetAsync(int i);
        }

        public interface ITestServiceC : IAppService
        {
            Task DoAsync(int i);
            Task<string> GetAsync(int i);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class ServiceDecorator : IActionExecutor<ITestServiceC>
        {
            private readonly IActionExecutor<ITestServiceC> _decoratee;

            protected ServiceDecorator(IActionExecutor<ITestServiceC> decoratee)
            {
                _decoratee = decoratee;
            }

            public Task ExecuteAsync(Func<ITestServiceC, Task> action)
            {
                Action();
                return _decoratee.ExecuteAsync(action);
            }

            public Task<TResult> ExecuteAsync<TResult>(Func<ITestServiceC, Task<TResult>> action)
            {
                Action();
                return _decoratee.ExecuteAsync(action);
            }

            public abstract void Action();
        }

        public class TestCommandA : ICommand { }

        public class TestCommandB : ICommand<string> { }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class TestCommandC : ICommand { }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class TestHandlerAandB : ICommandHandler<TestCommandA>, ICommandHandler<TestCommandB, string>
        {
            public abstract Task HandleAsync(TestCommandA command);

            public abstract Task<string> HandleAsync(TestCommandB command);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class TestHandlerC : ICommandHandler<TestCommandC>
        {
            public abstract Task HandleAsync(TestCommandC command);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class HandlerDecorator : ICommandHandler<TestCommandC>
        {
            private readonly ICommandHandler<TestCommandC> _decoratee;

            protected HandlerDecorator(ICommandHandler<TestCommandC> decoratee)
            {
                _decoratee = decoratee;
            }

            public Task HandleAsync(TestCommandC command)
            {
                Action();
                return _decoratee.HandleAsync(command);
            }

            public abstract void Action();
        }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class TestResultA { }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class TestResultB { }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class TestResultC { }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class TestFinderAandB : IFinder<TestResultA>, IFinder<TestResultB, int>
        {
            public abstract Task<TestResultA> FindAsync();
            public abstract Task<TestResultB> FindAsync(int criteria);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class TestFinderC : IFinder<TestResultC>
        {
            public abstract Task<TestResultC> FindAsync();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public abstract class FinderDecorator : IFinder<TestResultC>
        {
            private readonly IFinder<TestResultC> _decoratee;

            protected FinderDecorator(IFinder<TestResultC> decoratee)
            {
                _decoratee = decoratee;
            }

            public Task<TestResultC> FindAsync()
            {
                Action();
                return _decoratee.FindAsync();
            }

            public abstract void Action();
        }
    }
}