<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType005ModBase.aspx.cs"
    Inherits="CardType_CardType005ModBase" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Perso廠與卡種基本設定修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function delConfirm()
        {
            if (true == document.getElementById("chkDel").checked)
            {
                return confirm('確認刪除此筆訊息?');
            }
        }        
 //-->
</script>

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
                        <font class="PageTitle">Perso廠與卡種基本設定修改/刪除</font>
                        <td align="right">
                            &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                                Text="確定" OnClientClick="return delConfirm();" />&nbsp;
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
                        <table class="tbl_row" width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr class="tbl_row" valign="baseline">
                                <td width="10%" align="right">
                                </td>
                                <td>
                                    Perso廠：<asp:Label ID="lbFactory_RID" runat="server"></asp:Label></td>
                                <td width="15%" align="right">
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td colspan="4" rowspan="3">
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="uctrlCARDNAME" runat="server"></uc1:uctrlCardType>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                            </tr>
                            <tr class="tbl_row">
                            </tr>
                            <tr>
                                <td>
                                    <div align="right">
                                        &nbsp;</div>
                                </td>
                                <td>
                                    &nbsp;<asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                                <td>
                                    &nbsp;</td>
                                <td>
                                    &nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2" style="height: 24px">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" OnClientClick="return delConfirm();" />&nbsp;
                        <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
