<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomeBoardIframe.aspx.cs"
    Inherits="Portal.HomeBoardIframe" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>综合管理信息系统</title>
    <meta http-equiv="pragma" content="no-cache">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="styles/treemenu/<%=SysColor%>/style.css" rel="stylesheet" type="text/css">
    <script language="JScript" src="/CommonWebResource/CoreLib/Combine/SimplePageInc.js"
        type="text/javascript"></script>
    <script language="JScript" src="/CommonWebResource/Theme/<%=(SysColor=="blue"?"Default":"Vista")%>/MiniCssInc.js"
        type="text/javascript"></script>
    <style type="text/css">
        html {
            overflow: hidden;
        }
    </style>
</head>
<body style="margin: 0px;" scroll="no">
    <div class="shortcutmenu" style="display: none">
        <ul id="ulShortcut">
        </ul>
    </div>
    <div id="Div_top">
        <div class="Div_top_bg2">
            <a href="javascript:;"><img title="反馈信息" src="styles/treemenu/<%=SysColor%>/icon/feedback.gif"
                            onclick="showFeedback()" onmouseover="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/feedback2.gif')"
                            onmouseout="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/feedback.gif')" width="27px"
                            height="27px" border="0"><img onclick="Refresh()" title="刷新当前页"
                                src="styles/treemenu/<%=SysColor%>/icon/refresh.gif" width="27px" height="27px" border="0"
                                onmouseover="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/refresh2.gif')" onmouseout="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/refresh.gif')"></a><a
                                    href="javascript:;"><img onclick="SetCurHomePage()" title="主页" src="styles/treemenu/<%=SysColor%>/icon/home.gif"
                                        width="27px" height="27px" border="0" onmouseover="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/home2.gif')"
                                        onmouseout="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/home.gif')"></a><a
                                                href="javascript:;"><img onclick="DoRelogin()" title="注销" src="styles/treemenu/<%=SysColor%>/icon/logon.gif"
                                                    onmouseover="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/logon2.gif')" onmouseout="MouseChange(this,'styles/treemenu/<%=SysColor%>/icon/logon.gif')"
                                                    width="27px" height="27px" border="0"></a>
        </div>
    </div>
    <div id="divTop" style="width: 100%;">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td class="ahhy_top_logobg">
                    <img src="styles/treemenu/<%=SysColor%>/logo.gif" width="399px" height="50px" alt="logo">
                </td>
            </tr>
            <tr>
                <td class="ahhy_top_menu_bg">
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="table-layout: fixed;">
                        <tr>
                            <td width="16px" name="scrollTD" style="display: none">
                                <a href="javascript:;">
                                    <img onclick="RadMenuScroll('left')" style="cursor: hand;" src="styles/treemenu/<%=SysColor%>/bt_left.gif"
                                        width="16px" height="16px" border="0" /></a>
                            </td>
                            <td>
                                <div id="MenumBar" style="overflow: hidden; width: 100%; position: relative">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr class="menu_top">
                                            <%=menuhtml %>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td width="15px" name="scrollTD" style="display: none">
                                <a href="javascript:;">
                                    <img onclick="RadMenuScroll('right')" style="cursor: hand;" src="styles/treemenu/<%=SysColor%>/bt_right.gif"
                                        width="16" height="16" border="0" /></a>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div id="divFrameForm">
        <iframe id="frameForm" src="" frameborder="no" onload="iFrameHeight()"
            scrolling="no" style="width: 100%;" height="1500px"></iframe>
        <div class="dh-footmenu-block" style="display: none">
        </div>
    </div>
    <div style="width: 100%;" id="divFoot">
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="hbdl_foot_bg">
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td width="15">&nbsp;
                            </td>
                            <td width="260">
                                <%--<nobr> 欢迎您使用上海金慧软件有限公司的产品</nobr>--%>
                                <nobr> 欢迎您使用上海众引传播有限公司的产品</nobr>
                            </td>
                            <td align="right" valign="middle">
                                <img src="styles/treemenu/<%=SysColor%>/icon/user.gif" width="16" height="16" align="absmiddle" />
                                <%=UserName%>(<%=SystemName%>) &nbsp;&nbsp;
                            </td>
                            <td width="150">&nbsp;
                                <img src="styles/treemenu/<%=SysColor%>/icon/time.gif" width="16" height="16" align="absmiddle" />
                                <%=CurrentTime.ToShortDateString()%>
                                &nbsp;
                                <%=Weekday
                                %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
<script type="text/javascript">

    $(document).ready(function () {
        var frame = $("#frameForm");
        //frame.attr("src","<%=HomePageUrl %>");
        iFrameHeight();
        initShortcut();
        //scrollImages();
        Refresh(true);
        //initDeptHomeMenu();

        $(window).resize(iFrameHeight);
        $(window).resize(scrollImages);
    });
    
    function RadMenuScroll(flag) {
        if (flag && flag == "right") {
            jQuery("#MenumBar").animate({
                scrollLeft: "+=200"
            })
        }
        else {
            jQuery("#MenumBar").animate({
                scrollLeft: "-=200"
            })
        }
    }

    function CenterStr() {
        var width = window.screen.availWidth;
        var height = window.screen.availHeight;
        var left = (window.screen.width - width) / 2;
        var top = (window.screen.height - height) / 2;
        return "left=" + left + ",top=" + top + ",width=" + width + ",height=" + height + ",scrollbars=yes";
    }

    function DoRelogin() {
        window.parent.isStop = false;
        window.parent.open("/Portal/LoginBegin.aspx?t=" + Math.random(), "_self");
    }

    function InitTaskArea(data) {
        var cssExist = { 'color': '#f4460a', 'font-weight': 'bolder', 'font-size': '13px' };
        var cssNo = { 'color': '#000000', 'font-weight': 'normal', 'font-size': '12px' };

        if (data.NewAlarm == "0") {
            $("#newAlarm").css(cssNo);
            $("#imgNewAlarm").attr("src", "styles/treemenu/<%=SysColor%>/icon/warning_no.png");
        }
        else {
            $("#newAlarm").css(cssExist);
            $("#imgNewAlarm").attr("src", "styles/treemenu/<%=SysColor%>/icon/warning_exist.png");
        }

        if (data.NewTask == "0") {
            $("#newTask").css(cssNo);
            $("#imgNewTask").attr("src", "styles/treemenu/<%=SysColor%>/icon/task_no.png");
        }
        else {
            $("#newTask").css(cssExist);
            $("#imgNewTask").attr("src", "styles/treemenu/<%=SysColor%>/icon/task_exist.png");
        }

        if (data.NewMessage == "0") {
            $("#newMessage").css(cssNo);
            $("#imgUnreadMsg").attr("src", "styles/treemenu/<%=SysColor%>/icon/message_no.png");
        }
        else {
            $("#newMessage").css(cssExist);
            $("#imgUnreadMsg").attr("src", "styles/treemenu/<%=SysColor%>/icon/message_exist.png");
        }
    }

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
        if(isFirst ==undefined || isFirst ==false){
            var frame = $("#frameForm");
            if(frame !=null){
                var url = frame.attr("src",url);
                if(url.indexOf("HomeBoard") >=0){
                    frame.attr("src", frame.attr("src"));
                }else{
                    if(url.indexOf("MenuBlock")>=0){
                        var tabs = frame[0].contentWindow.mini.get("MyTabs");
                        if(tabs != null){
                            var activeTab = tabs.getActiveTab();
                            if(activeTab !=null)
                                tabs.reloadTab(activeTab);
                        }
                    }
                }
            }
        }
    }

    function SetCurHomePage() {
        //window.frameForm.location = "MenuBlock.aspx?Height=" + screen.availHeight;
        if (<%=DeptHomeEnabled %>) {
            reloadDeptHomeMenu(true);
        $(".dh-fm-left li:eq(0)").addClass("current");
    }
    $("#frameForm")[0].src = "<%=HomePageUrl %>";
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

    //点击菜单
    var tempEle;
    function onitemclick(ele, value, url, childCount) {
        if (childCount && childCount == "0")
            $("#frameForm").attr("src", url);
        else
            $("#frameForm").attr("src", "MenuBlock.aspx?ID=" + value + "&Height=" + (document.documentElement.offsetHeight - 122));

        jQuery(ele).addClass("current");
        if (tempEle != null) {
            $(tempEle).removeClass("current");
        }
        tempEle = ele;
        reloadDeptHomeMenu();
    }

    //iFrame高度自适应
    function iFrameHeight() {
        var ifm = document.getElementById("frameForm");
        var subWeb = document.frames ? document.frames["frameForm"].document : ifm.contentDocument;
        if (ifm != null && subWeb != null) {
            ifm.height = document.documentElement.offsetHeight - 112;
            if ($(".dh-footmenu-block").is(":visible"))
                ifm.height = ifm.height - $(".dh-footmenu-block").height();
        }
    }

    var DivFootBarHeight = 28;
    function initDeptHomeMenu() {
        if (<%=DeptHomeEnabled %>) {
            var $DeptMenu = $(".dh-footmenu");
        $DeptMenu.data("initbottom", $DeptMenu.css("bottom"));
        bindMenuEvent();
        $DeptMenu.show();
    }
    }

    function bindMenuEvent() {
        var $imgArrow = $(".dh-footmenu-uparrow img");
        $imgArrow.bind("click", function (event) {
            var _self = event.target;
            var imgSrc = $(_self).attr("src");
            if (imgSrc.lastIndexOf("upbutton") > -1) {
                deptMenuSlideUp();
            }
            else {
                deptMenuSlideDown();
            }
        });

        $(".dh-fm-left li[DeptHomeID]").bind("click", function (event) {
            leftMenuClick($(this));
        });

        $(".dh-moremain li").bind("click", function (event) {
            var deptHomeID = $(this).attr("DeptHomeID");
            leftMenuClick($(".dh-fm-left li[DeptHomeID='" + deptHomeID + "']"));
        });

        $(".dh-fm-more").bind("click", function () {
            if (!$(this).hasClass("current")) {
                $(this).addClass("current");
                $(".dh-moremain").show("fast");
            }
            else {
                $(this).removeClass("current");
                $(".dh-moremain").hide("fast");
            }
        });

        function leftMenuClick($dom) {
            $(".dh-footmenu-uparrow").hide();
            $(".dh-fm-left li").removeClass("current");
            $dom.addClass("current");
            $(".dh-moremain li").removeClass("current");
            $(".dh-moremain li[DeptHomeID='" + $dom.attr("DeptHomeID") + "']").addClass("current");

            if ($(".dh-footmenu-block").is(":hidden")) {
                $(".dh-footmenu-block").show();
            }
            $(".dh-moremain").hide("fast");
            $(".dh-fm-more").removeClass("current");
            $("#frameForm").attr("src", "DeptHomeboard.aspx?DeptHomeID=" + $dom.attr("DeptHomeID"));
        }
    }

    function deptMenuSlideUp() {
        var $imgArrow = $(".dh-footmenu-uparrow img");
        $(".dh-footmenu").animate({ bottom: DivFootBarHeight }, 'fast', function () {
            $imgArrow.attr("src", $imgArrow.attr("src").replace("upbutton", "downbutton"));
        });
    }

    function deptMenuSlideDown() {
        var initBottom = $(".dh-footmenu").data("initbottom");
        var $imgArrow = $(".dh-footmenu-uparrow img");
        if ($(".dh-fm-more").hasClass("current")) {
            $(".dh-fm-more").removeClass("current");
            $(".dh-moremain").hide("fast");
        }
        $(".dh-fm-left li[DeptHomeID]").removeClass("current");
        $(".dh-moremain li").removeClass("current");
        $(".dh-footmenu").animate({ bottom: initBottom }, 'fast', function () {
            $imgArrow.attr("src", $imgArrow.attr("src").replace("downbutton", "upbutton"));
        });

    }

    function reloadDeptHomeMenu(isHome) {
        var $menu = $(".dh-footmenu");
        if (isHome) {
            $menu.show();
        }
        else {
            $menu.hide();
        }
        if ($(".dh-footmenu-uparrow img").attr("src").indexOf("downbutton") > -1) {
            deptMenuSlideDown();
            $(".dh-footmenu-block").hide();
            iFrameHeight();
            $(".dh-footmenu-uparrow").show();
        }
    }

    function scrollImages() {
        var barW = jQuery("#MenumBar").width();
        var tableW = jQuery(".menu_top").width();
        var scrollTDs = jQuery(".ahhy_top_menu_bg").find("td[name='scrollTD']");
        if (tableW > barW)
            scrollTDs.show();
        else
            scrollTDs.hide();
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
                    var $img = $('<img src="styles/treemenu/<%=SysColor%>/icon/delete.gif" alt="删除" name="delete" width="20" height="27" border="0" />').mouseout(function () { MouseChange(this, 'styles/treemenu/<%=SysColor%>/icon/delete.gif'); }).mouseover(function () { MouseChange(this, 'styles/treemenu/<%=SysColor%>/icon/delete2.gif'); });
                    var $right = $("<span></span>").addClass("shortcutmenu_span_right").hide().append($("<a></a>").attr("href", "javascript:void(0);").click(function () { delShortcut(item.ID, item.Name); }).append($img));
                    $ul.append($("<li></li>").mouseover(function () { $(this).addClass("shortcutmenu_onmouse"); $(this).find(".shortcutmenu_span_right").show(); }).mouseout(function () { $(this).removeClass("shortcutmenu_onmouse"); $(this).find(".shortcutmenu_span_right").hide(); }).append($left).append($right));
                });
            }
        });
    }

    function openShortcut(url, title) {
        openWindow(url, { title: title, width: "80%", height: "80%" });
        $(".shortcutmenu").hide();
    }

    function delShortcut(id, name) {
        msgUI("确认删除" + name + "吗?", 2, function (action) {
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
        openWindow("/Base/PortalBlock/Feedback/MyFeedbackList", { title: "反馈信息", width: "80%", height: "80%" });
    }

</script>
