<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0012.aspx.cs" Inherits="Finance_Finance0012" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>總行會計付款</title>
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
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">總行會計付款</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td width="10%" align="right">
                                    版面簡稱：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30"></asp:TextBox></td>
                                <td align="right">
                                    空白卡廠：
                                    <label>
                                    </label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropBlankFactory" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    SAP單號：</td>
                                <td>
                                    <asp:TextBox ID="txtSAP_Serial_Number" runat="server" MaxLength="15"></asp:TextBox></td>
                                <td align="right">
                                    發票號碼：</td>
                                <td>
                                    <asp:TextBox ID="txtInvoiceNumber" runat="server" MaxLength="12"></asp:TextBox></td>
                            </tr>
                            <tr valign="bottom">
                                <td align="right" valign="bottom">
                                    請款日期：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="請款日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right" valign="bottom">
                                    &nbsp;</td>
                                <td align="right" valign="bottom">
                                    &nbsp;</td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    &nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Text="查詢" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table name="queryResult" id="queryResult" width="100%">
                            <tr>
                                <td align="left" colspan="10" width="100%">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tbody>
                                                    <tr valign="baseline">
                                                        <td>
                                                            <cc1:GridViewPageingByDB ID="gvpbSAP" runat="server" AllowPaging="True" DataKeyNames="SAP_Serial_Number"
                                                                OnOnSetDataSource="gvpbSAP_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" OnRowDataBound="gvpbSAP_RowDataBound">
                                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                                <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                    PreviousPageText="&lt;" Visible="False" />
                                                                <Columns>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="SAP_Serial_Number" DataNavigateUrlFormatString="Finance0012Edit.aspx?ActionType=Edit&amp;SAP_Serial_Number={0}"
                                                                        DataTextField="SAP_Serial_Number" HeaderText="SAP單號 " SortExpression="SAP_Serial_Number" />
                                                                    <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="空白卡廠" SortExpression="Factory_ShortName_CN" />
                                                                    <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號" SortExpression="Budget_ID" />
                                                                    <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號" SortExpression="Agreement_Code" />
                                                                    <asp:BoundField DataField="Ask_Date" HeaderText="請款日期" SortExpression="Ask_Date" />
                                                                    <asp:BoundField DataField="Pay_Date" HeaderText="出帳日期" SortExpression="Pay_Date" />
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
        </div>
     
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
     
     
    </form>
</body>
</html>
