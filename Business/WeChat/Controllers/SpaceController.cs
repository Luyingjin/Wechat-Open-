using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Formula;
using WeChat.Logic.BusinessFacade;
using WeChat.Logic;

namespace WeChat.Controllers
{
    public class SpaceController : BaseController
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string mpID = this.Request["MpID"];
            var mpUserRelationFO = Formula.FormulaHelper.CreateFO<MpUserRelationFO>();
            if (String.IsNullOrEmpty(mpID))
                mpID = mpUserRelationFO.GetDefaultMpID(this.CurrentUserInfo.UserID);
            else
            {
                mpUserRelationFO.SetDefaultMp(this.CurrentUserInfo.UserID, mpID);
                mpUserRelationFO.SetFocusMp(this.CurrentUserInfo.UserID, mpID);
            }
            this.ViewBag.CurrentUserID = this.CurrentUserInfo.UserID;
            if (String.IsNullOrEmpty(mpID)) throw new Exception("您没有参与公众号，无法进入微信管理");
            string userID = this.ViewBag.CurrentUserID;
            var mpInfo = entities.Set<MpAccount>().Where(c => c.ID == mpID).FirstOrDefault();
            this.ViewBag.MpInfo = mpInfo;
            this.ViewBag.MpJson = JsonHelper.ToJson(mpInfo);
            var t = SysBool.T.ToString();
            var userMp = entities.Set<MpAccountUserRelation>()
                .Where(c => c.UserID == this.CurrentUserInfo.UserID && c.IsUsed == t && c.MpAccount != null)
                .Select(c => c.MpAccount).ToList();
            this.ViewBag.RelationMp = JsonHelper.ToJson(userMp);
            return View();
        }

        /// <summary>
        /// 获取用户常用公众号
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        public JsonResult GetMyMpInfo(QueryBuilder qb)
        {
            string userID = this.CurrentUserInfo.UserID;
            var used = SysBool.T.ToString();
            var mpInfoList = entities.Set<MpAccountUserRelation>()
                .Where(c => c.UserID == userID && c.IsUsed == used && c.MpAccount != null&&c.MpAccount.IsDelete==0).Select(c => c.MpAccount).Where(qb);

            GridData gridData = new GridData(mpInfoList);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        /// <summary>
        /// 获取用户所有公众号
        /// </summary>
        /// <param name="qb"></param>
        /// <returns></returns>
        public JsonResult GetAllMpInfo(QueryBuilder qb)
        {
            string userID = this.CurrentUserInfo.UserID;
            var mpInfoList = entities.Set<MpAccountUserRelation>()
                .Where(c => c.UserID == userID && c.MpAccount != null && c.MpAccount.IsDelete == 0).Select(c => c.MpAccount).Where(qb);

            GridData gridData = new GridData(mpInfoList);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        /// <summary>
        /// 获取公众号下的菜单
        /// </summary>
        /// <param name="DefineID"></param>
        /// <param name="SpaceCode"></param>
        /// <param name="ProjectInfoID"></param>
        /// <returns></returns>
        public JsonResult GetSpaceMenu()
        {
            var data=CacheHelper.Get("WxSpaceMenu");
            if (data == null)
            {
                var menu = entities.Set<MPAccountSpaceMenu>().OrderBy(c => c.SortIndex).ToList();
                CacheHelper.Set("WxSpaceMenu", menu);
                return Json(menu);
            }
            return Json(data);
        }

        /// <summary>
        /// 设置常用公众号
        /// </summary>
        /// <param name="MpID"></param>
        /// <returns></returns>
        public JsonResult SetFocusMp(string MpID)
        {
            var mpUserRelationFO = Formula.FormulaHelper.CreateFO<MpUserRelationFO>();
            mpUserRelationFO.SetFocusMp(this.CurrentUserInfo.UserID, MpID);
            return Json(JsonAjaxResult.Successful());
        }

        /// <summary>
        /// 取消常用公众号
        /// </summary>
        /// <param name="MpID"></param>
        /// <returns></returns>
        public JsonResult CancelFocusMp(string MpID)
        {
            var mpUserRelationFO = Formula.FormulaHelper.CreateFO<MpUserRelationFO>();
            mpUserRelationFO.CancelFocusMp(this.CurrentUserInfo.UserID, MpID);
            return Json(JsonAjaxResult.Successful());
        }

        /// <summary>
        /// 设置默认公众号
        /// </summary>
        /// <param name="MpID"></param>
        /// <returns></returns>
        public JsonResult SetDefaultMp(string MpID)
        {
            var mpUserRelationFO = Formula.FormulaHelper.CreateFO<MpUserRelationFO>();
            mpUserRelationFO.SetDefaultMp(this.CurrentUserInfo.UserID, MpID);
            return Json(JsonAjaxResult.Successful());
        }
    }
}
