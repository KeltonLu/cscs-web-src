<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository015.aspx.cs" Inherits="Depository_Depository015" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>物料採購作業</title>
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
                    <td colspan="2" style="width: 971px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" style="width: 971px">
                        <font class="PageTitle">物料採購作業</font></td>
                </tr>
                <tr>
                    <td colspan="2" style="width: 971px">
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    品名：
                                </td>
                                <td width="35%">
                                    <asp:TextBox ID="txtMaterial" runat="server" size="12" MaxLength="30"></asp:TextBox>
                                </td>
                                <td width="10%" align="right">
                                    採購日期：
                                </td>
                                <td width="45%">
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
                            </tr>
                            <tr valign="baseline">
                                <td colspan="4" align="right">
                                    <asp:Button ID="btnSearch" OnClick="btnSearch_Click" runat="server" Text="查詢" CssClass="btn">
                                    </asp:Button>
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnAdd" OnClick="btnAdd_Click" runat="server" Text="新增" CssClass="btn"
                                        CausesValidation="False"></asp:Button>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tbody>
                                        <tr valign="baseline">
                                            <td>
                                                <cc1:GridViewPageingByDB ID="gvpbPurchase" runat="server" CssClass="GridView" EditPageUrl="#"
                                                    EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl="" ColumnsHeaderText=""
                                                    SortDescImageUrl="~/images/desc.gif" SortAscImageUrl="~/images/asc.gif" OnRowDataBound="gvpbPurchase_RowDataBound"
                                                    PreviousPageText="<" NextPageText=">" LastPageText=">>" FirstPageText="<<" AutoGenerateColumns="False"
                                                    AllowSorting="True" OnOnSetDataSource="gvpbPurchase_OnSetDataSource" DataKeyNames="PurchaseOrder_RID"
                                                    AllowPaging="True">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <Columns>
                                                        <asp:HyperLinkField DataNavigateUrlFields="PurchaseOrder_RID" DataNavigateUrlFormatString="Depository015Mod.aspx?ActionType=Edit&amp;PurchaseOrder_RID={0}"
                                                            DataTextField="PurchaseOrder_RID" HeaderText="採購單號" SortExpression="PurchaseOrder_RID">
                                                        </asp:HyperLinkField>
                                                        <asp:TemplateField HeaderText="品名">
                                                            <edititemtemplate>
<asp:TextBox id="TextBox2" runat="server"  __designer:wfdid="w12"></asp:TextBox>
</edititemtemplate>
                                                            <itemstyle horizontalalign="Right" width="10%" />
                                                            <itemtemplate>
<asp:Label id="lblName" runat="server" __designer:wfdid="w1"></asp:Label> 
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Unit_Price" HeaderText="單價" DataFormatString="{0:N2}" HtmlEncode="False">
                                                            <itemstyle horizontalalign="Right" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Total_Num" HeaderText="採購數量" DataFormatString="{0:N0}" HtmlEncode="False">
                                                            <itemstyle horizontalalign="Right" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Total_Price" HeaderText="採購金額" DataFormatString="{0:N2}" HtmlEncode="False">
                                                            <itemstyle horizontalalign="Right" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="送貨Perso廠">
                                                            <edititemtemplate>
<asp:TextBox id="TextBox5" runat="server"  __designer:wfdid="w24"></asp:TextBox>
</edititemtemplate>
                                                            <itemstyle horizontalalign="Left" width="20%" />
                                                            <itemtemplate>
<asp:Label id="lblPersoFactory" runat="server"  __designer:wfdid="w23"></asp:Label>
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="送貨數量">
                                                            <edititemtemplate>
<asp:TextBox id="TextBox6" runat="server" __designer:wfdid="w26"></asp:TextBox>
</edititemtemplate>
                                                            <itemstyle horizontalalign="Right" width="20%" verticalalign="Top" />
                                                            <itemtemplate>
<asp:Label id="lblNumber" runat="server" __designer:wfdid="w25"></asp:Label>
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="交貨日">
                                                            <edititemtemplate>
<asp:TextBox id="TextBox7" runat="server" __designer:wfdid="w29"></asp:TextBox>
</edititemtemplate>
                                                            <itemstyle horizontalalign="Left" verticalalign="Top" />
                                                            <itemtemplate>
<asp:Label id="lblDeliveryDate" runat="server" __designer:wfdid="w28"></asp:Label>
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="結案日期">
                                                            <edititemtemplate>
<asp:TextBox id="TextBox1" runat="server" __designer:wfdid="w3"></asp:TextBox>
</edititemtemplate>
                                                            <itemtemplate>
<asp:Label id="lblCaseDate" runat="server" __designer:wfdid="w2"></asp:Label>
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="SAP單號" DataField="SAP_Serial_Number" />
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
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="False"
            ShowMessageBox="True"></asp:ValidationSummary>
    </form>
</body>
</html>
