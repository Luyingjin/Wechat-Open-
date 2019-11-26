using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// 单元格的数据，数据格式校验时，可以精确定位到某个单元格。
    /// </summary>
    public class ExcelCellInfo
    {
        public ExcelCellInfo(CellConfig config)
        {
            this.Structure = config;
        }

        /// <summary>
        /// 字段值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName
        {
            get
            {
                if (Structure == null)
                    throw new Exception("单元格的配置信息Structure 不能为NULL！");

                return Structure.FieldName;
            }
        }

        /// <summary>
        /// 单元格的配置信息
        /// </summary>
        public CellConfig Structure { get; set; }
    }
}
