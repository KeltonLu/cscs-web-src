<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo003Edit.aspx.cs"
    Inherits="BaseInfo_BaseInfo003Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>廠商資料編輯</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>

            <script type="text/javascript">
                function ConfirmDel() {
                    if (CheckClientValidate()) {

                        // add by tree 20161021 獲取不到chkDel
                        if (document.getElementById('chkDel') != undefined) {
                            if (document.getElementById('chkDel').checked) {
                                return confirm('是否刪除此筆訊息');
                            }
                        }
                        return true;

                    }
                    else {
                        return false;
                    }
                }
                function returnform() {
                    window.location = "baseinfo003.aspx?con=1";
                }
            </script>

            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">
                            <asp:Label ID="lbTitle" runat="server"></asp:Label>
                        </font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" class="btn" OnClientClick="return ConfirmDel();"
                            OnClick="btnEdit_Click" />
                        &nbsp;
                        <input id="btnCancel" type="button" value="取消" class="btn" onclick="returnform();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" align="center" cellpadding="0" cellspacing="0" border="0">
                            <tr class="tbl_row">
                                <td align="right" style="width: 15%">
                                    <span class="style1">*</span>廠商類別：
                                </td>
                                <td style="width: 20%">
                                    <asp:CheckBox ID="chkIs_Blank" runat="server" Text="空白卡廠" />
                                    <asp:CheckBox ID="chkIs_Perso" runat="server" Text="PERSO廠" /></td>
                                <td align="right">
                                    <asp:Label ID="bllfid" runat="server" Text="廠商代號："></asp:Label>
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:Label ID="lblFactory_ID" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">統一編號：
                                </td>
                                <td style="width: 20%">
                                    <asp:TextBox ID="txtUnit_ID" runat="server" size="20" MaxLength="8" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtUnit_ID"
                                        ErrorMessage="統一編號必須輸入數字和字母的組合!" ValidationExpression="^[A-Za-z0-9]+$">*</asp:RegularExpressionValidator></td>
                                <td align="right">廠商名稱：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtFactory_Name" runat="server" size="20" MaxLength="30" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%;">
                                    <span class="style1">*</span>廠商簡稱：
                                </td>
                                <td style="width: 20%;">
                                    <asp:TextBox ID="txtFactory_ShortName_CN" runat="server" MaxLength="20" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFactory_ShortName_CN"
                                        ErrorMessage="廠商簡稱不能為空">*</asp:RequiredFieldValidator></td>
                                <td align="right">
                                    <span class="style1">*</span>英文簡稱：
                                </td>
                                <td align="left" colspan="2" style="width: 25%;">
                                    <asp:TextBox ID="txtFactory_ShortName_EN" runat="server" MaxLength="10" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtFactory_ShortName_EN"
                                        ErrorMessage="英文簡稱不能為空">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtFactory_ShortName_EN"
                                        ErrorMessage="英文簡稱輸入格式不正確!" ValidationExpression="\w*">*</asp:RegularExpressionValidator>
                                </td>
                                <td></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">負責人：
                                </td>
                                <td style="width: 22%">
                                    <asp:TextBox ID="txtFactory_Principal" runat="server" MaxLength="20" />
                                </td>
                                <td align="right">公司成立日期：
                                </td>
                                <td width="20%" colspan="2">
                                    <asp:TextBox ID="txtCreat_Date" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtCreat_Date')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                </td>
                                <td></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">公司聯絡人：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtCoLinkMan_Name" runat="server" size="20" MaxLength="20" />
                                </td>
                                <td align="right">公司聯絡人手機：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtCoLinkMan_Mobil" runat="server" size="20" MaxLength="20" />&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtCoLinkMan_Mobil"
                                        ErrorMessage="請輸入正確的號碼!" ValidationExpression="(\d|[-_()#])*">*</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">公司聯絡人電話：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtCoLinkMan_Phone" runat="server" size="20" MaxLength="20" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="txtCoLinkMan_Phone"
                                        ErrorMessage="請輸入正確電話號碼!" ValidationExpression="(\d|[-_()#])*">*</asp:RegularExpressionValidator></td>
                                <td align="right">公司聯絡人email：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtCoLinkMan_Email" runat="server" size="20" MaxLength="30" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtCoLinkMan_Email"
                                        ErrorMessage="請輸入正確的公司聯絡人郵箱地址!" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">公司地址：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtCoAddress" runat="server" size="20" MaxLength="50" />
                                </td>
                                <td align="right">公司電話：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtCoPhone" runat="server" size="20" MaxLength="20" />&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtCoPhone"
                                        ErrorMessage="請輸入正確的號碼!" ValidationExpression="(\d|[-_()#])*">*</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 1356327px">公司傳真：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtCoFax" runat="server" onblur="LimitLengthCheck(this,20)" onkeyup="LimitLengthCheck(this,20)" size="20"
                                        MaxLength="20" />
                                </td>
                                <td align="right">工廠聯絡人：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtFacLinkMan_Name" runat="server" size="20" MaxLength="20" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">工廠聯絡人手機：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtFacLinkMan_Mobil" runat="server" size="20" MaxLength="20" />&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ControlToValidate="txtFacLinkMan_Mobil"
                                        ErrorMessage="請輸入正確的號碼！" ValidationExpression="(\d|[-_()#])*">*</asp:RegularExpressionValidator></td>
                                <td align="right">工廠地址：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtFacAddress" runat="server" size="20" MaxLength="50" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">工廠聯絡人電話：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtFacLinkMan_Phone" runat="server" size="20" MaxLength="20" />&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server"
                                        ControlToValidate="txtFacLinkMan_Phone" ErrorMessage="請輸入正確的號碼！" ValidationExpression="(\d|[-_()#])*">*</asp:RegularExpressionValidator></td>
                                <td align="right">工廠聯絡人email：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtFacLinkMan_Email" runat="server" size="20" MaxLength="30" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtFacLinkMan_Email"
                                        ErrorMessage="請輸入正確的工廠聯絡人郵箱地址!" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">工廠電話：
                                </td>
                                <td style="width: 30%">
                                    <asp:TextBox ID="txtFacPhone" runat="server" size="20" MaxLength="20" />&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server"
                                        ControlToValidate="txtFacPhone" ErrorMessage="請輸入正確的號碼！" ValidationExpression="(\d|[-_()#])*">*</asp:RegularExpressionValidator></td>
                                <td align="right">工廠傳真：
                                </td>
                                <td style="width: 25%" colspan="3">
                                    <asp:TextBox ID="txtFacFax" onblur="LimitLengthCheck(this,20)" onkeyup="LimitLengthCheck(this,20)" runat="server" size="20"
                                        MaxLength="20" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">製造國：
                                </td>
                                <td style="width: 23%">
                                    <asp:TextBox ID="txtCountry_Name" runat="server" size="20" MaxLength="20" />
                                </td>
                                <td align="right">
                                    <span style="color: #ff0000">*</span>是否為合作廠商：
                                </td>
                                <td style="width: 26%" colspan="4">
                                    <asp:RadioButtonList ID="radlIs_Cooperate" runat="server" RepeatColumns="2">
                                        <asp:ListItem Selected="True">是</asp:ListItem>
                                        <asp:ListItem>否</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" style="width: 15%">主要產品：
                                </td>
                                <td style="width: 80%" colspan="5">
                                    <asp:TextBox ID="txtProduct_Main" runat="server" TextMode="MultiLine"
                                        Width="460px" />&nbsp;
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" style="width: 15%">備註：
                                </td>
                                <td style="width: 80%" colspan="5">
                                    <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine"
                                        Width="460px" />&nbsp;
                                </td>
                            </tr>
                            <tr id="trDel" runat="server" class="tbl_row">
                                <td></td>
                                <td colspan="5">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td>&nbsp;
                                </td>
                                <td align="right" colspan="5">
                                    <asp:Button ID="btnEditD" Text="確定" class="btn" runat="server" OnClientClick="return ConfirmDel();"
                                        OnClick="btnEditD_Click" />
                                    &nbsp;
                                    <input id="btnCancelD" value="取消" type="button" class="btn" onclick="returnform();" />
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
