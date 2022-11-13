namespace AutomaticMocking.Core
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Moq;
    using System;
    using System.Reflection;

    public class MockContainer : IServiceResolver
    {
        private readonly MockRepository _repository;
        private readonly MethodInfo _mockCreateMethod;
        private readonly ServiceCollection _serviceCollection;

        public MockContainer(MockRepository repository)
        {
            _repository = repository;
            _mockCreateMethod = _repository.GetType().GetMethod("Create", Array.Empty<Type>())!;
            _serviceCollection = new ServiceCollection();
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
            return _serviceCollection.BuildServiceProvider().GetRequiredService<T>();
        }

        public T Create<T>(Func<IServiceResolver, T> factory) where T : class
        {
            if (!IsMockedServiceRegistred<T>())
            {
                _serviceCollection.AddScoped(serviceProvider => factory(this));
            }
            return _serviceCollection.BuildServiceProvider().GetRequiredService<T>();
        }

        public void Register<TService, TImplementation>() where TImplementation : class
            => _serviceCollection.AddScoped(typeof(TService), typeof(TImplementation));

        public void Register<T>(T service) where T : class
            => _serviceCollection.AddScoped(serviceProvider => service);

        public void Register<TService>(Func<IServiceResolver, TService> factory) where TService : class
            => _serviceCollection.AddScoped(serviceProvider => factory(this));

        public T Resolve<T>() where T : class
            => _serviceCollection.BuildServiceProvider().GetRequiredService<T>();

        private bool IsMockedServiceRegistred<T>() where T : class 
            => _serviceCollection.Any(x => x.ServiceType == typeof(T));

        private void AddWithDependencies<T>(Func<IServiceProvider, T>? factory = null) where T : class
        {
            if (factory is not null)
            {
                return;
            }

            var createdType = typeof(T);
            if (createdType.IsClass)
            {
                _serviceCollection.AddScoped<T>();
            }

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
            var genericMethod = _mockCreateMethod.MakeGenericMethod(new[] { type });

            var mock = (Mock?)genericMethod.Invoke(_repository, null);

            if (mock is not null)
            {
                _serviceCollection.TryAddScoped(type, factory => mock!.Object);
            }
        }
    }
}