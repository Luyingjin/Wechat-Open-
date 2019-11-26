<%@ Page language="c#" Codebehind="SecurityTip.aspx.cs" AutoEventWireup="false" Inherits="Portal.SecurityTip" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" >
<html>
	<head>
		<title>帮助&支持</title>
        <script language="JScript" src="/CommonWebResource/Theme/Default/CssInc.js"></script>
	</head>
	<body class="SkinBody" style="MARGIN-TOP:20;MARGIN-bottom:20px;">
		<div align="center" valign="center">
			<table width="500" cellspacing="0" cellpadding="0" border="0">
				<!--页眉开始-->
				<tr>
					<td class="ToolBarBg" style="border:1px solid #000000; width:100%">
						<table border="0" cellpadding="0" style="border-collapse: collapse" width="700" id="table1">
							<tr>
								<td width="20%">
									<p>
										<img border="0" src="image/titlesupport.jpg">
								</td>
								<td width="80%" valign="bottom">
									<!--div align="center">
            <table border="0" cellpadding="0" style="border-collapse: collapse" width="450" id="table3" cellspacing="2">
              <tr>
                <td align="center">
                <img border="0" src="images/skin/website.gif" width="32" height="32"></td>
                <td align="center">
                <img border="0" src="images/skin/website.gif" width="32" height="32"></td>
                <td align="center">
                <img border="0" src="images/skin/website.gif" width="32" height="32"></td>
                <td align="center">
                <img border="0" src="images/skin/website.gif" width="32" height="32"></td>
                <td align="center">
                <img border="0" src="images/skin/website.gif" width="32" height="32"></td>
              </tr>
              <tr>
                <td align="center"><font color="#003F87">
                <a href="prj_index.htm">项目空间</a></font></td>
                <td align="center">　</td>
                <td align="center">　</td>
                <td align="center">　</td>
                <td align="center">　</td>
              </tr>
            </table>
            </div-->
								</td>
							</tr>
							<tr>
								<td colspan="2" height="4" class="ToolsHideBar"></td>
							</tr>
							<tr>
								<td colspan="2" bgcolor="#ffffff" height="100" style="background-image: url('image/bg.jpg'); background-repeat: repeat-x; background-position-y: bottom">
									<div align="center">
										<table>
										<tr>
											<td>
												<p align="center">
												<img border="0" src="image/warning.gif" width="32" height="32"></td>
												<td><FONT  color="red" style="font-size:10pt"><B>您的网页浏览器(Internet 
													Explore)没有正确设置，使用前请依照下列步骤设置:</B></FONT></td>
										</tr>
										</table>
										<table border="0" cellpadding="0" style="border-collapse: collapse" width="90%" id="table2">
											<br>
											如何将本系统的网络地址加入到 <b>信任站点</b> 安区区域，请依照下列步骤:
											<br>
											<br>
											1. 打开 Internet Explorer, 选择<b>工具</b>菜单, 点击 <b>Internet选项</b>.
											<br>
											<IMG height="147" src="/portal/image/IESetup01.jpg" width="140" border="0">
											<br>
											<br>
											2. 点击<b>安全</b>TAB页, 点击<b>受信的任站点</b>区域.
											<br>
											<IMG src="/portal/image/install01.jpg" border="0">
											<br>
											<br>
											3. 点击 <b>站点</b>按钮.
											<br>
											<br>
											4. 在 <b>将该站点添加到区域中</b> 文本框内, 输入 <b style="COLOR:red"><i>http://<%=GenerateServerUrl()%></i></b>, 
											点击 <b>添加</b>.
											<br>
											<IMG src="/portal/image/install02.jpg" border="0">
											<br>
											<br>
											5. 点击可信站点对话框<b>确定</b>, 点击<b>自定义级别...</b>，在安全设置中将"ActiveX控件和插件"下的所有选项都设置为"启用"，点击 <b>确定</b>按钮。
											<br>
											<IMG src="/portal/image/IESetup03.jpg" border="0">
											<br>
											<br>
											6. 点击<b>应用</b>按钮，弹出确定对话框,点击</B>是<b>按钮，点击<B>确定</B>按钮完成.
												<br>
												<IMG src="/portal/image/install03.jpg" border="0">&nbsp;
												<br>
												<p align="center">&nbsp;
												</p>
											</b>
										</table>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</body>
</html>
