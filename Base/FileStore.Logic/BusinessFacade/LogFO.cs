using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileStore.Logic.Domain;
using System.Web;

namespace FileStore.Logic.BusinessFacade
{
    public class LogFO
    {
        FileStoreEntities entities = new FileStoreEntities("FileStore");
        public void AddLog(string fileName, string operation, string desc)
        {
            int fileId = FileHelper.GetFileId(fileName);

            FsLog log = new FsLog();

            log.Description = desc;
            log.FileId = fileId;
            log.FileName = fileName;
            log.OperateTime = DateTime.Now;
            log.Operation = operation;
            log.OperatorIp = GetIp();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                log.OperatorName = HttpContext.Current.User.Identity.Name;

            if (Config.Constant.IsOracleDb)
            {
                var sqlHelper = Config.SQLHelper.CreateSqlHelper("FileStore");
                var sql = "INSERT INTO FSLOG(ID,FILEID,FILENAME,OPERATION,OPERATORNAME,OPERATORIP,OPERATETIME,DESCRIPTION)     VALUES         (FSLOG_SEQUENCE.nextval,'{0}','{1}','{2}','{3}','{4}',to_date('{5}','yyyy/MM/dd hh24:mi:ss'),'{6}')";
                sql = string.Format(sql, log.FileId, log.FileName, log.Operation, log.OperatorName, log.OperatorIp, log.OperateTime, log.Description);

                sqlHelper.ExecuteNonQuery(sql);
            }
            else
            {
                entities.FsLog.Add(log);
            }
            entities.SaveChanges();
        }

        private string GetIp()
        {
            string ip;
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) // 服务器， using proxy
            {
                //得到真实的客户端地址
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();  // Return real client IP.
            }
            else//如果没有使用代理服务器或者得不到客户端的ip  not using proxy or can't get the Client IP
            {

                //得到服务端的地址
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
            }

            return ip;

        }

    }
}
