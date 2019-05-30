using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.IoC
{
    /// <summary>
    /// service descriptor
    /// </summary>
    public class MServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// service behaviors
        /// </summary>
        public IEnumerable<Type> Behaviors
        {
            get; set;
        }

        public MServiceDescriptor(Type serviceType, object instance) : base(serviceType, instance)
        {
        }

        public MServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime)
        {
        }

        public MServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime) : base(serviceType, factory, lifetime)
        {
        }
    }
}
