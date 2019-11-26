using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula.Helper
{
    public class StringHelper
    {
        /// <summary>
        /// 源字符串数组中有目标中的任何一个则返回true
        /// </summary>
        /// <param name="srcArray"></param>
        /// <param name="destArray"></param>
        /// <returns></returns>
        public static bool Has(string[] srcArray, string[] destArray)
        {
            foreach (string dest in destArray)
            {
                if (dest == "")
                    continue;
                if (srcArray.Contains(dest))
                    return true;
            }
            return false;
        }

        public static bool Has(string srcSplit, string destSplit)
        {
            return Has(srcSplit.Split(','), destSplit.Split(','));
        }

        /// <summary>
        /// 源字符串数组中不包含目标的任意一个则返回False
        /// </summary>
        /// <param name="srcArray"></param>
        /// <param name="destArray"></param>
        /// <returns></returns>
        public static bool HasAll(string[] srcArray, string[] destArray)
        {
            foreach (string dest in destArray)
            {
                if (dest == "")
                    continue;
                if (!srcArray.Contains(dest))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 源字符串数组中不包含目标的任意一个则返回False
        /// </summary>
        /// <param name="srcArray"></param>
        /// <param name="destArray"></param>
        /// <returns></returns>
        public static bool HasAll(string srcSplit, string destSplit)
        {
            return HasAll(srcSplit.Split(','), destSplit.Split(','));
        }

        /// <summary>
        /// 字符串型数字加法
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string AddNum(string str, int num)
        {
            int src = 0;
            if (string.IsNullOrEmpty(str))
                src = 0;
            else
                src = int.Parse(str);
            return (src + num).ToString();
        }


        /// <summary>
        /// 获取逗号分隔字符串的个数
        /// </summary>
        /// <param name="csvStr"></param>
        /// <returns></returns>
        public static int GetCsvCount(string csvStr)
        {
            if (string.IsNullOrEmpty(csvStr))
                return 0;
            return csvStr.Split(',').Where(c => c != "").Distinct().Count();
        }

        public static string Distinct(string str)
        {
            return string.Join(",", str.Split(',').Where(c => c != "").Distinct());
        }

        /// <summary>
        /// 从源字符串中排除目标字符串
        /// </summary>
        /// <param name="src"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string Exclude(string src, string dest)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dest))
                return src;

            var d = dest.Split(',');


            return string.Join(",", src.Split(',').Where(c => !d.Contains(c)));
        }

        /// <summary>
        /// 源串冲加入目标串
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static string Include(string src, string dest)
        {
            if (string.IsNullOrEmpty(src))
                return dest;
            if (string.IsNullOrEmpty(dest))
                return src;

            string s = src.Trim(',') + "," + dest.Trim(',');

            return string.Join(",", s.Split(',').Distinct());
        }
    }
}
