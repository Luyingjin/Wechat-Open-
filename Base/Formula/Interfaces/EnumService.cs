using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Formula.Helper;
using Config;

namespace Formula
{
    public class EnumService : IEnumService
    {

        public DataRow GetEnumDefRow(string enumKey)
        {
            return Config.Logic.EnumService.GetEnumDefRow(enumKey);
        }

        public DataTable GetEnumTable(string enumKey, string category = "", string subCategory = "")
        {
            return Config.Logic.EnumService.GetEnumTable(enumKey, category, subCategory);
        }

        public string GetEnumJson(string enumKey, string category = "", string subCategory = "")
        {
            string key = string.Format("EnumJson_{0}_{1}_{2}", enumKey, category, subCategory);
            return (string)CacheHelper.Get(key, () =>
            {
                var dt = Config.Logic.EnumService.GetEnumTable(enumKey, category, subCategory);
                return JsonHelper.ToJson(dt);
            });
        }

        public virtual IList<DicItem> GetEnumDataSource(string enumKey, string category = "", string subCategory = "")
        {
            string key = string.Format("Enum_{0}_{1}_{2}", enumKey, category, subCategory);
            return (IList<DicItem>)CacheHelper.Get(key, () =>
            {
                return Config.Logic.EnumService.GetEnumDataSource(enumKey, category, subCategory);
            });
        }

        public virtual string GetEnumText(string enumKey, string value)
        {

            return (string)CacheHelper.Get("Enum_" + enumKey + "_" + value, () =>
            {
                return Config.Logic.EnumService.GetEnumText(enumKey, value);
            });
        }

        public virtual string GetEnumValue(string enumKey, string text)
        {
            return (string)CacheHelper.Get("Enum_" + enumKey + "_" + text, () =>
            {
                return Config.Logic.EnumService.GetEnumValue(enumKey, text);
            });
        }
    }
}
