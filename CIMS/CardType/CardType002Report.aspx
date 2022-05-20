<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="CardType002Report.aspx.cs"
    Inherits="CardType_CardType002Report" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡種基本資料維護報表</title>
    <meta http-equiv="pragma" content="no-cache">
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 800px; height: 600px;">
                <tr>
                    <td><%--Legend 2018/01/09 AsyncPostBackTimeout="0" 不檢查超時--%>
                        <asp:ScriptManager ID="ScriptManager1" AsyncPostBackTimeout="0" runat="server"></asp:ScriptManager>
                        <div style="overflow: auto;">
                            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
                                Height="800px" ProcessingMode="Remote" Width="1000px" ShowFindControls="False"
                                ShowRefreshButton="False">
                            </rsweb:ReportViewer>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
