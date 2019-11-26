<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdatePwd.aspx.cs" Inherits="Portal.UpdatePwd" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>密码修订</title>
    <style type="text/css">
        body, td, th
        {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }
        body
        {
            background-image: url(styles/pass_bg_gray.jpg);
            background-color: #dfdfdf;
            background-repeat: no-repeat;
            background-position: center top;
        }
        .div_input_bg
        {
            height: 26px;
            width: 161px;
        }
        .div_input_bg input
        {
            background: url(styles/pass_input_gray.jpg) no-repeat;
            border: 0px;
            height: 20px;
            width: 157px;
            padding: 2px;
            padding-left: 5px;
        }
        .text_login
        {
            color: #00F;
        }
        .gray666
        {
            color: #666;
        }
    </style>
</head>
<body>
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td width="50%" height="260">
                &nbsp;
            </td>
            <td width="50%">
            </td>
        </tr>
        <tr>
            <td height="150" colspan="2" align="center" valign="top">
                <form id="Form1" method="post" runat="server">
                <table width="300" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td width="100" height="30" align="right" valign="middle" class="gray666">
                            用户名：
                        </td>
                        <td valign="middle">
                            <div class="div_input_bg">
                                <asp:TextBox id="SystemName" runat="server" style="width:150px"></asp:TextBox>
									<asp:RequiredFieldValidator id="RequiredSystemName" runat="server" ErrorMessage="请输入用户名" ControlToValidate="SystemName"></asp:RequiredFieldValidator>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td height="35" align="right" valign="middle" class="gray666">
                            密&nbsp;&nbsp;&nbsp;&nbsp;码：
                        </td>
                        <td valign="middle">
                            <div class="div_input_bg">
                                <asp:TextBox ID="Password" runat="server" TextMode="Password" Style="width: 150px"></asp:TextBox></div>
                        </td>
                    </tr>
                    <tr>
                        <td height="30" align="right" class="gray666">
                            新密码：
                        </td>
                        <td valign="middle">
                            <div class="div_input_bg">
                                <asp:TextBox ID="NewPassword1" runat="server" TextMode="Password" Style="width: 150px"></asp:TextBox></div>
                        </td>
                    </tr>
                    <tr>
                        <td height="30" align="right" class="gray666">
                            重复新密码：
                        </td>
                        <td valign="middle">
                            <div class="div_input_bg">
                                <asp:TextBox ID="NewPassword2" runat="server" TextMode="Password" Style="width: 150px"></asp:TextBox></div>
                        </td>
                    </tr>
                    <tr>
                        <td height="40">
                            &nbsp;
                        </td>
                        <td valign="middle">
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td width="54">
                                        <asp:Button ID="ButtonSave" runat="server" Text="确定" CssClass="JcButton" OnClick="ButtonSave_Click"></asp:Button>
                                    </td>
                                    <td width="84">
                                        <asp:Button ID="ButtonBack" runat="server" CssClass="JcButton" Text="重新登录" CausesValidation="False"
                                                                OnClick="ButtonBack_Click"></asp:Button>
                                    </td>
                                    <td class="gray666">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td height="30" align="right" valign="middle" class="gray666">
                            &nbsp;
                        </td>
                        <td align="left" valign="middle" class="text_login">
                             <asp:Label ID="LabelError" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
                </form>
            </td>
        </tr>
        <tr>
            <td height="230">
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
 </body>
</html>
