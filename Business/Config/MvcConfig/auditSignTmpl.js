var auditSignTmpl_AuditSignTemplete = " <div  style='border:1px solid #828282'>"
                        +"<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                           "' width='100%'>$AuditItem</Table></div>";
var auditSignTmpl_AuditSignTempleteItem = "<tr><td align='left' colspan='3' >意见：</td></tr>"
    + "<tr><td align='left' colspan='6'>$SignComment</td></tr>"
    + "<tr>"
    + "<td align='right'>负责人签字:</td><td align='center' style='width:5px'>&nbsp;</td><td align='center' style='width: 100px'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td>"
    + "<td align='right' style='width:30px'>日期:</td><td align='center' style='width:5px'>&nbsp;</td><td align='center' style='width: 80px'>$SignTime</td></tr>";


var auditSignSingleTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                        + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                           "' width='100%'>$AuditItem</Table></div>";
var auditSignSingleTmpl_AuditSignTempleteItem = "<tr><td align='right'>$SignTitle：</td><td align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td></tr>"
    + "<tr><td align='right'>日期：</td><td align='left'>$SignTime</td></tr>"


  