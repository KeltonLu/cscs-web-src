<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report016.aspx.cs" Inherits="Report_Report016" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>匯入小計統計報表</title>
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
                        <font class="PageTitle">匯入小計統計報表</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td td colspan="9">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 10%">
                                                <font color="red">*</font>日期：</td>
                                            <td style="width: 40%">
                                                <asp:TextBox ID="txtDate_TimeFrom" onfocus="WdatePicker()" runat="server" Width="80px"
                                                    MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtDate_TimeFrom')})"
                                                        src="../images/calendar.gif" align="middle">～
                                                <asp:TextBox ID="txtDate_TimeTo" onfocus="WdatePicker()" runat="server" Width="80px"
                                                    MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtDate_TimeTo')})"
                                                        src="../images/calendar.gif" align="middle">
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDate_TimeFrom"
                                                    ErrorMessage="日期起不能為空">*</asp:RequiredFieldValidator>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtDate_TimeTo"
                                                    ErrorMessage="日期迄不能為空">*</asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtDate_TimeFrom"
                                                    ControlToValidate="txtDate_TimeTo" ErrorMessage="日期迄必須大於日期起" Operator="GreaterThanEqual"
                                                    Type="Date">*</asp:CompareValidator></td>
                                            <td style="width: 10%">
                                                <font color="red">*</font>顯示方式：</td>
                                            <td>
                                                <asp:RadioButtonList ID="rblShowType" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem Selected="True" Value="mon">月</asp:ListItem>
                                                    <asp:ListItem Value="day">日</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <font color="red">*</font>統計條件：</td>
                                            <td>
                                                <asp:RadioButtonList ID="rblCon" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rblCon_SelectedIndexChanged"
                                                    RepeatDirection="Horizontal">
                                                    <asp:ListItem Selected="True" Value="1">批次</asp:ListItem>
                                                    <asp:ListItem Value="2">Action</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                            <td style="width: 10%">
                                                <font color="red">*</font>Perso廠：</td>
                                            <td>
                                                &nbsp;<asp:DropDownList ID="dropFactory" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <font color="red">*</font>用途：</td>
                                            <td>
                                                &nbsp;<asp:DropDownList ID="dropCard_Purpose" runat="server" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                </asp:DropDownList></td>
                                            <td style="width: 10%">
                                                <font color="red">*</font>群組：</td>
                                            <td>
                                                &nbsp;<asp:DropDownList ID="dropCard_Group" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <asp:CheckBoxList ID="cblCon" runat="server" RepeatDirection="Horizontal">
                                                </asp:CheckBoxList></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="9" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="top">
                    <td style="height: 415px">
                        <div style="overflow:auto;">
                            <rsweb:ReportViewer ID="ReportView" runat="server" ProcessingMode="Remote" Width="100%"
                                ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False">
                            </rsweb:ReportViewer>
                        </div>
                    </td>
                </tr>
                <tr valign="top" style="display: none;">
                    <td>
                        <asp:GridView ID="gvpbReport" runat="server" Width="100%" OnRowDataBound="gvpbReport_RowDataBound">
                            <HeaderStyle BackColor="#B9BDAA" HorizontalAlign="Center" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <RowStyle CssClass="GridViewRow" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr style="display: none;">
                    <td align="right">
                        <asp:Button ID="btnExcelD" Height="24" runat="server" Text="匯出EXCEL格式" OnClick="btnExcel_Click"
                            class="btn" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
