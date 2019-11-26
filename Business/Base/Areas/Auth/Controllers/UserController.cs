using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Base.Logic.Domain;
using Config;
using System.Data;
using Formula.Helper;
using Base.Logic.BusinessFacade;
using Formula;
using System.Web.Security;
using System.Text;
using Newtonsoft.Json;
using Aspose.Words.Reporting;
using Formula.Exceptions;
using Base.Logic;

namespace Base.Areas.Auth.Controllers
{
    public class UserController : BaseController<S_A_User>
    {
        #region Excel 批量导入
        //public JsonResult VaildExcelDataUser()
        //{
        //    var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
        //    string data = reader.ReadToEnd();
        //    var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        //    var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

        //    var orgs = entities.Set<S_A_Org>().ToList();
        //    var errors = excelData.Vaildate(e =>
        //    {
        //        if (e.FieldName == "DeptName" && !string.IsNullOrWhiteSpace(e.Value))
        //        {
        //            var dept = orgs.FirstOrDefault(o => o.Name == e.Value);
        //            if (dept == null)
        //            {
        //                e.IsValid = false;
        //                e.ErrorText = string.Format("部门（{0}）不存在！", e.Value);
        //            }
        //        }
        //    });

        //    return Json(errors);
        //}

        //public JsonResult BatchSaveUser()
        //{
        //    var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
        //    string data = reader.ReadToEnd();
        //    var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        //    var list = JsonConvert.DeserializeObject<List<S_A_User>>(tempdata["data"]);

        //    entities.Configuration.AutoDetectChangesEnabled = false;
        //    var oldUsers = entities.Set<S_A_User>().ToList();
        //    var orgs = entities.Set<S_A_Org>().ToList();
        //    foreach (var user in list)
        //    {
        //        var newUser = oldUsers.FirstOrDefault(u => u.Code == user.Code);
        //        if (newUser != null)
        //        {
        //            newUser.Name = user.Name;
        //            newUser.Sex = user.Sex;
        //            newUser.WorkNo = user.WorkNo;
        //            newUser.Phone = user.Phone;
        //            newUser.MobilePhone = user.MobilePhone;
        //            newUser.Email = user.Email;
        //        }
        //        else
        //        {
        //            newUser = new S_A_User();
        //            newUser.ID = FormulaHelper.CreateGuid();
        //            newUser.Code = user.Code;
        //            newUser.Name = user.Name;
        //            newUser.Sex = user.Sex;
        //            newUser.WorkNo = user.WorkNo;
        //            newUser.Phone = user.Phone;
        //            newUser.MobilePhone = user.MobilePhone;
        //            newUser.Email = user.Email;
        //            newUser.DeptID = "";
        //            newUser.GroupID = "a1b10168-61a9-44b5-92ca-c5659456deb5";
        //            entities.Set<S_A_User>().Add(newUser);
        //        }

        //        if (!string.IsNullOrWhiteSpace(user.DeptName))
        //        {
        //            var dept = orgs.FirstOrDefault(o => o.Name == user.DeptName);
        //            newUser.DeptID = dept.ID;
        //            newUser.DeptFullID = dept.FullID;
        //            newUser.DeptName = dept.Name;
        //        }

        //        if (string.IsNullOrEmpty(newUser.Password))
        //            newUser.Password = newUser.Code.GetHashCode().ToString();
        //        if (newUser.S_A__OrgUser.Count == 0)
        //            newUser.S_A__OrgUser.Add(new S_A__OrgUser() { UserID = newUser.ID, OrgID = Config.Constant.OrgRootID });

        //    }
        //    try
        //    {
        //        entities.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.Error(ex);
        //        throw ex;
        //    }
        //    entities.Configuration.AutoDetectChangesEnabled = true;
        //    return Json("Success");
        //}

        //public ActionResult ExportWord(string id)
        //{
        //    var key = "user";
        //    var wordExporter = new AsposeWordExporter();

        //    var ds = new DataSet();
        //    var user = entities.Set<S_A_User>().Find(id);
        //    ds.AddList(user, "User");

        //    var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
        //    var tmplPath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, key) : string.Format("{0}\\{1}.doc", path, key);

        //    var buffer = wordExporter.ExportWord(ds, tmplPath);

        //    if (buffer != null)
        //    {
        //        return File(buffer, "application/vnd.ms-word", Url.Encode("用户信息") + ".doc");
        //    }

        //    return Content("导出数据失败，请检查相关配置！");
        //}
        #endregion

        public override JsonResult GetModel(string id)
        {
            AuthFO authFO = FormulaHelper.CreateFO<AuthFO>();
            var entity = GetEntity<S_A_User>(id);
            string deptNames = authFO.GetUserDeptNames(id);
            var dic = FormulaHelper.ModelToDic(entity);
            dic.Add("DeptNames", deptNames);
            return Json(dic);
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            string sql = "select * from S_A_User where IsDeleted='0' or IsDeleted is null";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            dt.Columns.Add("DeptNames");
            AuthFO authFO = FormulaHelper.CreateFO<AuthFO>();
            foreach (DataRow row in dt.Rows)
            {
                row["DeptNames"] = authFO.GetUserDeptNames(row["ID"].ToString());
            }

            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetRetiredList(QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            string sql = "select * from S_A_User where IsDeleted='1'";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            dt.Columns.Add("DeptNames");
            AuthFO authFO = FormulaHelper.CreateFO<AuthFO>();
            foreach (DataRow row in dt.Rows)
            {
                row["DeptNames"] = authFO.GetUserDeptNames(row["ID"].ToString());
            }

            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public override JsonResult Save()
        {
            var user = base.UpdateEntity<S_A_User>();

            if (entities.Set<S_A_User>().Count(c => c.Code == user.Code && c.ID != c.ID) > 0)
                throw new Exception("用户编号不能重复");

            if (!string.IsNullOrEmpty(user.RTX))
            {
                if (entities.Set<S_A_User>().Count(c => c.RTX == user.RTX && c.ID != user.ID) > 0)
                    throw new Exception("RTX帐号不能重复");
            }
            user.Code = user.Code.Trim();
            if (string.IsNullOrEmpty(user.Password))
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Code.ToLower(), "SHA1");
            if (user.S_A__OrgUser.Count == 0)
                user.S_A__OrgUser.Add(new S_A__OrgUser() { UserID = user.ID, OrgID = Config.Constant.OrgRootID });

            return base.JsonSave<S_A_User>(user);
        }

        public JsonResult Reset(string UserIDs)
        {
            var arr = UserIDs.Split(',');
            var users = entities.Set<S_A_User>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (var user in users)
            {
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", user.Code.Trim().ToLower(), Request["pwd"]), "SHA1");
            }
            //记录安全审计日志
            AuthFO.Log("修改密码", string.Join(",", users.Select(c => c.Name)), Request["pwd"]);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ResetAllUserPwd(string pwd)
        {

            var userList = entities.Set<S_A_User>().OrderBy(c => c.ID).Take(1000);

            int index = 1;
            while (userList.Count() > 0)
            {
                foreach (var user in userList)
                {
                    user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", user.Code.Trim().ToLower(), pwd), "SHA1");
                }
                entities.SaveChanges();
                userList = entities.Set<S_A_User>().OrderBy(c => c.ID).Skip(index * 1000).Take(1000);
                index++;
            }
            //记录安全审计日志
            AuthFO.Log("修改密码", "全部用户", pwd);
            return Json("");
        }

        public JsonResult Unlock(string UserIDs)
        {
            var ids = UserIDs.Split(',');
            var users = entities.Set<S_A_User>().Where(c => ids.Contains(c.ID)).ToList();
            foreach (var user in users)
            {
                user.ErrorCount = 0;
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult RetireUser(string UserIDs)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            foreach (var id in UserIDs.Split(','))
            {
                authBF.RetireUser(id);
            }
            return Json("");
        }

        public ActionResult RetireList()
        {
            return View();
        }

        public JsonResult UploadImg(string UserID, bool isPortrait = false)
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));

                S_A_UserImg sa = entities.Set<S_A_UserImg>().Where(c => c.UserID == UserID).SingleOrDefault();
                if (sa == null)
                {
                    sa = new S_A_UserImg() { ID = FormulaHelper.CreateGuid(), UserID = UserID };
                    entities.Set<S_A_UserImg>().Add(sa);
                }
                if (isPortrait)
                    sa.Picture = bt;
                else
                    sa.SignImg = bt;
                entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult FreeSign(string UserID, string imgType = "Sign")
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            S_A_UserImg sa = entities.Set<S_A_UserImg>().Where(c => c.UserID == UserID).SingleOrDefault();
            if (sa != null)
            {
                if (imgType == "Sign")
                    sa.SignImg = null;
                else
                    sa.Picture = null;
                entities.SaveChanges();
            }
            return Json(new { ImgType = imgType });
        }

        #region 权限一览表

        public JsonResult GetResList(QueryBuilder qb)
        {
            string rootID = Config.Constant.MenuRooID;
            if (!string.IsNullOrEmpty(Request["IsRuleView"]))
                rootID = Config.Constant.RuleRootID;


            var users = entities.Set<S_A_User>().Where(qb);

            var resList = entities.Set<S_A_Res>().Where(c => c.FullID.StartsWith(rootID)).ToList();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            string sql = "select * from S_A_User where IsDeleted='0'";
            DataTable dtUser = sqlHelper.ExecuteDataTable(sql, qb);
            string userIDs = string.Join("','", dtUser.AsEnumerable().Select(c => c.Field<string>("ID")));

            //Oracle数据库字段名长度最大为30，Oracle的substr函数比C#的多1

            sql = @"
select * from
(
select S_A_User.DeptName, S_A_User.Name,S_A_User.Code,S_A_User.WorkNo,S_A_User.ID as UserID,'a'+ {2} as ID from S_A_User  join 
(
--组织权限
select UserID, S_A_Res.* from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
union
--系统角色权限
select UserID,S_A_Res.* from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
union
--组织角色权限
select UserID,S_A_Res.* from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
) tb1 on S_A_User.ID in('{1}') and tb1.UserID = S_A_User.ID 
) a pivot (count(UserID) for ID in({0}))b
";

            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format(sql, "'" + string.Join("','"
                    , resList.Select(c => "a" + c.ID.Replace('-', '_').Substring(10)).Distinct().ToArray()) + "'"
                    , userIDs
                    , "SUBSTR(replace(tb1.ID,'-','_'),11)"
                   );

                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                foreach (DataColumn col in dt.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim('\'');
                }
                GridData data = new GridData(dt);
                data.total = qb.TotolCount;

                return Json(data);
            }
            else
            {
                sql = string.Format(sql
                    , string.Join(",", resList.Select(c => "a" + c.ID.Replace('-', '_').Substring(10)).Distinct().ToArray())
                    , userIDs
                    , "SUBSTRING(replace(tb1.ID,'-','_'),11,26)"
                   );

                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                GridData data = new GridData(dt);
                data.total = qb.TotolCount;

                return Json(data);
            }



        }

        private string getColHtml()
        {
            string rootID = Config.Constant.MenuRooID;
            if (!string.IsNullOrEmpty(Request["IsRuleView"]))
                rootID = Config.Constant.RuleRootID;

            var resList = entities.Set<S_A_Res>().Where(c => c.FullID.StartsWith(rootID)).ToList();
            StringBuilder sb = new StringBuilder();

            var rootRes = resList.Where(c => string.IsNullOrEmpty(c.ParentID)).FirstOrDefault();

            foreach (var res in resList.Where(c => c.ParentID == rootRes.ID))
            {
                sb.AppendFormat("<div field='{0}'>{1} {2}</div>", "a" + res.ID.Replace('-', '_').Substring(10), res.Name, GetColHtml(res.ID, resList));
            }
            return sb.ToString();
        }

        private string GetColHtml(string parentResID, List<S_A_Res> resList)
        {

            var childRes = resList.Where(c => c.ParentID == parentResID);
            if (childRes.Count() == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<div property='columns'>");
            foreach (var item in resList.Where(c => c.ParentID == parentResID))
            {
                string childCol = GetColHtml(item.ID, resList);
                string colName = item.Name;
                string width = "";
                if (childCol == "")
                {
                    colName = string.Join("<br/>", colName.ToArray());
                    width = "width='20'";
                }
                sb.AppendFormat("<div field='{0}' {3}>{1} {2}</div>", "a" + item.ID.Replace('-', '_').Substring(10), colName, childCol, width);
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        public ViewResult ResView()
        {
            ViewBag.Fields = getColHtml();
            return View();
        }

        public JsonResult GetResFrom(string UserCode, string resID)
        {
            resID = resID.Substring(1).Replace('_', '-');

            resID = entities.Set<S_A_Res>().SingleOrDefault(c => c.ID.EndsWith(resID)).ID;

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select ID from S_A_User where Code='{0}'", UserCode);
            string userID = sqlHelper.ExecuteScalar(sql).ToString();


            sql = @"

select ID,Name from S_A__OrgRes
join S_A__OrgUser on ResID='{1}' and UserID='{0}' and S_A__OrgRes.OrgID=S_A__OrgUser.OrgID
join S_A_Org on S_A_Org.ID=S_A__OrgRes.OrgID
union
select ID,Name from S_A__RoleRes
join S_A__RoleUser on ResID='{1}' and UserID='{0}' and S_A__RoleRes.RoleID=S_A__RoleUser.RoleID
join S_A_Role on S_A_Role.ID=S_A__RoleRes.RoleID
union
select ID,Name from S_A__RoleRes
join S_A__OrgRole on ResID='{1}' and S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A__OrgUser on UserID='{0}' and S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__RoleRes.RoleID

";

            sql = string.Format(sql, userID, resID);



            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            string result = string.Join("\n", dt.AsEnumerable().Select(c => c.Field<string>("Name")).ToArray());

            return Json(new { userName = result });
        }

        #endregion

        #region 用户授权

        public JsonResult GetUserRes(string nodeID)
        {
            return base.JsonGetRelationAll<S_A_User, S_A__UserRes, S_A_Res>(nodeID);
        }

        public JsonResult SetUserRes(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__UserRes>().Where(c => c.UserID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));
            return base.JsonSetRelation<S_A_User, S_A__UserRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        public JsonResult SetUserRule(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__UserRes>().Where(c => c.UserID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));
            return base.JsonSetRelation<S_A_User, S_A__UserRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        #endregion
    }
}
