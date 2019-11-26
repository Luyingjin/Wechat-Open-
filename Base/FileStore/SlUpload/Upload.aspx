<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="FileStore.SlUpload.Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>文件上传</title>

    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
	    overflow: hidden;
    }
    #silverlightControlHost {
	    height: 100%;
    }
    </style>
    <script src="/CommonWebResource/CoreLib/Basic/jQuery/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="/CommonWebResource/CoreLib/MiniUI/miniui.js" type="text/javascript"></script>
    <script type="text/javascript">

        function onSilverlightError(sender, args) {

            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            var errMsg = "Unhandled Error in Silverlight 2 Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

        // 上传完毕后操作,在Silverlight里回调使用
        function OnComplete(fstr) {
            if (window.Owner) {
                CloseWindow(fstr);
            }
            else {
                window.returnValue = fstr;
                window.close();
            }
        }

        // 取消上传
        function OnCancel() {
            CloseWindow("");
        }

        // 退出上传
        function DoExit() {
            CloseWindow("");
        }

        // 检查上传
        var filterExts = "<%=FilterExts%>".toLowerCase();
        function OnCheck(fileNames) {
            if (fileNames && fileNames != "") {
                var arrFiles = fileNames.split("\|");
                for (var i = 0; i < arrFiles.length; i++) {
                    var fileSplit = arrFiles[i].split("\.");
                    if (fileSplit.length == 1 || filterExts.indexOf(fileSplit[fileSplit.length - 1].toLowerCase()) > -1) {
                        alert("上传文件的格式非法！请确认！\r\n禁止清单：" + filterExts);
                        return false;
                    }
                }
                return true;
            }
            else {
                alert("请先选择要上传的文件！");
                return false;
            }
        }

        function CloseWindow(fstr) {
            if (window.CloseOwnerWindow) {
                if (fstr != "close")
                    window.CloseOwnerWindow(fstr);
            }
            else
                window.close();
        }
    </script>
</head>

<body>
    <!-- Runtime errors from Silverlight will be displayed here.
	This will contain debugging information and should be removed or hidden when debugging is completed -->
	<div id='errorLocation' style="font-size: small;color: Gray;"></div>

    <div id="silverlightControlHost" >
		<object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%" VIEWASTEXT>
			<param name="source" value="./FileUpload.xap" />
			<param name="onerror" value="onSilverlightError" />
			<param name="background" value="white" />
			<param name="minRuntimeVersion" value="2.0.31005.0" />
			<param name="autoUpgrade" value="true" />
			<param name="InitParams" value="<%=InitParams%>" />
			<a href="/portal/install/Silverlight.exe" style="text-decoration: none;">
     			<img src="/portal/install/ico/Logo_SL.png" alt="获取Silverlight控件" style="border-style: none"/>
			</a>
		</object>
		<iframe style='visibility:hidden;height:0;width:0;border:0px'></iframe>
    </div>   
</body>
</html>