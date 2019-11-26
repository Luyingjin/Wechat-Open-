using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Formula.Exceptions
{
    /// <summary>
    /// 应用系统异常抽象基类
    /// </summary>
    [Serializable]
    public abstract class AppException : SysException
    {
        #region 构造函数

        /// <summary>
        /// 应用系统异常构造函数
        /// </summary>
        protected AppException()
        {

        }

        /// <summary>
        /// 应用系统异常构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        protected AppException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 应用系统异常构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        protected AppException(string message, System.Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 应用系统异常构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
    }
}
