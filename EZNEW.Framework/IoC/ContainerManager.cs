using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.IoC
{
    /// <summary>
    /// DI container manager
    /// </summary>
    public static class ContainerManager
    {

        static IServiceCollection defaultServices = null;//default services

        /// <summary>
        /// DI Container
        /// </summary>
        public static IDIContainer Container { get; private set; } = null;

        /// <summary>
        /// default service provider
        /// </summary>
        public static IServiceCollection ServiceCollection
        {
            get
            {
                return defaultServices;
            }
            set
            {
                SetDefaultServiceCollection(value);
            }
        }

        /// <summary>
        /// get service provider
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null;

        /// <summary>
        /// set default service collection
        /// </summary>
        /// <param name="services">default services</param>
        static void SetDefaultServiceCollection(IServiceCollection services)
        {
            if (services == null)
            {
                defaultServices?.Clear();
                ServiceProvider = null;
            }
            else
            {
                defaultServices = services;
                ServiceProvider = defaultServices.BuildServiceProvider();
            }
        }

        /// <summary>
        /// register service to default service collection
        /// </summary>
        /// <param name="serviceDescriptors">service descriptors</param>
        internal static void RegisterToServiceCollection(params ServiceDescriptor[] serviceDescriptors)
        {
            if (serviceDescriptors == null || serviceDescriptors.Length <= 0)
            {
                return;
            }
            foreach (var sd in serviceDescriptors)
            {
                defaultServices.Add(sd);
            }
            ServiceProvider = defaultServices.BuildServiceProvider();
        }

        /// <summary>
        /// determine whether the service is registered
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns></returns>
        internal static bool ServiceCollectionIsRegister<T>()
        {
            return defaultServices?.Any(c => c.ServiceType == typeof(T)) ?? false;
        }

        /// <summary>
        /// resolve registered services
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns></returns>
        internal static T ResolveFromServiceCollection<T>()
        {
            if (ServiceProvider != null)
            {
                return ServiceProvider.GetService<T>();
            }
            return default(T);
        }

        /// <summary>
        /// get register service
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <returns></returns>
        internal static object ResolveFromServiceCollection(Type serviceType)
        {
            if (ServiceProvider != null && serviceType != null)
            {
                return ServiceProvider.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        /// build service provider from serrvice collection
        /// </summary>
        /// <returns></returns>
        internal static IServiceProvider BuildServiceProviderFromServiceCollection()
        {
            ServiceProvider = defaultServices?.BuildServiceProvider();
            return ServiceProvider;
        }

        /// <summary>
        /// init di container
        /// </summary>
        /// <param name="defaultServices">default services</param>
        /// <param name="container">DI container</param>
        /// <param name="serviceRegisterAction">service register action</param>
        public static void Init(IServiceCollection defaultServices = null, IDIContainer container = null, Action<IDIContainer> serviceRegisterAction = null)
        {
            defaultServices = defaultServices ?? new ServiceCollection();
            container = container ?? new ServiceProviderContainer();
            SetDefaultServiceCollection(defaultServices);
            if (defaultServices != null && !(container is ServiceProviderContainer))
            {
                container.Register(defaultServices.ToArray());
            }
            serviceRegisterAction?.Invoke(container);
            SetDefaultServiceCollection(defaultServices);
            Container = container;
        }

        /// <summary>
        /// determine whether register the specified type
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <returns>registered or not</returns>
        public static bool IsRegister<T>()
        {
            return Container?.IsRegister<T>() ?? ServiceCollectionIsRegister<T>();
        }

        /// <summary>
        /// resolve the specified type
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <returns>resolve type</returns>
        public static T Resolve<T>()
        {
            T data = default(T);
            if (Container != null)
            {
                data = Container.Resolve<T>();
            }
            else
            {
                data = ResolveFromServiceCollection<T>();
            }
            return data;
        }

        /// <summary>
        /// get register service
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <returns></returns>
        public static object Resolve(Type serviceType)
        {
            object data = null;
            if (Container != null)
            {
                data = Container.Resolve(serviceType);
            }
            else
            {
                data = ResolveFromServiceCollection(serviceType);
            }
            return data;
        }

        /// <summary>
        /// returns the registered service or default service type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="DTI">default service</typeparam>
        /// <returns></returns>
        public static T Resolve<T, DTI>() where DTI : T, new()
        {
            var service = Resolve<T>();
            if (service == null)
            {
                service = new DTI();
            }
            return service;
        }

        /// <summary>
        /// register service,use singleton lifetime by default
        /// </summary>
        /// <typeparam name="FT">from type</typeparam>
        /// <typeparam name="TT">target type</typeparam>
        /// <typeparam name="lifetime">lifetime(default:signleton)</typeparam>
        /// <typeparam name="behaviors">behaviors</typeparam>
        public static void Register<FT, TT>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(typeof(FT), typeof(TT), lifetime, behaviors);
        }

        /// <summary>
        /// register service,use singleton lifetime by default
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <param name="implementationType">implementation type</param>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        /// <param name="behaviors">behaviors</param>
        public static void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(new MServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// register service
        /// </summary>
        /// <param name="serviceDescriptor">service descriptor</param>
        public static void Register(ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor == null)
            {
                return;
            }
            if (defaultServices != null)
            {
                defaultServices.Add(serviceDescriptor);
                ServiceProvider = defaultServices.BuildServiceProvider();
            }
            if (Container != null && !(Container is ServiceProviderContainer))
            {
                Container.Register(serviceDescriptor);
            }
        }

        /// <summary>
        /// build service provider
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider BuildServiceProvider()
        {
            IServiceProvider provider = null;
            if (Container != null && !(Container is ServiceProviderContainer))
            {
                provider = Container.BuildServiceProvider();
            }
            if (provider == null)
            {
                provider = BuildServiceProviderFromServiceCollection();
            }
            return provider;
        }
    }
}
