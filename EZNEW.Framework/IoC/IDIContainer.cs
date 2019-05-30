using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.IoC
{
    /// <summary>
    /// DI container interface
    /// </summary>
    public interface IDIContainer
    {
        #region register

        /// <summary>
        /// register service
        /// </summary>
        /// <typeparam name="ST">service type</typeparam>
        /// <typeparam name="IT">implementation type</typeparam>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        /// <param name="behaviors">behaviors</param>
        void Register<ST, IT>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null);

        /// <summary>
        /// register service
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <param name="implementationType">implementation type</param>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        /// <param name="behaviors">behaviors</param>
        void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton,IEnumerable<Type> behaviors = null);

        /// <summary>
        /// register service
        /// </summary>
        /// <param name="serviceDescriptors">service descriptor</param>
        void Register(params ServiceDescriptor[] serviceDescriptors);

        #endregion

        #region determine register

        /// <summary>
        ///determine whether has registered the specified type
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <returns>determined value</returns>
        bool IsRegister<T>();

        #endregion

        #region get register

        /// <summary>
        /// get the register type by the specified type
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <returns>register type</returns>
        T Resolve<T>();

        /// <summary>
        /// get register service
        /// </summary>
        /// <param name="serviceType">service type</param>
        /// <returns></returns>
        object Resolve(Type serviceType);

        #endregion

        #region build service provider

        /// <summary>
        /// build service provider
        /// </summary>
        /// <returns></returns>
        IServiceProvider BuildServiceProvider();

        #endregion
    }
}
