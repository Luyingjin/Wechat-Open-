
//说明：
//  0、必须熟练掌握接口
//  1、setting参数可以省略
//  2、on开头方法为事件方法
/*---------------------------------------------------------基本方法开始--------------------------------------------------------*/
//  addExecuteParam(key, value);
//  execute(action, execSettings);
//  openWindow(url, windowSettings);
//  closeWindow(data);
//  showWindow(windowId);
//  hideWindow(windowId);
//  getQueryString(key);
//  hasQueryString(key);
//  getValues(rows, attr);
/*---------------------------------------------------------基本方法结束--------------------------------------------------------*/
/*---------------------------------------------------------DataGrid开始--------------------------------------------------------*/
//  onDrawSummaryCell
//  addGridEnum(gridId, fieldName, enumKey);
//  addGridLink(gridId, fieldName, url, gridLinkSettings);
//  addGridButton(gridId, fieldName, gridButtonSettings)
/*---------------------------------------------------------DataGrid结束--------------------------------------------------------*/
/*---------------------------------------------------------选择器开始--------------------------------------------------------*/
//  addSelector(name, url, selectorSettings);
//  addEnumSelector(name, enumKey, selectorSettings);
/*---------------------------------------------------------选择器结束--------------------------------------------------------*/
/*---------------------------------------------------------其他开始--------------------------------------------------------*/
//  setFormDisabled(normalSettings);
//  setFormItemDisabled(name);
//  setFormItemEditabled(name);
//  destroyIframeMemory(iframe);
//  replaceUrl(url, windowSetting);
/*---------------------------------------------------------其他结束--------------------------------------------------------*/

/*------------------------------------------通用工具类方法 开始----------------------------------------------------------------
formatCurrency（num） 格式化金额，输入1234.56，输出￥1234.56
--------------------------------------------通用工具类方法 结束--------------------------------------------------------------*/


/*---------------------------------------------------------全局变量开始--------------------------------------------------------*/
var alertTitleStr = "众引微信系统"; //消息框标题
var allowUrlOpenForm = false;           //是否允许地址栏直接打开Form表单

// 不启用MiniUI的debugger
mini_debugger = false;

//基本参数配置
var normalParamSettings = {
    key: "",                     //配置的key,用户从配置数组中查找
    formId: "dataForm",
    gridId: "dataGrid",         //执行关联的gridId
    treeId: "dataTree",         //执行关联的TreeId
    queryFormId: "queryForm",
    queryWindowId: "queryWindow",
    queryBoxId: "key",
    queryTreeBoxId: "treeKey",
    refresh: true,                  //是否刷新  
    paramFrom: "",          //action或url从哪个控件获取参数值
    validateForm: true,     //验证表单输入
    fullRelation: false,    //停止使用
    nodeFullID: "",         //停止使用
    relationData: ""        //停止使用 
};

//操作参数配置
var operateParamSettings = {
    mustConfirm: false,                 //执行前是否需要用户确认
    mustSelectRow: false,                     //必须选择行
    mustSelectRowMsg: "当前没有选择要操作的记录，请重新确认！",         //必须选择的提示信息的提示信息
    mustSelectOneRow: false,
    mustSelectOneRowMsg: "需要选择一条操作记录，请重新确认！",
    mustSelectNode: false,                    //必须选择节点
    mustSelectNodeMsg: "当前没有选择要操作的节点，请重新确认！",        //必须选择的提示信息的提示信息
    mustSelectLeafNode: false,
    mustSelectLeafNodeMsg: "当前没有选择叶子节点，请重新确认！"
};

//execute方法参数配置，继承自基本参数配置和操作参数配置
var executeParamSettings = {
    resetFormData: true,     //根据返回值重置表单数据
    showLoading: false,
    async: true,
    action: "",
    execParams: new Object(), //执行参数
    onComplete: null,         //执行完成后调用的方法
    onFail: null,         //执行失败后调用的方法
    actionTitle: "",          //可以为保存、删除等    
    loadingMessageId: "",     //loading窗口消息Id
    addQueryString: true,
    closeWindow: false        //执行成功后是否关闭窗口
};
executeParamSettings = jQuery.extend(true, {}, normalParamSettings, operateParamSettings, executeParamSettings);

//窗口参数配置，继承自基本参数配置和操作单数配置
var windowParamSettings = {
    url: "",
    allowResize: true,
    onDestroy: null,           //销毁时调用
    onLoad: null,              //加载完成时调用页面方法参数为contentWindow
    getDataAction: "",
    title: "",                 //窗口标题
    width: '80%',                //窗口宽度
    height: '80%',               //窗口高度
    addQueryString: true,
    showMaxButton: true,
    funcType: ""               //地址栏FuncType
};
windowParamSettings = jQuery.extend(true, {}, normalParamSettings, executeParamSettings, operateParamSettings, windowParamSettings);


//Grid连接参数配置，继承自窗口参数
var gridLinkParamSettings = {
    currentRow: null,    //当前行
    onFilter: null,     //过滤条件函数
    refresh: false,     //关闭后是否刷新grid   
    linkText: "",       //连接文本，默认为当前字段值
    isButton: false,    //是否显示为按钮
    paramField: "ID",   //执行参数字段 
    url: ""             //窗口Url
};
gridLinkParamSettings = jQuery.extend(true, {}, windowParamSettings, gridLinkParamSettings);

//Grid按钮参数配置，继承自执行参数
var gridButtonParamSettings = {
    onFilter: null,           //过滤条件函数
    onButtonClick: null,      //点击按钮执行方法
    linkText: "",             //连接文本，默认为当前字段值
    isButton: false,          //是否显示为按钮
    paramField: "ID",         //执行参数字段 
    mustConfirm: true         //是否需要用户确认
};
gridButtonParamSettings = jQuery.extend(true, {}, executeParamSettings, gridButtonParamSettings);


//选择器参数配置，集成自窗口参数
var selectorParamSettings = {
    name: "",                              //选择器的名字
    title: "请选择",                       //窗口标题   
    url: "/Base/Selector/List",            //选择窗口的Url
    autoCompleteAction: "AutoComplete",    //智能感知Action，尚未实现
    enumKey: "",                           //枚举Key    
    selectMode: "single",                  //选择模式单选还是多选，多选为multi
    onSelected: null,                       //选择完成的回调方法
    returnParams: "value:ID,text:Name",    //返回值处理格式，如value:ID,text:Name
    existValidateFields: "",               //Grid记录存在验证，与returnParams格式相同
    targetType: "form",                    //返回值目标还可以是grid和row
    isAppend: false                        //是否追加模式，功能尚未实现
};
selectorParamSettings = jQuery.extend(true, {}, windowParamSettings, selectorParamSettings);


/*---------------------------------------------------------全局变量结束--------------------------------------------------------*/

/*---------------------------------------------------------基本方法开始--------------------------------------------------------*/

var executeParamKeys = new Array();
var executeParamValues = new Array();
function addExecuteParam(key, value) {
    if (typeof (value) == "object" || typeof (value) == "array" || value.constructor == Array || value.constructor == Object)
        value = mini.encode(value);
    executeParamKeys.push(key);
    executeParamValues.push(value);
}

//异步调用action
function execute(action, execSettings) {
    var url = changeToFullUrl(action); //url转化为全路径   
    return executeUrl(url, execSettings);
}

function executeUrl(url, execSettings) {

    var settings = jQuery.extend(true, {}, executeParamSettings, execSettings);

    //将url中的{}参数替换掉
    url = replaceUrl(url, settings);

    //验证输入
    if (!validateInput(settings)) {
        msgUI("当前输入的信息有误，请重新检查！", 1);
        return;
    }
    //用户操作验证，如验证必须选中一行列表数据等
    if (!validateOperation(settings))
        return;

    //验证特殊处理
    if (settings.mustConfirm)
        msgUI("确认" + settings.actionTitle + "吗？", 2, _confirmBackFunc);
    else
        return _confirmBackFunc("ok");


    function _confirmBackFunc(action) {
        if (action == "ok") {

            if (settings.showLoading && !settings.loadingMessageId) {  //遮蔽罩
                settings.loadingMessageId = mini.loading("提交中...", "提交中", { width: 300 });
            }

            //获取执行参数
            var executeParams = getExecuteParams(settings);
            if (settings.addQueryString)
                url = addUrlSearch(url, executeParams); //url增加当前地址栏参数

            //异步执行
            return jQuery.ajax({
                url: url,
                type: "post",
                data: executeParams,
                cache: false,
                async: settings.async,
                success: function (text, textStatus) {
                    window.refreshList = true; //解决暂存后，点X关闭页面后不刷列表的问题
                    //弹出消息
                    if (settings.actionTitle.length > 0 && !settings.onComplete) {
                        msgUI(settings.actionTitle + "成功！"); //, 0, _callBackFunc);
                    }

                    _callBackFunc();


                    function _callBackFunc(act) {
                        var json;

                        try {
                            json = mini.decode(text);
                        }
                        catch (e) {
                            if (settings.onComplete)
                                settings.onComplete(text, settings);
                            else if (settings.closeWindow)
                                closeWindow();
                            return;
                        }

                        //刷新表单控件
                        if (settings.resetFormData) {
                            for (var key in json) {
                                var item = mini.getbyName(key);
                                if (item != undefined) {
                                    item.setValue(json[key]);
                                    //解决combobox控件BUG
                                    if (item.type == "combobox")
                                        if (item.getText() == "")
                                            item.setText(json[key]);
                                }
                            }
                        }
                        //刷新Grid
                        if (settings.refresh && !settings.onComplete) {
                            if (settings.gridId != "") {
                                var grid = mini.get(settings.gridId);
                                if (grid != undefined && grid.url) {
                                    grid.setUrl(addUrlSearch(grid.url));
                                    grid.reload();
                                }
                            }
                        }

                        if (settings.onComplete)
                            settings.onComplete(json, settings);
                        else if (settings.closeWindow)
                            closeWindow("refresh");

                        if (settings.loadingMessageId) {
                            try {
                                mini.hideMessageBox(settings.loadingMessageId);
                            } catch (e) { }
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var msg = getErrorFromHtml(jqXHR.responseText);
                    msgUI(msg, 4, function (act) {
                        if (settings.loadingMessageId)
                            mini.hideMessageBox(settings.loadingMessageId);
                        if (settings.onFail)
                            settings.onFail(msg, settings);
                    });

                }
            });
        }
    }



}

function openWindow(url, windowSettings) {

    if (typeof (url) == "undefined") {
        msgUI('当前url不能为空，请检查！', 4);
        return;
    }

    var settings = jQuery.extend(true, {}, windowParamSettings, windowSettings);

    //用户操作验证，如验证必须选中一行列表数据等
    if (!validateOperation(settings))
        return;

    //将url中的{}参数替换掉
    url = replaceUrl(url, settings);
    //转化为全路径
    url = changeToFullUrl(url);

    if (settings.addQueryString) {
        //url增加当前地址栏参数
        url = addUrlSearch(url);
    }

    settings.url = url;
    //打开窗口
    mini.open({
        url: url,
        title: settings.title,
        allowDrag: true,
        allowResize: settings.allowResize,
        showModal: true,
        showMaxButton: settings.showMaxButton,
        width: settings.width,
        height: settings.height,
        onload: function () {
            var iframe = this.getIFrameEl();
            if (settings.onLoad) {
                settings.onLoad(iframe.contentWindow);
            }
            if (jQuery.trim(this.title)) {
                if (iframe.contentWindow && typeof (iframe.contentWindow.document) != "unknown")
                    jQuery(iframe.contentWindow.document).attr("title", this.title);
            }
            if (typeof (iframe.contentWindow.jQuery) != "undefined" && typeof (iframe.contentWindow.jQuery) != "unknown") {
                var $input = jQuery("<input>").attr("type", "hidden").attr("id", "mini_window_id").val(this.id);
                iframe.contentWindow.jQuery("body").append($input);
            }

            if (settings.data && iframe.contentWindow.setData)
                iframe.contentWindow.setData(settings.data);
            else if (settings.getDataAction != "") {
                var action = changeToFullUrl(settings.getDataAction);
                action = replaceUrl(action, settings);
                execute(action, { refresh: false, onComplete: function (data, settings) {
                    if (typeof (iframe.contentWindow.setData) != "unknown")
                        iframe.contentWindow.setData(data); //将获取的数据设置到窗口
                }
                });
            }
        },
        ondestroy: function (data) {
            if (data)
                data = mini.clone(data);
            if (settings.onDestroy)
                settings.onDestroy(data, settings);
            else if (settings.action && data != "close") {
                addExecuteParam("RelationData", mini.encode(data));
                execute(settings.action, settings);
            }
            else if (settings.refresh) {
                var _refesh = true;
                if (data && data == "close") {
                    var iframe = this.getIFrameEl();
                    _refresh = iframe.contentWindow.refreshList; //执行过excute方法，则refreshList为true
                }
                if (_refesh) {
                    var grid = mini.get("#" + settings.gridId);
                    if (grid != undefined && grid.url) {
                        grid.setUrl(addUrlSearch(grid.url));
                        grid.reload();
                    }
                }
            }
            //释放内存
            var iframe = this.getIFrameEl();
            destroyIframeMemory(iframe);
        }
    });
}

function closeWindow(data) {
    if (window.CloseOwnerWindow) return window.CloseOwnerWindow(data, true);
    else window.close();
}

function showWindow(windowId) {
    var window = mini.get(windowId);
    if (!window) { msgUI("未找到指定的window窗体！", 4); return; }
    window.queryWindowId = windowId;
    window.show();
}

function showQueryWindow(btn, windowId) {
    var win = mini.get(windowId || 'queryWindow');
    if (!win) { msgUI("未找到指定的window窗体！", 4); return; }
    win.showAtEl(btn.el, {
        xAlign: "right",
        yAlign: "below"
    });
}

function hideWindow(windowId) {
    var window = mini.get(windowId);
    if (!window) { msgUI("未找到指定的window窗体！", 4); return; }
    window.hide();
}

//获取地址栏参数,如果参数不存在则返回空字符串
function getQueryString(key, url) {
    if (typeof (url) == "undefined")
        url = window.location.search;
    var re = new RegExp("[?&]" + key + "=([^\\&]*)", "i");
    var a = re.exec(url);
    if (a == null) return "";
    return a[1];
}

//判断是否包含地址栏参数
function hasQueryString(key, url) {
    if (typeof (url) == "undefined")
        url = window.location.search;

    var re = new RegExp("[?&]" + key + "=([^\\&]*)", "i");
    var a = re.exec(url);
    if (a == null) return false;
    return true;
}


//获取数组中对象的某个值，逗号分隔
function getValues(rows, attr) {
    var fieldValues = [];
    for (var i = 0; i < rows.length; i++) {
        if (rows[i] != null)
            fieldValues.push(rows[i][attr]);
    }
    return fieldValues.join(',');
}

/*---------------------------------------------------------基本方法结束--------------------------------------------------------*/

/*---------------------------------------------------------Grid 扩展开始--------------------------------------------------------*/

function onDrawSummaryCell(e) {
    var result = e.result;

    if (result.sumData && e.field) {
        if (result.sumData[e.field] != undefined) {
            e.cellHtml = "<div style='width: 100%; text-align: right;'>总计：" + result.sumData[e.field] + "</div>";
        }
        else if (result.avgData[e.field] != undefined) {
            e.cellHtml = "<div style='width: 100%; text-align: right;'>平均：" + result.avgData[e.field] + "</div>";
        }
        else {
            e.cellHtml = "";
        }
    }
}

function onEnumRender(e) {
    return getEnumText(e.sender.id + "." + e.field, e.value);
}

//获取枚举文本
function getEnumText(gridFieldID, enumValues) {

    if (enumValues == null)
        return "";

    var en = eval(gridEnums[gridFieldID]);
    if (en == undefined) {
        return enumValues;
    }

    var vals = [];
    if (enumValues.split)
        vals = enumValues.split(',');
    else
        vals[0] = enumValues.toString();

    for (var i = 0; i < vals.length; i++) {
        if (vals[i] == "")
            continue;
        for (var j = 0; j < en.length; j++) {
            if (en[j]["value"].toString() == vals[i]) {
                vals[i] = en[j]["text"];
            }
        }
    }

    return vals.join(',');
}

function addGridEnum(gridId, fieldName, enumKey) {

    gridEnums[gridId + "." + fieldName] = enumKey;

    jQuery("#" + gridId + " div[field='" + fieldName + "']").each(function (index) {
        jQueryitem = jQuery(this);
        jQueryitem.attr("renderer", "onEnumRender");
        //jQueryitem.attr("align", "center");
    });
}
var gridEnums = new Object();

function onLinkRender(e) {
    var key = e.sender.id + "." + e.field;
    var settings = getSettings(gridLinkSettingss, key);
    if (settings == undefined)
        return e.value;


    if (settings.onFilter) {
        if (!settings.onFilter(e))
            return e.value;
    }

    var url = settings.url.replace(/\{[0-9a-zA-Z_]*\}/g, function (field) {
        var key = field.substring(1, field.length - 1);

        //从当前行返回
        if (e.record[key])
            return e.record[key];

        //从地址栏返回
        if (hasQueryString(key)) {
            return getQueryString(key);
        }

    });

    //如果添加了枚举，那么翻译枚举
    if (gridEnums[key])
        e.value = getEnumText(key, e.value);

    //追加不传值默认查看
    if (settings.title == "" && settings.funcType == "view") settings.title = "查看";

    var text = settings.linkText == "" ? (e.value == null ? "" : e.value) : settings.linkText;
    var cls = settings.isButton == true ? 'class="mini-button"' : '';

    if (settings.isButton)
        text = "&nbsp;" + text + "&nbsp;";
    //按钮图片的样式
    if (settings.buttonClass != null && settings.buttonClass != "") {
        cls = '';
        text = '<span class=' + settings.buttonClass + ' style="width:16px;height:16px;overflow:hidden;margin:auto;text-align:center; display:block;cursor:pointer;"></span>';
    }

    var s = '<a ' + cls + 'href="#" onclick="onGridLinkClick(' + e.rowIndex + ',\'' + key + '\',\'' + url + '\');">' + text + '</a>';

    return s;
}

function onGridLinkClick(index, key, url) {
    var settings = getSettings(gridLinkSettingss, key);
    if (settings == undefined)
        return;
    var grid = mini.get(settings.gridId);
    var row = grid.getRow(index);
    grid.select(row); //选中行
    settings.currentRow = row;
    settings.execParams[settings.paramField] = row[settings.paramField];

    openWindow(url, settings);
}

function addGridLink(gridId, fieldName, url, gridLinkSettings) {

    var setting = jQuery.extend(true, {}, gridLinkParamSettings, { gridId: gridId }, gridLinkSettings);
    setting["key"] = gridId + "." + fieldName;
    setting["gridId"] = gridId;
    setting["url"] = url;
    if (!setting["title"] && setting["linkText"])
        setting["title"] = setting["linkText"];
    gridLinkSettingss.push(setting);

    jQuery("#" + gridId + " div[field='" + fieldName + "']").each(function (index) {
        var jQueryitem = jQuery(this);
        jQueryitem.attr("renderer", "onLinkRender");
        if (setting["linkText"] != "")
            jQueryitem.attr("align", "center");
    });
}
var gridLinkSettingss = new Array();


//Grid按钮列开始
function onButtonRender(e) {
    var key = e.sender.id + "." + e.field;
    var settings = getSettings(gridButtonSettingss, key);
    if (settings == undefined)
        return;

    if (settings.onFilter) {
        if (!settings.onFilter(e))
            return;
    }

    //如果添加了枚举，那么翻译枚举
    if (gridEnums[key])
        e.value = getEnumText(key, e.value);

    var cls = "";
    var text = settings.linkText == "" ? getEnumText(key, e.value) : settings.linkText;
    if (settings.isButton == true) {
        cls = 'class="mini-button"';
        text = '&nbsp;' + text + '&nbsp;';
    }
    //按钮图片的样式
    if (settings.buttonClass != null && settings.buttonClass != "") {
        cls = '';
        text = '<span class=' + settings.buttonClass + ' style="width:16px;height:16px;overflow:hidden;margin:auto;text-align:center; display:block;cursor:pointer;"></span>';
    }
    var s = '<a ' + cls + 'href="javascript:void(0);" onclick="onGridButtonClick(this,' + e.row._uid + ',\'' + key + '\',\'' + e.record[settings.paramField] + '\');">' + text + '</a>';

    return s;
}

function onGridButtonClick(sender, uid, key, execParam) {

    var settings = getSettings(gridButtonSettingss, key);
    if (settings == undefined)
        return;
    var grid = mini.get(settings.gridId);
    var row = grid.getRowByUID(uid);
    if (settings.onButtonClick != null) {
        settings.onButtonClick(row, settings, sender);
        return;
    }

    var url = settings.action.replace(/\{[0-9a-zA-Z_]*\}/g, function (field) {
        var key = field.substring(1, field.length - 1);

        //从当前行返回
        if (row[key])
            return row[key];

        //从地址栏返回
        if (hasQueryString(key)) {
            return getQueryString(key);
        }

    });

    addExecuteParam(settings.paramField, execParam);
    execute(url, settings);
}

function addGridButton(gridId, fieldName, gridButtonSettings) {

    var setting = jQuery.extend(true, {}, gridButtonParamSettings, { gridId: gridId }, gridButtonSettings);
    setting["key"] = gridId + "." + fieldName;

    if (setting["actionTitle"] != "" && setting["linkText"] == "")
        setting["linkText"] = setting["actionTitle"];
    else if (setting["linkText"] != "" && setting["actionTitle"] == "")
        setting["actionTitle"] = setting["linkText"];

    gridButtonSettingss.push(setting);

    jQuery("#" + gridId + " div[field='" + fieldName + "']").each(function (index) {
        var jQueryitem = jQuery(this);
        jQueryitem.attr("renderer", "onButtonRender");
        if (setting["actionTitle"] != "")
            jQueryitem.attr("align", "center");
    });

}

var gridButtonSettingss = new Array();


/*---------------------------------------------------------Grid 扩展结束--------------------------------------------------------*/

/*---------------------------------------------------------选择器开始--------------------------------------------------------*/
//选择器控件(buttonEdit点击触发)
function onSelecting(e) {
    //获取选择器设置
    var settings = getSettings(selectorSettingss, e.sender.name);
    if (settings == undefined)
        return;

    //url追加EnumKey参数
    var url = settings.url;
    if (settings.enumKey != "") {
        url += (url.indexOf('?') < 0 ? "?" : "&") + "EnumKey=" + settings.enumKey;
    }

    if (!settings.onSelected)
        settings.onSelected = onSelected;
    settings.onDestroy = settings.onSelected;
    var winSetting = jQuery.extend(true, {}, windowParamSettings, settings, { selectorId: e.sender.id, refresh: false });

    //grid的弹出选择增加CurrentRow
    if (winSetting.targetType == "row") {
        var selector = mini.get(winSetting.selectorId);
        var grid = mini.get(winSetting.gridId);
        var row = grid.getEditorOwnerRow(selector);
        winSetting.currentRow = row;
    }

    //打开窗口
    openWindow(url, winSetting);
}

//选择器窗口关闭触发
function onSelected(data, settings) {
    if (data == undefined || typeof (data) != "object" || typeof (data) == "undefined")
        return;

    data = mini.clone(data);

    //用返回值设置表单
    if (settings.targetType == "form") {
        var selector = mini.get(settings.selectorId);

        getSettings(selectorSettingss, settings.name).data = data;  //返回值保存到配置数组

        //设置value和text
        if (selector.uiCls == "mini-buttonedit") {
            var arrReturnParam = settings.returnParams.split(',');
            for (var i = 0; i < arrReturnParam.length; i++) {
                var field = arrReturnParam[i].split(':')[0];
                if (field == "value")
                    selector.setValue(getValues(data, arrReturnParam[i].split(':')[1]));
                else if (field == "text")
                    selector.setText(getValues(data, arrReturnParam[i].split(':')[1]));
            }
        }

        //处理returnParams
        var params = settings.returnParams.split(',');
        var alreadyField = {}; //已经赋值Value的字段
        for (var i = 0; i < params.length; i++) {
            var field = params[i].split(':')[0];
            var relateField = params[i].split(':')[1];

            if (field == "value" || field == "text" || $.trim(field) == "")
                continue;

            var item = mini.getbyName(field);
            if (item == undefined) {
                msgUI(settings.key + "控件的returnParams配置错误！", 4);
                return;
            }
            if (alreadyField[field] == true) //已经SetValue，则SetText
                item.setText(getValues(data, relateField));
            else
                item.setValue(getValues(data, relateField));
            alreadyField[field] = true;
        }

        //触发控件的onUserSelectChanged
        if (typeof (settings.onUserSelectChanged) != "undefined")
            settings.onUserSelectChanged(selector);
        //触发valueChanged
        if (selector.doValueChanged) {
            selector.doValueChanged();
        }
        if (selector.validate) {
            selector.validate();
        }
    }
    //用返回值设置Grid
    else if (settings.targetType == "grid") {
        var grid = mini.get(settings.gridId);

        for (var i = data.length - 1; i >= 0; i--) {
            var hasRow = false;
            if (settings.existValidateFields == "")
                hasRow = false;
            else {
                hasRow = grid.findRows(function (row) {
                    var roles = settings.existValidateFields.split(',');
                    var eq = true;
                    for (var j = 0; j < roles.length; j++) {
                        var srcField = roles[j].split(':')[0];
                        var destField = roles[j].split(':')[1];

                        if (data[i][destField] != row[srcField]) {
                            eq = false;
                            break;
                        }
                    }
                    if (eq == true)
                        return true;
                    else
                        return false;


                }).length > 0;
            }

            if (!hasRow) {

                var newRow = {};
                var arrReturnParam = settings.returnParams.split(',');
                for (var j = 0; j < arrReturnParam.length; j++) {
                    newRow[arrReturnParam[j].split(':')[0]] = data[i][arrReturnParam[j].split(':')[1]];
                }
                grid.addRow(newRow, 0);
            }
        }
    }
    else if (settings.targetType == "row") {
        var selector = mini.get(settings.selectorId);
        var grid = mini.get(settings.gridId);
        var row = grid.getEditorOwnerRow(selector);
        var col = null;
        for (var i = 0; i < grid.columns.length; i++) {
            if (grid.columns[i].editor == selector) {
                col = grid.columns[i];
                break;
            }
        }
        var autoCancelEdit = true;
        if (col == null) {//单行触发后编辑无法找到editor
            for (var i = 0; i < grid.columns.length; i++) {
                if (grid.columns[i].field == selector.name) {
                    col = grid.columns[i];
                    autoCancelEdit = false;
                    break;
                }
            }
        }
        var newRow = {};
        var arrReturnParam = settings.returnParams.split(',');
        for (var j = 0; j < arrReturnParam.length; j++) {
            if (arrReturnParam[j] == "")
                continue;
            if (arrReturnParam[j].split(':')[0] == "value") {
                var s = getValues(data, arrReturnParam[j].split(':')[1]);
                selector.setValue(s);
                newRow[col["field"]] = s;
            }
            else if (arrReturnParam[j].split(':')[0] == "text") {
                var s = getValues(data, arrReturnParam[j].split(':')[1]);
                selector.setText(s);
                newRow[col["displayField"]] = s;
            }
            else {
                var s = getValues(data, arrReturnParam[j].split(':')[1]);
                newRow[arrReturnParam[j].split(':')[0]] = s;
            }
        }
        if (autoCancelEdit) {
            grid.updateRow(row, newRow);
            grid.cancelEdit();
        }
    }
}

function addSelector(name, url, selectorSettings) {
    jQuery("input[name='" + name + "']").each(function (index) {
        jQuery(this).attr("onbuttonclick", "onSelecting");
        //如果选择器在grid内部，则修改参数gridId和targetType
        if (index == 0) {
            var grid = $(this).closest(".mini-datagrid");
            if (grid.length > 0) {
                selectorSettings.gridId = grid[0].id;
                selectorSettings.targetType = "row";
            }
        }
    }
    );
    var settings = jQuery.extend(true, {}, selectorParamSettings, selectorSettings, { name: name, key: name, url: url, addQueryString: false });
    selectorSettingss.push(settings);
}

var selectorSettingss = new Array();


/*---------------------------------------------------------选择器结束--------------------------------------------------------*/

/*---------------------------------------------------------枚举联动开始--------------------------------------------------------*/

function addEnumLinkage(enumdata, ctrl1, ctrl2) {
    var valueChanged;
    var jct = jQuery("input[name='" + ctrl1 + "']");
    valueChanged = jct.attr("onvaluechanged");
    jct.attr("onvaluechanged", "onEnumLinkaging");
    if (typeof (enumdata) == "string")
        enumdata = eval(enumdata);
    var settings = { name: ctrl1, target: ctrl2, enumdata: enumdata, valueChanged: valueChanged };
    enumLinkageSettingss.push(settings);
}

var enumLinkageSettingss = new Array();

function onEnumLinkaging(e) {
    var settings = getSettings(enumLinkageSettingss, e.name);
    var data = Array();
    var enumdata = settings.enumdata;
    var length = enumdata.length;
    for (var i = 0; i < length; i++) {
        if (enumdata[i]['category'] == e.value)
            data.push(enumdata[i]);
    }
    mini.getbyName(settings.target).setData(data);


    if (settings.valueChanged)
        settings.valueChanged(e);
}


/*---------------------------------------------------------枚举联动结束--------------------------------------------------------*/


function msgUI(content, msgType, callbackFunc) {
    var settings = {};
    switch (msgType) {
        case 5:
            mini.prompt(content, alertTitleStr, callbackFunc);
            return;
        case 4:
            settings = { buttons: ["ok"], iconCls: "mini-messagebox-error" };
            break;
        case 3:
            settings = { buttons: ["ok", "no", "cancel"], iconCls: "mini-messagebox-question" };
            break;
        case 2:
            settings = { buttons: ["ok", "cancel"], iconCls: "mini-messagebox-question" };
            break;
        case 1:
            settings = { buttons: ["ok"], iconCls: "mini-messagebox-warning" };
            break;
        default:
            settings = { buttons: ["ok"], iconCls: "mini-messagebox-info" };
            break;
    }
    settings = $.extend({ title: alertTitleStr, message: content, callback: callbackFunc }, settings);
    mini.showMessageBox(settings);
}


/*---------------------------------------------------------其他开始--------------------------------------------------------*/
//设置Form为只读
function setFormDisabled(normalSettings) {

    var settings = jQuery.extend(true, {}, normalParamSettings, normalSettings);
    if (jQuery("#" + settings.formId).length != 1) {
        return;
    }

    var form = new mini.Form("#" + settings.formId);
    var fields = form.getFields();
    for (var i = 0, l = fields.length; i < l; i++) {
        var c = fields[i];
        if (c.type == "textarea") {
            c.addCls("asLabel");
            c.setReadOnly(true);
            c.setValue(c.getValue() + "\n");
        }
        else {
            if (c.setReadOnly) c.setReadOnly(true);
            if (c.setIsValid) c.setIsValid(true);      //去除错误提示
            if (c.addCls) c.addCls("asLabel");         //增加asLabel外观
        }
    }
    //隐藏toolbar
    $("#" + settings.formId).find(".mini-toolbar").hide();

    //Grid设置不可填
    $("#" + settings.formId).find(".mini-datagrid").each(function () {
        var grid = mini.get("#" + $(this).attr("id"));
        grid.setAllowCellEdit(false);
        grid.setAllowCellSelect(false);
    });

    //TreeGrid设置不可填
    $("#" + settings.formId).find(".mini-treegrid").each(function () {
        var grid = mini.get("#" + $(this).attr("id"));
        grid.setAllowCellEdit(false);
        grid.setAllowCellSelect(false);
    });
}

//设置表单元素为只读
function setFormItemDisabled(name) {
    var c = mini.getbyName(name);
    if (c) {
        if (c.type == "textarea") {
            c.addCls("asLabel");
            c.setReadOnly(true);
        }
        else {
            if (c.setReadOnly) c.setReadOnly(true);
            if (c.setIsValid) c.setIsValid(true);      //去除错误提示
            if (c.addCls) c.addCls("asLabel");         //增加asLabel外观
        }
    }
    else {
        $("#" + name).find("[name]").each(function (index) {
            obj = mini.getbyName($(this).attr("name"));
            if (obj.setReadOnly) obj.setReadOnly(true);
            if (obj.addCls) obj.addCls("asLabel");         //增加asLabel外观
        });
    }
}

function setFormItemEditabled(eleKey) {
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
}



//销毁驻留内存的iframe
function destroyIframeMemory(iframe) {
    if (typeof (iframe) == "undefined" || iframe == null)
        return;
    if (typeof (iframe.contentWindow.document) != "unknown") {
        iframe = iframe.contentWindow;
        iframe.src = "about:blank";
        iframe.document.write("");
        //iframe.document.clear();
        if (jQuery.browser && jQuery.browser.msie)
            CollectGarbage();
    }
}

/*---------------------------------------------------------其他结束--------------------------------------------------------*/

/*---------------------------------------------------------私有方法--------------------------------------------------------*/

//替换掉Url中的{}参数
function replaceUrl(url, windowSetting) {
    var settings = jQuery.extend(true, {}, windowParamSettings, windowSetting);

    var result = url.replace(/\{[0-9a-zA-Z_]*\}/g, function (e) {
        var key = e.substring(1, e.length - 1);

        //从当前行返回
        if (settings.currentRow && settings.currentRow[key])
            return settings.currentRow[key];

        //从地址栏返回
        if (hasQueryString(key)) {
            return getQueryString(key);
        }

        //从指定控件返回
        if (settings.paramFrom != "") {

            var ctrl = mini.get(settings.paramFrom);
            if (ctrl == undefined) {
                msgUI("ID为" + settings.paramFrom + "的控件不存在！", 4);
                return;
            }

            var selectedData;
            if (ctrl.getSelected)
                selectedData = ctrl.getSelected();
            else if (ctrl.getSelectedNode)
                selectedData = ctrl.getSelectedNode();

            if (selectedData && selectedData[key])
                return selectedData[key];
        }

        //从列表返回
        var grid = mini.get(settings.gridId);
        if (grid != undefined && settings.paramFrom == "") {
            //返回当前行字段值
            var rows = grid.getSelecteds();
            if (rows.length > 0) {
                return getValues(rows, key);
            }
        }

        //从树返回
        var tree = mini.get(settings.treeId);
        if (tree != undefined && settings.paramFrom == "") {
            var node = tree.getSelectedNode();
            if (node && node[key]) {
                return node[key];
            }
        }

        //从表单返回
        var ctrl = mini.getbyName(key);
        if (ctrl && ctrl.getValue)
            return ctrl.getValue();

        return e;

    });

    //增加FuncType
    if (settings.funcType != "") {
        if (result.indexOf('?') < 0)
            result += "?FuncType=" + settings.funcType;
        else
            result += "&FuncType=" + settings.funcType;
    }

    return result;
}

function validateInput(settings) {

    var settings = jQuery.extend(true, {}, normalParamSettings, settings);

    if (settings.validateForm == false)
        return true;

    var result = true;

    //表单输入验证
    if (jQuery("#" + settings.formId).length == 1 && settings.validateForm) {
        var form = new mini.Form("#" + settings.formId);
        form.validate();
        if (form.isValid() == false)
            result = false;
    }

    //grid输入验证
    jQuery(".mini-datagrid").each(function () {
        var grid = mini.get("#" + jQuery(this).attr("id"));
        if (grid.allowCellEdit) {
            grid.validate();
            if (grid.isValid() == false) {
                //            var error = grid.getCellErrors()[0];
                //            grid.beginEditCell(error.record, error.column);
                result = false;
            }
        }

    });

    //treegrid输入验证
    jQuery(".mini-treegrid").each(function () {
        var grid = mini.get("#" + jQuery(this).attr("id"));
        if (grid.allowCellEdit) {
            grid.validate();
            if (grid.isValid() == false) {
                //            var error = grid.getCellErrors()[0];
                //            grid.beginEditCell(error.record, error.column);
                result = false;
            }
        }

    });

    return result;
}

//用户操作的验证，比如必须选中用一行
function validateOperation(settings) {

    //参数参数验证
    if (settings.paramFrom != "") {
        var ctrl = mini.get(settings.paramFrom);
        if (ctrl == undefined) {
            msgUI("当前不存在ID为" + settings.paramFrom + "的控件！", 4);
            return false;
        }
        if (ctrl.getSelectedNode && ctrl.getSelectedNode() == null) {
            msgUI(settings.mustSelectNodeMsg, 1);
            return false;
        }
        if (ctrl.getSelected && ctrl.getSelected() == null) {
            msgUI(settings.mustSelectRowMsg, 1);
            return false;
        }

    }

    //grid操作验证
    var grid = mini.get("#" + settings.gridId);
    if (grid != undefined) {

        var rows = grid.getSelecteds();
        if (settings.mustSelectRow && rows.length == 0) {
            msgUI(settings.mustSelectRowMsg, 1);
            return false;
        }
        if (settings.mustSelectOneRow && rows.length != 1) {
            msgUI(settings.mustSelectOneRowMsg, 1);
            return false;
        }
    }

    //树操作验证
    var tree = mini.get("#" + settings.treeId);
    if (tree != undefined) {

        var node = tree.getSelectedNode();

        if (settings.mustSelectNode && node == null) {
            msgUI(settings.mustSelectNodeMsg, 1);
            return false;
        }
        if (settings.mustSelectLeafNode && (node == null || node.isLeaf != true)) {
            msgUI(settings.mustSelectLeafNodeMsg, 1);
            return false;
        }
    }
    /*
    if (settings.mustConfirm) {
    if (!confirm("确认" + settings.actionTitle + "吗？"))
    return false;
    }
    */
    return true;
}

/**
*<summary>获取execute执行参数</summary>
**/
function getExecuteParams(settings) {

    var executeParams = new Object();

    //执行参数
    while (executeParamKeys.length > 0) {
        var k = executeParamKeys.pop();
        var v = executeParamValues.pop();
        if (executeParams[k] == undefined)
            executeParams[k] = v;
    }
    //执行参数
    for (var item in settings.execParams) {
        executeParams[item] = settings.execParams[item];
    }

    //表单数据 
    var form;
    if (jQuery("#" + settings.formId).length == 1) {
        form = new mini.Form("#" + settings.formId);
    }
    if (form != undefined) {
        if (settings.validateForm == true) {
            form.validate();
            if (form.isValid() == false) return;
        }

        var _formData = form.getData();
        //大字段赋值给Grid
        var grids = $("#" + settings.formId).find("div.mini-datagrid").get();
        for (var i = 0; i < grids.length; i++) {
            var grid_Id = grids[i].id;
            if (_formData[grid_Id] == undefined) {
                mini.get(grid_Id).commitEdit()
                _formData[grid_Id] = mini.encode(mini.get(grid_Id).getData());
            }
        }
        //富文本框取值
        if (typeof (KindEditor) != "undefined") {
            var arrTxtAreas = $("textarea.KindEditor");
            if (arrTxtAreas.length == KindEditor.instances.length) {
                $.each(arrTxtAreas, function (i, obj) {
                    KindEditor.instances[i].sync();
                    _formData[$(obj).attr("name")] = KindEditor.instances[i].html();
                    var text = KindEditor.instances[i].text();
                    _formData[$(obj).attr("name") + "Text"] = text.replace(/<img [^~]*?>/ig, '');
                });
            }
        }
        executeParams["FormData"] = mini.encode(_formData);
        executeParams["OldFormData"] = mini.encode(formData);
    }

    //执行参数增加Grid数据
    var grid = mini.get("#" + settings.gridId);
    if (grid != undefined) {
        grid.commitEdit();
        if (!executeParams["ListIDs"])
            executeParams["ListIDs"] = getValues(grid.getSelecteds(), grid.idField);
        if (!executeParams["ListData"])
            executeParams["ListData"] = mini.encode(grid.getChanges());
    }

    //执行参数加入Tree数据
    var tree = mini.get("#" + settings.treeId);
    if (tree != undefined) {

        var nodes = new Array();
        if (tree.showCheckBox)
            nodes = tree.getCheckedNodes();
        else if (tree.getSelectedNode() != null)
            nodes = [tree.getSelectedNode()];

        executeParams["TreeIDs"] = getValues(nodes, tree.idField);
    }

    //附件控件的值
    var fileIDs = "";
    mini.findControls(function ($) {
        if (!$.el) return false;
        if ($.type == "singlefile" || $.type == "multifile")
            fileIDs += "," + $.getValue();
    });
    executeParams._fileIDs = fileIDs;

    return executeParams;
}

//从设置数组中获取设置
function getSettings(settingss, key) {

    var settings;
    for (var i = 0; i < settingss.length; i++) {
        if (settingss[i]["key"] == key) {
            settings = settingss[i];
            break;
        }
    }

    if (!settings) {
        //alert("获取“" + name + "”相关配置失败！");
        return;
    }
    return settings;
}

//将当前地址栏参数加入到url
function addUrlSearch(url, execParams) {
    var newParams = [];

    var paramKeys = window.location.search.replace('?', '').split('&');
    for (var i = 0; i < paramKeys.length; i++) {
        var key = paramKeys[i].split('=')[0];
        if (key == "" || key == "_t" || key == "_winid")
            continue;
        if (typeof (execParams) == "undefined") {
            if (!hasQueryString(key, url))
                newParams.push(paramKeys[i]);
        }
        else {
            if (!hasQueryString(key, url) && execParams[key] == undefined)
                newParams.push(paramKeys[i]);
        }
    }

    if (url.indexOf('?') >= 0)
        return url + "&" + newParams.join('&');
    else
        return url + "?" + newParams.join('&');
}

//url增加参数
function addSearch(url, key, value) {
    if (!hasQueryString(key, url)) {
        if (url.indexOf('?') >= 0)
            return url + "&" + key + "=" + value;
        else
            return url + "?" + key + "=" + value;
    }
    else
        return url;
}

//转化为全路径
function changeToFullUrl(url, currentUrlPathName) {
    if (url.indexOf('/') == 0 || url.indexOf("http://") == 0 || url.indexOf('?') == 0 || url == "")
        return url;


    if (typeof (currentUrlPathName) == "undefined" || currentUrlPathName == "")
        currentUrlPathName = window.location.pathname;

    var currentPathNameParts = currentUrlPathName.split('/');
    var pathNameParts = url.split('?')[0].split('/');
    if (currentPathNameParts[currentPathNameParts.length - 1] == "")
        currentPathNameParts.pop(); //去掉一个反斜线
    if (pathNameParts[pathNameParts.length - 1] == "")
        pathNameParts.pop(); //去掉一个反斜线


    var index = currentPathNameParts.length - 1;

    for (var i = 0; i < pathNameParts.length; i++) {
        if (pathNameParts[i] == "..") {
            index = index - 1;
            if (index <= 0) {
                msgUI("Url错误：" + url + "！", 4);
                return;
            }
            continue;
        }

        if (index < currentPathNameParts.length)
            currentPathNameParts[index] = pathNameParts[i];
        else
            currentPathNameParts.push(pathNameParts[i]);
        index = index + 1;
    }
    var length = currentPathNameParts.length;
    for (var i = index; i < length; i++) {
        currentPathNameParts.pop();
    }

    var result = currentPathNameParts.join('/');

    if (url.indexOf('?') > 0)
        result += url.substring(url.indexOf('?'));

    return result;
}

/*---------------------------------------------------------私有方法--------------------------------------------------------*/

function getErrorFromHtml(html) {
    var msg = html.toLowerCase();
    if (msg.indexOf("<title>") > 0) {
        msg = msg.split('</title>')[0];
        msg = msg.split('<title>')[1];
        msg = msg.replace("<br>", "\r\n");
        msg = msg.replace("<br>", "\r\n");
        msg = msg.replace("<br>", "\r\n");
        msg = msg.replace("<br>", "\r\n");
        msg = msg.replace("<br>", "\r\n");
    }
    return msg;
}


/*------------通用工具类方法----------------------------------------------------------------*/
/**
* 将数值四舍五入(保留2位小数)后格式化成金额形式
* param num 数值(Number或者String)
* return 金额格式的字符串,如'1,234,567.45'
* type String
******/
function formatCurrency(num) {
    num = num.toString().replace(/\jQuery|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
    return "￥" + (((sign) ? '' : '-') + num + '.' + cents);
}
/*---------------------------通用工具类方法 end------------------------------------------------*/

