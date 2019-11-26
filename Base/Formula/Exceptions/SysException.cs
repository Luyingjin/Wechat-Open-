using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Formula.Exceptions
{
    /// <summary>
    /// 系统异常抽象基类
    /// </summary>    
    [Serializable]
    public abstract class SysException : Exception
    {
        #region 构造函数

        /// <summary>
        /// 系统异常构造函数
        /// </summary>
        protected SysException()
        {

        }

        /// <summary>
        /// 系统异常构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        protected SysException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 系统异常构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        protected SysException(string message, System.Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 系统异常构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        protected SysException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion

        #region 公共属性

        private string _Message;

        /// <summary>
        /// 消息
        /// </summary>
        public override string Message
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Message))
                    return base.Message;
                else
                    return _Message;
            }
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 设置异常消息
        /// </summary>
        /// <param name="message">消息</param>
        internal void SetMessage(string message)
        {
            _Message = message;
        }

        #endregion
    }
}
