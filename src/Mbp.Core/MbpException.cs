using System;
using System.Runtime.Serialization;

namespace Mbp.Core
{
    /// <summary>
    /// 异常基类
    /// </summary>
    [Serializable]
    public class MbpException : Exception
    {
        public MbpException()
        {

        }

        public MbpException(string message)
            : base(message)
        {

        }

        public MbpException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public MbpException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
