<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository016.aspx.cs" Inherits="Depository_Depository016" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>物料庫存異動作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td colspan="2" style="width: 971px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" style="width: 971px">
                        <font class="PageTitle">物料庫存異動作業</font></td>
                </tr>
                <tr>
                    <td colspan="2" style="width: 971px">
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    異動日期：
                                </td>
                                <td width="35%">
                                    <asp:TextBox ID="txtBeginDate" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBeginDate')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtEndDate" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEndDate')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期"
                                        ControlToCompare="txtBeginDate" ControlToValidate="txtEndDate" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td width="10%" align="right">
                                    品名：
                                </td>
                                <td width="45%">
                                    <asp:TextBox ID="txtMaterial" runat="server" size="12" onblur="LimitLengthCheck(this,20);"
                                        MaxLength="20"></asp:TextBox>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    Perso廠：
                                </td>
                                <td width="35%">
                                    <asp:DropDownList ID="dropFactory" runat="server" DataTextField="Factory_ShortName_CN"
                                        DataValueField="RID">
                                    </asp:DropDownList></td>
                                <td width="10%" align="right">
                                    異動項目：
                                </td>
                                <td width="45%">
                                    <asp:DropDownList ID="dropPARAM" runat="server" DataTextField="Param_Name" DataValueField="RID">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="4" align="right">
                                    <asp:Button ID="btnSearch" OnClick="btnSearch_Click" runat="server" Text="查詢" CssClass="btn">
                                    </asp:Button>
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnAdd" OnClick="btnAdd_Click" runat="server" Text="新增" CssClass="btn"
                                        CausesValidation="False"></asp:Button>
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
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tbody>
                                        <tr valign="baseline">
                                            <td>
                                                <cc1:GridViewPageingByDB ID="gvpbTransaction" runat="server" CssClass="GridView" EditPageUrl="#"
                                                    EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl="" ColumnsHeaderText=""
                                                    SortDescImageUrl="~/images/desc.gif" SortAscImageUrl="~/images/asc.gif" OnRowDataBound="gvpbTransaction_RowDataBound"
                                                    PreviousPageText="<" NextPageText=">" LastPageText=">>" FirstPageText="<<" AutoGenerateColumns="False"
                                                    AllowSorting="True" OnOnSetDataSource="gvpbTransaction_OnSetDataSource" DataKeyNames="RID"
                                                    AllowPaging="True">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <Columns>
                                                        <asp:BoundField DataField="Transaction_Date1" HeaderText="異動日期" SortExpression="Transaction_Date">
                                                            <itemstyle horizontalalign="Left" width="10%" />
                                                        </asp:BoundField>
                                                        <asp:HyperLinkField DataNavigateUrlFields="Transaction_ID" DataNavigateUrlFormatString="Depository016Mod.aspx?ActionType=Edit&amp;Transaction_ID={0}"
                                                            DataTextField="Transaction_ID" HeaderText="異動單號" SortExpression="Transaction_ID" />
                                                        <asp:BoundField DataField="Materiel_Name" HeaderText="品名">
                                                            <itemstyle horizontalalign="right" width="10%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Param_Name" HeaderText="異動項目" SortExpression="Param_Name">
                                                            <itemstyle horizontalalign="Left" width="20%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Transaction_Amount" HeaderText="數量" SortExpression="Transaction_Amount"
                                                            DataFormatString="{0:N0}" HtmlEncode="False">
                                                            <itemstyle horizontalalign="right" width="10%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Factory_Name" HeaderText="Perso廠" SortExpression="Factory_Name">
                                                            <itemstyle horizontalalign="Left" width="20%" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </cc1:GridViewPageingByDB>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="False"
            ShowMessageBox="True"></asp:ValidationSummary>
    </form>
</body>
</html>
