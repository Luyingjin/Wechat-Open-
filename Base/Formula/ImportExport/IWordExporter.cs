using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Aspose.Words.Reporting;
using Aspose.Words;

namespace Formula.ImportExport
{
    /// <summary>
    /// 导出Word接口，
    /// </summary>
    /// <remarks>
    /// 打印方案：导出Word并转化为PDF，PDF在Web中直接打开，通过PDF上的打印按钮可以直接打印。
    /// </remarks>
    public interface IWordExporter
    {
        /// <summary>
        /// 导出Word，通过在指定的地方，插入->域，选择MergeField，然后再用邮件合并的方式，来合成word文档
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="templateFilePath">Word模版文件</param>
        /// <param name="handleMergeField">合并普通字段的自定义处理逻辑，示例代码：e.Text = enumService.GetEnumText("OrgType", e.FieldValue.ToString());</param>
        /// <param name="handleMergeImageField">合并图片字段的自定义处理逻辑，示例代码：e.ImageStream = new MemoryStream((byte[])e.FieldValue);</param>
        /// <returns></returns>
        byte[] ExportWord(DataSet ds, string templateFilePath, Action<FieldMergingArgs> handleMergeField = null, Action<ImageFieldMergingArgs> handleMergeImageField = null);

        /// <summary>
        /// 导出Word并转为PDF，通过在指定的地方，插入->域，选择MergeField，然后再用邮件合并的方式，来合成word文档
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="templateFilePath">Word模版文件</param>
        /// <param name="handleMergeField">合并普通字段的自定义处理逻辑，示例代码：e.Text = enumService.GetEnumText("OrgType", e.FieldValue.ToString());</param>
        /// <param name="handleMergeImageField">合并图片字段的自定义处理逻辑，示例代码：e.ImageStream = new MemoryStream((byte[])e.FieldValue);</param>
        /// <returns></returns>
        byte[] ExportPDF(DataSet ds, string templateFilePath, Action<FieldMergingArgs> handleMergeField = null, Action<ImageFieldMergingArgs> handleMergeImageField = null);

    }
}
