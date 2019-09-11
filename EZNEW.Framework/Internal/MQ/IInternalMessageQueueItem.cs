using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Internal.MQ
{
    /// <summary>
    /// internal message queue command
    /// </summary>
    public interface IInternalMessageQueueItem
    {
        #region run internal message queue Item

        /// <summary>
        /// run internal message queue Item
        /// </summary>
        void Run(); 

        #endregion
    }
}
