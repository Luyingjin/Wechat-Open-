using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Formula.ImportExport
{
    public interface IExporterForm
    {
        /// <summary>
        /// 获取导出主表数据
        /// </summary>
        /// <param name="tmplKey">模板Key</param>
        /// <param name="title">导出的标题</param>
        /// <param name="id">主ID</param>
        /// <returns></returns>
        DataRow GetMasterData(string tmplKey, string title, string id);

        /// <summary>
        /// 获取导出从表数据
        /// </summary>
        /// <param name="tmplKey">模板Key</param>
        /// <param name="title">导出的标题</param>
        /// <param name="relateID">关联ID</param>
        /// <returns></returns>
        DataSet GetDetailData(string tmplKey, string title, string relateID);
    }
}
