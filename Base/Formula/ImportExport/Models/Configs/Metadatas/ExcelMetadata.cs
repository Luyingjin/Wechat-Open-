using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// Excel模板文件的元数据，包含文档属性
    /// </summary>
    public class ExcelMetadata
    {
        /// <summary>
        /// Excel模板的修订号
        /// </summary>
        public int RevisionNumber { get; set; }

        /// <summary>
        /// Excel模板的作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Excel模板的公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Excel模板的公司
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Excel模板的文件流
        /// </summary>
        public byte[] FileBuffer { get; set; }
    }
}
