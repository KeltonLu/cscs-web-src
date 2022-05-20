<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType006.aspx.cs" Inherits="CardType_CardType006" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>製卡類別設定</title>
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
                        <font class="PageTitle">批次設定</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td align="right" width="15%">
                                    卡種群組：</td>
                                <td width="25%">
                                    <asp:DropDownList ID="dropGroup_Name" runat="server" DataTextField="Group_Name" DataValueField="RID">
                                    </asp:DropDownList>
                                </td>
                                <td align="right" width="15%">
                                    製卡類別：</td>
                                <td width="45%">
                                    <asp:TextBox ID="txtType_Name" onkeyup=" LimitLengthCheck(this, 30)" runat="server"
                                        MaxLength="30" />
                                </td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" />
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
                                            <cc1:GridViewPageingByDB ID="gvpbCardType" runat="server" AllowPaging="True" DataKeyNames="RID"
                                                OnOnSetDataSource="gvpbCardType_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Group_Name" HeaderText="卡種群組" SortExpression="Group_Name">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="Type_Name" HeaderText="製卡類別" SortExpression="Type_Name"
                                                        DataNavigateUrlFields="RID" DataNavigateUrlFormatString="CardType006Edit.aspx?ActionType=Edit&RID={0}">
                                                        <itemstyle horizontalalign="Left" width="60%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Is_Report" HeaderText="報表使用">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Is_Import" HeaderText="匯入使用">
                                                        <itemstyle horizontalalign="Left" width="10%" />
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
