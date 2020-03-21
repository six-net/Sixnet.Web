using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Internal.MQ
{
    /// <summary>
    /// internal message queue command
    /// </summary>
    public interface IInternalMessageQueueCommand
    {
        #region run internal message queue command

        /// <summary>
        /// run internal message queue command
        /// </summary>
        void Run();

        #endregion
    }
}
