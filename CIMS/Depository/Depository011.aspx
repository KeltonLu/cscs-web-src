<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository011.aspx.cs" Inherits="Depository_Depository011" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>卡片庫存移轉</title>
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
                        <font class="PageTitle">卡片庫存移轉</font></td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="15%" align="right">
                                    移轉日期：
                                </td>
                                <td width="40%">
                                    <asp:TextBox Width="80px" ID="txtBeginDate" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBeginDate')})" src="../images/calendar.gif" />~
                                    <asp:TextBox Width="80px" ID="txtEndDate" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtEndDate')})" src="../images/calendar.gif" />
                                </td>
                                <td align="right">
                                    版面簡稱：</td>
                                <td>
                                    <asp:TextBox ID="txtCardType" MaxLength="30" onblur="LimitLengthCheck(this,30);" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    轉出Perso廠：</td>
                                <td>
                                    <asp:DropDownList ID="dropFromFactory" runat="server" DataTextField="Factory_ShortName_CN" DataValueField="Rid">
                                    </asp:DropDownList></td>
                                <td align="right">
                                    轉入Perso廠：</td>
                                <td>
                                    <asp:DropDownList ID="dropToFactory" runat="server" DataTextField="Factory_ShortName_CN" DataValueField="RID">
                                    </asp:DropDownList>
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
            </td> </tr>
            <tr valign="baseline" height="2px">
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr valign="baseline">
                <td>
                    <table width="100%">
                        <tr valign="baseline">
                            <td>
                                <cc1:GridViewPageingByDB ID="gvpbCardTypeStocksMove" runat="server" AllowPaging="True"
                                    DataKeyNames="CardType_Move_RID" OnOnSetDataSource="gvpbCardTypeStocksMove_OnSetDataSource"
                                    AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>"
                                    NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbCardTypeStocksMove_RowDataBound"
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
                                        <asp:HyperLinkField DataTextField="CardType_Move_RID" HeaderText="移轉單號" SortExpression="CardType_Move_RID"
                                            DataNavigateUrlFields="CardType_Move_RID" DataNavigateUrlFormatString="Depository011Mod.aspx?ActionType=Edit&amp;Move_ID={0}">
                                            <itemstyle horizontalalign="Left" width="10%" />
                                        </asp:HyperLinkField>
                                           <asp:BoundField DataField="Name" HeaderText="卡種" SortExpression="Name">
                                            <itemstyle horizontalalign="Left" width="10%" />
                                        </asp:BoundField>
                                           <asp:BoundField DataField="Move_Number" HeaderText="數量" SortExpression="Move_Number" DataFormatString="{0:N0}" HtmlEncode="False">
                                            <itemstyle horizontalalign="right" width="10%" />
                                        </asp:BoundField>
                                           <asp:BoundField DataField="From_Factory" HeaderText="轉出Perso廠" SortExpression="From_Factory">
                                            <itemstyle horizontalalign="Left" width="20%" />
                                        </asp:BoundField>
                                           <asp:BoundField DataField="To_Factory" HeaderText="轉入Perso廠" SortExpression="To_Factory">
                                            <itemstyle horizontalalign="Left" width="20%" />
                                        </asp:BoundField>
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
