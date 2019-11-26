using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Formula.Helper;
using System.Reflection;
using System.Collections;
using System.Data;

namespace MvcAdapter
{
    public class NewtonJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            //确认是否用于响应HTTP-Get请求
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Compare(context.HttpContext.Request.HttpMethod, "GET", true) == 0)
            {
                throw new InvalidOperationException("禁止Get请求");
            }

            HttpResponseBase response = context.HttpContext.Response;
            //设置媒体类型和编码方式
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ?
                "application/json" : this.ContentType;
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            //序列化对象，并写入当前的HttpResponse
            if (null == this.Data) return;


            if (this.Data is string)
            {
                response.Write(this.Data.ToString());                
            }
            else if (this.Data is DataRow)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataRow row = this.Data as DataRow;
                foreach (DataColumn col in row.Table.Columns)
                {
                    dic.Add(col.ColumnName, row[col]);
                }
                response.Write(JsonHelper.ToJson(dic));
            }
            else
            {
                response.Write(JsonHelper.ToJson(this.Data));
            }

        }

        #region remark

        //protected string ToJson(object obj)
        //{
        //    object result = null;

        //    Type t = obj.GetType();
        //    if (t == typeof(string))
        //        return obj.ToString();
        //    if (obj is GridData)
        //    {
        //        GridData gridData = obj as GridData;
        //        gridData.data = ToList(gridData.data);
        //        result = gridData;

        //    }
        //    else if (obj is IQueryable || obj is ICollection)
        //    {
        //        result = ToList(obj);
        //    }
        //    else if (obj is DataTable)
        //    {
        //        result = obj;
        //    }
        //    else
        //    {
        //        result = ToDic(obj);
        //    }

        //    return JsonHelper.ToJson(result);
        //}




        //protected List<Dictionary<string, object>> ToList(object obj)
        //{
        //    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();


        //    if (obj is ICollection)
        //    {
        //        ICollection list = obj as ICollection;
        //        foreach (var item in list)
        //        {
        //            result.Add(ToDic(item));
        //        }
        //    }
        //    else if(obj is IQueryable)
        //    {
        //        IQueryable list = obj as IQueryable;
        //        foreach (var item in list)
        //        {
        //            result.Add(ToDic(item));
        //        }

        //    }
        //    else if (obj is DataTable)
        //    {
        //        DataTable dt = obj as DataTable;
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            Dictionary<string, object> dic = new Dictionary<string, object>();
        //            foreach (DataColumn col in dt.Columns)
        //            {

        //                dic.Add(col.ColumnName, row[col]);
        //            }
        //            result.Add(dic);
        //        }
        //    }          
        //    return result;
        //}


        //protected Dictionary<string, object> ToDic(object obj)
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();

        //    IEnumerable<PropertyInfo> pis = obj.GetType().GetProperties();
        //    foreach (PropertyInfo pi in pis)
        //    {
        //        if (pi.PropertyType != typeof(string) && !pi.PropertyType.IsValueType)
        //            continue;

        //        object value = pi.GetValue(obj, null);

        //        dic.Add(pi.Name, value);

        //    }

        //    return dic;
        //}

        #endregion
    }
}
