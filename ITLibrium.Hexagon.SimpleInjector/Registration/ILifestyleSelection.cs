using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface ILifestyleSelection
    {
        IAssembliesSelection UseLifestyle(Lifestyle lifestyle);
    }
}