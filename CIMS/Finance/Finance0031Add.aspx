<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0031Add.aspx.cs" Inherits="Finance_Finance0031Add" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款及總行會計付款資訊新增</title>
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
                        <font class="PageTitle">物料請款帳務-請款及總行會計付款資訊新增</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right">
                                    品名：</td>
                                <td>
                                    &nbsp;<asp:TextBox ID="txtMaterial_Name" runat="server"></asp:TextBox></td>
                                <td align="right">
                                    採購日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date1" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date1')})" src="../images/calendar.gif" />
                                    （起）~ &nbsp;<asp:TextBox ID="txtFinish_Date1" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date1')})" src="../images/calendar.gif" />
                                    （迄）<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date1"
                                        ControlToValidate="txtFinish_Date1" ErrorMessage="日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">&nbsp;</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    &nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Text="查詢" />&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table id="queryResult" width="100%">
                            <tr class="tbl_list_row_2">
                                <td colspan="6">
                                    <cc1:GridViewPageingByDB ID="gvpbMATERIEL_PURCHASE_FORM" runat="server" AllowPaging="True"
                                        AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                        EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvpbMATERIEL_PURCHASE_FORM_OnSetDataSource"
                                        PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <Columns>
                                            <asp:BoundField DataField="PurchaseOrder_Detail_RID" HeaderText="採購單號" />
                                            <asp:BoundField DataField="Material_Name" HeaderText="品名">
                                                <itemstyle horizontalalign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Unit_Price" HeaderText="單價 " DataFormatString="{0:N2}"
                                                HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Total_Num" HeaderText="數量 " DataFormatString="{0:N0}"
                                                HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Total_Price" HeaderText="金額 " DataFormatString="{0:N2}"
                                                HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="是否請款 ">
                                                <itemstyle horizontalalign="Center" />
                                                <itemtemplate>
<asp:CheckBox id="cbAsk_Money" runat="server" Text="請款" __designer:wfdid="w6"></asp:CheckBox>
</itemtemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="6">
                                    &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                                        Text="確定" Visible="False" />
                                    &nbsp;
                                    <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                                        Text="取消"  />&nbsp; &nbsp;</td>
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
