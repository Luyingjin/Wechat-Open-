using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace FileStore
{
    public class FileHelper
    {
        public static long CopyFile(string srcFileName, string destFileName)
        {
            CheckFileFullPath(destFileName);
            FileInfo file = new FileInfo(srcFileName);
            file.CopyTo(destFileName);
            return file.Length;
        }
        public static string GetFileName(string fileFullName)
        {
            return Path.GetFileName(fileFullName);
        }
        public static string GetExtName(string fileFullName)
        {
            return Path.GetExtension(fileFullName);
        }
        public static long GetFileSize(string fileFullName)
        {
            FileInfo file = new FileInfo(fileFullName);
            return file.Length;
        }
        public static bool FileExist(string fileFullPath)
        {
            return File.Exists(fileFullPath);
        }
        public static bool MoveFile(string srcFullPath, string destFullPath)
        {

            CheckFileFullPath(destFullPath);

            File.Move(srcFullPath, destFullPath);
            return true;
        }



        public static string CombinePath(string path1, string path2)
        {
            string result = Path.Combine(path1, path2);
            return result;
        }
        public static bool AppendFile(string fileFullName, byte[] bytes, long fileSize)
        {     
            long pos = 0;
            using (FileStream fs = File.Open(fileFullName, FileMode.Append))
            {
                int length = bytes.Length > (int)(fileSize - fs.Length) ? (int)(fileSize - fs.Length) : bytes.Length;
                pos = fs.Length;
                fs.Write(bytes, 0, length);

                fs.Close();
            }

            //如果全部文件保存完成
            if (bytes.Length >= fileSize - pos)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void DelFile(string fileFullName)
        {
            if (File.Exists(fileFullName))
                File.Delete(fileFullName);
        }

     
        public static bool SaveFile(string fileFullName, byte[] bytes, long fileSize, int pos)
        {
            CheckFileFullPath(fileFullName);

            long length = bytes.Length > fileSize - pos ? fileSize - pos : bytes.Length;

            using (FileStream fileStream = new FileStream(fileFullName, FileMode.OpenOrCreate))
            {

                ////如果已经写过
                //if (pos < fileStream.Length)
                //{
                //    fileStream.Close();
                //    return;
                //}
                fileStream.Position = pos;
                fileStream.Write(bytes, 0, (int)length);
                fileStream.Close();
            }

            //如果全部文件保存完成
            if (bytes.Length >= fileSize - pos)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static byte[] GetFileBytes(string fileFullName)
        {
            FileInfo fileInfo = new FileInfo(fileFullName);
            FileStream fs = fileInfo.OpenRead();
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();
            return bytes;
        }

        public static byte[] GetFileBytes(string fileFullName, int pos, int length)
        {
            FileInfo fileInfo = new FileInfo(fileFullName);

            FileStream fs = fileInfo.OpenRead();

            long len = fs.Length - pos;
            if (len <= 0)
            {
                fs.Close();
                return new byte[0];
            }
            if (len > length)
                len = length;

            byte[] bytes = new byte[len];

            fs.Position = pos;
            fs.Read(bytes, 0, (int)len);


            fs.Close();
            return bytes;
        }

        public static string GetFolderPath(string fileName)
        {
            int fileId = GetFileId(fileName);
            return GetFolderPath(fileId);
        }

        public static string GetFolderPath(int fileId)
        {

            int num = fileId / 1000 + 1;
            string folderPath = string.Format("{0}{1}", num.ToString("D8"), "_FileStore");
            return folderPath;
        }

        public static int GetFileId(string fileName)
        {
            if (fileName.Contains('_'))
                fileName = fileName.Split('_').First();
            int fileId = 0;
            if (!int.TryParse(fileName, out fileId))
                throw new Exception(string.Format("文件名中包含无效的文件Id'{0}'", fileName));
            return fileId;
        }

        /// <summary>
        /// 去掉文件名、下划线，只保留文件Id
        /// </summary>
        /// <param name="fileIds"></param>
        /// <returns></returns>
        public static string FixFileIds(string fileIds)
        {
            Regex reg = new Regex("_.*?((?=,)|$)");

            return reg.Replace(fileIds, "");
        }


        public static int[] GetFileIds(string fileNames)
        {

            string[] strfileIds = FixFileIds(fileNames).Split(',');

            int[] fileIds = new int[strfileIds.Length];

            for (int i = 0; i < fileIds.Length; i++)
            {
                fileIds[i] = GetFileId(strfileIds[i]);
            }
            return fileIds;
        }

        public static void CheckFileFullPath(string fileFullPath)
        {
            string direction = fileFullPath.Remove(fileFullPath.LastIndexOf('\\'));

            if (!Directory.Exists(direction))
            {
                //创建物理文件夹
                Directory.CreateDirectory(direction);
            }
        }

        public static string GetFileFullPath(string rootFolderPath, string fileName, string fileGuid, bool allowEncrypt)
        {
            int fileID = GetFileId(fileName);

            string path = CombinePath(rootFolderPath, GetFolderPath(fileID));

            if (allowEncrypt)
                return CombinePath(path, fileGuid);
            else
                return CombinePath(path, fileName);
        }

    }

}