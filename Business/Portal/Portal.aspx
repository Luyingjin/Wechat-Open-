<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Portal.aspx.cs" Inherits="Portal.Frameworks.Portal" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>综合管理信息系统</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <script type="text/javascript" language="JScript" src="/CommonWebResource/CoreLib/Combine/SimplePageInc.js"></script>
    <script language="JScript" src="/CommonWebResource/Theme/<%=SysColor %>/MiniCssInc.js" type="text/javascript"></script>
    <script type="text/javascript">

        self.moveTo(0, 0);
        self.resizeTo(screen.availWidth, screen.availHeight);

        if (!SecurityTest()) {
            window.open("/portal/SecurityTip.aspx", "SecurityTip", "scrollbars=yes,resizable=yes,compact");
            window.open("/portal/LoginBegin.aspx", "_self");
        }



        function SecurityTest() {
            if (window.ActiveXObject == undefined) return true;
            var canxml, canscript, canactivex;
            try {
                var oXmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
                canxml = true;
            } catch (e)
            { canxml = false; }
            try {
                var oDate = new Date().toDateString();
                canscript = true;
            } catch (e)
            { canscript = false; }
            if (canxml && canscript)
                return true;
            else
                return false;

        }

        var ENTERPRISE_HOMEBOARD_URL = "<%=ENTERPRISE_HOMEBOARD_URL %>";
        function SetUrl() {
            var url = "<%=HOMEBOARD_URL%>?Height=" + screen.availHeight + "&Width=" + screen.availWidth;
            $("#Index").attr("src", url);
        }

        //CheckBrowser();

        //function CheckBrowser() {
        //    if (!CheckSilverlightInstalled()) {
        //        if(confirm("检测到您的Silverlight控件版本过低或没有安装，点击取消将跳过！"))
        //            window.open("/portal/install/ClientInstall.htm", "_self");
        //    }
        //}

        //function CheckSilverlightInstalled() {
        //    var isSilverlightInstalled = false;
        //    try {
        //        try {
        //            var slControl = new ActiveXObject('AgControl.AgControl'); //检查IE
        //            isSilverlightInstalled = true;
        //        }
        //        catch (e) {
        //            if (navigator.plugins["Silverlight Plug-In"]) //检查非IE
        //            {
        //                isSilverlightInstalled = true;
        //            }
        //        }
        //    } catch (e) { }

        //    return isSilverlightInstalled;
        //}

        var isStop = true;

        $(window).bind('beforeunload', function () {
            if (isStop) {
                return "确认关闭或重置本系统吗?";
            }
        })


        $(window).unload(function () {
            jQuery.get("/Portal/Logout.aspx?RequestAction=Logout");
        });

       
    </script>
</head>
<body style="padding: 0px; margin: 0px; overflow: hidden;" onload="SetUrl();">
    <div id="FrameArea" align="center" style="position: absolute; height: 100%; left: 0;
        width: 100%; top: 0">
        <iframe height="100%" id="Index" frameborder="0" src="" width="100%"></iframe>
    </div>
</body>
</html>
