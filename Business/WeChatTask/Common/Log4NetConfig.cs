using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using log4net.Config;

namespace WeChatTask.Common
{
    /// <summary>
    /// Log4Net服务配置类
    /// 默认只记录两种级别的日志：Error（异常日志）、Info（操作日志，这类日志需要开发人员手动调用）
    /// 可以自定义配置Log4Net，配置节名称是Log4NetConfigFile。如：<add key="Log4NetConfigFile" value="~/Log4Net.Config"/>
    /// 如果采用默认的配置，却希望修改日志路径则配置Log4NetPath，如：<add key="Log4NetPath" value="D:\Log"/>
    /// </summary>
    public class Log4NetConfig
    {
        public static void Configure(string configFile = null)
        {
            configFile = configFile ?? ConfigurationManager.AppSettings["Log4NetConfigFile"];
            Trace.WriteLine("日志配置文件的路径为：" + configFile);
            if (string.IsNullOrWhiteSpace(configFile))
            {
                // 默认采用文件作为存储媒介
                ConfigureFile();
            }
            else
            {
                if (!File.Exists(configFile))
                {
                    string configFilePath = AppDomain.CurrentDomain.BaseDirectory + configFile.TrimStart('\\');
                    if (File.Exists(configFilePath))
                    {
                        log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilePath));
                    }
                    else
                    {
                        Trace.WriteLine("找不到指定的配置文件，文件路径为：" + configFilePath);
                    }
                }
                else
                {
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
                }
            }
        }

        protected static void ConfigureFile()
        {
            var configXML = @"
<log4net>
  <!-- 错误日志类-->
  <logger name='Exception'>
    <level value='ALL' />
    <appender-ref ref='ExceptionFileAppender' />
  </logger>
  <!-- 信息日志类 -->
  <logger name='Log'>
    <level value='ALL' />
    <appender-ref ref='LogFileAppender' />
  </logger>
  <appender name='LogFileAppender' type='log4net.Appender.RollingFileAppender,log4net'>
    <!--日志文件路径,按文件大小方式输出时在这里指定文件名，并且前面的日志按天在文件名后自动添加当天日期形成文件-->
    <param name='File' value='{logpath}Logs\'/>
    <!--是否追加到文件-->
    <param name='AppendToFile' value='true'/>
    <!--记录日志写入文件时，不锁定文本文件-->
    <lockingModel type='log4net.Appender.FileAppender+MinimalLock'/>
    <!--Unicode编码-->
    <Encoding value='UTF-8'/>
    <!--最多产生的日志文件数，value='－1'为不限文件数-->
    <param name='MaxSizeRollBackups' value='10'/>
    <!--是否只写到一个文件中-->
    <param name='StaticLogFileName' value='false'/>
    <!--按照何种方式产生多个日志文件(日期[Date],文件大小[Size],混合[Composite])-->
    <param name='RollingStyle' value='Date'/>
    <!--按日期产生文件夹，文件名［在日期方式与混合方式下使用］-->
    <param name='DatePattern' value='yyyy-MM-dd/yyyyMMdd&quot;.txt&quot;'/>
    <!--每个文件的大小。只在混合方式与文件大小方式下使用，超出大小的在文件名后自动增加1重新命名-->
    <param name='maximumFileSize' value='5000KB' />
    <!--记录的格式。-->
    <layout type='log4net.Layout.PatternLayout'>
      <param name='ConversionPattern' value='&lt;HR COLOR=blue&gt;%n【时间】%d{yyyy-MM-dd HH:mm:ss}【线程】%t &lt;BR&gt;%n【信息】%m &lt;BR&gt;%n%n'/>
    </layout>
  </appender>

  <appender name='ExceptionFileAppender' type='log4net.Appender.RollingFileAppender,log4net'>
    <!--日志文件路径,按文件大小方式输出时在这里指定文件名，并且前面的日志按天在文件名后自动添加当天日期形成文件-->
    <param name='File' value='{logpath}Logs\'/>
    <!--是否追加到文件-->
    <param name='AppendToFile' value='true'/>
    <!--记录日志写入文件时，不锁定文本文件-->
    <lockingModel type='log4net.Appender.FileAppender+MinimalLock'/>
    <!--Unicode编码-->
    <Encoding value='UTF-8'/>
    <!--最多产生的日志文件数，value='－1'为不限文件数-->
    <param name='MaxSizeRollBackups' value='10'/>
    <!--是否只写到一个文件中-->
    <param name='StaticLogFileName' value='false'/>
    <!--按照何种方式产生多个日志文件(日期[Date],文件大小[Size],混合[Composite])-->
    <param name='RollingStyle' value='Date'/>
    <!--按日期产生文件夹，文件名［在日期方式与混合方式下使用］-->
    <param name='DatePattern' value='yyyy-MM-dd/yyyyMMdd&quot;-Exception.txt&quot;'/>
    <!--每个文件的大小。只在混合方式与文件大小方式下使用，超出大小的在文件名后自动增加1重新命名-->
    <param name='maximumFileSize' value='5000KB' />
    <!--记录的格式。-->
    <layout type='log4net.Layout.PatternLayout'>
      <param name='ConversionPattern' value='&lt;HR COLOR=red&gt;%n【异常时间】：%d{yyyy-MM-dd HH:mm:ss}【线程】%t &lt;BR&gt;%n%m &lt;BR&gt;%n%n'/>
    </layout>
  </appender>
</log4net>";

            var logpath = ConfigurationManager.AppSettings["Log4NetPath"] ?? "";
            if (!string.IsNullOrWhiteSpace(logpath) && !logpath.EndsWith("\\"))
            {
                logpath = logpath + "\\";
            }
            XmlConfigurator.Configure(new MemoryStream(Encoding.UTF8.GetBytes(configXML.Replace("{logpath}", logpath))));
        }

        protected static void ConfigureMongoDB()
        {
            XmlConfigurator.Configure(new MemoryStream(Encoding.UTF8.GetBytes(@"
<log4net>
	<appender name='MongoDBAppender' type='Log4Mongo.MongoDBAppender, Log4Mongo'>
		<connectionString value='mongodb://localhost' />
        <collectionName value='exlogs' />
	</appender>
	<root>
		<level value='ALL' />
		<appender-ref ref='MongoDBAppender' />
	</root>
</log4net>
")));
        }
    }
}
