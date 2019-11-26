using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Formula.Exceptions;

namespace Formula.Exceptions
{
    /// <summary>
    /// 访问控制异常
    /// </summary>
    /// <remarks>
    /// 访问控制异常主要是身份验证异常和授权控制异常基类    
    /// </remarks>
    public class AccessControlException : AppException
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public AccessControlException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        public AccessControlException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        public AccessControlException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        public AccessControlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
    }
}
