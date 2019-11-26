using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;

namespace WeChatTask.Common
{
    public class MailHelper
    {
        public static bool IsEmail(string inputData)
        {
            return new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*").Match(inputData).Success;
        }

        public static bool SendSmtpMail(string ToEmail, string FromEmail, string FromServer, string FromUserName, string FromLoginName, string FromEmailPassword, string Title, string Body, int ServerPort, bool IsAsnys = false)
        {
            MailMessage message = new MailMessage(FromEmail, ToEmail)
            {
                Subject = Title
            };
            Body = Body.Replace("\r\n", "<br />");
            message.Body = "<pre style=\"width:100%;word-wrap:break-word\">" + Body + "</pre>";
            message.From = new MailAddress(FromEmail, FromUserName);
            message.IsBodyHtml = true;
            try
            {
                string str = FromEmail.Substring(0, FromEmail.IndexOf("@"));
                SmtpClient client = new SmtpClient(FromServer, ServerPort)
                {
                    Credentials = new NetworkCredential(FromLoginName, FromEmailPassword)
                };
                if (IsAsnys)
                {
                    client.SendCompleted += new SendCompletedEventHandler(Client_SendCompleted);
                    object userToken = message;
                    client.SendAsync(message, userToken);
                }
                else
                {
                    client.Send(message);
                }
                return true;
            }
            catch (Exception exception)
            {
                //new AppException("发送邮件错误...", exception);
                Log4NetConfig.Configure();
                LogWriter.Error(exception.ToString());
            }
            return false;
        }
        public static void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string str3;
            StringBuilder builder;
            bool flag = true;
            MailMessage userState = (MailMessage)e.UserState;
            string subject = userState.Subject;
            string body = userState.Body;
            if (e.Cancelled)
            {
                flag = false;
            }
            if (e.Error != null)
            {
                flag = false;
            }
            if (!flag)
            {
                str3 = "Send Mail/Send Mail test asnc Error " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                builder = new StringBuilder();
                builder.Append("Send Failed!  ");
                builder.Append("Subject :" + subject + " ");
                builder.Append("Content :" + body);
                builder.Append("Exception :" + e.Error.ToString());
                //new AppException(builder.ToString());
                Log4NetConfig.Configure();
                LogWriter.Error(builder.ToString());
            }
            else
            {
                str3 = "Send Mail/Send Mail test asnc Error " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                builder = new StringBuilder();
                builder.Append("Send Sucess!  ");
                builder.Append("Subject :" + subject + " ");
                builder.Append("Content :" + body);
                //new AppException(builder.ToString());
                Log4NetConfig.Configure();
                LogWriter.Error(builder.ToString());
            }
        }

    }
}
