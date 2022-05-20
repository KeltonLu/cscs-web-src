<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SysInfo001.aspx.cs" Inherits="SysInfo_SysInfo001" %>
<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect" TagPrefix="uc2" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>系統使用記錄</title>
    <link href="../css/Global.css"  rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%" >
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">系統使用記錄</font>
                </td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <table border="0" width="100%" cellpadding="0" cellspacing="2">
                       <tr valign="baseline">
                            <td align="right" style="width:15%"> 
                                操作時間：
                            </td>
                            <td align="left" style="height: 24px;width:30%">
                                <asp:TextBox ID="txtOperate_DateFrom" Width="80px" onfocus="WdatePicker()" runat="server"  MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtOperate_DateFrom')})" src="../images/calendar.gif" align="absmiddle">~
                                 <asp:TextBox ID="txtOperate_DateTo" Width="80px" onfocus="WdatePicker()" runat="server"  MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtOperate_DateTo')})" src="../images/calendar.gif" align="absmiddle">
                                &nbsp;
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtOperate_DateFrom"
                                    ControlToValidate="txtOperate_DateTo" ErrorMessage="操作時間迄必須大於起" Operator="GreaterThanEqual"
                                    Type="Date">*</asp:CompareValidator>
                            </td>
                            <td style="width:15%" align="right">
                                操作人員：
                            </td>
                            <td>
                                <asp:DropDownList ID="dropUserID" runat="server">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td align="right">
                                操作類型： 
                            </td>
                            <td colspan="3">
                                <asp:DropDownList ID="dropParam_Code" runat="server">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td align="right" colspan="4">
                                <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
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
            <tr>
                <td>
                    <table width="100%">
                        <tr>
                            <td>
                                 <cc1:GridViewPageingByDB ID="gvpbBudget" runat="server" AllowPaging="True"
                                    DataKeyNames="RID" OnOnSetDataSource="gvpbBudget_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbBudget_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                        <asp:BoundField DataField="Operate_Date" HeaderText="操作時間" >
                                            <itemstyle width="16%" HorizontalAlign="left"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="UserName" HeaderText="操作人員" >
                                            <itemstyle width="16%" HorizontalAlign="left"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="functionname" HeaderText="作業名稱" >
                                            <itemstyle HorizontalAlign="left"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="param_name" HeaderText="操作類型" >
                                            <itemstyle width="10%" HorizontalAlign="left"/>
                                        </asp:BoundField>
                                    </Columns>
                                </cc1:GridViewPageingByDB>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </div>
    </form>
</body>
</html>
