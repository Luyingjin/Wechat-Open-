using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Formula.Helper;
using MvcAdapter;
using Formula;
using System.Web;

namespace Config
{
    public static class SQLHelperExtend
    {
        #region SQLHelper支持QueryBuilder


        public static GridData ExecuteGridData(this SQLHelper sqlHelper, string sql, QueryBuilder qb, bool dealOrderby = true)
        {
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb, dealOrderby);
            GridData gridData = new GridData(dt);
            gridData.total = qb.TotolCount;
            return gridData;
        }

        #endregion

        #region 查询列表为树状结构

        //    public static DataTable GetTreeNodes(this SQLHelper sqlHelper, string sql, string parentID)
        //    {     
        //        sql = string.Format("select * from {0} as table1 where ParentID ='{1}'", sql, parentID);
        //        DataTable dt = sqlHelper.ExecuteDataTable(sql);

        //        dt.Columns.Add(new DataColumn("isLeaf", typeof(bool)));
        //        dt.Columns.Add(new DataColumn("expanded", typeof(bool)));
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            row["isLeaf"] = row["ChildCount"].ToString() == "0";
        //            row["expanded"] = false;
        //        }

        //        return dt;
        //    }

        //    public static DataTable GetTree(this SQLHelper sqlHelper, string sql, string rootFullID, int loadLayCount = 1)
        //    {
        //        loadLayCount = loadLayCount + rootFullID.Split('.').Length;

        //        sql = string.Format("select * from {0} as table1 where FullID like '{1}%' and LayerIndex<'{2}'", sql, rootFullID, loadLayCount);
        //        DataTable dt = sqlHelper.ExecuteDataTable(sql);

        //        dt.Columns.Add(new DataColumn("isLeaf", typeof(bool)));
        //        dt.Columns.Add(new DataColumn("expanded", typeof(bool)));
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            row["isLeaf"] = row["ChildCount"].ToString() == "0";
        //            int layerIndex = Convert.ToInt32(row["LayerIndex"]);
        //            row["expanded"] = layerIndex + 1 < loadLayCount;
        //        }

        //        return dt;
        //    }



        //    public static DataTable GetTableTree(this SQLHelper sqlHelper, string sql, string layerFields, string layerNameFields = "", string orderBy = "")
        //    {

        //        if (orderBy == "")
        //            orderBy = layerFields;

        //        string[] notNullFields = layerFields.Split(',');
        //        for (int i = 0; i < notNullFields.Length; i++)
        //        {
        //            notNullFields[i] = string.Format("{0}=isnull({0},'')", notNullFields[i]);
        //        }


        //        string[] fields = layerFields.Split(',');

        //        List<string> listValueField = new List<string>();
        //        listValueField.Add("'-1'");
        //        listValueField.Add("''");
        //        string str = "";
        //        for (int i = 0; i < fields.Length; i++)
        //        {
        //            if (i == 0)
        //                str = fields[i];
        //            else
        //            {
        //                str += string.Format("+'.'+{0}", fields[i]);
        //            }
        //            listValueField.Add(str);
        //        }

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" select ChildCount");
        //        sb.AppendLine();

        //        //ID
        //        sb.Append(",ID= case ");
        //        sb.AppendLine();
        //        for (int i = 0; i < fields.Length; i++)
        //        {
        //            sb.AppendFormat(" when {0} is null then {1}", fields[i], listValueField[i + 1]);
        //            sb.AppendLine();
        //        }
        //        sb.AppendFormat(" else {0} end", listValueField[fields.Length + 1]);

        //        //ParentID
        //        sb.Append(",ParentID= case ");
        //        sb.AppendLine();
        //        for (int i = 0; i < fields.Length; i++)
        //        {
        //            sb.AppendFormat(" when {0} is null then {1}", fields[i], listValueField[i]);
        //            sb.AppendLine();
        //        }
        //        sb.AppendFormat(" else {0} end ", listValueField[fields.Length]);
        //        sb.AppendLine();

        //        //NodeName
        //        sb.Append(",NodeName= case ");
        //        sb.AppendLine();
        //        sb.AppendFormat(" when {0} is null then ''", fields[0]);
        //        sb.AppendLine();
        //        for (int i = 1; i < fields.Length; i++)
        //        {
        //            sb.AppendFormat(" when {0} is null then {1}", fields[i], fields[i - 1]);
        //            sb.AppendLine();
        //        }
        //        sb.AppendFormat(" else {0} end ", fields[fields.Length - 1]);
        //        sb.AppendLine();

        //        //NodeType
        //        sb.Append(",NodeType= case ");
        //        sb.AppendLine();
        //        sb.AppendFormat(" when {0} is null then ''", fields[0]);
        //        sb.AppendLine();
        //        for (int i = 1; i < fields.Length; i++)
        //        {
        //            sb.AppendFormat(" when {0} is null then '{1}'", fields[i], fields[i - 1]);
        //            sb.AppendLine();
        //        }
        //        sb.AppendFormat(" else '{0}' end ", fields[fields.Length - 1]);
        //        sb.AppendLine();


        //        sb.AppendLine();
        //        sb.Append(" from (");
        //        sb.AppendLine();
        //        sb.AppendFormat(" select {0},ChildCount=count(1) from (", layerFields);
        //        sb.AppendLine();
        //        sb.AppendFormat(" select {0} from ({1}) as table1", string.Join(",", notNullFields), sql);
        //        sb.AppendLine();
        //        sb.AppendFormat(") as table1 group by {0} with rollup", layerFields);
        //        sb.AppendLine();
        //        sb.AppendFormat(") as table2 order by {0}", orderBy);



        //        DataTable dtResult = sqlHelper.ExecuteDataTable(sb.ToString());
        //        foreach (var item in dtResult.AsEnumerable().Where(c => c["ID"].ToString() == "").ToArray())
        //        {
        //            dtResult.Rows.Remove(item);
        //        }

        //        DataRow row;
        //        //移除NodeNode为空的节点,并将ID和ParentID编码
        //        for (int i = dtResult.Rows.Count - 1; i >= 0; i--)
        //        {
        //            row = dtResult.Rows[i];
        //            if (row["NodeName"].ToString().Trim() == "")
        //                dtResult.Rows.Remove(row);
        //            else
        //            {
        //                row["ID"] = HttpContext.Current.Server.UrlEncode(row["ID"].ToString());
        //                row["ParentID"] = HttpContext.Current.Server.UrlEncode(row["ParentID"].ToString());
        //            }
        //        }


        //        //修正NodeName值（分级字段并非显示的Title字段）
        //        string[] layerNameFieldArr = layerNameFields.Split(',');
        //        for (int i = 0; i < layerNameFieldArr.Length; i++)
        //        {
        //            if (layerNameFieldArr[i] == "" || layerNameFieldArr[i] == fields[i])
        //                continue;
        //            string strSql = string.Format("select value={1},text=max({2}) from({0}) table1 group by {1}", sql, fields[i], layerNameFieldArr[i]);
        //            DataTable dt = sqlHelper.ExecuteDataTable(strSql);

        //            foreach (DataRow item in dtResult.AsEnumerable().Where(c => c["NodeType"].ToString() == fields[i]))
        //            {
        //                string value = item["NodeName"].ToString().Trim();
        //                if (value == "")
        //                    continue;
        //                string name = dt.AsEnumerable().Where(c => c["value"].ToString() == value).SingleOrDefault()["text"].ToString();
        //                item["NodeName"] = name;
        //            }
        //        }

        //        return dtResult;
        //    }



        #endregion

    }
}
