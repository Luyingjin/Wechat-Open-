function showFeedback() {
    if (ifDrag) {
        var url = window.location.href;
        //有此两参数会被认为是流程页面，会调用后台方法导致报错。故在此做替换。bh_cheng 2014-07-23
        url = url.replace("FlowCode", "流程编号");
        url = url.replace("TaskExecID", "任务ID");

        var title = $.trim($(document).attr("title"))
        var actionUrl = "/Base/PortalBlock/Feedback/Edit?title=" + title + "&url=" + url;
        openWindow(actionUrl, { title: "问题反馈", width: 650, height: 380 });
        /*
        window.top.mini.prompt("请填写反馈信息", "金慧综合管理系统", function (action, content) {
        if (action == "ok") {
        if ($.trim(content) == "") {
        msgUI("反馈信息不能为空");
        return;
        }
        var title = $.trim($(document).attr("title"))
        if (title == "") {
        msgUI("请输入反馈信息标题", 5, function (action, t) {
        if (action == "ok") {
        if ($.trim(t) == "") {
        msgUI("反馈信息标题不能为空");
        return;
        }
        saveFeedbackData({ Title: t, Content: content });
        }
        });
        }
        else {
        saveFeedbackData({ Title: title, Content: content });
        }
        }
        }, true);*/
    }
}

function saveFeedbackData(data) {
    data = $.extend(data, {
        Url: window.location.pathname + window.location.search,
        IsUse: "T"
    });
    var executeParams = new Object();
    executeParams["FormData"] = mini.encode(data);
    execute("/Base/PortalBlock/Feedback/Add", {
        validateForm: false,
        execParams: executeParams,
        onComplete: function () {
            msgUI("反馈信息已保存!");
        }
    });
}

