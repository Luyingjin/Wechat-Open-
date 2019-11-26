using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Formula.Exceptions
{
    /// <summary>
    /// 身份验证异常
    /// </summary>
    /// <remarks>
    /// 身份验证异常主要用在2个地方
    /// 1.没有身份的用户访问系统时发生
    /// 2.外部系统调用服务接口时，没有通过身份验证
    /// </remarks>
    public class AuthenticationException : AccessControlException
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthenticationException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        public AuthenticationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        public AuthenticationException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        public AuthenticationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
    }
}
