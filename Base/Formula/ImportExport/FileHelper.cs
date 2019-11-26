using System.IO;

namespace Formula.ImportExport
{
    /// <summary>
    /// 文件辅助类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 获取指定文件路径的字节数组
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static byte[] GetFileBuffer(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            long fileSize = fileStream.Length;
            byte[] fileBuffer = new byte[fileSize];
            fileStream.Read(fileBuffer, 0, (int)fileSize);
            fileStream.Close();

            return fileBuffer;
        }

        /// <summary>
        /// 将字节数组保存为文件
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="filePath">文件路径</param>
        public static void SaveFileBuffer(byte[] buffer, string filePath)
        {
            Stream s = new FileStream(filePath, FileMode.Create);
            s.Write(buffer, 0, buffer.Length);
            s.Flush();
            s.Close();

            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception(string.Format("文件没有写入，请确认目录是否有写权限！目录路径为：{0}", filePath));
            }
        }
    }
}
