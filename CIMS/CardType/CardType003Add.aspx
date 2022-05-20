<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType003Add.aspx.cs" Inherits="CardType_CardType003Add" %>

<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>版面送審作業新增</title>
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
                <tr style="height: 20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">版面送審作業新增</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit1" runat="server" class="btn" Text="確定" OnClick="btnSubmit1_Click" />&nbsp;&nbsp;
                        <asp:Button ID="btnCancel1" runat="server" class="btn" Text="取消" CausesValidation="False"
                            OnClick="btnCancel1_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table name="errorMessage" id="errorMessage" style="display: none">
                            <tr>
                                <td>
                                    <font class="error_message">資料儲存成功！</font>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%" cellspacing="3">
                                    <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                        <td colspan="2">
                                            <uc1:urctrlCardTypeSelect ID="UrctrlCardTypeSelect1" runat="server" />
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                        <td align="right" style="width: 15%;">
                                            <span class="style1"><span class="style1"></span></span><span style="color: #ff0000">
                                                *</span>批號：</td>
                                        <td>
                                            <asp:TextBox onkeyup=" LimitLengthCheck(this, 30)" ID="txtSerial_Number" runat="server"
                                                MaxLength="30" Width="210px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSerial_Number"
                                                ErrorMessage="批號不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            送審日期（起）：</td>
                                        <td>
                                            <asp:TextBox ID="txtBegin_Date" onfocus="WdatePicker()" runat="server" MaxLength="10"
                                                Width="80px"></asp:TextBox>&nbsp;
                                            <img onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif"
                                                align="absmiddle">
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                        <td align="right">
                                            <span style="color: #ff0000">*</span>送審狀態：</td>
                                        <td>
                                            <asp:DropDownList ID="dropSendCheck_Status" runat="server" DataTextField="Param_Name"
                                                DataValueField="Param_Code">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            完成日期（迄）：</td>
                                        <td>
                                            <asp:TextBox ID="txtFinish_Date" onfocus="WdatePicker()" runat="server" MaxLength="10"
                                                Width="80px"></asp:TextBox>
                                            <img onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif"
                                                align="absmiddle">
                                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                                ControlToValidate="txtFinish_Date" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                                Type="Date">*</asp:CompareValidator></td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            驗證編號：</td>
                                        <td>
                                            <asp:TextBox onkeyup=" LimitLengthCheck(this, 30)" ID="txtValidate_Number" runat="server"
                                                Width="100px" MaxLength="30"></asp:TextBox></td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnSubmit2" runat="server" class="btn" Text="確定" OnClick="btnSubmit1_Click" />&nbsp;&nbsp;
                        <asp:Button ID="btnCancel2" runat="server" class="btn" Text="取消" CausesValidation="False"
                            OnClick="btnCancel1_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
