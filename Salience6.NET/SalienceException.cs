using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.Serialization;

namespace Lexalytics
{
    [Serializable]
    public class SalienceException : Exception
    {
        #region Constructors
        public SalienceException()
        {
        }

        public SalienceException(string sMessage)
            : base(sMessage)
        {
        }

        public SalienceException(string sMessage, Exception innerException)
            : base(sMessage, innerException)
        {
        }

        protected SalienceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
