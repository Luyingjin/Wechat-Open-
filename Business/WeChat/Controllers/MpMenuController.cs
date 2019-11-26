using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Exceptions;
using WeChat.Logic.BusinessFacade;

namespace WeChat.Controllers
{
    public class MpMenuController : BaseController<MpMenu>
    {
        public override ActionResult Tree()
        {
            var MpID = GetQueryString("MpID");
            var exist = entities.Set<MpMenu>().Where(c => c.MpID == MpID).Any();
            if (!exist)
            {
                //初始化
                var node1 = GetEntity<MpMenu>("");
                node1.MpID = MpID;
                node1.ParentID = "";
                node1.Name = "全部";
                node1.IsDelete = 0;
                node1.FullPath = node1.ID;
                node1.Length = 1;
                node1.ChildCount = 0;
                node1.SortIndex = 1;
                entities.Set<MpMenu>().Add(node1);
                entities.SaveChanges();
            }
            return base.Tree();
        }

        public override JsonResult GetTree()
        {
            var MpID = GetQueryString("MpID");
            var query = entities.Set<MpMenu>().Where(c => c.MpID == MpID && c.IsDelete == 0).OrderBy(i => i.SortIndex).ThenBy(i => i.ID);
            return Json(query.ToList());
        }

        public override JsonResult GetModel(string id)
        {
            var pId = Request["ParentID"];
            var model = GetEntity<MpMenu>(id);
            if (!String.IsNullOrWhiteSpace(pId))
            {
                var parentEntity = GetEntity<MpMenu>(pId);
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
            var entity = UpdateEntity<MpMenu>();
            var MpID = GetQueryString("MpID");
            var exist = entities.Set<MpMenu>().Any(i => i.ID != entity.ID && i.MpID == MpID && i.IsDelete == 0 && i.Name == entity.Name.Trim()&&i.ParentID==entity.ParentID);
            if (exist)
            {
                throw new BusinessException(string.Format("节点编号[{0}]已存在，请重新输入！", entity.Name.Trim()));
            }

            var funcType = GetQueryString("FuncType");
            if (funcType.ToLower() == "insert")
            {
                entity.FullPath = entity.FullPath + "." + entity.ID;
                var FileType = GetQueryString("FileType");
                entity.MpID = GetQueryString("MpID");
                entity.Length = entity.Length + 1;
                entity.ChildCount = 0;
                entity.SortIndex = entities.Set<MpMenu>().Count(i => i.MpID == MpID && i.IsDelete == 0 && i.ParentID == entity.ParentID) + 1;
                entity.IsDelete = 0;
                var parentEntity = GetEntity<MpMenu>(entity.ParentID);
                parentEntity.ChildCount = entity.SortIndex;
                //entity.MenuKey = parentEntity.Length.ToString() + "_" + parentEntity.SortIndex.ToString() + "_" + entity.Length.ToString() + "_" + entity.SortIndex.ToString();
                entity.MenuKey = DateTime.Now.Ticks.ToString();

            }

            entities.SaveChanges();
            SaveMenuKey(MpID);
            return Json(new { ID = entity.ID });
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        public override JsonResult DeleteNode()
        {
            string id = GetQueryString("ID");
            //删除节点
            var deletenodes = entities.Set<MpMenu>().Where(i => i.ID == id).ToList();
            for (int i = 0; i < deletenodes.Count; i++)
                deletenodes[i].IsDelete = 1;
            entities.SaveChanges();
            //重新计算父节点的ChildCount
            var parentids = deletenodes.Select(c => c.ParentID).Distinct().ToList();
            foreach (var pid in parentids)
            {
                var parentEntity = GetEntity<MpMenu>(pid);
                parentEntity.ChildCount = entities.Set<MpMenu>().Count(i => i.MpID == parentEntity.MpID && i.IsDelete == 0 && i.ParentID == pid);
            }
            entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 同步菜单
        /// </summary>
        public JsonResult SyncNode()
        {
            string mpid = GetQueryString("MpID");

            var wxFO = Formula.FormulaHelper.CreateFO<WxFO>();
            wxFO.SaveMenu(mpid);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取子节点数量
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetChildCount(string ID)
        {
            var count = entities.Set<MpMenu>().Count(i => i.ParentID == ID && i.IsDelete == 0);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        private void SaveMenuKey(string MpID)
        {
            var treenodes = entities.Set<MpMenu>().Where(c => c.MpID == MpID && c.IsDelete == 0).ToList();//Dataset
            var level1nodes = treenodes.Where(c => c.Length == 2&& c.IsDelete == 0).OrderBy(c => c.SortIndex).ThenBy(c => c.ID).ToList();
            for(int i=0;i<level1nodes.Count();i++)
            {
                //更新第一层菜单的SortIndex
                level1nodes[i].SortIndex = i;
                var level2nodes = treenodes.Where(c => c.ParentID == level1nodes[i].ID && c.IsDelete == 0).OrderBy(c => c.SortIndex).ThenBy(c => c.ID).ToList();
                for (int j = 0; j < level2nodes.Count(); j++)
                {
                    //更新第二层菜单的SortIndex
                    level2nodes[j].SortIndex = j;

                }
            }
            entities.SaveChanges();
        }
    }
}
