using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Formula.ImportExport
{
    /// <summary>
    /// Excel导入的数据
    /// </summary>
    public class ExcelData
    {
        /// <summary>
        /// 初始化配置信息
        /// </summary>
        /// <param name="config"></param>
        public void InitConfig(ExcelConfig config)
        {
            Variables = new List<ExcelCellInfo>();
            Tables = new List<ExcelTableInfo>();

            foreach (var cellConfig in config.Variables)
            {
                Variables.Add(new ExcelCellInfo(cellConfig));
            }

            foreach (var tableConfig in config.Tables)
            {
                Tables.Add(new ExcelTableInfo(tableConfig));
            }
        }

        /// <summary>
        /// 变量数据
        /// </summary>
        public IList<ExcelCellInfo> Variables { get; set; }

        /// <summary>
        /// 表格数据
        /// </summary>
        public IList<ExcelTableInfo> Tables { get; set; }

        /// <summary>
        /// 获取一个数据表
        /// </summary>
        /// <param name="tableName">表名称，如果没有设置，则为第一个表格的数据</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName = "")
        {
            DataTable data = null;
            if (Tables.Count > 0)
            {
                // 构建表结构
                var table = Tables[0];
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    table = Tables.FirstOrDefault(t => t.TableName == tableName);
                }

                if (table != null)
                {
                    data = table.ToDataTable();
                }
            }

            return data;
        }
    }
}
