<%@ Page Language="c#" CodeBehind="HomeBoard.aspx.cs" AutoEventWireup="true" Inherits="Portal.Door.HomeBoard" %>
<%@ OutputCache Location="none" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>门户块框架</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
     <link href="styles/treemenu/<%=SysColor%>/style.css" rel="stylesheet" type="text/css">
    <script language="JScript" src="/CommonWebResource/CoreLib/Combine/SimplePageInc.js" type="text/javascript"></script>
    <script src="door/js/prototype.js" type="text/javascript"></script>
    <script src="door/js/follow.js" type="text/javascript"></script>
    <script src="door/js/drag.js" type="text/javascript"></script>
    <link href="door/css/door.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        html
        {
            padding:0;border:0;margin:0;
            width:100%;height:100%;   
        }
        body
        {
            font-family:Tahoma, Verdana, 宋体;   
            font-size:12px;
            line-height:22px;
        }
    </style>
    <script language="JScript" type="text/javascript">
        var _CurrentUserCode = "<%=userInfo.Code%>";
        var _params = { TemplateId: "<%=TemplateID%>", BlockType: "<%=BaseType%>" };
        var _blockType = "<%=BaseType%>";
        var _templateId = "<%=TemplateID %>";

        jQuery(function () {
            resizeContent();
        });

        jQuery(window).resize(function () {
            resizeContent();
        });

        function resizeContent() {
            jQuery("#Divlayout").height(jQuery("body").height() - jQuery("#divBtn").height() - 2);
        }

        //增加元素
        function addBlock(blocktype, blocktitle) {
            var eventImg = event.target || event.srcElement;
            var blockIds = getAllDragDiv();
            for (var i = 0; i < blockIds.length; i++) {
                if (blockIds[i] == blocktype) {
                    alert("该模块已经添加,无须重复添加!"); return;
                }
            }
            var layouttype = jQuery('#layouttype').val();
            var queryParam = jQuery.extend({}, { Param: "GetOneNew", BlockId: blocktype }, _params);
            jQuery.ajax({
                url: "door/BlockData.aspx",
                type: "post",
                data: queryParam,
                success: function (resp) {
                    var objBlock = eval("(" + resp + ")");
                    jQuery(eventImg).attr("src", "/Portal/Door/image/added.gif").attr("title", "已添加");
                    var colnum = jQuery('#layouttype').val();
                    var objCol = document.getElementById("col_" + colnum);
                    objCol.innerHTML += objBlock["Html"];
                    initDrag();
                    if (objBlock["DelayLoadSecond"] == 0)
                        loadDragContent(blocktype, objBlock["RepeatItemCount"]);
                    else
                        setTimeout(function () { loadDragContent(blocktype, objBlock["RepeatItemCount"]); }, objBlock["DelayLoadSecond"] * 1000);
                    loadScriptText(blocktype, objBlock["RelateScript"]);
                },
                error: function () {
                    alert("系统出现异常，请联系系统管理员！");
                }
            });
        }

        function loadScriptText(id, scriptText) {
            if (!document.getElementById(id)) {
                var script = document.createElement('script');
                script.id = id;
                script.type = 'text/javascript';
                try {
                    script.appendChild(document.createTextNode(scriptText));
                } catch (e) {
                    script.text = scriptText; //ie9以下
                }
                document.body.appendChild(script);
            }
        }

        //删除元素
        function delDragDiv(blockid) {
            Element.hide('popupImgMenuID');
            var logicv = window.confirm("确定删除这个元素吗？");
            if (logicv) {
                var rid = document.getElementById("drag_" + blockid);
                rid.parentNode.removeChild(rid);
                jQuery.ajax({ url: "door/BlockData.aspx", type: "post", data: jQuery.extend({}, { Param: "DeleteBlock", BlockId: blockid }, _params),
                    success: function (resp) {
                        jQuery(".paneladdimg[blockID='" + blockid + "']").attr("src", "/Portal/Door/image/add.gif").attr("title", "添加"); ;
                    }
                });

            }
        }

        function reset() {
            if (confirm("确定要恢复默认吗?")) {
                jQuery.ajax({
                    url: "door/BlockData.aspx",
                    type: "post",
                    data: jQuery.extend({}, { Param: "Reset" }, _params),
                    success: function (resp) {
                        window.location.reload();
                    },
                    error: function () {
                        alert("系统出现异常，请联系系统管理员！");
                    }
                });
            }
        }

        //展开隐藏元素
        function switchDrag(tid, imgid) {
            var openurl = "/Portal/Door/image/close.gif";
            var closeurl = "/Portal/Door/image/open.gif";
            showHiddenInfo(tid, imgid, openurl, closeurl);

        }

        //展开隐藏所有元素
        function switchDragAll(imgid) {
            var openurl = "/Portal/Door/image/close.gif";
            var closeurl = "/Portal/Door/image/open.gif";

            var objSS = document.getElementById('showstatus');
            var n = objSS.value;
            if (n == 1) {
                objSS.value = 0;
                jQuery(imgid).attr("src", openurl);
            }
            else {
                objSS.value = 1;
                jQuery(imgid).attr("src", closeurl);
            }
            var aryBlockId = [];
            aryBlockId = getAllDragDiv();
            var blockid = 0;
            for (var i = 0; i < aryBlockId.length; i++) {
                blockid = aryBlockId[i];
                var objDragSwitch = document.getElementById('drag_switch_' + blockid);

                if (n == 1) {
                    hiddenInfo(objDragSwitch, "drag_switch_img_" + blockid, openurl);
                }
                else {
                    showInfo(objDragSwitch, "drag_switch_img_" + blockid, closeurl);
                }
            }

        }

        //取所有拖动元素的blockid
        function getAllDragDiv() {
            var odjLT = document.getElementById('layouttype');
            var m = odjLT.value;
            var aryBlockId = [];
            for (var j = 1; j <= m; j++) {
                var col = document.getElementById("col_" + j);
                var colChilds = col.getElementsByTagName("div");
                for (var i = 0; i < colChilds.length; i++) {
                    if (colChilds[i].className == 'drag_div') {
                        var blockid = colChilds[i].id.replace("drag_", "");
                        aryBlockId[aryBlockId.length] = blockid;
                    }
                }
            }
            return aryBlockId;
        }

         function resetDragContent(id) {
            var objDivID = document.getElementById('drag_content_' + id);
            if (document.getElementById('loadcontentid_' + id) == null) {
                objDivID.innerHTML = '<div id="loadcontentid_' + id + '" style="width:100px"><img src="/Portal/Door/image/loading.gif"><span id="loadcontenttext_' + id + '" style="color:#333"></span>'

            }
        }

        var i = 0;
        //加载元素内容
        function loadDragContent(id, count, newId) {
            var loadId = id;
            if (newId) {
                loadId = newId;
            }
            var objDivID = document.getElementById('drag_content_' + id);
            var objOtext = document.getElementById('loadcontenttext_' + id);

            var objLoadcontentid = document.getElementById('loadcontentid_' + id);
            objOtext.innerHTML = "加载内容...";

            var saveGimgContent = {
                onCreate: function () {
                    Element.show('loadcontentid_' + id);
                },
                onComplete: function () {
                    if (Ajax.activeRequestCount == 0) {
                        Element.hide('loadcontentid_' + id);
                    }
                }
            };
            Ajax.Responders.register(saveGimgContent);
            url = "door/BlockData.aspx";
            queryString = "Param=GetContent&BlockId=" + loadId + "&Count=" + count + "&ClientWidth=" + document.body.clientWidth + "&BlockType=" + _blockType + "&TemplateId=" + _templateId;
            new Ajax.Request
	(
		url,
		{
		    method: "post",
		    onSuccess: function (resp) {
		        objDivID.innerHTML = resp.responseText;
		        if (document.getElementById('blocktypevalue_' + id).value == -1) {
		            loadingNewsPic(-1, id);
		        }

		    },
		    onFailure: function () {
		        //alert(url);

		    },
		    parameters: queryString
		}
	);
        }

        //获取模版边框值
        function getTplBolderColor(tpl) {
            var bcol = "#999";
            switch (tpl) {
                case "navarat":
                    bcol = "#FFB0B0";
                    break;
                case "orange":
                    bcol = "#FFC177";
                    break;
                case "yellow":
                    bcol = "#FFED77";
                    break;
                case "green":
                    bcol = "#CBE084";
                    break;
                case "blue":
                    bcol = "#A1D9ED";
                    break;
                case "gray":
                    bcol = "#BBBBBB";
                    break;
                case "o_navarat":
                    bcol = "#B78AA9";
                    break;
                case "o_orange":
                    bcol = "#D68C6F";
                    break;
                case "o_yellow":
                    bcol = "#A9B98C";
                    break;
                case "o_green":
                    bcol = "#96C38A";
                    break;
                case "o_blue":
                    bcol = "#579AE9";
                    break;
                case "o_gray":
                    bcol = "#8AA2B7";
                    break;
            }
            return bcol;

        }

        //元素编辑按钮开关
        function switchOptionImg(blockid, n) {

            if (n == 1) {
                if (document.getElementById('drag_myrss_img_' + blockid))
                    Element.show('drag_myrss_img_' + blockid);
                Element.show('drag_switch_img_' + blockid);
                Element.show('drag_refresh_img_' + blockid);
                //Element.show('drag_edit_img_' + blockid);
                Element.show('drag_delete_img_' + blockid);
            }
            else {
                if (document.getElementById('drag_myrss_img_' + blockid))
                    Element.hide('drag_myrss_img_' + blockid);
                Element.hide('drag_switch_img_' + blockid);
                Element.hide('drag_refresh_img_' + blockid);
                //Element.hide('drag_edit_img_' + blockid);
                Element.hide('drag_delete_img_' + blockid);

            }


        }

        //锁定界面
        function lockWindowPage() {

            var widthHeight = getScreenWH();
            var screenDiv = document.createElement("div");
            screenDiv.id = "locksrceen";
            screenDiv.style.zIndex = "100";
            screenDiv.style.width = widthHeight.width;
            screenDiv.style.height = widthHeight.height;
            screenDiv.style.background = "#000";
            screenDiv.style.filter = "alpha(Opacity=20)";
            screenDiv.style.position = "absolute";
            screenDiv.style.left = "0px";
            screenDiv.style.top = "0px";
            document.body.appendChild(screenDiv);
        }

        //解除锁定界面
        function unlockWindowPage() {
            var screenDiv = document.getElementById("locksrceen");
            if (screenDiv)
                screenDiv.parentNode.removeChild(screenDiv);
        }

        //全屏高宽度
        function getScreenWH() {
            var objData = new Object();
            var cwidth = document.body.clientWidth;
            var swidth = document.body.scrollWidth;
            var cheight = document.body.clientHeight;
            var sheight = document.body.scrollHeight;
            objData.width = cwidth > swidth ? cwidth : swidth;
            objData.height = cheight > sheight ? cheight : sheight;
            return objData;
        }
        var isSave = false;
        //设置列数
        function setLayoutType(n) {
            var objLayouttype = document.getElementById('layouttype');
            var m = objLayouttype.value;
            m = parseInt(m, 10);

            if (n < m) {
                var objInitText = document.getElementById('inittext');
                objInitText.innerHTML = "";
                lockWindowPage();

                var logIsSure = window.confirm("修改后的列数比现有的列数少，被删除列里的元素将转到最后一列！确定吗？");
                if (!logIsSure) {
                    objInitText.innerHTML = "";
                    unlockWindowPage();
                    return;
                }
            }
            for (i = 1; i <= 4; i++) {
                var objLayoutnum = document.getElementById("layoutnum_" + i);
                if (i == n) {
                    objLayoutnum.className = "layoutnumselect";
                }
                else {
                    objLayoutnum.className = "layoutnum";
                }
            }
            var objLayoutdisplay1 = document.getElementById('layoutdisplay1');
            var objLayoutdisplay2 = document.getElementById('layoutdisplay2');
            var objLayoutdisplay3 = document.getElementById('layoutdisplay3');
            var objLayoutdisplay4 = document.getElementById('layoutdisplay4');

            objLayouttype.value = n;
            if (n == 1) {
                objLayoutdisplay1.style.display = "";
                objLayoutdisplay2.style.display = "none";
                objLayoutdisplay3.style.display = "none";
                objLayoutdisplay4.style.display = "none";
                switch (m) {
                    case 1:
                        break;
                    case 2:
                        var col1 = document.getElementById('col_1');
                        var col2 = document.getElementById('col_2');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 100;
                        col1.style.width = "100%";
                        for (var i = 0; i < col2.childNodes.length; i++) {
                            if (!Element.hasClassName(col2.childNodes(i), 'no_drag'))
                                col1.innerHTML += col2.childNodes(i).outerHTML;
                        }
                        col2.parentNode.removeChild(col2);
                        break;
                    case 3:
                        var col1 = document.getElementById('col_1');
                        var col2 = document.getElementById('col_2');
                        var col3 = document.getElementById('col_3');
                        var objLayout1 = document.getElementById("layout1");
                        for (var j = 2; j <= 3; j++) {
                            var col = eval("col" + j);
                            for (var i = 0; i < col.childNodes.length; i++) {
                                if (!Element.hasClassName(col.childNodes(i), 'no_drag'))
                                    col1.innerHTML += col.childNodes(i).outerHTML;
                            }
                            col.parentNode.removeChild(col);
                        }
                        objLayout1.value = 100;
                        col1.style.width = "100%";
                        break;
                    case 4:
                        var col1 = document.getElementById('col_1');
                        var col2 = document.getElementById('col_2');
                        var col3 = document.getElementById('col_3');
                        var col4 = document.getElementById('col_4');
                        var objLayout1 = document.getElementById("layout1");
                        for (var j = 2; j <= 3; j++) {
                            var col = eval("col" + j);
                            for (var i = 0; i < col.childNodes.length; i++) {
                                if (!Element.hasClassName(col.childNodes(i), 'no_drag'))
                                    col1.innerHTML += col.childNodes(i).outerHTML;
                            }
                            col.parentNode.removeChild(col);
                        }
                        objLayout1.value = 100;
                        col1.style.width = "100%";
                        break;
                }
            }

            if (n == 2) {
                objLayoutdisplay1.style.display = "";
                objLayoutdisplay2.style.display = "";
                objLayoutdisplay3.style.display = "none";
                objLayoutdisplay4.style.display = "none";
                switch (m) {
                    case 1:
                        var col1 = document.getElementById('col_1');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 50;
                        col1.style.width = "50%";
                        addCol2();
                        break;
                    case 2:
                        break;
                    case 3:
                        var col1 = document.getElementById('col_1');
                        var col2 = document.getElementById('col_2');
                        var col3 = document.getElementById('col_3');
                        var objLayout1 = document.getElementById("layout1");
                        var objLayout2 = document.getElementById("layout2");
                        for (var i = 0; i < col3.childNodes.length; i++) {
                            if (!Element.hasClassName(col3.childNodes(i), 'no_drag'))
                                col2.innerHTML += col3.childNodes(i).outerHTML;
                        }
                        col3.parentNode.removeChild(col3);

                        objLayout1.value = 50;
                        col1.style.width = "50%";

                        var widthcol2 = 50;
                        col2.style.width = widthcol2 + "%"; ;
                        objLayout2.value = widthcol2;
                        break;
                    case 4:
                        var col1 = document.getElementById('col_1');
                        var col2 = document.getElementById('col_2');
                        var col3 = document.getElementById('col_3');
                        var col4 = document.getElementById('col_4');
                        var objLayout1 = document.getElementById("layout1");
                        var objLayout2 = document.getElementById("layout2");
                        for (var j = 3; j <= 4; j++) {
                            var col = eval("col" + j);
                            for (var i = 0; i < col.childNodes.length; i++) {
                                if (!Element.hasClassName(col.childNodes(i), 'no_drag'))
                                    col2.innerHTML += col.childNodes(i).outerHTML;
                            }
                            col.parentNode.removeChild(col);
                        }

                        objLayout1.value = 50;
                        col1.style.width = "50%";

                        var widthcol2 = 50;
                        col2.style.width = widthcol2 + "%"; ;
                        objLayout2.value = widthcol2;
                        break;

                }
            }

            if (n == 3) {
                objLayoutdisplay1.style.display = "";
                objLayoutdisplay2.style.display = "";
                objLayoutdisplay3.style.display = "";
                objLayoutdisplay4.style.display = "none";
                switch (m) {
                    case 1:
                        var col1 = document.getElementById('col_1');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 30;
                        col1.style.width = "30%";
                        addCol2();

                        var col2 = document.getElementById('col_2');
                        var objLayout2 = document.getElementById("layout2");
                        objLayout2.value = 40;
                        col2.style.width = "40%";
                        addCol3();
                        break;
                    case 2:
                        var col1 = document.getElementById('col_1');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 30;
                        col1.style.width = "30%";

                        var col2 = document.getElementById('col_2');
                        var objLayout2 = document.getElementById("layout2");
                        objLayout2.value = 40;
                        col2.style.width = "40%";
                        addCol3();
                        DragUtil.getSortIndex();
                        break;
                    case 3:
                        break;
                    case 4:
                        var col1 = document.getElementById('col_1');
                        var col3 = document.getElementById('col_3');
                        var objLayout3 = document.getElementById("layout3");
                        var objLayout4 = document.getElementById("layout4");
                        objLayout3.value = parseInt(objLayout3.value) + parseInt(objLayout4.value);
                        col3.style.width = objLayout3.value + "%";
                        var col4 = document.getElementById('col_4');
                        for (var i = 0; i < col4.childNodes.length; i++) {
                            if (!Element.hasClassName(col4.childNodes(i), 'no_drag'))
                                col3.innerHTML += col4.childNodes(i).outerHTML;
                        }
                        col4.parentNode.removeChild(col4);

                        break;
                }
            }
            if (n == 4) {
                objLayoutdisplay1.style.display = "";
                objLayoutdisplay2.style.display = "";
                objLayoutdisplay3.style.display = "";
                objLayoutdisplay4.style.display = "";
                switch (m) {
                    case 1:
                        var col1 = document.getElementById('col_1');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 25;
                        col1.style.width = "25%";

                        addCol2();
                        var col2 = document.getElementById('col_2');
                        var objLayout2 = document.getElementById("layout2");
                        objLayout2.value = 25;
                        col2.style.width = "25%";

                        addCol3();
                        var col3 = document.getElementById('col_3');
                        var objLayout3 = document.getElementById("layout3");
                        objLayout3.value = 25;
                        col3.style.width = "25%";

                        addCol4();
                        break;
                    case 2:
                        var col1 = document.getElementById('col_1');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 25;
                        col1.style.width = "25%";

                        var col2 = document.getElementById('col_2');
                        var objLayout2 = document.getElementById("layout2");
                        objLayout2.value = 25;
                        col2.style.width = "25%";

                        addCol3();
                        var col3 = document.getElementById('col_3');
                        var objLayout3 = document.getElementById("layout3");
                        objLayout3.value = 25;
                        col3.style.width = "25%";

                        addCol4();

                        break;
                    case 3:
                        var col1 = document.getElementById('col_1');
                        var objLayout1 = document.getElementById("layout1");
                        objLayout1.value = 25;
                        col1.style.width = "25%";

                        var col2 = document.getElementById('col_2');
                        var objLayout2 = document.getElementById("layout2");
                        objLayout2.value = 25;
                        col2.style.width = "25%";

                        var col3 = document.getElementById('col_3');
                        var objLayout3 = document.getElementById("layout3");
                        objLayout3.value = 25;
                        col3.style.width = "25%";

                        addCol4();
                        break;
                    case 4:
                        break;
                }
            }
            for (var i = n + 1; i < 5; i++) {
                document.getElementById('layout' + i).value = "0";
            }
            initDrag();
            var col1width = document.getElementById('layout1').value;
            var col2width = document.getElementById('layout2').value;
            var col3width = document.getElementById('layout3').value;
            var col4width = document.getElementById('layout4').value;
            var url = "Door/BlockData.aspx";
            queryString = "Param=ChangeColumns&layout1=" + col1width + "&layout2=" + col2width + "&layout3=" + col3width + "&layout4=" + col4width;
            queryString += "&Columns=" + n;
            queryString += "&TemplateString=" + DragUtil.getSortIndex();
            queryString += "&BlockType=" + _blockType;
            queryString += "&TemplateId=" + _templateId;
            new Ajax.Request(url, { method: "post", parameters: queryString });
            unlockWindowPage();
            //lockWindowPage();
            Element.hide('popupConMenuID');
        }


        //增加第二列
        function addCol2() {
            var colAry = [];
            colAry[colAry.length] = ' 	<div id="col_2_hidden_div" class="drag_div no_drag"><div id="col_2_hidden_div_h"></div></div>';

            var col1 = document.getElementById("col_1");

            var col1Width = document.getElementById("layout1").value;
            var col2Width = 100 - col1Width;
            document.getElementById("layout2").value = col2Width;

            var newColDiv = document.createElement("div");
            newColDiv.className = "col_div";
            newColDiv.id = "col_2";
            newColDiv.style.width = parseFloat(col2Width) + "%";
            newColDiv.innerHTML = colAry.join("");
            col1.parentNode.insertBefore(newColDiv, null)
            initDrag();
        }

        //增加第三列
        function addCol3() {
            var colAry = [];
            colAry[colAry.length] = ' 	<div id="col_3_hidden_div" class="drag_div no_drag"> <div id="col_3_hidden_div_h"></div></div>';

            var col1 = document.getElementById("col_1");

            var col1Width = document.getElementById("layout1").value;
            var col2Width = document.getElementById("layout2").value;
            var col3Width = 100 - col1Width - col2Width;

            document.getElementById("layout3").value = col3Width;
            var newColDiv = document.createElement("div");
            newColDiv.className = "col_div";
            newColDiv.id = "col_3";
            newColDiv.style.width = parseFloat(col3Width) + "%";
            newColDiv.innerHTML = colAry.join("");
            col1.parentNode.insertBefore(newColDiv, null);
            initDrag();
        }
        //增加第四列
        function addCol4() {
            var colAry = [];
            colAry[colAry.length] = ' 	<div id="col_4_hidden_div" class="drag_div no_drag"><div id="col_4_hidden_div_h"></div></div>';

            var col1 = document.getElementById("col_1");

            var col1Width = document.getElementById("layout1").value;
            var col2Width = document.getElementById("layout2").value;
            var col3Width = document.getElementById("layout3").value;
            var col4Width = 100 - col1Width - col2Width - col3Width;

            document.getElementById("layout4").value = col4Width;
            var newColDiv = document.createElement("div");
            newColDiv.className = "col_div";
            newColDiv.id = "col_4";
            newColDiv.style.width = parseFloat(col4Width) + "%";
            newColDiv.innerHTML = colAry.join("");
            col1.parentNode.insertBefore(newColDiv, null);
            initDrag();
        }

        //数值太小
        function isThinNum(n) {
            var logicv = true;
            if (n < 10) {
                logicv = window.confirm("输入的值偏小可能引起元素模块变形！确定吗？");
            }
            return logicv;
        }

        //改变宽度
        function changeColWidth() {
            var col1width = document.getElementById('layout1').value;
            var col2width = document.getElementById('layout2').value;
            var col3width = document.getElementById('layout3').value;
            var col4width = document.getElementById('layout4').value;
            if (isNaN(col1width) || isNaN(col2width) || isNaN(col3width) || isNaN(col4width)) {
                alert("只能输入数字！");
                return;
            }
            var total = parseFloat(col1width) + parseFloat(col2width) + parseFloat(col3width) + parseFloat(col4width);
            if (total > 100) {
                alert("列宽总和需<100%！");
                return;
            }
            for (var i = 1; i <= 4; i++) {
                if (document.getElementById('col_' + i.toString()) != null) {
                    var objCol = document.getElementById('col_' + i.toString());
                    var colwidth = document.getElementById('layout' + i.toString()).value;
                    if (!isInteger(colwidth)) {

                        document.getElementById('layout' + i.toString()).select();
                        return;

                    }
                    if (!isThinNum(colwidth)) {
                        document.getElementById('layout' + i.toString()).select();
                        return;
                    };
                    objCol.style.width = colwidth + "%";
                }
            }
            var objLayouttype = document.getElementById('layouttype');
            url = "Door/BlockData.aspx";
            queryString = "Param=ChangeWidth&layout1=" + col1width + "&layout2=" + col2width + "&layout3=" + col3width + "&layout4=" + col4width + "&Columns=" + objLayouttype.value;
            queryString += "&BlockType=" + _blockType;
            queryString += "&TemplateId=" + _templateId;
            new Ajax.Request(url, { method: "post", parameters: queryString });
            Element.hide('popupConMenuID');

        }

        //加载列内容的面板
        function loadColEdit() {

            if (document.getElementById('coleditcon') == null) {
                return;
            }
            var objDivID = document.getElementById('paneladdcontent');
            var objOtext = document.getElementById('coleditcontext');

            objOtext.innerHTML = "加载内容...";

            var saveGimgColCon = {
                onCreate: function () {
                    Element.show('coleditcon');
                },
                onComplete: function () {
                    if (Ajax.activeRequestCount == 0) {
                        Element.hide('coleditcon');
                    }
                }
            };
            Ajax.Responders.register(saveGimgColCon);

            url = "Door/BlockData.aspx";
            queryString = "Param=GetAllBlock";
            queryString += "&BlockType=" + _blockType;
            queryString += "&TemplateId=" + _templateId;
            new Ajax.Request(url, {
                method: "post",
                onSuccess: function (resp) {
                    objDivID.innerHTML = resp.responseText;
                    var blockIDs = "";
                    jQuery.each(jQuery(".drag_div[id^='drag_']"), function (i, item) {
                        var blockID = jQuery(item).attr("id").replace("drag_", "");
                        blockIDs += blockID + ",";
                    });
                    jQuery.each(jQuery(objDivID).find(".paneladdimg"), function (i, item) {
                        if (blockIDs.indexOf(jQuery.trim(jQuery(item).attr("blockID")) + ",") > -1) {
                            jQuery(item).attr("src", "/Portal/Door/image/added.gif").attr("title", "已添加"); ;
                        }
                    });
                    InitColsValue();

                },
                onFailure: function () {
                    //alert(url);
                },
                parameters: queryString
            });
        }

        function InitColsValue() {
            //var cols = jQuery("#layouttype").val();
            var cols = jQuery("#Divlayout").children().children().length;

            for (var i = 1; i <= 4; i++) {
                if (i == cols)
                    jQuery("#layoutnum_" + i).removeClass().addClass("layoutnumselect");
                else
                    jQuery("#layoutnum_" + i).removeClass().addClass("layoutnum");
            }
            for (var i = 1; i <= cols; i++) {
                jQuery("#layoutdisplay" + i).css("display", "");
                jQuery("#layout" + i).val(jQuery("#Divlayout").children().children().eq(i - 1)[0].style.width.replace("%", ""));
            }
            for (var i = cols + 1; i <= 4; i++) {
                if (document.getElementById("layoutdisplay" + i))
                    document.getElementById("layoutdisplay" + i).style.display = "none";
            }
        }

        //展开首页编辑器
        function showPanelCon() {
            var objPCM = document.getElementById('popupConMenuID');
            if (objPCM.style.display == "none") {
                closeAllItemEditor();
                Element.show('popupConMenuID');
            }
            else {
                Element.hide('popupConMenuID')
            }
            loadColEdit();

        }

        //关闭所有元素编辑器
        function closeAllItemEditor() {
            var aryBlockId = getAllDragDiv();
            for (var i = 0; i < aryBlockId.length; i++) {
                var blockid = aryBlockId[i];
                var objDE = document.getElementById('drag_editor_' + blockid);
                if (objDE.style.display != "none") {
                    objDE.style.display = "none";
                    objDE.innerHTML = '<div id="loadeditorid_' + blockid + '" style="width:100px"><img src="Door/image/loading.gif"><span id="loadeditortext_' + blockid + '" style="color:#333"></span></div>'
                }

            }
        }

        //初始化功能图标 元素内
        function initItemImg() {
            var aryBlockId = getAllDragDiv();
            for (var i = 0; i < aryBlockId.length; i++) {
                var blockid = aryBlockId[i];
                try {
                    var objDMI = document.getElementById('drag_myrss_img_' + blockid);
                    var objDSI = document.getElementById('drag_switch_img_' + blockid);
                    var objDRI = document.getElementById('drag_refresh_img_' + blockid);
                    var objDEI = document.getElementById('drag_edit_img_' + blockid);
                    var objDDI = document.getElementById('drag_delete_img_' + blockid);
                    if (objDMI) {
                        objDMI.onmouseover = function () { this.style.filter = '' };
                        objDMI.onmouseout = function () { this.style.filter = 'gray()' };
                        objDMI.style.display = "none";
                    }
                    if (objDSI) {
                        objDSI.onmouseover = function () { this.style.filter = '' };
                        objDSI.onmouseout = function () { this.style.filter = 'gray()' };
                        objDSI.style.display = "none";
                    }
                    if (objDRI) {
                        objDRI.onmouseover = function () { this.style.filter = '' };
                        objDRI.onmouseout = function () { this.style.filter = 'gray()' };
                        objDRI.style.display = "none";
                    }
                    if (objDEI) {
                        objDEI.onmouseover = function () { this.style.filter = '' };
                        objDEI.onmouseout = function () { this.style.filter = 'gray()' };
                        objDEI.style.display = "none";
                    }
                    if (objDDI) {
                        objDDI.onmouseover = function () { this.style.filter = '' };
                        objDDI.onmouseout = function () { this.style.filter = 'gray()' };
                        objDDI.style.display = "none";
                    }
                } catch (e) { };
            }
        }

        //初始化功能图标 列
        function initColImg() {
            var objCPCM = document.getElementById('controlPopupConMenu');
            objCPCM.onclick = function () {
                showPanelCon();
            };
            var objDSIA = document.getElementById('drag_switch_img_all');
            objDSIA.onclick = function () {
                switchDragAll(objDSIA);
            };
        }

        function showHiddenInfo(tid, imgid, openimg, closeimg) {
            var objTid = $(tid);
            var objImgid = $(imgid);
            if (objTid.style.display == '') {
                hiddenInfo(tid, imgid, openimg);
            }
            else {
                showInfo(tid, imgid, closeimg);
            }
        }

        function showInfo(tid, imgid, closeimg) {
            var objTid = $(tid);
            var objImgid = $(imgid);
            if (typeof (imgid) == 'object') {
                objImgid = imgid;
            }
            objTid.style.display = '';
            if (closeimg == "" || typeof (closeimg) == "undefined") {
                objImgid.src = "/images/arrowdown.gif"

            }
            else {
                objImgid.src = closeimg;
            }
        }

        function hiddenInfo(tid, imgid, openimg) {
            var objTid = $(tid);
            var objImgid = $(imgid);
            if (typeof (imgid) == 'object') {
                objImgid = imgid;
            }
            objTid.style.display = 'none';

            if (openimg == "" || typeof (openimg) == "undefined") {
                objImgid.src = "/images/arrowup.gif";
            }
            else {
                objImgid.src = openimg;
            }
        }

        //Integer;
        function isInteger(s) {
            s = trim(s);
            var p = /^[-\+]?\d+$/;
            return p.test(s);
        }

        //去左右空格; 
        function trim(s) {
            return rtrim(ltrim(s));
        }
        //去左空格; 
        function ltrim(s) {
            return s.replace(/^\s*/, "");
        }
        //去右空格; 
        function rtrim(s) {
            return s.replace(/\s*$/, "");
        }

    </script>
</head>
<body style="width: 100%;height:100%;overflow:hidden">
    <div style="width: 200px;height:22px;border:0px" id="divBtn">
        <div style="float: left; width: 100px">
            <img class="imglink" id="controlPopupConMenu" title="添加修改首页内容" style="margin-left: 10px"
                src="Door/image/add.gif">
            <img class="imglink" id="drag_switch_img_all" title="展开/隐藏所有元素" style="margin-left: 5px"
                    src="Door/image/open.gif">
            <img class="imglink" id="reset" title="恢复默认" style="margin-left: 5px; display: "
                src="Door/image/reset.gif" onclick="reset();">
        </div>
        <div id="inittext" style="float: left; width: 100px; color: #999; height: 20px">
        </div>
    </div>
    <div id="Divlayout" style="width: 100%;overflow-y:auto;overflow-x:hidden;border:0px" >
        <div id="TempDiv" style="width:98.5%;height:100%;border:0px" >
            <%=Html%>
        </div>
    </div>
    <div id="popupImgMenuID" style="display: none; position: absolute">
        <div style="padding-right: 2px; padding-left: 2px; padding-bottom: 2px; width: 100%;
            padding-top: 2px">
            <img class="imglink" title="关闭" onclick="Element.hide('popupImgMenuID')" src="Door/image/closetab.gif"
                align="right"></div>
        <div class="popupImgMenu" id="popupImgItem">
        </div>
        <div id="loadimgid" style="width: 100px">
            <img src="Door/image/loading.gif" align="absMiddle"><span
                id="loadtext" style="color: #333"></span></div>
    </div>
    <input type="hidden" name="tmpblockid"/>
    <input id="layouttype" type="hidden" value="<%=LayoutType%>" name="layouttype"/>
    <input id="showstatus" type="hidden" value="1" name="showstatus"/>
    <div id="popupConMenuID" style="border-right: #a2c7d9 1px solid; padding-right: 5px;
        border-top: #a2c7d9 1px solid; display: none; padding-left: 5px; z-index: 10;
        left: 15px; padding-bottom: 5px; border-left: #a2c7d9 1px solid; width: 200px;
        padding-top: 5px; border-bottom: #a2c7d9 1px solid; position: absolute; top: 20px;
        background-color: #f1f7f9">
        <div style="width: 100%;">
            <img class="imglink" onclick="Element.hide('popupConMenuID')" src="Door/image/closetab.gif"
                align="right"></div>
        <div style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
            width: 180px; color: #222; padding-top: 5px">
            <img style="cursor: hand" onclick="switchDrag('paneladdcontent',this)" src="Door/image/open.gif"
                align="absMiddle">
            添加首页内容</div>
        <div id="paneladdcontent" style="overflow: auto; width: 100%; height: 300px" align="left">
            <div id="coleditcon" style="width: 100px">
                <img src="Door/image/loading.gif"><span
                    id="coleditcontext" style="color: #333"></span></div>
        </div>
        <div style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
            width: 180px; color: #222; padding-top: 5px">
            <img style="cursor: hand" onclick="switchDrag('panelmodifycol',this)" src="Door/image/open.gif"
                align="absMiddle">
            修改首页排版</div>
        <div id="panelmodifycol">
            <div style="padding-right: 2px; display: inline; padding-left: 2px; padding-bottom: 2px;
                padding-top: 2px">
                <div class="layoutnum" id="layoutnum_1" onclick="setLayoutType(1)">
                    1列</div>
                <div class="layoutnumselect" id="layoutnum_2" onclick="setLayoutType(2)">
                    2列</div>
                <div class="layoutnum" id="layoutnum_3" onclick="setLayoutType(3)">
                    3列</div>
                <div class="layoutnum" id="layoutnum_4" onclick="setLayoutType(4)">
                    4列</div>
            </div>
            <div id="layoutdisplay1" style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
                width: 100%; color: #333333; padding-top: 5px">
                第1列宽
                <input class="block_input" id="layout1" style="width: 30px" maxlength="3" value="0"
                    name="layout1" onkeyup="value=value.replace(/[^0-9.]/g,'');" onbeforepaste="value=value.replace(/[^0-9.]/g,'');">%
                <input class="block_button" onclick="changeColWidth();" type="button" value="确定"
                    style="width: 35px"><input class="block_button" onclick="Element.hide('popupConMenuID');"
                        type="button" value="取消" style="width: 35px">
            </div>
            <div id="layoutdisplay2" style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
                width: 100%; color: #333333; padding-top: 5px">
                第2列宽
                <input class="block_input" id="layout2" style="width: 30px" maxlength="3" value="0"
                    name="layout2" onkeyup="value=value.replace(/[^0-9.]/g,'');" onbeforepaste="value=value.replace(/[^0-9.]/g,'');">%
            </div>
            <div id="layoutdisplay3" style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
                width: 100%; color: #333333; padding-top: 5px">
                第3列宽
                <input class="block_input" id="layout3" style="width: 30px" maxlength="3" value="0"
                    name="layout3" onkeyup="value=value.replace(/[^0-9.]/g,'');" onbeforepaste="value=value.replace(/[^0-9.]/g,'');">%
            </div>
            <div id="layoutdisplay4" style="padding-right: 5px; padding-left: 5px; padding-bottom: 5px;
                width: 100%; color: #333333; padding-top: 5px">
                第4列宽
                <input class="block_input" id="layout4" style="width: 30px" maxlength="3" value="0"
                    name="layout4" onkeyup="value=value.replace(/[^0-9.]/g,'');" onbeforepaste="value=value.replace(/[^0-9.]/g,'');">%
            </div>
        </div>
    </div>

    <script language="javascript" type="text/javascript">
        initDrag();
        initItemImg();
        initColImg();

    </script>
    <script language="javascript" type="text/javascript">
        function ExecuteFormFlow(urlstr, taskName, formWidth, formHeight, taskExecID, formInstanceID, taskStatus) {
            if (urlstr && urlstr != "") {
                if (urlstr.indexOf('?') > 0)
                    urlstr += "&";
                else
                    urlstr += "?";
                urlstr += "TaskExecID=" + jQuery.trim(taskExecID) + "&ID=" + jQuery.trim(formInstanceID) + "&TaskStatus=" + jQuery.trim(taskStatus);
                if (formWidth == "") formWidth = "880";
                if (formHeight=="") formHeight = "650";
                openWindow(urlstr, { title: taskName, width: formWidth, height: formHeight, onDestroy: function () { if (GetTaskList) GetTaskList('Undo'); } });
            }
        }

        function ExecuteAudit(flowId, taskKey) {
            //todo autid task
        }

        function MoreTask() {
            openWindow("/MvcConfig/Workflow/Task/MyTaskCenter", { width: 1050, height: 650 });
        }

        function MoreFinishedTask() {
            openWindow("/MvcConfig/Workflow/Task/MyTaskCenter?Status=Done", { width: 1050, height: 650 });
        }

        function MoreDesignTask() {
            openWindow("/MvcConfig/Project/DesignTask/DesignTaskList", { width: 1050, height: 650 });
        }

        function MoreMsg() {
            var url = "/Base/ShortMsg/Msg/Index";
            openWindow(url, { title: "我的消息", width: 1050, height: 650 });
        }


        function OpenMsg(url) {
            openWindow(url, { width: 820, height: 560, title: "我的消息" });
        }

        function jumpMenu(targ, selObj, restore) { //v3.0
            //eval(targ+".location='"+selObj.options[selObj.selectedIndex].value+"'");
            if (selObj.options[selObj.selectedIndex].value != "") window.open(selObj.options[selObj.selectedIndex].value, '查看信息', "");
        }

        function ShowHiddenObject(obj) {
            obj.style.display == "none" ? obj.style.display = "" : obj.style.display = "none";
        }

        function SetTaskList(dl) {
            var maxRecord = 6;
            var last = 0;
            var divTask = document.all("divTaskContent");
            if (divTask) {
                var szHtml = "";
                switch (dl.GetAttr("Type")) {
                    case "Approve":
                        {
                            if (dl.GetItemCount() > 0) {
                                if (maxRecord > dl.GetItemCount()) {
                                    last = maxRecord - dl.GetItemCount();
                                    maxRecord = dl.GetItemCount();
                                }
                                for (var i = 0; i < maxRecord; i++) {
                                    var di = dl.GetItem(i);
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'><img src='images/dot1.gif' width='6' height='9'></td><td height='22'>";
                                    if (di.GetAttr("Type") == "FreeFlow")
                                        szHtml += "<a href=\"javascript:ExecuteTask('" + di.GetAttr("ExecUrl") + "','" + di.GetAttr("Id") + "','" + di.GetAttr("FlowId") + "','" + di.GetAttr("RelateType") + "')\" title='" + di.GetAttr("TaskName") + "(" + di.GetAttr("FlowName") + di.GetAttr("CreateTime") + ")'>";
                                    else
                                        szHtml += "<a href=\"javascript:ExecuteTask('" + di.GetAttr("Type") + "','" + di.GetAttr("Id") + "','" + di.GetAttr("FlowId") + "','" + di.GetAttr("System") + "')\" title='" + di.GetAttr("TaskName") + "(" + di.GetAttr("FlowName") + di.GetAttr("CreateTime") + ")'>";

                                    if (di.GetAttr("CustomSign") == "T")
                                        szHtml += "<img src='/portal/image/急1.gif' border=0 width='16' height='16'>" + di.GetAttr("FlowName") + "</a></td>";
                                    else
                                        szHtml += di.GetAttr("FlowName") + "</a></td>";


                                    szHtml += "<td width=200 align=center>" + di.GetAttr("CreateTime") + "</td></tr></table><table width='95%'  border='0' cellspacing='0' cellpadding='0'>";
                                    szHtml += "<tr><td height='1' align=left colspan=2 background='images/line1.gif'></td></tr></table> ";
                                }

                                for (var i = last; i > 0; i--) {
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'></td><td height='22'>&nbsp;</td>";
                                    szHtml += "<td   width='120px' align='center' valign='middle' class='font_12_gray'></td></tr>" +
										"<tr><td height='1' colspan=4 ><table width='95%'  border='0' cellspacing='0' cellpadding='0'><tr><td height='1' background='images/line1.gif'></td></tr></table></td></tr>";
                                }

                            }
                            else {
                                for (var i = maxRecord; i > 0; i--) {
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'></td><td height='22'>&nbsp;</td>";
                                    szHtml += "<td   width='120px' align='center' valign='middle' class='font_12_gray'></td></tr>" +
										"<tr><td height='1' colspan=4 ><table width='95%'  border='0' cellspacing='0' cellpadding='0'><tr><td height='1' background='images/line1.gif'></td></tr></table></td></tr>";
                                }
                            }
                            szHtml += "</table>";
                            break;
                        }
                    case "Design":
                        {
                            if (dl.GetItemCount() > 0) {
                                if (maxRecord > dl.GetItemCount()) {
                                    last = maxRecord - dl.GetItemCount();
                                    maxRecord = dl.GetItemCount();
                                }
                                for (var i = 0; i < maxRecord; i++) {
                                    var di = dl.GetItem(i);
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'><img src='images/dot1.gif' width='6' height='9'></td><td height='22'>";
                                    if (di.GetAttr("CustomSign") == "T")
                                        szHtml += "<a href=\"javascript:OpenDesignTask('/project/workspace/PrjDesignTaskPut/PutProductMaint.aspx?TaskId=" + di.GetAttr("Id") + "&PrjId=" + di.GetAttr("PrjId") + "')\" title='" + di.GetAttr("Name") + "(" + di.GetAttr("PrjName") + di.GetAttr("PlanStartDate") + ")'><img src='images/middle_button02.gif' width='54' height='20' border='0' />" + di.GetAttr("Name") + "(" + di.GetAttr("PrjName") + ")</a></td>";
                                    else
                                        szHtml += "<a href=\"javascript:OpenDesignTask('/project/workspace/PrjDesignTaskPut/PutProductMaint.aspx?TaskId=" + di.GetAttr("Id") + "&PrjId=" + di.GetAttr("PrjId") + "')\" title='" + di.GetAttr("Name") + "(" + di.GetAttr("PrjName") + di.GetAttr("PlanStartDate") + ")'><img src='images/middle_button02.gif' width='54' height='20' border='0' />" + di.GetAttr("Name") + "(" + di.GetAttr("PrjName") + ")</a></td>";


                                    szHtml += "<td width=200 align=center>" + di.GetAttr("CreateTime") + "</td></tr></table><table width='95%'  border='0' cellspacing='0' cellpadding='0'>";
                                    szHtml += "<tr><td height='1' align=left colspan=2 background='images/line1.gif'></td></tr></table> ";
                                }

                                for (var i = last; i > 0; i--) {
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'></td><td height='22'>&nbsp;</td>";
                                    szHtml += "<td   width='120px' align='center' valign='middle' class='font_12_gray'></td></tr>" +
										"<tr><td height='1' colspan=4 ><table width='95%'  border='0' cellspacing='0' cellpadding='0'><tr><td height='1' background='images/line1.gif'></td></tr></table></td></tr>";
                                }

                            }
                            else {
                                for (var i = maxRecord; i > 0; i--) {
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'></td><td height='22'>&nbsp;</td>";
                                    szHtml += "<td   width='120px' align='center' valign='middle' class='font_12_gray'></td></tr>" +
										"<tr><td height='1' colspan=4 ><table width='95%'  border='0' cellspacing='0' cellpadding='0'><tr><td height='1' background='images/line1.gif'></td></tr></table></td></tr>";
                                }
                            }
                            szHtml += "</table>";
                            break;
                        }
                    case "Audit":
                        {
                            if (dl.GetItemCount() > 0) {
                                if (maxRecord > dl.GetItemCount()) {
                                    last = maxRecord - dl.GetItemCount();
                                    maxRecord = dl.GetItemCount();
                                }
                                for (var i = 0; i < maxRecord; i++) {
                                    var di = dl.GetItem(i);
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'><img src='images/dot1.gif' width='6' height='9'></td><td height='22'>";
                                    if (di.GetAttr("CustomSign") == "T")
                                        szHtml += "<a href=\"javascript:ExecuteTask('AuditTask','" + di.GetAttr("TaskKey") + "','" + di.GetAttr("FlowId") + "')\" title='[" + di.GetAttr("TaskCode") + "]" + di.GetAttr("TaskName") + "'>[" + di.GetAttr("TaskCode") + "]" + di.GetAttr("TaskName") + "</a></td>";
                                    else
                                        szHtml += "<a href=\"javascript:ExecuteTask('AuditTask','" + di.GetAttr("TaskKey") + "','" + di.GetAttr("FlowId") + "')\" title='[" + di.GetAttr("TaskCode") + "]" + di.GetAttr("TaskName") + "'>[" + di.GetAttr("TaskCode") + "]" + di.GetAttr("TaskName") + "</a></td>";


                                    szHtml += "<td width=200 align=center>" + di.GetAttr("CreateTime") + "</td></tr></table><table width='95%'  border='0' cellspacing='0' cellpadding='0'>";
                                    szHtml += "<tr><td height='1' align=left colspan=2 background='images/line1.gif'></td></tr></table> ";
                                }
                                for (var i = last; i > 0; i--) {
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'></td><td height='22'>&nbsp;</td>";
                                    szHtml += "<td   width='120px' align='center' valign='middle' class='font_12_gray'></td></tr>" +
									"<tr><td height='1' colspan=4 ><table width='95%'  border='0' cellspacing='0' cellpadding='0'><tr><td height='1' background='images/line1.gif'></td></tr></table></td></tr>";
                                }

                            }
                            else {
                                for (var i = maxRecord; i > 0; i--) {
                                    szHtml += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td width='10'></td><td width='10'></td><td height='22'>&nbsp;</td>";
                                    szHtml += "<td   width='120px' align='center' valign='middle' class='font_12_gray'></td></tr>" +
										"<tr><td height='1' colspan=4 ><table width='95%'  border='0' cellspacing='0' cellpadding='0'><tr><td height='1' background='images/line1.gif'></td></tr></table></td></tr>";
                                }
                            }
                            szHtml += "</table>";
                            break;
                        }
                }
                divTask.innerHTML = szHtml;
            }
        }
			
			
    </script>
</body>
</html>
