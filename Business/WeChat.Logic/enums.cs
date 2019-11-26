using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WeChat.Logic
{
    /// <summary>
    /// 是否
    /// </summary>
    [Description("是否")]
    public enum SysBool
    {
        /// <summary>
        /// 是
        /// </summary>
        [Description("是")]
        T,
        /// <summary>
        /// 否
        /// </summary>
        [Description("否")]
        F,
    }

    public class WeChatConfig
    {

        public static string FileServerName
        {
            get
            {
                if (!System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FS_ServerName"))
                    throw new Exception("请在配置文件中配置文件服务的名称:FS_ServerName");

                return System.Configuration.ConfigurationManager.AppSettings["FS_ServerName"];
            }
        }

        public static string MasterServerUrl
        {
            get
            {
                if (!System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FS_MasterServerUrl"))
                    throw new Exception("请在配置文件中配置文件服务的名称:FS_MasterServerUrl");

                return System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];
            }
        }
    }


    /// <summary>
    /// 是否
    /// </summary>
    [Description("微信消息类型")]
    public enum MpMessageType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        none,
        /// <summary>
        /// 图文消息
        /// </summary>
        [Description("图文消息")]
        mpnews,
        /// <summary>
        /// 多图文消息
        /// </summary>
        [Description("多图文消息")]
        mpmultinews,
        /// <summary>
        /// 文本
        /// </summary>
        [Description("文本")]
        text,
        /// <summary>
        /// 语音
        /// </summary>
        [Description("语音")]
        voice,
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        image,
        /// <summary>
        /// 视频
        /// </summary>
        [Description("视频")]
        video,
    }

    public enum MpEventType
    {
        AutoReply,
        Subscribe,
    }
}
