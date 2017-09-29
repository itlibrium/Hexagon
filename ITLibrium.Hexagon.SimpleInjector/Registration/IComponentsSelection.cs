using ITLibrium.Hexagon.SimpleInjector.Selectors;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface IComponentsSelection
    {
        ITypesSelection SelectComponents(params IComponentSelector[] selectors);
    }
}