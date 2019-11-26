using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcAdapter
{
    /// <summary>
    /// 配合minigrid的json数据结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridData
    {
        public GridData(object list)
        {
            data = list;
            sumData = new Dictionary<string, object>();
            avgData = new Dictionary<string, object>();
        }
        /// <summary>
        /// 总条数.
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// Grid表单数据.
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 汇总数据集合
        /// </summary>
        public Dictionary<string, object> sumData { get; set; }

        /// <summary>
        /// 平均值数据集合
        /// </summary>
        public Dictionary<string, object> avgData { get; set; }
    }
}
