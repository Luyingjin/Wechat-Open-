using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words.Reporting;
using Aspose.Words;
using System.IO;
using System.Data;
using Aspose.Words.Drawing;
using Config;
using Formula.Helper;

namespace Formula.ImportExport
{
    /// <summary>
    /// 利用第三方组件Aspose.Words来进行合并文档
    /// </summary>
    public class AsposeWordExporter : IWordExporter
    {
        public byte[] ExportWord(DataSet ds, string templateFilePath, Action<FieldMergingArgs> handleMergeField = null, Action<ImageFieldMergingArgs> handleMergeImageField = null)
        {
            var doc = GetDocumnet(ds, templateFilePath, handleMergeField, handleMergeImageField);

            // 将Document转化为byte[]。
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms, SaveFormat.Doc);
                data = ms.ToArray();
            }

            return data;
        }

        public byte[] ExportPDF(System.Data.DataSet ds, string templateFilePath, Action<FieldMergingArgs> handleMergeField = null, Action<ImageFieldMergingArgs> handleMergeImageField = null)
        {
            var doc = GetDocumnet(ds, templateFilePath, handleMergeField, handleMergeImageField);

            // 将Document转化为byte[]。
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms, SaveFormat.Pdf);
                data = ms.ToArray();
            }

            return data;
        }

        private Document GetDocumnet(DataSet ds, string templateFilePath, Action<FieldMergingArgs> handleMergeField = null, Action<ImageFieldMergingArgs> handleMergeImageField = null)
        {
            // 载入模板
            Document doc = new Document(templateFilePath);

            // 删除空白区域，比如子表没有数据，则显示显示表头。
            doc.MailMerge.RemoveEmptyRegions = true;

            // 设置合并时的自定义处理逻辑
            doc.MailMerge.FieldMergingCallback = new HandleMergeField(ds, handleMergeField, handleMergeImageField);

            //表单主表
            doc.MailMerge.Execute(ds.Tables["dtForm"].Rows[0]);

            // 调用邮件合并API，将嵌套的DataTable导出到Word中。
            doc.MailMerge.ExecuteWithRegions(ds);

            return doc;
        }
    }

    /// <summary>
    /// 合并字段的处理程序
    /// 参考：Aspose.Words.NET Examples（搜素IFieldMergingCallback） 或者 Aspose.Words的帮助文档
    /// </summary>
    internal class HandleMergeField : IFieldMergingCallback
    {
        Action<FieldMergingArgs> handleMergeField = null;
        Action<ImageFieldMergingArgs> handleMergeImageField = null;
        DataSet ds = null;

        /// <summary>
        /// handleMergeField的示例代码：e.Text = enumService.GetEnumText("OrgType", e.FieldValue.ToString());
        /// handleMergeImageField的示例代码：e.ImageStream = new MemoryStream((byte[])e.FieldValue);
        /// </summary>
        /// <param name="handleMergeField"></param>
        public HandleMergeField(DataSet ds, Action<FieldMergingArgs> handleMergeField = null, Action<ImageFieldMergingArgs> handleMergeImageField = null)
        {
            this.ds = ds;
            this.handleMergeField = handleMergeField;
            this.handleMergeImageField = handleMergeImageField;
        }

        /// <summary>
        /// 合并每条记录的每个字段都会触发这个函数，可以用来处理日期、枚举或者特殊字段，也可以用来处理合并单元格等复杂逻辑。
        /// </summary>
        void IFieldMergingCallback.FieldMerging(FieldMergingArgs args)
        {
            if (handleMergeField != null)
                handleMergeField(args);

            #region 处理大字段

            if (args.DocumentFieldName.StartsWith("Sign:"))
            {
                string fieldName = args.DocumentFieldName.Split(':')[1];
                if (ds.Tables.Contains(args.TableName))
                    InsertSign(args, ds.Tables[args.TableName].Rows[args.RecordIndex][fieldName].ToString());
                else
                    InsertSign(args, ds.Tables[0].Rows[0][fieldName].ToString());
            }
            else if (args.DocumentFieldName.Contains(":"))
            {
                var arr = args.DocumentFieldName.Split(':');
                string fieldName = arr[0];

                string json = "";
                if (ds.Tables.Contains(fieldName)) //子表的情况
                {
                    json = JsonHelper.ToJson(ds.Tables[fieldName]);
                }
                else if (ds.Tables.Contains(args.TableName))
                {
                    json = ds.Tables[args.TableName].Rows[args.RecordIndex][fieldName].ToString();
                }
                else
                {
                    json = ds.Tables[0].Rows[0][fieldName].ToString();
                }

                if (json.StartsWith("{")) //json对象
                {
                    var dic = JsonHelper.ToObject<Dictionary<string, object>>(json);
                    args.Text = dic[arr[1]].ToString();
                }
                else if (json.StartsWith("["))//json数组
                {
                    string signItem = arr[1];
                    int index = 0;
                    if (arr.Length == 3)
                        index = int.Parse(arr[2]);
                    var signs = JsonHelper.ToObject<List<Dictionary<string, object>>>(json);

                    if (index < signs.Count)
                    {
                        string str = "";
                        if (signs[index].ContainsKey(signItem) && signs[index][signItem] != null)
                            str = signs[index][signItem].ToString();

                        if (signItem == "SignTime")
                        {
                            args.Text = str.Split(' ')[0];
                        }
                        else if (signItem == "ExecUserID" || signItem == "TaskUserID")
                        {
                            InsertSign(args, str);
                        }
                        else
                        {
                            args.Text = str;
                        }
                    }
                    else
                    {
                        args.Text = "";
                    }
                }

            }

            #endregion


            if (args.FieldName == "ExecUserID" || args.FieldName == "TaskUserID")
            {
                if (args.FieldValue != null)
                    InsertSign(args, args.FieldValue.ToString());
            }
            else if (args.FieldName == "SignTime")
            {
                if (args.FieldValue != null)
                    args.Text = args.FieldValue.ToString().Split(' ')[0];
            }

        }

        /// <summary>
        /// 用于合并图片字段，图片可以是图片对象、文件路径名称、图片流
        /// </summary>
        /// <param name="args"></param>
        void IFieldMergingCallback.ImageFieldMerging(ImageFieldMergingArgs args)
        {
            if (handleMergeImageField != null)
                handleMergeImageField(args);
        }

        #region 私有方法

        /// <summary>
        /// 插入签名图片
        /// </summary>
        /// <param name="args"></param>
        /// <param name="userID"></param>
        private void InsertSign(FieldMergingArgs args, string userID)
        {
            IUserService service = FormulaHelper.GetService<IUserService>();
            byte[] bytes = service.GetSignImg(userID);
            DocumentBuilder builder = new DocumentBuilder(args.Document);
            builder.MoveToMergeField(args.FieldName);
            if (bytes != null)
            {
                Shape shape = builder.InsertImage(bytes);
                shape.Top = 10;
                shape.Height = 30;
                shape.Width = 80;
            }
            else
            {
                UserInfo user = service.GetUserInfoByID(userID);
                if (user != null)
                {
                    builder.InsertHtml(user.UserName);
                }
                else
                {
                    builder.InsertHtml("");
                }
            }
        }

        #endregion

    }
}
