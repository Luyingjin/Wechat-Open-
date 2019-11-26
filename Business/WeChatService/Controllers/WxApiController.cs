using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin;
using WeChat.Logic.Domain;
using WeChat.Logic.BusinessFacade;
using Formula;
using Formula.Helper;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Collections.Specialized;
using Senparc.Weixin.MP.TenPayLibV3;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace WeChat.Controllers
{
    public class WxApiController : BaseController
    {
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
            var urlmode = Request.QueryString["urlmode"] ?? "";
            if (account.MpOAuth2WhiteList.Where(c => c.Domain == "*").Count() == 0)
            {
                Uri ru = null;
                try
                {
                    ru = new Uri(urlmode.ToLower().Trim() == "base64" ? Base64Helper.DecodeBase64(reurl.Replace(" ", "+")) : reurl);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("mpid为“{0}”的静默授权失败，原因：解析reurl“{1}”错误", mpid, reurl));
                    return Content("reurl不正确");
                }
                var redomain=ru.Authority.ToLower();
                if (account.MpOAuth2WhiteList.Where(c => c.Domain == redomain).Count() == 0)
                {
                    LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：域名{1}不在白名单中", mpid, redomain));
                    return Content("您的域名未授权调用该接口");
                }
            }
            string appId = account.AppID;
            string secret = account.AppSecret;
            var constr = Request.QueryString["constr"] ?? "";
            var url = OAuthApi.GetAuthorizeUrl(appId, $"{Request.Url.Scheme}://{Request.Url.Host}/wechatservice/wxapi/OAuth2BaseCallback?mpid=" + mpid + "&reurl=" + reurl + "&constr=" + constr + "&urlmode=" + urlmode, "JeffreySu", OAuthScope.snsapi_base);
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
            var urlmode = Request.QueryString["urlmode"] ?? "";
            if (account.MpOAuth2WhiteList.Where(c => c.Domain == "*").Count() == 0)
            {
                Uri ru = null;
                try
                {
                    ru = new Uri(urlmode.ToLower().Trim() == "base64" ? Base64Helper.DecodeBase64(reurl.Replace(" ", "+")) : reurl);
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
            var url = OAuthApi.GetAuthorizeUrl(appId, $"{Request.Url.Scheme}://{Request.Url.Host}/wechatservice/wxapi/OAuth2UserInfoCallback?mpid=" + mpid + "&reurl=" + reurl + "&constr=" + constr + "&urlmode=" + urlmode, "JeffreySu", OAuthScope.snsapi_userinfo);
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
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：拒绝了授权，Url：{1}", mpid, Request.Url.ToString()));
                return Content("您拒绝了授权！");
            }

            if (state != "JeffreySu" && state != "JeffreySu?10000skip=true")
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                LogWriter.Info(string.Format("mpid为“{0}”的静默授权失败，原因：验证失败，Url：{1}", mpid, Request.Url.ToString()));
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

        public ActionResult GetUser(string mpid, string openid)
        {
            #region 校验公众号
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的GetUser获取失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }
            #endregion

            #region 获取用户信息
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();

            UserInfoJson userinfo = null;

            try
            {
                userinfo = wxFO.GetFans(mpid, openid);
            }
            catch
            {
                try
                {
                    userinfo = wxFO.GetFans(mpid, openid, true);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        errorcode = "500",
                        errormsg = ex.Message,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion

            return Json(new
            {
                errorcode = "200",
                errormsg = "",
                result = userinfo,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserJsonP(string mpid, string openid)
        {
            #region 校验公众号
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的GetUser获取失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }
            var callback = Request.QueryString["callback"] ?? "";
            #endregion

            #region 获取用户信息
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();

            UserInfoJson userinfo = null;

            try
            {
                userinfo = wxFO.GetFans(mpid, openid);
            }
            catch
            {
                try
                {
                    userinfo = wxFO.GetFans(mpid, openid, true);
                }
                catch (Exception ex)
                {
                    return Content(string.IsNullOrEmpty(callback) ? "" : string.Format("{0}({1})", callback, new JavaScriptSerializer().Serialize(
                    new
                    {
                        errorcode = "500",
                        errormsg = ex.Message,
                    })));
                }
            }
            #endregion

            return Content(string.IsNullOrEmpty(callback) ? "" : string.Format("{0}({1})", callback, new JavaScriptSerializer().Serialize(
                new
                {
                    errorcode = "200",
                    errormsg = "",
                    result = userinfo,
                })));
        }

        public ActionResult GetAccessToken(string mpid, int getnewtoken = 0)
        {
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的accesstoken获取失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            return Json(new
            {
                access_token = wxFO.GetAccessToken(mpid, getnewtoken == 1),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendRedPacket(string mpid, string token, string openid, string total, string activityname, string sendername, string sendmsg, string remark, string mchbillno = "", string sceneid = "")
        {
            #region 校验金额
            int rptotal = 0;
            if (!int.TryParse(total, out rptotal))
            {
                LogWriter.Info(string.Format("mpid为“{0}”的发送红包接口调用失败，原因：金额不正确", mpid));
                return Json(new
                {
                    errorcode = "202",
                    errormsg = "金额不正确",
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 校验公众号
            var account = GetAccount(mpid);
            if (account == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的发送红包接口调用失败，原因：公众号不存在", mpid));
                return Content("公众号不存在");
            }
            #endregion

            #region 校验活动
            var datenow = DateTime.Now;
            var activity = entities.Set<MpRedPacket>().Where(c => c.MpID == mpid && c.Token == token && c.IsDelete != 1).FirstOrDefault();
            if (activity == null)
            {
                LogWriter.Info(string.Format("mpid为“{0}”的发送红包接口调用失败，原因：活动不存在", mpid));
                return Json(new
                {
                    errorcode = "201",
                    errormsg = "活动不存在",
                }, JsonRequestBehavior.AllowGet);
            }
            if (activity.StartDate == null || activity.StartDate > DateTime.Now || activity.EndDate == null || activity.EndDate < DateTime.Now)
            {
                MpRedPacketLog mrpl = new MpRedPacketLog();
                mrpl.ID = FormulaHelper.CreateGuid();
                mrpl.MpID = mpid;
                mrpl.RPID = activity.ID;
                mrpl.Openid = openid;
                mrpl.Total = rptotal;
                mrpl.SendTime = datenow;
                mrpl.State = "0";
                mrpl.Msg = "活动未开始或已结束";
                entities.Set<MpRedPacketLog>().Add(mrpl);
                entities.SaveChanges();
                return Json(new
                {
                    errorcode = "203",
                    errormsg = "活动未开始或已结束",
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 发送红包
            if (string.IsNullOrEmpty(mchbillno))
                mchbillno = DateTime.Now.ToString("HHmmss") + TenPayV3Util.BuildRandomStr(28);
            else
            {
                //如果有传入
                var old = entities.Set<MpRedPacketLog>().FirstOrDefault(c => c.RPID == activity.ID && c.BillNO == mchbillno);
                if (old != null)
                {
                    return Json(new
                    {
                        errorcode = string.IsNullOrEmpty(old.Msg) ? "200" : "204",
                        errormsg = old.Msg,
                    }, JsonRequestBehavior.AllowGet);
                }
            }


            MpRedPacketLog mrpl2 = new MpRedPacketLog();
            mrpl2.ID = FormulaHelper.CreateGuid();
            mrpl2.MpID = mpid;
            mrpl2.RPID = activity.ID;
            mrpl2.Openid = openid;
            mrpl2.Total = rptotal;
            mrpl2.SendTime = datenow;
            mrpl2.BillNO = mchbillno;
            entities.Set<MpRedPacketLog>().Add(mrpl2);
            entities.SaveChanges();

            try
            {
                //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
                string cert = account.CertPhysicalPath;
                //私钥（在安装证书时设置）
                string password = account.CertPassword;

                string nonceStr = TenPayV3Util.GetNoncestr();
                RequestHandler packageReqHandler = new RequestHandler(null);

                //设置package订单参数
                packageReqHandler.SetParameter("nonce_str", nonceStr);              //随机字符串
                packageReqHandler.SetParameter("wxappid", account.AppID);         //公众账号ID
                packageReqHandler.SetParameter("mch_id", account.MchID);          //商户号
                packageReqHandler.SetParameter("mch_billno", mchbillno);                 //填入商家订单号
                packageReqHandler.SetParameter("send_name", sendername);                 //红包发送者名称
                packageReqHandler.SetParameter("re_openid", openid);
                packageReqHandler.SetParameter("total_amount", total);                //付款金额，单位分
                packageReqHandler.SetParameter("total_num", "1");               //红包发放总人数
                packageReqHandler.SetParameter("wishing", sendmsg);               //红包祝福语
                packageReqHandler.SetParameter("client_ip", System.Configuration.ConfigurationManager.AppSettings["ClientIP"]);               //调用接口的机器Ip地址
                packageReqHandler.SetParameter("act_name", activityname);   //活动名称
                packageReqHandler.SetParameter("remark", remark);   //备注信息
                if (!string.IsNullOrEmpty(sceneid))
                    packageReqHandler.SetParameter("scene_id", sceneid);   //场景
                string sign = packageReqHandler.CreateMd5Sign("key", account.WxPayAppSecret);
                packageReqHandler.SetParameter("sign", sign);                       //签名

                //发红包需要post的数据
                string data = packageReqHandler.ParseXML();

                //发红包接口地址
                string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //调用证书
                X509Certificate2 cer = new X509Certificate2(cert, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                #region 发起post请求
                HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webrequest.ClientCertificates.Add(cer);
                webrequest.Method = "post";

                byte[] postdatabyte = Encoding.UTF8.GetBytes(data);
                webrequest.ContentLength = postdatabyte.Length;
                Stream stream;
                stream = webrequest.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                HttpWebResponse httpWebResponse = (HttpWebResponse)webrequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();
                XDocument doc1 = XDocument.Parse(responseContent);
                var result_code = doc1.Root.Element("result_code");

                mrpl2.State = result_code.Value == "SUCCESS" ? "1" : "0";
                mrpl2.Msg = mrpl2.State == "1" ? "" : doc1.Root.Element("return_msg").Value;
                entities.SaveChanges();

                return Json(new
                {
                    errorcode = mrpl2.State == "1" ? "200" : "204",
                    errormsg = mrpl2.Msg,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                LogWriter.Error(ex, $"发送红包失败 openid {openid} 金额 {total}");

                mrpl2.State = "0";
                mrpl2.Msg = ex.Message;
                entities.SaveChanges();

                return Json(new
                {
                    errorcode = mrpl2.State == "1" ? "200" : "204",
                    errormsg = mrpl2.Msg,
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #endregion
        }
        public ActionResult GetWxPayJsApi(string mpid, string openid, string productname, int total,string orderno)
        {
            try
            {

                #region 校验公众号
                var account = GetAccount(mpid);
                if (account == null)
                {
                    LogWriter.Info(string.Format("mpid为“{0}”的发送红包接口调用失败，原因：公众号不存在", mpid));
                    return Content("公众号不存在");
                }
                #endregion

                string timeStamp = "";
                string nonceStr = "";
                string paySign = "";

                //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                orderno = string.IsNullOrEmpty(orderno) ? (DateTime.Now.ToString("yyyyMMddHHmmss") + TenPayV3Util.BuildRandomStr(3)) : orderno;
                
                //创建支付应答对象
                RequestHandler packageReqHandler = new RequestHandler(null);
                //初始化
                packageReqHandler.Init();

                timeStamp = TenPayV3Util.GetTimestamp();
                nonceStr = TenPayV3Util.GetNoncestr();

                //设置package订单参数
                packageReqHandler.SetParameter("appid", account.AppID);       //公众账号ID
                packageReqHandler.SetParameter("mch_id", account.MchID);          //商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr);                    //随机字符串
                packageReqHandler.SetParameter("body", productname);    //商品信息
                packageReqHandler.SetParameter("out_trade_no", orderno);      //商家订单号
                packageReqHandler.SetParameter("total_fee", total.ToString());                  //商品金额,以分为单位(money * 100).ToString()
                packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);   //用户的公网ip，不是商户服务器IP
                packageReqHandler.SetParameter("notify_url", $"{Request.Url.Scheme}://{Request.Url.Host}/wechatservice/wxapi/PayNotifyUrl");            //接收财付通通知的URL
                packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString());                        //交易类型
                packageReqHandler.SetParameter("openid", openid);                       //用户的openId
                packageReqHandler.SetParameter("attach", mpid);                       //用户的openId

                string sign = packageReqHandler.CreateMd5Sign("key", account.WxPayAppSecret);
                packageReqHandler.SetParameter("sign", sign);                       //签名

                string data = packageReqHandler.ParseXML();

                var result = TenPayV3.Unifiedorder(data);
                var res = XDocument.Parse(result);
                string prepayId = res.Element("xml").Element("prepay_id").Value;

                //设置支付参数
                RequestHandler paySignReqHandler = new RequestHandler(null);
                paySignReqHandler.SetParameter("appId", account.AppID);
                paySignReqHandler.SetParameter("timeStamp", timeStamp);
                paySignReqHandler.SetParameter("nonceStr", nonceStr);
                paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
                paySignReqHandler.SetParameter("signType", "MD5");
                paySign = paySignReqHandler.CreateMd5Sign("key", account.WxPayAppSecret);

                ViewData["appId"] = account.AppID;
                ViewData["timeStamp"] = timeStamp;
                ViewData["nonceStr"] = nonceStr;
                ViewData["package"] = string.Format("prepay_id={0}", prepayId);
                ViewData["paySign"] = paySign;
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        appId = account.AppID,
                        timeStamp = timeStamp,
                        nonceStr = nonceStr,
                        package = string.Format("prepay_id={0}", prepayId),
                        paySign = paySign,
                        orderNO= orderno
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.Info($"openid:'{openid}',total:{total},productname:'{productname}',ex:'{ex.Message}'");
                return Json(new
                {
                    success = false,
                    msg = ex.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PayNotifyUrl()
        {
            var returntmpl = @"<xml>
   <return_code><![CDATA[{0}]]></return_code>
   <return_msg><![CDATA[{1}]]></return_msg>
</xml>";
            try
            {
                ResponseHandler resHandler = new ResponseHandler(null);
                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");
                if (return_code == "SUCCESS")
                {
                    string mpid = resHandler.GetParameter("attach");
                    string openid = resHandler.GetParameter("openid");
                    string out_trade_no = resHandler.GetParameter("out_trade_no");
                    string total_fee = resHandler.GetParameter("total_fee");

                    #region 校验公众号
                    var account = GetAccount(mpid);
                    if (account == null)
                    {
                        LogWriter.Info(string.Format("mpid为“{0}”的收款回调验证失败，原因：公众号不存在", mpid));
                        return Content(string.Format(returntmpl, "FAIL", "Validate Error"), "text/xml");
                    }
                    #endregion

                    resHandler.SetKey(account.WxPayAppSecret);
                    //验证请求是否从微信发过来（安全）
                    if (resHandler.IsTenpaySign())
                    {
                        return Content(string.Format(returntmpl, return_code, return_msg), "text/xml");
                    }
                }
                return Content(string.Format(returntmpl, "FAIL", "Validate Error"), "text/xml");
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex,"微信支付回调验证失败");
                return Content(string.Format(returntmpl, "FAIL", "System Error"), "text/xml");
            }
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

        /// <summary>  
        /// 获取IP地址  
        /// </summary>  
        public string IPAddress
        {
            get
            {
                string userIP;
                // 如果使用代理，获取真实IP  
                if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                    userIP = Request.ServerVariables["REMOTE_ADDR"];
                else
                    userIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (userIP == null || userIP == "")
                    userIP = Request.UserHostAddress;
                return userIP;
            }
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

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
