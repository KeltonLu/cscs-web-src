<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository011_1.aspx.cs"
    Inherits="Depository_Depository011_1" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>物料庫存移轉作業</title>
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
                        <font class="PageTitle">物料庫存移轉作業</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="1" border="0">
                            <tr valign="baseline">
                                <td width="15%" align="right">
                                    移轉日期：
                                </td>
                                <td width="40%">
                                    <asp:TextBox ID="txtBeginDate" Width="80px" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBeginDate')})" src="../images/calendar.gif" />~
                                    <asp:TextBox ID="txtEndDate" Width="80px" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtEndDate')})" src="../images/calendar.gif" />
                                </td>
                                <td align="right">
                                    品名：</td>
                                <td>
                                    <asp:TextBox ID="txtMaterial" runat="server" onblur="LimitLengthCheck(this,30);"  MaxLength="30"></asp:TextBox>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    轉出Perso廠：</td>
                                <td>
                                    <asp:DropDownList ID="dropFromFactory" runat="server" DataTextField="Factory_ShortName_CN"
                                        DataValueField="Rid">
                                    </asp:DropDownList></td>
                                <td align="right">
                                    轉入Perso廠：</td>
                                <td>
                                    <asp:DropDownList ID="dropToFactory" runat="server" DataTextField="Factory_ShortName_CN"
                                        DataValueField="RID">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="4">
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Text="查詢" />
                                    &nbsp;<asp:Button ID="btnAdd" runat="server" CausesValidation="False" CssClass="btn"
                                        OnClick="btnAdd_Click" Text="新增" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="baseline">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbMaterielStocksMove" runat="server" AllowPaging="True"
                                                DataKeyNames="Move_ID" OnOnSetDataSource="gvpbMaterielStocksMove_OnSetDataSource"
                                                AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>"
                                                NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbMaterielStocksMove_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Move_Date1" HeaderText="移轉日期" SortExpression="Move_Date">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataNavigateUrlFields="Move_ID" DataNavigateUrlFormatString="Depository011_1Mod.aspx?ActionType=Edit&amp;Move_ID={0}"
                                                        DataTextField="Move_ID" HeaderText="移轉單號" SortExpression="Move_ID" />
                                                    <asp:BoundField DataField="Materiel_Name" HeaderText="品名" >
                                                        <itemstyle horizontalalign="right" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Move_Number" HeaderText="數量" SortExpression="Move_Number"
                                                        DataFormatString="{0:N0}" HtmlEncode="False">
                                                        <itemstyle horizontalalign="right" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="From_Factory_Name" HeaderText="轉出Perso廠" SortExpression="From_Factory_Name">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="To_Factory_Name" HeaderText="轉入Perso廠" SortExpression="To_Factory_Name">
                                                        <itemstyle horizontalalign="Left" width="20%" />
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
