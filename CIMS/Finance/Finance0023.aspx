<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0023.aspx.cs" Inherits="Finance_Finance0023" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>一般代製費用明細查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">一般代製費用明細查詢</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="13%">
                                    <span class="style1">*</span>用途：</td>
                                <td width="40%">
                                    <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" OnSelectedIndexChanged="dropCard_Purpose_RID_SelectedIndexChanged"
                                        AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                                <td width="10%" align="right">
                                    群組：</td>
                                <td width="37%">
                                    <asp:DropDownList ID="dropCard_Group_RID" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Perso厰：</td>
                                <td>
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    版面簡稱：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtName" MaxLength="20" onblur="LimitLengthCheck(this,20);" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    卡片耗用日期起:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBDate_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBDate_Time')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    ~
                                    <asp:TextBox ID="txtEDate_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEDate_Time')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBDate_Time"
                                        ControlToValidate="txtEDate_Time" ErrorMessage="耗用日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator></td>
                                <td align="right">
                                    代製項目：</td>
                                <td>
                                    <asp:TextBox ID="txtFinance" runat="server" MaxLength="20" onblur="LimitLengthCheck(this,20);"></asp:TextBox></td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="4" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="top" width="100%">
                    <td>
                        <table name="errorMessage" id="errorMessage" width="100%">
                            <tr valign="top" width="100%">
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbFinance" runat="server" CssClass="GridView" EditPageUrl="#"
                                        EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl="" ColumnsHeaderText=""
                                        SortDescImageUrl="~/images/desc.gif" SortAscImageUrl="~/images/asc.gif" OnRowDataBound="gvpbFinance_RowDataBound"
                                        PreviousPageText="<" NextPageText=">" LastPageText=">>" FirstPageText="<<" AutoGenerateColumns="False"
                                        AllowSorting="True" OnOnSetDataSource="gvpbFinance_OnSetDataSource" AllowPaging="True">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:BoundField DataField="Group_Name" HeaderText="用途群組" />
                                            <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠" />
                                            <asp:BoundField DataField="Date_Time" HeaderText="卡片耗用日期" />
                                            <asp:BoundField HeaderText="代製項目" DataField="Project_Name" />
                                            <asp:BoundField HeaderText="項目單價" DataField="Price" DataFormatString="{0:N4}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>                                           
                                             <asp:BoundField DataField="Replace_Name" HeaderText="替換后版面簡稱" />                                          
                                             <asp:BoundField DataField="Replace_Number" HeaderText="替換后數量" DataFormatString="{0:N0}"  HtmlEncode="False">
                                                <itemstyle width="8%" horizontalalign="Right" />
                                            </asp:BoundField>
                                             <asp:BoundField DataField="Name" HeaderText="替換前版面簡稱" />
                                            <asp:BoundField DataField="Number" HeaderText="替換前數量" DataFormatString="{0:N0}"  HtmlEncode="False">
                                                <itemstyle width="8%" horizontalalign="Right" />
                                            </asp:BoundField>                                           
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btnExcelD" Visible="false" Height="24" runat="server" Text="匯出EXCEL格式"
                                        OnClick="btnExcel_Click" class="btn" />
                                </td>
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
