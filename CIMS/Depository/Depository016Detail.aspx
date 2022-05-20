<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository016Detail.aspx.cs"
    Inherits="Depository_Depository016Detail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>物料庫存異動明細</title>
    <meta http-equiv="pragma" content="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px" valign="baseline">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">物料庫存異動明細</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmitUp" runat="server" CssClass="btn" OnClick="btnSubmitDn_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <input id="Button2" class="btn" onclick="window.close();" type="button" value="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table class="tbl_row" cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="15%">
                                    <span class="style1">*</span>品名：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropName" runat="server" DataTextField="Name">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="dropName"
                                        ErrorMessage="品名必須輸入">*</asp:RequiredFieldValidator>
                                    <asp:HiddenField ID="hidActionType" runat="server" />
                                    <asp:HiddenField ID="HidIndex" runat="server" />
                                    <asp:HiddenField ID="hidRID" runat="server" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span>異動項目：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropPARAMRID" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="dropPARAMRID"
                                        ErrorMessage="異動項目必須輸入">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span>數量：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTransactionAmount" runat="server" onfocus="DelDouhao(this)" onblur="CheckNum('txtTransactionAmount',9);value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtTransactionAmount',9)" MaxLength="11" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                            runat="server" ControlToValidate="txtTransactionAmount" ErrorMessage="數量必須輸入">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span>Perso廠：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropFactoryRID" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="dropFactoryRID"
                                        ErrorMessage="Perso廠必須輸入">*</asp:RequiredFieldValidator></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="height: 20px">
                        &nbsp;
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmitDn" runat="server" CssClass="btn" OnClick="btnSubmitDn_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <input id="Button1" class="btn" onclick="window.close();" type="button" value="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
