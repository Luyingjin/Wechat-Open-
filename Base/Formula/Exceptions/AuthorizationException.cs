using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Formula.Exceptions
{
    /// <summary>
    /// 授权验证异常
    /// </summary>    
    /// <remarks>
    /// 授权验证异常主要用在以下地方
    /// 1.用户访问了他不具有权限访问的资源
    /// 2.用户执行了他不能执行的操作
    /// </remarks>
    public class AuthorizationException : AccessControlException
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthorizationException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        public AuthorizationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        public AuthorizationException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        public AuthorizationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
    }
}
