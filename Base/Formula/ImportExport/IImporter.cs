using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// 数据导入借口，一般用于Excel的数据导入功能
    /// </summary>
    public interface IImporter
    {
        /// <summary>
        /// 从Excel中导入数据，读取excel，采用方案为先下载模板，然后填充数据，最后根据配置分析Excel数据并读取
        /// </summary>
        /// <param name="excelFile">excel数据文件</param>
        /// <param name="excelKey">Excel模板Key</param>
        /// <returns></returns>
        ExcelData Import(byte[] excelFile, string excelKey);

        /// <summary>
        /// 从Excel中导入数据，读取excel，采用方案为先下载模板，然后填充数据，最后根据配置分析Excel数据并读取
        /// </summary>
        /// <param name="excelFile">excel数据文件</param>
        /// <param name="config">Excel配置信息</param>
        /// <returns></returns>
        ExcelData Import(byte[] excelFile, ExcelConfig config);

        /// <summary>
        /// 获取指定Key的模板文件路径
        /// </summary>
        /// <param name="excelKey">数据导入注册的Key</param>
        /// <returns></returns>
        string GetExcelTemplateUrl(string excelKey);
    }
}
