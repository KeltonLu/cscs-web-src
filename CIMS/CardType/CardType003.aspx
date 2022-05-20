<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType003.aspx.cs" Inherits="CardType_CardType003" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc2" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>版面送審作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body bgcolor="#ffffff" class="body">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">版面送審作業</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td colspan="6">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table width="100%" cellpadding="0" cellspacing="2">
                                                <tr>
                                                    <td colspan="6">
                                                        <uc2:urctrlCardTypeSelect ID="UrctrlCardTypeSelect1" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr valign="baseline">
                                                    <td style="width: 15%;" align="right">送審日期：</td>
                                                    <td style="width: 35%;" align="left">
                                                        <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                                            Width="80px"></asp:TextBox>
                                                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                                        （起）~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                                            Width="80px"></asp:TextBox>
                                                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                                        （迄）<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                                            ControlToValidate="txtFinish_Date" ErrorMessage="送審日期迄必須大於起" Operator="GreaterThanEqual"
                                                            Type="Date">&nbsp;</asp:CompareValidator>
                                                    </td>
                                                    <td align="right" style="width: 5%;">批號：</td>
                                                    <td align="left" style="width: 15%;">
                                                        <asp:TextBox onkeyup=" LimitLengthCheck(this, 25)" ID="txtSerial_Number" runat="server"
                                                            MaxLength="25"></asp:TextBox></td>
                                                    <td align="right" style="width: 7%;">送審狀態：</td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="dropSendCheck_Status" runat="server" DataTextField="Param_Name"
                                                            DataValueField="Param_Code">
                                                        </asp:DropDownList></td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" align="right">
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查詢" OnClick="btnSearch_Click" />
                                    &nbsp;
                                    <asp:Button ID="btnAdd" runat="server" CssClass="btn" Text="新增" OnClick="btnAdd_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td valign="top">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="baseline">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbAudit" runat="server" AllowPaging="True" AllowSorting="True"
                                                AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView" DataKeyNames="RID"
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" OnRowDataBound="gvpbAudit_RowDataBound"
                                                OnOnSetDataSource="gvpbAudit_OnSetDataSource">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" Visible="False" />
                                                <Columns>
                                                    <asp:HyperLinkField DataNavigateUrlFormatString="CardType003Mod.aspx?ActionType=Edit&amp;RID={0}"
                                                        DataTextField="Name" HeaderText="卡種" DataNavigateUrlFields="RID" SortExpression="Name" />
                                                    <asp:BoundField DataField="Serial_Number" HeaderText="批號" SortExpression="Serial_Number" />
                                                    <asp:BoundField DataField="Begin_Date" HeaderText="送審日期" SortExpression="Begin_Date" />
                                                    <asp:BoundField DataField="SendCheck_Status" HeaderText="送審狀態" SortExpression="SendCheck_Status" />
                                                    <asp:BoundField DataField="Finish_Date" HeaderText="完成日期" SortExpression="Finish_Date" />
                                                    <asp:BoundField DataField="Validate_Number" HeaderText="驗證編號" SortExpression="Validate_Number" />
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
