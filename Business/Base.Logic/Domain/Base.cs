

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "Base"
//     Connection String:      "Data Source=.;Initial Catalog=MP_Base;User ID=sa;PWD=Password01!;"

// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Newtonsoft.Json;
using System.ComponentModel;

//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace Base.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class BaseEntities : Formula.FormulaDbContext
    {
        public IDbSet<S_A__OrgRes> S_A__OrgRes { get; set; } // S_A__OrgRes
        public IDbSet<S_A__OrgRole> S_A__OrgRole { get; set; } // S_A__OrgRole
        public IDbSet<S_A__OrgRoleUser> S_A__OrgRoleUser { get; set; } // S_A__OrgRoleUser
        public IDbSet<S_A__OrgUser> S_A__OrgUser { get; set; } // S_A__OrgUser
        public IDbSet<S_A__RoleRes> S_A__RoleRes { get; set; } // S_A__RoleRes
        public IDbSet<S_A__RoleUser> S_A__RoleUser { get; set; } // S_A__RoleUser
        public IDbSet<S_A__UserRes> S_A__UserRes { get; set; } // S_A__UserRes
        public IDbSet<S_A_AuthInfo> S_A_AuthInfo { get; set; } // S_A_AuthInfo
        public IDbSet<S_A_AuthLevel> S_A_AuthLevel { get; set; } // S_A_AuthLevel
        public IDbSet<S_A_AuthLog> S_A_AuthLog { get; set; } // S_A_AuthLog
        public IDbSet<S_A_Org> S_A_Org { get; set; } // S_A_Org
        public IDbSet<S_A_Res> S_A_Res { get; set; } // S_A_Res
        public IDbSet<S_A_Role> S_A_Role { get; set; } // S_A_Role
        public IDbSet<S_A_Security> S_A_Security { get; set; } // S_A_Security
        public IDbSet<S_A_User> S_A_User { get; set; } // S_A_User
        public IDbSet<S_A_UserImg> S_A_UserImg { get; set; } // S_A_UserImg
        public IDbSet<S_A_UserLinkMan> S_A_UserLinkMan { get; set; } // S_A_UserLinkMan
        public IDbSet<S_C_Holiday> S_C_Holiday { get; set; } // S_C_Holiday
        public IDbSet<S_D_FormToPDFRegist> S_D_FormToPDFRegist { get; set; } // S_D_FormToPDFRegist
        public IDbSet<S_D_FormToPDFTask> S_D_FormToPDFTask { get; set; } // S_D_FormToPDFTask
        public IDbSet<S_D_ModifyLog> S_D_ModifyLog { get; set; } // S_D_ModifyLog
        public IDbSet<S_D_PDFTask> S_D_PDFTask { get; set; } // S_D_PDFTask
        public IDbSet<S_H_Calendar> S_H_Calendar { get; set; } // S_H_Calendar
        public IDbSet<S_H_Feedback> S_H_Feedback { get; set; } // S_H_Feedback
        public IDbSet<S_H_ShortCut> S_H_ShortCut { get; set; } // S_H_ShortCut
        public IDbSet<S_I_FriendLink> S_I_FriendLink { get; set; } // S_I_FriendLink
        public IDbSet<S_I_NewsImage> S_I_NewsImage { get; set; } // S_I_NewsImage
        public IDbSet<S_I_NewsImageGroup> S_I_NewsImageGroup { get; set; } // S_I_NewsImageGroup
        public IDbSet<S_I_PublicInformation> S_I_PublicInformation { get; set; } // S_I_PublicInformation
        public IDbSet<S_I_PublicInformCatalog> S_I_PublicInformCatalog { get; set; } // S_I_PublicInformCatalog
        public IDbSet<S_M_Category> S_M_Category { get; set; } // S_M_Category
        public IDbSet<S_M_EnumDef> S_M_EnumDef { get; set; } // S_M_EnumDef
        public IDbSet<S_M_EnumItem> S_M_EnumItem { get; set; } // S_M_EnumItem
        public IDbSet<S_M_Field> S_M_Field { get; set; } // S_M_Field
        public IDbSet<S_M_Table> S_M_Table { get; set; } // S_M_Table
        public IDbSet<S_P_DoorBaseTemplate> S_P_DoorBaseTemplate { get; set; } // S_P_DoorBaseTemplate
        public IDbSet<S_P_DoorBlock> S_P_DoorBlock { get; set; } // S_P_DoorBlock
        public IDbSet<S_P_DoorTemplate> S_P_DoorTemplate { get; set; } // S_P_DoorTemplate
        public IDbSet<S_R_DataSet> S_R_DataSet { get; set; } // S_R_DataSet
        public IDbSet<S_R_Define> S_R_Define { get; set; } // S_R_Define
        public IDbSet<S_R_Field> S_R_Field { get; set; } // S_R_Field
        public IDbSet<S_RC_RuleCode> S_RC_RuleCode { get; set; } // S_RC_RuleCode
        public IDbSet<S_RC_RuleCodeData> S_RC_RuleCodeData { get; set; } // S_RC_RuleCodeData
        public IDbSet<S_S_Alarm> S_S_Alarm { get; set; } // S_S_Alarm
        public IDbSet<S_S_MsgBody> S_S_MsgBody { get; set; } // S_S_MsgBody
        public IDbSet<S_S_MsgReceiver> S_S_MsgReceiver { get; set; } // S_S_MsgReceiver
        public IDbSet<S_UI_Form> S_UI_Form { get; set; } // S_UI_Form
        public IDbSet<S_UI_List> S_UI_List { get; set; } // S_UI_List
        public IDbSet<S_UI_Selector> S_UI_Selector { get; set; } // S_UI_Selector
        public IDbSet<S_UI_SerialNumber> S_UI_SerialNumber { get; set; } // S_UI_SerialNumber
        public IDbSet<S_UI_Word> S_UI_Word { get; set; } // S_UI_Word

        static BaseEntities()
        {
            Database.SetInitializer<BaseEntities>(null);
        }

        public BaseEntities()
            : base("Name=Base")
        {
        }

        public BaseEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new S_A__OrgResConfiguration());
            modelBuilder.Configurations.Add(new S_A__OrgRoleConfiguration());
            modelBuilder.Configurations.Add(new S_A__OrgRoleUserConfiguration());
            modelBuilder.Configurations.Add(new S_A__OrgUserConfiguration());
            modelBuilder.Configurations.Add(new S_A__RoleResConfiguration());
            modelBuilder.Configurations.Add(new S_A__RoleUserConfiguration());
            modelBuilder.Configurations.Add(new S_A__UserResConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthInfoConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthLevelConfiguration());
            modelBuilder.Configurations.Add(new S_A_AuthLogConfiguration());
            modelBuilder.Configurations.Add(new S_A_OrgConfiguration());
            modelBuilder.Configurations.Add(new S_A_ResConfiguration());
            modelBuilder.Configurations.Add(new S_A_RoleConfiguration());
            modelBuilder.Configurations.Add(new S_A_SecurityConfiguration());
            modelBuilder.Configurations.Add(new S_A_UserConfiguration());
            modelBuilder.Configurations.Add(new S_A_UserImgConfiguration());
            modelBuilder.Configurations.Add(new S_A_UserLinkManConfiguration());
            modelBuilder.Configurations.Add(new S_C_HolidayConfiguration());
            modelBuilder.Configurations.Add(new S_D_FormToPDFRegistConfiguration());
            modelBuilder.Configurations.Add(new S_D_FormToPDFTaskConfiguration());
            modelBuilder.Configurations.Add(new S_D_ModifyLogConfiguration());
            modelBuilder.Configurations.Add(new S_D_PDFTaskConfiguration());
            modelBuilder.Configurations.Add(new S_H_CalendarConfiguration());
            modelBuilder.Configurations.Add(new S_H_FeedbackConfiguration());
            modelBuilder.Configurations.Add(new S_H_ShortCutConfiguration());
            modelBuilder.Configurations.Add(new S_I_FriendLinkConfiguration());
            modelBuilder.Configurations.Add(new S_I_NewsImageConfiguration());
            modelBuilder.Configurations.Add(new S_I_NewsImageGroupConfiguration());
            modelBuilder.Configurations.Add(new S_I_PublicInformationConfiguration());
            modelBuilder.Configurations.Add(new S_I_PublicInformCatalogConfiguration());
            modelBuilder.Configurations.Add(new S_M_CategoryConfiguration());
            modelBuilder.Configurations.Add(new S_M_EnumDefConfiguration());
            modelBuilder.Configurations.Add(new S_M_EnumItemConfiguration());
            modelBuilder.Configurations.Add(new S_M_FieldConfiguration());
            modelBuilder.Configurations.Add(new S_M_TableConfiguration());
            modelBuilder.Configurations.Add(new S_P_DoorBaseTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_P_DoorBlockConfiguration());
            modelBuilder.Configurations.Add(new S_P_DoorTemplateConfiguration());
            modelBuilder.Configurations.Add(new S_R_DataSetConfiguration());
            modelBuilder.Configurations.Add(new S_R_DefineConfiguration());
            modelBuilder.Configurations.Add(new S_R_FieldConfiguration());
            modelBuilder.Configurations.Add(new S_RC_RuleCodeConfiguration());
            modelBuilder.Configurations.Add(new S_RC_RuleCodeDataConfiguration());
            modelBuilder.Configurations.Add(new S_S_AlarmConfiguration());
            modelBuilder.Configurations.Add(new S_S_MsgBodyConfiguration());
            modelBuilder.Configurations.Add(new S_S_MsgReceiverConfiguration());
            modelBuilder.Configurations.Add(new S_UI_FormConfiguration());
            modelBuilder.Configurations.Add(new S_UI_ListConfiguration());
            modelBuilder.Configurations.Add(new S_UI_SelectorConfiguration());
            modelBuilder.Configurations.Add(new S_UI_SerialNumberConfiguration());
            modelBuilder.Configurations.Add(new S_UI_WordConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary>组织和权限资源关系表</summary>	
	[Description("组织和权限资源关系表")]
    public partial class S_A__OrgRes : Formula.BaseModel
    {
		/// <summary>权限资源ID</summary>	
		[Description("权限资源ID")]
        public string ResID { get; set; } // ResID (Primary key)
		/// <summary>组织机构ID</summary>	
		[Description("组织机构ID")]
        public string OrgID { get; set; } // OrgID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Res S_A_Res { get; set; } //  ResID - FK_S_A__OrgRes_S_A_Res
		[JsonIgnore]
        public virtual S_A_Org S_A_Org { get; set; } //  OrgID - FK_S_A__OrgRes_S_A_Org
    }

	/// <summary>组织和角色关系表</summary>	
	[Description("组织和角色关系表")]
    public partial class S_A__OrgRole : Formula.BaseModel
    {
		/// <summary>角色ID</summary>	
		[Description("角色ID")]
        public string RoleID { get; set; } // RoleID (Primary key)
		/// <summary>组织ID</summary>	
		[Description("组织ID")]
        public string OrgID { get; set; } // OrgID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_A_OrgRole_ARole
		[JsonIgnore]
        public virtual S_A_Org S_A_Org { get; set; } //  OrgID - FK_A_OrgRole_AOrg
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A__OrgRoleUser : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string OrgID { get; set; } // OrgID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string RoleID { get; set; } // RoleID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_S_A__OrgRoleUser_S_A_Role
    }

	/// <summary>组织和用户关系表</summary>	
	[Description("组织和用户关系表")]
    public partial class S_A__OrgUser : Formula.BaseModel
    {
		/// <summary>组织ID</summary>	
		[Description("组织ID")]
        public string OrgID { get; set; } // OrgID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Org S_A_Org { get; set; } //  OrgID - FK_A_OrgUser_AOrg
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_A_OrgUser_AUser
    }

	/// <summary>角色和权限资源关系表</summary>	
	[Description("角色和权限资源关系表")]
    public partial class S_A__RoleRes : Formula.BaseModel
    {
		/// <summary>权限资源ID</summary>	
		[Description("权限资源ID")]
        public string ResID { get; set; } // ResID (Primary key)
		/// <summary>角色ID</summary>	
		[Description("角色ID")]
        public string RoleID { get; set; } // RoleID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Res S_A_Res { get; set; } //  ResID - FK_S_A__RoleRes_S_A_Res
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_S_A__RoleRes_S_A_Role
    }

	/// <summary>角色和用户关系表</summary>	
	[Description("角色和用户关系表")]
    public partial class S_A__RoleUser : Formula.BaseModel
    {
		/// <summary>角色ID</summary>	
		[Description("角色ID")]
        public string RoleID { get; set; } // RoleID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID (Primary key)

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_Role S_A_Role { get; set; } //  RoleID - FK_A_RoleUser_ARole
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_A_RoleUser_AUser
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A__UserRes : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ResID { get; set; } // ResID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DenyAuth { get; set; } // DenyAuth

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_S_A__UserRes_S_A_User
		[JsonIgnore]
        public virtual S_A_Res S_A_Res { get; set; } //  ResID - FK_S_A__UserRes_S_A_Res
    }

	/// <summary>权限模块信息表</summary>	
	[Description("权限模块信息表")]
    public partial class S_A_AuthInfo : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>组织机构树的根</summary>	
		[Description("组织机构树的根")]
        public string OrgRootFullID { get; set; } // OrgRootFullID
		/// <summary>组织机构根</summary>	
		[Description("组织机构根")]
        public string ResRootFullID { get; set; } // ResRootFullID
		/// <summary>角色组ID</summary>	
		[Description("角色组ID")]
        public string RoleGroupID { get; set; } // RoleGroupID
		/// <summary>用户组ID</summary>	
		[Description("用户组ID")]
        public string UserGroupID { get; set; } // UserGroupID
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
    }

	/// <summary>分级授权</summary>	
	[Description("分级授权")]
    public partial class S_A_AuthLevel : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID
		/// <summary>用户姓名</summary>	
		[Description("用户姓名")]
        public string UserName { get; set; } // UserName
		/// <summary>可以授权的菜单根</summary>	
		[Description("可以授权的菜单根")]
        public string MenuRootFullID { get; set; } // MenuRootFullID
		/// <summary>可以授权的菜单根</summary>	
		[Description("可以授权的菜单根")]
        public string MenuRootName { get; set; } // MenuRootName
		/// <summary>可以授权的规则根</summary>	
		[Description("可以授权的规则根")]
        public string RuleRootFullID { get; set; } // RuleRootFullID
		/// <summary>可以授权的规则根</summary>	
		[Description("可以授权的规则根")]
        public string RuleRootName { get; set; } // RuleRootName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_AuthLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Operation { get; set; } // Operation
		/// <summary></summary>	
		[Description("")]
        public string OperationTarget { get; set; } // OperationTarget
		/// <summary></summary>	
		[Description("")]
        public string RelateData { get; set; } // RelateData
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string ClientIP { get; set; } // ClientIP
    }

	/// <summary>组织机构表</summary>	
	[Description("组织机构表")]
    public partial class S_A_Org : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全路径ID</summary>	
		[Description("全路径ID")]
        public string FullID { get; set; } // FullID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称（简称）</summary>	
		[Description("名称（简称）")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary>全称</summary>	
		[Description("全称")]
        public string ShortName { get; set; } // ShortName
		/// <summary>性质</summary>	
		[Description("性质")]
        public string Character { get; set; } // Character
		/// <summary>所在地</summary>	
		[Description("所在地")]
        public string Location { get; set; } // Location

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgRes> S_A__OrgRes { get { onS_A__OrgResGetting(); return _S_A__OrgRes;} }
		private ICollection<S_A__OrgRes> _S_A__OrgRes;
		partial void onS_A__OrgResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__OrgRole> S_A__OrgRole { get { onS_A__OrgRoleGetting(); return _S_A__OrgRole;} }
		private ICollection<S_A__OrgRole> _S_A__OrgRole;
		partial void onS_A__OrgRoleGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__OrgUser> S_A__OrgUser { get { onS_A__OrgUserGetting(); return _S_A__OrgUser;} }
		private ICollection<S_A__OrgUser> _S_A__OrgUser;
		partial void onS_A__OrgUserGetting();


        public S_A_Org()
        {
			IsDeleted = "0";
            _S_A__OrgRes = new List<S_A__OrgRes>();
            _S_A__OrgRole = new List<S_A__OrgRole>();
            _S_A__OrgUser = new List<S_A__OrgUser>();
        }
    }

	/// <summary>权限资源表</summary>	
	[Description("权限资源表")]
    public partial class S_A_Res : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全ID</summary>	
		[Description("全ID")]
        public string FullID { get; set; } // FullID
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>Url</summary>	
		[Description("Url")]
        public string Url { get; set; } // Url
		/// <summary>图标Url</summary>	
		[Description("图标Url")]
        public string IconUrl { get; set; } // IconUrl
		/// <summary>打开目标</summary>	
		[Description("打开目标")]
        public string Target { get; set; } // Target
		/// <summary>按钮ID</summary>	
		[Description("按钮ID")]
        public string ButtonID { get; set; } // ButtonID
		/// <summary>数据过滤</summary>	
		[Description("数据过滤")]
        public string DataFilter { get; set; } // DataFilter
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgRes> S_A__OrgRes { get { onS_A__OrgResGetting(); return _S_A__OrgRes;} }
		private ICollection<S_A__OrgRes> _S_A__OrgRes;
		partial void onS_A__OrgResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleRes> S_A__RoleRes { get { onS_A__RoleResGetting(); return _S_A__RoleRes;} }
		private ICollection<S_A__RoleRes> _S_A__RoleRes;
		partial void onS_A__RoleResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__UserRes> S_A__UserRes { get { onS_A__UserResGetting(); return _S_A__UserRes;} }
		private ICollection<S_A__UserRes> _S_A__UserRes;
		partial void onS_A__UserResGetting();


        public S_A_Res()
        {
			Type = "Menu";
            _S_A__OrgRes = new List<S_A__OrgRes>();
            _S_A__RoleRes = new List<S_A__RoleRes>();
            _S_A__UserRes = new List<S_A__UserRes>();
        }
    }

	/// <summary>角色表</summary>	
	[Description("角色表")]
    public partial class S_A_Role : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>角色组ID</summary>	
		[Description("角色组ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgRole> S_A__OrgRole { get { onS_A__OrgRoleGetting(); return _S_A__OrgRole;} }
		private ICollection<S_A__OrgRole> _S_A__OrgRole;
		partial void onS_A__OrgRoleGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__OrgRoleUser> S_A__OrgRoleUser { get { onS_A__OrgRoleUserGetting(); return _S_A__OrgRoleUser;} }
		private ICollection<S_A__OrgRoleUser> _S_A__OrgRoleUser;
		partial void onS_A__OrgRoleUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleRes> S_A__RoleRes { get { onS_A__RoleResGetting(); return _S_A__RoleRes;} }
		private ICollection<S_A__RoleRes> _S_A__RoleRes;
		partial void onS_A__RoleResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleUser> S_A__RoleUser { get { onS_A__RoleUserGetting(); return _S_A__RoleUser;} }
		private ICollection<S_A__RoleUser> _S_A__RoleUser;
		partial void onS_A__RoleUserGetting();


        public S_A_Role()
        {
            _S_A__OrgRole = new List<S_A__OrgRole>();
            _S_A__OrgRoleUser = new List<S_A__OrgRoleUser>();
            _S_A__RoleRes = new List<S_A__RoleRes>();
            _S_A__RoleUser = new List<S_A__RoleUser>();
        }
    }

	/// <summary>三权分离表</summary>	
	[Description("三权分离表")]
    public partial class S_A_Security : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>超级管理员账号,必须为administrator</summary>	
		[Description("超级管理员账号,必须为administrator")]
        public string SuperAdmin { get; set; } // SuperAdmin
		/// <summary>超级管理员密码</summary>	
		[Description("超级管理员密码")]
        public string SuperAdminPwd { get; set; } // SuperAdminPwd
		/// <summary>超级管理员密码安全</summary>	
		[Description("超级管理员密码安全")]
        public string SuperAdminSecurity { get; set; } // SuperAdminSecurity
		/// <summary>超级管理员密码修改时间</summary>	
		[Description("超级管理员密码修改时间")]
        public DateTime? SuperAdminModifyTime { get; set; } // SuperAdminModifyTime
		/// <summary>管理员IDs</summary>	
		[Description("管理员IDs")]
        public string AdminIDs { get; set; } // AdminIDs
		/// <summary>管理员Names</summary>	
		[Description("管理员Names")]
        public string AdminNames { get; set; } // AdminNames
		/// <summary>管理员修改时间</summary>	
		[Description("管理员修改时间")]
        public DateTime? AdminModifyTime { get; set; } // AdminModifyTime
		/// <summary>管理员安全</summary>	
		[Description("管理员安全")]
        public string AdminSecurity { get; set; } // AdminSecurity
    }

	/// <summary>用户表</summary>	
	[Description("用户表")]
    public partial class S_A_User : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户组ID</summary>	
		[Description("用户组ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>帐号</summary>	
		[Description("帐号")]
        public string Code { get; set; } // Code
		/// <summary>姓名</summary>	
		[Description("姓名")]
        public string Name { get; set; } // Name
		/// <summary>工号</summary>	
		[Description("工号")]
        public string WorkNo { get; set; } // WorkNo
		/// <summary>密码</summary>	
		[Description("密码")]
        public string Password { get; set; } // Password
		/// <summary>性别</summary>	
		[Description("性别")]
        public string Sex { get; set; } // Sex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>入职日期</summary>	
		[Description("入职日期")]
        public DateTime? InDate { get; set; } // InDate
		/// <summary>离职日期</summary>	
		[Description("离职日期")]
        public DateTime? OutDate { get; set; } // OutDate
		/// <summary>电话</summary>	
		[Description("电话")]
        public string Phone { get; set; } // Phone
		/// <summary>手机</summary>	
		[Description("手机")]
        public string MobilePhone { get; set; } // MobilePhone
		/// <summary>Email</summary>	
		[Description("Email")]
        public string Email { get; set; } // Email
		/// <summary>地址</summary>	
		[Description("地址")]
        public string Address { get; set; } // Address
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>最后登录时间</summary>	
		[Description("最后登录时间")]
        public string LastLoginTime { get; set; } // LastLoginTime
		/// <summary>最后登录IP</summary>	
		[Description("最后登录IP")]
        public string LastLoginIP { get; set; } // LastLoginIP
		/// <summary>最后登录SessionID</summary>	
		[Description("最后登录SessionID")]
        public string LastSessionID { get; set; } // LastSessionID
		/// <summary>登录错误次数</summary>	
		[Description("登录错误次数")]
        public int? ErrorCount { get; set; } // ErrorCount
		/// <summary></summary>	
		[Description("")]
        public DateTime? ErrorTime { get; set; } // ErrorTime
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary>当前项目ID</summary>	
		[Description("当前项目ID")]
        public string PrjID { get; set; } // PrjID
		/// <summary>当前项目名称</summary>	
		[Description("当前项目名称")]
        public string PrjName { get; set; } // PrjName
		/// <summary>当前部门ID</summary>	
		[Description("当前部门ID")]
        public string DeptID { get; set; } // DeptID
		/// <summary>当前部门全ID</summary>	
		[Description("当前部门全ID")]
        public string DeptFullID { get; set; } // DeptFullID
		/// <summary>当前部门名称</summary>	
		[Description("当前部门名称")]
        public string DeptName { get; set; } // DeptName
		/// <summary></summary>	
		[Description("")]
        public string RTX { get; set; } // RTX
		/// <summary>最后更新时间</summary>	
		[Description("最后更新时间")]
        public DateTime? ModifyTime { get; set; } // ModifyTime

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_A__OrgUser> S_A__OrgUser { get { onS_A__OrgUserGetting(); return _S_A__OrgUser;} }
		private ICollection<S_A__OrgUser> _S_A__OrgUser;
		partial void onS_A__OrgUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__RoleUser> S_A__RoleUser { get { onS_A__RoleUserGetting(); return _S_A__RoleUser;} }
		private ICollection<S_A__RoleUser> _S_A__RoleUser;
		partial void onS_A__RoleUserGetting();

		[JsonIgnore]
        public virtual ICollection<S_A__UserRes> S_A__UserRes { get { onS_A__UserResGetting(); return _S_A__UserRes;} }
		private ICollection<S_A__UserRes> _S_A__UserRes;
		partial void onS_A__UserResGetting();

		[JsonIgnore]
        public virtual ICollection<S_A_UserImg> S_A_UserImg { get { onS_A_UserImgGetting(); return _S_A_UserImg;} }
		private ICollection<S_A_UserImg> _S_A_UserImg;
		partial void onS_A_UserImgGetting();

		[JsonIgnore]
        public virtual ICollection<S_A_UserLinkMan> S_A_UserLinkMan { get { onS_A_UserLinkManGetting(); return _S_A_UserLinkMan;} }
		private ICollection<S_A_UserLinkMan> _S_A_UserLinkMan;
		partial void onS_A_UserLinkManGetting();


        public S_A_User()
        {
			SortIndex = 0;
			ErrorCount = 0;
			IsDeleted = "0";
            _S_A__OrgUser = new List<S_A__OrgUser>();
            _S_A__RoleUser = new List<S_A__RoleUser>();
            _S_A__UserRes = new List<S_A__UserRes>();
            _S_A_UserImg = new List<S_A_UserImg>();
            _S_A_UserLinkMan = new List<S_A_UserLinkMan>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_UserImg : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID
		/// <summary>签名图片</summary>	
		[Description("签名图片")]
        public byte[] SignImg { get; set; } // SignImg
		/// <summary>照片</summary>	
		[Description("照片")]
        public byte[] Picture { get; set; } // Picture

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_S_A_UserImg_S_A_User
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_A_UserLinkMan : Formula.BaseModel
    {
		/// <summary>表格标识</summary>	
		[Description("表格标识")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID标识</summary>	
		[Description("用户ID标识")]
        public string UserID { get; set; } // UserID
		/// <summary>联系人ID标识</summary>	
		[Description("联系人ID标识")]
        public string LinkManID { get; set; } // LinkManID
		/// <summary>排序</summary>	
		[Description("排序")]
        public double? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual S_A_User S_A_User { get; set; } //  UserID - FK_S_A_UserLinkMan_S_A_User
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_C_Holiday : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public int? Year { get; set; } // Year
		/// <summary></summary>	
		[Description("")]
        public int? Month { get; set; } // Month
		/// <summary></summary>	
		[Description("")]
        public int? Day { get; set; } // Day
		/// <summary></summary>	
		[Description("")]
        public DateTime? Date { get; set; } // Date
		/// <summary></summary>	
		[Description("")]
        public string DayOfWeek { get; set; } // DayOfWeek
		/// <summary></summary>	
		[Description("")]
        public string IsHoliday { get; set; } // IsHoliday
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_FormToPDFRegist : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>表单名称</summary>	
		[Description("表单名称")]
        public string FormName { get; set; } // FormName
		/// <summary>表名</summary>	
		[Description("表名")]
        public string TableName { get; set; } // TableName
		/// <summary>Word模板码</summary>	
		[Description("Word模板码")]
        public string TempCode { get; set; } // TempCode
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_FormToPDFTask : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string TempCode { get; set; } // TempCode
		/// <summary></summary>	
		[Description("")]
        public string FormID { get; set; } // FormID
		/// <summary></summary>	
		[Description("")]
        public string PDFFileID { get; set; } // PDFFileID
		/// <summary>表单最后更新时间</summary>	
		[Description("表单最后更新时间")]
        public DateTime? FormLastModifyDate { get; set; } // FormLastModifyDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? BeginTime { get; set; } // BeginTime
		/// <summary>完成时间</summary>	
		[Description("完成时间")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary></summary>	
		[Description("")]
        public string DoneLog { get; set; } // DoneLog
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_ModifyLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string ModifyMode { get; set; } // ModifyMode
		/// <summary></summary>	
		[Description("")]
        public string EntityKey { get; set; } // EntityKey
		/// <summary></summary>	
		[Description("")]
        public string CurrentValue { get; set; } // CurrentValue
		/// <summary></summary>	
		[Description("")]
        public string OriginalValue { get; set; } // OriginalValue
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string ClientIP { get; set; } // ClientIP
		/// <summary></summary>	
		[Description("")]
        public string UserHostAddress { get; set; } // UserHostAddress
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_D_PDFTask : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string FileID { get; set; } // FileID
		/// <summary></summary>	
		[Description("")]
        public string FileType { get; set; } // FileType
		/// <summary></summary>	
		[Description("")]
        public string PDFFileID { get; set; } // PDFFileID
		/// <summary></summary>	
		[Description("")]
        public string SWFFileID { get; set; } // SWFFileID
		/// <summary></summary>	
		[Description("")]
        public string SnapFileID { get; set; } // SnapFileID
		/// <summary></summary>	
		[Description("")]
        public int PDFPageCount { get; set; } // PDFPageCount
		/// <summary></summary>	
		[Description("")]
        public bool IsSplit { get; set; } // IsSplit
		/// <summary></summary>	
		[Description("")]
        public string Status { get; set; } // Status
		/// <summary></summary>	
		[Description("")]
        public DateTime? StartTime { get; set; } // StartTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_Calendar : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public DateTime? StartTime { get; set; } // StartTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? EndTime { get; set; } // EndTime
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string Grade { get; set; } // Grade
		/// <summary></summary>	
		[Description("")]
        public string Attachments { get; set; } // Attachments
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string Sponsor { get; set; } // Sponsor
		/// <summary></summary>	
		[Description("")]
        public string SponsorID { get; set; } // SponsorID
		/// <summary></summary>	
		[Description("")]
        public string Participators { get; set; } // Participators
		/// <summary></summary>	
		[Description("")]
        public string ParticipatorsID { get; set; } // ParticipatorsID
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_Feedback : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string Attachment { get; set; } // Attachment
		/// <summary></summary>	
		[Description("")]
        public string IsUse { get; set; } // IsUse
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_H_ShortCut : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary>图标的路径</summary>	
		[Description("图标的路径")]
        public string IconImage { get; set; } // IconImage
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string IsUse { get; set; } // IsUse
		/// <summary></summary>	
		[Description("")]
        public int? PageIndex { get; set; } // PageIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_FriendLink : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Icon { get; set; } // Icon
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Url { get; set; } // Url
		/// <summary></summary>	
		[Description("")]
        public string DeptId { get; set; } // DeptId
		/// <summary></summary>	
		[Description("")]
        public string DeptName { get; set; } // DeptName
		/// <summary></summary>	
		[Description("")]
        public string UserId { get; set; } // UserId
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_NewsImage : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string GroupID { get; set; } // GroupID
		/// <summary></summary>	
		[Description("")]
        public string PictureName { get; set; } // PictureName
		/// <summary></summary>	
		[Description("")]
        public byte[] PictureEntire { get; set; } // PictureEntire
		/// <summary></summary>	
		[Description("")]
        public byte[] PictureThumb { get; set; } // PictureThumb
		/// <summary>图片</summary>	
		[Description("图片")]
        public string Src { get; set; } // Src
		/// <summary>链接</summary>	
		[Description("链接")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>排序</summary>	
		[Description("排序")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary>创建日期</summary>	
		[Description("创建日期")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_NewsImageGroup : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorId { get; set; } // DeptDoorId
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorName { get; set; } // DeptDoorName
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_PublicInformation : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CatalogId { get; set; } // CatalogId
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string ContentText { get; set; } // ContentText
		/// <summary></summary>	
		[Description("")]
        public string Attachments { get; set; } // Attachments
		/// <summary></summary>	
		[Description("")]
        public string ReceiveDeptId { get; set; } // ReceiveDeptId
		/// <summary></summary>	
		[Description("")]
        public string ReceiveDeptName { get; set; } // ReceiveDeptName
		/// <summary></summary>	
		[Description("")]
        public string ReceiveUserId { get; set; } // ReceiveUserId
		/// <summary></summary>	
		[Description("")]
        public string ReceiveUserName { get; set; } // ReceiveUserName
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorId { get; set; } // DeptDoorId
		/// <summary></summary>	
		[Description("")]
        public string DeptDoorName { get; set; } // DeptDoorName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ExpiresTime { get; set; } // ExpiresTime
		/// <summary></summary>	
		[Description("")]
        public int? ReadCount { get; set; } // ReadCount
		/// <summary>重要度 1重要，0一般</summary>	
		[Description("重要度 1重要，0一般")]
        public string Important { get; set; } // Important
		/// <summary>紧急度 1重要，0一般</summary>	
		[Description("紧急度 1重要，0一般")]
        public string Urgency { get; set; } // Urgency
		/// <summary>置顶</summary>	
		[Description("置顶")]
        public string IsTop { get; set; } // IsTop
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_I_PublicInformCatalog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string CatalogName { get; set; } // CatalogName
		/// <summary></summary>	
		[Description("")]
        public string CatalogKey { get; set; } // CatalogKey
		/// <summary></summary>	
		[Description("")]
        public string IsOnHomePage { get; set; } // IsOnHomePage
		/// <summary></summary>	
		[Description("")]
        public int? InHomePageNum { get; set; } // InHomePageNum
		/// <summary></summary>	
		[Description("")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary>创建日期</summary>	
		[Description("创建日期")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUserName { get; set; } // CreateUserName
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
    }

	/// <summary>元数据分类表</summary>	
	[Description("元数据分类表")]
    public partial class S_M_Category : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全ID</summary>	
		[Description("全ID")]
        public string FullID { get; set; } // FullID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
    }

	/// <summary>枚举定义表</summary>	
	[Description("枚举定义表")]
    public partial class S_M_EnumDef : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>数据库连接名</summary>	
		[Description("数据库连接名")]
        public string ConnName { get; set; } // ConnName
		/// <summary>查询Sql</summary>	
		[Description("查询Sql")]
        public string Sql { get; set; } // Sql
		/// <summary>排序</summary>	
		[Description("排序")]
        public string Orderby { get; set; } // Orderby
		/// <summary>分类ID</summary>	
		[Description("分类ID")]
        public string CategoryID { get; set; } // CategoryID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_M_EnumItem> S_M_EnumItem { get { onS_M_EnumItemGetting(); return _S_M_EnumItem;} }
		private ICollection<S_M_EnumItem> _S_M_EnumItem;
		partial void onS_M_EnumItemGetting();


        public S_M_EnumDef()
        {
			Type = "Normal";
			SortIndex = 0;
            _S_M_EnumItem = new List<S_M_EnumItem>();
        }
    }

	/// <summary>枚举表</summary>	
	[Description("枚举表")]
    public partial class S_M_EnumItem : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>枚举定义ID</summary>	
		[Description("枚举定义ID")]
        public string EnumDefID { get; set; } // EnumDefID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>子枚举编号</summary>	
		[Description("子枚举编号")]
        public string SubEnumDefCode { get; set; } // SubEnumDefCode
		/// <summary>枚举分类</summary>	
		[Description("枚举分类")]
        public string Category { get; set; } // Category
		/// <summary>枚举子分类</summary>	
		[Description("枚举子分类")]
        public string SubCategory { get; set; } // SubCategory

        // Foreign keys
		[JsonIgnore]
        public virtual S_M_EnumDef S_M_EnumDef { get; set; } //  EnumDefID - FK_EnumItem_EnumDef

        public S_M_EnumItem()
        {
			SortIndex = 0.0;
        }
    }

	/// <summary>平台数据库字段表</summary>	
	[Description("平台数据库字段表")]
    public partial class S_M_Field : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>表ID</summary>	
		[Description("表ID")]
        public string TableID { get; set; } // TableID
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public double SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description

        // Foreign keys
		[JsonIgnore]
        public virtual S_M_Table S_M_Table { get; set; } //  TableID - FK_S_M_Field_S_M_Table

        public S_M_Field()
        {
			SortIndex = 0;
        }
    }

	/// <summary>平台数据库表</summary>	
	[Description("平台数据库表")]
    public partial class S_M_Table : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>编号</summary>	
		[Description("编号")]
        public string Code { get; set; } // Code
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary>数据库连接名</summary>	
		[Description("数据库连接名")]
        public string ConnName { get; set; } // ConnName
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_M_Field> S_M_Field { get { onS_M_FieldGetting(); return _S_M_Field;} }
		private ICollection<S_M_Field> _S_M_Field;
		partial void onS_M_FieldGetting();


        public S_M_Table()
        {
            _S_M_Field = new List<S_M_Field>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_P_DoorBaseTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string BaseType { get; set; } // BaseType
		/// <summary></summary>	
		[Description("")]
        public string IsDefault { get; set; } // IsDefault
		/// <summary></summary>	
		[Description("")]
        public string TemplateColWidth { get; set; } // TemplateColWidth
		/// <summary></summary>	
		[Description("")]
        public string TemplateString { get; set; } // TemplateString
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string IsEdit { get; set; } // IsEdit
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_P_DoorBlock : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string BlockName { get; set; } // BlockName
		/// <summary></summary>	
		[Description("")]
        public string BlockKey { get; set; } // BlockKey
		/// <summary></summary>	
		[Description("")]
        public string BlockTitle { get; set; } // BlockTitle
		/// <summary></summary>	
		[Description("")]
        public string BlockType { get; set; } // BlockType
		/// <summary></summary>	
		[Description("")]
        public string BlockImage { get; set; } // BlockImage
		/// <summary></summary>	
		[Description("")]
        public string Remark { get; set; } // Remark
		/// <summary></summary>	
		[Description("")]
        public string HeadHtml { get; set; } // HeadHtml
		/// <summary></summary>	
		[Description("")]
        public string ColorValue { get; set; } // ColorValue
		/// <summary></summary>	
		[Description("")]
        public string Color { get; set; } // Color
		/// <summary></summary>	
		[Description("")]
        public int? RepeatItemCount { get; set; } // RepeatItemCount
		/// <summary></summary>	
		[Description("")]
        public int? RepeatItemLength { get; set; } // RepeatItemLength
		/// <summary></summary>	
		[Description("")]
        public string RepeatDataDataSql { get; set; } // RepeatDataDataSql
		/// <summary></summary>	
		[Description("")]
        public string RepeatItemTemplate { get; set; } // RepeatItemTemplate
		/// <summary></summary>	
		[Description("")]
        public string FootHtml { get; set; } // FootHtml
		/// <summary></summary>	
		[Description("")]
        public int? DelayLoadSecond { get; set; } // DelayLoadSecond
		/// <summary></summary>	
		[Description("")]
        public double? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string RelateScript { get; set; } // RelateScript
		/// <summary></summary>	
		[Description("")]
        public string IsHidden { get; set; } // IsHidden
		/// <summary></summary>	
		[Description("")]
        public string TemplateId { get; set; } // TemplateId
		/// <summary></summary>	
		[Description("")]
        public string AllowUserIDs { get; set; } // AllowUserIDs
		/// <summary></summary>	
		[Description("")]
        public string AllowUserNames { get; set; } // AllowUserNames
		/// <summary></summary>	
		[Description("")]
        public string AllowTypes { get; set; } // AllowTypes
		/// <summary></summary>	
		[Description("")]
        public string IsEdit { get; set; } // IsEdit
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_P_DoorTemplate : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>自定义类别，User或Group</summary>	
		[Description("自定义类别，User或Group")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string BaseType { get; set; } // BaseType
		/// <summary></summary>	
		[Description("")]
        public string UserID { get; set; } // UserID
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string IsDefault { get; set; } // IsDefault
		/// <summary></summary>	
		[Description("")]
        public string TemplateColWidth { get; set; } // TemplateColWidth
		/// <summary></summary>	
		[Description("")]
        public string TemplateString { get; set; } // TemplateString
		/// <summary></summary>	
		[Description("")]
        public string BaseTemplateId { get; set; } // BaseTemplateId
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_R_DataSet : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DefineID { get; set; } // DefineID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string Sql { get; set; } // Sql

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_R_Field> S_R_Field { get { onS_R_FieldGetting(); return _S_R_Field;} }
		private ICollection<S_R_Field> _S_R_Field;
		partial void onS_R_FieldGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual S_R_Define S_R_Define { get; set; } //  DefineID - FK_S_R_DataSet_S_R_Define

        public S_R_DataSet()
        {
            _S_R_Field = new List<S_R_Field>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_R_Define : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_R_DataSet> S_R_DataSet { get { onS_R_DataSetGetting(); return _S_R_DataSet;} }
		private ICollection<S_R_DataSet> _S_R_DataSet;
		partial void onS_R_DataSetGetting();


        public S_R_Define()
        {
            _S_R_DataSet = new List<S_R_DataSet>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_R_Field : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string DataSetID { get; set; } // DataSetID
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string EnumKey { get; set; } // EnumKey

        // Foreign keys
		[JsonIgnore]
        public virtual S_R_DataSet S_R_DataSet { get; set; } //  DataSetID - FK_S_R_Field_S_R_DataSet
    }

	/// <summary>AHCJ_BASE.S_RC_RULECODE</summary>	
	[Description("AHCJ_BASE.S_RC_RULECODE")]
    public partial class S_RC_RuleCode : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string RuleName { get; set; } // RuleName
		/// <summary></summary>	
		[Description("")]
        public string Prefix { get; set; } // Prefix
		/// <summary></summary>	
		[Description("")]
        public string PostFix { get; set; } // PostFix
		/// <summary></summary>	
		[Description("")]
        public string Seperative { get; set; } // Seperative
		/// <summary></summary>	
		[Description("")]
        public decimal? Digit { get; set; } // Digit
		/// <summary></summary>	
		[Description("")]
        public decimal? StartNumber { get; set; } // StartNumber
		/// <summary></summary>	
		[Description("")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary></summary>	
		[Description("")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
    }

	/// <summary>AHCJ_BASE.S_RC_RULECODEDATA</summary>	
	[Description("AHCJ_BASE.S_RC_RULECODEDATA")]
    public partial class S_RC_RuleCodeData : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public decimal? Year { get; set; } // Year
		/// <summary></summary>	
		[Description("")]
        public decimal? AutoNumber { get; set; } // AutoNumber
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_S_Alarm : Formula.BaseModel
    {
		/// <summary>报警ID</summary>	
		[Description("报警ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>重要度:枚举1,0(重要不重要)</summary>	
		[Description("重要度:枚举1,0(重要不重要)")]
        public string Important { get; set; } // Important
		/// <summary>紧急度:枚举1,0 (紧急不紧急)</summary>	
		[Description("紧急度:枚举1,0 (紧急不紧急)")]
        public string Urgency { get; set; } // Urgency
		/// <summary>报警类型</summary>	
		[Description("报警类型")]
        public string AlarmType { get; set; } // AlarmType
		/// <summary>标题</summary>	
		[Description("标题")]
        public string AlarmTitle { get; set; } // AlarmTitle
		/// <summary>正文内容</summary>	
		[Description("正文内容")]
        public string AlarmContent { get; set; } // AlarmContent
		/// <summary>链接地址</summary>	
		[Description("链接地址")]
        public string AlarmUrl { get; set; } // AlarmUrl
		/// <summary>拥有人</summary>	
		[Description("拥有人")]
        public string OwnerName { get; set; } // OwnerName
		/// <summary>拥有人ID</summary>	
		[Description("拥有人ID")]
        public string OwnerID { get; set; } // OwnerID
		/// <summary>提醒时间</summary>	
		[Description("提醒时间")]
        public DateTime? SendTime { get; set; } // SendTime
		/// <summary>事务截止完成时间</summary>	
		[Description("事务截止完成时间")]
        public DateTime? DeadlineTime { get; set; } // DeadlineTime
		/// <summary></summary>	
		[Description("")]
        public string SenderName { get; set; } // SenderName
		/// <summary></summary>	
		[Description("")]
        public string SenderID { get; set; } // SenderID
		/// <summary></summary>	
		[Description("")]
        public string IsDelete { get; set; } // IsDelete
    }

	/// <summary>短消息表</summary>	
	[Description("短消息表")]
    public partial class S_S_MsgBody : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>类型</summary>	
		[Description("类型")]
        public string Type { get; set; } // Type
		/// <summary>标题</summary>	
		[Description("标题")]
        public string Title { get; set; } // Title
		/// <summary>内容</summary>	
		[Description("内容")]
        public string Content { get; set; } // Content
		/// <summary></summary>	
		[Description("")]
        public string ContentText { get; set; } // ContentText
		/// <summary>附件ID</summary>	
		[Description("附件ID")]
        public string AttachFileIDs { get; set; } // AttachFileIDs
		/// <summary>消息连接</summary>	
		[Description("消息连接")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary>是否系统消息</summary>	
		[Description("是否系统消息")]
        public string IsSystemMsg { get; set; } // IsSystemMsg
		/// <summary>发送时间</summary>	
		[Description("发送时间")]
        public DateTime? SendTime { get; set; } // SendTime
		/// <summary>发送者ID</summary>	
		[Description("发送者ID")]
        public string SenderID { get; set; } // SenderID
		/// <summary>发送者姓名</summary>	
		[Description("发送者姓名")]
        public string SenderName { get; set; } // SenderName
		/// <summary>接收人ID</summary>	
		[Description("接收人ID")]
        public string ReceiverIDs { get; set; } // ReceiverIDs
		/// <summary>接收人姓名</summary>	
		[Description("接收人姓名")]
        public string ReceiverNames { get; set; } // ReceiverNames
		/// <summary>接受部门ID</summary>	
		[Description("接受部门ID")]
        public string ReceiverDeptIDs { get; set; } // ReceiverDeptIDs
		/// <summary>接受部门</summary>	
		[Description("接受部门")]
        public string ReceiverDeptNames { get; set; } // ReceiverDeptNames
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary>是否发送回执</summary>	
		[Description("是否发送回执")]
        public string IsReadReceipt { get; set; } // IsReadReceipt
		/// <summary>重要性</summary>	
		[Description("重要性")]
        public string Importance { get; set; } // Importance

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<S_S_MsgReceiver> S_S_MsgReceiver { get { onS_S_MsgReceiverGetting(); return _S_S_MsgReceiver;} }
		private ICollection<S_S_MsgReceiver> _S_S_MsgReceiver;
		partial void onS_S_MsgReceiverGetting();


        public S_S_MsgBody()
        {
			IsSystemMsg = "0";
			IsDeleted = "0";
			IsReadReceipt = "0";
			Importance = "0";
            _S_S_MsgReceiver = new List<S_S_MsgReceiver>();
        }
    }

	/// <summary>短消息接收人表</summary>	
	[Description("短消息接收人表")]
    public partial class S_S_MsgReceiver : Formula.BaseModel
    {
		/// <summary>ID</summary>	
		[Description("ID")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>消息体ID</summary>	
		[Description("消息体ID")]
        public string MsgBodyID { get; set; } // MsgBodyID
		/// <summary>接收人ID</summary>	
		[Description("接收人ID")]
        public string UserID { get; set; } // UserID
		/// <summary>接收人姓名</summary>	
		[Description("接收人姓名")]
        public string UserName { get; set; } // UserName
		/// <summary>首次查看时间</summary>	
		[Description("首次查看时间")]
        public DateTime? FirstViewTime { get; set; } // FirstViewTime
		/// <summary>回复时间</summary>	
		[Description("回复时间")]
        public DateTime? ReplyTime { get; set; } // ReplyTime
		/// <summary>是否已经删除</summary>	
		[Description("是否已经删除")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary>删除时间</summary>	
		[Description("删除时间")]
        public DateTime? DeleteTime { get; set; } // DeleteTime

        // Foreign keys
		[JsonIgnore]
        public virtual S_S_MsgBody S_S_MsgBody { get; set; } //  MsgBodyID - FK_S_S_MsgReceiver_S_S_MsgBody

        public S_S_MsgReceiver()
        {
			IsDeleted = "0";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Form : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Category { get; set; } // Category
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string TableName { get; set; } // TableName
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string Script { get; set; } // Script
		/// <summary></summary>	
		[Description("")]
        public string ScriptText { get; set; } // ScriptText
		/// <summary></summary>	
		[Description("")]
        public string FlowLogic { get; set; } // FlowLogic
		/// <summary></summary>	
		[Description("")]
        public string HiddenFields { get; set; } // HiddenFields
		/// <summary></summary>	
		[Description("")]
        public string Layout { get; set; } // Layout
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string Setttings { get; set; } // Setttings
		/// <summary></summary>	
		[Description("")]
        public string SerialNumberSettings { get; set; } // SerialNumberSettings
		/// <summary></summary>	
		[Description("")]
        public string DefaultValueSettings { get; set; } // DefaultValueSettings
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public DateTime? ReleaseTime { get; set; } // ReleaseTime
		/// <summary></summary>	
		[Description("")]
        public string ReleasedData { get; set; } // ReleasedData
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_List : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string TableNames { get; set; } // TableNames
		/// <summary></summary>	
		[Description("")]
        public string Script { get; set; } // Script
		/// <summary></summary>	
		[Description("")]
        public string ScriptText { get; set; } // ScriptText
		/// <summary></summary>	
		[Description("")]
        public string HasRowNumber { get; set; } // HasRowNumber
		/// <summary></summary>	
		[Description("")]
        public string LayoutGrid { get; set; } // LayoutGrid
		/// <summary></summary>	
		[Description("")]
        public string LayoutField { get; set; } // LayoutField
		/// <summary></summary>	
		[Description("")]
        public string LayoutSearch { get; set; } // LayoutSearch
		/// <summary></summary>	
		[Description("")]
        public string LayoutButton { get; set; } // LayoutButton
		/// <summary></summary>	
		[Description("")]
        public string Settings { get; set; } // Settings
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string Released { get; set; } // Released
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Selector : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string URLSingle { get; set; } // URLSingle
		/// <summary></summary>	
		[Description("")]
        public string URLMulti { get; set; } // URLMulti
		/// <summary></summary>	
		[Description("")]
        public string Width { get; set; } // Width
		/// <summary></summary>	
		[Description("")]
        public string Height { get; set; } // Height
		/// <summary></summary>	
		[Description("")]
        public string Title { get; set; } // Title
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_SerialNumber : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string YearCode { get; set; } // YearCode
		/// <summary></summary>	
		[Description("")]
        public string MonthCode { get; set; } // MonthCode
		/// <summary></summary>	
		[Description("")]
        public string DayCode { get; set; } // DayCode
		/// <summary></summary>	
		[Description("")]
        public string CategoryCode { get; set; } // CategoryCode
		/// <summary></summary>	
		[Description("")]
        public string SubCategoryCode { get; set; } // SubCategoryCode
		/// <summary></summary>	
		[Description("")]
        public string OrderNumCode { get; set; } // OrderNumCode
		/// <summary></summary>	
		[Description("")]
        public string PrjCode { get; set; } // PrjCode
		/// <summary></summary>	
		[Description("")]
        public string OrgCode { get; set; } // OrgCode
		/// <summary></summary>	
		[Description("")]
        public string UserCode { get; set; } // UserCode
		/// <summary></summary>	
		[Description("")]
        public int? Number { get; set; } // Number
    }

	/// <summary></summary>	
	[Description("")]
    public partial class S_UI_Word : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Code { get; set; } // Code
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ConnName { get; set; } // ConnName
		/// <summary></summary>	
		[Description("")]
        public string SQL { get; set; } // SQL
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
		/// <summary></summary>	
		[Description("")]
        public string Items { get; set; } // Items
		/// <summary></summary>	
		[Description("")]
        public string CategoryID { get; set; } // CategoryID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary></summary>	
		[Description("")]
        public string ModifyUserName { get; set; } // ModifyUserName
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyTime { get; set; } // ModifyTime
    }


    // ************************************************************************
    // POCO Configuration

    // S_A__OrgRes
    internal partial class S_A__OrgResConfiguration : EntityTypeConfiguration<S_A__OrgRes>
    {
        public S_A__OrgResConfiguration()
        {
			ToTable("S_A__ORGRES");
            HasKey(x => new { x.ResID, x.OrgID });

            Property(x => x.ResID).HasColumnName("RESID").IsRequired().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Res).WithMany(b => b.S_A__OrgRes).HasForeignKey(c => c.ResID); // FK_S_A__OrgRes_S_A_Res
            HasRequired(a => a.S_A_Org).WithMany(b => b.S_A__OrgRes).HasForeignKey(c => c.OrgID); // FK_S_A__OrgRes_S_A_Org
        }
    }

    // S_A__OrgRole
    internal partial class S_A__OrgRoleConfiguration : EntityTypeConfiguration<S_A__OrgRole>
    {
        public S_A__OrgRoleConfiguration()
        {
			ToTable("S_A__ORGROLE");
            HasKey(x => new { x.RoleID, x.OrgID });

            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);
            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__OrgRole).HasForeignKey(c => c.RoleID); // FK_A_OrgRole_ARole
            HasRequired(a => a.S_A_Org).WithMany(b => b.S_A__OrgRole).HasForeignKey(c => c.OrgID); // FK_A_OrgRole_AOrg
        }
    }

    // S_A__OrgRoleUser
    internal partial class S_A__OrgRoleUserConfiguration : EntityTypeConfiguration<S_A__OrgRoleUser>
    {
        public S_A__OrgRoleUserConfiguration()
        {
			ToTable("S_A__ORGROLEUSER");
            HasKey(x => new { x.OrgID, x.RoleID, x.UserID });

            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__OrgRoleUser).HasForeignKey(c => c.RoleID); // FK_S_A__OrgRoleUser_S_A_Role
        }
    }

    // S_A__OrgUser
    internal partial class S_A__OrgUserConfiguration : EntityTypeConfiguration<S_A__OrgUser>
    {
        public S_A__OrgUserConfiguration()
        {
			ToTable("S_A__ORGUSER");
            HasKey(x => new { x.OrgID, x.UserID });

            Property(x => x.OrgID).HasColumnName("ORGID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Org).WithMany(b => b.S_A__OrgUser).HasForeignKey(c => c.OrgID); // FK_A_OrgUser_AOrg
            HasRequired(a => a.S_A_User).WithMany(b => b.S_A__OrgUser).HasForeignKey(c => c.UserID); // FK_A_OrgUser_AUser
        }
    }

    // S_A__RoleRes
    internal partial class S_A__RoleResConfiguration : EntityTypeConfiguration<S_A__RoleRes>
    {
        public S_A__RoleResConfiguration()
        {
			ToTable("S_A__ROLERES");
            HasKey(x => new { x.ResID, x.RoleID });

            Property(x => x.ResID).HasColumnName("RESID").IsRequired().HasMaxLength(50);
            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Res).WithMany(b => b.S_A__RoleRes).HasForeignKey(c => c.ResID); // FK_S_A__RoleRes_S_A_Res
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__RoleRes).HasForeignKey(c => c.RoleID); // FK_S_A__RoleRes_S_A_Role
        }
    }

    // S_A__RoleUser
    internal partial class S_A__RoleUserConfiguration : EntityTypeConfiguration<S_A__RoleUser>
    {
        public S_A__RoleUserConfiguration()
        {
			ToTable("S_A__ROLEUSER");
            HasKey(x => new { x.RoleID, x.UserID });

            Property(x => x.RoleID).HasColumnName("ROLEID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_Role).WithMany(b => b.S_A__RoleUser).HasForeignKey(c => c.RoleID); // FK_A_RoleUser_ARole
            HasRequired(a => a.S_A_User).WithMany(b => b.S_A__RoleUser).HasForeignKey(c => c.UserID); // FK_A_RoleUser_AUser
        }
    }

    // S_A__UserRes
    internal partial class S_A__UserResConfiguration : EntityTypeConfiguration<S_A__UserRes>
    {
        public S_A__UserResConfiguration()
        {
			ToTable("S_A__USERRES");
            HasKey(x => new { x.UserID, x.ResID });

            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(50);
            Property(x => x.ResID).HasColumnName("RESID").IsRequired().HasMaxLength(50);
            Property(x => x.DenyAuth).HasColumnName("DENYAUTH").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.S_A_User).WithMany(b => b.S_A__UserRes).HasForeignKey(c => c.UserID); // FK_S_A__UserRes_S_A_User
            HasRequired(a => a.S_A_Res).WithMany(b => b.S_A__UserRes).HasForeignKey(c => c.ResID); // FK_S_A__UserRes_S_A_Res
        }
    }

    // S_A_AuthInfo
    internal partial class S_A_AuthInfoConfiguration : EntityTypeConfiguration<S_A_AuthInfo>
    {
        public S_A_AuthInfoConfiguration()
        {
			ToTable("S_A_AUTHINFO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.OrgRootFullID).HasColumnName("ORGROOTFULLID").IsOptional().HasMaxLength(50);
            Property(x => x.ResRootFullID).HasColumnName("RESROOTFULLID").IsOptional().HasMaxLength(50);
            Property(x => x.RoleGroupID).HasColumnName("ROLEGROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.UserGroupID).HasColumnName("USERGROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_A_AuthLevel
    internal partial class S_A_AuthLevelConfiguration : EntityTypeConfiguration<S_A_AuthLevel>
    {
        public S_A_AuthLevelConfiguration()
        {
			ToTable("S_A_AUTHLEVEL");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.MenuRootFullID).HasColumnName("MENUROOTFULLID").IsOptional().HasMaxLength(2000);
            Property(x => x.MenuRootName).HasColumnName("MENUROOTNAME").IsOptional().HasMaxLength(200);
            Property(x => x.RuleRootFullID).HasColumnName("RULEROOTFULLID").IsOptional().HasMaxLength(2000);
            Property(x => x.RuleRootName).HasColumnName("RULEROOTNAME").IsOptional().HasMaxLength(200);
        }
    }

    // S_A_AuthLog
    internal partial class S_A_AuthLogConfiguration : EntityTypeConfiguration<S_A_AuthLog>
    {
        public S_A_AuthLogConfiguration()
        {
			ToTable("S_A_AUTHLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Operation).HasColumnName("OPERATION").IsOptional().HasMaxLength(50);
            Property(x => x.OperationTarget).HasColumnName("OPERATIONTARGET").IsOptional().HasMaxLength(50);
            Property(x => x.RelateData).HasColumnName("RELATEDATA").IsOptional();
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ClientIP).HasColumnName("CLIENTIP").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_Org
    internal partial class S_A_OrgConfiguration : EntityTypeConfiguration<S_A_Org>
    {
        public S_A_OrgConfiguration()
        {
			ToTable("S_A_ORG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsRequired().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.ShortName).HasColumnName("SHORTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Character).HasColumnName("CHARACTER").IsOptional().HasMaxLength(500);
            Property(x => x.Location).HasColumnName("LOCATION").IsOptional().HasMaxLength(50);
        }
    }

    // S_A_Res
    internal partial class S_A_ResConfiguration : EntityTypeConfiguration<S_A_Res>
    {
        public S_A_ResConfiguration()
        {
			ToTable("S_A_RES");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(200);
            Property(x => x.IconUrl).HasColumnName("ICONURL").IsOptional().HasMaxLength(200);
            Property(x => x.Target).HasColumnName("TARGET").IsOptional().HasMaxLength(50);
            Property(x => x.ButtonID).HasColumnName("BUTTONID").IsOptional().HasMaxLength(50);
            Property(x => x.DataFilter).HasColumnName("DATAFILTER").IsOptional().HasMaxLength(2000);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // S_A_Role
    internal partial class S_A_RoleConfiguration : EntityTypeConfiguration<S_A_Role>
    {
        public S_A_RoleConfiguration()
        {
			ToTable("S_A_ROLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_A_Security
    internal partial class S_A_SecurityConfiguration : EntityTypeConfiguration<S_A_Security>
    {
        public S_A_SecurityConfiguration()
        {
			ToTable("S_A_SECURITY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.SuperAdmin).HasColumnName("SUPERADMIN").IsOptional().HasMaxLength(50);
            Property(x => x.SuperAdminPwd).HasColumnName("SUPERADMINPWD").IsOptional().HasMaxLength(50);
            Property(x => x.SuperAdminSecurity).HasColumnName("SUPERADMINSECURITY").IsOptional().HasMaxLength(500);
            Property(x => x.SuperAdminModifyTime).HasColumnName("SUPERADMINMODIFYTIME").IsOptional();
            Property(x => x.AdminIDs).HasColumnName("ADMINIDS").IsOptional().HasMaxLength(500);
            Property(x => x.AdminNames).HasColumnName("ADMINNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.AdminModifyTime).HasColumnName("ADMINMODIFYTIME").IsOptional();
            Property(x => x.AdminSecurity).HasColumnName("ADMINSECURITY").IsOptional().HasMaxLength(500);
        }
    }

    // S_A_User
    internal partial class S_A_UserConfiguration : EntityTypeConfiguration<S_A_User>
    {
        public S_A_UserConfiguration()
        {
			ToTable("S_A_USER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.WorkNo).HasColumnName("WORKNO").IsOptional().HasMaxLength(50);
            Property(x => x.Password).HasColumnName("PASSWORD").IsOptional().HasMaxLength(50);
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.InDate).HasColumnName("INDATE").IsOptional();
            Property(x => x.OutDate).HasColumnName("OUTDATE").IsOptional();
            Property(x => x.Phone).HasColumnName("PHONE").IsOptional().HasMaxLength(50);
            Property(x => x.MobilePhone).HasColumnName("MOBILEPHONE").IsOptional().HasMaxLength(50);
            Property(x => x.Email).HasColumnName("EMAIL").IsOptional().HasMaxLength(50);
            Property(x => x.Address).HasColumnName("ADDRESS").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.LastLoginTime).HasColumnName("LASTLOGINTIME").IsOptional().HasMaxLength(50);
            Property(x => x.LastLoginIP).HasColumnName("LASTLOGINIP").IsOptional().HasMaxLength(50);
            Property(x => x.LastSessionID).HasColumnName("LASTSESSIONID").IsOptional().HasMaxLength(50);
            Property(x => x.ErrorCount).HasColumnName("ERRORCOUNT").IsOptional();
            Property(x => x.ErrorTime).HasColumnName("ERRORTIME").IsOptional();
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsRequired().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.PrjID).HasColumnName("PRJID").IsOptional().HasMaxLength(50);
            Property(x => x.PrjName).HasColumnName("PRJNAME").IsOptional().HasMaxLength(200);
            Property(x => x.DeptID).HasColumnName("DEPTID").IsOptional().HasMaxLength(50);
            Property(x => x.DeptFullID).HasColumnName("DEPTFULLID").IsOptional().HasMaxLength(500);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.RTX).HasColumnName("RTX").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
        }
    }

    // S_A_UserImg
    internal partial class S_A_UserImgConfiguration : EntityTypeConfiguration<S_A_UserImg>
    {
        public S_A_UserImgConfiguration()
        {
			ToTable("S_A_USERIMG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.SignImg).HasColumnName("SIGNIMG").IsOptional().HasMaxLength(2147483647);
            Property(x => x.Picture).HasColumnName("PICTURE").IsOptional().HasMaxLength(2147483647);

            // Foreign keys
            HasOptional(a => a.S_A_User).WithMany(b => b.S_A_UserImg).HasForeignKey(c => c.UserID); // FK_S_A_UserImg_S_A_User
        }
    }

    // S_A_UserLinkMan
    internal partial class S_A_UserLinkManConfiguration : EntityTypeConfiguration<S_A_UserLinkMan>
    {
        public S_A_UserLinkManConfiguration()
        {
			ToTable("S_A_USERLINKMAN");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.LinkManID).HasColumnName("LINKMANID").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_A_User).WithMany(b => b.S_A_UserLinkMan).HasForeignKey(c => c.UserID); // FK_S_A_UserLinkMan_S_A_User
        }
    }

    // S_C_Holiday
    internal partial class S_C_HolidayConfiguration : EntityTypeConfiguration<S_C_Holiday>
    {
        public S_C_HolidayConfiguration()
        {
			ToTable("S_C_HOLIDAY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional();
            Property(x => x.Month).HasColumnName("MONTH").IsOptional();
            Property(x => x.Day).HasColumnName("DAY").IsOptional();
            Property(x => x.Date).HasColumnName("DATE").IsOptional();
            Property(x => x.DayOfWeek).HasColumnName("DAYOFWEEK").IsOptional().HasMaxLength(50);
            Property(x => x.IsHoliday).HasColumnName("ISHOLIDAY").IsOptional().HasMaxLength(1);
        }
    }

    // S_D_FormToPDFRegist
    internal partial class S_D_FormToPDFRegistConfiguration : EntityTypeConfiguration<S_D_FormToPDFRegist>
    {
        public S_D_FormToPDFRegistConfiguration()
        {
			ToTable("S_D_FORMTOPDFREGIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FormName).HasColumnName("FORMNAME").IsOptional().HasMaxLength(200);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(200);
            Property(x => x.TempCode).HasColumnName("TEMPCODE").IsOptional().HasMaxLength(200);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(200);
        }
    }

    // S_D_FormToPDFTask
    internal partial class S_D_FormToPDFTaskConfiguration : EntityTypeConfiguration<S_D_FormToPDFTask>
    {
        public S_D_FormToPDFTaskConfiguration()
        {
			ToTable("S_D_FORMTOPDFTASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TempCode).HasColumnName("TEMPCODE").IsOptional().HasMaxLength(100);
            Property(x => x.FormID).HasColumnName("FORMID").IsOptional().HasMaxLength(100);
            Property(x => x.PDFFileID).HasColumnName("PDFFILEID").IsOptional().HasMaxLength(100);
            Property(x => x.FormLastModifyDate).HasColumnName("FORMLASTMODIFYDATE").IsOptional();
            Property(x => x.BeginTime).HasColumnName("BEGINTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.DoneLog).HasColumnName("DONELOG").IsOptional();
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
        }
    }

    // S_D_ModifyLog
    internal partial class S_D_ModifyLogConfiguration : EntityTypeConfiguration<S_D_ModifyLog>
    {
        public S_D_ModifyLogConfiguration()
        {
			ToTable("S_D_MODIFYLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyMode).HasColumnName("MODIFYMODE").IsOptional().HasMaxLength(50);
            Property(x => x.EntityKey).HasColumnName("ENTITYKEY").IsOptional().HasMaxLength(200);
            Property(x => x.CurrentValue).HasColumnName("CURRENTVALUE").IsOptional().HasMaxLength(2000);
            Property(x => x.OriginalValue).HasColumnName("ORIGINALVALUE").IsOptional().HasMaxLength(2000);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ClientIP).HasColumnName("CLIENTIP").IsOptional().HasMaxLength(50);
            Property(x => x.UserHostAddress).HasColumnName("USERHOSTADDRESS").IsOptional().HasMaxLength(50);
        }
    }

    // S_D_PDFTask
    internal partial class S_D_PDFTaskConfiguration : EntityTypeConfiguration<S_D_PDFTask>
    {
        public S_D_PDFTaskConfiguration()
        {
			ToTable("S_D_PDFTASK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(50);
            Property(x => x.FileType).HasColumnName("FILETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.PDFFileID).HasColumnName("PDFFILEID").IsOptional().HasMaxLength(50);
            Property(x => x.SWFFileID).HasColumnName("SWFFILEID").IsOptional().HasMaxLength(50);
            Property(x => x.SnapFileID).HasColumnName("SNAPFILEID").IsOptional().HasMaxLength(50);
            Property(x => x.PDFPageCount).HasColumnName("PDFPAGECOUNT").IsRequired();
            Property(x => x.IsSplit).HasColumnName("ISSPLIT").IsRequired();
            Property(x => x.Status).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
            Property(x => x.StartTime).HasColumnName("STARTTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
        }
    }

    // S_H_Calendar
    internal partial class S_H_CalendarConfiguration : EntityTypeConfiguration<S_H_Calendar>
    {
        public S_H_CalendarConfiguration()
        {
			ToTable("S_H_CALENDAR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(4000);
            Property(x => x.StartTime).HasColumnName("STARTTIME").IsOptional();
            Property(x => x.EndTime).HasColumnName("ENDTIME").IsOptional();
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(4000);
            Property(x => x.Grade).HasColumnName("GRADE").IsOptional().HasMaxLength(20);
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional().HasMaxLength(4000);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.Sponsor).HasColumnName("SPONSOR").IsOptional().HasMaxLength(50);
            Property(x => x.SponsorID).HasColumnName("SPONSORID").IsOptional().HasMaxLength(50);
            Property(x => x.Participators).HasColumnName("PARTICIPATORS").IsOptional();
            Property(x => x.ParticipatorsID).HasColumnName("PARTICIPATORSID").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_H_Feedback
    internal partial class S_H_FeedbackConfiguration : EntityTypeConfiguration<S_H_Feedback>
    {
        public S_H_FeedbackConfiguration()
        {
			ToTable("S_H_FEEDBACK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.Url).HasColumnName("URL").IsOptional();
            Property(x => x.Attachment).HasColumnName("ATTACHMENT").IsOptional().HasMaxLength(4000);
            Property(x => x.IsUse).HasColumnName("ISUSE").IsOptional().HasMaxLength(1);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
        }
    }

    // S_H_ShortCut
    internal partial class S_H_ShortCutConfiguration : EntityTypeConfiguration<S_H_ShortCut>
    {
        public S_H_ShortCutConfiguration()
        {
			ToTable("S_H_SHORTCUT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional();
            Property(x => x.IconImage).HasColumnName("ICONIMAGE").IsOptional().HasMaxLength(250);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.IsUse).HasColumnName("ISUSE").IsOptional().HasMaxLength(1);
            Property(x => x.PageIndex).HasColumnName("PAGEINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_FriendLink
    internal partial class S_I_FriendLinkConfiguration : EntityTypeConfiguration<S_I_FriendLink>
    {
        public S_I_FriendLinkConfiguration()
        {
			ToTable("S_I_FRIENDLINK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Icon).HasColumnName("ICON").IsOptional().HasMaxLength(100);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(200);
            Property(x => x.DeptId).HasColumnName("DEPTID").IsOptional().HasMaxLength(2000);
            Property(x => x.DeptName).HasColumnName("DEPTNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.UserId).HasColumnName("USERID").IsOptional().HasMaxLength(2000);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(4000);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_NewsImage
    internal partial class S_I_NewsImageConfiguration : EntityTypeConfiguration<S_I_NewsImage>
    {
        public S_I_NewsImageConfiguration()
        {
			ToTable("S_I_NEWSIMAGE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(50);
            Property(x => x.PictureName).HasColumnName("PICTURENAME").IsOptional().HasMaxLength(500);
            Property(x => x.PictureEntire).HasColumnName("PICTUREENTIRE").IsOptional().HasMaxLength(2147483647);
            Property(x => x.PictureThumb).HasColumnName("PICTURETHUMB").IsOptional().HasMaxLength(2147483647);
            Property(x => x.Src).HasColumnName("SRC").IsOptional().HasMaxLength(500);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_NewsImageGroup
    internal partial class S_I_NewsImageGroupConfiguration : EntityTypeConfiguration<S_I_NewsImageGroup>
    {
        public S_I_NewsImageGroupConfiguration()
        {
			ToTable("S_I_NEWSIMAGEGROUP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(2000);
            Property(x => x.DeptDoorId).HasColumnName("DEPTDOORID").IsOptional().HasMaxLength(200);
            Property(x => x.DeptDoorName).HasColumnName("DEPTDOORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_PublicInformation
    internal partial class S_I_PublicInformationConfiguration : EntityTypeConfiguration<S_I_PublicInformation>
    {
        public S_I_PublicInformationConfiguration()
        {
			ToTable("S_I_PUBLICINFORMATION");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CatalogId).HasColumnName("CATALOGID").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ContentText).HasColumnName("CONTENTTEXT").IsOptional();
            Property(x => x.Attachments).HasColumnName("ATTACHMENTS").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveDeptId).HasColumnName("RECEIVEDEPTID").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveDeptName).HasColumnName("RECEIVEDEPTNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveUserId).HasColumnName("RECEIVEUSERID").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiveUserName).HasColumnName("RECEIVEUSERNAME").IsOptional().HasMaxLength(2000);
            Property(x => x.DeptDoorId).HasColumnName("DEPTDOORID").IsOptional().HasMaxLength(200);
            Property(x => x.DeptDoorName).HasColumnName("DEPTDOORNAME").IsOptional().HasMaxLength(200);
            Property(x => x.ExpiresTime).HasColumnName("EXPIRESTIME").IsOptional();
            Property(x => x.ReadCount).HasColumnName("READCOUNT").IsOptional();
            Property(x => x.Important).HasColumnName("IMPORTANT").IsOptional().HasMaxLength(50);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.IsTop).HasColumnName("ISTOP").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_I_PublicInformCatalog
    internal partial class S_I_PublicInformCatalogConfiguration : EntityTypeConfiguration<S_I_PublicInformCatalog>
    {
        public S_I_PublicInformCatalogConfiguration()
        {
			ToTable("S_I_PUBLICINFORMCATALOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.CatalogName).HasColumnName("CATALOGNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CatalogKey).HasColumnName("CATALOGKEY").IsOptional().HasMaxLength(50);
            Property(x => x.IsOnHomePage).HasColumnName("ISONHOMEPAGE").IsOptional().HasMaxLength(1);
            Property(x => x.InHomePageNum).HasColumnName("INHOMEPAGENUM").IsOptional();
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.CreateUserName).HasColumnName("CREATEUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_Category
    internal partial class S_M_CategoryConfiguration : EntityTypeConfiguration<S_M_Category>
    {
        public S_M_CategoryConfiguration()
        {
			ToTable("S_M_CATEGORY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_M_EnumDef
    internal partial class S_M_EnumDefConfiguration : EntityTypeConfiguration<S_M_EnumDef>
    {
        public S_M_EnumDefConfiguration()
        {
			ToTable("S_M_ENUMDEF");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Sql).HasColumnName("SQL").IsOptional().HasMaxLength(500);
            Property(x => x.Orderby).HasColumnName("ORDERBY").IsOptional().HasMaxLength(200);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_M_EnumItem
    internal partial class S_M_EnumItemConfiguration : EntityTypeConfiguration<S_M_EnumItem>
    {
        public S_M_EnumItemConfiguration()
        {
			ToTable("S_M_ENUMITEM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.EnumDefID).HasColumnName("ENUMDEFID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SubEnumDefCode).HasColumnName("SUBENUMDEFCODE").IsOptional().HasMaxLength(50);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.SubCategory).HasColumnName("SUBCATEGORY").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_M_EnumDef).WithMany(b => b.S_M_EnumItem).HasForeignKey(c => c.EnumDefID); // FK_EnumItem_EnumDef
        }
    }

    // S_M_Field
    internal partial class S_M_FieldConfiguration : EntityTypeConfiguration<S_M_Field>
    {
        public S_M_FieldConfiguration()
        {
			ToTable("S_M_FIELD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.TableID).HasColumnName("TABLEID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsRequired();
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.S_M_Table).WithMany(b => b.S_M_Field).HasForeignKey(c => c.TableID); // FK_S_M_Field_S_M_Table
        }
    }

    // S_M_Table
    internal partial class S_M_TableConfiguration : EntityTypeConfiguration<S_M_Table>
    {
        public S_M_TableConfiguration()
        {
			ToTable("S_M_TABLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // S_P_DoorBaseTemplate
    internal partial class S_P_DoorBaseTemplateConfiguration : EntityTypeConfiguration<S_P_DoorBaseTemplate>
    {
        public S_P_DoorBaseTemplateConfiguration()
        {
			ToTable("S_P_DOORBASETEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.BaseType).HasColumnName("BASETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.IsDefault).HasColumnName("ISDEFAULT").IsOptional().HasMaxLength(1);
            Property(x => x.TemplateColWidth).HasColumnName("TEMPLATECOLWIDTH").IsOptional().HasMaxLength(100);
            Property(x => x.TemplateString).HasColumnName("TEMPLATESTRING").IsOptional().HasMaxLength(500);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.IsEdit).HasColumnName("ISEDIT").IsOptional().HasMaxLength(50);
        }
    }

    // S_P_DoorBlock
    internal partial class S_P_DoorBlockConfiguration : EntityTypeConfiguration<S_P_DoorBlock>
    {
        public S_P_DoorBlockConfiguration()
        {
			ToTable("S_P_DOORBLOCK");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.BlockName).HasColumnName("BLOCKNAME").IsOptional().HasMaxLength(50);
            Property(x => x.BlockKey).HasColumnName("BLOCKKEY").IsOptional().HasMaxLength(50);
            Property(x => x.BlockTitle).HasColumnName("BLOCKTITLE").IsOptional().HasMaxLength(50);
            Property(x => x.BlockType).HasColumnName("BLOCKTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.BlockImage).HasColumnName("BLOCKIMAGE").IsOptional().HasMaxLength(100);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(400);
            Property(x => x.HeadHtml).HasColumnName("HEADHTML").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ColorValue).HasColumnName("COLORVALUE").IsOptional().HasMaxLength(10);
            Property(x => x.Color).HasColumnName("COLOR").IsOptional().HasMaxLength(50);
            Property(x => x.RepeatItemCount).HasColumnName("REPEATITEMCOUNT").IsOptional();
            Property(x => x.RepeatItemLength).HasColumnName("REPEATITEMLENGTH").IsOptional();
            Property(x => x.RepeatDataDataSql).HasColumnName("REPEATDATADATASQL").IsOptional().HasMaxLength(1073741823);
            Property(x => x.RepeatItemTemplate).HasColumnName("REPEATITEMTEMPLATE").IsOptional().HasMaxLength(1073741823);
            Property(x => x.FootHtml).HasColumnName("FOOTHTML").IsOptional().HasMaxLength(1073741823);
            Property(x => x.DelayLoadSecond).HasColumnName("DELAYLOADSECOND").IsOptional();
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.RelateScript).HasColumnName("RELATESCRIPT").IsOptional().HasMaxLength(2000);
            Property(x => x.IsHidden).HasColumnName("ISHIDDEN").IsOptional().HasMaxLength(50);
            Property(x => x.TemplateId).HasColumnName("TEMPLATEID").IsOptional().HasMaxLength(50);
            Property(x => x.AllowUserIDs).HasColumnName("ALLOWUSERIDS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.AllowUserNames).HasColumnName("ALLOWUSERNAMES").IsOptional().HasMaxLength(1073741823);
            Property(x => x.AllowTypes).HasColumnName("ALLOWTYPES").IsOptional().HasMaxLength(1073741823);
            Property(x => x.IsEdit).HasColumnName("ISEDIT").IsOptional().HasMaxLength(50);
        }
    }

    // S_P_DoorTemplate
    internal partial class S_P_DoorTemplateConfiguration : EntityTypeConfiguration<S_P_DoorTemplate>
    {
        public S_P_DoorTemplateConfiguration()
        {
			ToTable("S_P_DOORTEMPLATE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.BaseType).HasColumnName("BASETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(100);
            Property(x => x.IsDefault).HasColumnName("ISDEFAULT").IsOptional().HasMaxLength(1);
            Property(x => x.TemplateColWidth).HasColumnName("TEMPLATECOLWIDTH").IsOptional().HasMaxLength(100);
            Property(x => x.TemplateString).HasColumnName("TEMPLATESTRING").IsOptional().HasMaxLength(500);
            Property(x => x.BaseTemplateId).HasColumnName("BASETEMPLATEID").IsOptional().HasMaxLength(50);
        }
    }

    // S_R_DataSet
    internal partial class S_R_DataSetConfiguration : EntityTypeConfiguration<S_R_DataSet>
    {
        public S_R_DataSetConfiguration()
        {
			ToTable("S_R_DATASET");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DefineID).HasColumnName("DEFINEID").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional().HasMaxLength(200);
            Property(x => x.Sql).HasColumnName("SQL").IsOptional().HasMaxLength(2000);

            // Foreign keys
            HasOptional(a => a.S_R_Define).WithMany(b => b.S_R_DataSet).HasForeignKey(c => c.DefineID); // FK_S_R_DataSet_S_R_Define
        }
    }

    // S_R_Define
    internal partial class S_R_DefineConfiguration : EntityTypeConfiguration<S_R_Define>
    {
        public S_R_DefineConfiguration()
        {
			ToTable("S_R_DEFINE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_R_Field
    internal partial class S_R_FieldConfiguration : EntityTypeConfiguration<S_R_Field>
    {
        public S_R_FieldConfiguration()
        {
			ToTable("S_R_FIELD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.DataSetID).HasColumnName("DATASETID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.EnumKey).HasColumnName("ENUMKEY").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.S_R_DataSet).WithMany(b => b.S_R_Field).HasForeignKey(c => c.DataSetID); // FK_S_R_Field_S_R_DataSet
        }
    }

    // S_RC_RuleCode
    internal partial class S_RC_RuleCodeConfiguration : EntityTypeConfiguration<S_RC_RuleCode>
    {
        public S_RC_RuleCodeConfiguration()
        {
			ToTable("S_RC_RULECODE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(72);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(100);
            Property(x => x.RuleName).HasColumnName("RULENAME").IsOptional().HasMaxLength(400);
            Property(x => x.Prefix).HasColumnName("PREFIX").IsOptional().HasMaxLength(100);
            Property(x => x.PostFix).HasColumnName("POSTFIX").IsOptional().HasMaxLength(100);
            Property(x => x.Seperative).HasColumnName("SEPERATIVE").IsOptional().HasMaxLength(100);
            Property(x => x.Digit).HasColumnName("DIGIT").IsOptional().HasPrecision(10,0);
            Property(x => x.StartNumber).HasColumnName("STARTNUMBER").IsOptional().HasPrecision(10,0);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(100);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(100);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
        }
    }

    // S_RC_RuleCodeData
    internal partial class S_RC_RuleCodeDataConfiguration : EntityTypeConfiguration<S_RC_RuleCodeData>
    {
        public S_RC_RuleCodeDataConfiguration()
        {
			ToTable("S_RC_RULECODEDATA");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(72);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(100);
            Property(x => x.Year).HasColumnName("YEAR").IsOptional().HasPrecision(10,0);
            Property(x => x.AutoNumber).HasColumnName("AUTONUMBER").IsOptional().HasPrecision(19,0);
        }
    }

    // S_S_Alarm
    internal partial class S_S_AlarmConfiguration : EntityTypeConfiguration<S_S_Alarm>
    {
        public S_S_AlarmConfiguration()
        {
			ToTable("S_S_ALARM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Important).HasColumnName("IMPORTANT").IsOptional().HasMaxLength(50);
            Property(x => x.Urgency).HasColumnName("URGENCY").IsOptional().HasMaxLength(50);
            Property(x => x.AlarmType).HasColumnName("ALARMTYPE").IsOptional().HasMaxLength(100);
            Property(x => x.AlarmTitle).HasColumnName("ALARMTITLE").IsOptional().HasMaxLength(200);
            Property(x => x.AlarmContent).HasColumnName("ALARMCONTENT").IsOptional();
            Property(x => x.AlarmUrl).HasColumnName("ALARMURL").IsOptional().HasMaxLength(200);
            Property(x => x.OwnerName).HasColumnName("OWNERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.OwnerID).HasColumnName("OWNERID").IsOptional().HasMaxLength(50);
            Property(x => x.SendTime).HasColumnName("SENDTIME").IsOptional();
            Property(x => x.DeadlineTime).HasColumnName("DEADLINETIME").IsOptional();
            Property(x => x.SenderName).HasColumnName("SENDERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SenderID).HasColumnName("SENDERID").IsOptional().HasMaxLength(50);
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional().HasMaxLength(50);
        }
    }

    // S_S_MsgBody
    internal partial class S_S_MsgBodyConfiguration : EntityTypeConfiguration<S_S_MsgBody>
    {
        public S_S_MsgBodyConfiguration()
        {
			ToTable("S_S_MSGBODY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ContentText).HasColumnName("CONTENTTEXT").IsOptional();
            Property(x => x.AttachFileIDs).HasColumnName("ATTACHFILEIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.IsSystemMsg).HasColumnName("ISSYSTEMMSG").IsOptional().HasMaxLength(1);
            Property(x => x.SendTime).HasColumnName("SENDTIME").IsOptional();
            Property(x => x.SenderID).HasColumnName("SENDERID").IsOptional().HasMaxLength(50);
            Property(x => x.SenderName).HasColumnName("SENDERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ReceiverIDs).HasColumnName("RECEIVERIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiverNames).HasColumnName("RECEIVERNAMES").IsOptional().HasMaxLength(2000);
            Property(x => x.ReceiverDeptIDs).HasColumnName("RECEIVERDEPTIDS").IsOptional().HasMaxLength(4000);
            Property(x => x.ReceiverDeptNames).HasColumnName("RECEIVERDEPTNAMES").IsOptional().HasMaxLength(4000);
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsRequired().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.IsReadReceipt).HasColumnName("ISREADRECEIPT").IsRequired().HasMaxLength(1);
            Property(x => x.Importance).HasColumnName("IMPORTANCE").IsRequired().HasMaxLength(1);
        }
    }

    // S_S_MsgReceiver
    internal partial class S_S_MsgReceiverConfiguration : EntityTypeConfiguration<S_S_MsgReceiver>
    {
        public S_S_MsgReceiverConfiguration()
        {
			ToTable("S_S_MSGRECEIVER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.MsgBodyID).HasColumnName("MSGBODYID").IsOptional().HasMaxLength(50);
            Property(x => x.UserID).HasColumnName("USERID").IsOptional().HasMaxLength(50);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.FirstViewTime).HasColumnName("FIRSTVIEWTIME").IsOptional();
            Property(x => x.ReplyTime).HasColumnName("REPLYTIME").IsOptional();
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsOptional().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();

            // Foreign keys
            HasOptional(a => a.S_S_MsgBody).WithMany(b => b.S_S_MsgReceiver).HasForeignKey(c => c.MsgBodyID); // FK_S_S_MsgReceiver_S_S_MsgBody
        }
    }

    // S_UI_Form
    internal partial class S_UI_FormConfiguration : EntityTypeConfiguration<S_UI_Form>
    {
        public S_UI_FormConfiguration()
        {
			ToTable("S_UI_FORM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Category).HasColumnName("CATEGORY").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.TableName).HasColumnName("TABLENAME").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.Script).HasColumnName("SCRIPT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ScriptText).HasColumnName("SCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.FlowLogic).HasColumnName("FLOWLOGIC").IsOptional().HasMaxLength(1073741823);
            Property(x => x.HiddenFields).HasColumnName("HIDDENFIELDS").IsOptional().HasMaxLength(500);
            Property(x => x.Layout).HasColumnName("LAYOUT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Setttings).HasColumnName("SETTTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.SerialNumberSettings).HasColumnName("SERIALNUMBERSETTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.DefaultValueSettings).HasColumnName("DEFAULTVALUESETTINGS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.ReleaseTime).HasColumnName("RELEASETIME").IsOptional();
            Property(x => x.ReleasedData).HasColumnName("RELEASEDDATA").IsOptional().HasMaxLength(1073741823);
        }
    }

    // S_UI_List
    internal partial class S_UI_ListConfiguration : EntityTypeConfiguration<S_UI_List>
    {
        public S_UI_ListConfiguration()
        {
			ToTable("S_UI_LIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional().HasMaxLength(2000);
            Property(x => x.TableNames).HasColumnName("TABLENAMES").IsOptional().HasMaxLength(500);
            Property(x => x.Script).HasColumnName("SCRIPT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ScriptText).HasColumnName("SCRIPTTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.HasRowNumber).HasColumnName("HASROWNUMBER").IsOptional().HasMaxLength(50);
            Property(x => x.LayoutGrid).HasColumnName("LAYOUTGRID").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutField).HasColumnName("LAYOUTFIELD").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutSearch).HasColumnName("LAYOUTSEARCH").IsOptional().HasMaxLength(1073741823);
            Property(x => x.LayoutButton).HasColumnName("LAYOUTBUTTON").IsOptional().HasMaxLength(1073741823);
            Property(x => x.Settings).HasColumnName("SETTINGS").IsOptional().HasMaxLength(2000);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.Released).HasColumnName("RELEASED").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_Selector
    internal partial class S_UI_SelectorConfiguration : EntityTypeConfiguration<S_UI_Selector>
    {
        public S_UI_SelectorConfiguration()
        {
			ToTable("S_UI_SELECTOR");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.URLSingle).HasColumnName("URLSINGLE").IsOptional().HasMaxLength(200);
            Property(x => x.URLMulti).HasColumnName("URLMULTI").IsOptional().HasMaxLength(200);
            Property(x => x.Width).HasColumnName("WIDTH").IsOptional().HasMaxLength(50);
            Property(x => x.Height).HasColumnName("HEIGHT").IsOptional().HasMaxLength(50);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
        }
    }

    // S_UI_SerialNumber
    internal partial class S_UI_SerialNumberConfiguration : EntityTypeConfiguration<S_UI_SerialNumber>
    {
        public S_UI_SerialNumberConfiguration()
        {
			ToTable("S_UI_SERIALNUMBER");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.YearCode).HasColumnName("YEARCODE").IsOptional().HasMaxLength(50);
            Property(x => x.MonthCode).HasColumnName("MONTHCODE").IsOptional().HasMaxLength(50);
            Property(x => x.DayCode).HasColumnName("DAYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.CategoryCode).HasColumnName("CATEGORYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.SubCategoryCode).HasColumnName("SUBCATEGORYCODE").IsOptional().HasMaxLength(50);
            Property(x => x.OrderNumCode).HasColumnName("ORDERNUMCODE").IsOptional().HasMaxLength(50);
            Property(x => x.PrjCode).HasColumnName("PRJCODE").IsOptional().HasMaxLength(50);
            Property(x => x.OrgCode).HasColumnName("ORGCODE").IsOptional().HasMaxLength(50);
            Property(x => x.UserCode).HasColumnName("USERCODE").IsOptional().HasMaxLength(50);
            Property(x => x.Number).HasColumnName("NUMBER").IsOptional();
        }
    }

    // S_UI_Word
    internal partial class S_UI_WordConfiguration : EntityTypeConfiguration<S_UI_Word>
    {
        public S_UI_WordConfiguration()
        {
			ToTable("S_UI_WORD");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.ConnName).HasColumnName("CONNNAME").IsOptional().HasMaxLength(50);
            Property(x => x.SQL).HasColumnName("SQL").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.Items).HasColumnName("ITEMS").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CategoryID).HasColumnName("CATEGORYID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUserName).HasColumnName("MODIFYUSERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyTime).HasColumnName("MODIFYTIME").IsOptional();
        }
    }

}

