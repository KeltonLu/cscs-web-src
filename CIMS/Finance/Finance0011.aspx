<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0011.aspx.cs" Inherits="Finance_Finance0011" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>請款放行作業 </title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                        <font class="PageTitle">請款放行作業 </font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="15%">
                                    空白卡厰：</td>
                                <td>
                                    <asp:DropDownList ID="dropBlankFactory" runat="server" >
                                    </asp:DropDownList></td>
                                <td align="right" id="budgetid">
                                    SAP單號：</td>
                                <td>
                                    <asp:TextBox ID="txtSAP_Serial_Number" runat="server" MaxLength="15" onblur="LimitLengthCheck(this,15);" style="ime-mode:disabled;" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    發票號碼：</td>
                                <td>
                                    <asp:TextBox ID="txtInvoiceNumber" runat="server" MaxLength="12"></asp:TextBox></td>
                                <td align="right">
                                    請款日期：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="請款日期区间迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    放行狀態：</td>
                                <td>
                                    <asp:DropDownList ID="dropPass_Status" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="1">暫存</asp:ListItem>
                                        <asp:ListItem Value="2">退回</asp:ListItem>
                                        <asp:ListItem Value="3">待放行</asp:ListItem>
                                        <asp:ListItem Value="4">已放行</asp:ListItem>
                                    </asp:DropDownList></td>
                                <td align="right">
                                    出帳狀態：</td>
                                <td>
                                    <asp:DropDownList ID="dropIs_Finance" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="Y">已出帳</asp:ListItem>
                                        <asp:ListItem Value="N">未出帳</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查詢" OnClick="btnSearch_Click" />
                                    &nbsp;
                                    <asp:Button ID="btnAdd" runat="server" CssClass="btn" OnClick="btnAdd_Click" Text="新增" CausesValidation="False" />
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table name="queryResult" id="queryResult" width="100%">
                            <tr>
                                <td colspan="6">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tbody>
                                                    <tr valign="baseline">
                                                        <td>
                                                            <cc1:GridViewPageingByDB ID="gvpbSAP" runat="server" CssClass="GridView" OnOnSetDataSource="gvpbSAP_OnSetDataSource" SortDescImageUrl="~/images/desc.gif"
                                                                SortAscImageUrl="~/images/asc.gif" PreviousPageText="<" NextPageText=">" LastPageText=">>"
                                                                FirstPageText="<<" EditPageUrl="#" EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl=""
                                                                DataKeyNames="SAP_Serial_Number" ColumnsHeaderText="" AutoGenerateColumns="False" AllowSorting="True"
                                                                AllowPaging="True" OnRowDataBound="gvpbSAP_RowDataBound">
                                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                                <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                    PreviousPageText="&lt;" Visible="False" />
                                                                <Columns>
                                                                    <asp:HyperLinkField DataNavigateUrlFormatString="Finance0011Mod.aspx?ActionType=Edit&amp;SAP_Serial_Number={0}"
                                                                        DataTextField="SAP_Serial_Number" HeaderText="SAP單號 " DataNavigateUrlFields="SAP_Serial_Number" SortExpression="SAP_Serial_Number">
                                                                    </asp:HyperLinkField>
                                                                    <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號" SortExpression="Budget_ID">
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號" SortExpression="Agreement_Code">
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Ask_Date" HeaderText="請款日期" SortExpression="Ask_Date">
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Pass_Status" HeaderText="放行狀態" SortExpression="Pass_Status">
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="Is_Finance" HeaderText="出帳狀態 " SortExpression="Is_Finance">
                                                                    </asp:BoundField>
                                                                </Columns>
                                                            </cc1:GridViewPageingByDB>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
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
