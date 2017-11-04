using System;
using System.Collections.Generic;
using System.Linq;
using ITLibrium.Hexagon.App.Services;
using ITLibrium.Hexagon.SimpleInjector.Selectors;

namespace ITLibrium.Hexagon.App.SimpleInjector.Selectors
{
    internal class ActionExecutorsSelector : IRelatedComponentsSelector
    {
        public IEnumerable<Type> GetRelatedComponents(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(IAppService)))
            {
                yield return typeof(IActionExecutor<>).MakeGenericType(type);
                yield return typeof(ActionExecutor<>).MakeGenericType(type);
            }
        }
    }
}