using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Formula.ImportExport
{
    public class CellValidationArgs
    {
        public CellValidationArgs()
        {
            this.IsValid = true;
        }

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
        /// 字段值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 枚举Key
        /// </summary>
        public string EnumKey { get; set; }

        /// <summary>
        ///  是否验证通过
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 错误提示文本
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// 所在数据行
        /// </summary>
        public DataRow Record { get; set; }

    }
}
