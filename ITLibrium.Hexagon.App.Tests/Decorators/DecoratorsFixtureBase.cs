using System;
using System.Collections.Generic;
using ITLibrium.Hexagon.App.Decorators;
using Shouldly;

namespace ITLibrium.Hexagon.App.Tests.Decorators
{
    internal class DecoratorsFixtureBase
    {
        private readonly List<MethodType> _methods = new List<MethodType>();

        private readonly ExecutionOrder _executionOrder;

        protected DecoratorsFixtureBase(ExecutionOrder executionOrder)
        {
            _executionOrder = executionOrder;
        }

        public void RegisterMethodExecution(MethodType method)
        {
            _methods.Add(method);
        }

        public void AssertCorrectExecutionOrder()
        {
            _methods.Count.ShouldBe(2);
            switch (_executionOrder)
            {
                case ExecutionOrder.AfterDecorated:
                    _methods[0].ShouldBe(MethodType.Decoraded);
                    _methods[1].ShouldBe(MethodType.Decorator);
                    break;
                case ExecutionOrder.BeforeDecorated:
                    _methods[0].ShouldBe(MethodType.Decorator);
                    _methods[1].ShouldBe(MethodType.Decoraded);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_executionOrder));
            }
        }
    }
}