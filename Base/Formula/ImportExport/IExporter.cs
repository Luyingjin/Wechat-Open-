using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Formula.ImportExport
{
    /// <summary>
    /// 导出数据接口，一般常用的是Excel和Word，Excel用于列表，Word用于单张表单
    /// </summary>
    /// <remarks>
    /// 导出方案：用户自定义导出模板，变量绑定表达式 &=[Data Source].[Field Name] 或者 &=DataSource.FieldName，然后通过第三方组件（如Aspose）进行导出
    /// 列表动态导出方案：客户端回传列表信息和数据源获取的相关信息，在服务端动态生成模板进行导出
    /// 打印方案：导出Word并转化为PDF，PDF在Web中直接打开，通过PDF上的打印按钮可以直接打印。
    /// </remarks>
    public interface IExporter
    {
        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <param name="dicVariable"></param>
        /// <param name="templateBuffer"></param>
        /// <returns></returns>
        byte[] Export(IDictionary<string, object> dicVariable, byte[] templateBuffer);

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="templateBuffer"></param>
        /// <returns></returns>
        byte[] Export(DataTable dt, byte[] templateBuffer);

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="templateBuffer"></param>
        /// <returns></returns>
        byte[] Export(IDictionary<string, object> dicVariable, DataSet ds, byte[] templateBuffer);
    }
}
