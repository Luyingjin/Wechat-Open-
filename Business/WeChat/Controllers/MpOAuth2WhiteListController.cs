using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Exceptions;
using WeChat.Logic.Domain;

namespace WeChat.Controllers
{
    public class MpOAuth2WhiteListController : BaseController<MpOAuth2WhiteList>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var mpid = GetQueryString("MpID");
            var result = entities.Set<MpOAuth2WhiteList>().Where(c =>c.MpID == mpid && c.IsDelete == 0).WhereToGridData(qb);
            return Json(result);
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            #region 数据校验
            MpOAuth2WhiteList entity = UpdateEntity<MpOAuth2WhiteList>();
            if (string.IsNullOrEmpty(entity.Domain))
                throw new BusinessException("Domain不能为空");
            #endregion

            #region 关键字校验唯一性
            var keyword = entity.Domain;
            var MpID = GetQueryString("MpID");
            var exist = entities.Set<MpOAuth2WhiteList>().Any(i => i.ID != entity.ID && i.MpID == MpID && i.IsDelete == 0 && i.Domain == entity.Domain.Trim());
            if (exist)
            {
                throw new BusinessException(string.Format("Domain[{0}]已存在，请重新输入！", entity.Domain.Trim()));
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
            var entity = GetEntity<MpOAuth2WhiteList>(ID);
            #endregion

            #region 假删除
            entity.IsDelete = 1;
            entities.SaveChanges();
            return Json("");
            #endregion
        }
        

    }
}
