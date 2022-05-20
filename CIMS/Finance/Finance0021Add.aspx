<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0021Add.aspx.cs" Inherits="Finance_Finance0021Add" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                            <td class="PageTitle">
                                請款新增
                            </td>
                        </table>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr>
                                <td style="width: 15%" align="right">
                                    Perso廠：
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="style1">*</span>卡片耗用日期：
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDateFrom" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtDateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">（起）~ &nbsp;<asp:TextBox ID="txtDateTo"
                                                onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img
                                                    onclick="WdatePicker({el:$dp.$('txtDateTo')})" src="../images/calendar.gif" align="absmiddle">（迄）
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="卡片耗用日期（起）不能為空"
                                        ControlToValidate="txtDateFrom">&nbsp;</asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="卡片耗用日期（迄）不能為空"
                                        ControlToValidate="txtDateTo">&nbsp;</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtDateFrom"
                                        ControlToValidate="txtDateTo" ErrorMessage="卡片耗用日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                    <asp:Button ID="btnCal" runat="server" CssClass="btn" Height="25px" Text="計算出帳金額"
                                        OnClick="btnCal_Click" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbmsg" runat="server" Text="查無資料" Visible="false" ForeColor="red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView Width="100%" ID="gvSAP" runat="server" CssClass="GridView" OnRowDataBound="gvSAP_RowDataBound">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <RowStyle CssClass="GridViewRow" />
                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" Visible="False" />
                            <SelectedRowStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateField HeaderText="SAP單號">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txt1" MaxLength="15" runat="server" onblur="LimitLengthCheck(this,15);" style="ime-mode:disabled;" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox>
                                        
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="請款日">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txt2" onfocus="WdatePicker()" runat="server"></asp:TextBox>
                                      
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="出帳日">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txt3" onfocus="WdatePicker()" runat="server"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="發票號碼">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txt4" MaxLength="12" runat="server"></asp:TextBox>
                                      
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnSumbit" CssClass="btn" runat="server" Text="確定" OnClick="btnSumbit_Click" />&nbsp;&nbsp;
                        <asp:Button ID="btnCanel" CssClass="btn" runat="server" Text="取消" OnClick="btnCanel_Click"
                            CausesValidation="False" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
