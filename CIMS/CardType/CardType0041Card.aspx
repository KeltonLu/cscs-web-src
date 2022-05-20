<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType0041Card.aspx.cs"
    Inherits="CardType_CardType0041Card" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>代製項目與卡種設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline" height="20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">代製項目與卡種設定</font>
                    </td>
                </tr>
                <tr valign="top" class="tbl_row">
                    <td>
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                            <tr valign="baseline">
                                <td align="right" style="width: 15%">
                                    Perso廠：</td>
                                <td style="width: 15%" align="left">
                                    <asp:DropDownList ID="dropFactory_RID" runat="server">
                                    </asp:DropDownList></td>
                                <td align="right" style="width: 15%">
                                    代製項目：
                                </td>
                                <td align="left" style="width: 15%">
                                    <asp:TextBox ID="txtProject_Name" runat="server" MaxLength="20" onkeyup="LimitLengthCheck(this, 30)"></asp:TextBox>
                                </td>
                                <td align="right" style="width: 15%">
                                    使用期間：</td>
                                <td align="left">
                                    <asp:TextBox ID="txtUse_Date_Begin" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_Begin')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtUse_Date_End" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_End')})" src="../images/calendar.gif" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6">
                               
                                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td>
                                                        <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                      
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" colspan="6">
                                    &nbsp;<asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />
                                    &nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="top">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbPersoProject" runat="server" AllowPaging="True"
                                                DataKeyNames="RID" OnOnSetDataSource="gvpbPersoProject_OnSetDataSource" AllowSorting="True"
                                                AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbPersoProject_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <Columns>
                                                    <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠" SortExpression="Factory_ShortName_CN">
                                                        <itemstyle horizontalalign="Left" width="25%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Project_Code" HeaderText="代製項目編號" SortExpression="Project_Code">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="Project_Name" HeaderText="代製項目" DataNavigateUrlFields="RID"
                                                       DataNavigateUrlFormatString="CardType0041CardMod.aspx?ActionType=Edit&amp;RID={0}">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Use_Date_Begin" HeaderText="使用期間 起">
                                                        <itemstyle horizontalalign="Right" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Use_Date_End" HeaderText="使用期間 迄">
                                                        <itemstyle horizontalalign="Right" width="10%" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
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
