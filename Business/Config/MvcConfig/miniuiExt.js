
//说明：
//  0、必须熟练掌握接口
//  1、setting参数可以省略
/*---------------------------------------------------------基本方法开始--------------------------------------------------------*/
//  clearDataForm(normalSettings);
//  clearQueryForm(normalSettings);
//  search(normalSettings);
//  quickSearch(searchFields, normalSettings)
//  add(windowSettings);
//  edit(windowSettings);
//  view(windowSettings);
//  del(execSettings);
//  save(execSettings);
//  saveList(execSettings);
//  saveSortedList(execSettings);
//  addRow(newRowJson, normalSettings);   //createId参数用于自动产生ID，推荐表单子表使用
//  delRow(normalSettings);
//  ExportExcel(gridId)     导出Excel
/*---------------------------------------------------------基本方法结束--------------------------------------------------------*/
/*---------------------------------------------------------树维护开始--------------------------------------------------------*/
//  saveNode(execSettings);
//  searchTree(normalSettings);
//  nodeViewing(windowSettings);
//  nodeEditing(windowSettings);
//  nodeAdding(windowSettings);
//  nodeDeleting(windowSettings);
//  onNodeDroping(e);
//  onNodeDrop(e);
//  relationAppending(url, windowSettings);
//  relationAppended(data, settings);
//  relationSetting(url, windowSettings);
//  relationSet(url, windowSettings);
//  delRelation(execSettings);
//  addRelationData(windowSettings);
//  editRelationData(windowSettings);
//  viewRelationData(windowSettings);
//  saveRelationData(execSettings);
//  delRelationData(execSettings);
/*---------------------------------------------------------树维护结束--------------------------------------------------------*/
/*---------------------------------------------------------选人选部门开始--------------------------------------------------------*/
//  returnForm(windowSettings);
//  returnList(windowSettings);
//  addSingleUserSelector(name, selectorSettings);
//  addMultiUserSelector(name, selectorSettings);
//  addSingleOrgSelector(name, selectorSettings);
//  addMultiOrgSelector(name, selectorSettings);
//  addSingleRoleSelector(name, selectorSettings);
//  addMultiRoleSelector(name, selectorSettings);
/*---------------------------------------------------------选人选部门结束--------------------------------------------------------*/
/*---------------------------------------------------------流程方法开始--------------------------------------------------------*/
//  flowLoadMenubar(jsonFormData);
//  flowAdd(flowCode,windowSettings);
//  flowEdit(flowCode,windowSettings);
/*---------------------------------------------------------流程方法结束--------------------------------------------------------*/

//  ExportWord(tmplCode,id);

//参数开关
allowUrlOpenForm = true; // 允许从地址栏打开表单页面
allowResizeOpenWindow = true; //允许改变弹出窗口大小

/*---------------------------------------------------------页面配置信息开始--------------------------------------------------------*/
var listConfig = {
    title: "",
    width: 720,
    height: 605
};
var treeConfig = {
    title: "",
    width: 720,
    height: 605
};
var relationConfig = {
    title: "",
    width: 720,
    height: 605
};
var alertTitleStr = "MGCC微信管理系统";
/*---------------------------------------------------------页面配置信息结束--------------------------------------------------------*/
/*---------------------------------------------------------权限常量开始--------------------------------------------------------*/

/*---------------------------------------------------------权限常量结束--------------------------------------------------------*/
/*---------------------------------------------------------Url常量开始--------------------------------------------------------*/
var urlConstant = {
    singleOrg: "/MvcConfig/Auth/Org/Selector?SelectMode=Single&RootFullID=",
    multiOrg: "/MvcConfig/Auth/Org/Selector?SelectMode=Multi&RootFullID=",
    singleRole: "/MvcConfig/Auth/Role/Selector?SelectMode=Single&GroupID=",
    multiRole: "/MvcConfig/Auth/Role/Selector?SelectMode=Multi&GroupID=",
    singleUser: "/MvcConfig/Auth/User/SingleSelector?RootFullID=",
    multiUser: "/MvcConfig/Auth/User/MultiSelector?RootFullID=",
    singleScopeUser: "/MvcConfig/Auth/User/SingleScopeSelector?GroupID=",
    multiScopeUser: "/MvcConfig/Auth/User/MultiScopeSelector?GroupID=",
    multiRes: "/MvcConfig/Auth/Res/Selector?SelectMode=Multi&RootFullID=a1b10168-61a9-44b5-92ca-c5659456deb5",
    multiRule: "/MvcConfig/Auth/Res/RuleSelector?SelectMode=Multi&RootFullID=a1b10168-61a9-44b5-92ca-c5659456deb6",
    singleRes: "/MvcConfig/Auth/Res/Selector?SelectMode=Single&RootFullID=a1b10168-61a9-44b5-92ca-c5659456deb5",
    singleRule: "/MvcConfig/Auth/Res/RuleSelector?SelectMode=Single&RootFullID=a1b10168-61a9-44b5-92ca-c5659456deb6",
    singleEnum: "/MvcConfig/Meta/Enum/SingleSelector?EnumKey=",
    multiEnum: "/MvcConfig/Meta/Enum/MultiSelector?EnumKey="
};


/*---------------------------------------------------------Url常量结束--------------------------------------------------------*/
/*---------------------------------------------------------通用枚举开始--------------------------------------------------------*/

var yesNo = [{ "value": "1", "text": "是" }, { "value": "0", "text": "否"}];

/*---------------------------------------------------------通用枚举结束--------------------------------------------------------*/


/*---------------------------------------------------------页面加载过程开始----------------------------------------------------*/
var alreadyInit = false;        //是否已经执行过PageInit


//解决grid开始排序失效
var gridSortFields = {};
var gridSortOrder = {};
//window.status = "|a0." + new Date().getSeconds() + ":" + new Date().getMilliseconds();

$(document).ready(function () {

});

function preInit() {

    //附加只读样式
    $("input[enabled='false']").removeAttr("enabled").attr("readonly", "true").addClass("asLabel");

    //预处理树，树的获取数据Url增加当前地址栏参数
    $(".mini-tree").each(function () {
        var $tree = $(this);
        var url = $tree.attr("url");
        if (url != undefined && url != null && url != "") {
            url = changeToFullUrl(url);
            url = addUrlSearch(url); //url增加当前地址栏参数        
            $tree.attr("url", url);
        }
    });

    //预处理Outlook树，树的获取数据Url增加当前地址栏参数
    $(".mini-outlooktree").each(function () {
        var $tree = $(this);
        var url = $tree.attr("url");
        if (url != undefined && url != null && url != "") {
            url = changeToFullUrl(url);
            url = addUrlSearch(url); //url增加当前地址栏参数        
            $tree.attr("url", url);
        }
    });

    //预处理combobox，Url增加当前地址栏参数，并转化全路径
    $(".mini-combobox").each(function () {
        var $combobox = $(this);
        var url = $combobox.attr("url");
        if (url != undefined && url != null && url != "") {
            url = changeToFullUrl(url);
            url = addUrlSearch(url); //url增加当前地址栏参数        
            $combobox.attr("url", url);
        }
        if ($combobox.attr("valueField") == undefined || $combobox.attr("valueField") == "id") {
            $combobox.attr("valueField", "value");
        }
    });

    //预处理radiobuttonlist
    $(".mini-radiobuttonlist").each(function () {
        var $item = $(this);
        if ($item.attr("valueField") == undefined) {
            $item.attr("valueField", "value");
        }
    });

    //预处理checkboxlist
    $(".mini-checkboxlist").each(function () {
        var $item = $(this);
        if ($item.attr("valueField") == undefined) {
            $item.attr("valueField", "value");
        }
    });

    //预处理Grid
    $(".mini-datagrid").each(function () {
        var $item = $(this);
        //解决Grid控件开始排序字段失效问题
        gridSortFields[$item.attr("id")] = $item.attr("sortField");
        gridSortOrder[$item.attr("id")] = $item.attr("sortOrder");

        //Grid列头居中
        $item.find("div[field]").each(function () {
            var $item = $(this);
            $item.attr("headeralign", "center");
        });

        if ($item.attr("pagesize") == undefined)
            $item.attr("pagesize", "20");

        if ($item.attr("allowunselect") == undefined)
            $item.attr("allowunselect", "true");

        if ($item.attr("sizeList") == undefined)
            $item.attr("sizeList", "[10,20,50,100,200,300,500]");

    });

    //设置页面的window默认值
    $(".mini-window").each(function () {
        var $item = $(this);
        $item.attr("showmodal", true);
        $item.attr("allowresize", false);
        $item.attr("allowdrag", true);
    });

    //全部按钮plain设置为true
    $(".mini-button").each(function () {
        var $item = $(this);
        if ($item.attr("plain") == undefined)
            $item.attr("plain", true);
    });

    //字段编辑权限
    if (typeof (readonlyControl) != "undefined") {
        var arr = readonlyControl.split(',');
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] == "")
                continue;
            //容器
            $("#" + arr[i]).attr("readonly", "true").addClass("asLabel");
            $("#" + arr[i]).find("input").each(function (index, item) { $(item).attr("readonly", "true"); });

            //表单字段
            $("input[name='" + arr[i] + "']").attr("readonly", "true").addClass("asLabel");
            $("div[name='" + arr[i] + "']").attr("readonly", "true").addClass("asLabel");

            //列表Field
            if (arr[i].split('.').length = 2)
                $("#" + arr[i].split('.')[0] + " div[field='" + arr[i].split('.')[1] + "']").attr("readonly", "true").addClass("asLabel");
        }
    }

    //按钮字段权限,移除没有权限的控件
    if (typeof (noneAuthControl) != "undefined") {
        var arr = noneAuthControl.split(',');
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] == "")
                continue;
            //按钮或容器
            $("#" + arr[i]).remove();
            //表单字段
            $("input[name='" + arr[i] + "']").remove();
            $("div[name='" + arr[i] + "']").remove();

            //列表Field
            if (arr[i].split('.').length = 2)
                $("#" + arr[i].split('.')[0] + " div[field='" + arr[i].split('.')[1] + "']").remove();
        }
    }
    mini.parse();

    //临时性调整edit的表头固定的layout的滚轴处理
    var _layC = $(".mini-layout-region-body > div");
    _layC.each(function (i, n) { if (n.id == "formlayout") $(n).css("width", "96%"); });

    pageInit();
}

var formData; //add for data

function pageInit() {
    if (alreadyInit)
        return;
    alreadyInit = true;

    if (getQueryString("FuncType").toLowerCase() == "view") {
        if (getQueryString("TaskExecID") == "")
            $(".mini-toolbar").hide();
        setFormDisabled(); //设置表单为只读

        var ss = $(".mini-layout:has(#north .mini-toolbar)"); //查布局控件下必须有在north区域的toolbar，定位固定表头，隐藏高度
        if (ss.length > 0 && ss[0].id)
            mini.get(ss[0].id).hideRegion("north");
    }
    //调用界面的pageLoad方法
    if (typeof (pageLoad) != "undefined")
        pageLoad();
    if ($("#" + normalParamSettings.formId).length == 1) {
        if (!allowUrlOpenForm && window.parent == window) { //禁止从地址栏直接打开
            $(".mini-button").hide();
            alert("不能从地址栏直接打开页面，窗口将关闭！");
            closeWindow();
            return;
        }

        //流程的表单控制
        flowFormControl();


        var $form = $("#" + normalParamSettings.formId);
        if ($form.attr("autogetdata") != "false") {  //如果不自动获取表单数据则直接退出方法

            var formUrl = "GetModel";
            if ($form.attr("url") != undefined)
                formUrl = $form.attr("url");
            formUrl = changeToFullUrl(formUrl); //url转换为全路径
            formUrl = addUrlSearch(formUrl); //url增加当前地址栏参数
            //加载Form数据
            var form = new mini.Form("#" + normalParamSettings.formId);

            $.ajax({
                url: formUrl,
                type: "post",
                cache: false,
                success: function (text) {
                    var data = mini.decode(text);
                    if (data) {
                        formData = data; //add for data

                        //设置表单数据
                        form.setData(data);
                        form.setChanged(false);

                        //解决combobox的BUG
                        var fields = form.getFields();
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            if (field.type == "combobox") {
                                var v = field.getValue();
                                var t = field.getText();
                                if (t == "")
                                    field.setText(v);
                            }
                        }

                        //富文本框赋初始值
                        if (typeof (KindEditor) != "undefined") {
                            var arrTxtAreas = $("textarea.KindEditor");
                            if (arrTxtAreas.length == KindEditor.instances.length) {
                                $.each(arrTxtAreas, function (i, obj) {
                                    KindEditor.instances[i].html($.trim(data[$(obj).attr("name")]));
                                });
                            }
                        }
                    }

                    //将地址栏参数赋值给form的空值隐藏控件
                    $("form .mini-hidden").each(function () {
                        var name = $(this).attr("name");
                        if (hasQueryString(name)) {
                            var field = mini.getbyName(name);
                            if (field.getValue() == "")
                                field.setValue(getQueryString(name));
                        }
                    });

                    //大字段赋值给Grid
                    $("form .mini-datagrid").each(function () {
                        var id = $(this).attr("id");
                        if ((data || 0)[id] != undefined)
                            mini.get(id).setData(mini.decode(data[id]));
                    });

                    //调用界面上的onFormSetValue方法
                    if (typeof (onFormSetData) != "undefined")
                        onFormSetData(data);

                    //流程：加载FlowBar
                    var flowMenubar = mini.get("flowMenubar");
                    if (flowMenubar != undefined) {
                        flowLoadMenubar(new mini.Form("#" + normalParamSettings.formId).getData());
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var msg = getErrorFromHtml(jqXHR.responseText);
                    msgUI(msg, 4, function (act) {

                    });

                }
            });

        }
    }

    //加载Grid
    $(".mini-datagrid").each(function () {
        var grid = mini.get("#" + $(this).attr("id"));

        grid.on("loaderror", function (e) {
            var msg = getErrorFromHtml(e.errorMsg);
            msgUI(msg, 4);
        });


        //修正首次没有排序的错误
        if (gridSortFields[grid.id]) {
            grid.sortField = gridSortFields[grid.id];
        }
        if (gridSortOrder[grid.id]) {
            grid.sortOrder = gridSortOrder[grid.id];
        }

        //纠正小写ID的错误
        if (grid.idField == "id")
            grid.idField = "ID";

        //TODO:设置grid属性值

        //加载grid
        if (grid.url) {
            grid.setUrl(changeToFullUrl(grid.url));
            grid.setUrl(addUrlSearch(grid.url)); //url增加当前地址栏参数           
            grid.reload();
        }
    });


}


/*---------------------------------------------------------页面加载过程结束----------------------------------------------------*/

function clearDataForm(normalSettings) {
    var settings = $.extend(true, {}, normalParamSettings, normalSettings);
    var form = new mini.Form("#" + settings.formId);
    form.clear();
}

function clearQueryForm(normalSettings) {
    var settings = $.extend(true, {}, normalParamSettings, normalSettings);
    var form = new mini.Form("#" + settings.queryFormId);
    form.clear();
}

function search(normalSettings) {
    var settings = $.extend(true, {}, normalParamSettings, normalSettings);

    var _formId = $("#" + settings.queryWindowId).find("form").attr("id");
    var form = new mini.Form("#" + _formId);
    var grid = mini.get("#" + settings.gridId);

    var data = {};
    form.validate();
    if (form.isValid() == false) return;
    data = form.getData();
    if (grid != undefined)
        grid.load({ queryFormData: mini.encode(data) });

    hideWindow(settings.queryWindowId);
}

function quickSearch(searchFields, normalSettings) {
    var settings = $.extend(true, {}, normalParamSettings, normalSettings);

    var grid = mini.get("#" + settings.gridId);

    var data = {};
    var keyCo = mini.get(settings.queryBoxId);
    if (keyCo == undefined) {
        msgUI("当前快速查询文本框" + settings.queryBoxId + "不存在，请重新检查！", 1);
        return;
    }
    var keys = searchFields.split(',');
    for (i = 0, len = keys.length; i < len; i++) {
        data["$IL$" + keys[i]] = keyCo.getValue();
    }
    data["IsOrRelation"] = "True"; //快速查询条件间为或关系

    if (grid != undefined)
        grid.load({ queryFormData: mini.encode(data) });
}

function add(windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "增加" + listConfig.title;
    if (windowSettings.url == undefined)
        windowSettings.url = "Edit";

    var settings = $.extend(true, {}, windowParamSettings, listConfig, windowSettings);

    if (settings["createId"] == true || settings["isAutoId"] == true) {
        var url = changeToFullUrl("GetGuid");
        jQuery.ajax({
            url: url,
            type: "post",
            data: "",
            cache: false,
            async: settings.async,
            success: function (text, textStatus) {
                if (settings.url.indexOf('?') > 0)
                    settings.url += "&ID=" + text;
                else
                    settings.url += "?ID=" + text;
                openWindow(settings.url, settings);
            }
        });
    }
    else
        openWindow(settings.url, settings);
}

function edit(windowSettings) {

    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "编辑" + listConfig.title;
    if (windowSettings.url == undefined)
        windowSettings.url = "Edit";
    if (windowSettings.mustSelectOneRow == undefined)
        windowSettings.mustSelectOneRow = true;

    windowSettings.url = addSearch(windowSettings.url, "ID", "{ID}");

    var settings = $.extend(true, {}, windowParamSettings, listConfig, windowSettings);

    openWindow(settings.url, settings);
}

function view(windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "查看" + listConfig.title;
    if (windowSettings.url == undefined)
        windowSettings.url = "Edit";
    if (windowSettings.mustSelectOneRow == undefined)
        windowSettings.mustSelectOneRow = true;
    if (windowSettings.funcType == undefined)
        windowSettings.funcType = "view";
    windowSettings.url = addSearch(windowSettings.url, "ID", "{ID}");

    var settings = $.extend(true, {}, windowParamSettings, listConfig, windowSettings);

    openWindow(settings.url, settings);
}


function del(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "Delete";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "删除";
    if (execSettings.mustSelectRow == undefined)
        execSettings.mustSelectRow = true;
    if (execSettings.mustConfirm == undefined)
        execSettings.mustConfirm = true;
    if (execSettings.validateForm == undefined)
        execSettings.validateForm = false;

    var settings = $.extend(true, {}, executeParamSettings, execSettings);

    execute(settings.action, settings);
}

function save(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "Save";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "保存";
    if (execSettings.closeWindow == undefined)
        execSettings.closeWindow = true;

    if (execSettings.onComplete == undefined)
        execSettings.onComplete = function (text) {
            formData = mini.decode(text);
            msgUI(settings.actionTitle + "成功！");
            if (settings.closeWindow)
                closeWindow("refresh");
        };

    var settings = $.extend(true, {}, executeParamSettings, execSettings);



    execute(settings.action, settings);
}

function saveList(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "SaveList";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "保存";

    var settings = $.extend(true, {}, executeParamSettings, execSettings);

    execute(settings.action, settings);
}

function saveSortedList(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "SaveSortedList";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "保存";

    var settings = $.extend(true, {}, executeParamSettings, execSettings);
    var grid = mini.get(settings.gridId);
    addExecuteParam("SortedListData", grid.getData());
    addExecuteParam("DeletedListData", grid.getChanges("removed"));
    execute(settings.action, settings);
}

function addRow(newRowJson, normalSettings) {
    var settings = $.extend(true, {}, normalParamSettings, normalSettings);
    var dataGrid = mini.get("#" + settings.gridId);
    if (dataGrid == undefined)
        return;
    for (var key in newRowJson) {
        if (newRowJson[key] && typeof (newRowJson[key]) == "string")
            newRowJson[key] = replaceUrl(newRowJson[key], normalSettings);
    }
    if (settings["createId"] == true || settings["isAutoId"] == true) {
        var url = changeToFullUrl("GetGuid");
        jQuery.ajax({
            url: url,
            type: "post",
            data: "",
            cache: false,
            async: settings.async,
            success: function (text, textStatus) {
                newRowJson["ID"] = text;
                if (settings.isLast)
                    dataGrid.addRow(newRowJson);
                else
                    dataGrid.addRow(newRowJson, 0);
                dataGrid.validateRow(newRowJson);   //加入新行，马上验证新行
            }
        });
    }
    else {
        if (settings.isLast)
            dataGrid.addRow(newRowJson);
        else
            dataGrid.addRow(newRowJson, 0);
        dataGrid.validateRow(newRowJson);   //加入新行，马上验证新行
    }
}

function delRow(normalSettings) {
    var settings = $.extend(true, {}, normalParamSettings, normalSettings);
    var dataGrid = mini.get("#" + settings.gridId);
    if (dataGrid == undefined)
        return;
    var rows = dataGrid.getSelecteds();
    if (rows.length > 0) {
        dataGrid.removeRows(rows, true);
    }
}

/*---------------------------------------------------------Tree方法开始----------------------------------------------------*/

function saveNode(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "SaveNode";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "保存";
    if (execSettings.onComplete == undefined)
        execSettings.onComplete = returnForm;

    var settings = $.extend(true, {}, executeParamSettings, execSettings);

    execute(settings.action, settings);
}

function searchTree(normalSettings) {

    var settings = $.extend(true, {}, normalParamSettings, normalSettings);

    var tree = mini.get(settings.treeId);
    var key = mini.get(settings.queryTreeBoxId).getValue();
    if (key == "") {
        tree.clearFilter();
    } else {
        tree.filter(function (node) {
            var text = node[tree.textField];
            var isleaf = tree.isLeaf(node);
            if (text.indexOf(key) != -1) {
                return true;
            }
        });

        // 展开所有
        tree.expandAll();
    }
}

function nodeViewing(windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "查看" + treeConfig.title;
    if (windowSettings.url == undefined)
        windowSettings.url = "NodeEdit";
    if (windowSettings.mustSelectNode == undefined)
        windowSettings.mustSelectNode = true;
    if (windowSettings.funcType == undefined)
        windowSettings.funcType = "view";
    if (windowSettings.paramFrom == undefined)
        windowSettings.paramFrom = "dataTree";

    windowSettings.url = addSearch(windowSettings.url, "ID", "{ID}");

    var settings = $.extend(true, {}, windowParamSettings, treeConfig, windowSettings);

    openWindow(settings.url, settings);
}

function nodeEditing(windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "编辑" + treeConfig.title;
    if (windowSettings.url == undefined)
        windowSettings.url = "NodeEdit";
    if (windowSettings.mustSelectNode == undefined)
        windowSettings.mustSelectNode = true;
    if (windowSettings.onDestroy == undefined)
        windowSettings.onDestroy = nodeEdited;
    if (windowSettings.paramFrom == undefined)
        windowSettings.paramFrom = "dataTree";

    windowSettings.url = addSearch(windowSettings.url, "ID", "{ID}");

    var settings = $.extend(true, {}, windowParamSettings, treeConfig, windowSettings);

    openWindow(settings.url, settings);
}

function nodeEdited(data, windowSettings) {
    if (typeof (data) != "object")
        return;
    var tree = mini.get(windowSettings.treeId);
    var node;
    if (tree.showCheckBox)
        node = tree.getCheckedNodes()[0];
    else
        node = tree.getSelectedNode();
    tree.updateNode(node, data);
}

function nodeAdding(windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "增加" + treeConfig.title;
    if (windowSettings.url == undefined)
        windowSettings.url = "NodeEdit";
    if (windowSettings.mustSelectNode == undefined)
        windowSettings.mustSelectNode = true;
    if (windowSettings.onDestroy == undefined)
        windowSettings.onDestroy = nodeAdded;
    if (windowSettings.paramFrom == undefined)
        windowSettings.paramFrom = "dataTree";

    windowSettings.url = addSearch(windowSettings.url, "ParentID", "{ID}");
    windowSettings.url = addSearch(windowSettings.url, "FullID", "{FullID}");


    var settings = $.extend(true, {}, windowParamSettings, treeConfig, windowSettings);

    openWindow(settings.url, settings);
}

function nodeAdded(data, windowSettings) {
    if (typeof (data) != "object")
        return;
    var tree = mini.get(windowSettings.treeId);

    var node;
    if (tree.showCheckBox)
        node = tree.getCheckedNodes()[0];
    else
        node = tree.getSelectedNode();

    tree.addNode(data, "add", node);
    tree.expandNode(node);
}

function nodeDeleting(execSetting) {
    execSetting = $.extend(true, {}, execSetting);
    if (execSetting.action == undefined)
        execSetting.action = "DeleteNode";
    if (execSetting.actionTitle == undefined)
        execSetting.actionTitle = "删除";
    if (execSetting.mustSelectNode == undefined)
        execSetting.mustSelectNode = true;
    if (execSetting.mustConfirm == undefined)
        execSetting.mustConfirm = true;
    if (execSetting.onComplete == undefined)
        execSetting.onComplete = nodeDeleted;
    if (execSetting.paramFrom == undefined)
        execSetting.paramFrom = "dataTree";

    execSetting.action = addSearch(execSetting.action, "FullID", "{FullID}");

    var settings = $.extend(true, {}, executeParamSettings, execSetting);

    execute(settings.action, settings);
}

function nodeDeleted(data, execSettings) {
    var tree = mini.get(execSettings.treeId);

    var node;
    if (tree.showCheckBox)
        node = tree.getCheckedNodes()[0];
    else
        node = tree.getSelectedNode();

    tree.removeNode(node);
}

function onNodeDroping(e) {
    //不能拖放到非同级节点的前后
    if ((e.effect == "before" || e.effect == "after") && e.targetNode.ParentID != e.node.ParentID)
        e.effect = "no";
}

function onNodeDrop(e) {
    addExecuteParam("dragNode", mini.encode(e.dragNode));
    addExecuteParam("dropNode", mini.encode(e.dropNode));
    addExecuteParam("dragAction", e.dragAction);
    execute("DropNode", { validateForm: false });
}

//关联数据选择,windowSetting默认fullRelation为false
function relationAppending(url, windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "选择";
    if (windowSettings.mustSelectNode == undefined)
        windowSettings.mustSelectNode = true;
    if (windowSettings.onDestroy == undefined)
        windowSettings.onDestroy = relationAppended;
    if (windowSettings.paramFrom == undefined)
        windowSettings.paramFrom = "dataTree";
    if (windowSettings.action == undefined)
        windowSettings.action = "AppendRelation";
    if (windowSettings.actionTitle == undefined)
        windowSettings.actionTitle = "保存";

    var windowSettings = $.extend(true, {}, relationConfig, windowSettings);

    openWindow(url, windowSettings);
}

//关联数据选择完成
function relationAppended(data, settings) {
    if (data == undefined || data == "close" || data.length == 0)
        return;
    settings = $.extend(true, {}, executeParamSettings, settings);

    var node = mini.get(settings.treeId).getSelectedNode();

    addExecuteParam("NodeFullID", node.FullID);
    addExecuteParam("RelationData", mini.encode(data));
    addExecuteParam("FullRelation", settings.fullRelation);

    execute(settings.action, settings);
}

//关联数据选择,windowSetting默认fullRelation为false
function relationSetting(url, windowSettings) {
    windowSettings = $.extend(true, {}, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "选择";
    if (windowSettings.mustSelectNode == undefined)
        windowSettings.mustSelectNode = true;
    if (windowSettings.onDestroy == undefined)
        windowSettings.onDestroy = relationSet;
    if (windowSettings.paramFrom == undefined)
        windowSettings.paramFrom = "dataTree";
    if (windowSettings.action == undefined)
        windowSettings.action = "SetRelation";
    if (windowSettings.actionTitle == undefined)
        windowSettings.actionTitle = "保存";

    var windowSettings = $.extend(true, {}, relationConfig, windowSettings);

    openWindow(url, windowSettings);
}

//关联数据选择完成
function relationSet(data, settings) {
    if (data == undefined || data == "close")
        return;

    settings = $.extend(true, {}, executeParamSettings, settings);
    var node = mini.get(settings.treeId).getSelectedNode();

    addExecuteParam("NodeFullID", node.FullID);
    addExecuteParam("RelationData", mini.encode(data));
    addExecuteParam("FullRelation", settings.fullRelation);

    execute(settings.action, settings);
}

//关联数据删除，execSettings默认fullRelation为false,execSettings中可以包含参数relationData
function delRelation(execSettings) {
    execSettings = $.extend({ fullRelation: false }, execSettings);
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "删除";
    if (execSettings.mustConfirm == undefined)
        execSettings.mustConfirm = true;
    if (execSettings.action == undefined)
        execSettings.action = "DeleteRelation";

    execSettings = $.extend(true, {}, executeParamSettings, execSettings);

    if (execSettings.nodeFullID == "") {
        var node = mini.get(execSettings.treeId).getSelectedNode();
        if (node == null) {
            msgUI("当前没有选择要操作的节点，请重新确认！", 1);
            return;
        }
        execSettings.nodeFullID = node.FullID;
    }

    if (execSettings.relationData == "") {
        var rows = mini.get(execSettings.gridId).getSelecteds();
        if (rows.length == 0) {
            msgUI("当前没有选择要操作的记录，请重新确认！", 1);
            return;
        }
        execSettings.relationData = rows;
    }

    addExecuteParam("NodeFullID", execSettings.nodeFullID);
    addExecuteParam("RelationData", mini.encode(execSettings.relationData));
    addExecuteParam("FullRelation", execSettings.fullRelation);


    execute(execSettings.action, execSettings);
}

function addRelationData(windowSettings) {

    windowSettings = $.extend({ fullRelation: false }, windowSettings);
    if (windowSettings.title == undefined)
        windowSettings.title = "增加" + relationConfig.title;
    if (windowSettings.mustSelectNode == undefined)
        windowSettings.mustSelectNode = true;
    if (windowSettings.paramFrom == undefined)
        windowSettings.paramFrom = "dataTree";
    if (windowSettings.url == undefined)
        windowSettings.url = "RelationDataEdit";

    windowSettings = $.extend(true, {}, windowParamSettings, windowSettings);
    var node = mini.get(windowSettings.treeId).getSelectedNode();
    if (node == null) {
        msgUI("当前没有选择要操作的节点，请重新确认！", 1);
        return;
    }

    windowSettings.url = addSearch(windowSettings.url, "NodeFullID", node.FullID);
    windowSettings.url = addSearch(windowSettings.url, "FullRelation", windowSettings.fullRelation);

    var windowSettings = $.extend(true, {}, relationConfig, windowSettings);

    openWindow(windowSettings.url, windowSettings);
}

function editRelationData(windowSettings) {
    windowSettings = $.extend({ url: "RelationDataEdit" }, windowSettings);

    if (windowSettings.title == undefined)
        windowSettings.title = "编辑" + relationConfig.title;
    if (windowSettings.width == undefined)
        windowSettings.width = relationConfig.width;
    if (windowSettings.height == undefined)
        windowSettings.height = relationConfig.height;

    edit(windowSettings);
}

function viewRelationData(windowSettings) {
    windowSettings = $.extend({ url: "RelationDataEdit" }, windowSettings);

    if (windowSettings.title == undefined)
        windowSettings.title = "查看" + relationConfig.title;
    if (windowSettings.width == undefined)
        windowSettings.width = relationConfig.width;
    if (windowSettings.height == undefined)
        windowSettings.height = relationConfig.height;

    view(windowSettings);
}

function saveRelationData(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "SaveRelationData";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "保存";
    if (execSettings.onComplete == undefined)
        execSettings.onComplete = returnForm;

    var settings = $.extend(true, {}, executeParamSettings, execSettings);

    execute(settings.action, settings);
}

//关联数据删除
function delRelationData(execSettings) {
    execSettings = $.extend(true, {}, execSettings);
    if (execSettings.action == undefined)
        execSettings.action = "DeleteRelationData";
    if (execSettings.actionTitle == undefined)
        execSettings.actionTitle = "删除";
    if (execSettings.mustConfirm == undefined)
        execSettings.mustConfirm = true;
    execSettings = $.extend(true, {}, executeParamSettings, execSettings);

    var node = mini.get(execSettings.treeId).getSelectedNode();
    var relationData = mini.get(execSettings.gridId).getSelecteds();

    if (node == null) {
        msgUI("当前没有选择要操作的节点，请重新确认！", 1);
        return;
    }
    if (relationData.length == 0) {
        msgUI("当前没有选择要操作的记录，请重新确认！", 1);
        return;
    }
    addExecuteParam("RelationData", mini.encode(relationData));
    execute(execSettings.action, execSettings);
}

/*---------------------------------------------------------Tree方法结束----------------------------------------------------*/

/*---------------------------------------------------------选人选部门开始--------------------------------------------------------*/

function returnForm(windowSettings) {
    windowSettings = $.extend(true, {}, windowParamSettings, windowSettings);
    var form = new mini.Form(windowSettings.formId);
    closeWindow(form.getData());
}

function returnList(windowSettings) {
    windowSettings = $.extend(true, {}, windowParamSettings, windowSettings);
    var grid = mini.get(windowSettings.gridId);
    closeWindow(grid.getSelecteds());
}

function addSingleUserSelector(name, selectorSettings) {
    selectorSettings = $.extend({ width: 520, height: 480, selectMode: "single", targetType: "form", allowResize: false }, selectorSettings);
    if (selectorSettings.targetType == "form") {
        selectorSettings.returnParams = "value:ID,text:Name," + $.trim(selectorSettings.returnParams);
        addAutoUserSelect(name, selectorSettings);
    }
    var url = urlConstant.singleUser;
    if (selectorSettings.OrgIDs || selectorSettings.RoleIDs) {
        url = urlConstant.singleScopeUser;
    }
    if (selectorSettings.OrgIDs)
        url = url + "&OrgIDs=" + selectorSettings.OrgIDs;
    if (selectorSettings.RoleIDs)
        url = url + "&RoleIDs=" + selectorSettings.RoleIDs;
    addSelector(name, url, selectorSettings);
}

function addMultiUserSelector(name, selectorSettings) {
    selectorSettings = $.extend({ width: 750, height: 595, selectMode: "multi", targetType: "form", allowResize: false }, selectorSettings);
    if (selectorSettings.targetType == "form") {
        selectorSettings.returnParams = "value:ID,text:Name," + $.trim(selectorSettings.returnParams);
        addAutoUserSelect(name, selectorSettings);
    }
    var url = urlConstant.multiUser;
    if (selectorSettings.OrgIDs || selectorSettings.RoleIDs)
        url = urlConstant.multiScopeUser;
    if (selectorSettings.OrgIDs)
        url = url + "&OrgIDs=" + selectorSettings.OrgIDs;
    if (selectorSettings.RoleIDs)
        url = url + "&RoleIDs=" + selectorSettings.RoleIDs;
    addSelector(name, url, selectorSettings);
}

function addSingleOrgSelector(name, selectorSettings) {
    selectorSettings = $.extend({ width: 320, height: 580, allowResize: false }, selectorSettings);
    var url = urlConstant.singleOrg;
    if (selectorSettings.OrgType)
        url += "&OrgType=" + selectorSettings.OrgType;
    addSelector(name, url, selectorSettings);
}

function addMultiOrgSelector(name, selectorSettings) {
    selectorSettings = $.extend({ width: 320, height: 580, allowResize: false }, selectorSettings);
    var url = urlConstant.multiOrg;
    if (selectorSettings.OrgType)
        url += "&OrgType=" + selectorSettings.OrgType;
    addSelector(name, url, selectorSettings);
}

function addSingleRoleSelector(name, selectorSettings) {
    selectorSettings = $.extend({ width: 720, height: 580, allowResize: false }, selectorSettings);
    addSelector(name, urlConstant.singleRole, selectorSettings);
}

function addMultiRoleSelector(name, selectorSettings) {
    selectorSettings = $.extend({ width: 720, height: 580, allowResize: false }, selectorSettings);
    addSelector(name, urlConstant.multiRole, selectorSettings);
}
function showHelp() {
    msgUI("帮助手册制作中...");
}
/*---------------------------------------------------------选人选部门结束--------------------------------------------------------*/

/*---------------------------------------------------------流程方法开始--------------------------------------------------------*/

//
var flowExecMethod = null;

var flowCode = getQueryString("FlowCode");
//(此方法为私有方法)给Url追加流程编号。（启动流程时，最终确定FlowCode，以便做到表单数据可以改变启动的流程）
function flowUrlAppendFlowCode(url) {

    if (flowCode == "")//当前地址栏没有FlowCode则不追加
        return url;

    if (url.indexOf('?') > 0)
        url += '&';
    else
        url += '?';
    url += "FlowCode=" + flowCode;

    return url;
}


//加载流程菜单条
function flowLoadMenubar(jsonFormData, flowCode) {
    if (flowCode)
        this.flowCode = flowCode;

    var menubar = mini.get("flowMenubar");
    if (!menubar)
        return;
    menubar.disable();

    if (!jsonFormData)
        jsonFormData = new mini.Form("dataForm").getData();

    jsonFormData = mini.encode(jsonFormData);

    var url = flowUrlAppendFlowCode("GetFlowButtons"); //如果存在flowCode则直接追加（流程切换）
    url = changeToFullUrl(url); //url转化为全路径   
    url = addUrlSearch(url); //url增加当前地址栏参数

    $.ajax({
        url: url,
        type: "post",
        data: { formData: jsonFormData },
        cache: false,
        success: function (text) {
            var data = mini.decode(text);
            menubar.loadList(data, "id", "pid");
            menubar.enable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var msg = jqXHR.responseText.toLowerCase();
            if (msg.indexOf("<title>") > 0) {
                msg = jqXHR.responseText.split('</title>')[0];
                msg = msg.split('<title>')[1];
                msg = msg.replace("<br>", "\r\n");
                msg = msg.replace("<br>", "\r\n");
                msg = msg.replace("<br>", "\r\n");
                msg = msg.replace("<br>", "\r\n");
                msg = msg.replace("<br>", "\r\n");
            }
            msgUI(msg, 4);
        }
    });

}

//流程的表单控制
function flowFormControl() {

    if (getQueryString("FlowCode") == "" && getQueryString("TaskExecID") == "")
        return;
    //MvcConfig站点只有MvcConfig/UI/Form/PageView使用表单控制
    if (window.location.href.indexOf("MvcConfig") >= 0 && window.location.href.indexOf("MvcConfig/UI/Form/PageView") <= 0)
        return;

    var url = flowUrlAppendFlowCode("GetFormControlInfo"); //如果存在flowCode则直接追加（流程切换）
    url = changeToFullUrl(url); //url转化为全路径   
    url = addUrlSearch(url); //url增加当前地址栏参数
    $.ajax({
        url: url,
        type: "post",
        cache: false,
        error: function (jqXHR, textStatus, errorThrown) {
            msgUI("获取流程控制信息失败：GetFormControlInfo");
        },
        success: function (text) {
            var data = mini.decode(text);

            var HiddenElements = data.HiddenElements.split(',');
            var VisibleElements = data.VisibleElements.split(',');
            var EditableElements = data.EditableElements.split(',');
            var DisableElements = data.DisableElements.split(',');

            for (var i = 0; i < HiddenElements.length; i++) {
                var eleKey = HiddenElements[i];

                if (eleKey == "") continue;
                var obj = mini.getbyName(eleKey);
                if (!obj)
                    obj = mini.get(eleKey);
                if (obj) {
                    obj.hide();
                }
                else {
                    $("#" + eleKey).hide();
                }
            }
            for (var i = 0; i < DisableElements.length; i++) {
                var eleKey = DisableElements[i];
                if (eleKey == "") continue;
                if (eleKey == normalParamSettings.formId)
                    setFormDisabled();
                else {
                    var obj = mini.getbyName(eleKey);
                    if (!obj)
                        obj = mini.get(eleKey);
                    if (obj) {
                        if (obj.type == "datagrid") {
                            obj.setAllowCellEdit(false);
                            obj.setAllowCellSelect(false);
                        }
                        else {
                            if (obj.setReadOnly) obj.setReadOnly(true);
                            if (obj.addCls) obj.addCls("asLabel");         //增加asLabel外观
                        }
                    }
                    else {
                        $("#" + eleKey).find("[name]").each(function (index) {
                            obj = mini.getbyName($(this).attr("name"));
                            if (obj && obj.setReadOnly) obj.setReadOnly(true);
                            if (obj && obj.addCls) obj.addCls("asLabel");         //增加asLabel外观
                        });
                    }
                }
            }
            for (var i = 0; i < VisibleElements.length; i++) {
                var eleKey = VisibleElements[i];

                if (eleKey == "") continue;
                if (eleKey == "toolbar")
                    $(".mini-toolbar").show();
                var obj = mini.getbyName(eleKey);
                if (!obj)
                    obj = mini.get(eleKey);
                if (obj) {
                    obj.show();
                }
                else {
                    $("#" + eleKey).show();
                }
            }
            for (var i = 0; i < EditableElements.length; i++) {
                var eleKey = EditableElements[i];
                if (eleKey == "") continue;
                var obj = mini.getbyName(eleKey);
                if (!obj)
                    obj = mini.get(eleKey);
                if (obj) {
                    if (obj.type == "datagrid") {
                        obj.setAllowCellEdit(true);
                        obj.setAllowCellSelect(true);
                    }
                    else {
                        if (obj.setReadOnly) obj.setReadOnly(false);
                        if (obj.setEnabled) obj.setEnabled(true);
                        if (obj.removeCls) obj.removeCls("asLabel");         //增加asLabel外观
                    }
                }
                else {
                    $("#" + eleKey).find("[name]").each(function (index) {
                        obj = mini.getbyName($(this).attr("name"));
                        if (obj && obj.setReadOnly) obj.setReadOnly(false);
                        if (obj && obj.setEnabled) obj.setEnabled(true);
                        if (obj && obj.removeCls) obj.removeCls("asLabel");         //增加asLabel外观
                    });
                }
            }
            //重新计算布局高度等
            mini.layout();
        }
    });
}

//流程提交方法
var flowRouting; // 流程操作参数
function flowSubmitting(flowRouting) {

    if (typeof (onFormSubmitting) != "undefined") {
        if (!onFormSubmitting(flowRouting))
            return;
    }

    var notNullFields = flowRouting.notNullFields.split(',');
    for (var i = 0; i < notNullFields.length; i++) {
        if (notNullFields[i] == "")
            continue;
        var ctrl = mini.getbyName(notNullFields[i]);
        if (ctrl)
            ctrl.required = true;
    }

    //验证输入
    if (!validateInput()) {
        msgUI("当前输入的信息有误，请重新检查！", 1);
        return;
    }

    mini.getbyName("Advice").setValue(flowRouting.defaultComment);

    this.flowRouting = flowRouting;

    if (this.flowRouting.mustInputComment == true) {//如果需要输入意见
        flowExecMethod = flowExecUserSelecting;
        showWindow('adviceWindow');
    }
    else {
        flowExecUserSelecting();
    }
}

//流程执行人选择
function flowExecUserSelecting() {
    var url = "";

    //流程结束需要选人（启动新流程）
    //    //流程结束不需要选人
    //    if (this.flowRouting.flowComplete == true) {
    //        flowSubmit("", "", "", "");
    //        return;
    //    }

    //多个分支的时候直接提交，在后台取人
    if (this.flowRouting.routingID.split(',').length > 1) {
        flowSubmit("", "", "", "");
        return;
    }

    //下一步执行数据取自表单
    if (flowRouting.orgIDFromField != "")
        flowRouting.orgIDs = mini.getbyName(flowRouting.orgIDFromField).getValue();
    if (flowRouting.roleIDsFromField != "")
        flowRouting.roleIDs = mini.getbyName(flowRouting.roleIDsFromField).getValue();
    if (flowRouting.userIDsFromField != "")
        flowRouting.userIDs = mini.getbyName(flowRouting.userIDsFromField).getValue();
    if (flowRouting.userIDsGroupFromField != "")
        flowRouting.userIDsGroup = mini.getbyName(flowRouting.userIDsGroupFromField).getValue();
    //角色或部门取自表单时,此ajax方法同步执行
    if (flowRouting.orgIDFromField != "" || flowRouting.roleIDsFromField != "") {
        if (this.flowRouting.selectMode != "SelectOneOrg" && this.flowRouting.selectMode != "SelectMultiOrg") {
            var url = "GetUserIDs?roleIDs=" + flowRouting.roleIDs + "&orgIDs=" + flowRouting.orgIDs;
            url = changeToFullUrl(url);
            jQuery.ajax({
                url: url,
                type: "post",
                data: {},
                cache: false,
                async: false,
                success: function (data, textStatus) {
                    flowRouting.userIDs = data;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    msgUI("获取用户失败：" + url);
                }
            });
        }
    }

    //已经有执行人
    if (this.flowRouting.userIDs != "" && this.flowRouting.selectAgain == false && this.flowRouting.selectMode != 'SelectDoback') {
        //无需再单选人或多选人
        if (this.flowRouting.selectMode == "SelectOneUser" || this.flowRouting.selectMode == "SelectMultiUser") {
            flowSubmit(flowRouting.userIDs, flowRouting.userIDsGroup, flowRouting.roleIDs, flowRouting.orgIDs);
            return;
        }
        //如果执行人为一个，则无需再范围单选人
        if (this.flowRouting.selectMode == "SelectOneUserInScope" || this.flowRouting.selectMode == "SelectMultiUserInScope") {
            if (flowRouting.userIDs.split(',').length == 1) {
                flowSubmit(flowRouting.userIDs, flowRouting.userIDsGroup, flowRouting.roleIDs, flowRouting.orgIDs);
                return;
            }
        }
    }

    switch (this.flowRouting.selectMode) {
        case "SelectOneUser":
            url = urlConstant.singleUser;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '单选人', width: 520, height: 480 });
            break;
        case "SelectMultiUser":
            url = urlConstant.multiUser;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '多选人', width: 750, height: 595 });
            break;
        case "SelectOneUserInScope":
            url = urlConstant.singleScopeUser + "&OrgIDs=" + flowRouting.orgIDs + "&RoleIDs=" + flowRouting.roleIDs;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '范围单选人', width: 550, height: 600 });
            break;
        case "SelectMultiUserInScope":
            url = urlConstant.multiScopeUser + "&OrgIDs=" + flowRouting.orgIDs + "&RoleIDs=" + flowRouting.roleIDs;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '范围多选人', width: 550, height: 600 });
            break;
        case "SelectOneOrg":
            url = urlConstant.singleOrg;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '单选部门', width: 320, height: 580 });
            break;
        case "SelectMultiOrg":
            url = urlConstant.multiOrg;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '多选部门', width: 320, height: 580 });
            break;
        case "SelectDoback":
            url = "/MvcConfig/Auth/User/DobackSelector?UserIDs" + this.flowRouting.userIDs;
            openWindow(url, { onDestroy: flowExecUserSelected, title: '范围重新选人', width: 550, height: 600 });
            break;
        default:
            flowSubmit(flowRouting.userIDs, flowRouting.userIDsGroup, flowRouting.roleIDs, flowRouting.orgIDs);
            break;
    }
}

function flowExecUserSelected(data, setting) {
    if (data == undefined || data == "close" || data.length == 0)
        return;
    data = mini.decode(data);

    if (flowRouting.selectMode == "SelectOneOrg" || flowRouting.selectMode == "SelectMultiOrg") {
        var roleIDs = flowRouting.roleIDs;
        var orgIDs = getValues(data, "ID");

        var url = "GetUserIDs?roleIDs=" + roleIDs + "&orgIDs=" + orgIDs;
        url = changeToFullUrl(url);
        jQuery.ajax({
            url: url,
            type: "post",
            data: {},
            cache: false,
            async: "async",
            success: function (data, textStatus) {
                var userIDs = data;
                flowSubmit(userIDs, "", roleIDs, orgIDs);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                msgUI("获取用户失败：" + url);
            }
        });
    }
    else {
        var userIDs = getValues(data, "ID");
        flowSubmit(userIDs, "", flowRouting.roleIDs, flowRouting.orgIDs);
    }
}

function flowSubmit(nextExecUserIDs, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs) {

    if (flowRouting.routingID.indexOf(',') > 0) {//分支路由
        mini.confirm("确认要" + flowRouting.routingName + "吗？", "提交确认", function (e1) {
            if (e1 == "ok")
                flowSubmitFunc(nextExecUserIDs, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs);
        });
    }
    else if (nextExecUserIDs != "") {
        var url = changeToFullUrl("UserNames?userIDs=" + nextExecUserIDs); //url转化为全路径

        $.ajax({
            url: url,
            type: "post",
            cache: false,
            error: function (jqXHR, textStatus, errorThrown) {
                msgUI("获取用户失败：" + url);
            },
            success: function (text) {
                var msg = "<div >";
                var arr = ("接收人：" + text).split(',');
                var index = 1;
                for (var i = 0; i < arr.length; i++) {
                    if (i < arr.length - 1)
                        msg += arr[i] + "，";
                    else
                        msg += arr[i];

                    index++;
                    if (index > 5) {
                        index = 0;
                        msg += "<br/>";
                    }
                }
                if (text.split(',').length > 1)
                    msg += "(共" + text.split(',').length + "人)";

                msg += "</div>";


                mini.confirm(msg, "确认要" + flowRouting.routingName + "吗？", function (e1) {
                    if (e1 == "ok")
                        flowSubmitFunc(nextExecUserIDs, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs);
                });
            }
        });
    }
    else {
        mini.confirm("确认要" + flowRouting.routingName + "吗？", "提交确认", function (e1) {
            if (e1 == "ok")
                flowSubmitFunc(nextExecUserIDs, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs);
        });
    }
}

//流程提交到后台执行
function flowSubmitFunc(nextExecUserIDs, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs) {
    addExecuteParam("RoutingID", flowRouting.routingID);
    addExecuteParam("NextExecUserIDs", nextExecUserIDs);
    addExecuteParam("NextExecUserIDsGroup", nextExecUserIDsGroup);
    addExecuteParam("NextExecRoleIDs", nextExecRoleIDs);
    addExecuteParam("NextExecOrgIDs", nextExecOrgIDs);
    addExecuteParam("ExecComment", mini.getbyName("Advice").getValue());
    addExecuteParam("FlowCode", flowCode);

    var loadingMessageId = mini.loading("提交中...", "表单提交", { width: 300 });

    var url = "Submit?Urgency=" + mini.getbyName("urgency").getValue();
    url = flowUrlAppendFlowCode(url); //如果存在flowCode则直接追加（流程切换）
    execute(url, { loadingMessageId: loadingMessageId, actionTitle: flowRouting.routingName, onComplete: function (data, settings) {
        formData = mini.decode(data);
        if (getQueryString("closeForm") == "false") {
            flowLoadMenubar(new mini.Form("dataForm").getData());
        }
        else if (flowRouting.closeForm == true) {
            closeWindow("refresh");
        }
        else {
            //弱控不关闭页面时，刷新整个页面
            mini.hideMessageBox(loadingMessageId);
            window.location.reload();
        }

        if (typeof (onFormSubmit) != "undefined") {
            if (!onFormSubmit(flowRouting))
                return;
        }
    }
    });
}

function flowDoBack(taskExecId, routingId, title) {
    if (typeof (onFormSubmitting) != "undefined") {
        if (!onFormSubmitting())
            return;
    }
    flowExecMethod = function () {
        msgUI("确定" + title + "吗？", 2, function (act) {
            if (act == "ok") {
                execSettings = { validateForm: false, onComplete: function (text) {
                    if (typeof (onFormSubmit) != "undefined") {
                        if (onFormSubmit())
                            closeWindow("");
                    }
                    else
                        closeWindow("");
                }
                };
                var url = "DoBack?TaskExecID=" + taskExecId + "&routingID=" + routingId;
                addExecuteParam("ExecComment", mini.getbyName("Advice").getValue());
                execute(url, execSettings);
            }
        });
    };
    showWindow('adviceWindow');
}

function flowDoBackFirst(taskExecId) {
    if (typeof (onFormSubmitting) != "undefined") {
        if (!onFormSubmitting())
            return;
    }
    flowExecMethod = function () {
        msgUI("确定驳回首环节吗？", 2, function (act) {
            if (act == "ok") {
                execSettings = { validateForm: false, onComplete: function (text) {
                    if (typeof (onFormSubmit) != "undefined") {
                        if (onFormSubmit())
                            closeWindow("");
                    }
                    else
                        closeWindow("");
                }
                };
                var url = "DoBackFirst?TaskExecID=" + taskExecId;
                addExecuteParam("ExecComment", mini.getbyName("Advice").getValue());
                execute(url, execSettings);
            }
        });
    };
    showWindow('adviceWindow');
}

function flowDoBackFirstReturn(taskExecId) {
    if (typeof (onFormSubmitting) != "undefined") {
        if (!onFormSubmitting())
            return;
    }
    flowExecMethod = function () {
        msgUI("确定送驳回人吗？", 2, function (act) {
            if (act == "ok") {
                execSettings = { validateForm: false, onComplete: function (text) {
                    if (typeof (onFormSubmit) != "undefined") {
                        if (onFormSubmit())
                            closeWindow("");
                    }
                    else
                        closeWindow("");
                }
                };
                var url = "DoBackFirstReturn?TaskExecID=" + taskExecId;
                addExecuteParam("ExecComment", mini.getbyName("Advice").getValue());
                execute(url, execSettings);
            }
        });
    };
    showWindow('adviceWindow');
}

function flowTrace(winSettings) {
    var settings = $.extend({ title: '流程跟踪', width: 1000 }, winSettings);
    openWindow('/MvcConfig/Workflow/Trace/Diagram?ID={ID}&FuncType=FlowTrace', settings);
}

function flowExport(tmplCode, id) {
    ExportWord(tmplCode, id);
}

function flowSave(execSettings) {
    if (typeof (onFormSaving) != "undefined") {
        if (!onFormSaving())
            return;
    }

    execSettings = $.extend({ closeWindow: false, refresh: false, validateForm: false }, execSettings);
    save(execSettings);

    if (typeof (onFormSaved) != "undefined") {
        if (!onFormSaved())
            return;
    }
}

function flowDelete(execSettings) {
    execSettings = $.extend({ actionTitle: '删除', validateForm: false, mustConfirm: true, closeWindow: true }, execSettings);
    if (getQueryString("closeForm") == "false") {
        execSettings.closeWindow = false;
    }
    execute('Delete', execSettings);
}

function flowAdd(flowCode, windowSettings) {
    settings = $.extend({
        url: 'Edit?FlowCode=' + flowCode, onDestroy: function () {
            var grid = mini.get('dataGrid');
            if (grid) grid.reload();
        }
    }, windowSettings);

    if (settings.url.indexOf('FlowCode') < 0) {
        if (settings.url.indexOf('?') < 0) {
            settings.url += "?";
        }
        else {
            settings.url += "&";
        }
        settings.url += "FlowCode=" + flowCode;
    }

    add(settings);
}

function flowEdit(flowCode, windowSettings) {
    settings = $.extend({
        url: 'Edit?ID={ID}&FlowCode=' + flowCode, onDestroy: function () {
            var grid = mini.get('dataGrid');
            if (grid) grid.reload();
        }
    }, windowSettings);

    if (settings.url.indexOf('FlowCode') < 0) {
        if (settings.url.indexOf('?') < 0) {
            settings.url += "?";
        }
        else {
            settings.url += "&";
        }
        settings.url += "FlowCode=" + flowCode;
    }
}

function flowWithdraw() {
    execute("DeleteFlow", { mustConfirm: true, actionTitle: '撤销', onComplete: function (data, settings) {
        flowLoadMenubar(new mini.Form("dataForm").getData());
    }
    });
}

function flowWithdrawAsk() {
    execute("WithdrawAskTask", { mustConfirm: true, actionTitle: '撤销', onComplete: function (data, settings) {
        flowLoadMenubar(new mini.Form("dataForm").getData());
    }
    });
}

function flowDelegating() {
    var url = urlConstant.singleUser;
    openWindow(url, { onDestroy: flowDelegated });

}
function flowDelegated(data) {
    if (data == undefined || data == "close" || data.length == 0)
        return;
    addExecuteParam("NextExecUserIDs", getValues(data, "ID"));
    execute("DelegateTask", { actionTitle: '委托', onComplete: function (data, settings) {
        closeWindow("refresh");
    }
    });
}

function flowCirculating() {
    var url = urlConstant.multiUser;
    openWindow(url, { onDestroy: flowCirculated });
}
function flowCirculated(data) {
    if (data == undefined || data == "close" || data.length == 0)
        return;
    addExecuteParam("NextExecUserIDs", getValues(data, "ID"));
    execute("CirculateTask", { actionTitle: '传阅', onComplete: function (data, settings) {
        flowLoadMenubar(new mini.Form("dataForm").getData());
    }
    });
}

function flowAsking() {
    var url = urlConstant.multiUser;
    openWindow(url, { onDestroy: flowAsked });
}

function flowAsked(data) {
    if (data == undefined || data == "close" || data.length == 0)
        return;
    addExecuteParam("NextExecUserIDs", getValues(data, "ID"));
    execute("AskTask", { actionTitle: '加签', onComplete: function (data, settings) {
        flowLoadMenubar(new mini.Form("dataForm").getData());
    }
    });
}

function flowViewing() {
    flowExecMethod = flowViewed;
    showWindow('adviceWindow');
}
function flowViewed() {
    addExecuteParam("ExecComment", mini.getbyName("Advice").getValue());
    execute("ViewTask", { onComplete: function (data, settings) {
        closeWindow("refresh");
    }
    });
}

function flowReplying() {
    flowExecMethod = flowReply;
    showWindow('adviceWindow');
}

function flowReply() {
    addExecuteParam("ExecComment", mini.getbyName("Advice").getValue());
    execute("ViewTask", { actionTitle: '回复', onComplete: function (data, settings) {
        closeWindow("refresh");
    }
    });
}

/*************************************
** 导出Excel
**************************************/
function ExportExcel(key, gridId, includeColumns, detailGridId) {
    var grdId = gridId || "dataGrid";
    var grid = mini.get(grdId);
    if (!grid) {
        alert("找不到Grid，请您确认Grid的ID是否设置正确！");
        return;
    }
    var columns = grid.getBottomColumns();
    if (grid.totalCount > 10000) {
        if (!confirm("您需要导出的数据超过一万条，您确认要导出吗？")) {
            return;
        }
    }

    function getColumns(columns) {
        columns = columns.clone();
        for (var i = columns.length - 1; i >= 0; i--) {
            var column = columns[i];
            if (!column.field || column.header.trim() == '') {
                columns.removeAt(i);
            } else {
                if (includeColumns.length == 0 || includeColumns.indexOf(column.field.toLowerCase() + ',') >= 0) {
                    var c = { ChineseName: column.header.trim(), FieldName: column.field, TableName: key };

                    // 判断是否为枚举字段
                    var enumKey = gridEnums[grdId + "." + column.field]
                    if (enumKey) {
                        c.EnumKey = enumKey;
                        c.EnumDataSource = window[enumKey];
                    }

                    // 判断是否为时间字段，设置格式化字符串
                    if (column.dateFormat) {
                        c.DateFormat = column.dateFormat;
                    }

                    columns[i] = c;
                }
            }
        }
        return columns;
    }

    // 若有子Grid,追加到导出列表中
    if (typeof detailGridId != "undefined") {
        var detailGrid = mini.get(detailGridId || "detailGrid");
        var detailColumns = detailGrid.getBottomColumns();
        var cloneColumns = columns.clone();
        $.each(detailColumns, function (i, column) {
            cloneColumns.push(column);
        });
        columns = cloneColumns;
    }
    // 获取列信息
    var columns = getColumns(columns);

    var listbox = mini.get("gridColumns" + $.trim(key));
    listbox.loadData(columns);
    listbox.selectAll();

    var win = mini.get('excelWindow' + $.trim(key));
    win.show();

    return;
}

/*************************************
** 导出表单Excel
** tmplKey:Excel表单模板Key
** title:导出Excel的名字
** id:表单数据关联ID
** typeName:完整类型，包含命名空间（必须继承IExportForm）
** pathRoot: 可为空，默认当前站点名字；也可指定，例如："Market"
**************************************/
function exportFormExcel(tmplKey, title, id, typeName, pathRoot) {
    var $iframe = $("iframe[name='ifrm_ExportForm']");
    if ($iframe.length == 0) {
        $iframe = $("<iframe name='ifrm_ExportForm'></iframe>").hide();
        $iframe.appendTo("body");
    }
    var $form = $("#Form_ExportForm");
    if (typeof ($form[0]) == "undefined") {
        if (typeof (pathRoot) == "undefined")
            pathRoot = document.location.pathname.split('/')[1];
        $form = $("<form></form>").attr("id", "Form_ExportForm").attr('action', '/' + pathRoot + '/AsposeExcel/ExportForm').attr('method', 'post').attr("target", "ifrm_ExportForm").hide();
        var $hidTmplKey = $("<input type='hidden' name='tmplKey' />");
        var $hidTitle = $("<input type='hidden' name='title' />");
        var $hidID = $("<input type='hidden' name='id' />");
        var $hidTypeName = $("<input type='hidden' name='typeName' />");
        $form.append($hidID).append($hidTmplKey).append($hidTitle).append($hidTypeName);
        $form.appendTo("body");
    }
    $form.find("input[name='tmplKey']").val(tmplKey);
    $form.find("input[name='title']").val(title);
    $form.find("input[name='id']").val(id);
    $form.find("input[name='typeName']").val($.trim(typeName));
    $form.submit();
}

// 响应自定义列的弹出层的导出事件
function downloadExcelData(key, gridId, detailGridId, relateColumn) {
    var grid = mini.get(gridId || "dataGrid");
    var dataurl = changeToFullUrl(grid.getUrl());
    var columns = mini.get("gridColumns" + $.trim(key)).getSelecteds();

    // 提交下载表单（利用iframe模拟Ajax）
    var $excelForm = $("#excelForm" + $.trim(key));

    if ($excelForm.length == 0) {
        alert('请确保ID为excelForm的表单存在！');
    }

    var formData = {
        dataUrl: dataurl,
        queryFormData: grid._dataSource.loadParams.queryFormData || '',
        sortField: grid.sortField,
        sortOrder: grid.sortOrder,
        excelKey: key,
        title: document.title,
        jsonColumns: mini.encode(columns)
    };
    // 若有子Grid,追加子Grid参数
    if (typeof detailGridId != "undefined" && typeof relateColumn != "undefined") {
        var detailGrid = mini.get(detailGridId || "detailGrid");
        formData["dataUrl"] = undefined;
        formData["masterDataUrl"] = dataurl;
        formData["masterColumn"] = grid.idField;
        formData["detailDataUrl"] = changeToFullUrl(detailGrid.getUrl());
        formData["relateColumn"] = relateColumn || "relateId";
    }
    for (var p in formData) {
        $excelForm.find("input[name='" + p + "']").val(formData[p]);
    }
    $excelForm.submit();

    closeExcelWindow(key);
}

function closeExcelWindow(key) {
    var win = mini.get('excelWindow' + $.trim(key));
    win.hide();
}
/*************************************
** Excel导入
**************************************/
function ImportExcel(key, vaildUrl, saveUrl) {
    openWindow("/MvcConfig/Aspose/ImportExcel?excelKey=" + key + "&vaildURL=" + vaildUrl + "&saveURL=" + saveUrl, { title: "数据导入", width: 520, height: 200, showMaxButton: false });
}
/*---------------------------------------------------------流程方法结束--------------------------------------------------------*/


//Word导出
function ExportWord(tmplCode, id) {
    var url = "/MvcConfig/UI/Word/Export?TmplCode=" + tmplCode + "&ID=" + id;
    window.open(url);
}
