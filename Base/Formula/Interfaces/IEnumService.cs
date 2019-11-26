using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using System.Data;

namespace Formula
{
    /// <summary>
    /// 枚举服务接口，用于获取下拉框的数据源
    /// </summary>
    public interface IEnumService : ISingleton
    {
        string GetEnumJson(string enumKey, string category = "", string subCategory = "");

        DataRow GetEnumDefRow(string enumKey);


        DataTable GetEnumTable(string enumKey, string category = "", string subCategory = "");
        

        /// <summary>
        /// 获取指定枚举Key的枚举数据集合
        /// </summary>
        /// <param name="enumKey">枚举Key</param>
        /// <param name="category">分类</param>
        /// <param name="subCategory">子分类</param>
        /// <returns>枚举数据集合</returns>
        IList<DicItem> GetEnumDataSource(string enumKey, string category = "", string subCategory = "");

        /// <summary>
        /// 获取指定枚举项值的枚举项文本
        /// </summary>
        /// <param name="enumKey">枚举Key</param>
        /// <param name="value">枚举项值</param>
        /// <returns></returns>
        string GetEnumText(string enumKey, string value);

        /// <summary>
        /// 获取指定枚举项文本的枚举项值
        /// </summary>
        /// <param name="enumKey">枚举Key</param>
        /// <param name="text">枚举项文本</param>
        /// <returns></returns>
        string GetEnumValue(string enumKey, string text);
    }

}
