﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report033print.aspx.cs" Inherits="Report_Report033print" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>紙品物料日耗用預測報表</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <table border="0" cellpadding="0" cellspacing="2" style="width: 800px;height:600px;">
            <tr>
                <td>
                    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0"></asp:ScriptManager>
                    <div style="overflow:auto;">
                        <rsweb:reportviewer id="ReportViewer1" runat="server" font-names="Verdana" font-size="8pt"
                            height="600px" processingmode="Remote" width="800px" ShowFindControls="False" ShowRefreshButton="False">
                        </rsweb:reportviewer>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
