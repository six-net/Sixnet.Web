using EZNEW.Framework.IoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Drawing
{
    /// <summary>
    /// verification code factory
    /// </summary>
    public static class VerificationCodeFactory
    {
        /// <summary>
        /// generate code
        /// </summary>
        /// <returns></returns>
        public static VerificationCodeBase GetVerificationCode()
        {
            return ContainerManager.Resolve<VerificationCodeBase>();
        }
    }

    /// <summary>
    /// VerificationCode Type
    /// </summary>
    public enum VerificationCodeType
    {
        Number = 2,
        Letter = 4,
        NumberAndLetter = 6
    }
}
