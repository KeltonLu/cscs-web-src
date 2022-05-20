<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType005AddBase.aspx.cs"
    Inherits="CardType_CardType005AddBase" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Perso廠與卡種基本設定新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">Perso廠與卡種基本設定新增</font>
                        <td align="right">
                            &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                                Text="確定" />&nbsp;
                            <asp:Button ID="btnCancel1" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                                Text="取消" />&nbsp;
                        </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table id="errorMessage" style="display: none">
                            <tr>
                                <td>
                                    <font class="error_message">資料保存成功！</font>
                                </td>
                            </tr>
                        </table>
                        <table class="tbl_row" width="100%" border="0" cellpadding="0" cellspacing="2">
                            <tr class="tbl_row" valign="baseline">
                                <td width="15%" align="right">
                                    <span class="style1">*</span>Perso廠：</td>
                                <td>
                                    <asp:DropDownList ID="dropFactory" runat="server" Width="88px">
                                    </asp:DropDownList></td>
                                <td width="15%" align="right">
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="left" colspan="4" rowspan="3">
                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="uctrlCARDNAME" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                            </tr>
                            <tr class="tbl_row">
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit2" runat="server"
                            class="btn" OnClick="btnSubmit1_Click" Text="確定" />&nbsp;
                        <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
