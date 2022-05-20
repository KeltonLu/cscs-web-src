<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0031.aspx.cs" Inherits="Finance_Finance0031" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>物料郵資費用-請款及會計付款</title>
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
                        <font class="PageTitle">物料郵資費用-請款及會計付款</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="15%">
                                    請款項目：</td>
                                <td>
                                    <asp:DropDownList ID="dropAsk_Project" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="1">寄卡單</asp:ListItem>
                                        <asp:ListItem Value="2">信封</asp:ListItem>
                                        <asp:ListItem Value="3">DM</asp:ListItem>
                                        <asp:ListItem Value="4">郵資費</asp:ListItem>
                                    </asp:DropDownList></td>
                                <td align="right">
                                    SAP單號：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSAP_Serial_Number" runat="server" MaxLength="15" onblur="LimitLengthCheck(this,15);" style="ime-mode:disabled;" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox></td>
                            </tr>
                            <tr>
                            </tr>
                            <tr>
                                <td align="right">
                                    請款日期：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date1" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date1')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date1" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date1')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date1"
                                        ControlToValidate="txtFinish_Date1" ErrorMessage="請款日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right">
                                    出帳日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date2" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date2')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date2" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date2')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToCompare="txtBegin_Date2"
                                        ControlToValidate="txtFinish_Date2" ErrorMessage="出帳日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Text="查詢" />
                                    &nbsp;
                                    <asp:Button ID="btnAdd" runat="server" CssClass="btn" OnClick="btnAdd_Click" Text="新增" />&nbsp;
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                        <asp:Label ID="lbMsg" runat="server" ForeColor="red" Text="查無資料" Visible="false"></asp:Label></td>
                </tr>
                <tr valign="top">
                    <td>
                        <table id="queryResult" width="100%">
                            <tr class="tbl_list_row_2">
                                <td colspan="5">
                                    <cc1:GridViewPageingByDB ID="gvpbMATERIEL_SAP" runat="server" AllowPaging="True"
                                        AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                        EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvpbMATERIEL_SAP_OnSetDataSource"
                                        PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                        OnRowDataBound="gvpbMATERIEL_SAP_RowDataBound">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <Columns>
                                            <asp:BoundField DataField="Ask_Date" HeaderText="請款日期" SortExpression="Ask_Date" />
                                            <asp:HyperLinkField DataNavigateUrlFields="RID" DataNavigateUrlFormatString="Finance0031Mod.aspx?ActionType=Edit&amp;RID={0}"
                                                DataTextField="Material_Type_Name" HeaderText="請款項目" SortExpression="Material_Type_Name" />
                                            <asp:BoundField DataField="Sum" HeaderText="出帳金額 " SortExpression="Sum" DataFormatString="{0:N2}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SAP_ID" HeaderText="SAP單號 " SortExpression="SAP_ID" />
                                            <asp:BoundField DataField="Pay_Date" HeaderText="出帳日 " SortExpression="Pay_Date" />
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
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
