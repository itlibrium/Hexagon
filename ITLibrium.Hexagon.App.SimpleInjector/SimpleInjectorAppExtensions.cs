using System;
using System.Linq;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.Gates;
using ITLibrium.Hexagon.App.Queries;
using ITLibrium.Hexagon.App.Services;
using ITLibrium.Hexagon.App.SimpleInjector.Gates;
using ITLibrium.Hexagon.App.SimpleInjector.Selectors;
using ITLibrium.Hexagon.SimpleInjector.Registration;
using ITLibrium.Hexagon.SimpleInjector.Selectors;

namespace ITLibrium.Hexagon.App.SimpleInjector
{
    public static class SimpleInjectorAppExtensions
    {
        public static ITypesSelection IncludeAppLogic(this ITypesSelection typesSelection)
        {
            typesSelection.Include(Components.Custom(IsAppLogic));
            typesSelection.Include(new ActionExecutorsSelector());
            typesSelection.Include(
                typeof(IAppService),
                typeof(IActionExecutor), typeof(IActionExecutor<>), typeof(ActionExecutor<>),
                typeof(ICommandHandler), typeof(ICommandHandler<>), typeof(ICommandHandler<,>),
                typeof(IFinder), typeof(IFinder<>), typeof(IFinder<,>),
                typeof(IGate), typeof(SimpleInjectorGate),
                typeof(IGatePolicy));

            return typesSelection;
        }
        
        private static bool IsAppLogic(Type type)
        {
            return type.GetInterfaces().Any(i => i == typeof(IAppService)
                                                 || i == typeof(ICommandHandler)
                                                 || i == typeof(IFinder)
                                                 || i == typeof(IGatePolicy));
        }

//        public static ITypesSelection IncludeTransaction(this ITypesSelection typesSelection)
//        {
//            throw new NotImplementedException();
//            
//            typesSelection.IncludeDecorator(typeof(IActionExecutor<>), typeof(ServiceTransactionDecorator<>),
//                Components.AnnotatedBy(typeof(TransactionalAttribute)));
//
//            return typesSelection;
//        }
    }
}