<style type='text/css'>
    .icon-delegate
    {
        background: url(/Workflow/Scripts/Images/delegate.gif) no-repeat;
    }
    .icon-ask
    {
        background: url(/Workflow/Scripts/Images/ask.gif) no-repeat;
    }
    .icon-circulate
    {
        background: url(/Workflow/Scripts/Images/circulate.gif) no-repeat;
    }
     .icon-view
    {
        background: url(/Workflow/Scripts/Images/view.gif) no-repeat;
    }
</style>

<div id='adviceWindow' class='mini-window' title='请输入意见' style='width: 600px; height: 185px;' showmodal='true' allowresize='false' allowdrag='true'>
    <div class='queryDiv'>
        <table>
            <tr>
                <td width='16%' style='text-align: right'>意见输入区</td>
                <td>
                    <input name='Advice' style='width: 100%;height:100px' class='mini-textarea' />
                </td>
                <td width='5%' />
            </tr>
        </table>
        <div>
            <a class='mini-button' name='btnConfirm' onclick='hideWindow("adviceWindow");flowExecMethod();' iconcls='icon-find' style='margin-right: 20px;'>确定</a>
            <a class='mini-button' onclick='hideWindow("adviceWindow");' iconcls='icon-undo'>取消</a>
        </div>
    </div>
</div>
<ul id='flowMenubar' class='mini-menubar' style='width: 100%; position: fixed; top: 0; left: 0; z-index: 100;'>
    <div property='toolbar'>
        <div name="urgency" class="mini-checkbox"  text="是否紧急"></div>&nbsp;&nbsp;
    </div>
</ul>
<div style='width:100%;height:31px'></div>