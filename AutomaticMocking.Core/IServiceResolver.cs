namespace AutomaticMocking.Core
{
    public interface IServiceResolver
    {
        T Resolve<T>() where T : class;
    }
}