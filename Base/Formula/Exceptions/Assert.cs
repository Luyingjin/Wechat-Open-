using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.Exceptions
{
    /// <summary>
    /// 断言类，该类根据添加判断，抛出指定的异常和异常信息
    /// </summary>
    public static class Assert
    {
        #region 公共方法

        /// <summary>
        /// 判断对象是否为空,如果不为空则抛出异常
        /// </summary>
        /// <typeparam name="T">异常的类型</typeparam>
        /// <param name="value">对象</param>
        /// <param name="message">消息</param>
        /// <param name="parameters">消息中需要格式化的参数</param>
        public static void IsNull<T>(object value, string message, params object[] parameters)
            where T : SysException, new()
        {
            if (value != null)
                ThrowException<T>(message, parameters);

        }

        /// <summary>
        /// 判断对象是否不为空,如果为空则抛出异常
        /// </summary>
        /// <typeparam name="T">异常的类型</typeparam>
        /// <param name="value">对象</param>
        /// <param name="message">消息</param>
        /// <param name="parameters">消息中需要格式化的参数</param>
        public static void IsNotNull<T>(object value, string message, params object[] parameters)
            where T : SysException, new()
        {
            if (value == null)
                ThrowException<T>(message, parameters);
        }

        /// <summary>
        /// 判断条件是否为True,如果为false则抛出异常
        /// </summary>
        /// <typeparam name="T">异常的类型</typeparam>
        /// <param name="condition">条件</param>
        /// <param name="message">消息</param>
        /// <param name="parameters">消息中需要格式化的参数</param>
        public static void IsTrue<T>(bool condition, string message, params object[] parameters)
            where T : SysException, new()
        {
            if (!condition)
                ThrowException<T>(message, parameters);
        }

        /// <summary>
        /// 判断条件是否为False,如果为True则抛出异常
        /// </summary>
        /// <typeparam name="T">异常的类型</typeparam>
        /// <param name="condition">条件</param>
        /// <param name="message">消息</param>
        /// <param name="parameters">消息中需要格式化的参数</param>
        public static void IsFalse<T>(bool condition, string message, params object[] parameters)
            where T : SysException, new()
        {
            if (condition)
                ThrowException<T>(message, parameters);
        }

        /// <summary>
        /// 不判断条件，直接抛出异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        public static void Fatal<T>(string message, params object[] parameters)
             where T : SysException, new()
        {
            ThrowException<T>(message, parameters);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 格式化消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="parameters">消息中需要格式化的参数</param>
        /// <returns>格式化后的消息</returns>
        private static string FormatMessage(string message, params object[] parameters)
        {
            if (parameters == null)
                return message;

            return string.Format(message, parameters);
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <typeparam name="T">异常的类型</typeparam>       
        /// <param name="message">消息</param>
        /// <param name="parameters">消息中需要格式化的参数</param>
        private static void ThrowException<T>(string message, params object[] parameters)
            where T : SysException, new()
        {
            string msg = FormatMessage(message, parameters);
            T t = new T();
            t.SetMessage(msg);

            throw t;
        }

        #endregion

    }
}
