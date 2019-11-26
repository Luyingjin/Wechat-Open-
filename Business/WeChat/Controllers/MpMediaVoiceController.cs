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

namespace WeChat.Controllers
{
    public class MpMediaVoiceController : BaseController<MpMediaVoice>
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
            var result = entities.Set<MpMediaVoice>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
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
                var mediaid = wxFO.AddMediaFile(mpid, fileInfo.FileFullPath);
                var entity = GetEntity<MpMediaVoice>("");
                EntityCreateLogic(entity);
                entity.IsDelete = 0;
                entity.MediaID = mediaid;
                entity.MpID = mpid;
                entity.Name = fileInfo.FileName.Substring(fileInfo.FileName.IndexOf('_') + 1);
                entity.FileID = fileInfo.FileName;
                entities.Set<MpMediaVoice>().Add(entity);
            }
            entities.SaveChanges();
            return Json(JsonAjaxResult.Successful());
        }

        public override JsonResult Delete()
        {
            #region 假删除
            var IDs = (Request["ListIDs"] ?? "").Split(',');
            var mpid = GetQueryString("MpID");
            var etys = entities.Set<MpMediaVoice>().Where(c => IDs.Contains(c.ID) && c.IsDelete == 0).ToList();
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
