<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0021Edit.aspx.cs"
    Inherits="Finance_Finance0021Edit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款修改</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="0"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">請款修改</font>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table id="queryResult1" width="100%">
                            <tr class="tbl_list_row_1">
                                <td colspan="8">
                                    <asp:GridView ID="gvSAP" runat="server" CssClass="GridView" OnRowDataBound="gvSAP_RowDataBound"
                                        Width="100%">
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
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt1"
                                                        ErrorMessage="SAP單號不能為空">*</asp:RequiredFieldValidator>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="請款日">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt2" runat="server" onfocus="WdatePicker()"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator21" runat="server" ControlToValidate="txt2"
                                                        ErrorMessage="请款日不能为空">*</asp:RequiredFieldValidator>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="出帳日">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt3" runat="server" onfocus="WdatePicker()"></asp:TextBox>
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
                                <td align="right" colspan="14">
                                    &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSumbit" runat="server" CssClass="btn" OnClick="btnSumbit_Click"
                                        Text="確定" />
                                    &nbsp;
                                    <asp:Button ID="btnCanel" runat="server" CausesValidation="False" CssClass="btn"
                                        OnClick="btnCanel_Click" Text="取消" />&nbsp; &nbsp;</td>
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
