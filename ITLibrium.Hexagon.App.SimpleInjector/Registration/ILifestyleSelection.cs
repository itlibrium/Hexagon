using SimpleInjector;

namespace ITLibrium.Hexagon.App.SimpleInjector.Registration
{
    public interface ILifestyleSelection : IAssembliesSelection
    {
        IAssembliesSelection UseLifestyle(Lifestyle lifestyle);
    }
}