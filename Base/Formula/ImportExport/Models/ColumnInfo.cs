using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// 模板中的列信息
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 中文名称（只在导出的时候使用）
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段所属表的名称（只在导出的时候使用）
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 时间格式字段的Format
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// 字段所关联的枚举Key
        /// </summary>
        public string EnumKey { get; set; }

        /// <summary>
        /// 字段所关联的枚举数据源
        /// </summary>
        public List<DicItem> EnumDataSource { get; set; }
    }

    /// <summary>
    /// 枚举项
    /// </summary>
    public class DicItem
    {
        /// <summary>
        /// 枚举项值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 枚举文本
        /// </summary>
        public string Text { get; set; }
    }
}
