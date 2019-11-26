using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// Excel模板元数据信息的存储接口
    /// </summary>
    public interface IExcelMetadataStorage
    {
        /// <summary>
        /// 写入模板的元数据
        /// </summary>
        /// <param name="excelKey">模板Key</param>
        /// <param name="tmplBuffer">Excel模板文件流</param>
        /// <param name="keywords">关键字</param>
        /// <returns></returns>
        void AddMetadata(string excelKey, byte[] tmplBuffer, string keywords);

        /// <summary>
        /// 获取指定Key的模板元数据
        /// </summary>
        /// <param name="key">模板Key</param>
        /// <returns></returns>
        ExcelMetadata GetMetadataByKey(string key);

        /// <summary>
        /// 判断是否为有效的Excel数据文件
        /// </summary>
        /// <param name="xlsBuffer">上传的Excel数据文件</param>
        /// <param name="metadata">模板的元数据信息</param>
        /// <returns></returns>
        bool IsValidExcel(byte[] xlsBuffer, ExcelMetadata metadata);
    }
}
