using System;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface ITypesSelection
    {
        ITypesSelection ExcludeTypes(params Type[] types);
        ITypesSelection IncludeTypes(params Type[] types);
    }
}