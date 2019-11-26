/*--------------下载控件全局变量--------------*/
var SWFUPLOAD_MODE = false;

/*------------------file ---------------------*/

var SlUploadUrl = getRootPath() + "/FileStore/SlUpload/Upload.aspx";
var DownloadUrl = getRootPath() + "/FileStore/Download.aspx";

function getFile(url, fileId) {
    var result = "";
    $.ajax({
        url: url,
        type: "post",
        data: { id: fileId },
        cache: false,
        async: false,
        success: function (text) {
            result = text;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var msg = "提交服务器失败";
            msgUI(msg, 4);
        }
    });
    window.open(url + "?auth=" + result);
}

function getRootPath() {
    var pathName = window.location.pathname.substring(1);
    var webName = pathName == '' ? '' : pathName.substring(0, pathName.indexOf('/'));
    return window.location.protocol + '//' + window.location.host;
}


//将文件Id中的文件名处理掉
function fixFileIds(fileIds) {
    var result = fileIds.replace(/[^_,0-9]/g, function (n) {
        return "";
    }).replace(/_+/g, "_");

    return result;
}

function getMiniFileName(fileName) {
    var start = fileName.indexOf('_');
    var end = fileName.lastIndexOf('_');
    if (end == start)
        end = fileName.length;
    return fileName.substr(start + 1, end - start - 1);
}

function DownloadFile(fileId) {
    var url = DownloadUrl;
    var result = "";
    $.ajax({
        url: url,
        type: "post",
        data: { id: fileId },
        cache: false,
        async: false,
        success: function (text) {
            result = text;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var msg = "提交服务器失败";
            msgUI(msg, 4);
        }
    });
    window.open(url + "?auth=" + result);
}

function loadUploadifyResource(callback) {
    includeCSS("Uploadify_CSS", "/CommonWebResource/RelateResource/Uploadify/uploadify.css");
    ajaxPage("Uploadify_JS", "/CommonWebResource/RelateResource/Uploadify/jquery.uploadify.js", callback);
}

function getUploadifySettting(miniFile) {
    var setting = {};
    var fileTypeDesc = [];
    var fileTypeExts = [];
    $.each(miniFile.filter.split('|'), function (i, item) {
        if ($.trim(item) != "") {
            if (i % 2 == 0) {
                fileTypeDesc.push(item);
            }
            else {
                fileTypeExts.push(item);
            }
        }
    });
    if (fileTypeDesc.length > 0 && fileTypeExts.length > 0) {
        setting["fileTypeDesc"] = fileTypeDesc.join(',');
        setting["fileTypeExts"] = fileTypeExts.join(';');
    }
    if ($.trim(miniFile.maximumupload) != "") {
        setting["fileSizeLimit"] = miniFile.maximumupload;
    }
    if ($.trim(miniFile.maxnumbertoupload) != "") {
        setting["uploadlimit"] = parseInt(miniFile.maxnumbertoupload);
    }
    return setting;
}

//--------------------------------mini-MultiFile对象定义开始 从mini-Panel继承----------------------------------------
mini.ux.MultiFile = function () {
    mini.ux.MultiFile.superclass.constructor.call(this);

    this.files = [];
    this.initControls();
    this.initEvents();
}
mini.extend(mini.ux.MultiFile, mini.Splitter, {
    formField: true,
    width: "100%",
    height: 120,
    allowResize: false,
    handlerSize: 0,
    required: false, //是否必填
    readonly: false, //是否只读
    downloadDisabled: false, //是否下载
    disabled: false, //是否禁用
    perrowcount: 2, //每行文件数
    maximumupload: "", //上传的最大文件大小
    maxnumbertoupload: "", //上传的最大文件数量
    filter: "", //上传的文件类型
    allowthumbnail: false, //是否缩略图
    src: "system", //所属系统模块
    uiCls: "mini-multifile",
    uploadifyID: "",

    initControls: function () {
        this.setPanes([{ showCollapseButton: false, style: "border-right:0px;overflow-y:auto;overflow-x:none" }, { size: 40, showCollapseButton: false}]);
        var paneBtn = this.getPaneEl(2);
        var uploadHTML = "<a name='btnAdd' class='mini-button' plain=\"true\" iconCls=\"icon-extend-upload\" tooltip=\"上传\"></a>";
        if (typeof SWFUPLOAD_MODE != "undefined" && SWFUPLOAD_MODE) {
            this.uploadifyID = "multifile_uploadify_" + this.id;
            uploadHTML = "<span class=\"uploadify-multi-button\"></span>"
        }
        var btnHtml = "<table width=\"100%\" height=\"100%\" style=\"background-color:#F0F0F0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"text-align:center;vertical-align:top\">"
            + "<table cellpadding=\"0\" cellspacing=\"0\" align=\"center\">"
            + "<tr><td>" + uploadHTML + "</td></tr>"
            + "<tr><td><a name='btnDel' class='mini-button' plain=\"true\" iconCls=\"icon-extend-filedelete\" tooltip=\"删除\"></a></td></tr>"
            + "<tr><td><a name='btnClear' class='mini-button' plain=\"true\" iconCls=\"icon-extend-trash\" tooltip=\"清空\"></a></td></tr>"
            + "<tr><td><a name='btnDownload' class='mini-button' plain=\"true\" iconCls=\"icon-extend-download\" tooltip=\"下载\"></a></td></tr>"
            + "</table></td></tr></table>";
        paneBtn.innerHTML = btnHtml;
        mini.parse(this.el);
        var attrs = mini.ux.MultiFile.superclass.getAttrs.call(this, this.el);
        this.FilePaneEl = this.getPaneEl(1);
        if (this.uploadifyID == "") {
            this._btnAdd = mini.getbyName("btnAdd", this);
        }
        this._btnDel = mini.getbyName("btnDel", this);
        this._btnDownload = mini.getbyName("btnDownload", this);
        this._btnClear = mini.getbyName("btnClear", this);
    },
    initEvents: function () {
        if (this.uploadifyID == "") {
            this._btnAdd.on("click", function (e) {
                //弹上传页面，并取返回值
                this.uploadFile();
            }, this);
        }
        this._btnDel.on("click", function (e) {
            var delFileChecked = $(this.FilePaneEl).find("input:checked");
            if (delFileChecked.length == 0) {
                msgUI("当前没有勾选要删除的文件，请重新检查！", 1);
                return;
            }
            var ctrl = this;
            msgUI("确认要删除文件吗？", 2, function (action) {
                if (action == "ok") {
                    //获取内容区选中checkbox，并移除
                    var delFiles = [];
                    $.each(delFileChecked, function (i, val) {
                        delFiles.push($(val).val());
                    });
                    delFilesCallback(ctrl.id);
                }
            });
        }, this);
        this._btnDownload.on("click", this.downloadFiles, this);
        this._btnClear.on("click", function (e) {
            //清除内容区的内容
            var bodyEl = this.FilePaneEl;
            var fileIds = this.getValue();
            if (fileIds == "") return;
            var ctrlId = this.id;
            msgUI("确认要删除所有文件吗？", 2,
                function (action) {
                    if (action == "ok") {
                        clearFilesCallback(ctrlId);
                    }
                }
            );
        }, this);
    },
    setValue: function (files) {
        this.files = files && files != "" ? files.split(',') : [];
        this.renderFiles();
    },
    addValue: function (files) {
        if (files && files != "") {
            var oldFiles = this.files.length > 0 ? this.files.join(",") + "," : "";
            this.files = (oldFiles + files).split(",");
            this.renderFiles();
        }
    },
    removeValue: function () {
        var delFileChecked = $(this.FilePaneEl).find("input:checked");
        if (delFileChecked.length > 0) {
            var arrdelfiles = [];
            $.each(delFileChecked, function (i, val) {
                arrdelfiles.push($(val).val());
            });
            if (arrdelfiles.length > 0) {
                var newfiles = [];
                $.each(this.files, function (i, val) {
                    if ($.inArray(val, arrdelfiles) == -1) {
                        newfiles.push(val);
                    }
                });
                this.files = newfiles;
                this.renderFiles();
            }
        }
    },
    renderFiles: function () {
        var bodyHtml = "";
        var ctrlId = this.id;
        $.each(this.files, function (i, file) {
            if ($.trim(file) != "") {
                var filename = getMiniFileName(file);
                var fileId = file.substring(0, file.indexOf("_"));
                bodyHtml += "<span><table width=100%><tbody><tr>"
                    + "<td align='right' width='20'><input type='checkbox' id='_FileId' value='" + file + "'></td>"
                    + "<td width='5'></td>"
                    + "<td align='left'><span class='MultiMode_FileName' id='_FileName'><a fileid='" + fileId + "' file='" + file + "' style='text-decoration: underline;cursor:hand;color: #1122CC;'>" + filename + "</a></span></td>"
                    + "</tr></tbody></table></span>";
            }
        });
        this.FilePaneEl.innerHTML = bodyHtml;
        if (!this.downloadDisabled && !this.disabled) {
            $(this.FilePaneEl).find("a").each(function (i) {
                $(this).bind("click", function () { DoViewFile($(this).attr("fileid"), $(this).attr("file")); });
            });
        }
    },
    getValue: function () {
        var value = this.files.join(",");
        return value;
    },
    getText: function () {
        var arrFiles = this.getValue().split(',');
        var txt = "";
        $.each(arrFiles, function (i, file) {
            if ($.trim(file) != "") {
                var filename = getMiniFileName(file);
                txt += filename;
                if (i < arrFiles.length - 1)
                    txt += ",";
            }
        });
        return txt;
    },
    setReadOnly: function (isReadonly) {
        if (isReadonly) {
            this.readonly = true;
            this.addCls("mini-multifile-readonly");
            if (this.uploadifyID != "")
                $("#" + this.uploadifyID).parent().hide();
            else
                this._btnAdd.hide();
            this._btnDel.hide();
            this._btnClear.hide();
        }
        else {
            this.readonly = false;
            this.removeCls("mini-multifile-readonly");
            if (this.uploadifyID != "")
                $("#" + this.uploadifyID).parent().show();
            else
                this._btnAdd.show();
            this._btnDel.show();
            this._btnClear.show();
        }
    },
    setDisabled: function (isDisabled) {
        if (isDisabled) {
            this.disabled = true;
            this.addCls("mini-multifile-disabled");
            this.disable();
            $(this.FilePaneEl).find("input[type='checkbox']").prop("disabled", true);
            $(this.FilePaneEl).find("a").each(function (i) {
                $(this).unbind("click");
            });
            $(this.FilePaneEl).find("#_FileName").prop("disabled", true);
            if (this.uploadifyID != "")
                $("#" + this.uploadifyID).uploadify('disable', true);
            else
                this._btnAdd.disable();
            this._btnDel.disable();
            this._btnClear.disable();
            this._btnDownload.disable();
        }
        else {
            this.disabled = false;
            this.removeCls("mini-multifile-disabled");
            this.enable();
            $(this.FilePaneEl).find("input[type='checkbox']").prop("disabled", false);
            $(this.FilePaneEl).find("a").each(function (i) {
                $(this).bind("click", function () { DoViewFile($(this).attr("fileid"), $(this).attr("file")); });
            });
            $(this.FilePaneEl).find("#_FileName").prop("disabled", false);
            if (this.uploadifyID != "")
                $("#" + this.uploadifyID).uploadify('disable', false);
            else
                this._btnAdd.enable();
            this._btnDel.enable();
            this._btnClear.enable();
            this._btnDownload.enable();
        }
    },
    setDownloadDisabled: function (isDisabled) { //使用该方法，请先调用setReadOnly
        if (isDisabled) {
            this.downloadDisabled = true;
            $(this.FilePaneEl).find("a").each(function (i) {
                $(this).unbind("click");
            });
            this._btnDownload.disable();
        }
        else {
            this.downloadDisabled = false;
            $(this.FilePaneEl).find("a").each(function (i) {
                $(this).bind("click", function () { DoViewFile($(this).attr("fileid"), $(this).attr("file")); });
            });
            this._btnDownload.enable();
        }
    },
    setRequired: function (isRequired) {
        if (isRequired && isRequired != "false") {
            this.required = true;
            this.addCls("mini-required");
        }
        else {
            this.required = false;
            this.removeCls("mini-required");
        }
    },
    validate: function () {
        var isVal = this.isValid();
        var errorId = this.id + "-errorIcson";
        if (!isVal) {
            if ($("#" + errorId).length == 0) {
                //添加error图标
                var imgError = $("<img id='" + errorId + "' src='/CommonWebResource/Theme/Default/MiniUI/images/panel/error.gif' width='14' height='14' title='不能为空' />");
                $("#" + this.id).after(imgError);
            }
            return false;
        }
        else {
            $("#" + errorId).remove();
            return true;
        }
    },
    isValid: function () {
        if (this.required) {
            if (this.getValue() != "") {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    },
    uploadFile: function () {
        var value = "FileIds";
        var multiFile = this;
        var relateId = mini.getbyName("ID");
        var url = SlUploadUrl + "?value=" + value + "&" + "FileMode=multi&IsLog=undefined&Filter=" + escape(multiFile.filter) + "&MaximumUpload=" + multiFile.maximumupload + "&MaxNumberToUpload=" + multiFile.maxnumbertoupload + "&AllowThumbnail=" + multiFile.allowthumbnail + "&RelateId=&Src=" + multiFile.src;
        if (relateId)
            url += "&RelateId=" + relateId.getValue();
        mini.open({
            url: url,
            width: 500,
            height: 300,
            title: "多附件上传",
            ondestroy: function (rtnValue) {
                if (rtnValue.substring(0, 3) == "err") {
                    msgUI(rtnValue, 4);
                    return;
                }
                else if (rtnValue == "close") {
                }
                else {
                    multiFile.addValue(rtnValue);
                }
            }
        });
    },
    downloadFiles: function () {
        if (this.files.length > 0) {
            DownloadFile(escape(this.files.join(",")));
        }
    },
    getAttrs: function (el) {
        var attrs = mini.ux.MultiFile.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "required", "readonly", "disabled", "perrowcount", "maximumupload", "maxnumbertoupload", "filter", "allowthumbnail", "src"]
        );
        return attrs;
    }
});

function DoViewFile(fileId, file) {
    if (!file)
        return;
    getFile(DownloadUrl, fileId);
}

function delFilesCallback(ctrlId) {
    if (ctrlId == "")
        return;
    var miniCtrl = mini.get(ctrlId);
    miniCtrl.removeValue();
}

function clearFilesCallback(ctrlId) {
    if (ctrlId == "")
        return;
    var minictrl = mini.get(ctrlId);
    minictrl.setValue("");
}

mini.regClass(mini.ux.MultiFile, "multifile");

//--------------------------------mini-SingleFile对象定义开始 从mini-Control继承----------------------------------------
mini.ux.SingleFile = function () {
    mini.ux.SingleFile.superclass.constructor.call(this);
    this.file = "";
    this.initControls();
    this.initEvents();
}

mini.extend(mini.ux.SingleFile, mini.Control, {
    formField: true,
    title: "上传",
    width: 200,
    required: false, //是否必填
    readonly: false, //是否只读
    disabled: false, //是否禁用
    maximumupload: "", //上传的最大文件大小
    maxnumbertoupload: "1", //上传的最大文件数量
    filter: "", //上传的文件类型
    allowthumbnail: false, //是否缩略图
    src: "system", //所属系统模块
    uiCls: "mini-singlefile",
    uploadifyID: "",
    initControls: function () {
        mini.parse(this.el);
        var attrs = mini.ux.SingleFile.superclass.getAttrs.call(this, this.el);
        var $border = $("<span>").addClass("mini-singlefile-border");
        $border.append($("<input>").attr("type", "text").addClass("mini-singlefile-input").attr("readonly", "readonly"));
        var $icon = $("<span>").addClass("mini-singlefile-buttons");
        $icon.append($("<span>").addClass("mini-singlefile-remove").attr("title", "删除"));
        if (typeof SWFUPLOAD_MODE != "undefined" && SWFUPLOAD_MODE) {
            this.uploadifyID = "singlefile_uploadify_" + this.id;
            $icon.append(($("<span>").addClass("uploadify-single-button")).append($("<input>").attr("id", this.uploadifyID).attr("type", "file")));
        }
        else {
            $icon.append(($("<span>").addClass("mini-singlefile-button")).append($("<span>").addClass("mini-singlefile-upload").attr("title", "上传")));
        }
        $border.append($icon);
        $(this.el).append($border);
        this._fileBorder = $border;
        this._txtDownLoad = $(this.el).find("input.mini-singlefile-input");
        this._btnRemove = $(this.el).find("span.mini-singlefile-remove");
        if (this.uploadifyID == "")
            this._btnUpLoad = $(this.el).find("span.mini-singlefile-upload");
        else
            this._btnUpLoad = $(this.el).find(".uploadify-single-button");
    },
    initEvents: function () {
        var ctrl = this;
        this._txtDownLoad.click(function (event) {
            ctrl.downloadFile();
        });
        this._btnRemove.click(function (event) {
            ctrl.removeValue();
        });
        this._btnUpLoad.click(function (event) {
            ctrl.uploadFile();
        });
        if (this.uploadifyID == "") {
            this._btnUpLoad.parent().mouseover(function (event) {
                if (ctrl.enabled)
                    ctrl._btnUpLoad.parent().addClass("mini-singlefile-button-hover");
            });
            this._btnUpLoad.parent().mouseout(function (event) {
                ctrl._btnUpLoad.parent().removeClass("mini-singlefile-button-hover");
            });
        }
        mini.on(this._txtDownLoad[0], "change", this.__OnChange, this);
    },
    render: function () {
        var filename = getMiniFileName(this.file);
        this._txtDownLoad.attr("value", filename).change();
        this._txtDownLoad.attr("title", filename);
    },
    setValue: function (file) {
        this.file = $.trim(file);
        this.render();
    },
    getValue: function () {
        return $.trim(this.file);
    },
    getText: function () {
        var filename = getMiniFileName(this.getValue());
        return filename;
    },
    setReadOnly: function (isReadonly) {
        if (isReadonly) {
            this.readonly = true;
            this.addCls("mini-singlefile-readonly");
            this._btnRemove.hide();
            if (this.uploadifyID == "")
                this._btnUpLoad.parent().hide();
            else
                this._btnUpLoad.hide();
        }
        else {
            this.readonly = false;
            this.removeCls("mini-singlefile-readonly");
            this._btnRemove.show();
            if (this.uploadifyID == "")
                this._btnUpLoad.parent().show();
            else
                this._btnUpLoad.show();
        }
    },
    setDisabled: function (isDisabled) {
        if (isDisabled) {
            this.disabled = true;
            this.addCls("mini-singlefile-disabled");
            this._txtDownLoad.attr("disabled", true);
            this._btnRemove.unbind("click");
            if (this.uploadifyID == "") {
                this._btnUpLoad.unbind("click");
                this._btnUpLoad.parent().unbind();
            }
        }
        else {
            this.disabled = false;
            this.removeCls("mini-singlefile-disabled");
            this._txtDownLoad.removeAttr("disabled");
            var ctrl = this;
            this._btnRemove.click(function (event) {
                ctrl.removeValue();
            });
            if (this.uploadifyID == "") {
                this._btnUpLoad.click(function (event) {
                    ctrl.uploadFile();
                });
                this._btnUpLoad.parent().mouseover(function (event) {
                    if (ctrl.enabled) {
                        ctrl._btnUpLoad.parent().addClass("mini-singlefile-button-hover");
                    }
                });
                this._btnUpLoad.parent().mouseout(function (event) {
                    ctrl._btnUpLoad.parent().removeClass("mini-singlefile-button-hover");
                });
            }
        }
        if (this.uploadifyID != "")
            $("#" + this.uploadifyID).uploadify('disable', this.disabled);
    },
    setDownloadDisabled: function (isDisabled) { //使用该方法，请先调用setReadOnly
        if (isDisabled) {
            this._txtDownLoad.unbind("click");
        }
        else {
            var ctrl = this;
            this._txtDownLoad.click(function (event) {
                ctrl.downloadFile();
            });
        }
    },
    setRequired: function (isRequired) {
        if (isRequired && isRequired != "false") {
            this.required = true;
            this.addCls("mini-required");
        }
        else {
            this.required = false;
            this.removeCls("mini-required");
        }
    },
    validate: function () {
        var isVal = this.isValid();
        var errorId = this.id + "-errorIcon";
        if (!isVal) {
            if ($("#" + errorId).length == 0) {
                //添加error图标
                var imgError = $("<img id='" + errorId + "' src='/CommonWebResource/Theme/Default/MiniUI/images/panel/error.gif' width='14' height='14' title='不能为空' />");
                $("#" + this.id).after(imgError);
            }
            return false;
        }
        else {
            $("#" + errorId).remove();
            return true;
        }
    },
    isValid: function () {
        if (this.required) {
            if (this.getValue() != "") {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    },
    addValue: function (value) {
        var oldValue = this.file;
        if ($.trim(oldValue) != "") {
            this.file = $.trim(value);
            callbackSetValue(this.id);
        }
        else {
            this.setValue(value);
        }
    },
    removeValue: function () {
        if ($.trim(this.file) != "") {
            var ctrl = this;
            var file = ctrl.file;
            msgUI("确认要删除文件吗？", 2,
                function (action) {
                    if (action == "ok") {
                        ctrl.file = "";
                        callbackSetValue(ctrl.id);
                    }
                }
            );
        }
    },
    uploadFile: function () {
        var value = "FileIds";
        var fileCtrl = this;
        var relateId = mini.getbyName("ID");
        var url = SlUploadUrl + "?value=" + value + "&" + "FileMode=multi&IsLog=undefined&Filter=" + escape(fileCtrl.filter) + "&MaximumUpload=" + fileCtrl.maximumupload + "&MaxNumberToUpload=" + fileCtrl.maxnumbertoupload + "&AllowThumbnail=" + fileCtrl.allowthumbnail + "&RelateId=&Src=" + fileCtrl.src;
        if (relateId)
            url += "&RelateId=" + relateId.getValue();
        mini.open({
            url: url,
            width: 500,
            height: 300,
            title: "单附件上传",
            ondestroy: function (rtnValue) {
                if (rtnValue.substring(0, 3) == "err") {
                    msgUI(rtnValue, 4);
                    return;
                }
                else if (rtnValue == "close") {
                }
                else {
                    fileCtrl.addValue(rtnValue);
                }
            }
        });
    },
    downloadFile: function () {
        if ($.trim(this.file) != "") {
            DownloadFile(escape(this.file));
        }
    },
    getAttrs: function (el) {
        var attrs = mini.ux.SingleFile.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "required", "readonly", "disabled", "maximumupload", "filter", "allowthumbnail", "src", "onchange"]
        );
        return attrs;
    },
    __OnChange: function (e) {
        var t = mini.findParent(e.target, "mini-singlefile");
        if (t) {
            var val = this.getValue();
            var ev = {
                sender: this,
                value: val,
                text: this.getText(),
                downloadValue: DownloadUrl + "?FileId=" + val
            };
            this.fire("change", ev);
        }
    }
});

function callbackSetValue(ctrlId) {
    if (ctrlId == "")
        return;
    var minictrl = mini.get(ctrlId);
    var val = minictrl.file;
    minictrl.setValue(val);
}

mini.regClass(mini.ux.SingleFile, "singlefile");

//--------------------------------File-Uploadify--------------------------------------------------
$(function () {
    if (typeof SWFUPLOAD_MODE != "undefined" && SWFUPLOAD_MODE) {
        loadUploadifyResource(initUploadify);

        function initUploadify() {
            var queueID = "fileQueue_mini";
            $("body").append($("<div>").attr("id", queueID).addClass("uploadify-fileQueue"));
            $.each($(".mini-multifile"), function (i, dom) {
                var miniFile = mini.getbyUID(dom.uid);
                var fileID = miniFile.uploadifyID;
                $(miniFile.getEl()).find("table table tr td:eq(0) span.uploadify-multi-button").append($("<input>").attr("type", "file").attr("id", fileID));
                var uid = miniFile.uid;
                $("#" + fileID).uploadify($.extend({
                    swf: '/commonwebresource/RelateResource/Uploadify/uploadify.swf',
                    uploader: '/FileStore/SWFUpload/FileUploadHandler.ashx',
                    queueID: queueID,
                    buttonClass: "mini-multifile-upload",
                    buttonText: "",
                    width: 20,
                    height: 20,
                    onUploadSuccess: function (file, data, response) {
                        mini.getbyUID(uid).addValue(data);
                    },
                    onSWFReady: function () {
                        if (miniFile.readonly) {
                            $("#" + fileID).hide();
                        }
                        if (miniFile.disabled) {
                            $("#" + fileID).uploadify('disable', true);
                        }
                    }
                }, getUploadifySettting(miniFile)));
            });

            $.each($(".mini-singlefile"), function (i, dom) {
                var miniFile = mini.getbyUID(dom.uid);
                var fileID = miniFile.uploadifyID;
                var uid = miniFile.uid;
                $("#" + miniFile.uploadifyID).uploadify($.extend({
                    swf: '/commonwebresource/RelateResource/Uploadify/uploadify.swf',
                    uploader: "/FileStore/SWFUpload/FileUploadHandler.ashx",
                    multi: false,
                    queueID: queueID,
                    buttonClass: "mini-singlefile-upload",
                    buttonText: "",
                    width: 15,
                    height: 15,
                    onUploadSuccess: function (file, data, response) {
                        mini.getbyUID(uid).setValue(data);
                    },
                    onSWFReady: function () {
                        if (miniFile.disabled) {
                            $("#" + fileID).uploadify('disable', true);
                        }
                    }
                }, getUploadifySettting(miniFile)));
            });
        }
    }
});

//--------------------------------mini-AuditSign会签控件------------------------------------------

mini.ux.AuditSign = function () {
    mini.ux.AuditSign.superclass.constructor.call(this);
    this.initScript();
}

mini.extend(mini.ux.AuditSign, mini.Control, {
    formField: true,
    uiCls: "mini-auditsign",
    defaultTmpl: "/MvcConfig/auditSignTmpl.js", //审批签名模板
    tmplurl: "", //自定义模板
    tmplItem: "AuditItem",
    tmplName: "auditSignTmpl",
    signTitle: "",
    initScript: function () {
        ajaxPage(this.tmplName, this.defaultTmpl);
    },
    render: function () {
        var renderHTML = "";
        var itemName = this.tmplItem;
        var tmplName = this.tmplName;
        var signTitle = this.signTitle;
        var data = this.dataField;
        if ($.trim(this.tmplurl) == "" && this.dataField.length == 0) {
            data = [{ SignComment: "", ExecUserID: "", SignTime: ""}];
        }
        $.each(data, function (i, auditSign) {
            var tmpItem = eval(tmplName + "_AuditSignTempleteItem");
            tmpItem = tmpItem.replace("$SignTitle", signTitle);
            for (var prop in auditSign) {
                if (typeof (auditSign[prop]) == "string") {
                    var propVal = auditSign[prop];
                    if (prop == "SignTime" && $.trim(propVal) != "") {
                        propVal = $.trim(propVal);
                        propVal = propVal.replace("/", "-");
                        propVal = propVal.split(' ')[0];
                        //propVal = propVal.replace("-", "/");
                        //propVal = new Date(propVal).format("yyyy-MM-dd");

                    }
                    tmpItem = tmpItem.replace("$" + prop, propVal);
                }
            }
            renderHTML += tmpItem;
        });
        this.el.innerHTML = eval(tmplName + "_AuditSignTemplete").replace("$" + itemName, renderHTML);
    },
    setTmplurl: function (url) {
        this.tmplName = getFileNameNoExt(url);
        ajaxPage(this.tmplName, url);
    },
    setValue: function (val) {
        if (typeof (val) == "string") {
            val = eval($.trim(val) == "" ? [] : val);
        }
        else if (typeof (val) == "undefined" || val == null)
            val = [];
        this.dataField = val;
        this.render();
    },
    getValue: function () {
        if (typeof (this.dataField) == "undefined" || this.dataField == null)
            this.dataField = [];
        return mini.encode(this.dataField);
    },
    getFormValue: function () {
        if (typeof (this.dataField) == "undefined" || this.dataField == null)
            this.dataField = [];
        return mini.encode(this.dataField);
    },
    getAttrs: function (el) {
        var attrs = mini.ux.AuditSign.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "tmplurl", "tmplName", "signTitle"]
        );
        return attrs;
    }
});

mini.regClass(mini.ux.AuditSign, "auditsign");

//--------------------------------辅助方法------------------------------------------
function getFileNameNoExt(url) {
    var fileName = getFileName(url);
    return fileName.replace(getFileExt(fileName));
}

function getFileName(url) {
    var pos = url.lastIndexOf("/");
    if (pos == -1) {
        pos = url.lastIndexOf("\\")
    }
    var filename = url.substr(pos + 1)
    var ext = getFileExt(filename);
    return filename.replace(ext, "");
}

//取文件后缀名
function getFileExt(filepath) {
    if (filepath != "") {
        var pos = "." + filepath.replace(/.+\./, "");
        return pos;
    }
}

function ajaxPage(sId, url, callBack) {
    var oXmlHttp = getHttpRequest();
    oXmlHttp.onreadystatechange = function () {
        //4代表数据发送完毕
        if (oXmlHttp.readyState == 4) {
            //0为访问的本地，200代表访问服务器成功，304代表没做修改访问的是缓存
            if (oXmlHttp.status == 200 || oXmlHttp.status == 0 || oXmlHttp.status == 304) {
                includeJS(sId, oXmlHttp.responseText);
                if (typeof callBack == "function") {
                    callBack();
                }

            }
            else {
            }
        }
    }
    oXmlHttp.open("GET", url, false);
    oXmlHttp.send(null);
}

function includeJS(sId, source) {
    if ((source != null) && (!document.getElementById(sId))) {
        var myHead = document.getElementsByTagName("HEAD").item(0);
        var myScript = document.createElement("script");
        myScript.language = "javascript";
        myScript.type = "text/javascript";
        myScript.id = sId;
        try {
            myScript.appendChild(document.createTextNode(source));
        }
        catch (ex) {
            myScript.text = source;
        }
        myHead.appendChild(myScript);
    }
}

function includeCSS(sId, source) {
    if ((source != null) && (!document.getElementById(sId))) {
        var myHead = document.getElementsByTagName("HEAD").item(0);
        var node = document.createElement('link');
        node.rel = "stylesheet";
        node.type = 'text/css';
        node.id = sId;
        node.href = source;
        try {
            node.appendChild(document.createTextNode(source));
        }
        catch (ex) {
            node.text = source;
        }
        myHead.appendChild(node);
    }
}

function getHttpRequest() {
    if (window.ActiveXObject)//IE
    {
        return new ActiveXObject("MsXml2.XmlHttp");
    }
    else if (window.XMLHttpRequest)//其他
    {
        return new XMLHttpRequest();
    }
}

Date.prototype.format = function (format) //author: meizz
{
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
    (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
      RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

/*----------signpic控件------------------*/
mini.ux.SignPic = function () {
    mini.ux.SignPic.superclass.constructor.call(this);
    this.initControls();
}

mini.extend(mini.ux.SignPic, mini.Control, {
    uiCls: "mini-signpic",
    width: 80,
    height: 30,
    userId: "",
    src: "/MvcConfig/Image/GetSignPic?UserId=",
    noneImageSrc: "/CommonWebResource/RelateResource/image/signname.jpg",
    initControls: function () {
        var $img = $("<img>");
        $(this.el).append($img);
        this._Image = $img;
        this._Image.attr("src", this.noneImageSrc).height(this.height).width(this.width);
    },
    setUserid: function (userId) {
        this.userId = userId;
        if ($.trim(userId) != "")
            this._Image.attr("src", this.src + userId);
        else
            this._Image.attr("src", this.noneImageSrc);
    },
    setValue: function (value) {
        this.setUserid(value);
    },
    getValue: function () {
        return this.userId;
    },
    getAttrs: function (el) {
        var attrs = mini.ux.SignPic.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "userid", "width", "height"]
        );
        return attrs;
    }
});

mini.regClass(mini.ux.SignPic, "signpic");

mini.ux.PortraitPic = function () {
    mini.ux.PortraitPic.superclass.constructor.call(this);
    this.initControls();
}

mini.extend(mini.ux.PortraitPic, mini.Control, {
    uiCls: "mini-portraitpic",
    width: 90,
    height: 120,
    userId: "",
    src: "/MvcConfig/Image/GetUserPic?UserId=",
    noneImageSrc: "/CommonWebResource/RelateResource/image/photo.jpg",
    initControls: function () {
        var $img = $("<img>");
        $(this.el).append($img);
        this._Image = $img;
        this._Image.attr("src", this.noneImageSrc).height(this.height).width(this.width);
    },
    setUserid: function (userId) {
        this.userId = userId;
        if ($.trim(userId) != "")
            this._Image.attr("src", this.src + userId);
        else
            this._Image.attr("src", this.noneImageSrc);
    },
    setValue: function (value) {
        this.setUserid(value);
    },
    getValue: function () {
        return this.userId;
    },
    getAttrs: function (el) {
        var attrs = mini.ux.PortraitPic.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "userid", "width", "height"]
        );
        return attrs;
    }
});

mini.regClass(mini.ux.PortraitPic, "portraitpic");

/*----------pic控件------------------*/
mini.ux.Pic = function () {
    mini.ux.Pic.superclass.constructor.call(this);
    this.initControls();
}

mini.extend(mini.ux.Pic, mini.Control, {
    uiCls: "mini-pic",
    imageWidth: "",
    imageHeight: "",
    src: "",
    noneImageSrc: "",
    initControls: function () {
        var $img = $("<img>");
        $(this.el).append($img);
        this._Image = $img;
        this._Image.attr("src", this.noneImageSrc);

    },
    setNoneImageSrc: function (noneImageSrc) {
        this._noneImageSrc = noneImageSrc;
        if (this._src == undefined || this._src == "" || this._src == null)
            this._Image.attr("src", noneImageSrc);
    },
    setSrc: function (src) {
        this._src = src;
        if (src != undefined && src != "" && src != null)
            this._Image.attr("src", src);
        else
            this._Image.attr("src", this._noneImageSrc);
    },
    getSrc: function () {
        return this._src;
    },
    setImageWidth: function (value) {
        this._imageWidth = value;
        this._Image.width(value);
    },
    setImageHeight: function (value) {
        this._imageHeight = value;
        this._Image.height(value);
    },
    getAttrs: function (el) {
        var attrs = mini.ux.Pic.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
               "noneImageSrc", "src", "imageWidth", "imageHeight"]
        );
        return attrs;
    }
});

mini.regClass(mini.ux.Pic, "pic");

/*-----------userselect ----------------------------*/
var USERSELECT_GRIDURL_TMPL = "/MvcConfig/Auth/User/SelectUsers?OrgIDs={OrgIDs}&RoleIDs={RoleIDs}";
function addAutoUserSelect(name, selectorParamSettings) {
    var $btn = jQuery("input[name='" + name + "']");
    if ($btn) {
        $btn.attr("id", name);
        $btn.each(function (index) { $(this).attr("onfocus", "onUserSelectFocus"); });
        $btn.each(function (index) { $(this).attr("onkeydown", "onUserSelectKeyDown"); });
        $btn.each(function (index) { $(this).attr("onkeyup", "onUserSelectKeyUp"); });
        $btn.each(function (index) { $(this).attr("onblur", "onUserSelectBlur"); });
        $btn.each(function (index) { $(this).attr("onvaluechanged", "onUserSelectValueChanged"); });
        $btn.addClass("userselectbox");
        $btn.find("input").bind("dragenter", function () { return false });
        if (mini.get(name + "_grid") == null) {
            var userGrid = new mini.DataGrid();
            userGrid.set(
                $.extend({
                    id: name + "_grid",
                    width: 300,
                    height: 280,
                    style: "position:absolute;z-index:9999;",
                    borderStyle: "border-bottom:1px solid #999999;",
                    showPager: false,
                    visible: false,
                    url: "",
                    onrowclick: onUserGridRowClick,
                    columns:
                 [{ field: "Name", headerAlign: "center", width: 60, header: "姓名" },
                 { field: "WorkNo", headerAlign: "center", width: 60, header: "工号" },
                 { field: "DeptName", headerAlign: "center", header: "所属部门"}]
                }, selectorParamSettings.gridParamSettings)
            );
            userGrid.render(document.body);
        }

    }
}

function showUserGrid(userSelect) {
    var userGrid = mini.get(userSelect.id + "_grid");
    var $userSelect = $(userSelect.getEl());
    var $grid = $(userGrid.getEl());
    var left = $userSelect.offset().left - (userGrid.getWidth() - userSelect.getWidth());
    if (($userSelect.offset().left + userGrid.getWidth()) <= $(window).width()
        || left <= 0) {
        left = $userSelect.offset().left;
    }
    $grid.css("left", left);
    $grid.css("top", $userSelect.offset().top + userSelect.getHeight());
    userGrid.show();
}

function onUserSelectFocus(e) {
    var settings = getSettings(selectorSettingss, e.sender.name);

    //如果grid为url为空则设url
    var grid = mini.get(e.sender.id + "_grid");
    if ($.trim(grid.url) == "") {
        grid.setUrl(USERSELECT_GRIDURL_TMPL.replace("{OrgIDs}", $.trim(settings.OrgIDs)).replace("{RoleIDs}", $.trim(settings.RoleIDs)));
    }

    var userData = null;
    if (typeof (settings.data) != "undefined")
        userData = settings.data;
    if ($.trim(e.sender.getText()) != "") {
        var txt = e.sender.getText();
        var selectMode = settings.selectMode;
        var val = txt.lastIndexOf(",") == txt.length - 1 || selectMode == "single" ? txt : txt + ",";
        e.sender.setText(val);
        movePosition($(e.sender.getEl()).find("input")[0], val.length);

        var arrVal = e.sender.getValue().split(',');
        var arrTxt = txt.split(',');
        if (userData == null) {
            userData = [];
            $.each(arrVal, function (i, val) {
                if ($.trim(val) != "" && typeof (arrTxt[i]) != "undefined" && $.trim(arrTxt[i]) != "")
                    userData.push({ "ID": val, "Name": arrTxt[i] });
            });
        }
    }
    else {
        userData = [];
    }
    settings.oldData = mini.clone(userData);
    settings.data = userData;
}

function onUserSelectKeyDown(e) {
    if (e.sender.allowInput && e.sender.enabled && !(e.sender.readOnly)) {
        var userGrid = mini.get(e.sender.id + "_grid");
        var event = e.htmlEvent;
        var txt = e.sender.getText();

        switch (event.keyCode) {
            case 38: //up
            case 40: //down
                if (userGrid.visible && userGrid.data.length > 0) {
                    var sel = userGrid.getSelected();
                    if (sel) {
                        var rowIndex = userGrid.indexOf(sel);
                        if (event.keyCode == 38) {
                            if (rowIndex > 0) {
                                rowIndex--;
                                userGrid.deselect(sel);
                                userGrid.select(userGrid.getRow(rowIndex));
                            }
                        }
                        else if (event.keyCode == 40) {
                            if (rowIndex < userGrid.data.length - 1) {
                                rowIndex++;
                                userGrid.deselect(sel);
                                userGrid.select(userGrid.getRow(rowIndex));
                            }
                        }
                    }
                    else {
                        userGrid.select(userGrid.getRow(0));
                    }
                }
                break;
            case 8: 	//backspace
            case 46: //delete
                if ($.trim(txt) != "") {
                    var userSelectData = getSettings(selectorSettingss, e.sender.name).data;
                    var objUserText = $(e.sender.getEl()).find("input")[0];
                    var pos = getPosition(objUserText);
                    if (pos == 0 && event.keyCode == 8) return;
                    var indexPos = txt.substring(0, pos).split(',').length - 1;
                    var arr = txt.split(',');

                    if (indexPos > userSelectData.length - 1) {
                        e.sender.setValue(getUserIDArray(userSelectData).join(','));
                    }
                    else {
                        var posStart = -1;
                        if ($.trim(arr[indexPos]) != "") {
                            if (event.keyCode == 8 && indexPos == 0) {
                                posStart = 1;
                            }
                            arr[indexPos] = "";
                            userSelectData.splice(indexPos, 1);
                        }
                        else {
                            var indexTmp = event.keyCode == 8 ? indexPos - 1 : indexPos + 1;
                            if (arr[indexTmp] != "undefined")
                                arr.splice(indexTmp, 1);
                            if (userSelectData[indexTmp] != "undefined")
                                userSelectData.splice(indexTmp, 1);
                            if (indexTmp == 0) {
                                posStart = 1;
                            }
                        }
                        e.sender.setText(arr.join(','));
                        if (posStart > -1) {
                            movePosition(objUserText, posStart);
                        }
                        else {
                            movePosition(objUserText, arr.slice(0, indexPos + 1).join(',').length);
                        }
                    }
                }
                break;
            case 9: //tab
            case 13: //enter
            case 27: //escr
            case 32: 	//space
            case 35: 	//end
            case 36: 	//home
            case 37: 	//left
            case 39: //right
            case 46: //delete
                break;
            default:
                var pos = getPosition($(e.sender.getEl()).find("input")[0]);
                if (txt.substring(pos, txt.length).indexOf(",") > -1) {
                    var obj = $(e.sender.getEl()).find("input")[0];
                    movePosition(obj, obj.value.length);
                }
        }
    }
}

function onUserSelectKeyUp(e) {
    if (e.sender.allowInput && e.sender.enabled && !(e.sender.readOnly)) {
        var userGrid = mini.get(e.sender.id + "_grid");
        var txt = e.sender.getText();
        var event = e.htmlEvent;

        switch (event.keyCode) {
            case 27: //escr
            case 8: 	//backspace
            case 46: //delete
                if (userGrid.visible)
                    userGrid.hide();
                break;
            case 37: //left
            case 38: //up
            case 39: //right
            case 40: //down
                break;
            case 13: //enter
            case 9: //tab
                if (userGrid.visible) {
                    var sel = userGrid.getSelected();
                    if (sel) {
                        //赋值
                        addUser(e.sender, sel);
                        userGrid.hide();
                    }
                }
                break;
            default:
                var userSelectData = getSettings(selectorSettingss, e.sender.name).data;
                txt = txt.replace(getUserNameArray(userSelectData).join(',') + ",", "");
                var userid = getUserIDArray(userSelectData).join(',');
                userGrid.load({
                    key: txt,
                    value: userid
                }, function (data) {
                    if (userGrid.data.length > 0) {
                        if (userGrid.data.length == 1) {
                            //赋值
                            addUser(e.sender, userGrid.data[0]);
                            if (userGrid.visible)
                                userGrid.hide();
                        }
                        else {
                            showUserGrid(e.sender);
                        }
                    }
                    else {
                        if (userGrid.visible)
                            userGrid.hide();
                    }
                });
        }
        e.sender.setValue(getUserIDArray(getSettings(selectorSettingss, e.sender.name).data).join(','));
    }
}

function onUserSelectBlur(e) {
    //焦点不在grid上则隐藏grid
    setTimeout(function () {
        if (!(e.sender.enabled) || e.sender.readOnly) { return; }
        var userGrid = mini.get(e.sender.id + "_grid");
        if (!isGridClick) {
            userGrid.hide();
            var settings = getSettings(selectorSettingss, e.sender.name);
            e.sender.setText(getUserNameArray(settings.data).join(','));
            if (getUserIDArray(settings.oldData).join(',') != getUserIDArray(settings.data).join(',')) {
                onSelected(settings.data, $.extend(settings, { selectorId: e.sender.id }));
                if (typeof (settings.onUserSelectChanged) != "undefined") {
                    settings.onUserSelectChanged(e.sender);
                }
            }
        }
    }, 200);
}

function onUserSelectValueChanged(e) {
    var arrUserId = getUserIDArray(getSettings(selectorSettingss, e.sender.name).data);
    e.sender.setValue(arrUserId.join(','));
}

var isGridClick = false;
function onUserGridRowClick(e) {
    isGridClick = true;
    var rec = e.record;
    var userSelect = mini.get(e.sender.id.replace("_grid", ""));
    addUser(userSelect, rec);
    var arrUserName = getUserNameArray(getSettings(selectorSettingss, userSelect.name).data);
    userSelect.setText(arrUserName.join(','));
    e.sender.hide();
    isGridClick = false;

    if (userSelect.validate) {
        userSelect.validate();
    }
}

function addUser(miniCtrl, userInfo) {
    var userid = userInfo.ID
    var username = userInfo.Name;
    if ($.trim(miniCtrl.name) != "") {
        var userSelectData = getSettings(selectorSettingss, miniCtrl.name).data;
        var selectMode = getSettings(selectorSettingss, miniCtrl.name).selectMode;
        if (selectMode == "single") {
            userSelectData = [];
        }
        if ((getUserIDArray(userSelectData).join(',') + ",").indexOf(userid + ",") == -1)
            userSelectData.push(userInfo);
        miniCtrl.setValue(getUserIDArray(userSelectData).join(','));
        var txt = getUserNameArray(userSelectData).join(',');
        if (selectMode == "single")
            miniCtrl.setText(txt);
        else
            miniCtrl.setText(txt + ",");

        getSettings(selectorSettingss, miniCtrl.name).data = userSelectData;
    }
}

function getUserIDArray(userData) {
    var val = [];
    if (typeof (userData) != "undefined") {
        $.each(userData, function (i, user) {
            if ($.trim(user.ID) != "") {
                val.push(user.ID);
            }
        });
    }
    return val;
}

function getUserNameArray(userData) {
    var val = [];
    if (typeof (userData) != "undefined") {
        $.each(userData, function (i, user) {
            if ($.trim(user.Name) != "") {
                val.push(user.Name);
            }
        });
    }
    return val;
}

//单行文本框
function getPosition(ctrl) {
    var CaretPos = 0;
    if (document.selection) { // IE Support 
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    } else if (ctrl.selectionStart || ctrl.selectionStart == '0') {// Firefox support 
        CaretPos = ctrl.selectionStart;
    }
    return (CaretPos);
}

function movePosition(obj, len) {
    obj.focus();
    if (document.selection) {
        var sel = obj.createTextRange();
        sel.moveStart('character', len);
        sel.collapse();
        sel.select();
    } else if (typeof obj.selectionStart == 'number' && typeof obj.selectionEnd == 'number') {
        obj.selectionStart = obj.selectionEnd = len;
    }
}


//--------------------------------mini-word控件 从mini.Control继承------------------------------------------
mini.ux.Word = function () {
    mini.ux.Word.superclass.constructor.call(this);
    this.initControls();
    this.initEvents();
}

mini.extend(mini.ux.Word, mini.Control, {
    formField: true,
    module: document.location.pathname.split('/')[1],
    desc: "Word",
    grid_id: "",
    required: false, //是否必填
    readonly: false, //是否只读
    disabled: false, //是否禁用
    revise: false,
    uiCls: "mini-word",

    initControls: function () {
        mini.parse(this.el);
        var attrs = mini.ux.Word.superclass.getAttrs.call(this, this.el);
        var $border = $("<span>").addClass("mini-word-border");
        var $a = $("<a></a>").attr("href", "javascript:void(0);").text(this.desc);
        var $spanLink = $("<span></span>").css({ "padding": "1px", "vertical-align": "middle" }).append($a);
        var $spanDel = $("<span>").addClass("mini-singlefile-remove").attr("title", "删除").hide();
        $(this.el).append($border.append($spanLink).append($spanDel)).append($("<input>").attr("type", "hidden"));
        this._border = $border;
        this._aLink = $a;
        this._btnDelete = $(this.el).find(".mini-singlefile-remove");
        this._hidDocID = $(this.el).find("input[type='hidden']");
    },
    initEvents: function () {
        var _self = this;
        this._aLink.bind("click", function (event) {
            var url = _self.getUrl(_self.grid_id);
            openWindow(url, {
                title: "在线Office协同办公组件",
                width: "90%",
                height: "90%"
            });
        });
        this._btnDelete.bind("click", function (event) {
            _self.removeValue();
        });

        //监听grid,treegrid的cellbeginedit
        $.each($(".mini-grid,mini-treegrid"), function (i, grid) {
            mini.get(grid.id).on("cellbeginedit", function (e) {
                _self.grid_id = e.sender.id;
            });
        });
        mini.on(this._hidDocID[0], "propertychange", this.__OnChange, this);
    },
    setDesc: function (desc) {
        this.desc = desc;
        this._aLink.text(this.desc);
    },
    getValue: function () {
        return this._hidDocID.val();
    },
    setValue: function (docID) {
        this._hidDocID.val(docID);
        this.setStatusStyle();
    },
    validate: function () {
        var isVal = this.isValid();
        var errorId = this.id + "-errorIcon";
        if (!isVal) {
            if ($("#" + errorId).length == 0) {
                //添加error图标
                var $imgError = $("<span>").attr("id", errorId).addClass("mini-word-error").attr("title", "不能为空");
                $(this._border).append($imgError);
            }
            return false;
        }
        else {
            $("#" + errorId).remove();
            return true;
        }
    },
    isValid: function () {
        if (this.required) {
            if (this.getValue() != "") {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    },
    setReadOnly: function (readonly) {
        if (readonly) {
            this.readonly = true;
        }
        else {
            this.readonly = false;
        }
        this.setStatusStyle();
    },
    setDisabled: function (disabled) {
        if (disabled) {
            this.disabled = true;
        }
        else {
            this.disabled = false;
        }
        this.setStatusStyle();
    },
    setRequired: function (required) {
        if (required) {
            this.required = true;
        }
        else {
            this.required = false;
        }
    },
    setRevise: function (revise) {
        this.revise = revise || revise == "true";
    },
    setStatusStyle: function () {
        if (this.readonly || this.disabled) {
            this._btnDelete.hide();
            if (this._hidDocID.val() != "") {
                this._border.show();
            }
            else {
                this._border.hide();
            }
        }
        else {
            this._border.show();
            if (this._hidDocID.val() != "")
                this._btnDelete.show();
            else
                this._btnDelete.hide();
        }
    },
    setCallback: function (callBack) {
        this.callback = callBack;
    },
    getUrl: function (gridId) {
        var url = "/" + this.module + "/WebOffice.axd?mini_ctrl=" + this.id + "&callback=wordSaveCallBack";
        if ($.trim(this._hidDocID.val()) != "")
            url += "&DocID=" + $.trim(this._hidDocID.val());
        if (this.readonly || this.disabled) {
            url += "&readonly=True";
        }
        if (this.revise) {
            url += "&revise=True";
        }
        if ($.trim(gridId) != "") {
            url += "&grid_id=" + gridId;
        }
        return url;
    },
    removeValue: function () {
        if ($.trim(this._hidDocID.val()) != "") {
            var _self = this;
            var _val = _self.getValue();
            msgUI("确认要删除文件吗？", 2,
                function (action) {
                    if (action == "ok") {
                        _self.setValue("");
                        if (_self.grid_id != "") {
                            wordSaveUpdateGrid(_self.grid_id, "");
                        }
                    }
                }
            );
        }
    },
    setModule: function (module) {
        this.module = module;
    },
    getAttrs: function (el) {
        var attrs = mini.ux.Word.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "required", "readonly", "disabled", "desc", "revise", "module", "onchange"]
        );
        return attrs;
    },
    __OnChange: function (e) {
        var t = mini.findParent(e.target, "mini-word");
        if (t) {
            var val = this.getValue();
            var ev = {
                sender: this,
                value: val
            };
            this.fire("change", ev);
        }
    }

});

function wordSaveCallBack(docid, minid, gridid) {
    if (mini.get(minid)) {
        mini.get(minid).setValue(docid);
        mini.get(minid).validate();
    }
    if (mini.get(gridid)) {
        wordSaveUpdateGrid(gridid, docid);
    }
}

function wordSaveUpdateGrid(gridid, docid) {
    var grid = mini.get(gridid);
    if (grid) {
        var cell = grid.getCurrentCell();
        if (cell) {
            var field = cell[1]["field"];
            var row = grid.getRow(cell[0]["_id"] - 1);
            var newData = eval("({ " + field + ": '" + docid + "'})");
            grid.updateRow(row, newData);
        }
    }
}

mini.regClass(mini.ux.Word, "word");


//--------------------------------mini-pinyincomplete控件 从mini.autocomplete继承------------------------------------------
mini.ux.PinYinComplete = function () {
    mini.ux.PinYinComplete.superclass.constructor.call(this);
    this.initControls();
}

mini.extend(mini.ux.PinYinComplete, mini.AutoComplete, {
    formField: true,
    enumCode: "",
    valueFromSelect: true,
    disabled: false,
    uiCls: "mini-pinyincomplete",
    initControls: function () {
        this.setValueField("value");
        this.setTextField("text");
    },
    setEnumCode: function (enumCode) {
        this.enumCode = enumCode;
        this.url = "/Base/Meta/Enum/GetItemListByPinYin?EnumCode=" + enumCode;
    },
    setDisabled: function (disabled) {
        if (disabled) {
            this.disabled = true;
            this.disable();
        }
        else {
            this.disabled = false;
            this.enable();
        }
    },
    getAttrs: function (el) {
        var attrs = mini.ux.PinYinComplete.superclass.getAttrs.call(this, el);
        mini._ParseString(el, attrs,
            [
                "required", "readonly", "disabled", "enumCode"]
        );
        return attrs;
    }
});

mini.regClass(mini.ux.PinYinComplete, "pinyincomplete");

