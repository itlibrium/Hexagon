using SimpleInjector;

namespace ITLibrium.Hexagon.SimpleInjector.Registration
{
    public interface ILifestyleSelection : IAssembliesSelection
    {
        IAssembliesSelection UseScopedLifestyle();
        IAssembliesSelection UseLifestyle(Lifestyle lifestyle);
    }
}