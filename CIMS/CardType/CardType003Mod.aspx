<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType003Mod.aspx.cs" Inherits="CardType_CardType003Mod" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>版面送審作業修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function delConfirm()
        {
            if(CheckClientValidate())
            {
             if (true == document.getElementById("chkDel").checked)
                {
                    return confirm('確認刪除此筆訊息?');
                }
            }
        }        
 -->
</script>

<body bgcolor="#ffffff" class="body">
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">版面送審作業修改/刪除</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit1" runat="server" class="btn" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                        <asp:Button ID="btnCancel1" runat="server" class="btn" Text="取消" OnClick="btnCancel_Click"
                            CausesValidation="False" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table id="errorMessage" style="display: none">
                            <tr>
                                <td>
                                    <font class="error_message">資料儲存失敗！</font>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td colspan="2">
                        <table width="100%" cellspacing="3">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">
                                    <span class="style1"></span>卡種：
                                </td>
                                <td width="85%">
                                    <asp:Label ID="lbCardType" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                <td align="right">
                                    <span class="style1"></span><span style="color: #ff0000">&nbsp;*</span>批號：</td>
                                <td style="height: 26px">
                                    &nbsp;<asp:TextBox onkeyup=" LimitLengthCheck(this, 30)" ID="txtSerial_Number" runat="server"
                                        MaxLength="30" Width="210px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSerial_Number"
                                        ErrorMessage="批號不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    送審日期（起）：</td>
                                <td>
                                    &nbsp;<asp:TextBox ID="txtBegin_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox><img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})"
                                            src="../images/calendar.gif" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="height: 24px;">
                                    <span class="style1">*</span>送審狀態：</td>
                                <td style="height: 24px">
                                    &nbsp;<asp:DropDownList ID="dropSendCheck_Status" runat="server" DataTextField="Param_Name"
                                        DataValueField="Param_Code">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    完成日期（迄）：</td>
                                <td>
                                    &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox><img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})"
                                            src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="height: 26px;">
                                    驗證編號：</td>
                                <td style="height: 26px">
                                    &nbsp;<asp:TextBox onkeyup=" LimitLengthCheck(this, 30)" ID="txtValidate_Number"
                                        runat="server" Width="100px" MaxLength="15"></asp:TextBox></td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" style="width: 107px">
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnSubmit2" runat="server" class="btn" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" />
                        &nbsp;&nbsp;<asp:Button ID="btnCancel2" runat="server" class="btn" Text="取消" OnClick="btnCancel_Click"
                            CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        <asp:HiddenField ID="HidCardType_RID" runat="server" />
        &nbsp;
    </form>
</body>
</html>
