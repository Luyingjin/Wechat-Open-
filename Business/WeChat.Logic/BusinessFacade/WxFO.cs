using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeChat.Logic.Domain;
using WeChat.Logic.WxContainer;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.Groups;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using Formula;
using Formula.Helper;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs.Media;

namespace WeChat.Logic.BusinessFacade
{
    public class WxFO
    {
        WeChatEntities entities = FormulaHelper.GetEntities<WeChatEntities>();

        #region 令牌
        /// <summary>
        /// 获取公众号认证令牌
        /// </summary>
        /// <param name="MpID"></param>
        /// <returns></returns>
        public string GetAccessToken(string MpID,bool GetNewToken=false)
        {
            var token = "";
            var account = CacheHelper.Get(string.Format("WxAccount{0}", MpID)) as MpAccount;
            if (account == null)
            {
                account = entities.MpAccount.Where(c => c.ID == MpID && c.IsDelete == 0).FirstOrDefault();
                CacheHelper.Set(string.Format("WxAccount{0}", MpID), account);
            }
            if (account != null)
            {
                var result = WxAccessTokenContainer.TryGetTokenResult(account.AppID, account.AppSecret, GetNewToken);
                
                if (result.access_token != account.AccessToken)
                {
                    var a = entities.MpAccount.Where(c => c.ID == MpID).FirstOrDefault();
                    a.AccessToken = result.access_token;
                    a.ExpireTime = DateTime.Now.AddSeconds(result.expires_in);
                    entities.SaveChanges();
                }
                token = account.AccessToken;
            }
            return token;
        }

        /// <summary>
        /// 获取jssdk认证令牌
        /// </summary>
        /// <param name="MpID"></param>
        /// <returns></returns>
        public string GetJsApiTicket(string MpID)
        {
            var token = "";
            var account = CacheHelper.Get(string.Format("WxAccount{0}", MpID)) as MpAccount;
            if (account == null)
            {
                account = entities.MpAccount.Where(c => c.ID == MpID && c.IsDelete == 0).FirstOrDefault();
                CacheHelper.Set(string.Format("WxAccount{0}", MpID), account);
            }
            if (account != null)
            {
                var result = WxJsApiTicketContainer.TryGetTicketResult(account.AppID, account.AppSecret);
                token = result.ticket;
            }
            return token;
        }
        #endregion

        #region 菜单
        /// <summary>
        /// 保存菜单到微信
        /// </summary>
        /// <param name="MpID"></param>
        public void SaveMenu(string MpID)
        {
            var menus = entities.MpMenu.Where(c => c.MpID == MpID && c.IsDelete == 0).ToList();
            var bg = new ButtonGroup();
            foreach (var rootButton in menus.Where(c => c.Length == 2))
            {
                var subbuttons = menus.Where(c => c.ParentID == rootButton.ID);
                #region 无子菜单
                if (subbuttons.Count() == 0)
                {
                    //底部单击按钮
                    if (rootButton.Type == null ||
                        (rootButton.Type.Equals("CLICK", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(rootButton.ID)))
                    {
                        throw new WeixinMenuException("单击按钮的key不能为空！");
                    }

                    if (rootButton.Type.Equals("CLICK", StringComparison.OrdinalIgnoreCase))
                    {
                        //点击
                        bg.button.Add(new SingleClickButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                    else if (rootButton.Type.Equals("VIEW", StringComparison.OrdinalIgnoreCase))
                    {
                        //URL
                        bg.button.Add(new SingleViewButton()
                        {
                            name = rootButton.Name,
                            url = rootButton.LinkUrl,
                            type = rootButton.Type
                        });
                    }
                    else if (rootButton.Type.Equals("LOCATION_SELECT", StringComparison.OrdinalIgnoreCase))
                    {
                        //弹出地理位置选择器
                        bg.button.Add(new SingleLocationSelectButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                    else if (rootButton.Type.Equals("PIC_PHOTO_OR_ALBUM", StringComparison.OrdinalIgnoreCase))
                    {
                        //弹出拍照或者相册发图
                        bg.button.Add(new SinglePicPhotoOrAlbumButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                    else if (rootButton.Type.Equals("PIC_SYSPHOTO", StringComparison.OrdinalIgnoreCase))
                    {
                        //弹出系统拍照发图
                        bg.button.Add(new SinglePicSysphotoButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                    else if (rootButton.Type.Equals("PIC_WEIXIN", StringComparison.OrdinalIgnoreCase))
                    {
                        //弹出微信相册发图器
                        bg.button.Add(new SinglePicWeixinButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                    else if (rootButton.Type.Equals("SCANCODE_PUSH", StringComparison.OrdinalIgnoreCase))
                    {
                        //扫码推事件
                        bg.button.Add(new SingleScancodePushButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                    else
                    {
                        //扫码推事件且弹出“消息接收中”提示框
                        bg.button.Add(new SingleScancodeWaitmsgButton()
                        {
                            name = rootButton.Name,
                            key = rootButton.MenuKey,
                            type = rootButton.Type
                        });
                    }
                }
                #endregion

                #region 有子菜单
                else
                {
                    var subButton = new SubButton(rootButton.Name);
                    bg.button.Add(subButton);
                    foreach (var subSubButton in subbuttons)
                    {
                        if (subSubButton.Name == null)
                        {
                            continue; //没有设置菜单
                        }

                        if (subSubButton.Type.Equals("CLICK", StringComparison.OrdinalIgnoreCase)
                            && string.IsNullOrEmpty(subSubButton.ID))
                        {
                            throw new WeixinMenuException("单击按钮的key不能为空！");
                        }


                        if (subSubButton.Type.Equals("CLICK", StringComparison.OrdinalIgnoreCase))
                        {
                            //点击
                            subButton.sub_button.Add(new SingleClickButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                        else if (subSubButton.Type.Equals("VIEW", StringComparison.OrdinalIgnoreCase))
                        {
                            //URL
                            subButton.sub_button.Add(new SingleViewButton()
                            {
                                name = subSubButton.Name,
                                url = subSubButton.LinkUrl,
                                type = subSubButton.Type
                            });
                        }
                        else if (subSubButton.Type.Equals("LOCATION_SELECT", StringComparison.OrdinalIgnoreCase))
                        {
                            //弹出地理位置选择器
                            subButton.sub_button.Add(new SingleLocationSelectButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                        else if (subSubButton.Type.Equals("PIC_PHOTO_OR_ALBUM", StringComparison.OrdinalIgnoreCase))
                        {
                            //弹出拍照或者相册发图
                            subButton.sub_button.Add(new SinglePicPhotoOrAlbumButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                        else if (subSubButton.Type.Equals("PIC_SYSPHOTO", StringComparison.OrdinalIgnoreCase))
                        {
                            //弹出系统拍照发图
                            subButton.sub_button.Add(new SinglePicSysphotoButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                        else if (subSubButton.Type.Equals("PIC_WEIXIN", StringComparison.OrdinalIgnoreCase))
                        {
                            //弹出微信相册发图器
                            subButton.sub_button.Add(new SinglePicWeixinButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                        else if (subSubButton.Type.Equals("SCANCODE_PUSH", StringComparison.OrdinalIgnoreCase))
                        {
                            //扫码推事件
                            subButton.sub_button.Add(new SingleScancodePushButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                        else
                        {
                            //扫码推事件且弹出“消息接收中”提示框
                            subButton.sub_button.Add(new SingleScancodeWaitmsgButton()
                            {
                                name = subSubButton.Name,
                                key = subSubButton.MenuKey,
                                type = subSubButton.Type
                            });
                        }
                    }
                }
                #endregion
            }

            WxJsonResult result = null;
            try
            {
                result = CommonApi.CreateMenu(GetAccessToken(MpID), bg);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("更新MpID为{0}的菜单报错", MpID));
                result = CommonApi.CreateMenu(GetAccessToken(MpID, true), bg);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = CommonApi.CreateMenu(GetAccessToken(MpID, true), bg);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("更新MpID为" + MpID + "的菜单报错，原因：" + result.errmsg);
        }
        #endregion

        #region  素材
        /// <summary>
        /// 上传素材
        /// </summary>
        /// <param name="MpID">公众号ID</param>
        /// <param name="FileType">素材类型</param>
        /// <param name="FilePath">素材路径</param>
        /// <returns></returns>
        public string AddMediaFile(string MpID, string FilePath)
        {
            UploadForeverMediaResult result = null;
            try
            {
                result = MediaApi.UploadForeverMedia(GetAccessToken(MpID), FilePath);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("添加MpID为{0}的素材", MpID));
                result = MediaApi.UploadForeverMedia(GetAccessToken(MpID, true), FilePath);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = MediaApi.UploadForeverMedia(GetAccessToken(MpID, true), FilePath);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("上传素材失败，原因："+result.errmsg);

            return result.media_id;
        }

        /// <summary>
        /// 上传视频素材
        /// </summary>
        /// <param name="MpID">公众号ID</param>
        /// <param name="FileType">素材类型</param>
        /// <param name="FilePath">素材路径</param>
        /// <param name="FilePath">标题</param>
        /// <param name="FilePath">说明</param>
        /// <returns></returns>
        public string AddVideoFile(string MpID, string FilePath,string Title,string Introduction)
        {
            UploadForeverMediaResult result = null;
            try
            {
                result = MediaApi.UploadForeverVideo(GetAccessToken(MpID), FilePath, Title, Introduction);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("添加MpID为{0}的素材", MpID));
                result = MediaApi.UploadForeverVideo(GetAccessToken(MpID, true), FilePath, Title, Introduction);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = MediaApi.UploadForeverVideo(GetAccessToken(MpID, true), FilePath, Title, Introduction);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("上传素材失败，原因：" + result.errmsg);

            return result.media_id;
        }

        public void DelMediaFile(string MpID, string MediaID)
        {
            WxJsonResult result = null;
            try
            {
                result = MediaApi.DeleteForeverMedia(GetAccessToken(MpID), MediaID);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("删除MpID为{0}的素材{1}", MpID, MediaID));
                result = MediaApi.DeleteForeverMedia(GetAccessToken(MpID, true), MediaID);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = MediaApi.DeleteForeverMedia(GetAccessToken(MpID, true), MediaID);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("删除MpID为" + MpID + "的素材" + MediaID + "失败，原因：" + result.errmsg);
        }

        /// <summary>
        /// 上传图文
        /// </summary>
        /// <param name="MpID">公众号ID</param>
        /// <param name="Article">图文内容</param>
        /// <returns></returns>
        public string AddMediaNews(string MpID, MpMediaArticle Article)
        {
            var news = new NewsModel()
            {
                title = Article.Title,
                author = Article.Author,
                digest = Article.Description,
                content = Article.Content,
                content_source_url = Article.Url,
                show_cover_pic = Article.ShowPic,
                thumb_media_id = Article.PicMediaID,
            };
            UploadForeverMediaResult result = null;
            try
            {
                result = MediaApi.UploadNews(GetAccessToken(MpID), Senparc.Weixin.Config.TIME_OUT, news);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("添加MpID为{0}的图文失败", MpID));
                result = MediaApi.UploadNews(GetAccessToken(MpID, true), Senparc.Weixin.Config.TIME_OUT, news);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = MediaApi.UploadNews(GetAccessToken(MpID, true), Senparc.Weixin.Config.TIME_OUT, news);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("添加MpID为" + MpID + "的图文失败，原因：" + result.errmsg);

            return result.media_id;
        }

        /// <summary>
        /// 上传图文
        /// </summary>
        /// <param name="MpID">公众号ID</param>
        /// <param name="Articles">图文内容</param>
        /// <returns></returns>
        public string AddMediaNews(string MpID,params MpMediaArticle[] Articles)
        {
            var news = Articles.Select(c => new NewsModel()
            {
                title = c.Title,
                author = c.Author,
                digest = c.Description,
                content = c.Content,
                content_source_url = c.Url,
                show_cover_pic = c.ShowPic,
                thumb_media_id = c.PicMediaID,
            }).ToArray();
            UploadForeverMediaResult result = null;
            try
            {
                result = MediaApi.UploadNews(GetAccessToken(MpID), Senparc.Weixin.Config.TIME_OUT, news);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("添加MpID为{0}的图文失败", MpID));
                result = MediaApi.UploadNews(GetAccessToken(MpID, true), Senparc.Weixin.Config.TIME_OUT, news);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = MediaApi.UploadNews(GetAccessToken(MpID, true), Senparc.Weixin.Config.TIME_OUT, news);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("添加MpID为" + MpID + "的图文失败，原因：" + result.errmsg);
            return result.media_id;
        }

        /// <summary>
        /// 修改图文
        /// </summary>
        /// <param name="MpID">公众号ID</param>
        /// <param name="MediaID">素材ID</param>
        /// <param name="Article">图文内容</param>
        /// <returns></returns>
        public void UpdateMediaNews(string MpID, string MediaID, int index, MpMediaArticle Article)
        {
            var news = new NewsModel()
            {
                title = Article.Title,
                author = Article.Author,
                digest = Article.Description,
                content = Article.Content,
                content_source_url = Article.Url,
                show_cover_pic = Article.ShowPic,
                thumb_media_id = Article.PicMediaID,
            };
            WxJsonResult result = null;
            try
            {
                result = MediaApi.UpdateForeverNews(GetAccessToken(MpID), MediaID, index, news, Senparc.Weixin.Config.TIME_OUT);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("修改MpID为{0}的图文失败", MpID));
                result = MediaApi.UpdateForeverNews(GetAccessToken(MpID, true), MediaID, index, news, Senparc.Weixin.Config.TIME_OUT);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = MediaApi.UpdateForeverNews(GetAccessToken(MpID, true), MediaID, index, news, Senparc.Weixin.Config.TIME_OUT);
            if (result.errcode != ReturnCode.请求成功)
                throw new WeixinException("修改MpID为" + MpID + "的图文失败，原因：" + result.errmsg);
        }
        #endregion

        #region 用户分组
        /// <summary>
        /// 获取最新的用户分组
        /// </summary>
        /// <param name="MpID"></param>
        public void GetGroup(string MpID)
        {
            GroupsJson result = null;
            try
            {
                result = GroupsApi.Get(GetAccessToken(MpID));
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("获取MpID为{0}的分组报错", MpID));
                result = GroupsApi.Get(GetAccessToken(MpID, true));
            }
            if (result.errcode != ReturnCode.请求成功)
                result = GroupsApi.Get(GetAccessToken(MpID, true));
            if (result.errcode != ReturnCode.请求成功)
                throw new Exception(string.Format("获取MpID为{0}的分组报错，错误编号:{1}，错误消息:{2}", MpID, result.errcode, result.errmsg));

            #region 处理根节点
            var rootgroup = entities.Set<MpGroup>().Where(c => c.MpID == MpID && c.Length == 1).FirstOrDefault();
            if (rootgroup == null)
            {
                rootgroup = new MpGroup();
                rootgroup.ID = Formula.FormulaHelper.CreateGuid();
                rootgroup.MpID = MpID;
                rootgroup.Name = "全部";
                rootgroup.ParentID = "-1";
                rootgroup.FullPath = rootgroup.ID;
                rootgroup.Length = 1;
                rootgroup.ChildCount = result.groups.Count();
                entities.Set<MpGroup>().Add(rootgroup);
            }
            #endregion

            #region 保存最新的节点
            var oldgroups = entities.Set<MpGroup>().Where(c => c.MpID == MpID && c.Length == 2).ToList();
            var notgroupid = "";
            foreach (var wxgroup in result.groups)
            {
                var g = oldgroups.Where(c => c.WxGroupID == wxgroup.id).FirstOrDefault();
                if (g == null)
                {
                    g = new MpGroup();
                    g.ID = Formula.FormulaHelper.CreateGuid();
                    g.MpID = MpID;
                    g.Name = wxgroup.name;
                    g.ParentID = rootgroup.ID;
                    g.FullPath = string.Format("{0}.{1}", rootgroup.ID, g.ID);
                    g.Length = 2;
                    g.ChildCount = 0;
                    g.FansCount = wxgroup.count;
                    g.WxGroupID = wxgroup.id;
                    entities.Set<MpGroup>().Add(g);
                    if (g.Name == "未分组")
                        notgroupid = g.ID;
                }
                else
                {
                    g.Name = wxgroup.name;
                    g.FansCount = wxgroup.count;
                    if (g.Name == "未分组")
                        notgroupid = g.ID;
                }
            }
            #endregion

            #region 剔除老的节点
            var deletegroups = oldgroups.Where(c => !result.groups.Select(d => d.id).Contains(c.WxGroupID.Value)).ToList();
            if (notgroupid == "")
                throw new Exception("找不到未分组节点");
            foreach (var dg in deletegroups)
            {
                for (int i = 0; i < dg.MpFans.Count(); i++)
                    dg.MpFans.ElementAt(i).GroupID = notgroupid;
                entities.Set<MpGroup>().Remove(dg);
            }
            entities.SaveChanges();
            //暂时不明白为什么会出现重复分组
            var samegroups = oldgroups.Where(c => result.groups.Select(d => d.id).Contains(c.WxGroupID.Value)).GroupBy(c=>c.WxGroupID).Where(c=>c.Count()>1).ToList();
            foreach (var sg in samegroups)
            {
                foreach (var dg in sg.Where((c, i) => i > 0))
                {
                    for (int i = 0; i < dg.MpFans.Count(); i++)
                        dg.MpFans.ElementAt(i).GroupID = notgroupid;
                    entities.Set<MpGroup>().Remove(dg);
                }
            }
            #endregion

            entities.SaveChanges();
        }

        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GroupsJson_Group AddGroup(string MpID, string Name)
        {
            CreateGroupResult result = null;
            try
            {
                result = GroupsApi.Create(GetAccessToken(MpID), Name);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("创建MpID为{0}的分组报错", MpID));
                result = GroupsApi.Create(GetAccessToken(MpID, true), Name);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = GroupsApi.Create(GetAccessToken(MpID, true), Name);
            if (result.errcode != ReturnCode.请求成功)
                throw new Exception(string.Format("创建MpID为{0}的分组报错，错误编号:{1}，错误消息:{2}", MpID, result.errcode, result.errmsg));
            return result.group;

        }

        /// <summary>
        /// 修改分组名称
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="GroupID"></param>
        /// <param name="Name"></param>
        public void UpdateGroup(string MpID, int GroupID, string Name)
        {
           WxJsonResult result = null;
           try
           {
               result = GroupsApi.Update(GetAccessToken(MpID), GroupID, Name);
           }
           catch (Exception ex)
           {
               LogWriter.Error(ex, string.Format("更新MpID为{0}的分组报错", MpID));
               result = GroupsApi.Update(GetAccessToken(MpID, true), GroupID, Name);
           }
           if (result.errcode != ReturnCode.请求成功)
               result = GroupsApi.Update(GetAccessToken(MpID, true), GroupID, Name);
           if (result.errcode != ReturnCode.请求成功)
               throw new Exception(string.Format("更新MpID为{0}的分组报错，错误编号:{1}，错误消息:{2}", MpID, result.errcode, result.errmsg));
        }

        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="GroupID"></param>
        public void DeleteGroup(string MpID, int GroupID)
        {
           WxJsonResult result = null;
           try
           {
               result = GroupsApi.Delete(GetAccessToken(MpID), GroupID);
           }
           catch (Exception ex)
           {
               LogWriter.Error(ex, string.Format("删除MpID为{0}的分组报错", MpID));
               result = GroupsApi.Delete(GetAccessToken(MpID, true), GroupID);
           }
           if (result.errcode != ReturnCode.请求成功)
               result = GroupsApi.Delete(GetAccessToken(MpID, true), GroupID);
           if (result.errcode != ReturnCode.请求成功)
               throw new Exception(string.Format("删除MpID为{0}的分组报错，错误编号:{1}，错误消息:{2}", MpID, result.errcode, result.errmsg));
        }

        /// <summary>
        /// 移动用户分组
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="OpenID"></param>
        /// <param name="GroupID"></param>
        public void MoveGroup(string MpID, string OpenID, int GroupID)
        {
            WxJsonResult result = null;
            try
            {
                result = GroupsApi.MemberUpdate(GetAccessToken(MpID), OpenID, GroupID);
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("移动MpID为{0}的用户分组报错", MpID));
                result = GroupsApi.MemberUpdate(GetAccessToken(MpID, true), OpenID, GroupID);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = GroupsApi.MemberUpdate(GetAccessToken(MpID, true), OpenID, GroupID);
            if (result.errcode != ReturnCode.请求成功)
                throw new Exception(string.Format("移动MpID为{0}的用户分组报错，错误编号:{1}，错误消息:{2}", MpID, result.errcode, result.errmsg));
        }
        #endregion

        #region 用户
        /// <summary>
        /// 根据openid获取用户基本信息
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="OpenID"></param>
        /// <returns></returns>
        public UserInfoJson GetFans(string MpID, string OpenID, bool GetNewToken = false)
        {
            return UserApi.Info(GetAccessToken(MpID, GetNewToken), OpenID);
        }

        /// <summary>
        /// 更新用户备注
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="OpenID"></param>
        /// <param name="Remark"></param>
        public void UpdateFans(string MpID, string OpenID, string Remark)
        {
            WxJsonResult result = null;
            try
            {
                result = UserApi.UpdateRemark(GetAccessToken(MpID), OpenID, Remark);
            }
            catch (Exception ex)
            {
                result = UserApi.UpdateRemark(GetAccessToken(MpID, true), OpenID, Remark);
            }
        }

        public void RefreshFans(string MpID)
        {
            var openidlist = new List<string>();
            OpenIdResultJson fansopenids = null;
            try
            {
                fansopenids = UserApi.Get(GetAccessToken(MpID), "");
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex, string.Format("获取MpID为{0}的openid报错", MpID));
                fansopenids = UserApi.Get(GetAccessToken(MpID, true), "");
            }

            if (fansopenids.count > 0)
            {
                SaveFans(MpID, fansopenids.data.openid);
                openidlist.AddRange(fansopenids.data.openid);
            }
            while (!string.IsNullOrEmpty(fansopenids.next_openid))
            {
                try
                {
                    fansopenids = UserApi.Get(GetAccessToken(MpID), fansopenids.next_openid);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("获取MpID为{0}，openid为{1}的用户信息报错", MpID, fansopenids.next_openid));
                    fansopenids = UserApi.Get(GetAccessToken(MpID, true), fansopenids.next_openid);
                }

                if (fansopenids.count > 0)
                {
                    SaveFans(MpID, fansopenids.data.openid);
                    openidlist.AddRange(fansopenids.data.openid);
                }
            }
            var now = DateTime.Now.AddDays(-1);
            entities.Set<MpFans>().Where(c => c.MpID == MpID && c.IsFans == "1" && c.UpdateTime < now).Update(c => c.IsFans = "0");
            entities.SaveChanges();
        }

        private void SaveFans(string MpID, List<string> OpenIDs)
        {
            foreach (var openid in OpenIDs)
            {
                UserInfoJson wxinfo = null;
                try
                {
                    wxinfo = UserApi.Info(GetAccessToken(MpID), openid);
                }
                catch (Exception ex)
                {
                    LogWriter.Error(ex, string.Format("获取MpID为{0}，openid为{1}的用户信息报错", MpID, openid));
                    wxinfo = UserApi.Info(GetAccessToken(MpID, true), openid);
                }
                if (wxinfo.errcode != ReturnCode.请求成功)
                    throw new Exception(string.Format("获取MpID为{0}，openid为{1}的用户信息报错，错误编号:{2}，错误消息:{3}", MpID, openid, wxinfo.errcode, wxinfo.errmsg));


                var group = entities.Set<MpGroup>().Where(c => c.MpID == MpID && c.WxGroupID == wxinfo.groupid).FirstOrDefault();
                if (group == null)
                    continue;
                var entity = entities.Set<MpFans>().Where(c => c.MpID == MpID && c.OpenID == openid).FirstOrDefault();
                if (entity == null)
                {
                    entity = new MpFans();
                    entity.ID = Formula.FormulaHelper.CreateGuid();
                    entity.City = wxinfo.city;
                    entity.Country = wxinfo.country;
                    entity.HeadImgUrl = wxinfo.headimgurl;
                    entity.IsFans = "1";
                    entity.Language = wxinfo.language;
                    entity.MpID = MpID;
                    entity.NickName = wxinfo.nickname;
                    entity.OpenID = wxinfo.openid;
                    entity.Province = wxinfo.province;
                    entity.Remark = wxinfo.remark;
                    entity.Sex = wxinfo.sex.ToString();
                    entity.SubscribeTime = DateTimeHelper.GetDateTimeFromXml(wxinfo.subscribe_time);
                    entity.UniionID = wxinfo.unionid;
                    entity.WxGroupID = wxinfo.groupid;
                    entity.GroupID = group.ID;
                    entity.UpdateTime = DateTime.Now;
                    entities.Set<MpFans>().Add(entity);
                }
                else
                {
                    entity.City = wxinfo.city;
                    entity.Country = wxinfo.country;
                    entity.HeadImgUrl = wxinfo.headimgurl;
                    entity.IsFans = "1";
                    entity.Language = wxinfo.language;
                    entity.MpID = MpID;
                    entity.NickName = wxinfo.nickname;
                    entity.OpenID = wxinfo.openid;
                    entity.Province = wxinfo.province;
                    entity.Remark = wxinfo.remark;
                    entity.Sex = wxinfo.sex.ToString();
                    entity.SubscribeTime = DateTimeHelper.GetDateTimeFromXml(wxinfo.subscribe_time);
                    entity.UniionID = wxinfo.unionid;
                    entity.WxGroupID = wxinfo.groupid;
                    entity.GroupID = group.ID;
                    entity.UpdateTime = DateTime.Now;
                }
            }
        }
        #endregion

        #region 消息
        /// <summary>
        /// 预览素材
        /// </summary>
        /// <param name="MpID"></param>
        /// <param name="MediaID"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public string PreViewMedia(string MpID, string MediaID, string openId)
        {
            SendResult result = null;
            try
            {
                result = GroupMessageApi.SendGroupMessagePreview(GetAccessToken(MpID), Senparc.Weixin.MP.GroupMessageType.mpnews, MediaID, openId);
            }
            catch (Exception ex)
            {
                result = GroupMessageApi.SendGroupMessagePreview(GetAccessToken(MpID, true), Senparc.Weixin.MP.GroupMessageType.mpnews, MediaID, openId);
            }
            if (result.errcode != ReturnCode.请求成功)
                result = GroupMessageApi.SendGroupMessagePreview(GetAccessToken(MpID, true), Senparc.Weixin.MP.GroupMessageType.mpnews, MediaID, openId);
            return result.msg_id;
        }

        public void SendMessage(MpMessage Message)
        {
            SendResult result = null;
            var groupids = Message.GroupIDs.Split(',').Where(c => !string.IsNullOrEmpty(c));
            var allgroups = entities.Set<MpGroup>().Where(c => c.MpID == Message.MpID).ToArray();
            var groups = from a in allgroups
                         join b in groupids
                         on a.ID equals b
                         select a;
            //按分组发送
            if (new string[] { Message.Country, Message.Province, Message.City, Message.Sex }.All(c => string.IsNullOrEmpty(c)))
            {

                #region 发送全部分组
                if (string.IsNullOrEmpty(Message.GroupIDs) || groups.Any(c => c.Name == "全部"))
                {
                    if (Message.MessageType == MpMessageType.text.ToString())
                    {
                        try
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), "", Message.Content, GroupMessageType.text, true);
                        }
                        catch (Exception ex)
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.Content, GroupMessageType.text, true);
                        }
                        if (result.errcode != ReturnCode.请求成功)
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.Content, GroupMessageType.text, true);
                    }
                    else if (Message.MessageType == MpMessageType.video.ToString())
                    {
                        try
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), "", Message.VideoMediaID, GroupMessageType.video, true);
                        }
                        catch (Exception ex)
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.VideoMediaID, GroupMessageType.video, true);
                        }
                        if (result.errcode != ReturnCode.请求成功)
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.VideoMediaID, GroupMessageType.video, true);
                    }
                    else if (Message.MessageType == MpMessageType.voice.ToString())
                    {
                        try
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), "", Message.VoiceMediaID, GroupMessageType.voice, true);
                        }
                        catch (Exception ex)
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.VoiceMediaID, GroupMessageType.voice, true);
                        }
                        if (result.errcode != ReturnCode.请求成功)
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.VoiceMediaID, GroupMessageType.voice, true);
                    }
                    else if (Message.MessageType == MpMessageType.image.ToString())
                    {
                        try
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), "", Message.ImageMediaID, GroupMessageType.image, true);
                        }
                        catch (Exception ex)
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.ImageMediaID, GroupMessageType.image, true);
                        }
                        if (result.errcode != ReturnCode.请求成功)
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.ImageMediaID, GroupMessageType.image, true);
                    }
                    else if (Message.MessageType == MpMessageType.mpnews.ToString())
                    {
                        try
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), "", Message.ArticleMediaID, GroupMessageType.mpnews, true);
                        }
                        catch (Exception ex)
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.ArticleMediaID, GroupMessageType.mpnews, true);
                        }
                        if (result.errcode != ReturnCode.请求成功)
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.ArticleMediaID, GroupMessageType.mpnews, true);
                    }
                    else if (Message.MessageType == MpMessageType.mpmultinews.ToString())
                    {
                        try
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), "", Message.ArticleGroupMediaID, GroupMessageType.mpnews, true);
                        }
                        catch (Exception ex)
                        {
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.ArticleGroupMediaID, GroupMessageType.mpnews, true);
                        }

                        if (result.errcode != ReturnCode.请求成功)
                            result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), "", Message.ArticleGroupMediaID, GroupMessageType.mpnews, true);
                    }
                }
                #endregion

                #region 发送特定分组
                else
                {
                    foreach (var groupid in groups.Where(c => c.WxGroupID != null).Select(c => c.WxGroupID.ToString()))
                    {
                        if (Message.MessageType == MpMessageType.text.ToString())
                        {
                            try
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), groupid, Message.Content, GroupMessageType.text, true);
                            }
                            catch (Exception ex)
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.Content, GroupMessageType.text, true);
                            }
                            if (result.errcode != ReturnCode.请求成功)
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.Content, GroupMessageType.text, true);
                        }
                        else if (Message.MessageType == MpMessageType.video.ToString())
                        {
                            try
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), groupid, Message.VideoMediaID, GroupMessageType.video, true);
                            }
                            catch (Exception ex)
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.VideoMediaID, GroupMessageType.video, true);
                            }
                            if (result.errcode != ReturnCode.请求成功)
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.VideoMediaID, GroupMessageType.video, true);
                        }
                        else if (Message.MessageType == MpMessageType.voice.ToString())
                        {
                            try
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), groupid, Message.VoiceMediaID, GroupMessageType.voice, true);
                            }
                            catch (Exception ex)
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.VoiceMediaID, GroupMessageType.voice, true);
                            }
                            if (result.errcode != ReturnCode.请求成功)
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.VoiceMediaID, GroupMessageType.voice, true);
                        }
                        else if (Message.MessageType == MpMessageType.image.ToString())
                        {
                            try
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), groupid, Message.ImageMediaID, GroupMessageType.image, true);
                            }
                            catch (Exception ex)
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.ImageMediaID, GroupMessageType.image, true);
                            }
                            if (result.errcode != ReturnCode.请求成功)
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.ImageMediaID, GroupMessageType.image, true);
                        }
                        else if (Message.MessageType == MpMessageType.mpnews.ToString())
                        {
                            try
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), groupid, Message.ArticleMediaID, GroupMessageType.mpnews, true);
                            }
                            catch (Exception ex)
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.ArticleMediaID, GroupMessageType.mpnews, true);
                            }
                            if (result.errcode != ReturnCode.请求成功)
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.ArticleMediaID, GroupMessageType.mpnews, true);
                        }
                        else if (Message.MessageType == MpMessageType.mpmultinews.ToString())
                        {
                            try
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID), groupid, Message.ArticleGroupMediaID, GroupMessageType.mpnews, true);
                            }
                            catch (Exception ex)
                            {
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.ArticleGroupMediaID, GroupMessageType.mpnews, true);
                            }

                            if (result.errcode != ReturnCode.请求成功)
                                result = GroupMessageApi.SendGroupMessageByGroupId(GetAccessToken(Message.MpID, true), groupid, Message.ArticleGroupMediaID, GroupMessageType.mpnews, true);
                        }
                    }
                }
                #endregion
            }
            //按openid发送
            else
            {
                #region 过滤粉丝
                var fans = entities.Set<MpFans>().Where(c => c.MpID == Message.MpID && c.IsFans == "1");
                if (!string.IsNullOrEmpty(Message.GroupIDs) && !groups.Any(c => c.Name == "全部"))
                {
                    fans = fans.Where(c => groupids.Contains(c.GroupID));
                }
                if (!string.IsNullOrEmpty(Message.Sex))
                {
                    var sexes = Message.Sex.Split(',');
                    fans = fans.Where(c => sexes.Contains(c.Sex));
                }
                if (!string.IsNullOrEmpty(Message.Country))
                {
                    fans = fans.Where(c => Message.Country == c.Country);
                }
                if (!string.IsNullOrEmpty(Message.Province))
                {
                    fans = fans.Where(c => Message.Province == c.Province);
                }
                if (!string.IsNullOrEmpty(Message.City))
                {
                    var cities = Message.City.Split(',');
                    fans = fans.Where(c => cities.Contains(c.City));
                }
                #endregion

                #region 发送消息
                if (Message.MessageType == MpMessageType.text.ToString())
                {
                    try
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID), GroupMessageType.text, Message.Content, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    catch (Exception ex)
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.text, Message.Content, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    if (result.errcode != ReturnCode.请求成功)
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.text, Message.Content, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                }
                else if (Message.MessageType == MpMessageType.video.ToString())
                {
                    var video = entities.Set<MpMediaVideo>().Where(c => c.MpID == Message.MpID && c.ID == Message.VideoID).FirstOrDefault();
                    if (video == null)
                        throw new Exception(string.Format("视频{0}不存在", Message.VideoName));
                    try
                    {
                        result = GroupMessageApi.SendVideoGroupMessageByOpenId(GetAccessToken(Message.MpID), video.Title, video.Description, Message.VideoMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    catch (Exception ex)
                    {
                        result = GroupMessageApi.SendVideoGroupMessageByOpenId(GetAccessToken(Message.MpID, true), video.Title, video.Description, Message.VideoMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    if (result.errcode != ReturnCode.请求成功)
                        result = GroupMessageApi.SendVideoGroupMessageByOpenId(GetAccessToken(Message.MpID, true), video.Title, video.Description, Message.VideoMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                }
                else if (Message.MessageType == MpMessageType.voice.ToString())
                {
                    try
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID), GroupMessageType.voice, Message.VoiceMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    catch (Exception ex)
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.voice, Message.VoiceMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    if (result.errcode != ReturnCode.请求成功)
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.voice, Message.VoiceMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                }
                else if (Message.MessageType == MpMessageType.image.ToString())
                {
                    try
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID), GroupMessageType.image, Message.ImageMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    catch (Exception ex)
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.image, Message.ImageMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    if (result.errcode != ReturnCode.请求成功)
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.image, Message.ImageMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                }
                else if (Message.MessageType == MpMessageType.mpnews.ToString())
                {
                    try
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID), GroupMessageType.mpnews, Message.ArticleMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    catch (Exception ex)
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.mpnews, Message.ArticleMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    if (result.errcode != ReturnCode.请求成功)
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.mpnews, Message.ArticleMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                }
                else if (Message.MessageType == MpMessageType.mpmultinews.ToString())
                {
                    try
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID), GroupMessageType.mpnews, Message.ArticleGroupMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    catch (Exception ex)
                    {
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.mpnews, Message.ArticleGroupMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                    }
                    if (result.errcode != ReturnCode.请求成功)
                        result = GroupMessageApi.SendGroupMessageByOpenId(GetAccessToken(Message.MpID, true), GroupMessageType.mpnews, Message.ArticleGroupMediaID, Senparc.Weixin.Config.TIME_OUT, fans.Select(c => c.OpenID).ToArray());
                }
                #endregion
            }

            if (result != null)
            {
                Message.WxMsgID = result.msg_id;
                GetSendResult sendresult = null;
                try
                {
                    sendresult = GroupMessageApi.GetGroupMessageResult(GetAccessToken(Message.MpID), result.msg_id);
                }
                catch (Exception ex)
                {
                    sendresult = GroupMessageApi.GetGroupMessageResult(GetAccessToken(Message.MpID, true), result.msg_id);
                }
                if (result.errcode != ReturnCode.请求成功)
                    sendresult = GroupMessageApi.GetGroupMessageResult(GetAccessToken(Message.MpID, true), result.msg_id);
                Message.State = sendresult.msg_status;
            }

        }
        #endregion
    }
}
