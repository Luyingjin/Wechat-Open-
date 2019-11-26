<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginBegin.aspx.cs" Inherits="Portal.LoginBegin"%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" >
<html>
<head>
    <title>综合管理信息系统</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <script type="text/javascript" language="JScript" src="/CommonWebResource/CoreLib/Combine/SimplePageInc.js"></script>
      <script src="/CommonWebResource/Theme/Default/MiniCssInc.js" type="text/javascript"></script>
    <link href="styles/style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        * html #login_div {
	        padding-top:411px;
	        width:361px;
	        height:91px;
	        margin:0px auto;
	        z-index:9999;
        }
    </style>
    <script type="text/javascript" language="javascript">
        var CUserName = "";
        var CDomain = "";
        var i = 1;
        var numkey;
        var domainLogin = false; //是否域账户登录

        $(document).ready(function () {
            // 判断是否允许弹出窗口
            //            if (!IsPopupDisabled()) {
            //                msgUI("浏览器设置了阻止弹出窗口！");
            //            }

            $(window).load(function () {
                Init();
                MM_preloadImages('styles/login_bt.gif');
            })
        });



        function IsPopupDisabled() {
            var e = false;
            var pw1 = null;
            var pw2 = null;
            try {
                do {
                    var d = new Date();
                    var wName = "ptest_" + d.getTime();
                    var testUrl = "";
                    pw1 = window.open(testUrl, wName, "width=0,height=0,left=5000,top=5000", true);
                    if (null == pw1 || true == pw1.closed) {
                        e = true;
                        break;
                    }
                    pw2 = window.open(testUrl, wName, "width=0,height=0,left=5000,top=5000");
                    if (null == pw2 || true == pw2.closed) {
                        e = true;
                        break;
                    }
                    pw1.close();
                    pw2.close();
                    pw1 = pw2 = null;
                }
                while (false);
            }
            catch (ex) {
                e = true;
            }
            if (null != pw1) {
                try { if (!pw1.closed) pw1.close(); } catch (ex) { }
            }
            if (null != pw2) {
                try { if (!pw2.closed) pw2.close(); } catch (ex) { }
            }
            return !e;
        }

        function SetCookie(sName, sValue) {
            date = new Date();
            document.cookie = sName + "=" + escape(sValue) + "; expires=Fri, 31 Dec 2099 23:59:59 GMT;";
        }

        function GetCookie(sName) {
            var aCookie = document.cookie.split("; ");
            for (var i = 0; i < aCookie.length; i++) {
                var aCrumb = aCookie[i].split("=");
                if (sName == aCrumb[0]) {
                    if (aCrumb[1])
                        return unescape(aCrumb[1]);
                    else
                        return "";
                }
            }
            return "";
        }

        function Init() {
            GetTopWindow(window);
            if (topWin != window) {
                //防止重新登陆时外框消失
                topWin.location.href = "/portal/LoginBegin.aspx";
                return false;
            }
            else {
                $("#LoginName").val(GetCookie("LoginName"));
                $("#Password").val(GetCookie("Password"));
                if (GetCookie("SaveInfo") == "T") {
                    $("#SaveInfo").attr("checked", "true");
                }
                else {
                    $("#SaveInfo").attr("checked", "false");
                    $("#LoginName").focus();
                }
            }

            //客户端获取计算机的NameBIOS及当前登录用户 windows集成登录使用
            //            var wshNetwork = new ActiveXObject("WScript.Network");
            //            CUserName = wshNetwork.UserName;
            //            CDomain = wshNetwork.UserDomain;
            //            //msgUI(CDomain);debugger;
            //            if (CDomain.toLowerCase() == "smedi") {
            //                domainLogin = true;
            //            OnSubmit();
            //}
        }

        var topWin = null;
        function GetTopWindow(win) {
            if (win.parent != win) {
                GetTopWindow(win.parent);
            }
            if (topWin == null)
                topWin = win;
            else
                return;
        }

        function DestroySelf() {
            window.opener = null;
            window.open('', '_self');
            window.close();
        }
        function LoginSucc(url) {
            window.location = url;
            //            window.open(url ,"BE_MAIN" + Math.round(Math.random() * 1000), "height=" + (window.screen.availHeight - 60) + ", width=" + (window.screen.availWidth - 14) + ", top=0, left=0, menubar=0, location=0, resizable=1, status=1");
            //            setTimeout('DestroySelf();', 100)      
        }

        function GotoChangePwd() {
            var strLogin = $("#LoginName").val();
            window.location.href = "UpdatePwd.aspx?LoginName=" + strLogin;
        }

        function OnSubmit() {
            $("#LoginName").val($("#LoginName").val().trim());
            if ($("#SaveInfo").attr("checked")) {
                SetCookie("LoginName", $("#LoginName").val());
                SetCookie("Password", $("#Password").val());
                SetCookie("SaveInfo", "T");
            }
            else {
                SetCookie("LoginName", "");
                SetCookie("Password", "");
                SetCookie("SaveInfo", "F");
            }

            $.ajax({
                url: window.location,
                type: "post",
                data: { Todo: "Validate", LoginName: $("#LoginName").val(), Password: $("#Password").val(), CDomain: "Local", FromUrl: $("#Password").val() },
                success: function (resp) {
                    var json = mini.decode(resp);
                    if (json.State == "T") {
                        switch ($("#selSystem").val()) {
                            case "document":
                                fromUrl = "/portal/Portal.aspx";
                                break;
                            default:
                                fromUrl = "/portal/Portal.aspx?PortalType=" + $("#selSystem").val();
                                break;
                        }
                        LoginSucc(fromUrl);
                    }
                    else {
                        msgUI(json.Desc);
                    }
                },
                error: function () {
                    msgUI("系统出现异常，请联系系统管理员！");
                }
            });

            //如果是windows集成域登录，则只需要判定用户名，跳过密码认证
            //            if (domainLogin) {
            //                dpname = new DataParam("LoginName", CUserName);
            //                dpCDomain = new DataParam("CDomain", "CDomain");
            //            }
        }

        String.prototype.trim = function () {
            return this.replace(/(^\s*)|(\s*$)/g, "");
        }

        function MM_swapImgRestore() { //v3.0
            var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
        }

        function MM_preloadImages() { //v3.0
            var d = document; if (d.images) {
                if (!d.MM_p) d.MM_p = new Array();
                var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
                    if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; }
            }
        }

        function MM_findObj(n, d) { //v4.01
            var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
                d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
            }
            if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
            for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
            if (!x && d.getElementById) x = d.getElementById(n); return x;
        }

        function MM_nbGroup(event, grpName) { //v6.0
            var i, img, nbArr, args = MM_nbGroup.arguments;
            if (event == "init" && args.length > 2) {
                if ((img = MM_findObj(args[2])) != null && !img.MM_init) {
                    img.MM_init = true; img.MM_up = args[3]; img.MM_dn = img.src;
                    if ((nbArr = document[grpName]) == null) nbArr = document[grpName] = new Array();
                    nbArr[nbArr.length] = img;
                    for (i = 4; i < args.length - 1; i += 2) if ((img = MM_findObj(args[i])) != null) {
                        if (!img.MM_up) img.MM_up = img.src;
                        img.src = img.MM_dn = args[i + 1];
                        nbArr[nbArr.length] = img;
                    }
                }
            } else if (event == "over") {
                document.MM_nbOver = nbArr = new Array();
                for (i = 1; i < args.length - 1; i += 3) if ((img = MM_findObj(args[i])) != null) {
                    if (!img.MM_up) img.MM_up = img.src;
                    img.src = (img.MM_dn && args[i + 2]) ? args[i + 2] : ((args[i + 1]) ? args[i + 1] : img.MM_up);
                    nbArr[nbArr.length] = img;
                }
            } else if (event == "out") {
                for (i = 0; i < document.MM_nbOver.length; i++) {
                    img = document.MM_nbOver[i]; img.src = (img.MM_dn) ? img.MM_dn : img.MM_up;
                }
            } else if (event == "down") {
                nbArr = document[grpName];
                if (nbArr)
                    for (i = 0; i < nbArr.length; i++) { img = nbArr[i]; img.src = img.MM_up; img.MM_dn = 0; }
                document[grpName] = nbArr = new Array();
                for (i = 2; i < args.length - 1; i += 2) if ((img = MM_findObj(args[i])) != null) {
                    if (!img.MM_up) img.MM_up = img.src;
                    img.src = img.MM_dn = (args[i + 1]) ? args[i + 1] : img.MM_up;
                    nbArr[nbArr.length] = img;
                }
            }
        }

        function MM_swapImage() { //v3.0
            var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
                if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
        }

        function EnterKeyDown(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "");
            var keyCode = evt.keyCode ? evt.keyCode : evt.which;
            if (keyCode == 13) {
                evt.returnValue = false;
                evt.cancel = true;
                OnSubmit();
            }
        }
    </script>
</head>
<body>
    <input id="FromUrl" type="hidden" runat="server">
    <input id="type" type="hidden" name="type" runat="server">
    <input id="Hidden1" type="hidden" name="ClientIpMac">

        <div id="login_div">
            <div class="div_top">
                <select id="selSystem" name="" style="display:none">
                <option value="Enterprise">企业门户</option>
                <option value="Person">个人门户</option>
                </select>
            </div>
         <div class="div_input">
          <div class="div_input_bg1"><input id="LoginName" type="text" name="LoginName" onkeydown="return EnterKeyDown(event)" class="div_input_bg" style="outline: none;" /></div>
          <div class="div_input_bg2"><input id="Password" type="password" name="Password" onkeydown="return EnterKeyDown(event)"  class="div_input_bg"/ style="outline: none;"></div>
         </div>
          <div class="div_bt">
            <a href="javascript:;"  onclick="OnSubmit();" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('登录按钮21','','styles/login_bt.gif',1)">
            <img src="styles/login_bt_on.gif" alt="登录按钮" name="登录按钮21" width="54" height="54" border="0" id="登录按钮21" /></a></div>
          <div class="div_foot"><input type="checkbox" name="SaveInfo" id="SaveInfo" />&nbsp;&nbsp;&nbsp; 保留登录信息&nbsp;&nbsp;&nbsp; <a href="javascript:void(0);" onclick="GotoChangePwd()">修改密码</a></div>
        </div>
</body>
</html>
