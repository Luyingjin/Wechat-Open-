using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using System.Data;
using Config;
using Base.Logic.Domain;

namespace Base.Logic.BusinessFacade
{
    public class MetaFO
    {
        public void ImportTable(string connName, bool ImportFields = true)
        {
            if (string.IsNullOrEmpty(connName))
                return;

            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            string sql = @"select tableName=d.name,description=f.value from sysobjects d 
left join sys.extended_properties   f   on   d.id=f.major_id   and   f.minor_id=0 
where d.xtype='U' and d.name<>'sysdiagrams' order by d.name";

            if (Config.Constant.IsOracleDb)
            {
                sql = "SELECT table_name tableName,'' as description FROM user_tables";
            }

            DataTable dtSource = sqlHelper.ExecuteDataTable(sql);
            sql = string.Format("select * from S_M_Table where ConnName='{0}'", connName);
            DataTable dtTarget = baseSqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dtTarget.Rows)
            {
                string id = row["ID"].ToString();
                string code = row["Code"].ToString();
                string name = row["Name"].ToString();
                DataRow sourceRow = dtSource.AsEnumerable().Where(c => c["tableName"].ToString().ToLower() == code.ToLower()).SingleOrDefault();
                if (sourceRow != null)
                {
                    string description = sourceRow["description"].ToString().Replace('\'', '\"');
                    if (description.Length > 50)
                        description = description.Substring(0, 50);

                    if (description != "")//改为以数据库为准
                    {
                        baseSqlHelper.ExecuteNonQuery(string.Format("update S_M_Table set Name='{1}' where ID='{0}'", id, description));
                    }

                    dtSource.Rows.Remove(sourceRow);
                }
                else
                {
                    baseSqlHelper.ExecuteNonQuery(string.Format("delete from S_M_Table where connName='{0}' and Code='{1}'", connName, code));
                }
            }

            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                DataRow row = dtSource.Rows[i];
                string description = row["description"].ToString().Replace('\'', '\"');
                if (description.Length > 50)
                    description = description.Substring(0, 50);
                baseSqlHelper.ExecuteNonQuery(string.Format("insert into S_M_Table(ID,Code,Name,ConnName) values('{0}','{1}','{2}','{3}')",
                    FormulaHelper.CreateGuid(i), row["tableName"], description, connName));
            }


            sql = string.Format("select * from S_M_Table where ConnName='{0}'", connName);
            dtTarget = baseSqlHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dtTarget.Rows)
            {
                ImportField(row["ID"].ToString());
            }



        }

        public void ImportField(string tableID)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();

            S_M_Table table = entities.Set<S_M_Table>().Where(c => c.ID == tableID).SingleOrDefault();

            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(table.ConnName);

            string sql = string.Format(@"SELECT  fieldCode= a.name , description= isnull(g.[value],''),fieldType=b.name,sortIndex=a.column_id
FROM  sys.columns a left join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = a.column_id)
left join systypes b on a.user_type_id=b.xusertype  
WHERE  object_id =(SELECT object_id FROM sys.tables WHERE name = '{0}')", table.Code);

            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format(@"select 
                     column_name fieldCode, 
                     data_type fieldType, 
                     data_length fieldLenght,
                     '' as description,
                     0 as sortIndex
                     from USER_TAB_COLS utc 
                     where table_name = upper('{0}')
                     order by column_id", table.Code);
            }

            DataTable dtSource = sqlHelper.ExecuteDataTable(sql);

            DataTable dtTarget = baseSqlHelper.ExecuteDataTable(string.Format("select * from S_M_Field where TableID='{0}'", tableID));

            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dtTarget.Rows)
            {
                string id = row["ID"].ToString();
                string code = row["Code"].ToString();
                string name = row["Name"].ToString();

                DataRow sourceRow = dtSource.AsEnumerable().Where(c => c["fieldCode"].ToString().ToLower() == code.ToLower()).FirstOrDefault();
                if (sourceRow != null)
                {
                    string description = sourceRow["description"].ToString().Replace('\'', '\"');
                    if (description.Length > 50)
                        description = description.Substring(0, 50);

                    var sqlUpdate = "";
                    if (description != "") //改为以数据库为准
                    {
                        sqlUpdate = string.Format(" UPDATE S_M_FIELD SET NAME='{1}',TYPE='{2}',SORTINDEX='{3}' WHERE ID='{0}'", id, description, sourceRow["fieldType"], sourceRow["sortIndex"]);
                        sb.AppendFormat(sqlUpdate);
                    }
                    else
                    {
                        sqlUpdate = string.Format(" UPDATE S_M_FIELD SET TYPE='{1}',SORTINDEX='{2}' WHERE ID='{0}'", id, sourceRow["fieldType"], sourceRow["sortIndex"]);
                        sb.AppendFormat(sqlUpdate);
                    }
                    if (Config.Constant.IsOracleDb)
                    {
                        baseSqlHelper.ExecuteNonQuery(sqlUpdate);
                    }
                    dtSource.Rows.Remove(sourceRow);
                }
                else
                {
                    var sqlDelete = string.Format(" DELETE FROM S_M_FIELD WHERE TABLEID='{0}' AND CODE='{1}'", tableID, code);
                    if (Config.Constant.IsOracleDb)
                    {
                        baseSqlHelper.ExecuteNonQuery(sqlDelete);
                    }
                    else
                    {
                        sb.AppendFormat(sqlDelete);
                    }
                }
            }

            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                DataRow row = dtSource.Rows[i];
                string description = row["description"].ToString().Replace('\'', '\"');
                if (description.Length > 50)
                    description = description.Substring(0, 50);
                string fieldType = row["fieldType"].ToString();
                object sortIndex = row["sortIndex"].ToString();

                var sqlInsert = string.Format(" INSERT INTO S_M_FIELD(ID,TABLEID,CODE,NAME,TYPE,SORTINDEX) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
FormulaHelper.CreateGuid(i), tableID, row["fieldCode"], description, fieldType, sortIndex);
                if (Config.Constant.IsOracleDb)
                {
                    baseSqlHelper.ExecuteNonQuery(sqlInsert);
                }
                else
                {
                    sb.AppendFormat(sqlInsert);
                }
            }

            if (!Config.Constant.IsOracleDb)
            {
                baseSqlHelper.ExecuteNonQuery(sb.ToString());
            }

        }

        public void ImportField(string connName, string tableName)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            S_M_Table table = entities.Set<S_M_Table>().Where(c => c.ConnName == connName && c.Code == tableName).SingleOrDefault();
            ImportField(table.ID);
        }


    }
}
