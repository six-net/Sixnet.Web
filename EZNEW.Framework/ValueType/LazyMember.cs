using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Lazy Member
    /// </summary>
    public class LazyMember<T>
    {
        #region Propertys

        /// <summary>
        /// Get Value
        /// </summary>
        public T Value
        {
            get
            {
                return GetValue();
            }
            private set
            {
                SetValue(value, false);
            }
        }

        /// <summary>
        /// value factory
        /// </summary>
        protected Func<T> ValueFactory
        {
            get; set;
        }

        /// <summary>
        /// is created value
        /// </summary>
        protected bool IsCreatedValue
        {
            get; set;
        }

        /// <summary>
        /// Get Current Value
        /// </summary>
        public T CurrentValue { get; private set; } = default(T);

        #endregion

        #region constructor

        /// <summary>
        /// instance a LazyMember<> object
        /// </summary>
        /// <param name="valueFactory">value factory</param>
        public LazyMember(Func<T> valueFactory)
        {
            ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        #endregion

        #region methods

        /// <summary>
        /// get value
        /// </summary>
        /// <returns></returns>
        T GetValue()
        {
            if (IsCreatedValue)
            {
                return CurrentValue;
            }
            var newValue = ValueFactory();
            SetValue(newValue, true);
            return CurrentValue;
        }

        /// <summary>
        /// set value
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="createdValue">set value is inited or not</param>
        public void SetValue(T value, bool createdValue = true)
        {
            IsCreatedValue = createdValue;
            CurrentValue = value;
        }

        /// <summary>
        /// implicit convertto T
        /// </summary>
        /// <param name="lazyMember"></param>
        public static implicit operator T(LazyMember<T> lazyMember)
        {
            if (lazyMember == null)
            {
                return default(T);
            }
            return lazyMember.Value;
        }

        /// <summary>
        /// implicit convertto lazymember
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator LazyMember<T>(T value)
        {
            var lazyMember = new LazyMember<T>(() => default(T));
            lazyMember.SetValue(value, true);
            return lazyMember;
        }

        #endregion
    }
}
