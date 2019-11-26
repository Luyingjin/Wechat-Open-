using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.ComponentModel;

namespace Config
{
    public class Constant
    {
        public static bool IsOracleDb
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["IsOracleDb"] == "True";
            }
        }

        /// <summary>
        /// 入口树根节点ID
        /// </summary>
        public readonly static string MenuRooID = "a1b10168-61a9-44b5-92ca-c5659456deb5";
        /// <summary>
        /// 组织树根节点ID
        /// </summary>
        public readonly static string OrgRootID = "a1b10168-61a9-44b5-92ca-c5659456deb5";
        /// <summary>
        /// 用户组ID
        /// </summary>
        public readonly static string UserGroupID = "a1b10168-61a9-44b5-92ca-c5659456deb5";

        /// <summary>
        /// 角色组ID
        /// </summary>
        public readonly static string RoleGroupID = "a1b10168-61a9-44b5-92ca-c5659456deb5";

        /// <summary>
        /// 规则树根节点ID
        /// </summary>
        public readonly static string RuleRootID = "a1b10168-61a9-44b5-92ca-c5659456deb6";

        /// <summary>
        /// 系统管理菜单ID
        /// </summary>
        public readonly static string SystemMenuFullID = "a1b10168-61a9-44b5-92ca-c5659456deb5.a1b400f2-a325-4124-a2e0-d860653c760b";


        /// <summary>
        /// 授权菜单ID，分级授权使用
        /// </summary>
        private static string _AuthrizeMenuFullID = null;
        public static string AuthrizeMenuFullID
        {
            get
            {
                if (_AuthrizeMenuFullID == null)
                {
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                    string sql = "select FullID from S_A_Res where ID='a2a2011e-3d38-40cb-9db1-3057ca6ee633'";
                    _AuthrizeMenuFullID = sqlHelper.ExecuteScalar(sql).ToString();
                }
                return _AuthrizeMenuFullID;
            }
        }
        //数据添加记录日志
        public static string[] DataAddLog = { "Base.S_A_User", "Base.S_A_Org" };
        //数据修改记录日志
        public static string[] DataModifyLog = { "Base.S_A_User", "Base.S_A_Org" };
        //数据删除记录日志
        public static string[] DataDeleteLog = { "Base.S_A_User", "Base.S_A_Org", "Project.S_W_WBS" };
    }


    //数据库连接字符串，对应web.config中的字符串key
    public enum ConnEnum
    {
        [Description("Base")]
        Base,
        [Description("WeChat")]
        WeChat,
        [Description("FileStore")]
        FileStore,
        [Description("WorkFlow")]
        WorkFlow,
        [Description("Project")]
        Project,
        [Description("DocConfig")]
        DocConfig,
        [Description("PM")]
        PM,
        [Description("ProjectBaseConfig")]
        ProjectBaseConfig,
        [Description("WorkTime")]
        WorkTime,
        [Description("HR")]
        HR,
        [Description("DocSystem")]
        DocSystem,
        [Description("OfficeAuto")]
        OfficeAuto,
    }

    public enum MsgReceiverType
    {
        UserType,
        PropertyType,
        SysRoleType
    }

    public enum MsgType
    {
        [Description("平台消息")]
        Normal,
        [Description("在线消息")]
        Online,
    }

    /// <summary>
    /// 真假
    /// </summary>
    public enum YesNo
    {

        Yes = 1,
        No = 0,
    }


    public enum RoleType
    {
        [Description("系统角色")]
        SysRole,
        [Description("组织角色")]
        OrgRole,
    }

    [Description("组织节点类型")]
    public enum OrgType
    {
        [Description("组织")]
        Org,
        [Description("管理部门")]
        Dept,
        [Description("生产部门")]
        ManufactureDept,
        [Description("子部门")]
        SubDept,
        [Description("岗位")]
        Post,
    }

    /// <summary>
    /// 组织结构
    /// </summary>
    public sealed class Org
    {
        public string ID { get; set; }
        /// <summary>父节点ID</summary>	
        public string ParentID { get; set; }
        /// <summary>全路径ID</summary>	
        public string FullID { get; set; }
        /// <summary>编号</summary>	
        public string Code { get; set; }
        /// <summary>名称</summary>	
        public string Name { get; set; }
        ///<summary>排序字段</summary>
        public float SortIndex { get; set; }
    }

    /// <summary>
    /// 角色信息
    /// </summary>
    public class Role
    {
        /// <summary>ID</summary>	
        public string ID { get; set; }
        /// <summary>编号</summary>	
        public string Code { get; set; }
        /// <summary>名称</summary>	
        public string Name { get; set; }
        /// <summary>类型</summary>	
        public string Type { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public sealed class UserInfo
    {
        public string UserID { get; set; }
        public string Code { get; set; }
        public string WorkNo { get; set; }
        public string Sex { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }

        public string UserOrgID { get; set; }
        public string UserFullOrgID { get; set; }
        public string UserOrgName { get; set; }
        public string UserPrjID { get; set; }
        public string UserPrjName { get; set; }
        public double SortIndex { get; set; }
        public string UserOrgIDs { get; set; }
        public string UserFullOrgIDs { get; set; }

        public string AppID { get; set; }
        public string MpID { get; set; }
    }

    /// <summary>
    /// 入口信息
    /// </summary>
    public sealed class Res
    {
        /// <summary>ID</summary>	
        public string ID { get; set; }
        /// <summary>父ID</summary>	
        public string ParentID { get; set; }
        /// <summary>全ID</summary>	
        public string FullID { get; set; }
        /// <summary>名称</summary>	
        public string Name { get; set; }
        /// <summary>类型</summary>	
        public string Type { get; set; }
        /// <summary>Url</summary>	
        public string Url { get; set; }
        /// <summary>按钮ID</summary>	
        public string ButtonID { get; set; }
        /// <summary>数据过滤</summary>	
        public string DataFilter { get; set; }
        ///// <summary>组织全路径</summary>	
        //public string OrgFullID { get; set; }
        ///// <summary>部门ID</summary>	
        //public string DeptID { get; set; }
        ///// <summary>部门名称</summary>	
        //public string DeptName { get; set; }
        ///// <summary>是否当前部门权限</summary>	
        //public string IsCurrentDeptAuth { get; set; }
    }

    /// <summary>
    /// 枚举项
    /// </summary>
    public class DicItem
    {
        /// <summary>
        /// 枚举项值
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        /// <summary>
        /// 枚举文本
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "subCategory")]
        public string SubCategory { get; set; }

    }
}
