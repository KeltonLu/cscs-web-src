<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report007_01print.aspx.cs" Inherits="Report_Report007_01print" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>廠商製卡卡數明細表</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%; height: 600px;">
                <tr>
                    <td>
                        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0"></asp:ScriptManager>
                        <div style="overflow: auto;">
                            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
                                Height="600px" ProcessingMode="Remote" Width="1000px" ShowFindControls="False" ShowRefreshButton="False">
                            </rsweb:ReportViewer>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
