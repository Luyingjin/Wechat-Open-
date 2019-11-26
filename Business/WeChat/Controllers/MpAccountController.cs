using Formula;
using Formula.Exceptions;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using WeChat.Logic;

namespace WeChat.Controllers
{
    public class MpAccountController : BaseController<MpAccount>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var result = entities.Set<MpAccount>().Where(c => c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        public override JsonResult Save()
        {

            #region 公众号唯一性校验
            MpAccount entity = UpdateEntity<MpAccount>();
            var exists = entities.Set<MpAccount>().Where(c => (c.Name == entity.Name || c.AppID == entity.AppID) && c.IsDelete == 0);
            string id = GetQueryString("ID");
            if (!string.IsNullOrEmpty(id))
                exists = exists.Where(c => c.ID != id);
            if (exists.Count() > 0)
            {
                var obj = exists.First();
                if (obj.Name == entity.Name)
                    throw new BusinessException(string.Format("系统中已存在名称为“{0}”的公众号", entity.Name));
                else
                    throw new BusinessException(string.Format("系统中已存在AppID为“{0}”的公众号", entity.AppID));
            }
            #endregion

            #region 保存数据
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {
            #region 假删除
            string ID = Request["ListIDs"];
            var entity = GetEntity<MpAccount>(ID);
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }

        public JsonResult SetRoleUser(string mpid, string relationData)
        {
            string[] arrRelateID = GetValues(relationData, "ID").Distinct().Where(c=>!string.IsNullOrEmpty(c)).ToArray();
            var originalList = entities.Set<MpAccountUserRelation>().Where(c => c.MpID == mpid).ToArray();
            //新增的用户
            var addlist = arrRelateID.Where(c => !originalList.Select(d => d.UserID).Contains(c));
            //需要删除的用户
            var dellist = originalList.Where(c => !arrRelateID.Contains(c.UserID));
            foreach (var item in dellist)
                entities.Set<MpAccountUserRelation>().Remove(item);
            foreach (var id in addlist)
            {
                var model = new MpAccountUserRelation();
                model.ID = FormulaHelper.CreateGuid();
                model.MpID = mpid;
                model.UserID = id;
                model.IsUsed = SysBool.F.ToString();
                model.IsDefault = SysBool.F.ToString();
                entities.Set<MpAccountUserRelation>().Add(model);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetRoleUser(string mpid)
        {
            var result = entities.Set<MpAccountUserRelation>().Where(c => c.MpID == mpid);
            return Json(result);
        }
    }
}
