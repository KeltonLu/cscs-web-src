<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo002.aspx.cs" Inherits="BaseInfo_BaseInfo002" %>

<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>合約作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1" border="0">
                <tr style="height: 20px">
                    <td>&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">合約作業</font>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                            <tr valign="baseline">
                                <td style="width: 15%;" align="right">合約編號：
                                </td>
                                <td style="width: 20%;" align="left">
                                    <asp:TextBox ID="txtAgreement_Code" onblur="LimitLengthCheck(this,20)" onkeyup="LimitLengthCheck(this,20)" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                                <td style="width: 10%;" align="right">合約名稱：
                                </td>
                                <td align="left" style="width: 20%;">
                                    <asp:TextBox ID="txtAgreement_Name" onblur="LimitLengthCheck(this,30)" onkeyup="LimitLengthCheck(this,30)" runat="server" MaxLength="30"></asp:TextBox>
                                </td>
                                <td style="width: 10%;" align="right">空白卡廠：
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="dropFactory_RID" runat="server" DataTextField="Factory_ShortName_cn" DataValueField="RID">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">卡數：</td>
                                <td align="left">
                                    <%--Dana 20161021 最大長度由9改為11 --%>
                                    <asp:TextBox ID="txtCard_Number1" onkeyup="CheckNum('txtCard_Number1',9)" onfocus="DelDouhao(this)" Style="ime-mode: disabled; text-align: right" onblur="value=GetValue(this.value)" runat="server" MaxLength="11" Width="70px"></asp:TextBox>~
                                <asp:TextBox ID="txtCard_Number2" onkeyup="CheckNum('txtCard_Number2',9)" onfocus="DelDouhao(this)" Style="ime-mode: disabled; text-align: right" onblur="value=GetValue(this.value)" runat="server" MaxLength="11" Width="70px"></asp:TextBox>
                                </td>
                                <td align="right">有效期間：</td>
                                <td colspan="3" align="left">
                                    <asp:TextBox ID="txtBegin_Time" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBegin_Time')})" src="../images/calendar.gif" align="absmiddle">~
                                 <asp:TextBox ID="txtEnd_Time" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEnd_Time')})" src="../images/calendar.gif" align="absmiddle">
                                    &nbsp;
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Time"
                                    ControlToValidate="txtEnd_Time" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                    Type="Date">*</asp:CompareValidator>
                                </td>

                            </tr>
                            <tr valign="baseline">
                                <td colspan="6">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <uc1:urctrlCardTypeSelect ID="UrctrlCardTypeSelect" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" colspan="6">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                                <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click" CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbAgreement" runat="server" AllowPaging="True"
                                                DataKeyNames="RID" OnOnSetDataSource="gvpbAgreement_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbAgreement_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="合約編號" SortExpression="Agreement_Code">
                                                        <ItemStyle Width="20%" />
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hlAgreement_Code" runat="server"></asp:HyperLink><br />
                                                            <asp:Label ID="lbAgreement_Code" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="合約名稱">
                                                        <ItemStyle Width="20%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbAgreement_Name" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="卡數">
                                                        <ItemStyle Width="15%" HorizontalAlign="right" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbCard_Number" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="剩餘卡數">
                                                        <ItemStyle Width="15%" HorizontalAlign="right" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbCard_Number_r" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="有效期間">
                                                        <ItemStyle Width="15%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbTime" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="空白卡廠">
                                                        <ItemStyle Width="15%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbFactory_Name" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
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
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />

        </div>
    </form>
</body>
</html>
