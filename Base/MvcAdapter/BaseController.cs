using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Entity;
using Formula.Helper;
using Formula.DynConditionObject;
using Formula;
using System.Reflection;
using Formula.Exceptions;
using Config;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Net;
using System.Configuration;
using System.Data.Entity.Validation;
using System.ComponentModel;
using log4net;
using System.Web.Configuration;

namespace MvcAdapter
{
    /// <summary>
    /// 实体状态
    /// </summary>
    public enum EntityStatus
    {
        added,
        modified,
        deleted,
    }

    public abstract class BaseController : Controller
    {
        #region entities

        /// <summary>
        /// 实体对象库
        /// </summary>
        protected abstract DbContext entities
        {
            get;
        }

        #endregion

        #region 异常处理

        protected override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;

            //if (!filterContext.HttpContext.Request.IsAjaxRequest())
            //    return;

            TransferExceptionMessage(filterContext);
            base.OnException(filterContext);
        }

        protected virtual void TransferExceptionMessage(ExceptionContext filterContext)
        {
            Exception exp = filterContext.Exception.GetBaseException();
            string msg = exp.Message;

            if (String.IsNullOrEmpty(msg))
                msg = "在处理您的请求时发生异常，请刷新页面并再次尝试。";
            else if (exp is SqlException)
            {
                var msgSplit = msg.Split('\'');
                if (msgSplit.Length == 5 && msgSplit[0].Contains("不能在具有唯一索引") && msgSplit[4].Contains("中插入重复键的行"))
                {
                    msg = msgSplit[1] + "不能重复";
                }
            }
            else if (exp is DbEntityValidationException)
            {
                var sb = new StringBuilder();
                var validateExp = exp as DbEntityValidationException;

                List<Type> listEntityType = new List<Type>();
                foreach (var validationErrors in validateExp.EntityValidationErrors)
                {
                    Type entityType = validationErrors.Entry.Entity.GetType();
                    object[] arr = entityType.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    string entityName = arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : entityType.Name;
                    entityName = entityName.TrimEnd('表');

                    if (!listEntityType.Contains(entityType))
                        listEntityType.Add(entityType);

                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var finfo = entityType.GetProperty(validationError.PropertyName);
                        object[] arr1 = finfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                        string fieldName = arr.Length > 0 ? ((DescriptionAttribute)arr1[0]).Description : validationError.PropertyName;
                        if (string.IsNullOrEmpty(fieldName))
                            fieldName = validationError.PropertyName;

                        var msgSplit = validationError.ErrorMessage.Split('\'');

                        //"The field Description must be a string or array type with a maximum length of '500'."
                        string msgFeature = string.Format("The field {0} must be a string or array type with a maximum length of", validationError.PropertyName);
                        if (msgSplit.Length == 3 && msgSplit[0].Trim() == msgFeature)
                        {
                            if (listEntityType.Count() > 1)
                            {
                                sb.AppendFormat("{0}的{1}长度不能超过{2}个字符.\r\n", entityName, fieldName, msgSplit[1]);
                            }
                            else
                            {
                                sb.AppendFormat("{0}长度不能超过{1}个字符.\r\n", fieldName, msgSplit[1]);

                            }
                        }
                    }

                }

                msg = sb.ToString();
            }
            else if (exp.GetType().Name.Contains("OracleException"))
            {
                if (msg.StartsWith("ORA-00001: unique constraint"))
                {
                    msg = msg.Substring(msg.IndexOf('.') + 1);
                    msg = msg.Split(')')[0] + "不能重复！";
                }
            }

            string result = "发生异常，且没有异常信息";
            if (!string.IsNullOrEmpty(msg))
                result = msg;
            else if (!string.IsNullOrEmpty(filterContext.Exception.Message))
                result = filterContext.Exception.Message;


            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HttpContext.ClearError();
                HttpContext.Response.Clear();
                HttpContext.Response.Write(result);
            }
            else
            {
                throw new Exception(result);
            }

        }

        #endregion

        #region 辅助方法

        #region GetValues

        protected string[] GetValues(string listJson, string attr)
        {
            List<Dictionary<string, object>> list = JsonHelper.ToObject<List<Dictionary<string, object>>>(listJson);
            return list.Select(c => c[attr].ToString()).ToArray();
        }

        protected string GetValueString(string listJson, string attr)
        {
            List<Dictionary<string, object>> list = JsonHelper.ToObject<List<Dictionary<string, object>>>(listJson);
            return string.Join(",", list.Select(c => c[attr].ToString()).ToArray());
        }

        /// <summary>
        /// 根据地址栏参数过滤或者根据IsDeleted过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        protected IQueryable<T> DataBaseFilter<T>(IQueryable<T> query)
        {
            //地址栏参数作为查询条件
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key == null || key == "_" || key == "_t" || key == "_winid" || key == "ID" || key == "FullID" || key == "FULLID" || key.StartsWith("$"))
                    continue;
                if (typeof(T).GetProperty(key) != null)
                {
                    Specifications res = new Specifications();
                    res.AndAlso(key, Request[key].Split(','), QueryMethod.In);
                    query = query.Where(res.GetExpression<T>());
                }
                else if (typeof(T).GetProperty(key.ToUpper()) != null)//兼容Oracle
                {
                    Specifications res = new Specifications();
                    res.AndAlso(key.ToUpper(), Request[key].Split(','), QueryMethod.In);
                    query = query.Where(res.GetExpression<T>());
                }

            }
            //过滤已删除数据
            var isDeletedPty = typeof(T).GetProperty("IsDeleted");
            if (isDeletedPty != null && string.IsNullOrEmpty(Request["IsDeleted"]) && Request["ContainDeletedData"] != "True")
            {
                Specifications res = new Specifications();
                res.Or("IsDeleted", "0", QueryMethod.Equal, "Group1");
                res.Or("IsDeleted", null, QueryMethod.Equal, "Group1");
                query = query.Where(res.GetExpression<T>());
            }

            return query;
        }


        #endregion

        #region GetQueryString

        private Dictionary<string, object> _formDic;
        private Dictionary<string, object> formDic
        {
            get
            {
                if (_formDic == null)
                {
                    if (!string.IsNullOrEmpty(Request["FormData"]))
                        _formDic = JsonHelper.ToObject<Dictionary<string, object>>(Request["FormData"]);
                    else
                        _formDic = new Dictionary<string, object>();
                }
                return _formDic;
            }
        }

        /// <summary>
        /// 获取地址栏参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetQueryString(string key)
        {
            string value = Request.QueryString[key];
            if (string.IsNullOrEmpty(value))
                value = Request.Form[key];
            if (string.IsNullOrEmpty(value))
            {
                if (formDic.ContainsKey(key))
                    value = formDic[key].ToString();
            }

            if (value != null)
                return value;
            else
                return "";
        }

        #endregion

        #region GetEntity<TEntity>

        /// <summary>
        /// 根据ID获取实体对象，如果ID为空，则返回新对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected TEntity GetEntity<TEntity>(string id) where TEntity : class, new()
        {
            if (string.IsNullOrEmpty(id))
            {
                TEntity obj = new TEntity { };
                PropertyInfo pi = typeof(TEntity).GetProperty("ID");
                pi.SetValue(obj, FormulaHelper.CreateGuid(), null);
                EntityCreateLogic<TEntity>(obj);

                PropertyInfo piState = typeof(TEntity).GetProperty("_state");
                if (piState != null)
                    piState.SetValue(obj, "added", null);
                return obj;
            }
            else
            {
                Specifications res = new Specifications();
                res.AndAlso("ID", id, QueryMethod.Equal);
                TEntity obj = entities.Set<TEntity>().Where(res.GetExpression<TEntity>()).FirstOrDefault();

                if (obj == null)
                {
                    obj = new TEntity { };
                    PropertyInfo pi = typeof(TEntity).GetProperty("ID");
                    pi.SetValue(obj, id, null);
                    EntityCreateLogic<TEntity>(obj);
                    PropertyInfo piState = typeof(TEntity).GetProperty("_state");
                    if (piState != null)
                        piState.SetValue(obj, "modified", null);
                }
                return obj;
            }
        }

        #endregion

        #region UpdateList<TEntity>


        protected List<TEntity> UpdateList<TEntity>() where TEntity : class,new()
        {
            string listJson = Request.Form["ListData"];
            return UpdateList<TEntity>(listJson);
        }

        /// <summary>
        /// 保存列表方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        protected List<TEntity> UpdateList<TEntity>(string listData) where TEntity : class,new()
        {
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(listData);
            return UpdateList<TEntity>(rows);
        }

        protected List<TEntity> UpdateList<TEntity>(List<Dictionary<string, object>> arrDic) where TEntity : class,new()
        {
            List<TEntity> list = new List<TEntity>();

            PropertyInfo piID = typeof(TEntity).GetProperty("ID");
            PropertyInfo piState = typeof(TEntity).GetProperty("_state");

            foreach (Dictionary<string, object> row in arrDic)
            {
                String id = row.Keys.Contains("ID") && row["ID"] != null ? row["ID"].ToString() : "";
                String state = row.Keys.Contains("_state") && row["_state"] != null ? row["_state"].ToString() : "";

                TEntity entity = null;
                if (state == "added" || id == "")
                {
                    if (id == "")
                    {
                        entity = new TEntity { };
                        id = FormulaHelper.CreateGuid();
                        piID.SetValue(entity, id, null);
                        UpdateEntity<TEntity>(entity, row);
                        EntityCreateLogic<TEntity>(entity);
                        EntitySaveLogic<TEntity>(entity);
                        entities.Set<TEntity>().Add(entity);
                        list.Add(entity);
                        if (piState != null)
                            piState.SetValue(entity, "added", null);
                    }
                    else
                    {
                        entity = entities.Set<TEntity>().Find(id);
                        if (entity == null)
                        {
                            entity = new TEntity { };
                            piID.SetValue(entity, id, null);
                            UpdateEntity<TEntity>(entity, row);
                            EntityCreateLogic<TEntity>(entity);
                            EntitySaveLogic<TEntity>(entity);
                            entities.Set<TEntity>().Add(entity);
                            list.Add(entity);
                            if (piState != null)
                                piState.SetValue(entity, "added", null);

                        }
                        else
                        {
                            UpdateEntity<TEntity>(entity, row);
                            EntityCreateLogic<TEntity>(entity);
                            EntitySaveLogic<TEntity>(entity);
                            list.Add(entity);
                            if (piState != null)
                                piState.SetValue(entity, "modified", null);
                        }
                    }
                }
                else if (state == "removed" || state == "deleted")
                {
                    entity = entities.Set<TEntity>().Find(id);
                    if (entity != null)
                    {
                        entities.Set<TEntity>().Remove(entity);
                        list.Add(entity);
                        if (piState != null)
                            piState.SetValue(entity, "deleted", null);
                    }
                }
                else if (state == "modified" || state == "")
                {
                    entity = entities.Set<TEntity>().Find(id);
                    if (entity != null)
                    {
                        UpdateEntity<TEntity>(entity, row);
                        EntityModifyLogic<TEntity>(entity);
                        EntitySaveLogic<TEntity>(entity);
                        list.Add(entity);
                        if (piState != null)
                            piState.SetValue(entity, "modified", null);
                    }
                    else
                    {
                        if (id == "")
                            id = FormulaHelper.CreateGuid();
                        entity = new TEntity { };
                        piID.SetValue(entity, id, null);
                        UpdateEntity<TEntity>(entity, row);
                        EntityCreateLogic<TEntity>(entity);
                        EntitySaveLogic<TEntity>(entity);
                        entities.Set<TEntity>().Add(entity);
                        list.Add(entity);
                        if (piState != null)
                            piState.SetValue(entity, "added", null);
                    }
                }
            }

            return list;
        }

        #endregion

        #region UpdateSortedList

        protected List<TEntity> UpdateSortedList<TEntity>() where TEntity : class,new()
        {
            return UpdateSortedList<TEntity>(Request["SortedListData"], Request["DeletedListData"]);
        }

        protected List<TEntity> UpdateSortedList<TEntity>(string sortedListData, string deletedListData) where TEntity : class,new()
        {
            if (!string.IsNullOrEmpty(deletedListData))
            {
                var ids = GetValues(deletedListData, "ID");
                Specifications res = new Specifications();
                res.AndAlso("ID", ids, QueryMethod.In);
                entities.Set<TEntity>().Delete(res.GetExpression<TEntity>());
            }

            var list = UpdateList<TEntity>(sortedListData);

            PropertyInfo pi = typeof(TEntity).GetProperty("SortIndex", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    pi.SetValue(list[i], (double)i, null);
                }
            }
            return list;
        }

        #endregion

        #region UpdateNode<TEntity>

        public TEntity UpdateNode<TEntity>() where TEntity : class ,new()
        {
            return UpdateNode<TEntity>(GetQueryString("ID"));
        }

        public TEntity UpdateNode<TEntity>(string id) where TEntity : class ,new()
        {
            TEntity entity = UpdateEntity<TEntity>(id);

            Specifications res = new Specifications();
            string fullID = Request["FullID"];
            string parentID = Request["ParentID"];
            if (string.IsNullOrEmpty(parentID) && !string.IsNullOrEmpty(fullID))
                parentID = fullID.Split('.').Last();

            if (!string.IsNullOrEmpty(parentID))
            {
                PropertyInfo ptyParentID = typeof(TEntity).GetProperty("ParentID");
                ptyParentID.SetValue(entity, parentID, null);
            }

            PropertyInfo pi = typeof(TEntity).GetProperty("ID");
            id = pi.GetValue(entity, null).ToString();


            if (!string.IsNullOrEmpty(fullID)) //新增的情况
            {
                if (!fullID.EndsWith(id))
                {
                    fullID += "." + id;
                    typeof(TEntity).GetProperty("FullID").SetValue(entity, fullID, null);

                    //如果包含LayerIndex字段则设置
                    PropertyInfo pInfoLayerIndex = typeof(TEntity).GetProperty("LayerIndex");
                    if (pInfoLayerIndex != null)
                        pInfoLayerIndex.SetValue(entity, fullID.Split('.').Count(), null);

                    //如果包含ChildCount字段则设置父节点的ChildCount
                    PropertyInfo pInfoChildCount = typeof(TEntity).GetProperty("ChildCount");
                    if (pInfoChildCount != null)
                    {
                        res.Clear();
                        res.AndAlso("ID", parentID, QueryMethod.Equal);
                        var parent = entities.Set<TEntity>().Where(res.GetExpression<TEntity>()).SingleOrDefault();

                        int childCount = Convert.ToInt32(pInfoChildCount.GetValue(parent, null));
                        pInfoChildCount.SetValue(parent, childCount + 1, null);
                        pInfoChildCount.SetValue(entity, 0, null);
                    }

                    //如果包含SortIndex
                    PropertyInfo ptySortIndex = typeof(TEntity).GetProperty("SortIndex");
                    if (ptySortIndex != null)
                    {
                        res.Clear();
                        res.AndAlso("ParentID", parentID, QueryMethod.Equal);
                        var last = entities.Set<TEntity>().Where(res.GetExpression<TEntity>()).OrderBy("SortIndex", false).FirstOrDefault();
                        if (last == null)
                        {
                            ptySortIndex.SetValue(entity, 0.0, null);
                        }
                        else
                        {
                            ptySortIndex.SetValue(entity, Convert.ToDouble(ptySortIndex.GetValue(last, null)) + 1, null);
                        }
                    }
                }
            }



            return entity;
        }

        #endregion

        #endregion

        #region JsonResult辅助方法

        #region JsonGetList

        protected JsonResult JsonGetList<T>(QueryBuilder qb, IQueryable<T> query = null) where T : class,new()
        {
            if (query == null)
                query = entities.Set<T>().AsQueryable();

            query = DataBaseFilter<T>(query);

            var data = query.WhereToGridData(qb);
            return Json(data);
        }


        #endregion

        #region JsonGetModel

        protected JsonResult JsonGetModel<T>(string id) where T : class,new()
        {
            return Json(GetEntity<T>(id));
        }

        #endregion

        #region JsonSave<TEntity>

        protected JsonResult JsonSave<TEntity>(TEntity entity = null) where TEntity : class,new()
        {
            if (entity == null)
            {
                entity = UpdateEntity<TEntity>();
            }

            entities.SaveChanges();

            PropertyInfo pi = typeof(TEntity).GetProperty("ID");
            if (pi != null)
                return Json(new { ID = pi.GetValue(entity, null) });
            return Json(new { ID = "" });
        }

        #endregion

        #region JsonSaveList

        protected JsonResult JsonSaveList<T>(string listData = "") where T : class,new()
        {
            if (listData == "")
                listData = Request["ListData"];

            UpdateList<T>(listData);
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonSaveSortedList

        protected JsonResult JsonSaveSortedList<T>(string sortedListData = "", string deletedListData = "") where T : class,new()
        {
            if (string.IsNullOrEmpty(sortedListData))
                sortedListData = Request["SortedListData"];
            if (string.IsNullOrEmpty(deletedListData))
                deletedListData = Request["DeletedListData"];

            UpdateSortedList<T>(sortedListData, deletedListData);

            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonDelete

        protected JsonResult JsonDelete<T>(string listIDs) where T : class,new()
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", listIDs.Split(','), QueryMethod.In);


            PropertyInfo ptyChildCount = typeof(T).GetProperty("ChildCount");
            PropertyInfo ptyParentID = typeof(T).GetProperty("ParentID");
            PropertyInfo ptyFullID = typeof(T).GetProperty("FullID");
            if (ptyChildCount == null)
            {
                entities.Set<T>().Delete<T>(res.GetExpression<T>());
            }
            else //维护父节点ChildCount
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (var item in entities.Set<T>().Where<T>(res.GetExpression<T>()).ToArray())
                {
                    string parentID = ptyParentID.GetValue(item, null).ToString();
                    if (!dic.Keys.Contains(parentID))
                        dic.Add(parentID, 0);
                    dic[parentID] = dic[parentID] + 1;
                    res.Clear();
                    res.AndAlso("FullID", ptyFullID.GetValue(item, null).ToString(), QueryMethod.StartsWith);
                    entities.Set<T>().Delete(res.GetExpression<T>());
                }
                var pids = dic.Select(c => c.Key).ToArray();
                res.Clear();
                res.AndAlso("ID", pids, QueryMethod.In);
                foreach (var item in entities.Set<T>().Where(res.GetExpression<T>()).ToArray())
                {
                    int childCount = int.Parse(ptyChildCount.GetValue(item, null).ToString());
                    string pid = typeof(T).GetProperty("ID").GetValue(item, null).ToString();
                    childCount = childCount - dic[pid];
                    ptyChildCount.SetValue(item, childCount, null);
                }

            }
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonGetTree

        protected JsonResult JsonGetTree<T>(string rootFullID, IQueryable<T> query = null) where T : class,new()
        {
            return JsonGetTree<T>(rootFullID, -1, query);
        }

        protected JsonResult JsonGetTree<T>(string rootFullID, int loadLayerCont, IQueryable<T> query = null) where T : class,new()
        {
            if (rootFullID == null)
                rootFullID = "";

            Specifications res = new Specifications();
            if (query == null)
                query = entities.Set<T>().AsQueryable();

            res.Clear();
            res.AndAlso("FullID", rootFullID, QueryMethod.StartsWith);
            query = query.Where(res.GetExpression<T>());

            query = DataBaseFilter<T>(query); //数据过滤

            if (typeof(T).GetProperty("SortIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) != null)
                query = query.OrderBy("SortIndex", true);

            if (loadLayerCont > 0) //lazy tree
            {
                loadLayerCont += rootFullID.Split('.').Length;

                res.Clear();
                res.AndAlso("LayerIndex", rootFullID.Split('.').Length + loadLayerCont, QueryMethod.LessThan);
                var result = query.Where(res.GetExpression<T>()).ToArray();

                List<Dictionary<string, object>> list = FormulaHelper.CollectionToListDic<T>(result);

                foreach (Dictionary<string, object> dic in list)
                {
                    dic["isLeaf"] = dic["ChildCount"].ToString() == "0";
                    int layerIndex = Convert.ToInt32(dic["LayerIndex"]);
                    dic["expanded"] = layerIndex < loadLayerCont;
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region JsonGetTreeNodes

        protected JsonResult JsonGetTreeNodes<T>(string parentID, IQueryable<T> query = null) where T : class,new()
        {
            if (query == null)
                query = entities.Set<T>().AsQueryable();

            if (string.IsNullOrEmpty(parentID))
                throw new BusinessException("缺少地址栏参数parentID");

            PropertyInfo ptySortIndex = typeof(T).GetProperty("SortIndex");

            Specifications res = new Specifications();

            res.Clear();
            res.AndAlso("ParentID", parentID, QueryMethod.Equal);
            query = query.Where(res.GetExpression<T>());

            query = DataBaseFilter<T>(query); //数据过滤

            if (ptySortIndex != null)
                query = query.OrderBy("SortIndex", true);

            List<Dictionary<string, object>> list = FormulaHelper.CollectionToListDic<T>(query.ToArray());

            foreach (Dictionary<string, object> dic in list)
            {
                dic["isLeaf"] = dic["ChildCount"].ToString() == "0";
                int layerIndex = Convert.ToInt32(dic["LayerIndex"]);
                dic["expanded"] = false;
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region JsonSaveNode<TEntity>

        protected JsonResult JsonSaveNode<TEntity>(TEntity entity = null) where TEntity : class,new()
        {
            if (entity == null)
            {
                entity = UpdateNode<TEntity>();
            }
            string id = typeof(TEntity).GetProperty("ID").GetValue(entity, null).ToString();
            object parentID = typeof(TEntity).GetProperty("ParentID").GetValue(entity, null);
            string fullID = typeof(TEntity).GetProperty("FullID").GetValue(entity, null).ToString();

            entities.SaveChanges();

            return Json(new { ID = id, ParentID = parentID, FullID = fullID });
        }

        #endregion

        #region JsonDeleteNode

        protected JsonResult JsonDeleteNode<T>(string fullID) where T : class,new()
        {
            if (!fullID.Contains("."))
                throw new BusinessException("不能删除跟节点");

            Specifications res = new Specifications();
            res.AndAlso("FullID", fullID, QueryMethod.StartsWith);
            entities.Set<T>().Delete<T>(res.GetExpression<T>());


            //父节点ChildCount修改
            PropertyInfo ptyChildCount = typeof(T).GetProperty("ChildCount");
            if (fullID.Contains(".") && ptyChildCount != null)
            {
                string parentFullID = fullID.Substring(0, fullID.LastIndexOf("."));
                res.Clear();
                res.AndAlso("FullID", parentFullID, QueryMethod.Equal);
                var entity = entities.Set<T>().Where(res.GetExpression<T>()).SingleOrDefault();

                int childCount = Convert.ToInt32(ptyChildCount.GetValue(entity, null));
                if (childCount > 0)
                    childCount = childCount - 1;
                ptyChildCount.SetValue(entity, childCount, null);
            }


            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonDropNode<TEntity>

        protected JsonResult JsonDropNode<TEntity>() where TEntity : class,new()
        {
            string dragAction = Request.Form["dragAction"];
            Dictionary<string, object> dragNodeDic = JsonHelper.ToObject<Dictionary<string, object>>(Request.Form["dragNode"]);
            Dictionary<string, object> dropNodeDic = JsonHelper.ToObject<Dictionary<string, object>>(Request.Form["dropNode"]);

            PropertyInfo ptyID = typeof(TEntity).GetProperty("ID");
            PropertyInfo ptyParentID = typeof(TEntity).GetProperty("ParentID");
            PropertyInfo ptyFullID = typeof(TEntity).GetProperty("FullID");
            PropertyInfo ptySortIndex = typeof(TEntity).GetProperty("SortIndex");
            Specifications res = new Specifications();

            #region 所需参数

            string dragNodeID = dragNodeDic["ID"].ToString();
            string dragNodeFullID = dragNodeDic["FullID"].ToString();
            string dropNodeID = dropNodeDic["ID"].ToString();
            string dropNodeFullID = dropNodeDic["FullID"].ToString();


            List<TEntity> dragNodeList;
            List<TEntity> dropNodeList;

            TEntity dragNode;
            TEntity dropNode;

            #endregion

            #region 获取参数


            //dragNodeList
            res.Clear();
            res.AndAlso("FullID", dragNodeFullID, QueryMethod.StartsWith);
            dragNodeList = entities.Set<TEntity>().Where(res.GetExpression<TEntity>()).ToList();
            //dropNodeList
            res.Clear();
            object dropNodeParentID = dropNodeDic["ParentID"];
            if (dragAction == "add")
                dropNodeParentID = dropNodeDic["ID"].ToString();
            res.AndAlso("ParentID", dropNodeParentID, QueryMethod.Equal);
            dropNodeList = entities.Set<TEntity>().Where(res.GetExpression<TEntity>()).OrderBy("SortIndex", true).ToList();

            //dragNode
            dragNode = entities.Set<TEntity>().Find(dragNodeID);
            //dropNode
            dropNode = entities.Set<TEntity>().Find(dropNodeID);

            #endregion

            #region 移动节点

            if (dragAction == "add")
            {
                string newDragNodeFullID = dropNodeFullID + "." + dragNodeID;
                string newDragParentID = dropNodeID;

                //设置ParentID           
                ptyParentID.SetValue(dragNode, newDragParentID, null);

                //设置FullID
                foreach (TEntity item in dragNodeList)
                {
                    string fullID = ptyFullID.GetValue(item, null).ToString();
                    fullID = fullID.Replace(dragNodeFullID, newDragNodeFullID);
                    ptyFullID.SetValue(item, fullID, null);
                }
            }

            #endregion

            #region 排序

            //排序
            for (int i = 0; i < dropNodeList.Count; i++)
            {
                ptySortIndex.SetValue(dropNodeList[i], (double)i, null);
            }

            double d = 0;
            if (dragAction == "add")
                d = dropNodeList.Count;
            else
            {
                d = (double)ptySortIndex.GetValue(dropNode, null);
                if (dragAction == "before")
                    d = d - 0.3;
                else if (dragAction == "after")
                    d = d + 0.3;
            }


            ptySortIndex.SetValue(dragNode, d, null);

            #endregion

            entities.SaveChanges();

            return Json("");
        }

        #endregion

        #region JsonGetRelationAll

        protected JsonResult JsonGetRelationAll<T, TN, N>(string nodeFullID)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {
            if (string.IsNullOrEmpty(nodeFullID))
                return Json("");

            nodeFullID = nodeFullID.Split('.').Last(); //取ID

            string TidName = typeof(T).Name.Split('_').Last() + "ID";

            Specifications res = new Specifications();
            res.AndAlso(TidName, nodeFullID, QueryMethod.Equal);


            var c = Expression.Parameter(typeof(TN), "c");
            var propertyName = Expression.Property(c, typeof(N).Name);
            var lambda = Expression.Lambda<Func<TN, N>>(propertyName, c);

            var query = entities.Set<TN>().Where(res.GetExpression<TN>()).Select(lambda);

            query = DataBaseFilter<N>(query);
            return Json(query);
        }

        #endregion

        #region JsonGetRelationList

        protected JsonResult JsonGetRelationList<T, N>(string nodeFullID, QueryBuilder qb)
            where T : class,new()
            where N : class,new()
        {
            if (string.IsNullOrEmpty(nodeFullID))
                return Json("");
            nodeFullID = nodeFullID.Split('.').Last();

            string TidName = typeof(T).Name.Split('_').Last() + "ID";

            Specifications res = new Specifications();
            res.AndAlso(TidName, nodeFullID, QueryMethod.Equal);
            var query = entities.Set<N>().Where(res.GetExpression<N>());

            query = DataBaseFilter<N>(query);

            return Json(query.WhereToGridData(qb));
        }



        protected JsonResult JsonGetRelationList<T, TN, N>(string nodeFullID, QueryBuilder qb)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {
            if (string.IsNullOrEmpty(nodeFullID))
                return Json("");
            nodeFullID = nodeFullID.Split('.').Last();

            string TidName = typeof(T).Name.Split('_').Last() + "ID";

            Specifications res = new Specifications();
            res.AndAlso(TidName, nodeFullID, QueryMethod.Equal);


            var c = Expression.Parameter(typeof(TN), "c");
            var propertyName = Expression.Property(c, typeof(N).Name);
            var lambda = Expression.Lambda<Func<TN, N>>(propertyName, c);

            var query = entities.Set<TN>().Where(res.GetExpression<TN>()).Select(lambda);

            query = DataBaseFilter<N>(query);

            return Json(query.WhereToGridData(qb));
        }

        #endregion

        #region JsonSetRelation

        protected JsonResult JsonSetRelation<T, TN, N>(string leftID, string relationData, IQueryable<TN> originalList)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {

            if (string.IsNullOrEmpty(leftID) || string.IsNullOrEmpty(relationData))
                return Json("");

            string[] arrRelateID = GetValues(relationData, "ID").Distinct().ToArray();
            string TidName = typeof(T).Name.Split('_').Last() + "ID";
            string NidName = typeof(N).Name.Split('_').Last() + "ID";

            PropertyInfo pID = typeof(TN).GetProperty("ID");
            PropertyInfo pTID = typeof(TN).GetProperty(TidName);
            PropertyInfo pNID = typeof(TN).GetProperty(NidName);

            Specifications res = new Specifications();
            foreach (string rightID in arrRelateID)
            {
                TN tn = new TN();
                pTID.SetValue(tn, leftID, null);
                pNID.SetValue(tn, rightID, null);
                res.Clear();
                res.AndAlso(TidName, leftID, QueryMethod.Equal);
                res.AndAlso(NidName, rightID, QueryMethod.Equal);
                var item = originalList.Where<TN>(res.GetExpression<TN>()).SingleOrDefault();
                if (item == null) //如果关系不存在，则添加
                {
                    if (pID != null)
                        pID.SetValue(tn, FormulaHelper.CreateGuid(), null);
                    entities.Set<TN>().Add(tn);
                }
            }

            //删除不存在的关系               
            foreach (var item in originalList)
            {
                string id = pNID.GetValue(item, null).ToString();
                if (!arrRelateID.Contains(id))
                    entities.Set<TN>().Remove(item);
            }

            entities.SaveChanges();
            return Json("");
        }

        protected JsonResult JsonSetRelation<T, TN, N>(string nodeFullID, string relationData, string fullRelation)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {
            if (string.IsNullOrEmpty(nodeFullID) || string.IsNullOrEmpty(relationData))
                return Json("");
            bool bFullRelation = string.IsNullOrEmpty(fullRelation) ? false : bool.Parse(fullRelation);


            string[] arrNodeID = nodeFullID.Split('.');
            if (bFullRelation == false)
                arrNodeID = new string[] { nodeFullID.Split('.').Last() };


            string[] arrRelateID = GetValues(relationData, "ID").Distinct().ToArray();
            string TidName = typeof(T).Name.Split('_').Last() + "ID";
            string NidName = typeof(N).Name.Split('_').Last() + "ID";

            PropertyInfo pID = typeof(TN).GetProperty("ID");
            PropertyInfo pTID = typeof(TN).GetProperty(TidName);
            PropertyInfo pNID = typeof(TN).GetProperty(NidName);


            Specifications res = new Specifications();
            foreach (string leftID in arrNodeID)
            {
                res.Clear();
                res.AndAlso(TidName, leftID, QueryMethod.Equal);
                //已经存在的关系
                IQueryable<TN> originalList = entities.Set<TN>().Where(res.GetExpression<TN>());

                foreach (string rightID in arrRelateID)
                {
                    TN tn = new TN();
                    pTID.SetValue(tn, leftID, null);
                    pNID.SetValue(tn, rightID, null);
                    res.Clear();
                    res.AndAlso(TidName, leftID, QueryMethod.Equal);
                    res.AndAlso(NidName, rightID, QueryMethod.Equal);

                    var item = originalList.Where<TN>(res.GetExpression<TN>()).SingleOrDefault();
                    if (item == null) //如果关系不存在，则添加
                    {
                        if (pID != null)
                            pID.SetValue(tn, FormulaHelper.CreateGuid(), null);
                        entities.Set<TN>().Add(tn);
                    }
                }

                //删除不存在的关系               
                foreach (var item in originalList)
                {
                    string id = pNID.GetValue(item, null).ToString();
                    if (!arrRelateID.Contains(id))
                        entities.Set<TN>().Remove(item);
                }

            }


            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonAppendRelation

        protected JsonResult JsonAppendRelation<T, TN, N>(string nodeFullID, string relationData, string fullRelation)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {
            if (string.IsNullOrEmpty(nodeFullID) || string.IsNullOrEmpty(relationData))
                return Json("");
            bool bFullRelation = string.IsNullOrEmpty(fullRelation) ? false : bool.Parse(fullRelation);


            string[] arrNodeID = nodeFullID.Split('.');
            if (bFullRelation == false)
                arrNodeID = new string[] { nodeFullID.Split('.').Last() };


            string[] arrRelateID = GetValues(relationData, "ID").Distinct().ToArray();
            string TidName = typeof(T).Name.Split('_').Last() + "ID";
            string NidName = typeof(N).Name.Split('_').Last() + "ID";




            PropertyInfo pID = typeof(TN).GetProperty("ID");
            PropertyInfo pTID = typeof(TN).GetProperty(TidName);
            PropertyInfo pNID = typeof(TN).GetProperty(NidName);

            Specifications res = new Specifications();
            foreach (string leftID in arrNodeID)
            {
                res.Clear();
                res.AndAlso(TidName, leftID, QueryMethod.Equal);
                //已经存在的关系
                IQueryable<TN> originalList = entities.Set<TN>().Where(res.GetExpression<TN>());

                foreach (string rightID in arrRelateID)
                {
                    TN tn = new TN();
                    pTID.SetValue(tn, leftID, null);
                    pNID.SetValue(tn, rightID, null);
                    res.Clear();
                    res.AndAlso(TidName, leftID, QueryMethod.Equal);
                    res.AndAlso(NidName, rightID, QueryMethod.Equal);

                    var item = originalList.Where<TN>(res.GetExpression<TN>()).SingleOrDefault();
                    if (item == null) //如果关系不存在，则添加
                    {
                        if (pID != null)
                            pID.SetValue(tn, FormulaHelper.CreateGuid(), null);
                        entities.Set<TN>().Add(tn);
                    }
                }

            }

            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonDeleteRelation

        protected JsonResult JsonDeleteRelation<T, TN, N>(string nodeFullID, string relationData, string fullRelation)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {

            if (string.IsNullOrEmpty(nodeFullID) || string.IsNullOrEmpty(relationData))
                return Json("");

            bool bFullRelation = string.IsNullOrEmpty(fullRelation) ? false : bool.Parse(fullRelation);

            Specifications res = new Specifications();

            string[] arrRelateID = GetValues(relationData, "ID").Distinct().ToArray();

            string[] arrNodeID;
            if (bFullRelation == true)
            {
                res.Clear();
                res.AndAlso("FullID", nodeFullID, QueryMethod.StartsWith);
                var c = Expression.Parameter(typeof(T), "c");
                var propertyName = Expression.Property(c, "ID");
                var lambda = Expression.Lambda<Func<T, string>>(propertyName, c);
                arrNodeID = entities.Set<T>().Where(res.GetExpression<T>()).Select(lambda).ToArray();
            }
            else
            {
                arrNodeID = new string[] { nodeFullID.Split('.').Last() };
            }


            string TidName = typeof(T).Name.Split('_').Last() + "ID";
            string NidName = typeof(N).Name.Split('_').Last() + "ID";


            foreach (string leftID in arrNodeID)
            {
                res.Clear();
                res.AndAlso(TidName, leftID, QueryMethod.Equal);
                res.AndAlso(NidName, arrRelateID, QueryMethod.In);
                entities.Set<TN>().Delete(res.GetExpression<TN>());
            }

            entities.SaveChanges();

            return Json("");
        }

        #endregion

        #region JsonSaveRelationData

        protected JsonResult JsonSaveRelationData<T, N>(N entity = null)
            where T : class,new()
            where N : class,new()
        {
            if (entity == null)
                entity = UpdateEntity<N>();

            string nodeFullID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(nodeFullID))
            {
                entities.SaveChanges();
                return Json("");
            }

            string nodeID = nodeFullID.Split('.').Last();
            string TidName = typeof(T).Name.Split('_').Last() + "ID";
            typeof(N).GetProperty(TidName).SetValue(entity, nodeID, null);
            entities.SaveChanges();
            return Json("");
        }

        protected JsonResult JsonSaveRelationData<T, TN, N>(N entity = null)
            where T : class,new()
            where TN : class,new()
            where N : class,new()
        {
            if (entity == null)
                entity = UpdateEntity<N>();

            string nodeFullID = Request["NodeFullID"];

            //添加的情况           
            bool fullRelation = string.IsNullOrEmpty(Request["FullRelation"]) ? false : bool.Parse(Request["FullRelation"]);

            if (string.IsNullOrEmpty(nodeFullID))
            {
                entities.SaveChanges();
                return Json("");
            }


            string[] arrNodeID = nodeFullID.Split('.');
            if (fullRelation)
                arrNodeID = nodeFullID.Split('.');
            else
                arrNodeID = new string[] { nodeFullID.Split('.').Last() };


            string relateID = typeof(N).GetProperty("ID").GetValue(entity, null).ToString();


            string TidName = typeof(T).Name.Split('_').Last() + "ID";
            string NidName = typeof(N).Name.Split('_').Last() + "ID";

            PropertyInfo pID = typeof(TN).GetProperty("ID");
            PropertyInfo pTID = typeof(TN).GetProperty(TidName);
            PropertyInfo pNID = typeof(TN).GetProperty(NidName);

            foreach (string leftID in arrNodeID)
            {
                Specifications res = new Specifications();
                res.AndAlso(TidName, leftID, QueryMethod.Equal);
                res.AndAlso(NidName, relateID, QueryMethod.Equal);
                TN tn = entities.Set<TN>().Where(res.GetExpression<TN>()).SingleOrDefault();
                if (tn == null)
                {
                    tn = new TN();
                    pTID.SetValue(tn, leftID, null);
                    pNID.SetValue(tn, relateID, null);
                    if (pID != null)
                        pID.SetValue(tn, FormulaHelper.CreateGuid(), null);
                    entities.Set<TN>().Add(tn);
                }
            }

            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region JsonDeleteRelationData

        protected JsonResult JsonDeleteRelationData<N>(string relationData)
            where N : class,new()
        {
            Specifications res = new Specifications();

            if (string.IsNullOrEmpty(relationData))
                return Json("");

            string[] arrRelateID = GetValues(relationData, "ID").Distinct().ToArray();

            res.AndAlso("ID", arrRelateID, QueryMethod.In);

            entities.Set<N>().Delete(res.GetExpression<N>());

            entities.SaveChanges();

            return Json("");
        }

        #endregion

        #endregion

        #region 虚方法

        #region UpdateEntityFromJson

        public virtual TEntity UpdateEntityFromJson<TEntity>(string formJson) where TEntity : class ,new()
        {
            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(formJson);

            string id = formDic["ID"].ToString();

            var entity = entities.Set<TEntity>().Find(id);
            if (entity == null)
            {
                entity = new TEntity { };
                PropertyInfo pi = typeof(TEntity).GetProperty("ID");
                pi.SetValue(entity, id, null);
                EntityCreateLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity, formDic);
                entities.Set<TEntity>().Add(entity);
            }
            else
            {
                EntityModifyLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity, formDic);
            }

            TryValidateModel(entity);

            return entity;
        }

        #endregion

        #region UpdateEntity<TEntity>

        public virtual TEntity UpdateEntity<TEntity>() where TEntity : class ,new()
        {
            string id = GetQueryString("ID");
            return UpdateEntity<TEntity>(id);
        }

        public virtual TEntity UpdateEntity<TEntity>(string id) where TEntity : class ,new()
        {

            var entity = entities.Set<TEntity>().Find(id);
            if (entity == null)
            {
                if (string.IsNullOrEmpty(id))
                    id = FormulaHelper.CreateGuid();
                entity = new TEntity { };
                PropertyInfo pi = typeof(TEntity).GetProperty("ID");
                pi.SetValue(entity, id, null);

                //设置对象状态为添加
                pi = typeof(TEntity).GetProperty("_state");
                pi.SetValue(entity, EntityStatus.added.ToString(), null);

                // 设置SortIndex 字段的默认值
                SetSortIndexDefaultValue(entity);
                EntityCreateLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity);
                //pi.SetValue(entity, id, null); //ID被UpdateEntity冲掉，重新赋值
                entities.Set<TEntity>().Add(entity);
            }
            else
            {
                PropertyInfo pi = typeof(TEntity).GetProperty("_state");
                //设置对象状态为修改
                pi = typeof(TEntity).GetProperty("_state");
                pi.SetValue(entity, EntityStatus.modified.ToString(), null);

                // 设置SortIndex 字段的默认值
                SetSortIndexDefaultValue(entity);
                EntityModifyLogic<TEntity>(entity);
                UpdateEntity<TEntity>(entity);
            }

            TryValidateModel(entity);

            return entity;
        }

        /// <summary>
        /// 设置SortIndex 字段的默认值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        private void SetSortIndexDefaultValue<TEntity>(TEntity entity) where TEntity : class ,new()
        {
            var pi = typeof(TEntity).GetProperty("SortIndex");
            if (pi != null && pi.GetValue(entity, null) == null)
            {
                if (pi.PropertyType.FullName.IndexOf("Double") >= 0)
                {
                    pi.SetValue(entity, 0.0, null);
                }
                else
                {
                    pi.SetValue(entity, 0, null);
                }
            }
        }

        public virtual TEntity UpdateEntity<TEntity>(TEntity entity) where TEntity : class ,new()
        {
            string json = Request.Form["FormData"];

            string charFilter = System.Configuration.ConfigurationManager.AppSettings["CharFilter"];
            if (!string.IsNullOrEmpty(charFilter))
            {
                foreach (string str in charFilter.Split(','))
                {
                    if (json.Contains(str))
                        throw new BusinessException(string.Format("禁止输入字符串“{0}”", str));
                }
            }

            var formDic = JsonHelper.ToObject<Dictionary<string, object>>(json);
            return UpdateEntity<TEntity>(entity, formDic);
        }

        /// <summary>
        /// 更新字典内容到对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dic"></param>
        /// <param name="entity"></param>
        public virtual TEntity UpdateEntity<TEntity>(TEntity entity, Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                if (key == "ID")
                    continue;

                PropertyInfo pi = typeof(TEntity).GetProperty(key);
                if (pi == null || pi.CanWrite == false)
                    continue;

                if (pi.PropertyType.FullName == "System.String")
                {
                    //为兼容Oracle，不能使用bool型，因此使用char(1)
                    string value = "";
                    if (dic[key] != null)
                        value = dic[key].ToString();
                    if (value == "true")
                        value = "1";
                    else if (value == "false")
                        value = "0";

                    pi.SetValue(entity, value, null);
                }
                else if (dic[key] == null || dic[key].ToString() == "")
                {
                    pi.SetValue(entity, null, null);
                }
                else if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(Nullable<bool>))
                {
                    string value = dic[key].ToString();
                    if (value.ToLower() == "true" || value == "1")
                        pi.SetValue(entity, true, null);
                    else
                        pi.SetValue(entity, false, null);
                }
                else if (pi.PropertyType.IsValueType)
                {
                    Object value = null;
                    Type t = System.Nullable.GetUnderlyingType(pi.PropertyType);
                    if (t == null)
                        t = pi.PropertyType;
                    MethodInfo mis = t.GetMethod("Parse", new Type[] { typeof(string) });
                    try
                    {
                        value = mis.Invoke(null, new object[] { dic[key].ToString() });
                    }
                    catch
                    {
                        throw new Exception(string.Format("数据类型转换失败:将‘{0}’转换为{1}类型时.", dic[key].ToString(), t.Name));
                    }
                    pi.SetValue(entity, value, null);
                }

            }

            return entity;
        }

        #endregion

        #region EntityCreateLogic<TEntity>

        /// <summary>
        /// 实体对象创建逻辑
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected virtual void EntityCreateLogic<TEntity>(TEntity entity)
        {
            UserInfo user = FormulaHelper.GetUserInfo();

            PropertyInfo pi = typeof(TEntity).GetProperty("CreateUserID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserID, null);

            pi = typeof(TEntity).GetProperty("CreateUserName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserName, null);

            pi = typeof(TEntity).GetProperty("CreateUser", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserName, null);

            pi = typeof(TEntity).GetProperty("OrgID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserOrgID, null);

            pi = typeof(TEntity).GetProperty("OrgName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserOrgName, null);

            pi = typeof(TEntity).GetProperty("Org", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserOrgName, null);

            pi = typeof(TEntity).GetProperty("CreateTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, DateTime.Now, null);

            pi = typeof(TEntity).GetProperty("CreateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, DateTime.Now, null);


            //以下为修改逻辑
            pi = typeof(TEntity).GetProperty("ModifyUserID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserID, null);

            pi = typeof(TEntity).GetProperty("ModifyUserName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserName, null);

            pi = typeof(TEntity).GetProperty("ModifyUser", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserName, null);

            pi = typeof(TEntity).GetProperty("ModifyTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, DateTime.Now, null);

            pi = typeof(TEntity).GetProperty("ModifyDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, DateTime.Now, null);
        }

        #endregion

        #region EntityModifyLogic<TEntity>

        /// <summary>
        /// 实体对象修改逻辑
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected virtual void EntityModifyLogic<TEntity>(TEntity entity)
        {
            UserInfo user = FormulaHelper.GetUserInfo();

            PropertyInfo pi = typeof(TEntity).GetProperty("ModifyUserID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserID, null);

            pi = typeof(TEntity).GetProperty("ModifyUserName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserName, null);

            pi = typeof(TEntity).GetProperty("ModifyUser", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, user.UserName, null);

            pi = typeof(TEntity).GetProperty("ModifyTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, DateTime.Now, null);

            pi = typeof(TEntity).GetProperty("ModifyDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi != null)
                pi.SetValue(entity, DateTime.Now, null);
        }

        #endregion

        #region EntitySaveLogic<TEntity>

        /// <summary>
        /// 实体对象保存逻辑
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected virtual void EntitySaveLogic<TEntity>(TEntity entity)
        {
        }

        #endregion

        #endregion

        #region 基类Json方法重载

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            NewtonJsonResult result = new NewtonJsonResult() { Data = data, ContentType = contentType, ContentEncoding = contentEncoding, JsonRequestBehavior = behavior };

            return result;
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            NewtonJsonResult result = new NewtonJsonResult() { Data = data, ContentType = contentType, ContentEncoding = contentEncoding };

            return result;
        }

        #endregion

        #region 如果Action没有定义，则直接查找View文件
        protected override void HandleUnknownAction(string actionName)
        {
            if (Request.HttpMethod == "POST")
                throw new Exception("没有Action:" + actionName);

            // 搜索文件是否存在
            var filePath = "";
            if (RouteData.DataTokens["area"] != null)
                filePath = string.Format("~/Areas/{2}/Views/{1}/{0}.cshtml", actionName, RouteData.Values["controller"], RouteData.DataTokens["area"]);
            else
                filePath = string.Format("~/Views/{1}/{0}.cshtml", actionName, RouteData.Values["controller"]);
            if (System.IO.File.Exists(Server.MapPath(filePath)))
            {
                View(filePath).ExecuteResult(ControllerContext);
            }
            else
            {
                base.HandleUnknownAction(actionName);
            }
        }
        #endregion

        public string GetGuid()
        {
            return FormulaHelper.CreateGuid();
        }
    }

    public abstract class BaseController<T> : BaseController where T : class, new()
    {
        #region 基本Action方法

        public virtual ActionResult List()
        {
            return View();
        }

        public virtual ActionResult Edit()
        {
            return View();
        }

        public virtual ActionResult Tree()
        {
            return View();
        }

        public virtual ActionResult NodeEdit()
        {
            return View();
        }

        #endregion

        #region 基本Json方法

        public virtual JsonResult GetList(QueryBuilder qb)
        {
            return base.JsonGetList<T>(qb);
        }

        public virtual JsonResult GetModel(string id)
        {
            return base.JsonGetModel<T>(id);
        }

        public virtual JsonResult Save()
        {
            return base.JsonSave<T>();
        }

        public virtual JsonResult SaveList()
        {
            return base.JsonSaveList<T>(Request["ListData"]);
        }

        public virtual JsonResult SaveSortedList()
        {
            return base.JsonSaveSortedList<T>(Request["SortedListData"]);
        }

        public virtual JsonResult Delete()
        {
            return base.JsonDelete<T>(Request["ListIDs"]);
        }

        public virtual JsonResult GetTree()
        {
            return base.JsonGetTree<T>(Request["RootFullID"]);
        }

        public virtual JsonResult SaveNode()
        {
            return base.JsonSaveNode<T>();
        }

        public virtual JsonResult DeleteNode()
        {
            return base.JsonDeleteNode<T>(Request["FullID"]);
        }

        public virtual JsonResult DropNode()
        {
            return JsonDropNode<T>();
        }

        #endregion
    }

    public abstract class BaseController<T, N> : BaseController<T>
        where T : class,new()
        where N : class,new()
    {

        #region 基本Action方法

        public ActionResult RelationDataEdit()
        {
            return View();
        }

        #endregion

        public virtual JsonResult GetRelationData()
        {
            return base.JsonGetModel<N>(GetQueryString("ID"));
        }

        public virtual JsonResult GetRelationList(QueryBuilder qb)
        {
            return base.JsonGetRelationList<T, N>(Request["NodeFullID"], qb);
        }

        public virtual JsonResult SaveRelationData()
        {
            return base.JsonSaveRelationData<T, N>();
        }

        public virtual JsonResult DeleteRelationData()
        {
            return base.JsonDeleteRelationData<N>(Request["RelationData"]);
        }
    }

    public abstract class BaseController<T, TN, N> : BaseController<T>
        where T : class,new()
        where TN : class,new()
        where N : class,new()
    {
        #region 基本Action方法
        public ActionResult RelationDataEdit()
        {
            return View();
        }
        #endregion

        public virtual JsonResult GetRelationData()
        {
            return base.JsonGetModel<N>(GetQueryString("ID"));
        }

        public virtual JsonResult GetRelationAll()
        {
            return base.JsonGetRelationAll<T, TN, N>(Request["NodeFullID"]);
        }

        public virtual JsonResult GetRelationList(QueryBuilder qb)
        {
            return base.JsonGetRelationList<T, TN, N>(Request["NodeFullID"], qb);
        }

        public virtual JsonResult SetRelation()
        {
            return base.JsonSetRelation<T, TN, N>(Request["NodeFullID"], Request["RelationData"], Request["FullRelation"]);
        }

        public virtual JsonResult AppendRelation()
        {
            return base.JsonAppendRelation<T, TN, N>(Request["NodeFullID"], Request["RelationData"], Request["FullRelation"]);
        }

        public virtual JsonResult DeleteRelation()
        {
            return base.JsonDeleteRelation<T, TN, N>(Request["NodeFullID"], Request["RelationData"], Request["FullRelation"]);
        }

        public virtual JsonResult SaveRelationData()
        {
            return base.JsonSaveRelationData<T, TN, N>();
        }

        public virtual JsonResult DeleteRelationData()
        {
            return base.JsonDeleteRelationData<N>(Request["RelationData"]);
        }
    }

}
