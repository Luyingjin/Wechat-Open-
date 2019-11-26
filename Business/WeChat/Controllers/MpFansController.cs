using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using WeChat.Logic.BusinessFacade;
using Formula.Helper;
using Config.Logic;

namespace WeChat.Controllers
{
    public class MpFansController : BaseController<MpFans>
    {
        public override ActionResult List()
        {
            var MpID = GetQueryString("MpID");
            var ge = CacheHelper.Get(string.Format("WxGroupEnum{0}", MpID)) as string;
            if (ge == null)
            {
                var groupenum = entities.Set<MpGroup>().Where(c => c.MpID == MpID && c.Length == 2).Select(c => new { text = c.Name, value = c.ID });
                ge = JsonHelper.ToJson(groupenum);
                CacheHelper.Set(string.Format("WxGroupEnum{0}", MpID), ge);
            }
            ViewBag.groupenum = ge;
            return base.List();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var groupid = GetQueryString("GroupID");
            if (!string.IsNullOrEmpty(groupid))
            {
                var result = entities.Set<MpFans>().Where(c => c.MpID == mpid && c.GroupID == groupid && c.IsFans=="1").WhereToGridData(qb);
                return Json(result);
            }
            else
            {
                var result = entities.Set<MpFans>().Where(c => c.MpID == mpid && c.IsFans == "1").WhereToGridData(qb);
                return Json(result);
            }
        }

        public override JsonResult Save()
        {
            #region 微信处理
            MpFans entity = UpdateEntity<MpFans>();
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            wxFO.UpdateFans(entity.MpID, entity.OpenID, entity.Remark);
            #endregion

            #region 保存数据
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public JsonResult MoveGroup()
        {
            string ID = Request["ID"];
            string GroupID = Request["GroupID"];
            var fans = entities.Set<MpFans>().Where(c => c.ID==ID).FirstOrDefault();
            if(fans==null)
                Json(new { message="找不到粉丝"});
            var group = entities.Set<MpGroup>().Where(c => c.ID == GroupID).FirstOrDefault();
            if (group == null)
                Json(new { message = "找不到分组" });

            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            try
            {
                wxFO.MoveGroup(fans.MpID, fans.OpenID, group.WxGroupID.Value);
            }
            catch (Exception ex)
            {
                Json(new { message = ex.Message });
            }

            fans.GroupID = GroupID;
            entities.SaveChanges();
            return Json(new { message = "" });
        }

        public JsonResult UpdataAllFans()
        {
            string MpID = Request["MpID"];
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            try
            {
                wxFO.GetGroup(MpID);
                wxFO.RefreshFans(MpID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { message = "success" });
        }

        public JsonResult GetCountryEnum()
        {
            var MpID = GetQueryString("MpID");
            var result = entities.Set<MpFans>().Where(c => c.MpID == MpID && !string.IsNullOrEmpty(c.Country)).Select(c => new { ID = c.Country, Name = c.Country }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProvinceEnum()
        {
            var MpID = GetQueryString("MpID");
            var Country = GetQueryString("Country");
            var result = entities.Set<MpFans>().Where(c => c.MpID == MpID && c.Country == Country && !string.IsNullOrEmpty(c.Province)).Select(c => new { ID = c.Province, Name = c.Province }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityEnum()
        {
            var MpID = GetQueryString("MpID");
            var Country = GetQueryString("Country");
            var Province = GetQueryString("Province");
            var result = entities.Set<MpFans>().Where(c => c.MpID == MpID && c.Country == Country && c.Province == Province && !string.IsNullOrEmpty(c.City)).Select(c => new { ID = c.City, Name = c.City }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
