using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Web.Script.Serialization;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using WeChat.Logic.BusinessFacade;
using WeChat.Logic.Domain;
using WeChat.Logic.Service.Utilities;
using MvcAdapter;
using Formula.Helper;
using Config.Logic;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP.AdvancedAPIs.Groups;

namespace WeChat.Logic.Service.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler 
    {
        private string GetWelcomeInfo()
        {
            //获取Senparc.Weixin.MP.dll版本信息
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(HttpContext.Current.Server.MapPath("~/bin/Senparc.Weixin.MP.dll"));
            var version = string.Format("{0}.{1}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart);
            return string.Format(
@"欢迎关注【Senparc.Weixin.MP 微信公众平台SDK】，当前运行版本：v{0}。
您可以发送【文字】【位置】【图片】【语音】等不同类型的信息，查看不同格式的回复。

您也可以直接点击菜单查看各种类型的回复。
还可以点击菜单体验微信支付。

SDK官方地址：http://weixin.senparc.com
源代码及Demo下载地址：https://github.com/JeffreySu/WeiXinMPSDK
Nuget地址：https://www.nuget.org/packages/Senparc.Weixin.MP",
                version);
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey

            return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            var rs = CacheHelper.Get(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey));
            var rstype = CacheHelper.Get(string.Format("EventClickRequest_{0}_{1}_Type", account.AppID, requestMessage.EventKey));
            #region 记录日志
            try
            {
                var opid = requestMessage.FromUserName;
                var entityclick = new MpEventClickViewLog();
                entityclick.ID = Formula.FormulaHelper.CreateGuid();
                entityclick.MpID = account.ID;
                entityclick.OpenID = opid;
                entityclick.EventKey = requestMessage.EventKey;
                entityclick.EventType = "点击";
                entityclick.MsgID = requestMessage.MsgId.ToString();
                entityclick.CreateDate = System.DateTime.Now;
                entities.Set<MpEventClickViewLog>().Add(entityclick);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录click事件出错：原因{1}", account.ID, ex.Message));
            }
            #endregion

            #region 推送消息
            if (rs == null || rstype == null)
            {
                var entity = entities.Set<MpMenu>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.MenuKey == requestMessage.EventKey).FirstOrDefault();
                if (entity != null)
                {
                    CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}_Type", account.AppID, requestMessage.EventKey), entity.MediaType, cachesecond);
                    if (entity.MediaType == MpMessageType.image.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageImage>();
                        responseMessage.Image.MediaId = entity.ImageMediaID;
                        CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.MediaType == MpMessageType.text.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                        responseMessage.Content = entity.Content;
                        CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.MediaType == MpMessageType.voice.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVoice>();
                        responseMessage.Voice.MediaId = entity.VoiceMediaID;
                        CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.MediaType == MpMessageType.video.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVideo>();
                        var video = entities.Set<MpMediaVideo>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.ID == entity.VideoID).FirstOrDefault();
                        if (video == null)
                            return null;
                        responseMessage.Video.MediaId = video.MediaID;
                        responseMessage.Video.Title = video.Title;
                        responseMessage.Video.Description = video.Description;
                        CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.MediaType == MpMessageType.mpnews.ToString())
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
                        CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey), responseMessage, cachesecond);
                        //LogWriter.Info(string.Format("EventClickRequest_{0}_{1}查询转换结束{2}", account.AppID, requestMessage.EventKey, new JavaScriptSerializer().Serialize(responseMessage)));
                        return responseMessage;
                    }
                    else if (entity.MediaType == MpMessageType.mpmultinews.ToString())
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
                        CacheHelper.Set(string.Format("EventClickRequest_{0}_{1}", account.AppID, requestMessage.EventKey), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else
                    {
                        return null;
                    }
                }
                //其他回复
                else
                {
                    return null;
                }
            }
            else
            {
                var rstp = rstype.ToString();
                if (rstp == MpMessageType.image.ToString())
                    return rs as ResponseMessageImage;
                else if (rstp == MpMessageType.mpmultinews.ToString())
                    return rs as ResponseMessageNews;
                else if (rstp == MpMessageType.mpnews.ToString())
                {
                    //var jss = new JavaScriptSerializer();
                    //LogWriter.Info(string.Format("EventClickRequest_{0}_{1}开始转换{2}", account.AppID, requestMessage.EventKey, jss.Serialize(rs)));
                    var result = rs as ResponseMessageNews;
                    //if (result == null)
                    //{
                    //    LogWriter.Info(string.Format("EventClickRequest_{0}_{1}转ResponseMessageNews为null", account.AppID, requestMessage.EventKey));
                    //}
                    //else
                    //{
                    //    LogWriter.Info(string.Format("EventClickRequest_{0}_{1}转换结束{2}", account.AppID, requestMessage.EventKey, jss.Serialize(result)));
                    //}
                    return result;
                }
                else if (rstp == MpMessageType.text.ToString())
                    return rs as ResponseMessageText;
                else if (rstp == MpMessageType.video.ToString())
                    return rs as ResponseMessageVideo;
                else if (rstp == MpMessageType.voice.ToString())
                    return rs as ResponseMessageVoice;
                return rs as IResponseMessageBase;
            }
            #endregion
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            return null;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            return null;
        }

        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            try
            {
                var opid = requestMessage.FromUserName;
                var entityevent = new MpEventScanLog();
                entityevent.ID = Formula.FormulaHelper.CreateGuid();
                entityevent.MpID = account.ID;
                entityevent.OpenID = opid;
                entityevent.EventContent = requestMessage.EventKey;
                entityevent.EventType = "已关注";
                entityevent.MsgID = requestMessage.MsgId.ToString();
                entityevent.CreateDate = System.DateTime.Now;
                entities.Set<MpEventScanLog>().Add(entityevent);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录扫码事件出错：原因{1}", account.ID, ex.Message));
            }
            return null;
        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            try
            {
                var opid = requestMessage.FromUserName;
                var entityclick = new MpEventClickViewLog();
                entityclick.ID = Formula.FormulaHelper.CreateGuid();
                entityclick.MpID = account.ID;
                entityclick.OpenID = opid;
                entityclick.EventKey = requestMessage.EventKey;
                entityclick.EventType = "查看";
                entityclick.MsgID = requestMessage.MsgId.ToString();
                entityclick.CreateDate = System.DateTime.Now;
                entities.Set<MpEventClickViewLog>().Add(entityclick);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("MPID{0}记录click事件出错：原因{1}", account.ID, ex.Message));
            }
            return null;

        }

        public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var msgid=requestMessage.MsgID.ToString();
            var entity = entities.Set<MpMessage>().Where(c => c.MpID == account.ID && c.WxMsgID == msgid).FirstOrDefault();
            if (entity != null)
            {
                entity.State = requestMessage.Status;
                entity.SendCount = requestMessage.FilterCount;
                entity.SuccessCount = requestMessage.SentCount;
                entity.FailCount = requestMessage.ErrorCount;
                entities.SaveChanges();
            }
            return null;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var rs = CacheHelper.Get(string.Format("SubscribeRequest_{0}", account.AppID));
            var rstype = CacheHelper.Get(string.Format("SubscribeRequest_{0}_Type", account.AppID));
            var opid = requestMessage.FromUserName;
            #region 记录日志
            try
            {
                var entityevent = new MpEventScanLog();
                entityevent.ID=Formula.FormulaHelper.CreateGuid();
                entityevent.MpID=account.ID;
                entityevent.OpenID=opid;
                entityevent.EventContent = requestMessage.EventKey.StartsWith("qrscene_") ? requestMessage.EventKey.Substring(8) : requestMessage.EventKey; ;
                entityevent.EventType="未关注";
                entityevent.MsgID = requestMessage.MsgId.ToString();
                entityevent.CreateDate=System.DateTime.Now;
                entities.Set<MpEventScanLog>().Add(entityevent);
                entities.SaveChanges();
            }
            catch(Exception ex)
            {
                LogWriter.Info(string.Format( "MPID{0}记录扫码事件出错：原因{1}",account.ID,ex.Message));
            }
            #endregion

            #region 更新粉丝
            WxFO wxfo = new WxFO();
            try
            {
                UserInfoJson wxinfo = null;
                try
                {
                    wxinfo = UserApi.Info(wxfo.GetAccessToken(account.ID), opid);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("获取MpID为{0}，openid为{1}的用户信息报错", account.ID, opid));
                    wxinfo = UserApi.Info(wxfo.GetAccessToken(account.ID, true), opid);
                }
                if (wxinfo.errcode != ReturnCode.请求成功)
                    throw new Exception(string.Format("获取MpID为{0}，openid为{1}的用户信息报错，错误编号:{2}，错误消息:{3}", account.ID, opid, wxinfo.errcode, wxinfo.errmsg));

                var group = entities.Set<MpGroup>().Where(c => c.MpID == account.ID && c.WxGroupID == wxinfo.groupid).FirstOrDefault();
                var entityfans = entities.Set<MpFans>().Where(c => c.MpID == account.ID && c.OpenID == opid).FirstOrDefault();

                #region 保存分组
                if (group == null)
                {
                    var rootgroup = entities.Set<MpGroup>().Where(c => c.MpID == account.ID && c.Length == 1).FirstOrDefault();
                    if (rootgroup == null)
                    {
                        rootgroup = new MpGroup();
                        rootgroup.ID = Formula.FormulaHelper.CreateGuid();
                        rootgroup.MpID = account.ID;
                        rootgroup.Name = "全部";
                        rootgroup.ParentID = "-1";
                        rootgroup.FullPath = rootgroup.ID;
                        rootgroup.Length = 1;
                        rootgroup.ChildCount = 0;
                        entities.Set<MpGroup>().Add(rootgroup);
                    }
                    var g = new MpGroup();
                    g.ID = Formula.FormulaHelper.CreateGuid();
                    g.MpID = account.ID;
                    g.ParentID = rootgroup.ID;
                    g.FullPath = string.Format("{0}.{1}", rootgroup.ID, g.ID);
                    g.Length = 2;
                    g.ChildCount = 0;
                    g.WxGroupID = wxinfo.groupid;
                    
                    GroupsJson groups = null;
                    try
                    {
                        groups = GroupsApi.Get(wxfo.GetAccessToken(account.ID));
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Error(ex, string.Format("获取MpID为{0}的分组报错", account.ID));
                        groups = GroupsApi.Get(wxfo.GetAccessToken(account.ID, true));
                    }
                    if (groups.errcode != ReturnCode.请求成功)
                        throw new Exception(string.Format("获取MpID为{0}的分组报错，错误编号:{1}，错误消息:{2}", account.ID, groups.errcode, groups.errmsg));

                    var wg = groups.groups.Where(c => c.id == wxinfo.groupid).FirstOrDefault();
                    if (wg != null)
                    {
                        g.Name = wg.name;
                        g.FansCount = wg.count;
                    }
                    entities.Set<MpGroup>().Add(g);
                }
                #endregion

                #region 保存粉丝
                if (entityfans == null)
                {
                    entityfans = new MpFans();
                    entityfans.ID = Formula.FormulaHelper.CreateGuid();
                    entityfans.City = wxinfo.city;
                    entityfans.Country = wxinfo.country;
                    entityfans.HeadImgUrl = wxinfo.headimgurl;
                    entityfans.IsFans = "1";
                    entityfans.Language = wxinfo.language;
                    entityfans.MpID = account.ID;
                    entityfans.NickName = wxinfo.nickname;
                    entityfans.OpenID = wxinfo.openid;
                    entityfans.Province = wxinfo.province;
                    entityfans.Remark = wxinfo.remark;
                    entityfans.Sex = wxinfo.sex.ToString();
                    entityfans.SubscribeTime = DateTimeHelper.GetDateTimeFromXml(wxinfo.subscribe_time);
                    entityfans.UniionID = wxinfo.unionid;
                    entityfans.WxGroupID = wxinfo.groupid;
                    entityfans.GroupID = group.ID;
                    entityfans.UpdateTime = DateTime.Now;
                    entities.Set<MpFans>().Add(entityfans);
                }
                else
                {
                    entityfans.City = wxinfo.city;
                    entityfans.Country = wxinfo.country;
                    entityfans.HeadImgUrl = wxinfo.headimgurl;
                    entityfans.IsFans = "1";
                    entityfans.Language = wxinfo.language;
                    entityfans.MpID = account.ID;
                    entityfans.NickName = wxinfo.nickname;
                    entityfans.OpenID = wxinfo.openid;
                    entityfans.Province = wxinfo.province;
                    entityfans.Remark = wxinfo.remark;
                    entityfans.Sex = wxinfo.sex.ToString();
                    entityfans.SubscribeTime = DateTimeHelper.GetDateTimeFromXml(wxinfo.subscribe_time);
                    entityfans.UniionID = wxinfo.unionid;
                    entityfans.WxGroupID = wxinfo.groupid;
                    entityfans.GroupID = group.ID;
                    entityfans.UpdateTime = DateTime.Now;
                }
                #endregion

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.Error(string.Format("粉丝订阅更新数据库失败，原因：{0}", ex.Message));
            }
            #endregion

            #region 推送消息
            if (rs == null || rstype == null)
            {
                var eventtype = MpEventType.Subscribe.ToString();
                var entity = entities.Set<MpEvent>().Where(c => c.MpID == account.ID && c.IsDelete == 0 && c.EventType == eventtype).FirstOrDefault();
                if (entity != null)
                {
                    CacheHelper.Set(string.Format("SubscribeRequest_{0}_Type", account.AppID), entity.ReplyType, cachesecond);
                    if (entity.ReplyType == MpMessageType.none.ToString())
                    {
                        return null;
                    }
                    else if (entity.ReplyType == MpMessageType.image.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageImage>();
                        responseMessage.Image.MediaId = entity.ImageMediaID;
                        CacheHelper.Set(string.Format("SubscribeRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.text.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                        responseMessage.Content = entity.Content;
                        CacheHelper.Set(string.Format("SubscribeRequest_{0}", account.AppID), responseMessage, cachesecond);
                        return responseMessage;
                    }
                    else if (entity.ReplyType == MpMessageType.voice.ToString())
                    {
                        var responseMessage = base.CreateResponseMessage<ResponseMessageVoice>();
                        responseMessage.Voice.MediaId = entity.VoiceMediaID;
                        CacheHelper.Set(string.Format("SubscribeRequest_{0}", account.AppID), responseMessage, cachesecond);
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
                        CacheHelper.Set(string.Format("SubscribeRequest_{0}", account.AppID), responseMessage, cachesecond);
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
                        CacheHelper.Set(string.Format("SubscribeRequest_{0}", account.AppID), responseMessage, cachesecond);
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
                        CacheHelper.Set(string.Format("SubscribeRequest_{0}", account.AppID), responseMessage, cachesecond);
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
            #endregion
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var opid = requestMessage.FromUserName;
            var entity = entities.Set<MpFans>().Where(c => c.MpID == account.ID && c.OpenID == opid).FirstOrDefault();
            if (entity != null)
            {
                entity.IsFans = "0";
                entity.UpdateTime = DateTime.Now;
                entities.SaveChanges();
            }
            return null;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            return null;
        }
    }
}
