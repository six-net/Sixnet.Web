using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EZNEW.Framework.Fault
{
    /// <summary>
    /// micbeach framework exception
    /// </summary>
    public class EZNEWException : Exception
    {
        public EZNEWException(){ }

        public EZNEWException(string message) : base(message) { }

        public EZNEWException(string message, Exception innerException) : base(message, innerException) { }

        protected EZNEWException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
