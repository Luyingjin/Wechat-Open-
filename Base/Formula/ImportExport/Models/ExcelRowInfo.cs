using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// 数据行的相关数据，包含本行所有列的数据
    /// </summary>
    public class ExcelRowInfo
    {
        public ExcelRowInfo(int rowIndex)
        {
            Cells = new List<ExcelCellInfo>();
            this.RowIndex = rowIndex;
        }

        /// <summary>
        /// 行的索引值
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 本行所有单元格的数据
        /// </summary>
        public IList<ExcelCellInfo> Cells { get; set; }

        /// <summary>
        /// 获取指定字段的值
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>值</returns>
        public string this[string fieldName]
        {
            get
            {
                var cell = Cells.FirstOrDefault(c => c.FieldName.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
                if (cell != null)
                {
                    return cell.Value;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
