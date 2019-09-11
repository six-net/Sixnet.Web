using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Application
{
    /// <summary>
    /// Application Info
    /// </summary>
    public class ApplicationInfo
    {
        #region Propertys

        /// <summary>
        /// App Code
        /// </summary>
        public string AppCode
        {
            get; set;
        }

        /// <summary>
        /// App Secret
        /// </summary>
        public string AppSecret
        {
            get; set;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Application Type
        /// </summary>
        public ApplicationType Type
        {
            get; set;
        }

        /// <summary>
        /// Application Status
        /// </summary>
        public ApplicationStatus Status
        {
            get; set;
        }

        #endregion
    }
}
