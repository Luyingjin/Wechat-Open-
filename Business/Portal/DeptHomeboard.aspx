<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeptHomeboard.aspx.cs" Inherits="Portal.DeptHomeboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>部门门户</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="/CommonWebResource/CoreLib/Basic/jQuery/jquery-1.6.2.min.js" type="text/javascript"></script>  
    <script src="/CommonWebResource/CoreLib/MiniUI/miniui.js" type="text/javascript"></script>  
    <script src="/CommonWebResource/CoreLib/MiniUI/baseMiniuiExt.js" type="text/javascript"></script>
    <script src="/MvcConfig/miniuiExt.js" type="text/javascript"></script>           
    <script src="/CommonWebResource/Theme/Default/MiniCssInc.js" type="text/javascript"></script>    
    <link href="styles/dept/style.css" rel="stylesheet" type="text/css" />
</head>
<body style="overflow:hidden">
<div class="mini-splitter" style="width:100%;height:100%;" borderStyle="border:0px" allowResize="false" handlerSize=10>
    <div style="border: 10px solid #e6e6e6;background-color: white;border-right-width:0px;border-bottom-width:0px">
        <div class="mini-splitter" handlerSize=0 allowResize="false" vertical="true" style="width:100%;height:100%;" borderStyle="border:0px">
            <div size="108" showCollapseButton="false" style="border-bottom:10px solid #e6e6e6">
                <div class="mini-splitter" handlerSize=0 allowResize="false" style="width:100%;height:100%;" borderStyle="border:0px">
                    <div showCollapseButton="false" style="border:0px">
                        <div class="dh-pn-right">
                            <div class="dh-pn-float" style="margin-top:0px;">
                            </div>
                        </div>
                    </div>
                    <div showCollapseButton="false" size="25" style="border:0px">
                        <div class="dh-pn-page">
    	                    <div class="dh-pn-pageno dh-pn-pageon" style="display:none">1</div>
                            <div class="dh-pn-pageno " style="display:none">2</div>
                            <div class="dh-pn-pageno " style="display:none">3</div>
                            <div class="dh-pn-pageno " style="display:none">4</div>
                            <div class="dh-pn-pageno " id="newsImageMore" style="display:none">&#8230;</div>
                        </div>
                    </div>
                </div>
            </div>
            <div showCollapseButton="false" style="border:0px">
                <div class="dh-n-header">
                    <div class="dh-n-tab">
        	                <ul>
                            <li class="current">全部</li>
                            <%=HtmlCatalog%>
                            </ul>
                    </div>
                    <div class="dh-n-search">
                            <input id="InformKey" name="InformKey" type="text" class="dh-rs-input" placeholder="部门新闻查询" />
                            <a href="javascript:void(0);" onclick="searchPublicInfo()"></a>
          	        </div>
                </div>
                <div class="mini-fit" style="overflow-x:hidden;">
                    <div class="dh-news">
                    </div>
                </div>
            </div>        
        </div>
    </div>
    <div showCollapseButton="true" size="300" style="background-color:#f1f5f8;border: 10px solid #e6e6e6;border-left-width:0px;border-bottom-width:0px" showHeader="false">
        <div class="dh-r-header">
        <div class="dh-r-search">
            <input name="UserKey" type="text" class="dh-rs-input" id="UserKey" placeholder="部门人员查询" />
            <a href="javascript:void(0);" onclick="searchUser()"></a>
        </div>
        </div>
        <div class="mini-fit" style="overflow-x:hidden">
            <div class="dh-r-user">
                <ul>
                </ul>
            </div>
        </div>
    </div>
</div>
<div class="ay_main" style="display:none">
 	<div class="ay_main_photo"><img src="styles/dept/user_photo.jpg" width="103" /></div>
    <div class="ay_main_right">
    	<div class="ay_right_top">
            <div class="ay_name">
                <%--姓名--%>
            </div>
        </div>
        <div class="ay_info">
        	<ul>
        		<li class="post" style="display:none"><%--职务：行政楼203室--%></li>
                <li class="mobilephoto" style="display:none"><%--手机：13050607000--%></li>
                <li class="phone" style="display:none"><%--分机：021-62822000-2036--%></li>
                <li class="email" style="display:none">邮箱：<a><%--wangdahai@goodwaysoft.com--%></a></li>
             </ul>
        </div>
    </div>
</div>

</body>
</html>
<script type="text/javascript">
    var NewsImageScrollInterval = 5000;

    $(function () {
        init();
    });

    function init() {
        searchPublicInfo();
        searchUser();
        loadNewsImage(getQueryString("DeptHomeID"));
        loadPublicInfo();
        bindPageEvent();
    }

    function bindPageEvent() {
        $("input[name='UserKey']").keydown(function (event) {
            if (event.keyCode == 13) {
                searchUser();
            }
        });
        $("input[name='InformKey']").keydown(function (event) {
            if (event.keyCode == 13) {
                searchPublicInfo();
            }
        });
    }

    var DivImageNewsHeight = 108
    function scrollImageNews(index) {
        $(".dh-pn-page div").removeClass("dh-pn-pageon");
        $(".dh-pn-page div").eq(index).addClass("dh-pn-pageon");
        $(".dh-pn-float").stop().animate({
            'marginTop': -(DivImageNewsHeight * index)
        });
    }

    function loadNewsImage(deptHomeID) {
        addExecuteParam("DeptHomeID", $.trim(deptHomeID));
        execute("/Base/PortalBlock/NewsImage/GetNewsImageGroupByDeptHome", {
            onComplete: function (data) {
                var interval;
                renderInfo(data);
                renderPage(data);
                bindEvent();

                function renderInfo(data) {
                    var $pnfloat = $(".dh-pn-float");
                    if (data.length > 0) {
                        $.each(data, function (i, item) {
                            if (i < 4) {
                                //图片信息
                                var $photo = $("<img>").attr("src", "/Base/PortalBlock/NewsImage/GetPic?ID=" + item.NewsImageID + "&Height=98").attr("height", 98).attr("title", $.trim(item.Title)).data("galleryid", item.ID);
                                var $divPhoto = $("<div></div>").addClass("dh-pn-photo").append($("<li></li>").css({ width: "147px", float: "left" }).append($photo));

                                //新闻信息
                                var $divTB = $("<div></div>").append(getContentTable(item));
                                var $divDesc = $("<div></div>").addClass("dh-pn-content").text($.trim(item.Remark));
                                var $divmainContent = $("<div></div>").addClass("dh-pn-main").append($("<div></div>").addClass("dh-pn-margin").append($divTB).append($divDesc));

                                var $tbNewsImage = createTable([{}], [{ width: 180, children: [$divPhoto] }, { children: [$divmainContent]}]);
                                $pnfloat.append($("<div></div>").addClass("dh-pn-line").append($tbNewsImage));

                                //新闻信息方法
                                function getContentTable(item) {
                                    var $divTitle = $("<div></div>").addClass("dh-pn-title").append($("<a></a>").attr("href", "javascript:void(0)").text($.trim(item.Title)).data("galleryid", item.ID));
                                    var $date = $("<div></div>").addClass("dh-pn-date").text(item.CreateTime.format("yyyy年MM月dd日hh:mm:ss"));
                                    return createTable([{}], [{ children: [$divTitle] }, { width: 150, children: [$date]}]);
                                }
                            }
                        });
                    }
                    else {
                        $pnfloat.append($("<div></div>").addClass("dh-nonews").html("没有新闻"));
                    }
                }

                function renderPage(data) {
                    var num = data.length > 4 ? 5 : data.length;
                    var $pageDivs = $(".dh-pn-page div");
                    $.each($pageDivs, function (i, div) {
                        if (i < num) {
                            $(div).show();
                        }
                        else {
                            return false;
                        }
                    });
                }

                function bindEvent() {
                    var $pageDivs = $(".dh-pn-page div:visible:not(div[id])");
                    if ($pageDivs.length > 1) {
                        interval = setInterval(autoScroll, NewsImageScrollInterval);
                        $.each($pageDivs, function (i, div) {
                            $(div).hover(function (event) {
                                scrollImageNews(i);
                                clearInterval(interval);
                            }, function (event) {
                                interval = setInterval(autoScroll, NewsImageScrollInterval);
                            });

                        });
                        $(".dh-pn-float").hover(function () {
                            clearInterval(interval);
                        }, function () {
                            interval = setInterval(autoScroll, NewsImageScrollInterval);
                        });
                    }
                    if ($("#newsImageMore").is(":visible")) {
                        $("#newsImageMore").click(function () {
                            openWindow("/Base/PortalBlock/NewsImage/Gallerys", {
                                width: "80%",
                                height: "90%",
                                title: "图片新闻库"
                            });
                        });
                    }
                    $(".dh-pn-title a").click(function (event) {
                        var id = $(this).data("galleryid");
                        var title = $(this).text();
                        clickNewsImage(id, title);
                    });
                    $(".dh-pn-photo img").click(function (event) {
                        var id = $(this).data("galleryid");
                        var title = $(this).attr("title");
                        clickNewsImage(id, title);
                    });

                    function autoScroll() {
                        var newsImageCount = $(".dh-pn-page div:visible:not(div[id])").length;
                        var nextIndex = $(".dh-pn-page div.dh-pn-pageon").index() + 1;
                        if (newsImageCount == nextIndex)
                            nextIndex = 0;
                        scrollImageNews(nextIndex);
                    }

                    function clickNewsImage(id, title) {
                        openWindow("/base/PortalBlock/NewsImage/Gallery?ID=" + id, {
                            width: "100%",
                            height: "100%",
                            title: title,
                            showMaxButton: false
                        });
                    }
                }
            }
        });
    }

    function createTable(TRs, TDs) {
        var $tb = $("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"></table>");
        $.each(TRs, function (i, tr) {
            var $tr = $("<tr></tr>");
            $.each(TDs, function (i, td) {
                var $td = $("<td></td>");
                if (typeof (td["width"]) != "undefined")
                    $td.attr("width", td["width"]);
                if (typeof (td["children"]) == "object") {
                    $.each(td["children"], function (i, child) {
                        $td.append(child);
                    });
                }
                $tr.append($td);
            });
            $tb.append($tr);
        });
        return $tb;
    }

    function searchPublicInfo() {
        var $catalog = $(".dh-n-tab li.current");
        renderPublicInfo(getQueryString("DeptHomeID"), $catalog.attr("catalogID"), $("#InformKey").val());
    }

    function searchUser() {
        loadUser(getQueryString("DeptHomeID"), $("#UserKey").val());
    }

    function loadPublicInfo() {
        $(".dh-n-tab li").click(function () {
            $(".dh-n-tab li").removeClass("current");
            $(this).addClass("current");
            renderPublicInfo(getQueryString("DeptHomeID"), $(this).attr("catalogID"), $("#InformKey").val());
        });
    }

    var deptUsersData;
    function loadUser(deptHomeID, key) {
        if (typeof (deptUsersData) == "undefined") {
            addExecuteParam("Key", $.trim(key));
            addExecuteParam("NodeFullID", $.trim(deptHomeID));
            execute("/Base/Auth/OrgUser/GetRelationList", {
                onComplete: function (data) {
                    if (!data || typeof (data) == "string" || typeof (data) == "undefined")
                        data = [];
                    else if (typeof (data) == "object")
                        data = data["data"];
                    deptUsersData = data;
                    renderUserInfo(data);
                    bindEvent();
                }
            });
        }
        else {
            var filterData = [];
            $.each(deptUsersData, function (i, userData) {
                if ($.trim(userData["Name"]).indexOf(key) > -1 || $.trim(userData["WorkNo"]).indexOf(key) > -1 || $.trim(userData["OrgRoleName"]).indexOf(key) > -1) {
                    filterData.push(userData);
                }
            });
            renderUserInfo(filterData);
            bindEvent();
        }

        function renderUserInfo(infos) {
            var $user = $(".dh-r-user ul:first");
            $user.children().remove();
            $.each(infos, function (i, info) {
                var $li = $("<li></li>").data("user", info);
                var sex = $.trim(info.Sex) == "" ? "male" : info.Sex;
                $li.append($("<img>").attr("src", "styles/dept/" + sex + ".png").width(41).height(41));
                $li.append($("<span></span>").addClass("name").text(info.Name));
                var phone = $.trim(info.MobilePhone);
                if (phone == "" && $.trim(info.Phone) != "")
                    phone = "分机：" + $.trim(info.Phone);
                $li.append($("<h6></h6>").text(phone));
                $li.append($("<span></span>").addClass("role").attr("title", info.OrgRoleName).text(info.OrgRoleName));
                $user.append($li);
            });

        }

        function bindEvent() {
            var timeout, postCardHidden = false;
            $(".dh-r-user li").hover(function () {
                //载入详细信息
                loadUserCard(this);

                function loadUserCard(dom) {
                    var user = $(dom).data("user");
                    if ($.trim(user.UserImgID) != "") {
                        $(".ay_main .ay_main_photo").find("img").attr("src", "/MvcConfig/Image/GetUserPic?UserId=" + user.ID);
                    }
                    else {
                        $(".ay_main .ay_main_photo").find("img").attr("src", "styles/dept/user_photo.jpg");
                    }
                    $(".ay_main").find(".ay_name").html(user.Name);
                    if ($.trim(user.Roles) != "")
                        $(".ay_main .post").text("职务：" + user.Roles).show();
                    else
                        $(".ay_main .post").hide();
                    if ($.trim(user.MobilePhone) != "")
                        $(".ay_main .mobilephoto").text("手机：" + user.MobilePhone).show();
                    else
                        $(".ay_main .mobilephoto").hide();
                    if ($.trim(user.Phone) != "")
                        $(".ay_main .phone").text("分机：" + user.Phone).show();
                    else
                        $(".ay_main .phone").hide();
                    if ($.trim(user.Email) != "") {
                        $(".ay_main .email a").attr("href", "mailto:" + user.Email).text(user.Email).attr("title", user.Email);
                        $(".ay_main .email").show();
                    }
                    else {
                        $(".ay_main .email").hide();
                    }
                    var top = $(dom).offset().top - 2;
                    var left = $(dom).offset().left - $(".ay_main").width() - 20;
                    if ((top + $(".ay_main").height()) > $("body").height()) {
                        top = $(dom).offset().top - 2 - ($(".ay_main").height() - $(dom).height());
                    }
                    $(".ay_main").css({ "top": top, "left": left });

                    timeout = setTimeout(function () {
                        $(".ay_main").stop().show();
                    }, 500);
                }
            }, function () {
                clearTimeout(timeout);
            });

            $(".dh-r-user").hover(function (event) {
                postCardShow(event.target);
            }, function (event) {
                postCardHide(event.target);
            });

            $(".ay_main").hover(function (event) {
                postCardShow(event.target);
            }, function (event) {
                postCardHide(event.target);
            });

            function postCardShow(dom) {
                postCardHidden = false;
                $(dom).fadeIn();
            }

            function postCardHide(dom) {
                postCardHidden = true;
                setTimeout(function () {
                    if (postCardHidden) {
                        $(".ay_main").fadeOut(); 
                    }
                }, 200);
            }
        }
    }

    function renderPublicInfo(deptHomeID, catalogID, key) {
        addExecuteParam("DeptHomeID", $.trim(deptHomeID));
        addExecuteParam("CatalogID", $.trim(catalogID));
        addExecuteParam("Key", $.trim(key));
        execute("/Base/PortalBlock/PublicInformation/GetListByDeptHome", {
            onComplete: function (data) {
                render(data);
            }
        });

        function render(infos) {
            var $news = $(".dh-news");
            $news.children().remove();
            if (infos.length > 0) {
                $.each(infos, function (i, info) {
                    var $divInfo = getDivPublicInfo(info);
                    $news.append($divInfo);

                    function getDivPublicInfo(info) {
                        var $tr = $('<tr></tr>');
                        var htmlTitle = $.trim(info["IsTop"]) == "1" ? "<b>" + info.Title + "</b>" : info.Title;
                        var $title = $("<div></div>").addClass("dh-n-title").append($("<a></a>").attr("href", "javascript:void(0);").html(htmlTitle).click(function () {
                            openWindow('/Base/PortalBlock/PublicInformation/Views?ID=' + info.ID, {
                                width: '60%', height: '85%', title: info.Title
                            });
                        }));
                        var $td1 = $("<td></td>").append($title);
                        var $td2 = $("<td></td>").width(150).append($("<div></div>").addClass("dh-n-date").text(info.CreateTime.format("yyyy年MM月dd日hh:mm:ss")));
                        $tr.append($td1);
                        $tr.append($td2);
                        var $tbody = $('<tbody></tbody>').append($tr);
                        var $div1 = $("<div></div>").append($('<table width="100%" border="0" cellSpacing="0" cellPadding="0"></table>').append($tbody));
                        var htmlContent = $.trim(info["IsTop"]) == "1" ? "<b>" + info.ContentText + "</b>" : info.ContentText;
                        var $div2 = $("<div></div>").addClass("dh-n-content").html(htmlContent);
                        var $div = $("<div></div>").addClass("dh-n-line").append($("<div></div>").addClass("dh-n-margin").append($div1).append($div2));
                        return $div;
                    }
                });
            }
            else {
                $news.append($("<div></div>").addClass("dh-nonews").html("没有新闻"));
            }
        }
    }

    Date.prototype.format = function (format) {
        /* 
        * eg:format="yyyy-MM-dd hh:mm:ss"; 
        */
        var o = {
            "M+": this.getMonth() + 1, // month  
            "d+": this.getDate(), // day  
            "h+": this.getHours(), // hour  
            "m+": this.getMinutes(), // minute  
            "s+": this.getSeconds(), // second  
            "q+": Math.floor((this.getMonth() + 3) / 3), // quarter  
            "S": this.getMilliseconds()
            // millisecond  
        }

        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4
                        - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1
                            ? o[k]
                            : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    }       
</script>
