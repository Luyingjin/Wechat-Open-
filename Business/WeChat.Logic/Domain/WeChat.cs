

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "WeChat"
//     Connection String:      "Data Source=pdata.mgcc.com.cn;Initial Catalog=MP_WeChat;User ID=sa;PWD=Password01!;"

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

namespace WeChat.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class WeChatEntities : Formula.FormulaDbContext
    {
        public IDbSet<MpAccount> MpAccount { get; set; } // MpAccount
        public IDbSet<MPAccountSpaceMenu> MPAccountSpaceMenu { get; set; } // MPAccountSpaceMenu
        public IDbSet<MpAccountUserRelation> MpAccountUserRelation { get; set; } // MpAccountUserRelation
        public IDbSet<MpEvent> MpEvent { get; set; } // MpEvent
        public IDbSet<MpEventClickViewLog> MpEventClickViewLog { get; set; } // MpEventClickViewLog
        public IDbSet<MpEventRequestMsgLog> MpEventRequestMsgLog { get; set; } // MpEventRequestMsgLog
        public IDbSet<MpEventScanLog> MpEventScanLog { get; set; } // MpEventScanLog
        public IDbSet<MpFans> MpFans { get; set; } // MpFans
        public IDbSet<MpGroup> MpGroup { get; set; } // MpGroup
        public IDbSet<MpJssdkWhiteList> MpJssdkWhiteList { get; set; } // MpJssdkWhiteList
        public IDbSet<MpKeyWordReply> MpKeyWordReply { get; set; } // MpKeyWordReply
        public IDbSet<MpMediaArticle> MpMediaArticle { get; set; } // MpMediaArticle
        public IDbSet<MpMediaArticleGroup> MpMediaArticleGroup { get; set; } // MpMediaArticleGroup
        public IDbSet<MpMediaArticleGroupItem> MpMediaArticleGroupItem { get; set; } // MpMediaArticleGroupItem
        public IDbSet<MpMediaImage> MpMediaImage { get; set; } // MpMediaImage
        public IDbSet<MpMediaVideo> MpMediaVideo { get; set; } // MpMediaVideo
        public IDbSet<MpMediaVoice> MpMediaVoice { get; set; } // MpMediaVoice
        public IDbSet<MpMenu> MpMenu { get; set; } // MpMenu
        public IDbSet<MpMessage> MpMessage { get; set; } // MpMessage
        public IDbSet<MpOAuth2WhiteList> MpOAuth2WhiteList { get; set; } // MpOAuth2WhiteList
        public IDbSet<MpOAuth2WhiteReurl> MpOAuth2WhiteReurl { get; set; } // MpOAuth2WhiteReurl
        public IDbSet<MpRedPacket> MpRedPacket { get; set; } // MpRedPacket
        public IDbSet<MpRedPacketLog> MpRedPacketLog { get; set; } // MpRedPacketLog
        public IDbSet<MpSelfArticle> MpSelfArticle { get; set; } // MpSelfArticle
        public IDbSet<MpSelfArticleGroup> MpSelfArticleGroup { get; set; } // MpSelfArticleGroup
        public IDbSet<MpSelfArticleGroupItem> MpSelfArticleGroupItem { get; set; } // MpSelfArticleGroupItem

        static WeChatEntities()
        {
            Database.SetInitializer<WeChatEntities>(null);
        }

        public WeChatEntities()
            : base("Name=WeChat")
        {
        }

        public WeChatEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new MpAccountConfiguration());
            modelBuilder.Configurations.Add(new MPAccountSpaceMenuConfiguration());
            modelBuilder.Configurations.Add(new MpAccountUserRelationConfiguration());
            modelBuilder.Configurations.Add(new MpEventConfiguration());
            modelBuilder.Configurations.Add(new MpEventClickViewLogConfiguration());
            modelBuilder.Configurations.Add(new MpEventRequestMsgLogConfiguration());
            modelBuilder.Configurations.Add(new MpEventScanLogConfiguration());
            modelBuilder.Configurations.Add(new MpFansConfiguration());
            modelBuilder.Configurations.Add(new MpGroupConfiguration());
            modelBuilder.Configurations.Add(new MpJssdkWhiteListConfiguration());
            modelBuilder.Configurations.Add(new MpKeyWordReplyConfiguration());
            modelBuilder.Configurations.Add(new MpMediaArticleConfiguration());
            modelBuilder.Configurations.Add(new MpMediaArticleGroupConfiguration());
            modelBuilder.Configurations.Add(new MpMediaArticleGroupItemConfiguration());
            modelBuilder.Configurations.Add(new MpMediaImageConfiguration());
            modelBuilder.Configurations.Add(new MpMediaVideoConfiguration());
            modelBuilder.Configurations.Add(new MpMediaVoiceConfiguration());
            modelBuilder.Configurations.Add(new MpMenuConfiguration());
            modelBuilder.Configurations.Add(new MpMessageConfiguration());
            modelBuilder.Configurations.Add(new MpOAuth2WhiteListConfiguration());
            modelBuilder.Configurations.Add(new MpOAuth2WhiteReurlConfiguration());
            modelBuilder.Configurations.Add(new MpRedPacketConfiguration());
            modelBuilder.Configurations.Add(new MpRedPacketLogConfiguration());
            modelBuilder.Configurations.Add(new MpSelfArticleConfiguration());
            modelBuilder.Configurations.Add(new MpSelfArticleGroupConfiguration());
            modelBuilder.Configurations.Add(new MpSelfArticleGroupItemConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary>公众号</summary>	
	[Description("公众号")]
    public partial class MpAccount : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>公众号名称</summary>	
		[Description("公众号名称")]
        public string Name { get; set; } // Name
		/// <summary>公众号描述</summary>	
		[Description("公众号描述")]
        public string Remark { get; set; } // Remark
		/// <summary>公众号类型</summary>	
		[Description("公众号类型")]
        public string Type { get; set; } // Type
		/// <summary></summary>	
		[Description("")]
        public string AppID { get; set; } // AppID
		/// <summary></summary>	
		[Description("")]
        public string AppSecret { get; set; } // AppSecret
		/// <summary>令牌</summary>	
		[Description("令牌")]
        public string Token { get; set; } // Token
		/// <summary>消息加解密密钥</summary>	
		[Description("消息加解密密钥")]
        public string EncodingAESKey { get; set; } // EncodingAESKey
		/// <summary>第三方平台回调令牌</summary>	
		[Description("第三方平台回调令牌")]
        public string AccessToken { get; set; } // AccessToken
		/// <summary></summary>	
		[Description("")]
        public string MchID { get; set; } // MchID
		/// <summary></summary>	
		[Description("")]
        public string WxPayAppSecret { get; set; } // WxPayAppSecret
		/// <summary></summary>	
		[Description("")]
        public string CertPhysicalPath { get; set; } // CertPhysicalPath
		/// <summary></summary>	
		[Description("")]
        public string CertPassword { get; set; } // CertPassword
		/// <summary>获取jssdk的密码</summary>	
		[Description("获取jssdk的密码")]
        public string JSSDKPassword { get; set; } // JSSDKPassword
		/// <summary>过期时间</summary>	
		[Description("过期时间")]
        public DateTime? ExpireTime { get; set; } // ExpireTime
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary></summary>	
		[Description("")]
        public bool? AutoSyncUser { get; set; } // AutoSyncUser

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpAccountUserRelation> MpAccountUserRelation { get { onMpAccountUserRelationGetting(); return _MpAccountUserRelation;} }
		private ICollection<MpAccountUserRelation> _MpAccountUserRelation;
		partial void onMpAccountUserRelationGetting();

		[JsonIgnore]
        public virtual ICollection<MpEvent> MpEvent { get { onMpEventGetting(); return _MpEvent;} }
		private ICollection<MpEvent> _MpEvent;
		partial void onMpEventGetting();

		[JsonIgnore]
        public virtual ICollection<MpFans> MpFans { get { onMpFansGetting(); return _MpFans;} }
		private ICollection<MpFans> _MpFans;
		partial void onMpFansGetting();

		[JsonIgnore]
        public virtual ICollection<MpGroup> MpGroup { get { onMpGroupGetting(); return _MpGroup;} }
		private ICollection<MpGroup> _MpGroup;
		partial void onMpGroupGetting();

		[JsonIgnore]
        public virtual ICollection<MpJssdkWhiteList> MpJssdkWhiteList { get { onMpJssdkWhiteListGetting(); return _MpJssdkWhiteList;} }
		private ICollection<MpJssdkWhiteList> _MpJssdkWhiteList;
		partial void onMpJssdkWhiteListGetting();

		[JsonIgnore]
        public virtual ICollection<MpKeyWordReply> MpKeyWordReply { get { onMpKeyWordReplyGetting(); return _MpKeyWordReply;} }
		private ICollection<MpKeyWordReply> _MpKeyWordReply;
		partial void onMpKeyWordReplyGetting();

		[JsonIgnore]
        public virtual ICollection<MpMediaArticle> MpMediaArticle { get { onMpMediaArticleGetting(); return _MpMediaArticle;} }
		private ICollection<MpMediaArticle> _MpMediaArticle;
		partial void onMpMediaArticleGetting();

		[JsonIgnore]
        public virtual ICollection<MpMediaArticleGroup> MpMediaArticleGroup { get { onMpMediaArticleGroupGetting(); return _MpMediaArticleGroup;} }
		private ICollection<MpMediaArticleGroup> _MpMediaArticleGroup;
		partial void onMpMediaArticleGroupGetting();

		[JsonIgnore]
        public virtual ICollection<MpMediaArticleGroupItem> MpMediaArticleGroupItem { get { onMpMediaArticleGroupItemGetting(); return _MpMediaArticleGroupItem;} }
		private ICollection<MpMediaArticleGroupItem> _MpMediaArticleGroupItem;
		partial void onMpMediaArticleGroupItemGetting();

		[JsonIgnore]
        public virtual ICollection<MpMediaImage> MpMediaImage { get { onMpMediaImageGetting(); return _MpMediaImage;} }
		private ICollection<MpMediaImage> _MpMediaImage;
		partial void onMpMediaImageGetting();

		[JsonIgnore]
        public virtual ICollection<MpMediaVideo> MpMediaVideo { get { onMpMediaVideoGetting(); return _MpMediaVideo;} }
		private ICollection<MpMediaVideo> _MpMediaVideo;
		partial void onMpMediaVideoGetting();

		[JsonIgnore]
        public virtual ICollection<MpMediaVoice> MpMediaVoice { get { onMpMediaVoiceGetting(); return _MpMediaVoice;} }
		private ICollection<MpMediaVoice> _MpMediaVoice;
		partial void onMpMediaVoiceGetting();

		[JsonIgnore]
        public virtual ICollection<MpMenu> MpMenu { get { onMpMenuGetting(); return _MpMenu;} }
		private ICollection<MpMenu> _MpMenu;
		partial void onMpMenuGetting();

		[JsonIgnore]
        public virtual ICollection<MpMessage> MpMessage { get { onMpMessageGetting(); return _MpMessage;} }
		private ICollection<MpMessage> _MpMessage;
		partial void onMpMessageGetting();

		[JsonIgnore]
        public virtual ICollection<MpOAuth2WhiteList> MpOAuth2WhiteList { get { onMpOAuth2WhiteListGetting(); return _MpOAuth2WhiteList;} }
		private ICollection<MpOAuth2WhiteList> _MpOAuth2WhiteList;
		partial void onMpOAuth2WhiteListGetting();

		[JsonIgnore]
        public virtual ICollection<MpRedPacket> MpRedPacket { get { onMpRedPacketGetting(); return _MpRedPacket;} }
		private ICollection<MpRedPacket> _MpRedPacket;
		partial void onMpRedPacketGetting();

		[JsonIgnore]
        public virtual ICollection<MpRedPacketLog> MpRedPacketLog { get { onMpRedPacketLogGetting(); return _MpRedPacketLog;} }
		private ICollection<MpRedPacketLog> _MpRedPacketLog;
		partial void onMpRedPacketLogGetting();

		[JsonIgnore]
        public virtual ICollection<MpSelfArticle> MpSelfArticle { get { onMpSelfArticleGetting(); return _MpSelfArticle;} }
		private ICollection<MpSelfArticle> _MpSelfArticle;
		partial void onMpSelfArticleGetting();

		[JsonIgnore]
        public virtual ICollection<MpSelfArticleGroup> MpSelfArticleGroup { get { onMpSelfArticleGroupGetting(); return _MpSelfArticleGroup;} }
		private ICollection<MpSelfArticleGroup> _MpSelfArticleGroup;
		partial void onMpSelfArticleGroupGetting();

		[JsonIgnore]
        public virtual ICollection<MpSelfArticleGroupItem> MpSelfArticleGroupItem { get { onMpSelfArticleGroupItemGetting(); return _MpSelfArticleGroupItem;} }
		private ICollection<MpSelfArticleGroupItem> _MpSelfArticleGroupItem;
		partial void onMpSelfArticleGroupItemGetting();


        public MpAccount()
        {
            _MpAccountUserRelation = new List<MpAccountUserRelation>();
            _MpEvent = new List<MpEvent>();
            _MpFans = new List<MpFans>();
            _MpGroup = new List<MpGroup>();
            _MpJssdkWhiteList = new List<MpJssdkWhiteList>();
            _MpKeyWordReply = new List<MpKeyWordReply>();
            _MpMediaArticle = new List<MpMediaArticle>();
            _MpMediaArticleGroup = new List<MpMediaArticleGroup>();
            _MpMediaArticleGroupItem = new List<MpMediaArticleGroupItem>();
            _MpMediaImage = new List<MpMediaImage>();
            _MpMediaVideo = new List<MpMediaVideo>();
            _MpMediaVoice = new List<MpMediaVoice>();
            _MpMenu = new List<MpMenu>();
            _MpMessage = new List<MpMessage>();
            _MpOAuth2WhiteList = new List<MpOAuth2WhiteList>();
            _MpRedPacket = new List<MpRedPacket>();
            _MpRedPacketLog = new List<MpRedPacketLog>();
            _MpSelfArticle = new List<MpSelfArticle>();
            _MpSelfArticleGroup = new List<MpSelfArticleGroup>();
            _MpSelfArticleGroupItem = new List<MpSelfArticleGroupItem>();
        }
    }

	/// <summary>微信管理菜单</summary>	
	[Description("微信管理菜单")]
    public partial class MPAccountSpaceMenu : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>父ID</summary>	
		[Description("父ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全ID</summary>	
		[Description("全ID")]
        public string FullID { get; set; } // FullID
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>Url</summary>	
		[Description("Url")]
        public string Url { get; set; } // Url
		/// <summary>图标Url</summary>	
		[Description("图标Url")]
        public string IconUrl { get; set; } // IconUrl
		/// <summary>打开目标</summary>	
		[Description("打开目标")]
        public string Target { get; set; } // Target
		/// <summary>描述</summary>	
		[Description("描述")]
        public string Description { get; set; } // Description
		/// <summary>排序索引</summary>	
		[Description("排序索引")]
        public double? SortIndex { get; set; } // SortIndex
    }

	/// <summary>公众号关联关系</summary>	
	[Description("公众号关联关系")]
    public partial class MpAccountUserRelation : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>用户ID</summary>	
		[Description("用户ID")]
        public string UserID { get; set; } // UserID
		/// <summary>公众号ID</summary>	
		[Description("公众号ID")]
        public string MpID { get; set; } // MpID
		/// <summary>是否常用</summary>	
		[Description("是否常用")]
        public string IsUsed { get; set; } // IsUsed
		/// <summary>是否默认</summary>	
		[Description("是否默认")]
        public string IsDefault { get; set; } // IsDefault

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID2
    }

	/// <summary>微信事件</summary>	
	[Description("微信事件")]
    public partial class MpEvent : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>事件类型</summary>	
		[Description("事件类型")]
        public string EventType { get; set; } // EventType
		/// <summary>事件编号</summary>	
		[Description("事件编号")]
        public string EventCode { get; set; } // EventCode
		/// <summary>回复类型</summary>	
		[Description("回复类型")]
        public string ReplyType { get; set; } // ReplyType
		/// <summary>回复内容</summary>	
		[Description("回复内容")]
        public string Content { get; set; } // Content
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleID { get; set; } // ArticleID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleName { get; set; } // ArticleName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleMediaID { get; set; } // ArticleMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleGroupID { get; set; } // ArticleGroupID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleGroupName { get; set; } // ArticleGroupName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleGroupMediaID { get; set; } // ArticleGroupMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ImageID { get; set; } // ImageID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ImageName { get; set; } // ImageName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ImageMediaID { get; set; } // ImageMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VideoID { get; set; } // VideoID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VideoName { get; set; } // VideoName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VideoMediaID { get; set; } // VideoMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VoiceID { get; set; } // VoiceID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VoiceName { get; set; } // VoiceName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VoiceMediaID { get; set; } // VoiceMediaID

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID13
    }

	/// <summary>微信click&view事件记录</summary>	
	[Description("微信click&view事件记录")]
    public partial class MpEventClickViewLog : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary></summary>	
		[Description("")]
        public string OpenID { get; set; } // OpenID
		/// <summary>事件内容</summary>	
		[Description("事件内容")]
        public string EventKey { get; set; } // EventKey
		/// <summary>事件类型(0-View;1-Click)</summary>	
		[Description("事件类型(0-View;1-Click)")]
        public string EventType { get; set; } // EventType
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string MsgID { get; set; } // MsgID
    }

	/// <summary>微信请求信息记录</summary>	
	[Description("微信请求信息记录")]
    public partial class MpEventRequestMsgLog : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary></summary>	
		[Description("")]
        public string OpenID { get; set; } // OpenID
		/// <summary>信息类型</summary>	
		[Description("信息类型")]
        public string MsgType { get; set; } // MsgType
		/// <summary>信息ID</summary>	
		[Description("信息ID")]
        public string MsgId { get; set; } // MsgId
		/// <summary>图片语言视频信息ID</summary>	
		[Description("图片语言视频信息ID")]
        public string MediaId { get; set; } // MediaId
		/// <summary>LINK主标题</summary>	
		[Description("LINK主标题")]
        public string Title { get; set; } // Title
		/// <summary>LINK副标题</summary>	
		[Description("LINK副标题")]
        public string Description { get; set; } // Description
		/// <summary>LINK URL</summary>	
		[Description("LINK URL")]
        public string url { get; set; } // url
		/// <summary>文本信息内容</summary>	
		[Description("文本信息内容")]
        public string Content { get; set; } // Content
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
    }

	/// <summary>微信扫码事件记录</summary>	
	[Description("微信扫码事件记录")]
    public partial class MpEventScanLog : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary></summary>	
		[Description("")]
        public string OpenID { get; set; } // OpenID
		/// <summary>事件内容</summary>	
		[Description("事件内容")]
        public string EventContent { get; set; } // EventContent
		/// <summary>事件类型(0-未关注;1-已关注)</summary>	
		[Description("事件类型(0-未关注;1-已关注)")]
        public string EventType { get; set; } // EventType
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public string MsgID { get; set; } // MsgID
    }

	/// <summary>微信粉丝</summary>	
	[Description("微信粉丝")]
    public partial class MpFans : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary></summary>	
		[Description("")]
        public string OpenID { get; set; } // OpenID
		/// <summary>用户的昵称</summary>	
		[Description("用户的昵称")]
        public string NickName { get; set; } // NickName
		/// <summary>用户的性别，值为1时是男性，值为2时是女性，值为0时是未知</summary>	
		[Description("用户的性别，值为1时是男性，值为2时是女性，值为0时是未知")]
        public string Sex { get; set; } // Sex
		/// <summary>用户的语言，简体中文为zh_CN</summary>	
		[Description("用户的语言，简体中文为zh_CN")]
        public string Language { get; set; } // Language
		/// <summary>用户所在城市</summary>	
		[Description("用户所在城市")]
        public string City { get; set; } // City
		/// <summary>用户所在省份</summary>	
		[Description("用户所在省份")]
        public string Province { get; set; } // Province
		/// <summary>用户所在国家</summary>	
		[Description("用户所在国家")]
        public string Country { get; set; } // Country
		/// <summary>用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。</summary>	
		[Description("用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。")]
        public string HeadImgUrl { get; set; } // HeadImgUrl
		/// <summary>用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间</summary>	
		[Description("用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间")]
        public DateTime? SubscribeTime { get; set; } // SubscribeTime
		/// <summary>只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段</summary>	
		[Description("只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段")]
        public string UniionID { get; set; } // UniionID
		/// <summary>公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注</summary>	
		[Description("公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注")]
        public string Remark { get; set; } // Remark
		/// <summary>用户所在的分组ID</summary>	
		[Description("用户所在的分组ID")]
        public int? WxGroupID { get; set; } // WxGroupID
		/// <summary>分组ID</summary>	
		[Description("分组ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>是否粉丝</summary>	
		[Description("是否粉丝")]
        public string IsFans { get; set; } // IsFans
		/// <summary></summary>	
		[Description("")]
        public DateTime? UpdateTime { get; set; } // UpdateTime

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID12
		[JsonIgnore]
        public virtual MpGroup MpGroup { get; set; } //  GroupID - GroupID1
    }

	/// <summary>微信分组</summary>	
	[Description("微信分组")]
    public partial class MpGroup : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>分组ID</summary>	
		[Description("分组ID")]
        public int? WxGroupID { get; set; } // WxGroupID
		/// <summary>分组名称</summary>	
		[Description("分组名称")]
        public string Name { get; set; } // Name
		/// <summary>粉丝数量</summary>	
		[Description("粉丝数量")]
        public int? FansCount { get; set; } // FansCount
		/// <summary>父节点ID</summary>	
		[Description("父节点ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全路径</summary>	
		[Description("全路径")]
        public string FullPath { get; set; } // FullPath
		/// <summary>深度</summary>	
		[Description("深度")]
        public int? Length { get; set; } // Length
		/// <summary>子节点数量</summary>	
		[Description("子节点数量")]
        public int? ChildCount { get; set; } // ChildCount

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpFans> MpFans { get { onMpFansGetting(); return _MpFans;} }
		private ICollection<MpFans> _MpFans;
		partial void onMpFansGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID1

        public MpGroup()
        {
            _MpFans = new List<MpFans>();
        }
    }

	/// <summary>Jssdk白名单</summary>	
	[Description("Jssdk白名单")]
    public partial class MpJssdkWhiteList : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>域名</summary>	
		[Description("域名")]
        public string Domain { get; set; } // Domain

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID17
    }

	/// <summary>微信关键字回复</summary>	
	[Description("微信关键字回复")]
    public partial class MpKeyWordReply : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>关键字</summary>	
		[Description("关键字")]
        public string KeyWord { get; set; } // KeyWord
		/// <summary>回复类型</summary>	
		[Description("回复类型")]
        public string ReplyType { get; set; } // ReplyType
		/// <summary>回复内容</summary>	
		[Description("回复内容")]
        public string Content { get; set; } // Content
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleID { get; set; } // ArticleID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleName { get; set; } // ArticleName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleMediaID { get; set; } // ArticleMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleGroupID { get; set; } // ArticleGroupID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleGroupName { get; set; } // ArticleGroupName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleGroupMediaID { get; set; } // ArticleGroupMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ImageID { get; set; } // ImageID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ImageName { get; set; } // ImageName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ImageMediaID { get; set; } // ImageMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VideoID { get; set; } // VideoID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VideoName { get; set; } // VideoName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VideoMediaID { get; set; } // VideoMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VoiceID { get; set; } // VoiceID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VoiceName { get; set; } // VoiceName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VoiceMediaID { get; set; } // VoiceMediaID

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID14
    }

	/// <summary>图文素材</summary>	
	[Description("图文素材")]
    public partial class MpMediaArticle : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>标题</summary>	
		[Description("标题")]
        public string Title { get; set; } // Title
		/// <summary>简介</summary>	
		[Description("简介")]
        public string Description { get; set; } // Description
		/// <summary>缩略图</summary>	
		[Description("缩略图")]
        public string PicFileID { get; set; } // PicFileID
		/// <summary>查看原文地址</summary>	
		[Description("查看原文地址")]
        public string Url { get; set; } // Url
		/// <summary>正文</summary>	
		[Description("正文")]
        public string Content { get; set; } // Content
		/// <summary>缩略图素材ID</summary>	
		[Description("缩略图素材ID")]
        public string MediaID { get; set; } // MediaID
		/// <summary>图文素材ID</summary>	
		[Description("图文素材ID")]
        public string PicMediaID { get; set; } // PicMediaID
		/// <summary>正文是否显示封面</summary>	
		[Description("正文是否显示封面")]
        public string ShowPic { get; set; } // ShowPic
		/// <summary>作者</summary>	
		[Description("作者")]
        public string Author { get; set; } // Author

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpMediaArticleGroupItem> MpMediaArticleGroupItem { get { onMpMediaArticleGroupItemGetting(); return _MpMediaArticleGroupItem;} }
		private ICollection<MpMediaArticleGroupItem> _MpMediaArticleGroupItem;
		partial void onMpMediaArticleGroupItemGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID4

        public MpMediaArticle()
        {
            _MpMediaArticleGroupItem = new List<MpMediaArticleGroupItem>();
        }
    }

	/// <summary>多图文素材</summary>	
	[Description("多图文素材")]
    public partial class MpMediaArticleGroup : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>缩略图素材ID</summary>	
		[Description("缩略图素材ID")]
        public string MediaID { get; set; } // MediaID

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpMediaArticleGroupItem> MpMediaArticleGroupItem { get { onMpMediaArticleGroupItemGetting(); return _MpMediaArticleGroupItem;} }
		private ICollection<MpMediaArticleGroupItem> _MpMediaArticleGroupItem;
		partial void onMpMediaArticleGroupItemGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID9

        public MpMediaArticleGroup()
        {
            _MpMediaArticleGroupItem = new List<MpMediaArticleGroupItem>();
        }
    }

	/// <summary>多图文素材明细</summary>	
	[Description("多图文素材明细")]
    public partial class MpMediaArticleGroupItem : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>公众号ID</summary>	
		[Description("公众号ID")]
        public string MpID { get; set; } // MpID
		/// <summary>多图文ID</summary>	
		[Description("多图文ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>单图文ID</summary>	
		[Description("单图文ID")]
        public string ArticleID { get; set; } // ArticleID
		/// <summary>排序</summary>	
		[Description("排序")]
        public int? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID11
		[JsonIgnore]
        public virtual MpMediaArticleGroup MpMediaArticleGroup { get; set; } //  GroupID - GroupID
		[JsonIgnore]
        public virtual MpMediaArticle MpMediaArticle { get; set; } //  ArticleID - ArticleID
    }

	/// <summary>图片素材</summary>	
	[Description("图片素材")]
    public partial class MpMediaImage : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary></summary>	
		[Description("")]
        public string MediaID { get; set; } // MediaID
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>文件ID</summary>	
		[Description("文件ID")]
        public string FileID { get; set; } // FileID

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID5
    }

	/// <summary>视频素材</summary>	
	[Description("视频素材")]
    public partial class MpMediaVideo : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>素材ID</summary>	
		[Description("素材ID")]
        public string MediaID { get; set; } // MediaID
		/// <summary>标题</summary>	
		[Description("标题")]
        public string Title { get; set; } // Title
		/// <summary>简介</summary>	
		[Description("简介")]
        public string Description { get; set; } // Description
		/// <summary>文件ID</summary>	
		[Description("文件ID")]
        public string FileID { get; set; } // FileID

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID7
    }

	/// <summary>声音素材</summary>	
	[Description("声音素材")]
    public partial class MpMediaVoice : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary></summary>	
		[Description("")]
        public string MediaID { get; set; } // MediaID
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name
		/// <summary>文件ID</summary>	
		[Description("文件ID")]
        public string FileID { get; set; } // FileID

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID8
    }

	/// <summary>微信菜单</summary>	
	[Description("微信菜单")]
    public partial class MpMenu : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>公众号ID</summary>	
		[Description("公众号ID")]
        public string MpID { get; set; } // MpID
		/// <summary>父节点ID</summary>	
		[Description("父节点ID")]
        public string ParentID { get; set; } // ParentID
		/// <summary>全路径</summary>	
		[Description("全路径")]
        public string FullPath { get; set; } // FullPath
		/// <summary>深度</summary>	
		[Description("深度")]
        public int? Length { get; set; } // Length
		/// <summary>子节点数量</summary>	
		[Description("子节点数量")]
        public int? ChildCount { get; set; } // ChildCount
		/// <summary>菜单名称</summary>	
		[Description("菜单名称")]
        public string Name { get; set; } // Name
		/// <summary>菜单类型</summary>	
		[Description("菜单类型")]
        public string Type { get; set; } // Type
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>菜单素材类型</summary>	
		[Description("菜单素材类型")]
        public string MediaType { get; set; } // MediaType
		/// <summary>是否获取openid</summary>	
		[Description("是否获取openid")]
        public string GetOpenID { get; set; } // GetOpenID
		/// <summary>跳转页面时的链接</summary>	
		[Description("跳转页面时的链接")]
        public string LinkUrl { get; set; } // LinkUrl
		/// <summary>推送文本时的内容</summary>	
		[Description("推送文本时的内容")]
        public string Content { get; set; } // Content
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleID { get; set; } // ArticleID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleName { get; set; } // ArticleName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleMediaID { get; set; } // ArticleMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleGroupID { get; set; } // ArticleGroupID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleGroupName { get; set; } // ArticleGroupName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleGroupMediaID { get; set; } // ArticleGroupMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ImageID { get; set; } // ImageID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ImageName { get; set; } // ImageName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ImageMediaID { get; set; } // ImageMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VideoID { get; set; } // VideoID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VideoName { get; set; } // VideoName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VideoMediaID { get; set; } // VideoMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VoiceID { get; set; } // VoiceID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VoiceName { get; set; } // VoiceName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VoiceMediaID { get; set; } // VoiceMediaID
		/// <summary>排序值</summary>	
		[Description("排序值")]
        public int? SortIndex { get; set; } // SortIndex
		/// <summary></summary>	
		[Description("")]
        public string MenuKey { get; set; } // MenuKey

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID3
    }

	/// <summary>高级群发日志</summary>	
	[Description("高级群发日志")]
    public partial class MpMessage : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>群发类型</summary>	
		[Description("群发类型")]
        public string MessageType { get; set; } // MessageType
		/// <summary>回复内容</summary>	
		[Description("回复内容")]
        public string Content { get; set; } // Content
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleID { get; set; } // ArticleID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleName { get; set; } // ArticleName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleMediaID { get; set; } // ArticleMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ArticleGroupID { get; set; } // ArticleGroupID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ArticleGroupName { get; set; } // ArticleGroupName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ArticleGroupMediaID { get; set; } // ArticleGroupMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string ImageID { get; set; } // ImageID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string ImageName { get; set; } // ImageName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string ImageMediaID { get; set; } // ImageMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VideoID { get; set; } // VideoID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VideoName { get; set; } // VideoName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VideoMediaID { get; set; } // VideoMediaID
		/// <summary>素材记录ID</summary>	
		[Description("素材记录ID")]
        public string VoiceID { get; set; } // VoiceID
		/// <summary>素材记录名称</summary>	
		[Description("素材记录名称")]
        public string VoiceName { get; set; } // VoiceName
		/// <summary>微信素材ID</summary>	
		[Description("微信素材ID")]
        public string VoiceMediaID { get; set; } // VoiceMediaID
		/// <summary>微信消息ID</summary>	
		[Description("微信消息ID")]
        public string WxMsgID { get; set; } // WxMsgID
		/// <summary>状态</summary>	
		[Description("状态")]
        public string State { get; set; } // State
		/// <summary>发送人数</summary>	
		[Description("发送人数")]
        public int? SendCount { get; set; } // SendCount
		/// <summary>成功人数</summary>	
		[Description("成功人数")]
        public int? SuccessCount { get; set; } // SuccessCount
		/// <summary>失败人数</summary>	
		[Description("失败人数")]
        public int? FailCount { get; set; } // FailCount
		/// <summary>完成时间</summary>	
		[Description("完成时间")]
        public DateTime? FinishDate { get; set; } // FinishDate
		/// <summary>分组名称</summary>	
		[Description("分组名称")]
        public string GroupNames { get; set; } // GroupNames
		/// <summary>分组ID</summary>	
		[Description("分组ID")]
        public string GroupIDs { get; set; } // GroupIDs
		/// <summary>用户的性别，值为1时是男性，值为2时是女性，值为0时是未知</summary>	
		[Description("用户的性别，值为1时是男性，值为2时是女性，值为0时是未知")]
        public string Sex { get; set; } // Sex
		/// <summary>用户所在城市</summary>	
		[Description("用户所在城市")]
        public string City { get; set; } // City
		/// <summary>用户所在省份</summary>	
		[Description("用户所在省份")]
        public string Province { get; set; } // Province
		/// <summary>用户所在国家</summary>	
		[Description("用户所在国家")]
        public string Country { get; set; } // Country
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID6
    }

	/// <summary>OAuth2.0白名单</summary>	
	[Description("OAuth2.0白名单")]
    public partial class MpOAuth2WhiteList : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>域名</summary>	
		[Description("域名")]
        public string Domain { get; set; } // Domain

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID18
    }

	/// <summary></summary>	
	[Description("")]
    public partial class MpOAuth2WhiteReurl : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary></summary>	
		[Description("")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary></summary>	
		[Description("")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary></summary>	
		[Description("")]
        public string Reurl { get; set; } // Reurl
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
    }

	/// <summary></summary>	
	[Description("")]
    public partial class MpRedPacket : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>公众号ID</summary>	
		[Description("公众号ID")]
        public string MpID { get; set; } // MpID
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string Token { get; set; } // Token
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? StartDate { get; set; } // StartDate
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? EndDate { get; set; } // EndDate
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpRedPacketLog> MpRedPacketLog { get { onMpRedPacketLogGetting(); return _MpRedPacketLog;} }
		private ICollection<MpRedPacketLog> _MpRedPacketLog;
		partial void onMpRedPacketLogGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - Fk_MpID

        public MpRedPacket()
        {
            _MpRedPacketLog = new List<MpRedPacketLog>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class MpRedPacketLog : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>公众号ID</summary>	
		[Description("公众号ID")]
        public string MpID { get; set; } // MpID
		/// <summary></summary>	
		[Description("")]
        public string RPID { get; set; } // RPID
		/// <summary></summary>	
		[Description("")]
        public string Openid { get; set; } // Openid
		/// <summary></summary>	
		[Description("")]
        public int? Total { get; set; } // Total
		/// <summary></summary>	
		[Description("")]
        public DateTime? SendTime { get; set; } // SendTime
		/// <summary></summary>	
		[Description("")]
        public string State { get; set; } // State
		/// <summary></summary>	
		[Description("")]
        public string Msg { get; set; } // Msg
		/// <summary></summary>	
		[Description("")]
        public string BillNO { get; set; } // BillNO

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - Fk_MpID1
		[JsonIgnore]
        public virtual MpRedPacket MpRedPacket { get; set; } //  RPID - Fk_RPID
    }

	/// <summary>自定义图文</summary>	
	[Description("自定义图文")]
    public partial class MpSelfArticle : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>标题</summary>	
		[Description("标题")]
        public string Title { get; set; } // Title
		/// <summary>简介</summary>	
		[Description("简介")]
        public string Description { get; set; } // Description
		/// <summary>缩略图</summary>	
		[Description("缩略图")]
        public string PicFileID { get; set; } // PicFileID
		/// <summary>查看原文地址</summary>	
		[Description("查看原文地址")]
        public string Url { get; set; } // Url

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpSelfArticleGroupItem> MpSelfArticleGroupItem { get { onMpSelfArticleGroupItemGetting(); return _MpSelfArticleGroupItem;} }
		private ICollection<MpSelfArticleGroupItem> _MpSelfArticleGroupItem;
		partial void onMpSelfArticleGroupItemGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID10

        public MpSelfArticle()
        {
            _MpSelfArticleGroupItem = new List<MpSelfArticleGroupItem>();
        }
    }

	/// <summary>自定义多图文</summary>	
	[Description("自定义多图文")]
    public partial class MpSelfArticleGroup : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string MpID { get; set; } // MpID
		/// <summary>创建人ID</summary>	
		[Description("创建人ID")]
        public string CreateUserID { get; set; } // CreateUserID
		/// <summary>创建人</summary>	
		[Description("创建人")]
        public string CreateUser { get; set; } // CreateUser
		/// <summary>创建时间</summary>	
		[Description("创建时间")]
        public DateTime? CreateDate { get; set; } // CreateDate
		/// <summary>修改人ID</summary>	
		[Description("修改人ID")]
        public string ModifyUserID { get; set; } // ModifyUserID
		/// <summary>修改人</summary>	
		[Description("修改人")]
        public string ModifyUser { get; set; } // ModifyUser
		/// <summary>修改时间</summary>	
		[Description("修改时间")]
        public DateTime? ModifyDate { get; set; } // ModifyDate
		/// <summary>是否删除</summary>	
		[Description("是否删除")]
        public int? IsDelete { get; set; } // IsDelete
		/// <summary>名称</summary>	
		[Description("名称")]
        public string Name { get; set; } // Name

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<MpSelfArticleGroupItem> MpSelfArticleGroupItem { get { onMpSelfArticleGroupItemGetting(); return _MpSelfArticleGroupItem;} }
		private ICollection<MpSelfArticleGroupItem> _MpSelfArticleGroupItem;
		partial void onMpSelfArticleGroupItemGetting();


        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID15

        public MpSelfArticleGroup()
        {
            _MpSelfArticleGroupItem = new List<MpSelfArticleGroupItem>();
        }
    }

	/// <summary>自定义多图文明细</summary>	
	[Description("自定义多图文明细")]
    public partial class MpSelfArticleGroupItem : Formula.BaseModel
    {
		/// <summary>标识符</summary>	
		[Description("标识符")]
        public string ID { get; set; } // ID (Primary key)
		/// <summary>公众号ID</summary>	
		[Description("公众号ID")]
        public string MpID { get; set; } // MpID
		/// <summary>多图文ID</summary>	
		[Description("多图文ID")]
        public string GroupID { get; set; } // GroupID
		/// <summary>单图文ID</summary>	
		[Description("单图文ID")]
        public string ArticleID { get; set; } // ArticleID
		/// <summary>排序</summary>	
		[Description("排序")]
        public int? SortIndex { get; set; } // SortIndex

        // Foreign keys
		[JsonIgnore]
        public virtual MpAccount MpAccount { get; set; } //  MpID - MpID16
		[JsonIgnore]
        public virtual MpSelfArticleGroup MpSelfArticleGroup { get; set; } //  GroupID - GroupID2
		[JsonIgnore]
        public virtual MpSelfArticle MpSelfArticle { get; set; } //  ArticleID - ArticleID1
    }


    // ************************************************************************
    // POCO Configuration

    // MpAccount
    internal partial class MpAccountConfiguration : EntityTypeConfiguration<MpAccount>
    {
        public MpAccountConfiguration()
        {
            ToTable("MPACCOUNT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional();
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.AppID).HasColumnName("APPID").IsOptional().HasMaxLength(50);
            Property(x => x.AppSecret).HasColumnName("APPSECRET").IsOptional().HasMaxLength(50);
            Property(x => x.Token).HasColumnName("TOKEN").IsOptional().HasMaxLength(200);
            Property(x => x.EncodingAESKey).HasColumnName("ENCODINGAESKEY").IsOptional().HasMaxLength(200);
            Property(x => x.AccessToken).HasColumnName("ACCESSTOKEN").IsOptional().HasMaxLength(200);
            Property(x => x.MchID).HasColumnName("MCHID").IsOptional().HasMaxLength(200);
            Property(x => x.WxPayAppSecret).HasColumnName("WXPAYAPPSECRET").IsOptional().HasMaxLength(200);
            Property(x => x.CertPhysicalPath).HasColumnName("CERTPHYSICALPATH").IsOptional().HasMaxLength(200);
            Property(x => x.CertPassword).HasColumnName("CERTPASSWORD").IsOptional().HasMaxLength(200);
            Property(x => x.JSSDKPassword).HasColumnName("JSSDKPASSWORD").IsOptional().HasMaxLength(200);
            Property(x => x.ExpireTime).HasColumnName("EXPIRETIME").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.AutoSyncUser).HasColumnName("AUTOSYNCUSER").IsOptional();
        }
    }

    // MPAccountSpaceMenu
    internal partial class MPAccountSpaceMenuConfiguration : EntityTypeConfiguration<MPAccountSpaceMenu>
    {
        public MPAccountSpaceMenuConfiguration()
        {
            ToTable("MPACCOUNTSPACEMENU");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(50);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(50);
            Property(x => x.FullID).HasColumnName("FULLID").IsOptional().HasMaxLength(500);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(200);
            Property(x => x.IconUrl).HasColumnName("ICONURL").IsOptional().HasMaxLength(200);
            Property(x => x.Target).HasColumnName("TARGET").IsOptional().HasMaxLength(50);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
        }
    }

    // MpAccountUserRelation
    internal partial class MpAccountUserRelationConfiguration : EntityTypeConfiguration<MpAccountUserRelation>
    {
        public MpAccountUserRelationConfiguration()
        {
            ToTable("MPACCOUNTUSERRELATION");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.UserID).HasColumnName("USERID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsRequired().HasMaxLength(36);
            Property(x => x.IsUsed).HasColumnName("ISUSED").IsOptional().HasMaxLength(50);
            Property(x => x.IsDefault).HasColumnName("ISDEFAULT").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasRequired(a => a.MpAccount).WithMany(b => b.MpAccountUserRelation).HasForeignKey(c => c.MpID); // MpID2
        }
    }

    // MpEvent
    internal partial class MpEventConfiguration : EntityTypeConfiguration<MpEvent>
    {
        public MpEventConfiguration()
        {
            ToTable("MPEVENT");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.EventType).HasColumnName("EVENTTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.EventCode).HasColumnName("EVENTCODE").IsOptional().HasMaxLength(50);
            Property(x => x.ReplyType).HasColumnName("REPLYTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ArticleID).HasColumnName("ARTICLEID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleName).HasColumnName("ARTICLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleMediaID).HasColumnName("ARTICLEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ArticleGroupID).HasColumnName("ARTICLEGROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleGroupName).HasColumnName("ARTICLEGROUPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleGroupMediaID).HasColumnName("ARTICLEGROUPMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ImageID).HasColumnName("IMAGEID").IsOptional().HasMaxLength(36);
            Property(x => x.ImageName).HasColumnName("IMAGENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ImageMediaID).HasColumnName("IMAGEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VideoID).HasColumnName("VIDEOID").IsOptional().HasMaxLength(36);
            Property(x => x.VideoName).HasColumnName("VIDEONAME").IsOptional().HasMaxLength(500);
            Property(x => x.VideoMediaID).HasColumnName("VIDEOMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VoiceID).HasColumnName("VOICEID").IsOptional().HasMaxLength(36);
            Property(x => x.VoiceName).HasColumnName("VOICENAME").IsOptional().HasMaxLength(500);
            Property(x => x.VoiceMediaID).HasColumnName("VOICEMEDIAID").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpEvent).HasForeignKey(c => c.MpID); // MpID13
        }
    }

    // MpEventClickViewLog
    internal partial class MpEventClickViewLogConfiguration : EntityTypeConfiguration<MpEventClickViewLog>
    {
        public MpEventClickViewLogConfiguration()
        {
            ToTable("MPEVENTCLICKVIEWLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsRequired().HasMaxLength(36);
            Property(x => x.OpenID).HasColumnName("OPENID").IsRequired().HasMaxLength(100);
            Property(x => x.EventKey).HasColumnName("EVENTKEY").IsOptional().HasMaxLength(500);
            Property(x => x.EventType).HasColumnName("EVENTTYPE").IsOptional().HasMaxLength(10);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.MsgID).HasColumnName("MSGID").IsOptional().HasMaxLength(500);
        }
    }

    // MpEventRequestMsgLog
    internal partial class MpEventRequestMsgLogConfiguration : EntityTypeConfiguration<MpEventRequestMsgLog>
    {
        public MpEventRequestMsgLogConfiguration()
        {
            ToTable("MPEVENTREQUESTMSGLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsRequired().HasMaxLength(36);
            Property(x => x.OpenID).HasColumnName("OPENID").IsRequired().HasMaxLength(100);
            Property(x => x.MsgType).HasColumnName("MSGTYPE").IsOptional().HasMaxLength(100);
            Property(x => x.MsgId).HasColumnName("MSGID").IsOptional().HasMaxLength(100);
            Property(x => x.MediaId).HasColumnName("MEDIAID").IsOptional().HasMaxLength(100);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
            Property(x => x.url).HasColumnName("URL").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
        }
    }

    // MpEventScanLog
    internal partial class MpEventScanLogConfiguration : EntityTypeConfiguration<MpEventScanLog>
    {
        public MpEventScanLogConfiguration()
        {
            ToTable("MPEVENTSCANLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsRequired().HasMaxLength(36);
            Property(x => x.OpenID).HasColumnName("OPENID").IsRequired().HasMaxLength(100);
            Property(x => x.EventContent).HasColumnName("EVENTCONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.EventType).HasColumnName("EVENTTYPE").IsOptional().HasMaxLength(10);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.MsgID).HasColumnName("MSGID").IsOptional().HasMaxLength(500);
        }
    }

    // MpFans
    internal partial class MpFansConfiguration : EntityTypeConfiguration<MpFans>
    {
        public MpFansConfiguration()
        {
            ToTable("MPFANS");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.OpenID).HasColumnName("OPENID").IsOptional().HasMaxLength(200);
            Property(x => x.NickName).HasColumnName("NICKNAME").IsOptional().HasMaxLength(200);
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(1);
            Property(x => x.Language).HasColumnName("LANGUAGE").IsOptional().HasMaxLength(50);
            Property(x => x.City).HasColumnName("CITY").IsOptional().HasMaxLength(36);
            Property(x => x.Province).HasColumnName("PROVINCE").IsOptional().HasMaxLength(36);
            Property(x => x.Country).HasColumnName("COUNTRY").IsOptional().HasMaxLength(36);
            Property(x => x.HeadImgUrl).HasColumnName("HEADIMGURL").IsOptional().HasMaxLength(500);
            Property(x => x.SubscribeTime).HasColumnName("SUBSCRIBETIME").IsOptional();
            Property(x => x.UniionID).HasColumnName("UNIIONID").IsOptional().HasMaxLength(200);
            Property(x => x.Remark).HasColumnName("REMARK").IsOptional().HasMaxLength(500);
            Property(x => x.WxGroupID).HasColumnName("WXGROUPID").IsOptional();
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.IsFans).HasColumnName("ISFANS").IsOptional().HasMaxLength(1);
            Property(x => x.UpdateTime).HasColumnName("UPDATETIME").IsOptional();

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpFans).HasForeignKey(c => c.MpID); // MpID12
            HasOptional(a => a.MpGroup).WithMany(b => b.MpFans).HasForeignKey(c => c.GroupID); // GroupID1
        }
    }

    // MpGroup
    internal partial class MpGroupConfiguration : EntityTypeConfiguration<MpGroup>
    {
        public MpGroupConfiguration()
        {
            ToTable("MPGROUP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.WxGroupID).HasColumnName("WXGROUPID").IsOptional();
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.FansCount).HasColumnName("FANSCOUNT").IsOptional();
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(36);
            Property(x => x.FullPath).HasColumnName("FULLPATH").IsOptional().HasMaxLength(500);
            Property(x => x.Length).HasColumnName("LENGTH").IsOptional();
            Property(x => x.ChildCount).HasColumnName("CHILDCOUNT").IsOptional();

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpGroup).HasForeignKey(c => c.MpID); // MpID1
        }
    }

    // MpJssdkWhiteList
    internal partial class MpJssdkWhiteListConfiguration : EntityTypeConfiguration<MpJssdkWhiteList>
    {
        public MpJssdkWhiteListConfiguration()
        {
            ToTable("MPJSSDKWHITELIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Domain).HasColumnName("DOMAIN").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpJssdkWhiteList).HasForeignKey(c => c.MpID); // MpID17
        }
    }

    // MpKeyWordReply
    internal partial class MpKeyWordReplyConfiguration : EntityTypeConfiguration<MpKeyWordReply>
    {
        public MpKeyWordReplyConfiguration()
        {
            ToTable("MPKEYWORDREPLY");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.KeyWord).HasColumnName("KEYWORD").IsOptional().HasMaxLength(50);
            Property(x => x.ReplyType).HasColumnName("REPLYTYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ArticleID).HasColumnName("ARTICLEID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleName).HasColumnName("ARTICLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleMediaID).HasColumnName("ARTICLEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ArticleGroupID).HasColumnName("ARTICLEGROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleGroupName).HasColumnName("ARTICLEGROUPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleGroupMediaID).HasColumnName("ARTICLEGROUPMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ImageID).HasColumnName("IMAGEID").IsOptional().HasMaxLength(36);
            Property(x => x.ImageName).HasColumnName("IMAGENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ImageMediaID).HasColumnName("IMAGEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VideoID).HasColumnName("VIDEOID").IsOptional().HasMaxLength(36);
            Property(x => x.VideoName).HasColumnName("VIDEONAME").IsOptional().HasMaxLength(500);
            Property(x => x.VideoMediaID).HasColumnName("VIDEOMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VoiceID).HasColumnName("VOICEID").IsOptional().HasMaxLength(36);
            Property(x => x.VoiceName).HasColumnName("VOICENAME").IsOptional().HasMaxLength(500);
            Property(x => x.VoiceMediaID).HasColumnName("VOICEMEDIAID").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpKeyWordReply).HasForeignKey(c => c.MpID); // MpID14
        }
    }

    // MpMediaArticle
    internal partial class MpMediaArticleConfiguration : EntityTypeConfiguration<MpMediaArticle>
    {
        public MpMediaArticleConfiguration()
        {
            ToTable("MPMEDIAARTICLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(2000);
            Property(x => x.PicFileID).HasColumnName("PICFILEID").IsOptional().HasMaxLength(500);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.MediaID).HasColumnName("MEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.PicMediaID).HasColumnName("PICMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ShowPic).HasColumnName("SHOWPIC").IsOptional().HasMaxLength(1);
            Property(x => x.Author).HasColumnName("AUTHOR").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMediaArticle).HasForeignKey(c => c.MpID); // MpID4
        }
    }

    // MpMediaArticleGroup
    internal partial class MpMediaArticleGroupConfiguration : EntityTypeConfiguration<MpMediaArticleGroup>
    {
        public MpMediaArticleGroupConfiguration()
        {
            ToTable("MPMEDIAARTICLEGROUP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.MediaID).HasColumnName("MEDIAID").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMediaArticleGroup).HasForeignKey(c => c.MpID); // MpID9
        }
    }

    // MpMediaArticleGroupItem
    internal partial class MpMediaArticleGroupItemConfiguration : EntityTypeConfiguration<MpMediaArticleGroupItem>
    {
        public MpMediaArticleGroupItemConfiguration()
        {
            ToTable("MPMEDIAARTICLEGROUPITEM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleID).HasColumnName("ARTICLEID").IsOptional().HasMaxLength(36);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMediaArticleGroupItem).HasForeignKey(c => c.MpID); // MpID11
            HasOptional(a => a.MpMediaArticleGroup).WithMany(b => b.MpMediaArticleGroupItem).HasForeignKey(c => c.GroupID); // GroupID
            HasOptional(a => a.MpMediaArticle).WithMany(b => b.MpMediaArticleGroupItem).HasForeignKey(c => c.ArticleID); // ArticleID
        }
    }

    // MpMediaImage
    internal partial class MpMediaImageConfiguration : EntityTypeConfiguration<MpMediaImage>
    {
        public MpMediaImageConfiguration()
        {
            ToTable("MPMEDIAIMAGE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.MediaID).HasColumnName("MEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMediaImage).HasForeignKey(c => c.MpID); // MpID5
        }
    }

    // MpMediaVideo
    internal partial class MpMediaVideoConfiguration : EntityTypeConfiguration<MpMediaVideo>
    {
        public MpMediaVideoConfiguration()
        {
            ToTable("MPMEDIAVIDEO");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.MediaID).HasColumnName("MEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(2000);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMediaVideo).HasForeignKey(c => c.MpID); // MpID7
        }
    }

    // MpMediaVoice
    internal partial class MpMediaVoiceConfiguration : EntityTypeConfiguration<MpMediaVoice>
    {
        public MpMediaVoiceConfiguration()
        {
            ToTable("MPMEDIAVOICE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.MediaID).HasColumnName("MEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);
            Property(x => x.FileID).HasColumnName("FILEID").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMediaVoice).HasForeignKey(c => c.MpID); // MpID8
        }
    }

    // MpMenu
    internal partial class MpMenuConfiguration : EntityTypeConfiguration<MpMenu>
    {
        public MpMenuConfiguration()
        {
            ToTable("MPMENU");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.ParentID).HasColumnName("PARENTID").IsOptional().HasMaxLength(36);
            Property(x => x.FullPath).HasColumnName("FULLPATH").IsOptional().HasMaxLength(500);
            Property(x => x.Length).HasColumnName("LENGTH").IsOptional();
            Property(x => x.ChildCount).HasColumnName("CHILDCOUNT").IsOptional();
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(50);
            Property(x => x.Type).HasColumnName("TYPE").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.MediaType).HasColumnName("MEDIATYPE").IsOptional().HasMaxLength(50);
            Property(x => x.GetOpenID).HasColumnName("GETOPENID").IsOptional().HasMaxLength(1);
            Property(x => x.LinkUrl).HasColumnName("LINKURL").IsOptional().HasMaxLength(500);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleID).HasColumnName("ARTICLEID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleName).HasColumnName("ARTICLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleMediaID).HasColumnName("ARTICLEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ArticleGroupID).HasColumnName("ARTICLEGROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleGroupName).HasColumnName("ARTICLEGROUPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleGroupMediaID).HasColumnName("ARTICLEGROUPMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ImageID).HasColumnName("IMAGEID").IsOptional().HasMaxLength(36);
            Property(x => x.ImageName).HasColumnName("IMAGENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ImageMediaID).HasColumnName("IMAGEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VideoID).HasColumnName("VIDEOID").IsOptional().HasMaxLength(36);
            Property(x => x.VideoName).HasColumnName("VIDEONAME").IsOptional().HasMaxLength(500);
            Property(x => x.VideoMediaID).HasColumnName("VIDEOMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VoiceID).HasColumnName("VOICEID").IsOptional().HasMaxLength(36);
            Property(x => x.VoiceName).HasColumnName("VOICENAME").IsOptional().HasMaxLength(500);
            Property(x => x.VoiceMediaID).HasColumnName("VOICEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();
            Property(x => x.MenuKey).HasColumnName("MENUKEY").IsOptional().HasMaxLength(50);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMenu).HasForeignKey(c => c.MpID); // MpID3
        }
    }

    // MpMessage
    internal partial class MpMessageConfiguration : EntityTypeConfiguration<MpMessage>
    {
        public MpMessageConfiguration()
        {
            ToTable("MPMESSAGE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.MessageType).HasColumnName("MESSAGETYPE").IsOptional().HasMaxLength(50);
            Property(x => x.Content).HasColumnName("CONTENT").IsOptional();
            Property(x => x.ArticleID).HasColumnName("ARTICLEID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleName).HasColumnName("ARTICLENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleMediaID).HasColumnName("ARTICLEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ArticleGroupID).HasColumnName("ARTICLEGROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleGroupName).HasColumnName("ARTICLEGROUPNAME").IsOptional().HasMaxLength(500);
            Property(x => x.ArticleGroupMediaID).HasColumnName("ARTICLEGROUPMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.ImageID).HasColumnName("IMAGEID").IsOptional().HasMaxLength(36);
            Property(x => x.ImageName).HasColumnName("IMAGENAME").IsOptional().HasMaxLength(500);
            Property(x => x.ImageMediaID).HasColumnName("IMAGEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VideoID).HasColumnName("VIDEOID").IsOptional().HasMaxLength(36);
            Property(x => x.VideoName).HasColumnName("VIDEONAME").IsOptional().HasMaxLength(500);
            Property(x => x.VideoMediaID).HasColumnName("VIDEOMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.VoiceID).HasColumnName("VOICEID").IsOptional().HasMaxLength(36);
            Property(x => x.VoiceName).HasColumnName("VOICENAME").IsOptional().HasMaxLength(500);
            Property(x => x.VoiceMediaID).HasColumnName("VOICEMEDIAID").IsOptional().HasMaxLength(200);
            Property(x => x.WxMsgID).HasColumnName("WXMSGID").IsOptional().HasMaxLength(50);
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(50);
            Property(x => x.SendCount).HasColumnName("SENDCOUNT").IsOptional();
            Property(x => x.SuccessCount).HasColumnName("SUCCESSCOUNT").IsOptional();
            Property(x => x.FailCount).HasColumnName("FAILCOUNT").IsOptional();
            Property(x => x.FinishDate).HasColumnName("FINISHDATE").IsOptional();
            Property(x => x.GroupNames).HasColumnName("GROUPNAMES").IsOptional().HasMaxLength(2000);
            Property(x => x.GroupIDs).HasColumnName("GROUPIDS").IsOptional().HasMaxLength(2000);
            Property(x => x.Sex).HasColumnName("SEX").IsOptional().HasMaxLength(50);
            Property(x => x.City).HasColumnName("CITY").IsOptional().HasMaxLength(2000);
            Property(x => x.Province).HasColumnName("PROVINCE").IsOptional().HasMaxLength(2000);
            Property(x => x.Country).HasColumnName("COUNTRY").IsOptional().HasMaxLength(2000);
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpMessage).HasForeignKey(c => c.MpID); // MpID6
        }
    }

    // MpOAuth2WhiteList
    internal partial class MpOAuth2WhiteListConfiguration : EntityTypeConfiguration<MpOAuth2WhiteList>
    {
        public MpOAuth2WhiteListConfiguration()
        {
            ToTable("MPOAUTH2WHITELIST");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Domain).HasColumnName("DOMAIN").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpOAuth2WhiteList).HasForeignKey(c => c.MpID); // MpID18
        }
    }

    // MpOAuth2WhiteReurl
    internal partial class MpOAuth2WhiteReurlConfiguration : EntityTypeConfiguration<MpOAuth2WhiteReurl>
    {
        public MpOAuth2WhiteReurlConfiguration()
        {
            ToTable("MPOAUTH2WHITEREURL");
            HasKey(x => x.MpID);

            Property(x => x.MpID).HasColumnName("MPID").IsRequired().HasMaxLength(100);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Reurl).HasColumnName("REURL").IsOptional().HasMaxLength(200);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
        }
    }

    // MpRedPacket
    internal partial class MpRedPacketConfiguration : EntityTypeConfiguration<MpRedPacket>
    {
        public MpRedPacketConfiguration()
        {
            ToTable("MPREDPACKET");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.Token).HasColumnName("TOKEN").IsOptional().HasMaxLength(200);
            Property(x => x.StartDate).HasColumnName("STARTDATE").IsOptional();
            Property(x => x.EndDate).HasColumnName("ENDDATE").IsOptional();
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpRedPacket).HasForeignKey(c => c.MpID); // Fk_MpID
        }
    }

    // MpRedPacketLog
    internal partial class MpRedPacketLogConfiguration : EntityTypeConfiguration<MpRedPacketLog>
    {
        public MpRedPacketLogConfiguration()
        {
            ToTable("MPREDPACKETLOG");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.RPID).HasColumnName("RPID").IsOptional().HasMaxLength(36);
            Property(x => x.Openid).HasColumnName("OPENID").IsOptional().HasMaxLength(200);
            Property(x => x.Total).HasColumnName("TOTAL").IsOptional();
            Property(x => x.SendTime).HasColumnName("SENDTIME").IsOptional();
            Property(x => x.State).HasColumnName("STATE").IsOptional().HasMaxLength(1);
            Property(x => x.Msg).HasColumnName("MSG").IsOptional();
            Property(x => x.BillNO).HasColumnName("BILLNO").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpRedPacketLog).HasForeignKey(c => c.MpID); // Fk_MpID1
            HasOptional(a => a.MpRedPacket).WithMany(b => b.MpRedPacketLog).HasForeignKey(c => c.RPID); // Fk_RPID
        }
    }

    // MpSelfArticle
    internal partial class MpSelfArticleConfiguration : EntityTypeConfiguration<MpSelfArticle>
    {
        public MpSelfArticleConfiguration()
        {
            ToTable("MPSELFARTICLE");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Title).HasColumnName("TITLE").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(2000);
            Property(x => x.PicFileID).HasColumnName("PICFILEID").IsOptional().HasMaxLength(500);
            Property(x => x.Url).HasColumnName("URL").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpSelfArticle).HasForeignKey(c => c.MpID); // MpID10
        }
    }

    // MpSelfArticleGroup
    internal partial class MpSelfArticleGroupConfiguration : EntityTypeConfiguration<MpSelfArticleGroup>
    {
        public MpSelfArticleGroupConfiguration()
        {
            ToTable("MPSELFARTICLEGROUP");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.CreateUserID).HasColumnName("CREATEUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.CreateUser).HasColumnName("CREATEUSER").IsOptional().HasMaxLength(50);
            Property(x => x.CreateDate).HasColumnName("CREATEDATE").IsOptional();
            Property(x => x.ModifyUserID).HasColumnName("MODIFYUSERID").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyUser).HasColumnName("MODIFYUSER").IsOptional().HasMaxLength(50);
            Property(x => x.ModifyDate).HasColumnName("MODIFYDATE").IsOptional();
            Property(x => x.IsDelete).HasColumnName("ISDELETE").IsOptional();
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(500);

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpSelfArticleGroup).HasForeignKey(c => c.MpID); // MpID15
        }
    }

    // MpSelfArticleGroupItem
    internal partial class MpSelfArticleGroupItemConfiguration : EntityTypeConfiguration<MpSelfArticleGroupItem>
    {
        public MpSelfArticleGroupItemConfiguration()
        {
            ToTable("MPSELFARTICLEGROUPITEM");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.MpID).HasColumnName("MPID").IsOptional().HasMaxLength(36);
            Property(x => x.GroupID).HasColumnName("GROUPID").IsOptional().HasMaxLength(36);
            Property(x => x.ArticleID).HasColumnName("ARTICLEID").IsOptional().HasMaxLength(36);
            Property(x => x.SortIndex).HasColumnName("SORTINDEX").IsOptional();

            // Foreign keys
            HasOptional(a => a.MpAccount).WithMany(b => b.MpSelfArticleGroupItem).HasForeignKey(c => c.MpID); // MpID16
            HasOptional(a => a.MpSelfArticleGroup).WithMany(b => b.MpSelfArticleGroupItem).HasForeignKey(c => c.GroupID); // GroupID2
            HasOptional(a => a.MpSelfArticle).WithMany(b => b.MpSelfArticleGroupItem).HasForeignKey(c => c.ArticleID); // ArticleID1
        }
    }

}

