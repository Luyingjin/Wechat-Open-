using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// 单元格的配置信息
    /// </summary>
    public class CellConfig
    {
        /// <summary>
        /// 行索引值
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列索引值
        /// </summary>
        public int ColIndex { get; set; }

        /// <summary>
        /// 对应的字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 枚举Key
        /// </summary>
        public string EnumKey { get; set; }
    }
}
