using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using WeChat.Logic.Service.Utilities;
using WeChat.Logic.Domain;
using Formula.Helper;
using Config.Logic;

namespace WeChat.Logic.Service.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private MpAccount account = null;
        private System.Data.Entity.DbContext entities = null;
        private static int cachesecond = 1000;
        private string domain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
        private string msgcachetime = System.Configuration.ConfigurationManager.AppSettings["MessageCacheTime"];

        public CustomMessageHandler(Stream inputStream, PostModel postModel, MpAccount ac, System.Data.Entity.DbContext etis, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
            account = ac;
            entities = etis;
            int.TryParse(msgcachetime, out cachesecond);
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var defaultMessage = DefaultResponseMessage(requestMessage);
            if (string.IsNullOrEmpty(requestMessage.Content))
                return defaultMessage;
            #region 记录日志
            try
            {
                var opid = requestMessage.FromUserName;
                var entitymsg = new MpEventRequestMsgLog();
                entitymsg.ID = Formula.FormulaHelper.CreateGuid();
                entitymsg.MpID = account.ID;
                entitymsg.OpenID = opid;
                entitymsg.MsgType = requestMessage.MsgType.ToString();
                entitymsg.MsgId = requestMessage.MsgId.ToString();
                entitymsg.Content = requestMessage.Content;
                entitymsg.CreateDate = System.DateTime.Now;
                entities.Set<MpEventRequestMsgLog>().Add(entitymsg);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录文本回复信息出错：原因{1}", account.ID, ex.Message));
            }
            #endregion
            var rs = CacheHelper.Get(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content));
            var rstype = CacheHelper.Get(string.Format("TextRequest_{0}_{1}_Type", account.AppID, requestMessage.Content));
            if (rs == null || rstype == null)
            {
                //关键字回复
                var entity = entities.Set<MpKeyWordReply>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.KeyWord == requestMessage.Content).FirstOrDefault();
                if (entity != null)
                {
                    CacheHelper.Set(string.Format("TextRequest_{0}_{1}_Type", account.AppID, requestMessage.Content), entity.ReplyType, cachesecond);
                    if (entity.ReplyType == MpMessageType.none.ToString())
                    {
                        return defaultMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.image.ToString())
                    {
                         var responseMessage = base.CreateResponseMessage<ResponseMessageImage>();
                         responseMessage.Image.MediaId = entity.ImageMediaID;
                         CacheHelper.Set(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content), responseMessage, cachesecond);
                         return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.text.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                        responseMessage.Content = entity.Content;
                        CacheHelper.Set(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.voice.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVoice>();
                        responseMessage.Voice.MediaId = entity.VoiceMediaID;
                        CacheHelper.Set(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.video.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVideo>();
                        var video = entities.Set<MpMediaVideo>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.VideoID).FirstOrDefault();
                        if (video == null)
                            return defaultMessage;
                        responseMessage.Video.MediaId = video.MediaID;
                        responseMessage.Video.Title = video.Title;
                        responseMessage.Video.Description = video.Description;
                        CacheHelper.Set(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.mpnews.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageNews>();
                        var article = entities.Set<MpSelfArticle>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.ArticleID).FirstOrDefault();
                        if (article == null)
                            return defaultMessage;
                        responseMessage.Articles.Add(new Article()
                        {
                            Title = article.Title,
                            Description = article.Description,
                            Url = article.Url,
                            PicUrl = string.Format("http://{0}/wechatservice/api/Image/Get/{1}", domain, article.PicFileID),
                        });
                        CacheHelper.Set(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.mpmultinews.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageNews>();
                        var article = entities.Set<MpSelfArticleGroup>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.ArticleGroupID).FirstOrDefault();
                        if (article == null || article.MpSelfArticleGroupItem == null || article.MpSelfArticleGroupItem.Count(c => c.MpSelfArticle != null) < 2)
                            return defaultMessage;
                        foreach (var item in article.MpSelfArticleGroupItem.Where(c => c.MpSelfArticle != null))
                            responseMessage.Articles.Add(new Article()
                            {
                                Title = item.MpSelfArticle.Title,
                                Description = item.MpSelfArticle.Description,
                                Url = item.MpSelfArticle.Url,
                                PicUrl = string.Format("http://{0}/wechatservice/api/Image/Get/{1}", domain, item.MpSelfArticle.PicFileID),
                            });
                        CacheHelper.Set(string.Format("TextRequest_{0}_{1}", account.AppID, requestMessage.Content), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else
                        return defaultMessage;
                }
                //其他回复
                else
                    return defaultMessage;
            }
            else
            {
                var rstp = rstype.ToString();
                if (rstp == MpMessageType.image.ToString())
                    return rs as ResponseMessageImage;
                else if (rstp == MpMessageType.mpmultinews.ToString())
                    return rs as ResponseMessageNews;
                else if (rstp == MpMessageType.mpnews.ToString())
                    return rs as ResponseMessageNews;
                else if (rstp == MpMessageType.text.ToString())
                    return rs as ResponseMessageText;
                else if (rstp == MpMessageType.video.ToString())
                    return rs as ResponseMessageVideo;
                else if (rstp == MpMessageType.voice.ToString())
                    return rs as ResponseMessageVoice;
                return rs as IResponseMessageBase;
            }
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            return DefaultResponseMessage(requestMessage);
        }

        public override IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            //记录短视频回复信息
            try
            {
                var opid = requestMessage.FromUserName;
                var entitymsg = new MpEventRequestMsgLog();
                entitymsg.ID = Formula.FormulaHelper.CreateGuid();
                entitymsg.MpID = account.ID;
                entitymsg.OpenID = opid;
                entitymsg.MsgType = requestMessage.MsgType.ToString();
                entitymsg.MsgId = requestMessage.MsgId.ToString();
                entitymsg.MediaId = requestMessage.MediaId;
                entitymsg.CreateDate = System.DateTime.Now;
                entities.Set<MpEventRequestMsgLog>().Add(entitymsg);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录短视频回复信息出错：原因{1}", account.ID, ex.Message));
            }
            return DefaultResponseMessage(requestMessage);
            
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            try
            {
                var opid = requestMessage.FromUserName;
                var entitymsg = new MpEventRequestMsgLog();
                entitymsg.ID = Formula.FormulaHelper.CreateGuid();
                entitymsg.MpID = account.ID;
                entitymsg.OpenID = opid;
                entitymsg.MsgType = requestMessage.MsgType.ToString();
                entitymsg.MsgId = requestMessage.MsgId.ToString();
                entitymsg.MediaId = requestMessage.MediaId;
                entitymsg.CreateDate = System.DateTime.Now;
                entities.Set<MpEventRequestMsgLog>().Add(entitymsg);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录图片回复信息出错：原因{1}", account.ID, ex.Message));
            }
            return DefaultResponseMessage(requestMessage);
            
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            try
            {
                var opid = requestMessage.FromUserName;
                var entitymsg = new MpEventRequestMsgLog();
                entitymsg.ID = Formula.FormulaHelper.CreateGuid();
                entitymsg.MpID = account.ID;
                entitymsg.OpenID = opid;
                entitymsg.MsgType = requestMessage.MsgType.ToString();
                entitymsg.MsgId = requestMessage.MsgId.ToString();
                entitymsg.MediaId = requestMessage.MediaId;
                entitymsg.CreateDate = System.DateTime.Now;
                entities.Set<MpEventRequestMsgLog>().Add(entitymsg);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录语音回复信息出错：原因{1}", account.ID, ex.Message));
            }
            return DefaultResponseMessage(requestMessage);
        }
        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            try
            {
                var opid = requestMessage.FromUserName;
                var entitymsg = new MpEventRequestMsgLog();
                entitymsg.ID = Formula.FormulaHelper.CreateGuid();
                entitymsg.MpID = account.ID;
                entitymsg.OpenID = opid;
                entitymsg.MsgType = requestMessage.MsgType.ToString();
                entitymsg.MsgId = requestMessage.MsgId.ToString();
                entitymsg.MediaId = requestMessage.MediaId;
                entitymsg.CreateDate = System.DateTime.Now;
                entities.Set<MpEventRequestMsgLog>().Add(entitymsg);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录视频回复信息出错：原因{1}", account.ID, ex.Message));
            }
            return DefaultResponseMessage(requestMessage);
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            try
            {
                var opid = requestMessage.FromUserName;
                var entitymsg = new MpEventRequestMsgLog();
                entitymsg.ID = Formula.FormulaHelper.CreateGuid();
                entitymsg.MpID = account.ID;
                entitymsg.OpenID = opid;
                entitymsg.MsgType = requestMessage.MsgType.ToString();
                entitymsg.MsgId = requestMessage.MsgId.ToString();
                entitymsg.Title = requestMessage.Title;
                entitymsg.Description = requestMessage.Description;
                entitymsg.url = requestMessage.Url;
                entitymsg.CreateDate = System.DateTime.Now;
                entities.Set<MpEventRequestMsgLog>().Add(entitymsg);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录链接回复信息出错：原因{1}", account.ID, ex.Message));
            }
            return DefaultResponseMessage(requestMessage);
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
             * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
             * 只需要在这里统一发出委托请求，如：
             * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
             * return responseMessage;
             */

            var rs = CacheHelper.Get(string.Format("DefaultRequest_{0}", account.AppID));
            var rstype = CacheHelper.Get(string.Format("DefaultRequest_{0}_Type", account.AppID));
            if (rs == null || rstype == null)
            {
                var eventtype = MpEventType.AutoReply.ToString();
                var entity = entities.Set<MpEvent>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.EventType == eventtype).FirstOrDefault();
                if (entity != null)
                {
                    CacheHelper.Set(string.Format("DefaultRequest_{0}_Type", account.AppID), entity.ReplyType, cachesecond);
                    if (entity.ReplyType == MpMessageType.none.ToString())
                    {
                        return null;
                    }
                    else if (entity.ReplyType == MpMessageType.image.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageImage>();
                        responseMessage.Image.MediaId = entity.ImageMediaID;
                        CacheHelper.Set(string.Format("DefaultRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.text.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                        responseMessage.Content = entity.Content;
                        CacheHelper.Set(string.Format("DefaultRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.voice.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVoice>();
                        responseMessage.Voice.MediaId = entity.VoiceMediaID;
                        CacheHelper.Set(string.Format("DefaultRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.video.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVideo>();
                        var video = entities.Set<MpMediaVideo>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.VideoID).FirstOrDefault();
                        if (video == null)
                            return null;
                        responseMessage.Video.MediaId = video.MediaID;
                        responseMessage.Video.Title = video.Title;
                        responseMessage.Video.Description = video.Description;
                        CacheHelper.Set(string.Format("DefaultRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.mpnews.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageNews>();
                        var article = entities.Set<MpSelfArticle>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.ArticleID).FirstOrDefault();
                        if (article == null)
                            return null;
                        responseMessage.Articles.Add(new Article()
                        {
                            Title = article.Title,
                            Description = article.Description,
                            Url = article.Url,
                            PicUrl = string.Format("http://{0}/wechatservice/api/Image/Get/{1}", domain, article.PicFileID),
                        });
                        CacheHelper.Set(string.Format("DefaultRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.mpmultinews.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageNews>();
                        var article = entities.Set<MpSelfArticleGroup>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.ArticleGroupID).FirstOrDefault();
                        if (article == null || article.MpSelfArticleGroupItem == null || article.MpSelfArticleGroupItem.Count(c => c.MpSelfArticle != null) < 2)
                            return null;
                        foreach (var item in article.MpSelfArticleGroupItem.Where(c => c.MpSelfArticle != null))
                            responseMessage.Articles.Add(new Article()
                            {
                                Title = item.MpSelfArticle.Title,
                                Description = item.MpSelfArticle.Description,
                                Url = item.MpSelfArticle.Url,
                                PicUrl = string.Format("http://{0}/wechatservice/api/Image/Get/{1}", domain, item.MpSelfArticle.PicFileID),
                            });
                        CacheHelper.Set(string.Format("DefaultRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else
                        return null;
                }
                //其他回复
                else
                    return null;
            }
            else
            {
                var rstp = rstype.ToString();
                if (rstp == MpMessageType.image.ToString())
                    return rs as ResponseMessageImage;
                else if (rstp == MpMessageType.mpmultinews.ToString())
                    return rs as ResponseMessageNews;
                else if (rstp == MpMessageType.mpnews.ToString())
                    return rs as ResponseMessageNews;
                else if (rstp == MpMessageType.text.ToString())
                    return rs as ResponseMessageText;
                else if (rstp == MpMessageType.video.ToString())
                    return rs as ResponseMessageVideo;
                else if (rstp == MpMessageType.voice.ToString())
                    return rs as ResponseMessageVoice;
                return rs as IResponseMessageBase;
            }
        }
    }
}
