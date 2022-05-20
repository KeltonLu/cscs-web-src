<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InOut006.aspx.cs" Inherits="InOut_InOut006" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>系統自動匯入記錄</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#FFFFFF" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">系統自動匯入記錄</font></td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="bottom">
                                <td width="14%" align="right">
                                    檔案類型：</td>
                                <td>
                                    <asp:DropDownList ID="dropFile_Name" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="1">小計檔</asp:ListItem>
                                        <asp:ListItem Value="2">廠商庫存異動檔</asp:ListItem>
                                        <asp:ListItem Value="3">代製費用異動檔</asp:ListItem>
                                        <asp:ListItem Value="4">物料庫存異動檔</asp:ListItem>
                                        <asp:ListItem Value="5">次月換卡預測檔</asp:ListItem>
                                        <asp:ListItem Value="6">年度換卡預測檔</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="bottom">
                                <td align="right">
                                    匯入日期：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="日期區間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator></td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="6" align="right">
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn" Text="查詢" OnClick="btnSubmit_Click" />&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp; &nbsp; &nbsp; &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:GridViewPageingByDB ID="gvpbIMPORT_HISTORY" runat="server" CssClass="GridView"
                            Width="1px" SortDescImageUrl="~/images/desc.gif" SortAscImageUrl="~/images/asc.gif"
                            PreviousPageText="<" NextPageText=">" LastPageText=">>" FirstPageText="<<" EditPageUrl="#"
                            EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl="" ColumnsHeaderText=""
                            AutoGenerateColumns="False" AllowSorting="True" OnRowDataBound="gvpbIMPORT_HISTORY_RowDataBound">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" Visible="False" />
                            <RowStyle CssClass="GridViewRow" />
                            <Columns>
                                <asp:BoundField DataField="Import_Date" HeaderText="匯入日期">
                                    <itemstyle horizontalalign="Left"></itemstyle>
                                    <headerstyle horizontalalign="Left"></headerstyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="File_Type" HeaderText="檔案類型" HtmlEncode="False">
                                    <itemstyle horizontalalign="Left"></itemstyle>
                                    <headerstyle horizontalalign="Left"></headerstyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="File_Name" HeaderText="檔名">
                                    <itemstyle horizontalalign="Left"></itemstyle>
                                    <headerstyle horizontalalign="Left"></headerstyle>
                                </asp:BoundField>
                            </Columns>
                        </cc1:GridViewPageingByDB>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
