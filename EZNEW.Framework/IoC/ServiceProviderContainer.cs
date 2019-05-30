using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EZNEW.Framework.IoC
{
    /// <summary>
    /// service provider container
    /// </summary>
    internal class ServiceProviderContainer : IDIContainer
    {
        /// <summary>
        /// determine whether the service is registered
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns></returns>
        public bool IsRegister<T>()
        {
            return ContainerManager.ServiceCollectionIsRegister<T>();
        }

        /// <summary>
        /// register service
        /// </summary>
        /// <typeparam name="ST">service type</typeparam>
        /// <typeparam name="IT">implementation type</typeparam>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        /// <param name="behaviors">behaviors</param>
        public void Register<ST, IT>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(typeof(ST), typeof(IT), lifetime, behaviors);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <param name="implementationType">implementation type</param>
        /// <param name="behaviors">behaviors</param>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(new MServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// register service
        /// </summary>
        /// <param name="serviceDescriptors">service descriptor</param>
        public void Register(params ServiceDescriptor[] serviceDescriptors)
        {
            ContainerManager.RegisterToServiceCollection(serviceDescriptors);
        }

        /// <summary>
        /// resolve registered services
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return ContainerManager.ResolveFromServiceCollection<T>();
        }

        /// <summary>
        /// get register service
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            return ContainerManager.ResolveFromServiceCollection(serviceType);
        }

        /// <summary>
        /// build service provider
        /// </summary>
        /// <returns></returns>
        public IServiceProvider BuildServiceProvider()
        {
            return ContainerManager.BuildServiceProviderFromServiceCollection();
        }
    }
}
