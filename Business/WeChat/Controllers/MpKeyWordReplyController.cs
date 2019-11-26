using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Logic.Domain;
using Formula.Exceptions;
using Config;
using System.Data;

namespace WeChat.Controllers
{
    public class MpKeyWordReplyController : BaseController<MpKeyWordReply>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpKeyWordReply>().Where(c => c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            #region 数据校验
            MpKeyWordReply entity = UpdateEntity<MpKeyWordReply>();
            if (string.IsNullOrEmpty(entity.KeyWord))
                throw new BusinessException("关键字不能为空");
            #endregion 
            
            #region 关键字校验唯一性
            var keyword = entity.KeyWord;
            var MpID = GetQueryString("MpID");
            var exist = entities.Set<MpKeyWordReply>().Any(i => i.ID != entity.ID && i.MpID == MpID && i.IsDelete == 0 && i.KeyWord == entity.KeyWord.Trim());
            if (exist)
            {
                throw new BusinessException(string.Format("关键字[{0}]已存在，请重新输入！", entity.KeyWord.Trim()));
            }
            //SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WeChat);
            //DataTable dt = sqlHelper.ExecuteDataTable(string.Format("SELECT * FROM MpKeyWordReply WHERE KeyWord='{0}' and IsDelete='0'", keyword));
            //if (dt.Rows.Count>0)
            //    throw new BusinessException("关键字不能重复");
            #endregion
            #region 保存数据
            if (entity.IsDelete == null)
                entity.IsDelete = 0;
            if (string.IsNullOrEmpty(entity.MpID))
                entity.MpID = GetQueryString("MpID");
            entities.SaveChanges();

            return Json(new { ID = entity.ID });
            #endregion
        }

        public override JsonResult Delete()
        {

            #region 微信处理
            string ID = Request["ListIDs"];
            //var entity = GetEntity<MpMediaArticle>(ID);
            var entity = GetEntity<MpKeyWordReply>(ID);
            #endregion

            #region 假删除
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }
    }
}
