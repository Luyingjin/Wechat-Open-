using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Formula.ImportExport
{
    /// <summary>
    /// 数据表的相关数据，由数据行组成
    /// </summary>
    public class ExcelTableInfo
    {
        public ExcelTableInfo(TableConfig config)
        {
            this.Structure = config;
            Rows = new List<ExcelRowInfo>();
        }

        public string TableName
        {
            get
            {
                if (Structure == null)
                    throw new Exception("单元格的配置信息Structure 不能为NULL！");

                return Structure.TableName;
            }
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public TableConfig Structure { get; set; }

        /// <summary>
        /// 所有行的数据
        /// </summary>
        public IList<ExcelRowInfo> Rows { get; set; }

        /// <summary>
        /// 转换为DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable data = new DataTable();

            // 构建表结构
            foreach (var col in Structure.Cells)
            {
                data.Columns.Add(col.FieldName);
            }

            // 填充数据
            foreach (var row in Rows)
            {
                var newRow = data.NewRow();
                foreach (var cell in row.Cells)
                {
                    newRow[cell.FieldName] = cell.Value;
                }
                data.Rows.Add(newRow);
            }

            return data;
        }
    }
}
