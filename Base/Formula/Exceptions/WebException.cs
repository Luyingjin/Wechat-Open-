using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Formula.Exceptions;

namespace Formula.Exceptions
{
    /// <summary>
    /// Web 前台的异常
    /// </summary>
    public class WebException : AppException
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        public WebException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        public WebException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        public WebException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
    }
}
