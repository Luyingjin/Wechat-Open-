function saveShortCut() {
    if (ifDrag) {
        msgUI("确认添加到快捷入口吗？", 2, function (action) {
            if (action == "ok") {
                var title = $.trim($(document).attr("title"))
                if (title == "") {
                    msgUI("请输入快捷入口名称：", 5, function (action, t) {
                        if (action == "ok") {
                            if ($.trim(t) == "") {
                                msgUI("快捷入口名称不能为空");
                                return;
                            }
                            saveShortCutData(t);
                        }
                    });
                }
                else {
                    saveShortCutData(title);
                }
            }
        });
    }
}

function saveShortCutData(title) {
    var data = {
        Url: window.location.pathname + window.location.search,
        Name: title,
        IsUse: "T",
        Type: "Normal",
        PageIndex: 0
    };
    var executeParams = new Object();
    executeParams["FormData"] = mini.encode(data);
    execute("/Base/PortalBlock/ShortCut/Add", {
        validateForm: false,
        execParams: executeParams,
        onComplete: function () {
            createSlideAnimate();
            var menuWindow = window.top.$("#Index")[0].contentWindow;
            if (menuWindow && menuWindow.bindShortcut) {
                menuWindow.bindShortcut();
            }
        }
    });
}

function createSlideAnimate(speed) {
    if (!speed) speed = 1000;
    var slideStart = getSlideStart(), slideEnd = getSlideEnd();
    if (slideStart != null && slideEnd != null) {
        var $div = $("<div></div>").attr("id", "divSlide").width(slideStart.width).height(slideStart.height).css("z-index", "9999").css("border", "2px solid #5680b6").css("position", "absolute").css("left", slideStart.left).css("top", slideStart.top);
        $div.appendTo(window.top.$("body"));
        $div.animate({
            left: slideEnd.left,
            top: slideEnd.top,
            width: slideEnd.width,
            height: slideEnd.height
        }, speed, function () {
            if (window.top.$("#divSlide")[0]) {
                window.top.$("#divSlide").remove();
            }
        });
    }
    else {
        msgUI("快捷入口设置成功！");
    }
}

function getSlideStart() {
    var animateStart = {};
    var menuWindow = window.top.$("#Index")[0].contentWindow;
    var myTab = window.parent.mini.get("MyTabs");
    var $divFrame = menuWindow.$("#divFrameForm");
    if (myTab && $divFrame[0]) {
        var $bodyEl = $(myTab.getTabBodyEl(myTab.getActiveTab()));
        animateStart = $.extend(animateStart, {
            width: $bodyEl.width(),
            height: $bodyEl.height(),
            left: $bodyEl.offset().left + $divFrame.offset().left,
            top: $bodyEl.offset().top + $divFrame.offset().top
        });
        return animateStart;
    }
    else if ($("#mini_window_id")[0]) {
        var miniWin = mini._topWindow.mini.get($("#mini_window_id").val());
        if (miniWin) {
            var $miniWin = $(miniWin.getEl());
            animateStart = $.extend(animateStart, {
                width: $miniWin.width(),
                height: $miniWin.height(),
                left: $miniWin.offset().left,
                top: $miniWin.offset().top
            });
        }
        return animateStart;
    }
    return null;
}

function getSlideEnd() {
    var animateEnd = {};
    var menuWindow = window.top.$("#Index")[0].contentWindow;
    var $imgEl = menuWindow.$("#imgShortcut");
    if ($imgEl[0]) {
        animateEnd = $.extend(animateEnd, {
            width: $imgEl.width(),
            height: $imgEl.height(),
            top: $imgEl.offset().top,
            left: $imgEl.offset().left
        });
        return animateEnd;
    }
    return null;
}

//鼠标移入移出按钮图标切换事件
function MouseChange(obj, e) {
    obj.src = e;
}
