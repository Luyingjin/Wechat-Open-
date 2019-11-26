using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using System.IO;
using System.Data;
using MvcAdapter.ImportExport;

namespace Formula.ImportExport
{
    /// <summary>
    /// Excel导入数据，采用第三方Aspose组件
    /// </summary>
    public class AsposeExcelImporter : IImporter
    {
        public ExcelData Import(byte[] excelFile, string excelKey)
        {
            return Import(excelFile, GetExcelConfig(excelKey));
        }

        public ExcelData Import(byte[] excelFile, ExcelConfig config)
        {
            // 判断上传的Excel数据文件是否采用了最新的模版
            if (!config.IsValidExcel(excelFile))
            {
                throw new Exception("您上传的文件不能导入，因为不是采用最新的模板，请先下载右上角的最新模板！");
            }

            var data = new ExcelData();
            data.InitConfig(config);
            var workbook = new Workbook(new MemoryStream(excelFile));
            var cells = workbook.Worksheets[0].Cells;
            var comments = workbook.Worksheets[0].Comments;

            // 填充表格数据，需要循环导入的数据
            foreach (var table in data.Tables)
            {
                var startRowIndex = table.Structure.StartRowIndex;
                // 循环行
                for (int i = startRowIndex; i <= cells.MaxDataRow + 1; i++)
                {
                    // 判断是否结束
                    if (IsEndRow()) break;

                    var row = new ExcelRowInfo(i);
                    // 循环列
                    foreach (var cell in table.Structure.Cells)
                    {
                        // 获取导入单元格的值
                        var value = cells[i, cell.ColIndex].StringValue.Trim();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            var cellInfo = new ExcelCellInfo(cell);
                            cellInfo.Value = value;
                            row.Cells.Add(cellInfo);
                        }
                    }
                    if (row.Cells.Count > 0)
                        table.Rows.Add(row);
                }
            }

            // 填充一次导入的数据
            foreach (var cell in data.Variables)
            {
                var value = cells[cell.Structure.RowIndex, cell.Structure.ColIndex].StringValue.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    cell.Value = value;
                }
            }

            return data;
        }

        /// <summary>
        /// 判断是否为最后一行
        /// </summary>
        /// <returns></returns>
        private bool IsEndRow()
        {
            return false;
        }

        public string GetExcelTemplateUrl(string excelKey)
        {
            throw new NotImplementedException();
        }

        public byte[] GetExcelTemplate(string excelkey)
        {
            IExcelMetadataStorage storage = new DefaultExcelMetadataStorage();
            var metadata = storage.GetMetadataByKey(excelkey);
            IExporter exporter = new AsposeExcelExporter();
            var dt = new DataTable();
            var buffer = exporter.Export(dt, metadata.FileBuffer);
            return buffer;
        }

        public ExcelConfig GetExcelConfig(string excelkey)
        {
            var config = new ExcelConfig(excelkey);
            Workbook workbook = new Workbook(new MemoryStream(config.Metadata.FileBuffer));
            var cells = workbook.Worksheets[0].Cells;
            var comments = workbook.Worksheets[0].Comments;

            // 循环行
            for (int i = 0; i < cells.MaxDataRow + 1; i++)
            {
                // 循环列
                for (int j = 0; j < cells.MaxDataColumn + 1; j++)
                {
                    string s = cells[i, j].StringValue.Trim();

                    var tableName = ParseTableName(s);
                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        var table = config.Tables.FirstOrDefault(t => t.TableName == tableName);
                        if (table == null)
                        {
                            table = new TableConfig
                            {
                                TableName = tableName,
                                StartColIndex = j,
                                StartRowIndex = i,
                            };
                            config.Tables.Add(table);
                        }

                        var note = string.Empty;
                        var comment = comments[i, j];
                        if (comment != null)
                        {
                            note = comment.Note;

                        }

                        var cellConfig = GetCellInfo(s, i, j, note);
                        table.Cells.Add(cellConfig);
                    }
                    else if (IsExp(s))
                    {
                        var note = string.Empty;
                        var comment = comments[i, j];
                        if (comment != null)
                        {
                            note = comment.Note;

                        }

                        var cellConfig = GetCellInfo(s, i, j, note);

                        config.Variables.Add(cellConfig);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// 从表达式中解析字段名称
        /// </summary>
        /// <returns></returns>
        private string ParseFieldName(string exp)
        {
            if (IsExp(exp))
            {
                if (exp.Contains('.'))
                {
                    var arr = exp.Replace("&=", "").Split('.');
                    if (arr.Length == 2)
                    {
                        return arr[1].Trim("[]".ToCharArray());
                    }
                }
                else
                {
                    return exp.Replace("&=", "");
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 从表达式中解析表名称
        /// </summary>
        /// <returns></returns>
        private string ParseTableName(string exp)
        {
            if (IsExp(exp))
            {
                if (exp.Contains('.'))
                {
                    var arr = exp.Replace("&=", "").Split('.');
                    if (arr.Length == 2)
                    {
                        return arr[0].Trim("[]".ToCharArray());
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 判断是否为绑定表达式，判断依据为：以&=开头的，但是不能以&=&=开头（这是公式）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsExp(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.StartsWith("&=") && !value.StartsWith("&=&=");
        }

        /// <summary>
        /// 获取单元格的信息
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private CellConfig GetCellInfo(string value, int row, int column, string CellComment)
        {
            CellConfig cellConfig = null;
            if (IsExp(value))
            {
                cellConfig = new CellConfig
                {
                    ColIndex = column,
                    RowIndex = row,
                    FieldName = ParseFieldName(value),
                };
            }
            return cellConfig;
        }
    }
}
