using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Portal
{
	/// <summary>
	/// WebForm1 的摘要说明。
	/// </summary>
	public class IEConfig : System.Web.UI.Page
	{
		string RegText=
			@"REGEDIT4

{ServerInTrust}

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\2]
@=""""
""1001""=dword:00000000
""1004""=dword:00000000
""1200""=dword:00000000
""1201""=dword:00000000
""1405""=dword:00000000

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\1]
@=""""
""1001""=dword:00000000
""1004""=dword:00000000
""1200""=dword:00000000
""1201""=dword:00000000
""1405""=dword:00000000

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings]
""UrlEncoding""=dword:00000001

";

		string domainReg=
@"[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains]
[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\{ServerName}]

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\{ServerName}\]
""https""=dword:00000002


[HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\New Windows\Allow]
""{IPAddress}""=hex:
";
		string ipReg=
            @"[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Ranges]
@=""""

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Ranges\{IPAddress}]
""*""=dword:00000002
			"":Range""=""{IPAddress}""

[HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\New Windows\Allow]
""{IPAddress}""=hex:
";

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "application/octet-stream ; Charset=GB2312";
			Response.AddHeader("Content-Disposition", "attachment;filename=Goodway IE Config.reg");
			Response.AddHeader("Content-Transfer-Encoding","binary"); 
			
			string serverName = Request.ServerVariables["SERVER_NAME"];
			if(serverName.IndexOf(".")>=0)
			{
				ipReg = ipReg.Replace("{IPAddress}",serverName);
				RegText = RegText.Replace("{ServerInTrust}",ipReg);
			}
			else
			{
				domainReg = domainReg.Replace("{ServerName}",serverName);
				RegText = RegText.Replace("{ServerInTrust}",domainReg);
			}

			// Create the PDF export object
			// Create a new memory stream that will hold the pdf output
			System.IO.MemoryStream memStream = new System.IO.MemoryStream();
			// Export the report to PDF:
			memStream.Write(System.Text.Encoding.ASCII.GetBytes(RegText),0,RegText.Length);
			// Write the PDF stream out
			Response.BinaryWrite(memStream.ToArray());
			// Send all buffered content to the client
			Response.End();
		}

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
