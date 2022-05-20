<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report019.aspx.cs" Inherits="Report_Report019" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>多功能報表</title>
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
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">多功能報表</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table width="100%" border="0" cellpadding="0" cellspacing="2" class="tbl_row" style="font-size: 10pt">
                            <tr align="left" valign="bottom">
                                <td width="7%" style="text-align: right">
                                    <span style="color: #ff0000">*</span>日期：
                                </td>
                                <td style="width: 31%">
                                    <asp:TextBox ID="txtBeginDate" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBeginDate')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtEndDate" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEndDate')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期"
                                        ControlToCompare="txtBeginDate" ControlToValidate="txtEndDate" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td style="width: 8%">
                                    <font color="red">*</font>Perso廠：</td>
                                <td style="width: 8%">
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 5%">
                                    <div align="right">
                                        <span style="font-size: 10pt; color: #ff0000">*</span>用途：
                                    </div>
                                </td>
                                <td style="width: 8%">
                                    <asp:DropDownList Style="position: relative" ID="dropUse" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="dropUse_SelectedIndexChanged">
                                    </asp:DropDownList></td>
                                <td style="width: 6%">
                                    <div align="right">
                                        <span style="font-size: 10pt; color: #ff0000">*</span>群組：</div>
                                </td>
                                <td style="width: 10%">
                                    <asp:DropDownList Style="position: relative" ID="dropGroup" runat="server">
                                    </asp:DropDownList></td>
                                <td valign="middle" width="8%" style="text-align: right; font-size: 10pt;">
                                    <span style="color: #ff0000">*</span>顯示方式：
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="radlDate" runat="server" RepeatColumns="2" Height="1px"
                                        Width="78px">
                                        <asp:ListItem Value="day" Selected="True">日</asp:ListItem>
                                        <asp:ListItem Value="month">月</asp:ListItem>
                                    </asp:RadioButtonList></td>
                            </tr>
                            <tr align="left" valign="bottom">
                                <td valign="middle" style="text-align: right">
                                    顯示欄位：
                                    <label>
                                    </label>
                                </td>
                                <td colspan="7">
                                    <label>
                                        <asp:RadioButtonList ID="radl" runat="server" Width="146px" RepeatColumns="2" Height="1px"
                                            OnSelectedIndexChanged="radl_SelectedIndexChanged" AutoPostBack="True">
                                            <asp:ListItem Selected="True">批次</asp:ListItem>
                                            <asp:ListItem>Action</asp:ListItem>
                                        </asp:RadioButtonList></label>
                                    <label>
                                    </label>
                                </td>
                            </tr>
                            <tr align="left" id="trShow1">
                                <td>
                                    &nbsp;</td>
                                <td colspan="7">
                                    <asp:CheckBox ID="CheckBox1" runat="server" Style="position: relative" Checked="True"
                                        Text="3D" />
                                    <asp:CheckBox ID="CheckBox2" runat="server" Style="position: relative" Checked="True"
                                        Text="DA" />
                                    <asp:CheckBox ID="CheckBox3" runat="server" Style="position: relative" Checked="True"
                                        Text="PM" />
                                    <asp:CheckBox ID="CheckBox4" runat="server" Style="position: relative" Checked="True"
                                        Text="RN" /></td>
                            </tr>
                            <tr align="left">
                                <td>
                                    &nbsp;</td>
                                <td colspan="7">
                                    <asp:CheckBoxList ID="CheckBoxList" runat="server" Style="position: relative; left: 0px;"
                                        RepeatDirection="Horizontal" RepeatColumns="15">
                                    </asp:CheckBoxList></td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="8" align="right">
                                    <asp:Button ID="btnSearch" OnClick="btnSearch_Click" runat="server" CssClass="btn"
                                        Text="查詢"></asp:Button>
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                 <tr valign="top">
                    <td style="height: 415px">
                        <div style="overflow:auto;">
                            <rsweb:reportviewer id="ReportView1" runat="server" processingmode="Remote"
                                width="100%" ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False"></rsweb:reportviewer>
                        </div>
                    </td>
                </tr>
                <tr style="display:none">
                    <td>
                        <table width="100%" cellpadding="0" cellspacing="2" border="0">
                            <tr>
                                <td >
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tr valign="baseline">
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbFunction" runat="server" AllowPaging="True" FirstPageText="<<"
                                                            LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" SortAscImageUrl="~/images/asc.gif"
                                                            SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                            EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" OnOnSetDataSource="gvpbFunction_OnSetDataSource"
                                                            OnRowDataBound="gvpbFunction_RowDataBound">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                        </cc1:GridViewPageingByDB>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div align="right">
                            <asp:Button Text="匯出Excel格式" CssClass="btn" runat="server" ID="btnReport" OnClick="btnReport_click" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" Style="position: relative" Width="136px" />
    </form>
</body>
</html>
