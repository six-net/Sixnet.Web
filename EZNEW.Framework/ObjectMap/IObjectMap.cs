using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ObjectMap
{
    /// <summary>
    /// object map interface
    /// </summary>
    public interface IObjectMap
    {
        #region convert object

        /// <summary>
        /// convert object
        /// </summary>
        /// <typeparam name="T">target data type</typeparam>
        /// <param name="sourceObj">source object</param>
        /// <returns>target data object</returns>
        T MapTo<T>(object sourceObj); 

        #endregion
    }
}
