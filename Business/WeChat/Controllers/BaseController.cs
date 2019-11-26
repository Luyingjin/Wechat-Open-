using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using System.Data.Entity;
using WeChat.Logic.Domain;
using Config;

namespace WeChat.Controllers
{
    public class BaseController : MvcAdapter.BaseController
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<WeChatEntities>();
                }
                return _entities;
            }
        }

        UserInfo _userInfo;
        protected UserInfo CurrentUserInfo
        {
            get
            {
                if (_userInfo == null)
                    _userInfo = FormulaHelper.GetUserInfo();
                return _userInfo;
            }
        }
    }

    public class BaseController<T> : MvcAdapter.BaseController<T> where T : class, new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<WeChatEntities>();
                }
                return _entities;
            }
        }
    }

    public class BaseApiController<T> : MvcAdapter.BaseApiController<T> where T : class, new()
    {
        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<WeChatEntities>();
                }
                return _entities;
            }
        }
    }
}
