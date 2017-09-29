using System;
using System.Collections.Generic;
using System.Linq;
using ITLibrium.Hexagon.SimpleInjector.Registration;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using Shouldly;
using SimpleInjector;
using Xunit;

namespace ITLibrium.Hexagon.SimpleInjector.Tests.Registration
{
    public class RegistrationPolicyTests
    {
        private readonly RegistrationPolicy _registrationPolicy = new RegistrationPolicy();
        
        [Fact]
        public void ImplementationOfSingleServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(IServiceA), typeof(ImplementationA)};
            Container container = Register(types);
            
            container.GetInstance<IServiceA>().ShouldBeOfType<ImplementationA>();
        }
        
        [Fact]
        public void ImplementationOfSingleGenericServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericService<>), typeof(GenericImplementationA)};
            Container container = Register(types);
            
            container.GetInstance<IGenericService<EntityA>>().ShouldBeOfType<GenericImplementationA>();
        }
        
        [Fact]
        public void ImplementationOfMultipleServicesRegisteredCorrectly()
        {
            var types = new[] {typeof(IServiceB), typeof(IServiceC), typeof(ImplementationBandC)};
            Container container = Register(types);
            
            container.GetInstance<IServiceB>().ShouldBeOfType<ImplementationBandC>();
            container.GetInstance<IServiceC>().ShouldBeOfType<ImplementationBandC>();
        }
        
        [Fact]
        public void ImplementationOfMultipleGenericServicesRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericService<>), typeof(GenericImplementationAandB)};
            Container container = Register(types);
            
            container.GetInstance<IGenericService<EntityA>>().ShouldBeOfType<GenericImplementationAandB>();
            container.GetInstance<IGenericService<EntityB>>().ShouldBeOfType<GenericImplementationAandB>();
        }
        
        [Fact]
        public void MultipleImplementationsOfServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(IServiceA), typeof(ImplementationA), typeof(ImplementationA2), typeof(ImplementationA3)};
            Container container = Register(types);
            
            List<IServiceA> instances = container.GetAllInstances<IServiceA>().ToList();
            instances.Count.ShouldBe(3);
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA));
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA2));
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA3));
        }
        
        [Fact]
        public void CompositeImplementationRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IServiceA), typeof(ImplementationA), typeof(ImplementationA2), typeof(ImplementationA3), typeof(CompositeImplementation)
            };
            Container container = Register(types);
            
            List<IServiceA> instances = container.GetAllInstances<IServiceA>().ToList();
            instances.Count.ShouldBe(3);
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA));
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA2));
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA3));
            
            container.GetInstance<IServiceA>().ShouldBeOfType<CompositeImplementation>();
        }
        
        [Fact]
        public void MultipleImplementationsOfGenericServiceRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IGenericService<>), typeof(GenericImplementationA), typeof(GenericImplementationA2)
            };
            Container container = Register(types);
            
            List<IGenericService<EntityA>> instances = container.GetAllInstances<IGenericService<EntityA>>().ToList();
            instances.Count.ShouldBe(2);
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationA));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationA2));
        }

        [Fact]
        public void OpenGenericImplementationRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericService<>), typeof(GenericImplementation<>)};
            Container container = Register(types);
            
            container.GetInstance<IGenericService<EntityA>>().ShouldBeOfType<GenericImplementation<EntityA>>();
            container.GetInstance<IGenericService<EntityB>>().ShouldBeOfType<GenericImplementation<EntityB>>();
        }
        
        [Fact]
        public void ImplementationDerivedFromOpenGenericRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericService<>), typeof(DerivedGenericImplementation)};
            Container container = Register(types);
            
            container.GetInstance<IGenericService<EntityA>>().ShouldBeOfType<DerivedGenericImplementation>();
        }
        
        [Fact]
        public void FallbackGenericImplementationRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericService<>), typeof(GenericImplementationA), typeof(GenericImplementation<>)};
            Container container = Register(types);
            
            container.GetInstance<IGenericService<EntityA>>().ShouldBeOfType<GenericImplementationA>();
            container.GetInstance<IGenericService<EntityB>>().ShouldBeOfType<GenericImplementation<EntityB>>();
        }
        
        [Fact]
        public void ImplementationWithoutServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(BaseImplementation), typeof(Implementation)};
            Container container = Register(types);
            
            container.GetInstance<Implementation>().ShouldBeOfType<Implementation>();
        }
        
        [Fact]
        public void OpenImplementationWithoutServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(GenericImplementation<>)};
            Container container = Register(types);
            
            container.GetInstance<GenericImplementation<EntityA>>().ShouldBeOfType<GenericImplementation<EntityA>>();
            container.GetInstance<GenericImplementation<EntityB>>().ShouldBeOfType<GenericImplementation<EntityB>>();
        }
        
        [Fact]
        public void AbstractClassesAreOmitted()
        {
            var types = new[] {typeof(BaseImplementation), typeof(Implementation)};
            Container container = Register(types);
            
            Should.Throw<ActivationException>(() => container.GetInstance<BaseImplementation>());
        }
        
        [Fact]
        public void ComplexScenarioRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IServiceA), typeof(ImplementationA), typeof(ImplementationA2), typeof(ImplementationA3), typeof(CompositeImplementation),
                typeof(IServiceB), typeof(IServiceC), typeof(ImplementationBandC),
                typeof(IGenericService<>), typeof(GenericImplementation<>),
                typeof(GenericImplementationA), typeof(GenericImplementationA2), typeof(GenericImplementationAandB), typeof(DerivedGenericImplementation),
                typeof(BaseImplementation), typeof(Implementation)
            };
            Container container = Register(types);
            
            IReadOnlyList<object> instances = container.GetAllInstances<IServiceA>().ToList();
            instances.Count.ShouldBe(3);
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA));
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA2));
            instances.ShouldContain(p => p.GetType() == typeof(ImplementationA3));
            container.GetInstance<IServiceA>().ShouldBeOfType<CompositeImplementation>();
            
            container.GetInstance<IServiceB>().ShouldBeOfType<ImplementationBandC>();
            container.GetInstance<IServiceC>().ShouldBeOfType<ImplementationBandC>();
            
            instances = container.GetAllInstances<IGenericService<EntityA>>().ToList();
            instances.Count.ShouldBe(4);
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationA));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationA2));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAandB));
            instances.ShouldContain(p => p.GetType() == typeof(DerivedGenericImplementation));
            
            container.GetInstance<IGenericService<EntityB>>().ShouldBeOfType<GenericImplementationAandB>();
            container.GetInstance<IGenericService<EntityC>>().ShouldBeOfType<GenericImplementation<EntityC>>();
            
            container.GetInstance<Implementation>().ShouldBeOfType<Implementation>();
            
            container.GetInstance<GenericImplementation<EntityA>>().ShouldBeOfType<GenericImplementation<EntityA>>();
            container.GetInstance<GenericImplementation<EntityB>>().ShouldBeOfType<GenericImplementation<EntityB>>();
            
            Should.Throw<ActivationException>(() => container.GetInstance<BaseImplementation>());
        }

        private Container Register(IEnumerable<Type> types)
        {
            var container = new Container();
            _registrationPolicy.Register(container, Lifestyle.Singleton, new HashSet<Type>(types));
            container.Verify();
            return container;
        }
        
        private interface IServiceA { }
        private interface IServiceB { }
        private interface IServiceC { }
        private class ImplementationA : IServiceA { }
        private class ImplementationBandC : IServiceB, IServiceC { }
        
        private class ImplementationA2 : IServiceA { }
        private class ImplementationA3 : IServiceA { }
        private class CompositeImplementation : IServiceA
        {
            public CompositeImplementation(IEnumerable<IServiceA> services) { }
        }

        private interface IGenericService<T> { }
        private class GenericImplementationA : IGenericService<EntityA> { }
        private class GenericImplementationA2 : IGenericService<EntityA> { }
        private class GenericImplementationAandB : IGenericService<EntityA>, IGenericService<EntityB> { }
        private class GenericImplementation<T> :IGenericService<T> { }
        private class DerivedGenericImplementation : GenericImplementation<EntityA> { }
        private class EntityA { }
        private class EntityB { }
        private class EntityC { }
        
        private abstract class BaseImplementation { }
        private class Implementation : BaseImplementation { }
    }
}