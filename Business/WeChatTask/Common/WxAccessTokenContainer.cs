using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.CommonAPIs;

namespace WeChatTask.Common
{
    public class AccessTokenBag
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public DateTime ExpireTime { get; set; }
        public AccessTokenResult AccessTokenResult { get; set; }
        /// <summary>
        /// 只针对这个AppId的锁
        /// </summary>
        public object Lock = new object();
    }

    /// <summary>
    /// 通用接口AccessToken容器，用于自动管理AccessToken，如果过期会重新获取
    /// </summary>
    public class WxAccessTokenContainer
    {
        static Dictionary<string, AccessTokenBag> AccessTokenCollection =
           new Dictionary<string, AccessTokenBag>(StringComparer.OrdinalIgnoreCase);

        public static Dictionary<string, AccessTokenBag> AccessTokens
        {
            get
            {
                return AccessTokenCollection;
            }
        }

        /// <summary>
        /// 注册应用凭证信息，此操作只是注册，不会马上获取Token，并将清空之前的Token，
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        public static void Register(string appId, string appSecret)
        {
            AccessTokenCollection[appId] = new AccessTokenBag()
            {
                AppId = appId,
                AppSecret = appSecret,
                ExpireTime = DateTime.MinValue,
                AccessTokenResult = new AccessTokenResult()
            };
        }

        /// <summary>
        /// 使用完整的应用凭证获取Token，如果不存在将自动注册
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string TryGetToken(string appId, string appSecret, bool getNewToken = false)
        {
            if (!CheckRegistered(appId))
            {
                Register(appId, appSecret);
            }
            return GetToken(appId);
        }

        /// <summary>
        /// 使用完整的应用凭证获取Token，如果不存在将自动注册
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static AccessTokenResult TryGetTokenResult(string appId, string appSecret, bool getNewToken = false)
        {
            if (!CheckRegistered(appId))
            {
                Register(appId, appSecret);
            }
            return GetTokenResult(appId, getNewToken);
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static string GetToken(string appId, bool getNewToken = false)
        {
            return GetTokenResult(appId, getNewToken).access_token;
        }

        /// <summary>
        /// 获取可用Token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken">是否强制重新获取新的Token</param>
        /// <returns></returns>
        public static AccessTokenResult GetTokenResult(string appId, bool getNewToken = false)
        {
            if (!AccessTokenCollection.ContainsKey(appId))
            {
                throw new WeixinException("此appId尚未注册，请先使用AccessTokenContainer.Register完成注册（全局执行一次即可）！");
            }

            var accessTokenBag = AccessTokenCollection[appId];
            lock (accessTokenBag.Lock)
            {
                if (getNewToken || accessTokenBag.ExpireTime <= DateTime.Now)
                {
                    var oldtoken = accessTokenBag.AccessTokenResult;
                    var oldtime = new DateTime(accessTokenBag.ExpireTime.Ticks);
                    //已过期，重新获取
                    accessTokenBag.AccessTokenResult = CommonApi.GetToken(accessTokenBag.AppId, accessTokenBag.AppSecret);
                    accessTokenBag.ExpireTime = DateTime.Now.AddSeconds(accessTokenBag.AccessTokenResult.expires_in);
                    LogWriter.Info(string.Format(@"AppID:{0}
OldToken:{1}
OldExpireTime:{2}
NewToken:{3}
NewExpireTime:{4}
GetNewToken:{5}", appId, oldtoken.access_token, oldtime, accessTokenBag.AccessTokenResult.access_token, accessTokenBag.ExpireTime, getNewToken));
                }
            }
            return accessTokenBag.AccessTokenResult;
        }

        /// <summary>
        /// 检查是否已经注册
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool CheckRegistered(string appId)
        {
            return AccessTokenCollection.ContainsKey(appId);
        }
    }
}
