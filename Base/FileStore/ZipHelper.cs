using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace FileStore
{
    public class ZipHelper : IDisposable
    {
        System.IO.Stream ms = HttpContext.Current.Response.OutputStream;
        ZipOutputStream zipStream;
        public ZipHelper()
        {
            zipStream = new ZipOutputStream(ms);
        }

        Crc32 crc;
        ZipEntry entry;

        public void AddFile(string fileName, long filesize)
        {
            crc = new Crc32();
            crc.Reset();
            entry = new ZipEntry(fileName);
            entry.DateTime = DateTime.Now;
            entry.Size = filesize;
            zipStream.PutNextEntry(entry);
        }

        public void AppentFileBytes(byte[] buffer, int length)
        {

            crc.Update(buffer, 0, length);
            entry.Crc = crc.Value;

            zipStream.Write(buffer, 0, length);
            zipStream.Flush();
        }

        public void Complete()
        {
            zipStream.Finish();
        }


        //public void AddByte(string fileName, byte[] buffer)
        //{
        //    Crc32 crc = new Crc32();
        //    ZipEntry entry = new ZipEntry(fileName);
        //    entry.DateTime = DateTime.Now;
        //    entry.Size = buffer.Length;

        //    crc.Reset();
        //    crc.Update(buffer);
        //    entry.Crc = crc.Value;
        //    zipStream.PutNextEntry(entry);
        //    zipStream.Write(buffer, 0, buffer.Length);
        //}

        //public byte[] GetBytes()
        //{
        //    zipStream.Finish();
        //    byte[] bytes = ms.ToArray();
        //    zipStream.Close();
        //    return bytes;
        //}


        public Stream GetStream()
        {
            zipStream.Finish();
            return ms;
        }


        public void Dispose()
        {
            zipStream.Close();
        }


    }

}