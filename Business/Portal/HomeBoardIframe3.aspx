<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomeBoardIframe3.aspx.cs"
    Inherits="Portal.HomeBoardIframe3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script language="JScript" src="/CommonWebResource/CoreLib/Combine/MiniPageInc.js"
        type="text/javascript"></script>
    <script src="styles/dropmenu/mt_dropdownC.js" type="text/javascript"></script>
    <script language="JScript" src="/CommonWebResource/Theme/<%=SysColor%>/MiniCssInc.js"
        type="text/javascript"></script>
    <link href="styles/dropmenu/menu/mt_style.css" rel="stylesheet" type="text/css" />
    <link href="styles/dropmenu/style.css" rel="stylesheet" type="text/css" />
</head>
<body style="margin: 0px;">
    <input type="hidden" id="hidUserId" value="<%=UserId %>" />
    <div class="shortcutmenu" style="display: none">
        <ul id="ulShortcut">
        </ul>
    </div>
    <div class="mini-layout" style="width: 100%; height: 100%;">
        <div showheader="false" region="north" style="border: 0;" height="88px" showsplit="false">
            <div id="Div_top">
                <div class="Div_top_left">
                </div>
                <div class="Div_top_bg2">
                    <a href="javascript:;">
                        <img src="styles/dropmenu/icon/shortcut.gif" id="imgShortcut" width="27px" height="27px"
                            title="快捷入口" border="0"></a> <a href="javascript:;">
                                <img onclick="showFeedback()" src="styles/dropmenu/icon/feedback.gif" width="28"
                                    height="28" border="0" title="反馈信息">
                            </a><a href="javascript:;">
                                <img onclick="Refresh()" src="styles/dropmenu/icon/refresh.gif" width="28" height="28"
                                    border="0" title="刷新任务/消息数"></a> <a href="javascript:;">
                                        <img onclick="SetCurHomePage()" src="styles/dropmenu/icon/home.gif" width="28" height="28"
                                            border="0" title="主页"></a> <a href="javascript:;">
                                                <img onclick="GotoDesktop()" src="styles/dropmenu/icon/desktop.gif" width="28" height="28"
                                                    border="0" title="个人桌面"></a> <a href="javascript:;">
                                                        <img onclick="DoRelogin()" src="styles/dropmenu/icon/logon.gif" width="28" height="28"
                                                            border="0" title="注销"></a>
                </div>
                <div class="Div_top_leftline">
                    <img src="styles/dropmenu/nov_toptline.gif" width="2" height="30"></div>
                <div class="Div_top_bg">
                    <img id="imgNewAlarm" title="新提醒" src="styles/treemenu/blue/icon/warning_no.png"
                        width="27" height="27" border="0" align="absmiddle" onclick="MoreAlarm()" style="cursor: pointer"><a
                            href="javascript:;" onclick="MoreAlarm()" title="新提醒" friend="imgNewAlarm"> <span
                                class="portal_toolbar_message" style="color: #000000; font-weight: normal;">提醒&nbsp;<span
                                    id="newAlarm">0</span></a>
                    <img id="imgNewTask" title="新任务" src="styles/treemenu/blue/icon/task_no.png" width="27"
                        height="27" border="0" align="absmiddle" onclick="MoreTask()" style="cursor: pointer"><a
                            href="javascript:;" onclick="MoreTask()" title="新任务" friend="imgNewTask"> <span class="portal_toolbar_message"
                                style="color: #000000; font-weight: normal;">任务&nbsp;<span id="newTask">0</span></a>
                    <img id="imgUnreadMsg" title="新消息" src="styles/treemenu/blue/icon/message_no.png"
                        width="27" height="27" border="0" align="absmiddle" onclick="MoreMsg()" style="cursor: pointer"><a
                            href="javascript:;" onclick="MoreMsg()" title="新消息"><span class="portal_toolbar_message"
                                style="color: #000000; font-weight: normal;"> 消息&nbsp;<span id="newMessage">0</span>
                        </a>
                </div>
                <div class="Div_top_left">
                </div>
            </div>
            <div style="width: 100%;">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td class="ahhy_top_logobg">
                            <img src="styles/dropmenu/logo.jpg" width="349" height="57" alt="logo">
                        </td>
                    </tr>
                    <tr>
                        <td class="ahhy_top_menu_bg">
                            <ul class="menu_top" id="meunInfo" runat="server">
                            </ul>
                    </tr>
                </table>
            </div>
        </div>
        <div region="center" style="border: 0px;">
            <iframe id="frameForm" src="" frameborder="no" scrolling="no" style="width: 100%;
                height: 100%"></iframe>
            <!--Tabs-->
            <div id="mainTabs" class="mini-tabs" activeindex="0" contextmenu="#tabsMenu" style="width: 100%;
                height: 100%; display: none" oncloseclick="onTabCloseClick" onbeforecloseclick="onTabBeforeCloseClick">
            </div>
        </div>
        <div showheader="false" region="south" style="border: 0;" height="25px" showsplit="false">
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ahhy_foot_bg">
                <tr>
                    <td width="61">
                        <img src="styles/dropmenu/hbdl_nov_footleft.gif" width="61" height="25" align="absmiddle">
                    </td>
                    <td width="200" class="ahhy_foot_link">
                        <%=UserName%>（<%=SystemName%>）<%-- | <a href="javascript:;">个人中心</a>--%>
                    </td>
                    <td width="200" valign="middle" class="ahhy_foot_link">
                        <%=CurrentTime.ToShortDateString()%>
                        <%=Weekday %>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td width="165">
                        <img src="styles/dropmenu/km_nov_81.gif" width="165" height="25" alt="jhlogo">
                    </td>
                </tr>
            </table>
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
        if (mtDropDown.isSupported()) {
            var userId = jQuery("#hidUserId").val();
            jQuery.getJSON("AjaxService.aspx?method=GetMenuData&userid=" + userId, function (data) {
                var ms = new mtDropDownSet(mtDropDown.direction.down, 0, 0, mtDropDown.reference.bottomLeft);
                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        BuildMenu(ms, data[i]);
                    }
                }
                mtDropDown.renderAll();
                mtDropDown.initialize();
            });
        }
        $("#frameForm").attr("src", "<% =HomePageUrl%>");
        Refresh(true);
    })

    function Refresh(isFirst) {
        jQuery.getJSON("AjaxService.aspx?method=refresh", function (data) {
            if (!data) return;
            $("#newAlarm").text(data.NewAlarm);
            $("#newTask").text(data.NewTask);
            $("#newMessage").text(data.NewMessage);
            InitTaskArea(data);
        });
        if (isFirst) {
            setTimeout("Refresh(true)", 300000); //5分钟
        }
        //刷新当前页面
        if (isFirst == undefined || isFirst == false) {
            var frame = $("#frameForm");
            if (frame != null) {
                var url = frame.attr("src", url);
                if (url.indexOf("HomeBoard") >= 0) {
                    frame.attr("src", frame.attr("src"));
                } else {
                    if (url.indexOf("MenuBlock") >= 0) {
                        var tabs = frame[0].contentWindow.mini.get("MyTabs");
                        if (tabs != null) {
                            var activeTab = tabs.getActiveTab();
                            if (activeTab != null)
                                tabs.reloadTab(activeTab);
                        }
                    }
                }
            }
        }
    }


    function SetCurHomePage() {
        closeAll();
        window.frameForm.location = "<%=HomePageUrl %>";
    }

    function GotoDesktop() {
        window.parent.switchPortal();
    }

    function MoreAlarm() {
        var url = "/Base/ShortMsg/Alarm/list";
        openWindow(url, { width: "80%", height: "90%", title: "提醒中心" });
    }
    function MoreTask() {
        var url = "/MvcConfig/WorkFlow/Task/MyTaskCenter";
        openWindow(url, { title: "我的任务", width: "80%", height: "80%" });
    }

    function MoreMsg() {
        var url = "/Base/ShortMsg/Msg/Index";
        openWindow(url, { title: "我的消息", width: "80%", height: "85%" });
    }

    function InitTaskArea(data) {
        var cssExist = { 'color': '#f4460a', 'font-weight': 'bolder', 'font-size': '13px' };
        var cssNo = { 'color': '#000000', 'font-weight': 'normal', 'font-size': '12px' };

        if (data.NewAlarm == "0") {
            $("#newAlarm").css(cssNo);
            $("#imgNewAlarm").attr("src", "styles/treemenu/blue/icon/warning_no.png");
        }
        else {
            $("#newAlarm").css(cssExist);
            $("#imgNewAlarm").attr("src", "styles/treemenu/blue/icon/warning_exist.png");
        }

        if (data.NewTask == "0") {
            $("#newTask").css(cssNo);
            $("#imgNewTask").attr("src", "styles/treemenu/blue/icon/task_no.png");
        }
        else {
            $("#newTask").css(cssExist);
            $("#imgNewTask").attr("src", "styles/treemenu/blue/icon/task_exist.png");
        }

        if (data.NewMessage == "0") {
            $("#newMessage").css(cssNo);
            $("#imgUnreadMsg").attr("src", "styles/treemenu/blue/icon/message_no.png");
        }
        else {
            $("#newMessage").css(cssExist);
            $("#imgUnreadMsg").attr("src", "styles/treemenu/blue/icon/message_exist.png");
        }
    }
    function DoRelogin() {
        window.parent.isStop = false;
        window.parent.open("/Portal/LoginBegin.aspx?t=" + Math.random(), "_self");
    }

    function menuclick(name, url, isA, ele) {
        if (isA) {
            $("#meunInfo").find("a").filter(function () { return $(this).attr("amenu") == "T" }).removeClass("current");
            $(ele).addClass("current");
        }
        if (url == null || url == "" || url == "undefined")
            return;
        $("#mainTabs").show();
        $("#frameForm").hide();
        showTab(name, url, true);
    }

    function showTab(name, url, canclose) {
        var tabs = mini.get("mainTabs");
        var id = "tab$" + name;
        var tab = tabs.getTab(name);
        if (!tab) {
            tab = {};
            tab.name = name;
            tab.title = name;
            tab.url = url;
            tab.showCloseButton = canclose;
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

    function onTabCloseClick(e) {
        tabsHide(e.sender);
    }

    var currentTab = null;

    function onBeforeOpen(e) {
        var tabs = mini.get("mainTabs");
        currentTab = tabs.getTabByEvent(e.htmlEvent);
        if (!currentTab) {
            e.cancel = true;
        }
    }

    ///////////////////////////
    function closeTab() {
        var tabs = mini.get("mainTabs");
        tabs.removeTab(currentTab);
        tabsHide(tabs);
    }
    function closeAllBut() {
        var tabs = mini.get("mainTabs");
        tabs.removeAll(currentTab);
        tabsHide(tabs);
    }

    function closeAll() {
        var tabs = mini.get("mainTabs");
        tabs.removeAll();
        tabsHide(tabs);
    }

    function tabsHide(tab) {
        if (tab.tabs.length == 0) {
            ShowHomePage();
        }
    }

    function ShowHomePage() {
        $("#mainTabs").hide();
        $("#frameForm").show();
    }

    //鼠标移入移出按钮图标切换事件
    function MouseChange(obj, e) {
        obj.src = e;
    }

    var scIsHidden = true;
    function initShortcut() {
        bindShortcut();
        $("#imgShortcut").hover(
        function () {
            $(".shortcutmenu").fadeIn();
            scIsHidden = false;
        },
        function () {
            scIsHidden = true;
            setTimeout('if (scIsHidden) {$(".shortcutmenu").fadeOut();}', 200);
        });
        $(".shortcutmenu").hover(
        function () {
            $(".shortcutmenu").fadeIn();
            scIsHidden = false;
        },
        function () {
            scIsHidden = true;
            setTimeout('if (scIsHidden) {$(".shortcutmenu").fadeOut();}', 200);
        });
    }

    function bindShortcut() {
        execute("/Base/PortalBlock/ShortCut/Select", { showLoading: false,
            onComplete: function (data) {
                var $ul = $("#ulShortcut");
                $ul.children().remove();
                $.each(data, function (i, item) {
                    var $left = $("<span></span>").addClass("shortcutmenu_span_left").append($("<a></a>").attr("href", "javascript:void(0);").text(item.Name).click(function () { openShortcut(item.Url, item.Name); }));
                    var $img = $('<img src="styles/treemenu/blue/icon/delete.gif" alt="删除" name="delete" width="20" height="27" border="0" />').mouseout(function () { MouseChange(this, 'styles/treemenu/blue/icon/delete.gif'); }).mouseover(function () { MouseChange(this, 'styles/treemenu/blue/icon/delete2.gif'); });
                    var $right = $("<span></span>").addClass("shortcutmenu_span_right").hide().append($("<a></a>").attr("href", "javascript:void(0);").click(function () { delShortcut(item.ID, item.Name); }).append($img));
                    $ul.append($("<li></li>").mouseover(function () { $(this).addClass("shortcutmenu_onmouse"); $(this).find(".shortcutmenu_span_right").show(); }).mouseout(function () { $(this).removeClass("shortcutmenu_onmouse"); $(this).find(".shortcutmenu_span_right").hide(); }).append($left).append($right));
                });
            }
        });
    }

    $(document).ready(function () {
        initShortcut();
    });

    function openShortcut(url, title) {
        openWindow(url, { title: title, width: "80%", height: "80%" });
        $(".shortcutmenu").hide();
    }

    function delShortcut(id, name) {
        mini.confirm("确认删除" + name + "吗?", "确认", function (action) {
            if (action == "ok") {
                execute("/Base/PortalBlock/ShortCut/Delete?ID=" + id, { showLoading: false,
                    onComplete: function (data) {
                        bindShortcut();
                    }
                });
            }
        });
    }

    function showFeedback() {
        openWindow("/Base/PortalBlock/Feedback/List", { title: "反馈信息", width: "80%", height: "80%" });
    }
</script>
