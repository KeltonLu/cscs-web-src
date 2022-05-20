<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType007.aspx.cs" Inherits="CardType_CardType007" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>匯入項目設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr valign="baseline" style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">匯入項目設定</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td align="right" width="15%">
                                    <span>類別：</span></td>
                                <td width="25%">
                                    <asp:DropDownList ID="dropType" runat="server" DataTextField="Text" DataValueField="Value"
                                        Font-Names="Times New Roman">
                                    </asp:DropDownList>
                                </td>
                                <td align="right" width="15%">
                                    <span>批次：</span></td>
                                <td width="45%">
                                    &nbsp;<asp:DropDownList ID="dropMakeCardType_RID" runat="server" DataTextField="Text"
                                        DataValueField="Value" Font-Names="Times New Roman">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td colspan="4" align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click"
                                        Font-Names="Times New Roman" />&nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" Font-Names="Times New Roman" />
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
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbCardType" runat="server" OnOnSetDataSource="gvpbCardType_OnSetDataSource"
                                                AllowSorting="True" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<"
                                                CssClass="GridView" AutoGenerateColumns="False" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" AllowPaging="True" Font-Names="Times New Roman">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Param_Name" HeaderText="類別" SortExpression="Param_Name">
                                                        <headerstyle width="20%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="File_Name" HeaderText="伺服器檔案名稱" SortExpression="File_Name"
                                                        DataNavigateUrlFields="RID" DataNavigateUrlFormatString="CardType007Mod.aspx?ActionType=Edit&amp;RID={0}">
                                                        <itemstyle horizontalalign="Left" width="15%" />
                                                        <headerstyle width="20%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Text" HeaderText="批次" SortExpression="Text">
                                                        <headerstyle width="20%" />
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
    </form>
</body>
</html>
