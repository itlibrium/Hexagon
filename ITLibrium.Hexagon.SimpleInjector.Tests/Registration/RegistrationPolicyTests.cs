using System;
using System.Collections.Generic;
using System.Linq;
using ITLibrium.Hexagon.SimpleInjector.Registration;
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
            container.GetInstance<ImplementationA>().ShouldBeOfType<ImplementationA>();
            container.GetRegistration(typeof(IServiceA)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(ImplementationA)).Lifestyle);
        }
        
        [Fact]
        public void ImplementationOfSingleGenericServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericServiceA<>), typeof(GenericImplementationAx)};
            Container container = Register(types);
            
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<GenericImplementationAx>();
            container.GetInstance<GenericImplementationAx>().ShouldBeOfType<GenericImplementationAx>();
            container.GetRegistration(typeof(IGenericServiceA<EntityX>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(GenericImplementationAx)).Lifestyle);
        }
        
        [Fact]
        public void ImplementationOfMultipleServicesRegisteredCorrectly()
        {
            var types = new[] {typeof(IServiceB), typeof(IServiceC), typeof(ImplementationBandC)};
            Container container = Register(types);
            
            container.GetInstance<IServiceB>().ShouldBeOfType<ImplementationBandC>();
            container.GetInstance<IServiceC>().ShouldBeOfType<ImplementationBandC>();
            container.GetInstance<ImplementationBandC>().ShouldBeOfType<ImplementationBandC>();
            container.GetRegistration(typeof(IServiceB)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(ImplementationBandC)).Lifestyle);
        }
        
        [Fact]
        public void ImplementationOfMultipleGenericServicesRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericServiceA<>), typeof(GenericImplementationAxy)};
            Container container = Register(types);
            
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<GenericImplementationAxy>();
            container.GetInstance<IGenericServiceA<EntityY>>().ShouldBeOfType<GenericImplementationAxy>();
            container.GetInstance<GenericImplementationAxy>().ShouldBeOfType<GenericImplementationAxy>();
            container.GetRegistration(typeof(IGenericServiceA<EntityX>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(GenericImplementationAxy)).Lifestyle);
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
            container.GetInstance<CompositeImplementation>().ShouldBeOfType<CompositeImplementation>();
            container.GetRegistration(typeof(IServiceA)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(CompositeImplementation)).Lifestyle);
        }
        
        [Fact]
        public void MultipleImplementationsOfGenericServiceRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IGenericServiceA<>), typeof(GenericImplementationAx), typeof(GenericImplementationAx2)
            };
            Container container = Register(types);
            
            List<IGenericServiceA<EntityX>> instances = container.GetAllInstances<IGenericServiceA<EntityX>>().ToList();
            instances.Count.ShouldBe(2);
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAx));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAx2));
        }

        [Fact]
        public void OpenImplementationRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericServiceA<>), typeof(GenericImplementationA<>)};
            Container container = Register(types);
            
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<GenericImplementationA<EntityX>>();
            container.GetInstance<IGenericServiceA<EntityY>>().ShouldBeOfType<GenericImplementationA<EntityY>>();
            container.GetInstance<GenericImplementationA<EntityX>>().ShouldBeOfType<GenericImplementationA<EntityX>>();
            container.GetInstance<GenericImplementationA<EntityY>>().ShouldBeOfType<GenericImplementationA<EntityY>>();
            container.GetRegistration(typeof(IGenericServiceA<EntityX>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(GenericImplementationA<EntityX>)).Lifestyle);
        }
        
        [Fact]
        public void NonGenericServicesForOpenImplementationAreOmitted()
        {
            var types = new[] {typeof(IGenericServiceBase), typeof(IGenericServiceA<>), typeof(GenericImplementationA<>)};
            Container container = Register(types);
            
            Should.Throw<ActivationException>(() => container.GetInstance<IGenericServiceBase>());
            Should.Throw<ActivationException>(() => container.GetAllInstances<IGenericServiceBase>());
        }
        
        [Fact]
        public void ImplementationDerivedFromOpenRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericServiceA<>), typeof(DerivedGenericImplementationA)};
            Container container = Register(types);
            
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<DerivedGenericImplementationA>();
            container.GetInstance<DerivedGenericImplementationA>().ShouldBeOfType<DerivedGenericImplementationA>();
            container.GetRegistration(typeof(IGenericServiceA<EntityX>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(DerivedGenericImplementationA)).Lifestyle);
        }
        
        [Fact]
        public void FallbackGenericImplementationRegisteredCorrectly()
        {
            var types = new[] {typeof(IGenericServiceA<>), typeof(GenericImplementationAx), typeof(GenericImplementationA<>)};
            Container container = Register(types);
            
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<GenericImplementationAx>();
            container.GetInstance<IGenericServiceA<EntityY>>().ShouldBeOfType<GenericImplementationA<EntityY>>();
        }
        
        [Fact]
        public void CollectionsForOpenImplementationRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IGenericServiceA<>), typeof(GenericImplementationAx), typeof(GenericImplementationAxy), typeof(GenericImplementationA<>)
            };
            Container container = Register(types);
            
            IReadOnlyList<object> instances = container.GetAllInstances<IGenericServiceA<EntityX>>().ToList();
            instances.Count.ShouldBe(3);
            instances.ShouldContain(i => i.GetType() == typeof(GenericImplementationAx));
            instances.ShouldContain(i => i.GetType() == typeof(GenericImplementationAxy));
            instances.ShouldContain(i => i.GetType() == typeof(GenericImplementationA<EntityX>));

            instances = container.GetAllInstances<IGenericServiceA<EntityY>>().ToList();
            instances.Count.ShouldBe(2);
            instances.ShouldContain(i => i.GetType() == typeof(GenericImplementationAxy));
            instances.ShouldContain(i => i.GetType() == typeof(GenericImplementationA<EntityY>));
            
            instances = container.GetAllInstances<IGenericServiceA<EntityZ>>().ToList();
            instances.Count.ShouldBe(1);
            instances.ShouldContain(i => i.GetType() == typeof(GenericImplementationA<EntityZ>));
        }
        
        [Fact]
        public void CompositeImplementationForGenericServicesRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IGenericServiceA<>), typeof(GenericImplementationAx), typeof(GenericImplementationA<>), typeof(CompositeImplementationForGenericServicesAx)
            };
            Container container = Register(types);
            
            List<IGenericServiceA<EntityX>> instances = container.GetAllInstances<IGenericServiceA<EntityX>>().ToList();
            instances.Count.ShouldBe(2);
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAx));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationA<EntityX>));
            
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<CompositeImplementationForGenericServicesAx>();
            container.GetInstance<CompositeImplementationForGenericServicesAx>().ShouldBeOfType<CompositeImplementationForGenericServicesAx>();
            container.GetRegistration(typeof(IGenericServiceA<EntityX>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(CompositeImplementationForGenericServicesAx)).Lifestyle);
        }
        
        [Fact]
        public void OpenNestedImplementationRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(IServiceA), typeof(ImplementationA),
                typeof(IGenericServiceA<>), typeof(GenericImplementationA<>),
                typeof(IGenericServiceB<,>), typeof(GenericImplementationB<,>)
            };
            Container container = Register(types);
            
            var instance1 = container.GetInstance<IGenericServiceB<IGenericServiceA<EntityX>, IServiceA>>();
            instance1.ShouldBeOfType<GenericImplementationB<IGenericServiceA<EntityX>, IServiceA>>();
            instance1.PropertyT1.ShouldBeOfType<GenericImplementationA<EntityX>>();
            instance1.PropertyT2.ShouldBeOfType<ImplementationA>();
            
            var instance2 = container.GetInstance<IGenericServiceB<IGenericServiceA<EntityY>, IServiceA>>();
            instance2.ShouldBeOfType<GenericImplementationB<IGenericServiceA<EntityY>, IServiceA>>();
            instance2.PropertyT1.ShouldBeOfType<GenericImplementationA<EntityY>>();
            instance2.PropertyT2.ShouldBeOfType<ImplementationA>();
            
            var instance3 = container.GetInstance<GenericImplementationB<IGenericServiceA<EntityX>, IServiceA>>();
            instance3.ShouldBeOfType<GenericImplementationB<IGenericServiceA<EntityX>, IServiceA>>();
            instance3.PropertyT1.ShouldBeOfType<GenericImplementationA<EntityX>>();
            instance3.PropertyT2.ShouldBeOfType<ImplementationA>();
            container.GetRegistration(typeof(IGenericServiceB<IGenericServiceA<EntityX>, IServiceA>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(GenericImplementationB<IGenericServiceA<EntityX>, IServiceA>)).Lifestyle);
            
            var instance4 = container.GetInstance<GenericImplementationB<IGenericServiceA<EntityY>, IServiceA>>();
            instance4.ShouldBeOfType<GenericImplementationB<IGenericServiceA<EntityY>, IServiceA>>();
            instance4.PropertyT1.ShouldBeOfType<GenericImplementationA<EntityY>>();
            instance4.PropertyT2.ShouldBeOfType<ImplementationA>();
            container.GetRegistration(typeof(IGenericServiceB<IGenericServiceA<EntityY>, IServiceA>)).Lifestyle
                .ShouldBe(container.GetRegistration(typeof(GenericImplementationB<IGenericServiceA<EntityY>, IServiceA>)).Lifestyle);
        }
        
        [Fact]
        public void ServicesWithOpenAndPartiallyClosedGenericArgumentsAreOmmited()
        {
            var types = new[]
            {
                typeof(IGenericServiceA<>), typeof(IGenericServiceB<,>), 
                typeof(IGenericServiceB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(EntityX)),
                typeof(IGenericServiceB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(IGenericServiceA<>)),
                typeof(GenericImplementationB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(EntityX)),
                typeof(GenericImplementationB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(IGenericServiceA<>))
            };
            Container container = Register(types);
            
            container.GetRootRegistrations().Length.ShouldBe(0);
        }
        
        [Fact]
        public void ImplementationWithoutServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(AbstractImplementation), typeof(Implementation)};
            Container container = Register(types);
            
            container.GetInstance<Implementation>().ShouldBeOfType<Implementation>();
        }
        
        [Fact]
        public void OpenImplementationWithoutServiceRegisteredCorrectly()
        {
            var types = new[] {typeof(GenericImplementationA<>)};
            Container container = Register(types);
            
            container.GetInstance<GenericImplementationA<EntityX>>().ShouldBeOfType<GenericImplementationA<EntityX>>();
            container.GetInstance<GenericImplementationA<EntityY>>().ShouldBeOfType<GenericImplementationA<EntityY>>();
        }
        
        [Fact]
        public void AbstractClassesAreOmitted()
        {
            var types = new[] {typeof(AbstractImplementation), typeof(BaseImplementation), typeof(Implementation)};
            Container container = Register(types);
            
            Should.Throw<ActivationException>(() => container.GetInstance<AbstractImplementation>());
        }
        
        [Fact]
        public void ClassesWithoutPublicConstructorAreOmitted()
        {
            var types = new[] {typeof(AbstractImplementation), typeof(BaseImplementation), typeof(Implementation)};
            Container container = Register(types);
            
            Should.Throw<ActivationException>(() => container.GetInstance<BaseImplementation>());
        }
        
        [Fact]
        public void ComplexScenarioRegisteredCorrectly()
        {
            var types = new[]
            {
                typeof(GenericImplementationB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(IGenericServiceA<>)),
                typeof(ImplementationA), typeof(ImplementationA2), typeof(ImplementationA3), typeof(CompositeImplementation), typeof(IServiceA),
                typeof(IGenericServiceB<,>), typeof(GenericImplementationB<,>),
                typeof(IServiceB), typeof(ImplementationBandC), typeof(IServiceC),
                typeof(GenericImplementationAxy), typeof(GenericImplementationAx), typeof(GenericImplementationAx2), typeof(DerivedGenericImplementationA),
                typeof(IGenericServiceB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(EntityX)),
                typeof(CompositeImplementationForGenericServicesAx),
                typeof(GenericImplementationA<>), typeof(IGenericServiceA<>),
                typeof(BaseImplementation), typeof(Implementation), typeof(AbstractImplementation),
                typeof(GenericImplementationB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(EntityX)),
                typeof(IGenericServiceB<,>).MakeGenericType(typeof(IGenericServiceA<>), typeof(IGenericServiceA<>))
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
            
            instances = container.GetAllInstances<IGenericServiceA<EntityX>>().ToList();
            instances.Count.ShouldBe(5);
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAx));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAx2));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationAxy));
            instances.ShouldContain(p => p.GetType() == typeof(DerivedGenericImplementationA));
            instances.ShouldContain(p => p.GetType() == typeof(GenericImplementationA<EntityX>));
            container.GetInstance<IGenericServiceA<EntityX>>().ShouldBeOfType<CompositeImplementationForGenericServicesAx>();
            
            container.GetInstance<IGenericServiceA<EntityY>>().ShouldBeOfType<GenericImplementationAxy>();
            container.GetInstance<IGenericServiceA<EntityZ>>().ShouldBeOfType<GenericImplementationA<EntityZ>>();
            
            Should.Throw<ActivationException>(() => container.GetInstance<IGenericServiceBase>());
            Should.Throw<ActivationException>(() => container.GetAllInstances<IGenericServiceBase>());
            
            var instance = container.GetInstance<IGenericServiceB<IGenericServiceA<EntityZ>, IServiceA>>();
            instance.ShouldBeOfType<GenericImplementationB<IGenericServiceA<EntityZ>, IServiceA>>();
            instance.PropertyT1.ShouldBeOfType<GenericImplementationA<EntityZ>>();
            instance.PropertyT2.ShouldBeOfType<CompositeImplementation>();
            
            container.GetInstance<Implementation>().ShouldBeOfType<Implementation>();
            
            container.GetInstance<GenericImplementationA<EntityX>>().ShouldBeOfType<GenericImplementationA<EntityX>>();
            container.GetInstance<GenericImplementationA<EntityY>>().ShouldBeOfType<GenericImplementationA<EntityY>>();
            
            Should.Throw<ActivationException>(() => container.GetInstance<AbstractImplementation>());
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
        private interface IGenericServiceBase { }
        private interface IGenericServiceA<T> : IGenericServiceBase { }
        private interface IGenericServiceB<T1, T2> : IGenericServiceBase
        {
            T1 PropertyT1 { get; }
            T2 PropertyT2 { get; }
        }
        private class GenericImplementationAx : IGenericServiceA<EntityX> { }
        private class GenericImplementationAx2 : IGenericServiceA<EntityX> { }
        private class GenericImplementationAxy : IGenericServiceA<EntityX>, IGenericServiceA<EntityY> { }
        private class GenericImplementationA<T> :IGenericServiceA<T> { }
        private class DerivedGenericImplementationA : GenericImplementationA<EntityX> { }
        private class CompositeImplementationForGenericServicesAx : IGenericServiceA<EntityX>
        {
            public CompositeImplementationForGenericServicesAx(IEnumerable<IGenericServiceA<EntityX>> services) { }
        }
        private class GenericImplementationB<T1, T2> : IGenericServiceB<T1, T2>
        {
            public T1 PropertyT1 { get; }
            public T2 PropertyT2 { get; }

            public GenericImplementationB(T1 propertyT1, T2 propertyT2)
            {
                PropertyT1 = propertyT1;
                PropertyT2 = propertyT2;
            }
        }
        private class EntityX { }
        private class EntityY { }
        private class EntityZ { }
        
        private abstract class AbstractImplementation { }
        private class BaseImplementation
        {
            protected BaseImplementation() { }
        }
        private class Implementation : BaseImplementation { }
    }
}