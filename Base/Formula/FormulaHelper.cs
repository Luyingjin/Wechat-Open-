using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using System.Data;
using Formula.Helper;
using Config;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;

namespace Formula
{
    public sealed class FormulaHelper
    {
        #region 生成ID

        public static string CreateGuid(int index)
        {
            //CAST(CAST(NEWID() AS BINARY(10)) + CAST(GETDATE() AS BINARY(6)) AS UNIQUEIDENTIFIER)
            byte[] guidArray = Guid.NewGuid().ToByteArray();
            DateTime now = DateTime.Now;

            DateTime baseDate = new DateTime(1900, 1, 1);

            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);

            TimeSpan msecs = new TimeSpan(now.Ticks - (new DateTime(now.Year, now.Month, now.Day).Ticks));
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)((msecs.TotalMilliseconds + index * 10) / 3.333333));

            Array.Copy(daysArray, 0, guidArray, 2, 2);
            //毫秒高位
            Array.Copy(msecsArray, 2, guidArray, 0, 2);
            //毫秒低位
            Array.Copy(msecsArray, 0, guidArray, 4, 2);
            return new System.Guid(guidArray).ToString();
        }

        /// <summary>
        /// 创建一个按时间排序的Guid
        /// </summary>
        /// <returns></returns>
        public static string CreateGuid()
        {
            //CAST(CAST(NEWID() AS BINARY(10)) + CAST(GETDATE() AS BINARY(6)) AS UNIQUEIDENTIFIER)
            byte[] guidArray = Guid.NewGuid().ToByteArray();
            DateTime now = DateTime.Now;

            DateTime baseDate = new DateTime(1900, 1, 1);

            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);

            TimeSpan msecs = new TimeSpan(now.Ticks - (new DateTime(now.Year, now.Month, now.Day).Ticks));
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            Array.Copy(daysArray, 0, guidArray, 2, 2);
            //毫秒高位
            Array.Copy(msecsArray, 2, guidArray, 0, 2);
            //毫秒低位
            Array.Copy(msecsArray, 0, guidArray, 4, 2);
            return new System.Guid(guidArray).ToString();
        }

        public static DateTime GetDateTimeFromGuid(string strGuid)
        {
            Guid guid = Guid.Parse(strGuid);

            DateTime baseDate = new DateTime(1900, 1, 1);
            byte[] daysArray = new byte[4];
            byte[] msecsArray = new byte[4];
            byte[] guidArray = guid.ToByteArray();

            // Copy the date parts of the guid to the respective byte arrays. 
            Array.Copy(guidArray, guidArray.Length - 6, daysArray, 2, 2);
            Array.Copy(guidArray, guidArray.Length - 4, msecsArray, 0, 4);

            // Reverse the arrays to put them into the appropriate order 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Convert the bytes to ints 
            int days = BitConverter.ToInt32(daysArray, 0);
            int msecs = BitConverter.ToInt32(msecsArray, 0);

            DateTime date = baseDate.AddDays(days);
            date = date.AddMilliseconds(msecs * 3.333333);

            return date;
        }

        #endregion

        #region 当前用户

        public static string UserID
        {
            get
            {
                return GetUserInfo().UserID;
            }
        }

        public static UserInfo GetUserInfo()
        {
            var userService = FormulaHelper.GetService<IUserService>();

            string systemName = userService.GetCurrentUserLoginName();
            if (string.IsNullOrEmpty(systemName))
                return new UserInfo(); //throw new TimeoutException("用户登录超时！"); // 匿名访问太多，导致报错没法开发

            string key = "UserInfo_" + systemName;
            UserInfo user = (UserInfo)CacheHelper.Get(key);
            if (user == null)
            {
                user = userService.GetUserInfoBySysName(systemName);
                if (user != null)
                    CacheHelper.Set(key, user);
            }
            return user;
        }

        public static UserInfo GetUserInfoByID(string userID)
        {
            return FormulaHelper.GetService<IUserService>().GetUserInfoByID(userID);
        }

        public static UserInfo GetUserInfoBySysName(string loginName)
        {
            return FormulaHelper.GetService<IUserService>().GetUserInfoBySysName(loginName);
        }

        #endregion

        #region 实体库

        public static void DisposeEntities()
        {
            foreach (var item in HttpContext.Current.Items)
            {
                if (item.GetType().BaseType == typeof(DbContext))
                    ((DbContext)item).Dispose();
            }
        }

        private static Dictionary<string, string> _entitiesDic = new Dictionary<string, string>();
        public static void RegistEntities<T>(ConnEnum conn)
        {
            _entitiesDic[typeof(T).FullName] = conn.ToString();
        }

        public static void RegistEntities<T>(string conn)
        {
            _entitiesDic[typeof(T).FullName] = conn;
        }


        public static T GetEntities<T>() where T : DbContext, new()
        {
            string key = typeof(T).FullName;

            if (HttpContext.Current == null)
            {
                var objectEntity = AppDomain.CurrentDomain.GetData(key);
                if (objectEntity != null) return (T)objectEntity;
                else
                {
                    string connName = ""; string entitiesName = key.Split('.').Last();
                    foreach (ConnectionStringSettings item in System.Configuration.ConfigurationManager.ConnectionStrings)
                    {
                        if (entitiesName.StartsWith(item.Name))
                        {
                            connName = item.Name;
                            break;
                        }
                    }
                    ConstructorInfo constructor = typeof(T).GetConstructor(new Type[] { typeof(string) });
                    T entities = (T)constructor.Invoke(new object[] { connName });
                    AppDomain.CurrentDomain.SetData(key, entities);
                    return entities;
                }
            }
            else if (HttpContext.Current.Items.Contains(key))
                return (T)HttpContext.Current.Items[key];
            else
            {
                string connName = "";// _entitiesDic[key];
                if (_entitiesDic.ContainsKey(key))
                    connName = _entitiesDic[key];
                else
                {
                    string entitiesName = key.Split('.').Last();
                    foreach (ConnectionStringSettings item in System.Configuration.ConfigurationManager.ConnectionStrings)
                    {
                        if (entitiesName.StartsWith(item.Name))
                        {
                            if (item.Name.Length > connName.Length)
                                connName = item.Name;
                        }
                    }
                }

                if (string.IsNullOrEmpty(connName))
                    throw new Exception(string.Format("配置文件中不包含{0}的链接字符串", key));

                ConstructorInfo constructor = typeof(T).GetConstructor(new Type[] { typeof(string) });
                T entities = (T)constructor.Invoke(new object[] { connName });

                HttpContext.Current.Items[key] = entities;
                return (T)HttpContext.Current.Items[key];
            }
        }


        #endregion

        #region 实体转化

        /// <summary>
        /// 对象列表转化为字典列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> CollectionToListDic<T>(ICollection<T> list)
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

            foreach (var item in list)
            {
                resultList.Add(ModelToDic<T>(item));
            }

            return resultList;
        }

        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ModelToDic<T>(T obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            PropertyInfo[] arrPtys = typeof(T).GetProperties();
            foreach (PropertyInfo destPty in arrPtys)
            {
                if (destPty.CanRead == false)
                    continue;
                //if (destPty.PropertyType.Name == "ICollection`1")
                //    continue;
                //if ((destPty.PropertyType.IsClass && destPty.PropertyType.Name != "String") || destPty.PropertyType.IsArray || destPty.PropertyType.IsInterface)
                //    continue;
                object value = destPty.GetValue(obj, null);
                dic.Add(destPty.Name, value);
            }
            return dic;
        }

        public static void UpdateModel(object dest, object src)
        {
            PropertyInfo[] destPtys = dest.GetType().GetProperties();
            PropertyInfo[] srcPtys = src.GetType().GetProperties();

            foreach (PropertyInfo destPty in destPtys)
            {
                if (destPty.CanRead == false)
                    continue;
                if (destPty.PropertyType.Name == "ICollection`1")
                    continue;
                if ((destPty.PropertyType.IsClass && destPty.PropertyType.Name != "String") || destPty.PropertyType.IsArray || destPty.PropertyType.IsInterface)
                    continue;

                PropertyInfo srcPty = srcPtys.Where(c => c.Name == destPty.Name).SingleOrDefault();
                if (srcPty == null)
                    continue;
                if (srcPty.CanWrite == false)
                    continue;

                object value = srcPty.GetValue(src, null);

                destPty.SetValue(dest, value, null);
            }
        }

        #endregion

        #region 创建FO对象

        public static T CreateFO<T>() where T : class,new()
        {
            #region 不使用容器，直接生成
            //if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            //    return Intercept.NewInstance<T>(new VirtualMethodInterceptor(), new IInterceptionBehavior[] { new TransactionBehavior() });
            //else
            //    return Intercept.NewInstance<T>(new VirtualMethodInterceptor(), new IInterceptionBehavior[] { });
            #endregion

            return new T();
        }

        #endregion

        #region  remark_AOP

        //#region CreateObject
        ///// <summary>
        ///// 创建一个已经在EntLibFormula注册类型的逻辑应用层服务对象
        ///// </summary>
        ///// <typeparam name="T">为接口类型</typeparam>
        ///// <returns></returns>
        //public static T CreateObject<T>()
        //{
        //    return Container.Resolve<T>();
        //}

        //#endregion

        //#region BuilderUp
        ///// <summary>
        ///// 对类实例进行对象注入
        ///// </summary>
        ///// <param name="T"></param>
        ///// <param name="existing"></param>
        //public static void BuilderUp(Type T, object existing)
        //{
        //    Container.BuildUp(T, existing);
        //}

        //#endregion

        //#region RegisterType

        ///// <summary>
        ///// 在EntLibFormula框架注册逻辑应用层业务类型
        ///// </summary>
        ///// <param name="T"></param>
        //public static void RegisterType(Type T)
        //{
        //    Container.RegisterType(T);                    
        //    Container.Configure<Interception>().SetInterceptorFor(T, new VirtualMethodInterceptor());
        //}

        //#endregion 

        //#region 容器

        //private static IUnityContainer _container = null;
        //private static object lockHelper = new object();
        //private static IUnityContainer Container
        //{
        //    get
        //    {
        //        if (_container == null)
        //        {
        //            lock (lockHelper)
        //            {
        //                if (_container == null)
        //                {
        //                    _container = new UnityContainer();
        //                    _container.AddNewExtension<Interception>();

        //                }
        //            }
        //        }
        //        return _container;
        //    }
        //}

        //#endregion

        #endregion

        #region 获取单例化服务
        /// <summary>
        /// 获取服务（服务是单例的）
        /// </summary>
        /// <typeparam name="T">服务接口</typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if (_DicSingletonSerivces.ContainsKey(typeof(T)))
            {
                return (T)Activator.CreateInstance(_DicSingletonSerivces[typeof(T)]);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T1">接口</typeparam>
        /// <typeparam name="T2">实现</typeparam>
        public static void RegisterService<T1, T2>()
            where T2 : T1
        //where T1 : ISingleton
        {
            _DicSingletonSerivces[typeof(T1)] = typeof(T2);
        }
        private static Dictionary<Type, Type> _DicSingletonSerivces = new Dictionary<Type, Type>();
        #endregion

        #region 创建权限数据过滤

        public static SearchCondition CreateAuthDataFilter()
        {
            UserInfo user = FormulaHelper.GetUserInfo();

            SearchCondition cnd = new SearchCondition();
            cnd.IsOrRelateion = true;

            if (HttpContext.Current.Request.UrlReferrer == null)
                return cnd;

            string url = HttpContext.Current.Request.UrlReferrer.PathAndQuery;

            if (url.StartsWith("/portal", StringComparison.CurrentCultureIgnoreCase)) //在ActionResult中进行权限过滤
                url = HttpContext.Current.Request.Url.PathAndQuery;


            //没有定义数据权限，则不过滤数据
            if (GetService<IResService>().GetRes(url, "Data").Count() == 0)
                return cnd;

            //当前用户的数据级权限          
            var resList = GetService<IResService>().GetRes(url, "Data", user.UserID);
            resList = resList.Where(c => c.Type == "Data").ToList();//三权分离的管理员会无条件拥有"系统管理菜单权限"，因此需要重新过滤Data

            if (resList.Where(c => c.DataFilter == "All").Count() > 0)
            {
                return cnd;
            }
            else
            {
                if (Config.Constant.IsOracleDb)
                {
                    foreach (var item in resList)
                    {
                        switch (item.DataFilter)
                        {
                            case "OrgID":
                                cnd.Add("ORGID", QueryMethod.Equal, user.UserOrgID);
                                break;
                            case "PrjID":
                                cnd.Add("PRJID", QueryMethod.Equal, user.UserPrjID);
                                break;
                            case "CreateUserID":
                                cnd.Add("CREATEUSERID", QueryMethod.Equal, user.UserID);
                                break;
                            default:
                                dealOtherDataAuth(cnd, item, user);
                                break;
                        }

                    }
                }
                else
                {
                    foreach (var item in resList)
                    {
                        switch (item.DataFilter)
                        {
                            case "OrgID":
                                cnd.Add("OrgID", QueryMethod.Equal, user.UserOrgID);
                                break;
                            case "PrjID":
                                cnd.Add("PrjID", QueryMethod.Equal, user.UserPrjID);
                                break;
                            case "CreateUserID":
                                cnd.Add("CreateUserID", QueryMethod.Equal, user.UserID);
                                break;
                            default:
                                dealOtherDataAuth(cnd, item, user);
                                break;
                        }

                    }
                }
            }

            if (cnd.Items.Count == 0)
                cnd.Add("ID", QueryMethod.Equal, "");

            return cnd;
        }

        private static void dealOtherDataAuth(SearchCondition cnd, Res res, UserInfo user)
        {
            string dataFilter = res.DataFilter;


            var arr = JsonHelper.ToObject<List<Dictionary<string, string>>>(dataFilter);
            foreach (var a in arr)
            {
                string fieldName = a["FieldName"];
                string queryMode = a["QueryMode"];
                string value = a["Value"];
                string orGroup = "";
                if (a.ContainsKey("OrGroup") && !string.IsNullOrEmpty(a["OrGroup"]))
                    orGroup = a["OrGroup"];


                #region 替换value中的变量
                Regex reg = new Regex("\\{[0-9a-zA-Z_]*\\}");
                value = reg.Replace(value, (Match m) =>
                {
                    string v = m.Value.Trim('{', '}');

                    if (!string.IsNullOrEmpty(HttpContext.Current.Request[v]))
                        return HttpContext.Current.Request[v];
                    switch (v)
                    {
                        case Formula.Constant.CurrentUserID:
                            return user.UserID;
                        case Formula.Constant.CurrentUserName:
                            return user.UserName;
                        case Formula.Constant.CurrentUserOrgID:
                            return user.UserOrgID;
                        case Formula.Constant.CurrentUserOrgIDs:
                            return user.UserOrgIDs;
                        case Formula.Constant.CurrentUserOrgName:
                            return user.UserOrgName;
                        case Formula.Constant.CurrentUserPrjID:
                            return user.UserPrjID;
                        case Formula.Constant.CurrentUserPrjName:
                            return user.UserPrjName;
                        case "CurrentTime":
                            return DateTime.Now.ToString();
                        default:
                            return m.Value;
                    }
                });
                #endregion

                QueryMethod q = (QueryMethod)Enum.Parse(typeof(QueryMethod), queryMode);
                cnd.Add(fieldName, q, value, orGroup, res.ID);
            }
        }

        #endregion

        #region 设置和获取对象属性

        public static void SetProperty(object obj, string ptyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(ptyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
                pi.SetValue(obj, value, null);
            else
                throw new Exception(string.Format("属性‘{0}’不存在", ptyName));
        }

        public static object GetProperty(object obj, string ptyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty(ptyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
                return pi.GetValue(obj, null);
            else
                throw new Exception(string.Format("属性‘{0}’不存在", ptyName));
        }

        public static bool HasProperty(object obj, string ptyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty(ptyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
                return true;
            else
                return false;
        }

        #endregion

        #region 判断是否有权限

        public static bool HasAuth(string resCode, string userID)
        {
            var user = FormulaHelper.GetUserInfoByID(userID);

            var resService = FormulaHelper.GetService<IResService>();

            if (resService.GetRes(resCode, "Code").Count() > 0)
            {
                if (resService.GetRes(resCode, "Code", userID).Count() == 0)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        #endregion

        #region 统一的上下文操作

        private static Dictionary<string, object> _AppContext = new Dictionary<string, object>();

        public static string ContextGetValueString(string key)
        {
            object obj = ContextGetValue(key);
            if (obj == null)
                return "";
            else
                return obj.ToString();
        }
        public static object ContextGetValue(string key)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(key))
                {
                    return HttpContext.Current.Items[key];
                }
                else if (string.IsNullOrEmpty(HttpContext.Current.Request[key]))
                {
                    return HttpContext.Current.Request[key];
                }
                else
                {
                    return null;
                }
            }
            else if (_AppContext.ContainsKey(key))
            {
                return _AppContext[key];
            }
            else
            {
                return null;
            }
        }

        public static void ContextSet(string key, object value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[key] = value;
            }
            else
            {
                _AppContext[key] = value;
            }
        }

        public static void ContextRemoveByKey(string key)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(key))
                    HttpContext.Current.Items.Remove(key);
            }
            else
            {
                if (_AppContext.ContainsKey(key))
                    _AppContext.Remove(key);
            }
        }

        public static bool ContextContainsKey(string key)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(key))
                    return true;
                if (HttpContext.Current.Request.QueryString.AllKeys.Contains(key))
                    return true;
            }
            else
            {
                if (_AppContext.ContainsKey(key))
                    return true;
            }

            return false;
        }

        #endregion

        #region 当前流程环节

        public static string FlowCurrentStepCode
        {
            get
            {
                return Formula.FormulaHelper.GetService<Formula.IWorkflowService>().GetFlowCurrentStepCode();
            }
        }

        #endregion

    }
}
