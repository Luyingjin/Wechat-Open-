

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: "FileStore"
//     Connection String:      "Data Source=10.10.1.235\gw02;Initial Catalog=FE_FileStore;User ID=sa;PWD=123.zxc;"

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

namespace FileStore.Logic.Domain
{
    // ************************************************************************
    // Database context
    public partial class FileStoreEntities : Formula.FormulaDbContext
    {
        public IDbSet<FsFile> FsFile { get; set; } // FsFile
        public IDbSet<FsLog> FsLog { get; set; } // FsLog
        public IDbSet<FsRootFolder> FsRootFolder { get; set; } // FsRootFolder
        public IDbSet<FsServer> FsServer { get; set; } // FsServer
        public IDbSet<UserFileServer> UserFileServer { get; set; } // UserFileServer

        static FileStoreEntities()
        {
            Database.SetInitializer<FileStoreEntities>(null);
        }

        public FileStoreEntities()
            : base("Name=FileStore")
        {
        }

        public FileStoreEntities(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new FsFileConfiguration());
            modelBuilder.Configurations.Add(new FsLogConfiguration());
            modelBuilder.Configurations.Add(new FsRootFolderConfiguration());
            modelBuilder.Configurations.Add(new FsServerConfiguration());
            modelBuilder.Configurations.Add(new UserFileServerConfiguration());
        }
    }

    // ************************************************************************
    // POCO classes

	/// <summary></summary>	
	[Description("")]
    public partial class FsFile : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int Id { get; set; } // Id (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string Name { get; set; } // Name
		/// <summary></summary>	
		[Description("")]
        public string ExtName { get; set; } // ExtName
		/// <summary></summary>	
		[Description("")]
        public long? FileSize { get; set; } // FileSize
		/// <summary></summary>	
		[Description("")]
        public string RelateId { get; set; } // RelateId
		/// <summary>文件编号</summary>	
		[Description("文件编号")]
        public string Code { get; set; } // Code
		/// <summary>文件版本号</summary>	
		[Description("文件版本号")]
        public string Version { get; set; } // Version
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string IsDeleted { get; set; } // IsDeleted
		/// <summary></summary>	
		[Description("")]
        public DateTime? DeleteTime { get; set; } // DeleteTime
		/// <summary></summary>	
		[Description("")]
        public string DeleteReason { get; set; } // DeleteReason
		/// <summary>文件来源</summary>	
		[Description("文件来源")]
        public string Src { get; set; } // Src
		/// <summary>文件是否在主服务器</summary>	
		[Description("文件是否在主服务器")]
        public string OnMaster { get; set; } // OnMaster
		/// <summary></summary>	
		[Description("")]
        public string UploadServerName { get; set; } // UploadServerName
		/// <summary></summary>	
		[Description("")]
        public string FileServerNames { get; set; } // FileServerNames
		/// <summary></summary>	
		[Description("")]
        public string Guid { get; set; } // Guid
		/// <summary></summary>	
		[Description("")]
        public string Status { get; set; } // Status
		/// <summary></summary>	
		[Description("")]
        public int? ErrorCount { get; set; } // ErrorCount
		/// <summary></summary>	
		[Description("")]
        public DateTime? ErrorTime { get; set; } // ErrorTime

        public FsFile()
        {
			IsDeleted = "False";
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class FsLog : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int Id { get; set; } // Id (Primary key)
		/// <summary></summary>	
		[Description("")]
        public int FileId { get; set; } // FileId
		/// <summary></summary>	
		[Description("")]
        public string FileName { get; set; } // FileName
		/// <summary></summary>	
		[Description("")]
        public string Operation { get; set; } // Operation
		/// <summary></summary>	
		[Description("")]
        public string OperatorName { get; set; } // OperatorName
		/// <summary></summary>	
		[Description("")]
        public string OperatorIp { get; set; } // OperatorIp
		/// <summary></summary>	
		[Description("")]
        public DateTime? OperateTime { get; set; } // OperateTime
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description
    }

	/// <summary></summary>	
	[Description("")]
    public partial class FsRootFolder : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int Id { get; set; } // Id (Primary key)
		/// <summary></summary>	
		[Description("")]
        public int? ServerId { get; set; } // ServerId
		/// <summary></summary>	
		[Description("")]
        public string RootFolderPath { get; set; } // RootFolderPath
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string Pwd { get; set; } // Pwd
		/// <summary></summary>	
		[Description("")]
        public DateTime? CreateTime { get; set; } // CreateTime
		/// <summary></summary>	
		[Description("")]
        public string IsFull { get; set; } // IsFull
		/// <summary></summary>	
		[Description("")]
        public string SrcFilter { get; set; } // SrcFilter
		/// <summary></summary>	
		[Description("")]
        public string ExtNameFilter { get; set; } // ExtNameFilter
		/// <summary></summary>	
		[Description("")]
        public string AllowEncrypt { get; set; } // AllowEncrypt

        // Foreign keys
		[JsonIgnore]
        public virtual FsServer FsServer { get; set; } //  ServerId - FK_FsRootFolder_FsServer
    }

	/// <summary></summary>	
	[Description("")]
    public partial class FsServer : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int Id { get; set; } // Id (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string ServerName { get; set; } // ServerName
		/// <summary></summary>	
		[Description("")]
        public string IsMaster { get; set; } // IsMaster
		/// <summary></summary>	
		[Description("")]
        public string HttpUrl { get; set; } // HttpUrl
		/// <summary></summary>	
		[Description("")]
        public string Description { get; set; } // Description

        // Reverse navigation
		[JsonIgnore]
        public virtual ICollection<FsRootFolder> FsRootFolder { get { onFsRootFolderGetting(); return _FsRootFolder;} }
		private ICollection<FsRootFolder> _FsRootFolder;
		partial void onFsRootFolderGetting();


        public FsServer()
        {
            _FsRootFolder = new List<FsRootFolder>();
        }
    }

	/// <summary></summary>	
	[Description("")]
    public partial class UserFileServer : Formula.BaseModel
    {
		/// <summary></summary>	
		[Description("")]
        public int Id { get; set; } // Id (Primary key)
		/// <summary></summary>	
		[Description("")]
        public string UserName { get; set; } // UserName
		/// <summary></summary>	
		[Description("")]
        public string ServerName { get; set; } // ServerName
    }


    // ************************************************************************
    // POCO Configuration

    // FsFile
    internal partial class FsFileConfiguration : EntityTypeConfiguration<FsFile>
    {
        public FsFileConfiguration()
        {
			ToTable("FSFILE");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName("NAME").IsOptional().HasMaxLength(200);
            Property(x => x.ExtName).HasColumnName("EXTNAME").IsOptional().HasMaxLength(50);
            Property(x => x.FileSize).HasColumnName("FILESIZE").IsOptional();
            Property(x => x.RelateId).HasColumnName("RELATEID").IsOptional().HasMaxLength(50);
            Property(x => x.Code).HasColumnName("CODE").IsOptional().HasMaxLength(50);
            Property(x => x.Version).HasColumnName("VERSION").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.IsDeleted).HasColumnName("ISDELETED").IsOptional().HasMaxLength(1);
            Property(x => x.DeleteTime).HasColumnName("DELETETIME").IsOptional();
            Property(x => x.DeleteReason).HasColumnName("DELETEREASON").IsOptional().HasMaxLength(200);
            Property(x => x.Src).HasColumnName("SRC").IsOptional().HasMaxLength(50);
            Property(x => x.OnMaster).HasColumnName("ONMASTER").IsOptional().HasMaxLength(1);
            Property(x => x.UploadServerName).HasColumnName("UPLOADSERVERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.FileServerNames).HasColumnName("FILESERVERNAMES").IsOptional().HasMaxLength(500);
            Property(x => x.Guid).HasColumnName("GUID").IsOptional().HasMaxLength(50);
            Property(x => x.Status).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
            Property(x => x.ErrorCount).HasColumnName("ERRORCOUNT").IsOptional();
            Property(x => x.ErrorTime).HasColumnName("ERRORTIME").IsOptional();
        }
    }

    // FsLog
    internal partial class FsLogConfiguration : EntityTypeConfiguration<FsLog>
    {
        public FsLogConfiguration()
        {
			ToTable("FSLOG");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.FileId).HasColumnName("FILEID").IsRequired();
            Property(x => x.FileName).HasColumnName("FILENAME").IsOptional().HasMaxLength(200);
            Property(x => x.Operation).HasColumnName("OPERATION").IsOptional().HasMaxLength(50);
            Property(x => x.OperatorName).HasColumnName("OPERATORNAME").IsOptional().HasMaxLength(50);
            Property(x => x.OperatorIp).HasColumnName("OPERATORIP").IsOptional().HasMaxLength(50);
            Property(x => x.OperateTime).HasColumnName("OPERATETIME").IsOptional();
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(200);
        }
    }

    // FsRootFolder
    internal partial class FsRootFolderConfiguration : EntityTypeConfiguration<FsRootFolder>
    {
        public FsRootFolderConfiguration()
        {
			ToTable("FSROOTFOLDER");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.ServerId).HasColumnName("SERVERID").IsOptional();
            Property(x => x.RootFolderPath).HasColumnName("ROOTFOLDERPATH").IsOptional().HasMaxLength(500);
            Property(x => x.UserName).HasColumnName("USERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.Pwd).HasColumnName("PWD").IsOptional().HasMaxLength(50);
            Property(x => x.CreateTime).HasColumnName("CREATETIME").IsOptional();
            Property(x => x.IsFull).HasColumnName("ISFULL").IsOptional().HasMaxLength(1);
            Property(x => x.SrcFilter).HasColumnName("SRCFILTER").IsOptional().HasMaxLength(200);
            Property(x => x.ExtNameFilter).HasColumnName("EXTNAMEFILTER").IsOptional().HasMaxLength(200);
            Property(x => x.AllowEncrypt).HasColumnName("ALLOWENCRYPT").IsOptional().HasMaxLength(1);

            // Foreign keys
            HasOptional(a => a.FsServer).WithMany(b => b.FsRootFolder).HasForeignKey(c => c.ServerId); // FK_FsRootFolder_FsServer
        }
    }

    // FsServer
    internal partial class FsServerConfiguration : EntityTypeConfiguration<FsServer>
    {
        public FsServerConfiguration()
        {
			ToTable("FSSERVER");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.ServerName).HasColumnName("SERVERNAME").IsOptional().HasMaxLength(50);
            Property(x => x.IsMaster).HasColumnName("ISMASTER").IsOptional().HasMaxLength(1);
            Property(x => x.HttpUrl).HasColumnName("HTTPURL").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName("DESCRIPTION").IsOptional().HasMaxLength(500);
        }
    }

    // UserFileServer
    internal partial class UserFileServerConfiguration : EntityTypeConfiguration<UserFileServer>
    {
        public UserFileServerConfiguration()
        {
			ToTable("USERFILESERVER");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("ID").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserName).HasColumnName("USERNAME").IsRequired().HasMaxLength(50);
            Property(x => x.ServerName).HasColumnName("SERVERNAME").IsRequired().HasMaxLength(50);
        }
    }

}

