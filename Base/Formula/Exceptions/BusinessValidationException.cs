using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Formula.Exceptions;

namespace Formula.Exceptions
{
    /// <summary>
    /// 业务逻辑的验证异常
    /// </summary>
    public class BusinessValidationException : AppException
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BusinessValidationException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        public BusinessValidationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        public BusinessValidationException(string message, System.Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        public BusinessValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="messageList">异常的消息列表</param>
        public BusinessValidationException(string[] messageList)
        {
            _MessageList = messageList;
        }

        #endregion


        #region 公共属性

        private string[] _MessageList;

        /// <summary>
        /// 异常的消息列表
        /// </summary>
        public string[] MessageList
        {
            get
            {
                if (_MessageList != null)
                    return _MessageList;

                return new string[] { Message };
            }
        }

        /// <summary>
        /// 消息信息
        /// </summary>
        public override string Message
        {
            get
            {
                if (MessageList != null && MessageList.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in MessageList)
                        sb.Append(s);

                    return sb.ToString();
                }
                else
                {
                    return base.Message;
                }
            }
        }

        #endregion
    }
}
