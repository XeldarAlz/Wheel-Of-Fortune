using System;
using System.Collections.Generic;

namespace WheelOfFortune.Infrastructure.DependencyInjection
{
    public sealed class ServiceLocator
    {
        private static ServiceLocator _instance;

        private readonly Dictionary<Type, object> _services;

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }

                return _instance;
            }
        }

        private ServiceLocator()
        {
            _services = new Dictionary<Type, object>(16);
        }

        public void Register<TService>(TService service) where TService : class
        {
            Type serviceType = typeof(TService);
            _services[serviceType] = service;
        }

        public TService Get<TService>() where TService : class
        {
            Type serviceType = typeof(TService);
            
            if (_services.TryGetValue(serviceType, out object service))
            {
                return service as TService;
            }

            return null;
        }

        public void Unregister<TService>() where TService : class
        {
            Type serviceType = typeof(TService);
            _services.Remove(serviceType);
        }
    }
}
