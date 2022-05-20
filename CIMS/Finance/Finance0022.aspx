<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0022.aspx.cs" Inherits="Finance_Finance0022" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>帳務查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>    
            <table bgcolor="#FFFFFF" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">帳務查詢</font></td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="bottom">
                                <td align="right">
                                    <span class="style1">*</span>用途：</td>
                                <td width="10%">
                                    <asp:DropDownList ID="dropCard_Purpose" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged">
                                    </asp:DropDownList></td>
                                <td align="right">
                                    <span class="style1">*</span>群組：</td>
                                <td width="69%">
                                    &nbsp;<asp:DropDownList ID="dropCard_Group" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="bottom">
                                <td width="14%" align="right">
                                    <span class="style1">*</span>Perso廠商：</td>
                                <td colspan="3">
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="bottom">
                                <td align="right">
                                    <span class="style1">*</span>卡片耗用日期：</td>
                                 <td colspan = "3">
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                     ~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="卡片耗用日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBegin_Date"
                                         ErrorMessage="卡片耗用日期起不能為空">*</asp:RequiredFieldValidator>
                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtFinish_Date"
                                         ErrorMessage="卡片耗用日期迄不能為空">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="8" align="right">
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Text="確定" />
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table width="100%" border="0" id="queryResult" style="display: table-row">
                            <tr>
                                <td align="left" colspan="12">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tbody>
                                                    <tr valign="baseline">
                                                        <td>
                                                            <cc1:GridViewPageingByDB ID="gvpbProjectCost" runat="server" AllowPaging="False" AllowSorting="False"
                                                                AutoGenerateColumns="True" ColumnsHeaderText="" CssClass="GridView" 
                                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvpbProjectCost_OnSetDataSource"
                                                                PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" OnRowDataBound="gvpbProjectCost_RowDataBound">
                                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                                <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                    PreviousPageText="&lt;" Visible="False" />
                                                            </cc1:GridViewPageingByDB>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="12" align="right">
                                    &nbsp;<asp:Button ID="btnExport" runat="server" CssClass="btn"
                                        OnClick="btnExport_Click" Text="匯出EXCEL文檔" Visible="False" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
