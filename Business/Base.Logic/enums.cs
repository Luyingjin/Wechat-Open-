using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Base.Logic
{

    [Description("角色类型")]
    public enum RoleType
    {
        [Description("系统角色")]
        SysRole,
        [Description("组织角色")]
        OrgRole,
    }

    [Description("资源类型")]
    public enum ResType
    {
        [Description("分类")]
        Menu,
        [Description("数据对象")]
        Data,
        [Description("按钮对象")]
        Button,
        [Description("页面对象")]
        Page,
        [Description("业务对象")]
        Code,
        [Description("字段可见")]
        Field,
        [Description("字段编辑")]
        FieldEdit,
    }

    [Description("数据过滤类型")]
    public enum DataFilterType
    {
        [Description("全部数据")]
        All,
        [Description("本部门数据")]
        OrgID,
        [Description("本项目数据")]
        PrjID,
        [Description("本人数据")]
        CreateUserID,
    }

    [Description("枚举类型")]
    public enum EnumType
    {
        [Description("普通枚举")]
        Normal,
        [Description("表枚举")]
        Table,
    }

}
