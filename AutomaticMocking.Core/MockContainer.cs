namespace AutomaticMocking.Core
{
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using System;
    using System.Reflection;

    public class MockContainer
    {
        private readonly MockRepository repository;
        private readonly MethodInfo mockCreateMethod;
        private readonly ServiceCollection serviceCollection;

        public MockContainer(MockRepository repository)
        {
            this.repository = repository;
            this.mockCreateMethod = this.repository.GetType().GetMethod("Create", Array.Empty<Type>())!;
            this.serviceCollection = new ServiceCollection();
        }

        public Mock<T> GetMock<T>() where T : class
        {
            var obj = this.Create<T>();
            return ((IMocked<T>)obj).Mock;
        }

        public T Create<T>() where T : class
        {
            if (!IsMockedServiceRegistred<T>())
            {
                AddWithDependencies<T>();
            }
            return serviceCollection.BuildServiceProvider().GetRequiredService<T>();
        }

        public T Create<T>(Func<IServiceProvider, T> factory) where T : class
        {
            if (!IsMockedServiceRegistred<T>())
            {
                serviceCollection.AddScoped(factory);
            }
            return serviceCollection.BuildServiceProvider().GetRequiredService<T>();
        }

        public void Register<T>() where T : class
            => this.serviceCollection.AddScoped<T>();

        private bool IsMockedServiceRegistred<T>() where T : class
        {
            return serviceCollection.Any(x => x.ServiceType == typeof(T));
        }
        private void AddWithDependencies<T>(Func<IServiceProvider, T>? factory = null) where T : class
        {
            var createdType = typeof(T);

            if (factory is not null)
            {

                return;
            }

            if (createdType.IsClass)
                serviceCollection.AddScoped<T>();

            if (createdType.IsInterface)
            {
                AddDependencyToServiceCollection(createdType);
                return;
            }

            foreach (var param in GetContructorParamsToRegistrer(createdType))
            {
                AddDependencyToServiceCollection(param.ParameterType);
            }
        }

        private static ParameterInfo[] GetContructorParamsToRegistrer(Type createdType)
        {
            var contructors = createdType.GetConstructors();
            var mostNumberOfContructorParams = contructors.Max(x => x.GetParameters().Length);

            var parameters = contructors.First(x => x.GetParameters().Length == mostNumberOfContructorParams)
                                        .GetParameters();

            return parameters.Where(p => p.ParameterType.IsInterface || p.ParameterType.IsClass).ToArray();
        }

        private void AddDependencyToServiceCollection(Type type)
        {
            var genericMethod = this.mockCreateMethod.MakeGenericMethod(new[] { type });

            var mock = (Mock?)genericMethod.Invoke(this.repository, null);

            if (mock is not null)
            {
                serviceCollection.AddScoped(type, factory => mock!.Object);
            }
        }
    }
}