<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Materiel002.aspx.cs" Inherits="Materiel_Materiel002" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>寄卡單種類基本檔</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="1" style="width: 100%">
                <tr valign="baseline" height="20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">寄卡單種類基本檔</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td style="width: 15%" align="right">
                                    品名：
                                </td>
                                <td style="width: 85%" align="left">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" onkeyup="LimitLengthCheck(this, 30)"></asp:TextBox>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" colspan="4">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />
                                    &nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline" height="20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="baseline">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbCardExponent" runat="server" AllowPaging="True"
                                                DataKeyNames="RID" OnOnSetDataSource="gvpbCardExponent_OnSetDataSource" AllowSorting="True"
                                                AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbCardExponent_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Serial_Number" HeaderText="品名編號" SortExpression="Serial_Number">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="Name" HeaderText="品名" DataNavigateUrlFields="RID"
                                                        DataNavigateUrlFormatString="Materiel002Mod.aspx?ActionType=Edit&amp;RID={0}"
                                                        SortExpression="Name">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Unit_Price1" HeaderText="單價">
                                                        <itemstyle horizontalalign="Right" width="15%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Wear_Rate1" HeaderText="耗損率">
                                                        <itemstyle horizontalalign="Right" width="15%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Safe_Type" HeaderText="安全存量">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:BoundField>
                                                     <asp:BoundField DataField="Billing_Type" HeaderText="帳務類別">
                                                        <itemstyle horizontalalign="left" width="10%" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
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
