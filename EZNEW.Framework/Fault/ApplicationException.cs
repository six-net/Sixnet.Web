using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EZNEW.Framework.Fault
{
    /// <summary>
    /// application exception
    /// </summary>
    public class ApplicationException : Exception
    {
        public ApplicationException() { }

        public ApplicationException(string message) : base(message) { }

        public ApplicationException(string message, Exception innerException) : base(message, innerException) { }

        protected ApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
