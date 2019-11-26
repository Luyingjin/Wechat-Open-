using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using System.Web.Security;
using Formula.Exceptions;

namespace Base.Areas.Auth.Controllers
{
    public class SecurityController : BaseController
    {

        public JsonResult ChangePwd(string pwd, string oldPwd)
        {
            var entity = entities.Set<S_A_Security>().SingleOrDefault(c => c.SuperAdmin == "administrator");
            if (entity == null)
                throw new BusinessException("超级管理员administrator不存在");

            string security = GetSuperAdminStr(entity);
            security = FormsAuthentication.HashPasswordForStoringInConfigFile(security, "SHA1");
            if (entity.SuperAdminSecurity != security)
                throw new BusinessException("超级管理员密码已经被恶意修改！");

            if (entity.SuperAdminPwd != FormsAuthentication.HashPasswordForStoringInConfigFile(oldPwd, "SHA1"))
                throw new BusinessException("旧密码输入不正确");

            string newPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "SHA1");

            //没有变化则不修改
            if (entity.SuperAdminPwd == newPwd)
                return Json("");
            entity.SuperAdminPwd = newPwd;
            entity.SuperAdminModifyTime = DateTime.Now;

            security = GetSuperAdminStr(entity);
            security = FormsAuthentication.HashPasswordForStoringInConfigFile(security, "SHA1");
            entity.SuperAdminSecurity = security;
            entities.SaveChanges();

            return Json("");
        }

        public JsonResult GetAdmin()
        {
            var entity = entities.Set<S_A_Security>().SingleOrDefault(c => c.SuperAdmin == "administrator");
            if (entity == null)
                throw new BusinessException("超级管理员administrator不存在");
            string security = GetAdminStr(entity);
            security = FormsAuthentication.HashPasswordForStoringInConfigFile(security, "SHA1");
            if (entity.AdminSecurity != security)
                throw new BusinessException("管理员信息已经被恶意修改！");

            return Json(entity);
        }

        public JsonResult SaveAdmin(string ids, string names,string pwd)
        {
            var entity = entities.Set<S_A_Security>().SingleOrDefault(c => c.SuperAdmin == "administrator");
            if (entity == null)
                throw new BusinessException("超级管理员administrator不存在");

            string security = GetAdminStr(entity);
            security = FormsAuthentication.HashPasswordForStoringInConfigFile(security, "SHA1");
            if (entity.AdminSecurity != security)
                throw new BusinessException("管理员信息已经被恶意修改！");

            security = GetSuperAdminStr(entity);
            security = FormsAuthentication.HashPasswordForStoringInConfigFile(security, "SHA1");
            if(entity.SuperAdminSecurity!=security)
                throw new BusinessException("超级管理员密码已经被恶意修改！");

            if (entity.SuperAdminPwd != FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "SHA1"))
                throw new BusinessException("超级管理员密码错误！");

            //没有变化则不修改
            if (entity.AdminIDs == ids && entity.AdminNames == names)
                return Json("");

            entity.AdminIDs = ids;
            entity.AdminNames = names;
            entity.AdminModifyTime = DateTime.Now;

            security = GetAdminStr(entity);
            security = FormsAuthentication.HashPasswordForStoringInConfigFile(security, "SHA1");
            entity.AdminSecurity = security;
            entities.SaveChanges();

            return Json("");
        }


        #region 私有方法

        private string GetAdminStr(S_A_Security security)
        {
            string str = "{0},{1},{2}";

            return string.Format(str, security.AdminIDs, security.AdminNames, security.AdminModifyTime);
        }

        private string GetSuperAdminStr(S_A_Security security)
        {
            string str = "{0},{1},{2}";
            return string.Format(str, security.SuperAdmin, security.SuperAdminPwd, security.SuperAdminModifyTime);

        }

        #endregion
    }
}
