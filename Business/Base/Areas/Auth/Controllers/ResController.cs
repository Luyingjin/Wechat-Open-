using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Base.Logic;
using Formula.Exceptions;
using Formula.Helper;

namespace Base.Areas.Auth.Controllers
{
    public class ResController : BaseController<S_A_Res>
    {
        public override JsonResult SaveNode()
        {
            var entity = UpdateNode<S_A_Res>();

            if (entity.Type == ResType.Data.ToString())
            {
                string data = ResType.Data.ToString();
                if (entities.Set<S_A_Res>().Where(c => c.Url == entity.Url && c.Type == data && c.DataFilter == entity.DataFilter && c.ID != entity.ID).Count() > 0)
                    throw new BusinessException("不能增加重复的数据权限");
            }

            if (entity.Type == ResType.Button.ToString())
            {
                string button = ResType.Button.ToString();
                if (entities.Set<S_A_Res>().Where(c => c.Url == entity.Url && c.Type == button && c.ButtonID == entity.ButtonID && c.ID != entity.ID).Count() > 0)
                    throw new BusinessException("不能增加重复的按钮权限");
            }

            return base.JsonSaveNode<S_A_Res>(entity);
        }

        public JsonResult GetRule(string id)
        {
            return JsonGetModel<S_A_Res>(id);
        }

    }
}
