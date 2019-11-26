using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.Weixin.MP.Entities;

namespace WeChatTask.Common
{
    public class WxApi
    {
        static Dictionary<string, AccessTokenBag> tokens = new Dictionary<string, AccessTokenBag>();
        static object tokenlock = new object();

        /// <summary>
        /// 获取公众号认证令牌
        /// </summary>
        /// <param name="MpID">公众号MpID</param>
        /// <param name="GetNewToken">是否强制获取最新</param>
        /// <returns></returns>
        public static string GetAccessToken(string MpID, bool GetNewToken = false)
        {
            #region 不强制获取最新
            if (!GetNewToken)
            {
                #region 缓存中没有token
                if (!tokens.ContainsKey(MpID))
                {
                    //判断公众号是否存在
                    var mpaccount = StaticObjects.wechatdh.ExecuteTable(string.Format("select AppID,AppSecret,AccessToken,ExpireTime from MpAccount where ID='{0}'", MpID.Replace("'", "''")));
                    if (mpaccount.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("获取AccessToken错误，MpID为{0}的公众号不存在"));
                    }
                    tokens.Add(MpID, new AccessTokenBag());
                    var exprieTime = DbTool.ToDateTime(mpaccount.Rows[0]["ExpireTime"]);
                    var accesstoken = DbTool.ToString(mpaccount.Rows[0]["AccessToken"]);
                    var appid = DbTool.ToString(mpaccount.Rows[0]["AppID"]);
                    var appsecret = DbTool.ToString(mpaccount.Rows[0]["AppSecret"]);
                    //如果数据库中token不存在，或者过期，则强制获取最新token
                    if (exprieTime == DateTime.MaxValue || exprieTime <= DateTime.Now)
                    {
                        var token = WxAccessTokenContainer.TryGetTokenResult(appid, appsecret, true);
                        StaticObjects.wechatdh.ExecuteNonQuery(string.Format("update MpAccount set AccessToken='{0}',ExpireTime='{1}' where ID='{2}'", token.access_token, DateTime.Now.AddSeconds(token.expires_in), MpID));
                        tokens[MpID] = WxAccessTokenContainer.AccessTokens[appid];
                    }
                    //否则就将数据库中的token加入到缓存
                    else
                    {
                        tokens[MpID].AppId = appid;
                        tokens[MpID].AppSecret = appsecret;
                        tokens[MpID].ExpireTime = exprieTime;
                        tokens[MpID].AccessTokenResult = new AccessTokenResult()
                        {
                            access_token = accesstoken,
                            expires_in = (int)(DateTime.Now - exprieTime).TotalSeconds,
                        };
                    }
                }
                #endregion

                #region 缓存中有token
                else
                {
                    //如果缓存中的token过期，则强制获取最新token
                    if (tokens[MpID].ExpireTime <= DateTime.Now)
                    {
                        var token = WxAccessTokenContainer.TryGetTokenResult(tokens[MpID].AppId, tokens[MpID].AppSecret, true);
                        StaticObjects.wechatdh.ExecuteNonQuery(string.Format("update MpAccount set AccessToken='{0}',ExpireTime='{1}' where ID='{2}'", token.access_token, DateTime.Now.AddSeconds(token.expires_in), MpID));
                        tokens[MpID] = WxAccessTokenContainer.AccessTokens[tokens[MpID].AppId];
                    }
                }
                #endregion
            }
            #endregion

            #region 强制获取最新
            else
            {
                #region 缓存中没有token
                if (!tokens.ContainsKey(MpID))
                {
                    //判断公众号是否存在
                    var mpaccount = StaticObjects.wechatdh.ExecuteTable(string.Format("select AppID,AppSecret,AccessToken,ExpireTime from MpAccount where ID='{0}'", MpID.Replace("'", "''")));
                    if (mpaccount.Rows.Count == 0)
                    {
                        throw new Exception(string.Format("获取AccessToken错误，MpID为{0}的公众号不存在"));
                    }
                    tokens.Add(MpID, new AccessTokenBag());
                    var exprieTime = DbTool.ToDateTime(mpaccount.Rows[0]["ExpireTime"]);
                    var accesstoken = DbTool.ToString(mpaccount.Rows[0]["AccessToken"]);
                    var appid = DbTool.ToString(mpaccount.Rows[0]["AppID"]);
                    var appsecret = DbTool.ToString(mpaccount.Rows[0]["AppSecret"]);
                    var token = WxAccessTokenContainer.TryGetTokenResult(appid, appsecret, true);
                    StaticObjects.wechatdh.ExecuteNonQuery(string.Format("update MpAccount set AccessToken='{0}',ExpireTime='{1}' where ID='{2}'", token.access_token, DateTime.Now.AddSeconds(token.expires_in), MpID));
                    tokens[MpID] = WxAccessTokenContainer.AccessTokens[appid];
                }
                #endregion

                #region 缓存中有token
                else
                {
                    var token = WxAccessTokenContainer.TryGetTokenResult(tokens[MpID].AppId, tokens[MpID].AppSecret, true);
                    StaticObjects.wechatdh.ExecuteNonQuery(string.Format("update MpAccount set AccessToken='{0}',ExpireTime='{1}' where ID='{2}'", token.access_token, DateTime.Now.AddSeconds(token.expires_in), MpID));
                    tokens[MpID] = WxAccessTokenContainer.AccessTokens[tokens[MpID].AppId];
                }
                #endregion
            }
            #endregion

            return tokens[MpID].AccessTokenResult.access_token;
        }
    }
}
