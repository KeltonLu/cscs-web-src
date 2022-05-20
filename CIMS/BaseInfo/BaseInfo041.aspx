<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo041.aspx.cs" Inherits="BaseInfo_BaseInfo041" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>角色管理</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="PageTitle" colspan="2">
                        角色管理
                    </td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td style="width: 15%" align="right">
                        角色編號：
                    </td>
                    <td align="left" style="width: 20%">
                        <asp:TextBox ID="txtRoleID" runat="server" MaxLength="7"></asp:TextBox>
                    </td>
                    <td style="width: 15%" align="right">
                        角色名稱：
                    </td>
                    <td align="left">
                        <asp:TextBox ID="txtRoleName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td align="right" colspan="4">
                        <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                        <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbdRoles" runat="server" DataKeyNames="RoleID" OnOnSetDataSource="gvpbdRoles_OnSetDataSource"
                                                AutoGenerateColumns="False" FirstPageText="首頁" LastPageText="尾頁" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" CheckRowNumber="True" OnRowDataBound="gvpbdRoles_RowDataBound"
                                                CellPadding="0" Width="100%" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                EditPageUrl="#" AllowPaging="True" AllowSorting="True">
                                                <HeaderStyle BackColor="#B9BDAA" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="首頁" LastPageText="尾頁" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:HyperLinkField DataTextField="ROLEID" HeaderText="角色編號" SortExpression="ROLEID"
                                                        DataNavigateUrlFields="ROLEID" DataNavigateUrlFormatString="BaseInfo041Edit.aspx?ActionType=Edit&ID={0}">
                                                        <itemstyle horizontalalign="Left" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="RoleName" HeaderText="角色名稱">
                                                        <itemstyle width="16%" horizontalalign="left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="刪除角色">
                                                        <itemstyle horizontalalign="Center" width="100px" />
                                                        <itemtemplate>
                                    <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="ibtnDelete_Command"></asp:ImageButton>
                                
</itemtemplate>
                                                    </asp:TemplateField>
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
    </form>
</body>
</html>
