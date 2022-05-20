<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository013Detail.aspx.cs"
    Inherits="Depository_Depository013Detail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡片下單採購明細表</title>
    <meta http-equiv="pragma" content="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
        <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1">
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">卡片下單採購明細表</font>
                </td>
                <td align="right">
                    <asp:Button ID="btnCompute1" Text="計算" runat="server" OnClick="btnCompute_Click"
                        CssClass="btn" />&nbsp;&nbsp;
                    <asp:Button ID="btnSubmit2" Text="確定" runat="server" OnClick="btnSubmit_Click" CssClass="btn" />&nbsp;&nbsp;
                    <input id="btnCancel3" class="btn" type="button" value="取消" onclick="window.close();" />
                </td>
            </tr>
        </table>
        <div id="div001" runat="server">
        </div>
        <div>
            <table width="100%">
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnCompute" Text="計算" runat="server" OnClick="btnCompute_Click" CssClass="btn" />&nbsp;&nbsp;
                        <asp:Button ID="btnSubmit" Text="確定" runat="server" OnClick="btnSubmit_Click" CssClass="btn" />&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" type="button" value="取消" onclick="window.close();" />
                    </td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        A：預估上月底庫存數</td>
                    <td>
                        F：已下單預計本月到貨數</td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        A'：預估上月底庫存數（調整後）</td>
                    <td>
                        G：預估本月底庫存數</td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        B：預估每月新進件</td>
                    <td>
                        G'：預估本月底庫存數（調整後，暫存）</td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        C：預估每月新件調整</td>
                    <td>
                        H：檢核欄位</td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        D1：預估每月掛補量</td>
                    <td>
                        J：系統建議or人員調整採購到貨量</td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        D2：預估每月毀補量</td>
                    <td>
                        K：調整後預估庫存=G'+J</td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td>
                        E：預估該月換卡耗用量</td>
                    <td>
                        L：調整後檢核欄位</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
