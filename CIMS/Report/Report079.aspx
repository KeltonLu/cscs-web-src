<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report079.aspx.cs" Inherits="Report_Report079" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>廠商卡片庫存查核表</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">廠商卡片庫存查核表</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td align="left" style="width: 43px">
                                    <font color="red">*</font>日期：</td>
                                <td style="width: 119px">
                                    <asp:TextBox ID="txtDate_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtDate_Time')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                </td>
                                <td align="right" style="width: 70px">
                                    <font color="red">*</font>Perso廠：</td>
                                <td>
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                           </tr>
                           <tr valign="baseline">
                            <td align="right" style="width: 43px">
                                 <font color="red">*</font>用途：</td>
                            <td>
                                <asp:DropDownList Style="position: relative" ID="dropUse" runat="server" OnSelectedIndexChanged="dropUse_SelectedIndexChanged"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                                </td>
                                <td align="right" style="width: 70px">
                                <font color="red">*</font>群組：
                                </td>
                                <td>
                                <asp:DropDownList Style="position: relative" ID="dropGroup" runat="server">
                                </asp:DropDownList>
                            </td>
                             </tr>              
                            <tr valign="baseline">
                                <td colspan="8" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="top">
                    <td style="height: 415px">
                        <div style="overflow:auto;">
                            <rsweb:ReportViewer ID="ReportView1" runat="server" ProcessingMode="Remote" Width="100%"
                                ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False">
                            </rsweb:ReportViewer>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
