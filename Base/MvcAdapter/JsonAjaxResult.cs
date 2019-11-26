using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MvcAdapter
{
    /// <summary>
    /// Ajax请求返回数据格式
    /// </summary>
    public class JsonAjaxResult
    {
        /// <summary>
        /// 请求的状态码，操作成功 200， 操作失败 500
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取设置返回值，如果是多个值可以是一个匿名对象，如 new { Id = "GUID", Name = "测试账号"}
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 返回成功的Ajax请求信息
        /// </summary>
        public static JsonAjaxResult Successful(string msg = "操作成功")
        {
            return new JsonAjaxResult { StatusCode = 200, Message = msg };
        }

        /// <summary>
        /// 返回失败的Ajax请求信息
        /// </summary>
        public static JsonAjaxResult Failure(string msg = "操作失败", object data = null)
        {
            HttpContext.Current.Response.StatusCode = 500;
            return new JsonAjaxResult { StatusCode = 500, Message = msg, Data = data };
        }
    }
}
