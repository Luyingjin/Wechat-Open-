using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using WeChat.wechatfile;
using WeChat.Logic;
using WeChat.Logic.BusinessFacade;
using MvcAdapter;
using Formula.Exceptions;

namespace WeChat.Controllers
{
    public class MpMediaVideoController : BaseController<MpMediaVideo>
    {
        private MasterService _masterService = null;
        private MasterService masterService
        {
            get
            {
                if (_masterService == null)
                {
                    _masterService = new MasterService();
                    _masterService.Url = WeChatConfig.MasterServerUrl;
                }
                return _masterService;
            }
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpMediaVideo>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public JsonResult AddFile()
        {
            var mpid = GetQueryString("MpID");
            var filename = GetQueryString("FileName");
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            foreach (var fn in filename.Split(',').Where(c => !string.IsNullOrEmpty(c)))
            {
                FsFileInfo fileInfo = masterService.GetFileInfo(fn, WeChatConfig.FileServerName);
                var entity = GetEntity<MpMediaVideo>("");
                EntityCreateLogic(entity);
                entity.IsDelete = 0;
                entity.MpID = mpid;
                entity.Title = fileInfo.FileName.Substring(fileInfo.FileName.IndexOf('_') + 1);
                entity.FileID = fileInfo.FileName;
                var mediaid = wxFO.AddVideoFile(mpid, fileInfo.FileFullPath, entity.Title, "");
                entity.MediaID = mediaid;
                entities.Set<MpMediaVideo>().Add(entity);
            }
            entities.SaveChanges();
            return Json(JsonAjaxResult.Successful());
        }

        public override JsonResult Save()
        {
            #region 数据校验
            MpMediaVideo entity = UpdateEntity<MpMediaVideo>();
            if (string.IsNullOrEmpty(entity.FileID))
                throw new BusinessException("视频不能为空");
            if (string.IsNullOrEmpty(entity.Title))
                throw new BusinessException("标题不能为空");
            #endregion

            #region 微信处理
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = GetQueryString("MpID");
            if (entity._state == EntityStatus.added.ToString())
            {
                var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
                FsFileInfo fileInfo = masterService.GetFileInfo(entity.FileID, WeChatConfig.FileServerName);
                if (fileInfo == null)
                    throw new Exception("找不到视频文件");
                var mediaid = wxFO.AddVideoFile(entity.MpID, fileInfo.FileFullPath, entity.Title, entity.Description);
                entity.MediaID = mediaid;
            }
            #endregion

            #region 保存数据
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {
            #region 假删除
            var IDs = (Request["ListIDs"] ?? "").Split(',');
            var mpid = GetQueryString("MpID");
            var etys = entities.Set<MpMediaVideo>().Where(c => IDs.Contains(c.ID) && c.IsDelete == 0).ToList();
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            for (int i = 0; i < etys.Count(); i++)
            {
                var entity = etys[i];
                if (string.IsNullOrEmpty(entity.MediaID))
                    continue;
                wxFO.DelMediaFile(mpid, entity.MediaID);
                entity.IsDelete = 1;
            }
            entities.SaveChanges();
            return Json(JsonAjaxResult.Successful());
            #endregion
        }
    }
}
