using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.ImportExport
{
    /// <summary>
    /// 常量定义类
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// 模板关键字：枚举
        /// </summary>
        public const string TEMPLATE_KEYWORDS_ENUM = "【枚举】";

        /// <summary>
        /// 模板关键字：格式化内容，可以用{0}来代替值，枚举中的内容 □ 改为 √
        /// </summary>
        public const string TEMPLATE_KEYWORDS_FORMATCONTENT = "【内容】";

        /// <summary>
        /// 模板关键字的常量数组
        /// </summary>
        public static string[] TEMPLATE_KEYWORDS = new string[] { TEMPLATE_KEYWORDS_ENUM, TEMPLATE_KEYWORDS_FORMATCONTENT };

        /// <summary>
        /// 特殊字符：枚举中的框
        /// </summary>
        public const string SPECIAL_SYMBOLS_BOX = "□";

        /// <summary>
        /// 特殊字符：枚举中的勾选
        /// </summary>
        public const string SPECIAL_SYMBOLS_CHECK = "√";

    }
}
