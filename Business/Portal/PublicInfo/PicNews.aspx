<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PicNews.aspx.cs" Inherits="Portal.PublicInfo.PicNews" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="/CommonWebResource/CoreLib/Basic/jQuery/jquery-1.6.2.min.js" type="text/javascript"></script>  
    <script src="/CommonWebResource/CoreLib/MiniUI/miniui.js" type="text/javascript"></script>  
    <script src="/CommonWebResource/CoreLib/MiniUI/baseMiniuiExt.js" type="text/javascript"></script>   
    <script src="/MvcConfig/miniuiExt.js" type="text/javascript"></script>           
    <script src="/CommonWebResource/Theme/Default/MiniCssInc.js" type="text/javascript"></script>
</head>
<body>
    <style type="text/css">
    *{margin:0;padding:0;list-style-type:none;}
    a,img{border:0;}
    body{font:12px/180% "宋体",Arial, Helvetica, sans-serif;}
    /*图片轮换*/
    #slideBox{width:340px;height:220px;overflow:hidden;position:relative;margin:0px auto;}
    #slideBox ul#show_pic{margin:0;padding:0;list-style:none;height:300px;width:4750px;position:absolute;}
    #slideBox ul#show_pic li{float:left;margin:0;padding:0;height:220px;width:340px}
    #slideBox ul#show_pic li img{display:block;}
    #iconBall{position:absolute;bottom:25px;right:0;}
    #iconBall li{float:left;color:#000;width:25px;height:16px;line-height:16px;cursor:pointer;text-align:center;font-size:14px;font-weight:bold;padding-top:1px; background:url(images/iconbg.png) no-repeat;}
    #iconBall li.active{background:url(images/iconbg2.png) no-repeat;color:#fff;}
    #slideText {width:475px;height:28px;background:rgba(0,0,0,0.7);color:#fff;position:absolute;left:0px;bottom:0px;*background:transparent;filter:progid:DXImageTransform.Microsoft.gradient(startColorstr=#b2000000,endColorstr=#b2000000);}
    #textBall{position:absolute;left:10px;bottom:3px;}
    #textBall li{float:left;cursor:pointer;display:none;color:#fff;font-size:12px; width:330px; height:25px; line-height:25px;}
    #textBall li.active{display:block;}
    #textBall li a {text-decoration:none;color:#fff;}
    </style>
	<div id="slideBox">
		<ul id="show_pic" style="left:0px">
            <%=PicHTML %>
		</ul>
		<div id="slideText"></div>
		<ul id="iconBall">
            <%=IconBallHTML%>
		</ul>
		<ul id="textBall">
            <%=TextBallHTML%>
		</ul>
	</div><!--slideBox end-->

<script type="text/javascript">
    var glide = new function () {
        function $id(id) { return document.getElementById(id); };
        this.layerGlide = function (auto, oEventCont, oTxtCont, oSlider, sSingleSize, second, fSpeed, point) {
            var oSubLi = $id(oEventCont).getElementsByTagName('li');
            var oTxtLi = $id(oTxtCont).getElementsByTagName('li');
            var interval, timeout, oslideRange;
            var time = 1;
            var speed = fSpeed
            var sum = oSubLi.length;
            var a = 0;
            var delay = second * 3000;
            var setValLeft = function (s) {
                return function () {
                    oslideRange = Math.abs(parseInt($id(oSlider).style[point]));
                    $id(oSlider).style[point] = -Math.floor(oslideRange + (parseInt(s * sSingleSize) - oslideRange) * speed) + 'px';
                    if (oslideRange == [(sSingleSize * s)]) {
                        clearInterval(interval);
                        a = s;
                    }
                }
            };
            var setValRight = function (s) {
                return function () {
                    oslideRange = Math.abs(parseInt($id(oSlider).style[point]));
                    $id(oSlider).style[point] = -Math.ceil(oslideRange + (parseInt(s * sSingleSize) - oslideRange) * speed) + 'px';
                    if (oslideRange == [(sSingleSize * s)]) {
                        clearInterval(interval);
                        a = s;
                    }
                }
            }

            function autoGlide() {
                for (var c = 0; c < sum; c++) { oSubLi[c].className = ''; oTxtLi[c].className = ''; };
                clearTimeout(interval);
                if (a == (parseInt(sum) - 1)) {
                    for (var c = 0; c < sum; c++) { oSubLi[c].className = ''; oTxtLi[c].className = ''; };
                    a = 0;
                    oSubLi[a].className = "active";
                    oTxtLi[a].className = "active";
                    interval = setInterval(setValLeft(a), time);
                    timeout = setTimeout(autoGlide, delay);
                } else {
                    a++;
                    oSubLi[a].className = "active";
                    oTxtLi[a].className = "active";
                    interval = setInterval(setValRight(a), time);
                    timeout = setTimeout(autoGlide, delay);
                }
            }

            if (auto) { timeout = setTimeout(autoGlide, delay); };
            for (var i = 0; i < sum; i++) {
                oSubLi[i].onmouseover = (function (i) {
                    return function () {
                        for (var c = 0; c < sum; c++) { oSubLi[c].className = ''; oTxtLi[c].className = ''; };
                        clearTimeout(timeout);
                        clearInterval(interval);
                        oSubLi[i].className = "active";
                        oTxtLi[i].className = "active";
                        if (Math.abs(parseInt($id(oSlider).style[point])) > [(sSingleSize * i)]) {
                            interval = setInterval(setValLeft(i), time);
                            this.onmouseout = function () { if (auto) { timeout = setTimeout(autoGlide, delay); }; };
                        } else if (Math.abs(parseInt($id(oSlider).style[point])) < [(sSingleSize * i)]) {
                            interval = setInterval(setValRight(i), time);
                            this.onmouseout = function () { if (auto) { timeout = setTimeout(autoGlide, delay); }; };
                        }
                    }
                })(i)
            }
        }
    }

    //调用语句
    glide.layerGlide(
	    true,         //设置是否自动滚动
	    'iconBall',   //对应索引按钮
	    'textBall',   //标题内容文本
	    'show_pic',   //焦点图片容器
	    340,          //设置滚动图片位移像素
	    2, 		  //设置滚动时间2秒 
	    0.1,          //设置过渡滚动速度
	    'left'		  //设置滚动方向“向左”
    );
</script>
<script type="text/javascript">
    function openGallary(id, title) {
        allowResizeOpenWindow = false;
        title = title.length > 40 ? title.substring(0, 40) + "..." : title;
        openWindow("/base/PortalBlock/NewsImage/Gallery?ID=" + id, {
            width: "100%",
            height: "100%",
            title: title,
            showMaxButton: false
        });
    }
</script>
</body>
</html>
