<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="CardType006Edit.aspx.cs"
    Inherits="CardType_CardType006Edit" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>製卡類別設定修改</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />    
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript">
            function ConfirmDel()
            {
                if(document.getElementById('chkDel').checked)
                {
                    return confirm("是否刪除此筆訊息");
                }else
                {
                    return CheckClientValidate();
                }
            }       
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr>
                    <td colspan="2" style="height: 20px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">批次設定修改/刪除</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEditUp" runat="server" Text="確定" OnClick="btnEditUp_Click" CssClass="btn"
                            OnClientClick="return ConfirmDel();" />&nbsp;&nbsp;
                            <input id="btnCancel" class="btn" onclick="returnform('CardType006.aspx')"
                            type="button" value="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table width="100%" cellspacing="0">
                            <tr class="tbl_row">
                                <td align="right" width="10%">
                                    <span class="style1"></span>卡種群組：</td>
                                <td width="90%">
                                    <asp:DropDownList ID="dropGroup_Name" runat="server" DataTextField="Group_Name" DataValueField="RID"
                                        Enabled="false">
                                    </asp:DropDownList><asp:CompareValidator ID="valcGroup_Name" runat="server" ControlToValidate="dropGroup_Name"
                                        ErrorMessage="卡種群組不能為空" ValueToCompare="-1" Operator="GreaterThan" Type="Integer">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" width="10%">
                                    <span class="style1">*</span>製卡類別：</td>
                                <td width="90%">
                                    <asp:TextBox ID="txtType_Name" onkeyup=" LimitLengthCheck(this, 30)" runat="server"
                                        MaxLength="30" />
                                    <asp:RequiredFieldValidator ID="valrType_Name" runat="server" ControlToValidate="txtType_Name"
                                        ErrorMessage="製卡類別不能為空">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" width="10%">
                                    <span class="style1">*</span>報表使用：</td>
                                <td width="90%">
                                    <asp:RadioButtonList ID="radlIs_Report" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="是" Value="Y"></asp:ListItem>
                                        <asp:ListItem Text="否" Value="N" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" width="10%">
                                    <span class="style1">*</span>匯入使用：</td>
                                <td width="90%">
                                    <asp:RadioButtonList ID="radlIs_Import" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="是" Value="Y"></asp:ListItem>
                                        <asp:ListItem Text="否" Value="N" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr id="trDel" runat="server" class="tbl_row">
                                <td width="10%">
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnEditDown" runat="server" Text="確定" OnClick="btnEditDown_Click"
                            CssClass="btn" OnClientClick="return ConfirmDel();" />&nbsp;&nbsp;
                            <input id="btnCancel1" class="btn" onclick="returnform('CardType006.aspx')"
                            type="button" value="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="valsCardType006Edit" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
