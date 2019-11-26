using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Config;
using Formula.Exceptions;
using System.Configuration;
using System.ComponentModel;

namespace Formula.Helper
{
    public class EnumBaseHelper
    {
        public static DataTable GetEnumTable(Type emType)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("text");
            dt.Columns.Add("value");
            foreach (var item in emType.GetFields())
            {
                if (item.FieldType.IsEnum)
                {
                    object[] arr = item.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    DataRow row = dt.NewRow();
                    row["text"] = arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : item.Name;
                    row["value"] = item.Name;

                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public static DataTable GetEnumTable(string enumKey, string category = "", string subCategory = "")
        {
            //数据库连接枚举
            return FormulaHelper.GetService<IEnumService>().GetEnumTable(enumKey, category, subCategory);
        }

        /// <summary>
        /// 获取枚举的description标签内容.
        /// </summary>
        /// <param name="enumType">类型</param>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumDescription(Type enumType, string enumValue)
        {
            System.Reflection.FieldInfo finfo = enumType.GetField(enumValue);
            object[] enumAttr = finfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
            if (enumAttr.Length > 0)
            {
                System.ComponentModel.DescriptionAttribute desc = enumAttr[0] as System.ComponentModel.DescriptionAttribute;
                if (desc != null)
                {
                    return desc.Description;
                }
            }
            return enumValue;
        }

        #region 私有方法

        #endregion

        #region EnumDefInfo

        public static EnumDefInfo GetEnumDef(Type emType)
        {
            string key = string.Format("enum_{0}", emType.Name); ;

            //从缓存中获取
            object obj = CacheHelper.Get(key);
            if (obj != null)
                return obj as EnumDefInfo;


            object[] arr = emType.GetCustomAttributes(typeof(DescriptionAttribute), true);
            string description = arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : emType.Name;

            EnumDefInfo enumDef = new EnumDefInfo() { Code = emType.Name, Name = emType.Name, Description = description };

            DataTable dt = GetEnumTable(emType);
            enumDef.EnumItem = GetEnumItemSet(dt);

            CacheHelper.Set(key, enumDef);
            return enumDef;
        }

        public static EnumDefInfo GetEnumDef(string enumKey, string category = "", string subCategory = "")
        {
            string key = enumKey;

            //从缓存中获取
            object obj = CacheHelper.Get(key);
            if (obj != null)
                return obj as EnumDefInfo;

            DataRow enumDef = FormulaHelper.GetService<IEnumService>().GetEnumDefRow(enumKey);

            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            EnumDefInfo result = new EnumDefInfo()
            {
                ID = enumDef["ID"].ToString(),
                Code = enumDef["Code"].ToString(),
                Name = enumDef["Name"].ToString(),
                Description = enumDef["Description"].ToString()
            };
            DataTable dt = GetEnumTable(key, category, subCategory);

            result.EnumItem = GetEnumItemSet(dt);
            //缓存
            CacheHelper.Set(key, result);


            return result;
        }


        private static HashSet<EnumItemInfo> GetEnumItemSet(DataTable dt)
        {
            HashSet<EnumItemInfo> result = new HashSet<EnumItemInfo>();

            foreach (DataRow row in dt.Rows)
            {
                EnumItemInfo item = new EnumItemInfo();
                foreach (var pi in typeof(EnumItemInfo).GetProperties())
                {
                    if (pi.Name == "Code" && row["value"] != System.DBNull.Value)
                    {
                        pi.SetValue(item, row["value"], null);
                        continue;
                    }
                    if (pi.Name == "Name" && row["text"] != System.DBNull.Value)
                    {
                        pi.SetValue(item, row["text"], null);
                        continue;
                    }

                    if (!row.Table.Columns.Contains(pi.Name))
                        continue;

                    if (row[pi.Name] != System.DBNull.Value)
                        pi.SetValue(item, row[pi.Name], null);
                }

                result.Add(item);
            }

            return result;
        }

        #endregion

    }


    public partial class EnumDefInfo
    {
        public EnumDefInfo()
        {
            this.EnumItem = new HashSet<EnumItemInfo>();
        }

        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<EnumItemInfo> EnumItem { get; set; }

    }

    public partial class EnumItemInfo
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SubCategory { get; set; }
        public string Category { get; set; }
        public Nullable<double> SortIndex { get; set; }
        public string Description { get; set; }

    }
}
