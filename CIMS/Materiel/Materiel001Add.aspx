<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Materiel001Add.aspx.cs" Inherits="Materiel_Materiel001Add" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>信封基本資料新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script type="text/javascript">
    <!--
        function doRadio(rad)
        {
            
            
            if (rad == document.getElementById("radSafe_Type1"))
            {
                document.getElementById("radSafe_Type1").checked = true;
                document.getElementById("radSafe_Type2").checked = false;
                document.getElementById("txtSafe_Number1").disabled = false;
                document.getElementById("txtSafe_Number2").disabled = true;
                document.getElementById("txtSafe_Number2").value = "";
            }
            else
            {
                document.getElementById("radSafe_Type1").checked = false;
                document.getElementById("radSafe_Type2").checked = true;
                document.getElementById("txtSafe_Number1").disabled = true;
                document.getElementById("txtSafe_Number1").value = "";
                document.getElementById("txtSafe_Number2").disabled = false;
            }
        }
        
        function checkRadio()
        {
            if(document.getElementById("radSafe_Type1").checked)
            {
                document.getElementById("radSafe_Type1").checked = true;
                document.getElementById("radSafe_Type2").checked = false;
                document.getElementById("txtSafe_Number1").disabled = false;
                document.getElementById("txtSafe_Number2").disabled = true;
                document.getElementById("txtSafe_Number2").value = "";
            }
            else
            {
                document.getElementById("radSafe_Type1").checked = false;
                document.getElementById("radSafe_Type2").checked = true;
                document.getElementById("txtSafe_Number1").disabled = true;
                document.getElementById("txtSafe_Number1").value = "";
                document.getElementById("txtSafe_Number2").disabled = false;
            }
        }
    //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table cellpadding="0" cellspacing="1" border="0" style="width: 100%">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td class="PageTitle">
                        信封基本資料新增
                    </td>
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit1_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('Materiel001.aspx')" type="button"
                            value="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table class="tbl_row" width="100%" cellpadding="0" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td align="right" width="15%">
                                    <span class="style1">*</span>品名：
                                </td>
                                <td style="width: 595px">
                                    <asp:TextBox ID="txtName" onkeyup="LimitLengthCheck(this, 20)" runat="server" MaxLength="20"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                                        ErrorMessage="品名不能為空">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span>單價：
                                </td>
                                <td style="width: 595px">
                                                        <%--Dana 20161021 最大長度由 7 改為 8 --%>
                                    <asp:TextBox ID="txtUnit_Price" onfocus="DelDouhao(this)" onblur="CheckAmt('txtUnit_Price',4,2);value=GetValue(this.value)"
                                        onkeyup="CheckAmt('txtUnit_Price',4,2)" runat="server" MaxLength="8" Style="ime-mode: disabled;
                                        text-align: right;"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtUnit_Price"
                                        ErrorMessage="單價不能為空">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span>耗損率：
                                </td>
                                <td style="width: 595px">
                                    <asp:TextBox ID="txtWear_Rate" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtWear_Rate',3)" runat="server" MaxLength="3" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>%
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtWear_Rate"
                                        ErrorMessage="耗損率不能為空">*</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtWear_Rate"
                                        ErrorMessage="耗損率不能超過100" Operator="LessThanEqual" ValueToCompare="100" Type="Integer">*</asp:CompareValidator></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span>安全存量：
                                </td>
                                <td style="width: 595px">
                                                        <%--Dana 20161021 最大長度由 6 改為 7 --%>
                                    <asp:RadioButton ID="radSafe_Type1" runat="server" onclick="doRadio(this);" Text="最低安全庫存" />
                                    <asp:TextBox ID="txtSafe_Number1" runat="server" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtSafe_Number1',6)" MaxLength="7" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>&nbsp;
                                    <asp:RadioButton ID="radSafe_Type2" runat="server" onclick="doRadio(this);" Text="安全天數" />
                                    <asp:TextBox ID="txtSafe_Number2" runat="server" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtSafe_Number2',3)" MaxLength="3" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>天
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    帳務類別：
                                </td>
                                <td>
                                    <input id="adrtCard" runat="server" checked="true" name="radioUse_Stop" type="radio"
                                        value="1" />信用卡帳&nbsp; &nbsp;
                                    <input id="adrtBlank" runat="server" name="radioUse_Stop" type="radio" value="2" />銀行帳
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="2" height="20px">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="2">
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <input id="radSafe_Type" value="0" runat="server" type="hidden" />
                        <input id="txtSafe_Number" value="0" runat="server" type="hidden" />
                        <asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClick="btnSubmit1_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('Materiel001.aspx')" type="button"
                            value="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
