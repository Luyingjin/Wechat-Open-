using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// Excel模板的配置信息
    /// </summary>
    public class ExcelConfig
    {
        private IExcelMetadataStorage storage = new DefaultExcelMetadataStorage();
        public ExcelConfig(string excelKey)
        {
            Variables = new List<CellConfig>();
            Tables = new List<TableConfig>();
            this.ExcelKey = excelKey;

            // TODO 改用IOC
            this.Metadata = storage.GetMetadataByKey(excelKey);
        }

        /// <summary>
        /// 变量，只需要导入导出一次的数据
        /// </summary>
        public IList<CellConfig> Variables { get; set; }

        /// <summary>
        /// 表格，需要循环导入导出的数据
        /// </summary>
        public IList<TableConfig> Tables { get; set; }

        /// <summary>
        /// Excel模板的标识，用于校验模板是否被修改过
        /// </summary>
        public string ExcelKey { get; set; }

        /// <summary>
        /// Excel模板的元数据信息
        /// </summary>
        public ExcelMetadata Metadata { get; set; }

        /// <summary>
        /// 判断是否为有效的Excel数据文件，判断依据为：文档的创建日期和版本号相同
        /// </summary>
        /// <returns></returns>
        public bool IsValidExcel(byte[] xlsBuffer)
        {
            return storage.IsValidExcel(xlsBuffer, this.Metadata);
        }
    }
}
