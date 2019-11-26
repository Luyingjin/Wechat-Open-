using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin;
using WeChat.Logic.Domain;
using WeChat.Logic.BusinessFacade;
using Formula;
using Formula.Helper;

namespace WeChat.Controllers
{
    public class WxApiController : BaseController
    {
        public string domain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
        public string cachetime = System.Configuration.ConfigurationManager.AppSettings["OAuth2WhiteListCacheTime"];
        public ActionResult OAuth2Base(string mpid)
        {
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }
            var reurl = Request.QueryString["reurl"];
            if (string.IsNullOrEmpty(reurl))
            {
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：reurl为空", mpid));
                return Content("reurl为空");
            }
            if (account.MpOAuth2WhiteList.Where(c => c.Domain == "*").Count() == 0)
            {
                Uri ru = null;
                try
                {
                    ru = new Uri(reurl);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("mpid为“{0}”的静默授权失败，原因：解析reurl“{1}”错误", mpid, reurl));
                    return Content("reurl不正确");
                }
                var redomain = ru.Authority.ToLower();
                if (account.MpOAuth2WhiteList.Where(c => c.Domain == redomain).Count() == 0)
                {
                    LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：域名{1}不在白名单中", mpid, redomain));
                    return Content("您的域名未授权调用该接口");
                }
            }
            string appId = account.AppID;
            string secret = account.AppSecret;
            var constr = Request.QueryString["constr"] ?? "";
            var urlmode = Request.QueryString["urlmode"] ?? "";
            var url = OAuthApi.GetAuthorizeUrl(appId, "http://" + domain + "/wechatservice/wxapi/OAuth2BaseCallback?mpid=" + mpid + "&reurl=" + reurl + "&constr=" + constr + "&urlmode=" + urlmode, "JeffreySu", OAuthScope.snsapi_base);
            return Redirect(url);
        }

        public ActionResult OAuth2UserInfo(string mpid)
        {
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }
            var reurl = Request.QueryString["reurl"];
            if (string.IsNullOrEmpty(reurl))
            {
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：reurl为空", mpid));
                return Content("reurl为空");
            }
            if (account.MpOAuth2WhiteList.Where(c => c.Domain == "*").Count() == 0)
            {
                Uri ru = null;
                try
                {
                    ru = new Uri(reurl);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("mpid为“{0}”的静默授权失败，原因：解析reurl“{1}”错误", mpid, reurl));
                    return Content("reurl不正确");
                }
                var redomain = ru.Authority.ToLower();
                if (account.MpOAuth2WhiteList.Where(c => c.Domain == redomain).Count() == 0)
                {
                    LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：域名{1}不在白名单中", mpid, redomain));
                    return Content("您的域名未授权调用该接口");
                }
            }
            string appId = account.AppID;
            string secret = account.AppSecret;
            var constr = Request.QueryString["constr"] ?? "";
            var urlmode = Request.QueryString["urlmode"] ?? "";
            var url = OAuthApi.GetAuthorizeUrl(appId, "http://" + domain + "/wechatservice/wxapi/OAuth2UserInfoCallback?mpid=" + mpid + "&reurl=" + reurl + "&constr=" + constr + "&urlmode=" + urlmode, "JeffreySu", OAuthScope.snsapi_userinfo);
            return Redirect(url);
        }

        public ActionResult OAuth2BaseCallback(string mpid)
        {
            var constr = string.IsNullOrEmpty(Request.QueryString["constr"]) ? "@" : Request.QueryString["constr"];
            string url = (Request.QueryString["reurl"] ?? "").Replace(constr, "&");
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];
            string urlmode = Request.QueryString["urlmode"] ?? "";
            if (urlmode.ToLower().Trim() == "base64")
            {
                url = Base64Helper.DecodeBase64(url.Replace(" ", "+"));
            }

            if (string.IsNullOrEmpty(code))
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：拒绝了授权，Url：{1}", mpid, Request.Url.ToString()));
                return Content("您拒绝了授权！");
            }

            if (state != "JeffreySu" && state != "JeffreySu?10000skip=true")
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：验证失败，Url：{1}", mpid, Request.Url.ToString()));
                return Content("验证失败！请从正规途径进入！");
            }

            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }

            //通过，用code换取access_token
            OAuthAccessTokenResult result = null;
            try
            {
                var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
                result = OAuthApi.GetAccessToken(account.AppID, account.AppSecret, code);
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权在通过code获取token时异常，原因：{1}", mpid, result.errmsg));
                return Content("错误：" + ex.Message);
            }

            if (result.errcode != ReturnCode.请求成功)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权在通过code获取token时失败，原因：{1}", mpid, result.errmsg));
                return Content("错误：" + result.errmsg);
            }

            //因为这里还不确定用户是否关注本微信，所以只能试探性地获取一下
            OAuthUserInfo userInfo = null;
            try
            {
                //已关注，可以得到详细信息
                userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                url = string.Format("{0}{1}openid={2}&nickname={3}&headimgurl={4}"
                    , url, url.Contains('?') ? "&" : "?", result.openid, userInfo.nickname, userInfo.headimgurl);
                return Redirect(url);
            }
            catch
            {
                //未关注，只能授权，无法得到详细信息
                //这里的 ex.JsonResult 可能为："{\"errcode\":40003,\"errmsg\":\"invalid openid\"}"

                url = string.Format("{0}{1}openid={2}"
                    , url, url.Contains('?') ? "&" : "?", result.openid);
                return Redirect(url);
            }
        }

        public ActionResult OAuth2UserInfoCallback(string mpid)
        {
            var constr = string.IsNullOrEmpty(Request.QueryString["constr"]) ? "@" : Request.QueryString["constr"];
            string url = (Request.QueryString["reurl"] ?? "").Replace(constr, "&");
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];
            string urlmode = Request.QueryString["urlmode"] ?? "";
            if (urlmode.ToLower().Trim() == "base64")
            {
                url = Base64Helper.DecodeBase64(url.Replace(" ", "+"));
            }

            if (string.IsNullOrEmpty(code))
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：拒绝了授权，Url：{1}", mpid, Request.Url.ToString()));
                return Content("您拒绝了授权！");
            }

            if (state != "JeffreySu" && state != "JeffreySu?10000skip=true")
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：验证失败，Url：{1}", mpid, Request.Url.ToString()));
                return Content("验证失败！请从正规途径进入！");
            }

            //通过，用code换取access_token
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }

            OAuthAccessTokenResult result = null;
            try
            {
                result = OAuthApi.GetAccessToken(account.AppID, account.AppSecret, code);
            }
            catch (Exception ex)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权在通过code获取token时异常，原因：{1}", mpid, result.errmsg));
                return Content("错误：" + ex.Message);
            }

            if (result.errcode != ReturnCode.请求成功)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权在通过code获取token时失败，原因：{1}", mpid, result.errmsg));
                return Content("错误：" + result.errmsg);
            }

            //因为这里还不确定用户是否关注本微信，所以只能试探性地获取一下
            OAuthUserInfo userInfo = null;
            try
            {
                //已关注，可以得到详细信息
                userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                url = string.Format("{0}{1}openid={2}&nickname={3}&headimgurl={4}"
                    , url, url.Contains('?') ? "&" : "?", result.openid, userInfo.nickname, userInfo.headimgurl);
                return Redirect(url);
            }
            catch (ErrorJsonResultException ex)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的认证授权失败，原因：{1}", mpid, ex.Message));
                return Content("错误：" + ex.Message);
            }
        }

        public ActionResult Jssdk(string mpid)
        {
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的jssdk获取失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }

            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();

            var ticket = wxFO.GetJsApiTicket(mpid);
            var url = Request.Params["callurl"] ?? "";
            if (string.IsNullOrEmpty(url))
                url = Request.UrlReferrer == null ? Request.Url.ToString() : Request.UrlReferrer.ToString();
            else
            {
                var constr = string.IsNullOrEmpty(Request.Params["constr"]) ? "@" : Request.Params["constr"];
                url = url.Replace(constr, "&");

                var urlmode = Request.QueryString["urlmode"] ?? "";
                if (urlmode.ToLower().Trim() == "base64")
                {
                    url = Base64Helper.DecodeBase64(url.Replace(" ", "+"));
                }
            }
            string timestamp = Convert.ToString(ConvertDateTimeInt(DateTime.Now));
            string nonceStr = createNonceStr();
            string rawstring = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url;
            string signature = SHA1_Hash(rawstring);

            return Json(new
            {
                appId = account.AppID,
                nonceStr = nonceStr,
                timestamp = timestamp,
                url = url,
                signature = signature,
                rawString = rawstring,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JssdkJsonP(string mpid)
        {
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的jssdk获取失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }

            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();

            var ticket = wxFO.GetJsApiTicket(mpid);
            var url = Request.QueryString["callurl"] ?? "";
            if (string.IsNullOrEmpty(url))
                url = Request.UrlReferrer == null ? Request.Url.ToString() : Request.UrlReferrer.ToString();
            else
            {
                var constr = string.IsNullOrEmpty(Request.QueryString["constr"]) ? "@" : Request.QueryString["constr"];
                url = url.Replace(constr, "&");

                var urlmode = Request.QueryString["urlmode"] ?? "";
                if (urlmode.ToLower().Trim() == "base64")
                {
                    url = Base64Helper.DecodeBase64(url.Replace(" ", "+"));
                }
            }
            string timestamp = Convert.ToString(ConvertDateTimeInt(DateTime.Now));
            string nonceStr = createNonceStr();
            string rawstring = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url;
            string signature = SHA1_Hash(rawstring);

            var callback = Request.QueryString["callback"] ?? "";
            return Content(string.IsNullOrEmpty(callback) ? "" : string.Format("{0}({1})", callback, new JavaScriptSerializer().Serialize(
                new
                {
                    appId = account.AppID,
                    nonceStr = nonceStr,
                    timestamp = timestamp,
                    url = url,
                    signature = signature,
                    rawString = rawstring,
                })));
        }

        #region jssdk私有函数
        private string createNonceStr()
        {
            int length = 16;
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "";
            Random rad = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }

        /// <summary> 
        /// 将c# DateTime时间格式转换为Unix时间戳格式 
        /// </summary> 
        /// <param name="time">时间</param> 
        /// <returns>double</returns> 
        public int ConvertDateTimeInt(System.DateTime time)
        {
            int intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = Convert.ToInt32((time - startTime).TotalSeconds);
            return intResult;
        }

        //SHA1哈希加密算法 
        public string SHA1_Hash(string str_sha1_in)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = System.Text.UTF8Encoding.Default.GetBytes(str_sha1_in);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            str_sha1_out = str_sha1_out.Replace("-", "").ToLower();
            return str_sha1_out;
        }
        #endregion

        protected MpAccount GetAccount(string MpID)
        {
            var account = CacheHelper.Get(string.Format("WxAccount{0}", MpID)) as MpAccount;
            if (account == null)
            {
                account = entities.Set<MpAccount>().Include("MpOAuth2WhiteList").Where(c => c.ID == MpID && c.IsDelete == 0).FirstOrDefault();
                if (account != null)
                {
                    int ct = 300;
                    int.TryParse(cachetime, out ct);
                    CacheHelper.Set(string.Format("WxAccount{0}", MpID), account, ct);
                }
            }
            if (account != null)
                return account;
            else
                return null;
        }
    }
}
