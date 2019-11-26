using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using WeChat.Logic.BusinessFacade;
using Formula.Exceptions;
using Formula.Helper;

namespace WeChat.Controllers
{
    public class MpGroupController : BaseController<MpGroup>
    {
        public override ActionResult Tree()
        {
            var MpID = GetQueryString("MpID");
            var exist = entities.Set<MpGroup>().Where(c => c.MpID == MpID).Any();
            if (!exist)
            {
                var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
                wxFO.GetGroup(MpID);
                wxFO.RefreshFans(MpID);
            }
            var ge = CacheHelper.Get(string.Format("WxGroupEnum{0}", MpID));
            if (ge == null)
            {
                var groupenum = entities.Set<MpGroup>().Where(c => c.MpID == MpID && c.Length == 2).Select(c => new { text = c.Name, value = c.ID });
                CacheHelper.Set(string.Format("WxGroupEnum{0}", MpID), JsonHelper.ToJson(groupenum));
                ge = CacheHelper.Get(string.Format("WxGroupEnum{0}", MpID));
            }
            TempData["groupEnum"] = ge;
            return base.Tree();
        }

        public override JsonResult GetTree()
        {
            var MpID = GetQueryString("MpID");
            var query = entities.Set<MpGroup>().Where(c => c.MpID == MpID).OrderBy(i => i.WxGroupID).ThenBy(i => i.ID);
            return Json(query.ToList(),JsonRequestBehavior.AllowGet);
        }

        public override JsonResult GetModel(string id)
        {
            var pId = Request["ParentID"];
            var model = GetEntity<MpGroup>(id);
            if (!String.IsNullOrWhiteSpace(pId))
            {
                var parentEntity = GetEntity<MpGroup>(pId);
                model.FullPath = parentEntity.FullPath;
                model.Length = parentEntity.Length;
            }
            return Json(model);
        }

        /// <summary>
        /// 保存管理类小类
        /// </summary>
        [ValidateInput(false)]
        public override JsonResult Save()
        {
            var entity = UpdateEntity<MpGroup>();
            var MpID = GetQueryString("MpID");
            var exist = entities.Set<MpGroup>().Any(i => i.ID != entity.ID && i.MpID == MpID && i.Name == entity.Name.Trim());
            if (exist)
            {
                throw new BusinessException(string.Format("分组名称[{0}]已存在，请重新输入！", entity.Name.Trim()));
            }

            var funcType = GetQueryString("FuncType");
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            if (funcType.ToLower() == "insert")
            {
                entity.FullPath = entity.FullPath + "." + entity.ID;
                entity.MpID = GetQueryString("MpID");
                entity.Length = entity.Length + 1;
                entity.ChildCount = 0;
                var group = wxFO.AddGroup(entity.MpID, entity.Name);
                entity.WxGroupID = group.id;
            }
            else
            {
                wxFO.UpdateGroup(entity.MpID, entity.WxGroupID ?? -1, entity.Name);
            }

            entities.SaveChanges();
            CacheHelper.Remove(string.Format("WxGroupEnum{0}", MpID));
            return Json(new { ID = entity.ID });
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        public override JsonResult DeleteNode()
        {
            string id = GetQueryString("ID");
            var deletenode = entities.Set<MpGroup>().Where(i => i.ID == id).FirstOrDefault();
            #region 删除微信分组
            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            wxFO.DeleteGroup(deletenode.MpID, deletenode.WxGroupID ?? -1);
            #endregion

            #region 将分组下的粉丝移动到未分组
            var notgroup = entities.Set<MpGroup>().Where(c => c.MpID == deletenode.MpID && c.Name == "未分组").FirstOrDefault();
            if (notgroup == null)
                throw new Exception("找不到未分组节点");
            //删除节点
            for (int i = 0; i < deletenode.MpFans.Count(); i++)
                deletenode.MpFans.ElementAt(i).GroupID = notgroup.ID;
            #endregion

            #region 更新父节点
            var parentEntity = GetEntity<MpGroup>(deletenode.ParentID);
            parentEntity.ChildCount = entities.Set<MpGroup>().Count(i => i.MpID == parentEntity.MpID && i.ParentID == deletenode.ParentID);
            #endregion
            entities.Set<MpGroup>().Remove(deletenode);
            entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
