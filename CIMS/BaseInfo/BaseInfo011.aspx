<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo011.aspx.cs" Inherits="BaseInfo_BaseInfo011" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>庫存類別設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">庫存類別設定</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="15%">
                                    類別名稱：</td>
                                <td width="85%">
                                    <asp:TextBox ID="txtStatus_Name" onkeyup=" LimitLengthCheck(this, 10)" runat="server"
                                        MaxLength="10" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" />
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
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbCardTypeStatus" runat="server" AllowPaging="True"
                                                OnOnSetDataSource="gvpbCardTypeStatus_OnSetDataSource" AllowSorting="True" FirstPageText="<<"
                                                LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" AutoGenerateColumns="False"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" OnRowDataBound="gvpbCardTypeStatus_RowDataBound">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:HyperLinkField DataTextField="Status_Code" HeaderText="類別編號" SortExpression="Status_Code"
                                                        DataNavigateUrlFields="RID" DataNavigateUrlFormatString="BaseInfo011Edit.aspx?ActionType=Edit&RID={0}">
                                                        <itemstyle horizontalalign="Left" width="15%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Status_Name" HeaderText="類別名稱" />
                                                    <asp:BoundField DataField="Operate" HeaderText="廠商庫存異動檔匯入對庫存增減" />
                                                    <asp:BoundField DataField="Is_UptDepository" HeaderText="是否可修改庫存增減" />
                                                    <asp:BoundField DataField="Is_Display" HeaderText="是否顯示在公式中" />
                                                    <asp:BoundField DataField="Comment" HeaderText="備註" />
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
