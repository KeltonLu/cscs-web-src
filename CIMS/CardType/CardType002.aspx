<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType002.aspx.cs" Inherits="CardType_CardType002" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡種基本資料查询</title>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="1" style="width: 100%">
                <tr valign="baseline" style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">卡種基本資料維護作業</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table border="0" cellpadding="0" cellspacing="2" class="tbl_row" width="100%">
                            <tr valign="baseline">
                                <td align="right" style="width: 10%">
                                    製卡群組：
                                </td>
                                <td align="left" style="width: 27%">
                                    <asp:DropDownList ID="dropCardType_Group_RID" runat="server" DataTextField="Group_Name"
                                        DataValueField="RID">
                                    </asp:DropDownList></td>
                                <td align="right" style="width: 8%">
                                    TYPE：</td>
                                <td align="left" style="width: 5%">
                                    <asp:TextBox ID="txtTYPE" runat="server" onkeyup="CheckNum('txtTYPE',3)" MaxLength="3"></asp:TextBox></td>
                                <td align="right" style="width: 10%">
                                    版面簡稱：</td>
                                <td align="left" style="width: 20%">
                                    <asp:TextBox ID="txtName" runat="server" onkeyup="LimitLengthCheck(this, 30)" MaxLength="30"></asp:TextBox>
                                </td>
                                <td align="right" style="width: 10%">
                                    使用狀態：
                                </td>
                                <td align="Left" style="width: 10%">
                                    <asp:DropDownList ID="dropUseType" runat="server">
                                        <asp:ListItem Value="">全部</asp:ListItem>
                                        <asp:ListItem Value="N">停用</asp:ListItem>
                                        <asp:ListItem Value="Y">使用</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" colspan="8">
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Text="查詢" Height="25px" />
                                    &nbsp;
                                    <asp:Button ID="btnAdd" runat="server" CausesValidation="False" CssClass="btn" OnClick="btnAdd_Click"
                                        Text="新增" Height="25px" />
                                    &nbsp;
                                    <asp:Button ID="btnExportExcel" runat="server" CausesValidation="False" CssClass="btn"
                                        Text="匯出Excel格式" Height="25px" OnClick="btnExportExcel_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline" height="2px">
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
                                            <cc1:GridViewPageingByDB ID="gvpbCardType" runat="server" AllowPaging="True" AllowSorting="True"
                                                AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView" DataKeyNames="RID"
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvpbCardType_OnSetDataSource"
                                                PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" OnRowDataBound="gvpbCardType_RowDataBound">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" Visible="False" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:HyperLinkField DataNavigateUrlFields="RID" DataNavigateUrlFormatString="CardType002Mod.aspx?ActionType=Edit&amp;RID={0}"
                                                        DataTextField="CardID" HeaderText="卡片編號  " SortExpression="CardID">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:HyperLinkField>
                                                     <asp:TemplateField HeaderText="製卡群組" >
                                                        <itemstyle horizontalalign="Left" width="24%" />
                                                        <itemtemplate>
                                                            <asp:Label id="lbGroup_Name" runat="server"></asp:Label> 
                                                        </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Name" HeaderText="版面簡稱">
                                                        <itemstyle horizontalalign="Left" width="23%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="BIN" HeaderText="BIN">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="CardExponentName" HeaderText="寄卡單">
                                                        <itemstyle horizontalalign="Center" width="23%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Begin_Time" HeaderText="起始日期" DataFormatString="{0:yyyy\/MM\/dd}"
                                                        HtmlEncode="False">
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
