<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Materiel003.aspx.cs" Inherits="Materiel_Materiel003" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DM種類基本檔查詢</title>
    <meta http-equiv="pragma" content="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <base target="_self">
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
                    <td class="PageTitle">
                        DM種類基本檔
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="bottom">
                                <td align="right" width="10%">
                                    品名：</td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" size="15" MaxLength="30"></asp:TextBox></td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="6" align="right">
                                    <asp:Button ID="btnQuery" class="btn" runat="server" Text="查詢" OnClick="btnSearch_Click" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnAdd" class="btn" runat="server" OnClick="btnAdd_Click" Text="新增" />
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
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbDM" runat="server" AllowPaging="True" DataKeyNames="RID"
                                                OnOnSetDataSource="gvpbDM_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                OnRowDataBound="gvpbDM_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Serial_Number" HeaderText="品名編號">
                                                        <itemstyle horizontalalign="left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="Name" HeaderText="品名" SortExpression="Name" DataNavigateUrlFields="RID"
                                                        DataNavigateUrlFormatString="Materiel003Mod.aspx?RID={0}">
                                                        <itemstyle horizontalalign="left" width="20%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Price" HeaderText="單價">
                                                        <itemstyle horizontalalign="right" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Rate" HeaderText="耗損率">
                                                        <itemstyle horizontalalign="right" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Date" HeaderText="有效期間">
                                                        <itemstyle horizontalalign="left" width="20%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Safe_Type" HeaderText="安全存量">
                                                        <itemstyle horizontalalign="left" width="20%" />
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
