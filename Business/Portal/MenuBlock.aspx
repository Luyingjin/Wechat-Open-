<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuBlock.aspx.cs" Inherits="Portal.MenuBlock" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" >
<html>
<head>
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <script language="JScript" src="/CommonWebResource/CoreLib/Combine/MiniPageInc.js" type="text/javascript"></script>
    <script language="JScript" src="/CommonWebResource/Theme/<%=SysColor%>/MiniCssInc.js" type="text/javascript"></script>
    <style id="icons" runat="server"></style>
</head>
<body>
<div  class="mini-layout" style="width:100%;height:100%;">
    <div title="<%=SubTitle %>" region="west" width="250" showSplitIcon="true">
        <!--Tree-->
        <ul id="leftTree" class="mini-tree" url="AjaxService.aspx?method=getsubtree&id=<%=SubID %>&userid=<%=UserId %>" style="width:100%;height:100%;" 
           showTreeIcon="true" textField="text" onnodeclick="onNodeSelect" idfield="id" parentfield="pid" resultastree="false" expandonload="true">        
        </ul>
    </div> 
    <div title="center" region="center" style="border:0px;">
        <div id="MyTabs" class="mini-tabs" contextMenu="#tabsMenu" style="width:100%;height:100%;display:none" onbeforecloseclick="onTabBeforeCloseClick">
        </div>
        <iframe id="mainiframe" style="height:100%;width:100%;border:0px" frameborder="0px" src=""></iframe>
    </div>     

</div>
<ul id="tabsMenu" class="mini-contextmenu" onbeforeopen="onBeforeOpen">        
    <li onclick="closeTab">关闭标签页</li>                
	<li onclick="closeAllBut">关闭其他标签页</li>
	<li onclick="closeAll">关闭所有标签页</li>        
</ul>                                              

</body>
</html>
<script type="text/javascript">

    $(document).ready(function () {

        $("#mainiframe").attr("src", "<%=SubHomeSrc%>");
    })

    function onNodeSelect(e) {
        $("#MyTabs").show();
        $("#mainiframe").hide();
        var node = e.node;
        var tabs = mini.get("MyTabs");
        var tab = tabs.getTab(node.id);
        if (node.url.trim() == "" || node.url.trim() == "about:blank") return;
        if (!tab) {
            var tab = {};
            tab.name = node.id;
            tab.title = node.text;
            tab.url = node.url;
            tab.showCloseButton = true;
            tab.onload = function () {
                var iframe = this.getTabIFrameEl(this.getActiveTab());
                if ($.trim(node.text) != "") {
                    $(iframe.contentWindow.document).attr("title", node.text);
                }
            };
            if (tabs.getTabs().length >= 5)//最多5个
                tabs.removeTab(0);
            tabs.addTab(tab);
        }
        if (tab) {
            tabs.activeTab(tab);
            tabs.reloadTab(tab);
        }
    }

    function onTabBeforeCloseClick(e) {
        var tabs = e.sender;
        var ifm = tabs.getTabIFrameEl(e.tab);
        destroyIframeMemory(ifm);
    }

    function onTabDoubleClick() {
        var ele = event.srcElement;
        if (ele) {
            var index = $(ele).attr("index");
            if (index) {
                var tabs = mini.get("MyTabs");
                if (tabs) {
                    tabs.removeTab(index);
                }
            }
        }
    }

    var currentTab = null;

    function onBeforeOpen(e) {
        var tabs = mini.get("MyTabs");
        currentTab = tabs.getTabByEvent(e.htmlEvent);
        if (!currentTab) {
            e.cancel = true;
        }
    }

    ///////////////////////////
    function closeTab() {
        var tabs = mini.get("MyTabs");
        tabs.removeTab(currentTab);
    }
    function closeAllBut() {
        var tabs = mini.get("MyTabs");
        tabs.removeAll(currentTab);
    }

    function closeAll() {
        var tabs = mini.get("MyTabs");
        tabs.removeAll();
    }


</script>


