using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Aspose.Cells;
using System.IO;
using Formula.ImportExport;

namespace MvcAdapter.ImportExport
{
    /// <summary>
    /// Excel导出数据，采用第三方Aspose组件
    /// </summary>
    /// <remarks>
    /// 模板的批注规则：【枚举】、【内容】
    ///     如果是枚举的单元格
    ///         【枚举】Project.Formulate
    ///         【内容】简易□    纸质□    胶装□   加厚□                  普装□    简精□    精装□   其他□
    ///             
    ///     如果是需要格式化的单元格
    ///         【内容】  说明：  {0}   页
    ///         
    ///     如果是日期单元格
    ///         设置单元格的日期格式（Excel自带功能）
    ///         
    /// 模板的导入字段格式为：&=$ProjectName，前后都不能有空格或其他任意字符
    /// </remarks>
    public class AsposeExcelExporter : IExporter
    {
        public byte[] Export(IDictionary<string, object> dicVariable, byte[] templateBuffer)
        {
            return Export(dicVariable, null, templateBuffer);
        }

        public byte[] Export(DataTable dt, byte[] templateBuffer)
        {
            var ds = new DataSet();
            if (dt != null)
            {
                ds.Tables.Add(dt);
            }
            return Export(null, ds, templateBuffer);
        }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="templateBuffer"></param>
        /// <returns></returns>
        public byte[] Export(IDictionary<string, object> dicVariable, DataSet ds, byte[] templateBuffer)
        {
            // 创建一个workbookdesigner对象
            WorkbookDesigner designer = new WorkbookDesigner();

            // 设置报表模板
            designer.Workbook = new Workbook(new MemoryStream(templateBuffer));

            // 设置DataSet对象，可以是多张表
            if (ds != null)
                designer.SetDataSource(ParseDataSource(ds));

            //设置变量对象
            if (dicVariable != null)
            {
                foreach (var key in dicVariable.Keys)
                {
                    designer.SetDataSource(key, dicVariable[key]);
                }
            }

            // 根据数据源处理生成报表内容
            designer.Process();

            // 替换书签，用于处理格式化内容的情况，如：共{0}页
            var worksheet = designer.Workbook.Worksheets[0];
            Cells cells = worksheet.Cells;
            var comments = worksheet.Comments;
            foreach (Comment c in comments)
            {
                var cellValue = cells[c.Row, c.Column].StringValue.Trim();
                var dic = ParseNote(c.Note);
                foreach (var keywords in dic.Keys)
                {
                    // 1. 处理枚举
                    if (keywords == Constant.TEMPLATE_KEYWORDS_ENUM)
                    {
                        var enumKey = dic[Constant.TEMPLATE_KEYWORDS_ENUM].Trim();
                        if (enumKey != "NULL")
                        {
                            // 考虑多选枚举的情况
                            if (cellValue.IndexOf(",") >= 0 && !string.IsNullOrWhiteSpace(cellValue))
                            {
                                var enumValues = cellValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                var newCellValue = "";
                                foreach (var enumValue in enumValues)
                                {
                                    // TODO 需要注入获取枚举文本的服务
                                    newCellValue += "," + Formula.FormulaHelper.GetService<Formula.IEnumService>().GetEnumText(enumKey, enumValue);
                                }
                                cellValue = newCellValue.Trim(',');
                            }
                            else
                            {
                                // TODO 需要注入获取枚举文本的服务
                                cellValue = Formula.FormulaHelper.GetService<Formula.IEnumService>().GetEnumText(enumKey, cellValue);
                            }
                        }
                        cells[c.Row, c.Column].PutValue(cellValue);
                    }

                    // 2. 处理格式化内容
                    if (keywords == Constant.TEMPLATE_KEYWORDS_FORMATCONTENT)
                    {
                        var content = dic[Constant.TEMPLATE_KEYWORDS_FORMATCONTENT].TrimStart();
                        // 是否包含枚举
                        if (dic.ContainsKey(Constant.TEMPLATE_KEYWORDS_ENUM))
                        {
                            // 考虑多选枚举的情况
                            if (cellValue.IndexOf(",") >= 0)
                            {
                                var enumValues = cellValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                foreach (var enumValue in enumValues)
                                {
                                    var index = content.IndexOf(enumValue);
                                    if (index >= 0 && !string.IsNullOrWhiteSpace(enumValue))
                                    {
                                        // 先找出对应枚举值之后的“□”的位置
                                        int pos = content.Substring(index).IndexOf(Constant.SPECIAL_SYMBOLS_BOX);
                                        // 取位置前部分+替换字符串+位置（加上查找字符长度）后部分
                                        content = content.Substring(0, index + pos) + Constant.SPECIAL_SYMBOLS_CHECK + content.Substring(index + pos + Constant.SPECIAL_SYMBOLS_BOX.Length);
                                    }
                                }
                            }
                            else
                            {
                                var index = content.IndexOf(cellValue);
                                if (index >= 0 && !string.IsNullOrWhiteSpace(cellValue))
                                {
                                    // 先找出对应枚举值之后的“□”的位置
                                    int pos = content.Substring(index).IndexOf(Constant.SPECIAL_SYMBOLS_BOX);
                                    // 取位置前部分+替换字符串+位置（加上查找字符长度）后部分
                                    content = content.Substring(0, index + pos) + Constant.SPECIAL_SYMBOLS_CHECK + content.Substring(index + pos + Constant.SPECIAL_SYMBOLS_BOX.Length);
                                }

                            }
                            cells[c.Row, c.Column].PutValue(content);
                        }
                        else
                        {
                            cells[c.Row, c.Column].PutValue(string.Format(content, cellValue));
                        }
                    }
                }
            }
            worksheet.Comments.Clear();

            // 将报表内容保存为文件流（字节数组）
            var excelStream = new MemoryStream();
            designer.Workbook.Save(excelStream, SaveFormat.Excel97To2003);
            var buffer = excelStream.ToArray();
            excelStream.Close();

            return buffer;
        }

        /// <summary>
        /// 解析报表模板
        /// </summary>
        public byte[] ParseTemplate(IList<ColumnInfo> columns, string excelKey, string title)
        {
            if (columns == null)
                throw new Exception("模板列信息不能为NULL");

            return CreateTemplate(columns, excelKey, title);
        }

        /// <summary>
        /// 动态生产简化模板，用于动态导出列表信息
        /// </summary>
        /// <param name="columns">模板中的列信息描述</param>
        /// <returns></returns>
        public byte[] CreateTemplate(IList<ColumnInfo> columns, string excelKey, string title)
        {
            var workbook = new Workbook();
            var sheet = (Worksheet)workbook.Worksheets[0];

            Cells cells = sheet.Cells;
            sheet.FreezePanes(2, 1, 2, 0);//冻结第一行
            //sheet.Name = jsonObject.sheetName;//接受前台的Excel工作表名

            //为标题设置样式     
            var styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 18;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 

            //题头样式 
            var styleHeader = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleHeader.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleHeader.Font.Name = "宋体";//文字字体 
            styleHeader.Font.Size = 14;//文字大小 
            styleHeader.Font.IsBold = true;//粗体 
            styleHeader.IsTextWrapped = true;//单元格内容自动换行 
            styleHeader.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            styleHeader.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //内容样式
            var styleContent = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleContent.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleContent.Font.Name = "宋体";//文字字体 
            styleContent.Font.Size = 12;//文字大小 
            styleContent.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            styleContent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            styleContent.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            styleContent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //表格列数 
            var columnCount = columns.Count;

            //生成行1 标题行    
            cells.Merge(0, 0, 1, columnCount);//合并单元格 
            cells[0, 0].PutValue(title);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 25);

            //生成题头列行 
            for (int i = 0; i < columnCount; i++)
            {
                cells[1, i].PutValue(columns[i].ChineseName);
                cells[1, i].SetStyle(styleHeader);
            }
            cells.SetRowHeight(1, 23);

            //生成内容行，第三行起始。变量绑定表达式 &=[Data Source].[Field Name] 或者 &=DataSource.FieldName
            for (int k = 0; k < columnCount; k++)
            {
                // 变量绑定表达式 &=[Data Source].[Field Name] 或者 &=DataSource.FieldName
                var exp = string.Format("&=[{0}].[{1}]", columns[k].TableName, columns[k].FieldName);
                cells[2, k].PutValue(exp);

                // 设置时间单元格格式
                if (!string.IsNullOrWhiteSpace(columns[k].DateFormat))
                {
                    styleContent.Number = 14;
                    cells[2, k].SetStyle(styleContent);
                    styleContent.Number = 0;
                }
                else
                {
                    cells[2, k].SetStyle(styleContent);
                }
            }
            cells.SetRowHeight(2, 22);


            //添加制表日期
            //cells[2 + rowCount, columnCount - 1].PutValue("制表日期:" + DateTime.Now.ToShortDateString());
            sheet.AutoFitColumns();//让各列自适应宽度
            //sheet.AutoFitRows();//让各行自适应宽度

            // 设置唯一Key，用于校验模板是否被修改过
            //workbook.BuiltInDocumentProperties.Comments = string.Join(",", columns.Select(c => c.FieldName).ToArray());
            //workbook.BuiltInDocumentProperties.Version = int.Parse(DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString());

            // 是否生产临时文件供跟踪测试
            // workbook.Save(@"D:/Tmpl.xls");

            // 将报表内容保存为文件流（字节数组）
            var excelStream = new MemoryStream();
            workbook.Save(excelStream, SaveFormat.Excel97To2003);
            var buffer = excelStream.ToArray();
            excelStream.Close();

            // 写入模板的元数据
            new DefaultExcelMetadataStorage().AddMetadata(excelKey, buffer, string.Join(",", columns.Select(c => c.FieldName).ToArray()));

            return buffer;
        }

        /// <summary>
        /// 解析数据源
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public DataSet ParseDataSource(DataSet ds)
        {
            var columns = System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Items["__ColumnInfo"] as List<ColumnInfo> : null;
            if (columns != null)
            {
                var enumColumns = columns.FindAll(c => !string.IsNullOrWhiteSpace(c.EnumKey) || !string.IsNullOrWhiteSpace(c.DateFormat));
                foreach (var col in enumColumns)
                {
                    var dt = ds.Tables[col.TableName];
                    foreach (DataRow row in dt.Rows)
                    {
                        var fieldValue = row[col.FieldName].ToString();

                        // 处理枚举值
                        if (!string.IsNullOrWhiteSpace(col.EnumKey) && col.EnumKey != "NULL")
                        {
                            var enumItem = col.EnumDataSource.FirstOrDefault(item => item.Value == fieldValue);
                            if (enumItem != null)
                            {
                                fieldValue = enumItem.Text;
                            }
                        }

                        // 处理日期字段
                        if (!string.IsNullOrWhiteSpace(col.DateFormat))
                        {
                            fieldValue = ParseDateTime(fieldValue, col.DateFormat);
                            if (fieldValue == "")
                            {
                                row[col.FieldName] = DBNull.Value;
                            }
                            else
                            {
                                row[col.FieldName] = fieldValue;
                            }
                        }
                        else
                        {
                            row[col.FieldName] = fieldValue;
                        }
                    }
                }
            }

            return ds;
        }

        // 解析日期格式
        public string ParseDateTime(string date, string format)
        {
            DateTime dt;
            if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out dt))
            {
                return dt.ToString(format);
            }

            return date ?? string.Empty;
        }

        /// <summary>
        /// 解析批注信息，获取单元格的元数据
        /// </summary>
        /// <param name="note">批注信息</param>
        /// <remarks>原理：找到所有模板关键字的位置，然后排序之后破开字符串。最后存入键值对中。</remarks>
        /// <returns></returns>
        public IDictionary<string, string> ParseNote(string note)
        {
            var dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(note))
            {
                // 获取所有模板关键字出现的位置
                var indexs = new List<int>();
                foreach (var word in Constant.TEMPLATE_KEYWORDS)
                {
                    var index = note.IndexOf(word);
                    if (index >= 0)
                        indexs.Add(index);
                }
                indexs.Sort(); // 排序，从小到大

                // 将元数据内容存入键值对
                for (var i = 0; i < indexs.Count; i++)
                {
                    // 破开得到元数据内容项，如：【枚举】Type
                    var item = "";
                    if (i == indexs.Count - 1)
                    {
                        item = note.Substring(indexs[i]);
                    }
                    else
                    {
                        item = note.Substring(indexs[i], indexs[i + 1] - indexs[i]);
                    }

                    // 解析元数据到键值对中
                    foreach (var word in Constant.TEMPLATE_KEYWORDS)
                    {
                        if (!dic.ContainsKey(word) && item.StartsWith(word))
                        {
                            dic.Add(word, item.Replace(word, ""));
                        }
                    }
                }
            }

            return dic;
        }

    }
}
