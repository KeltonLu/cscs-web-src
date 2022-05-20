<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository014Detail.aspx.cs" Inherits="Depository_Depository014Detail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>未命名頁面</title>
     <META HTTP-EQUIV="pragma" CONTENT="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
     <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
    <div id="div001" runat="server">    
    
    </div>
    <div>
    <table width="100%">
    <tr>
    <td colspan="2" align="right">
    <asp:Button id="btnCompute" Text="計算" runat="server" OnClick="btnCompute_Click"  CssClass="btn" />&nbsp;&nbsp;
    <asp:Button ID="btnSubmit" Text="確定" runat="server"  OnClick="btnSubmit_Click"  CssClass="btn"/>&nbsp;&nbsp;
    <input id="btnCancel" class="btn" type="button" value="取消" onclick="window.close();" />
    </td>
    </tr>
         <tr valign="baseline" class="tbl_row">
                <td>
                    A：預估前日庫存數</td>
                <td>
                    F：已下單預計當日到貨數</td>
            </tr>
            <tr valign="baseline" class="tbl_row">
                <td>
                    A'：預估前日庫存數（調整後）</td>
                <td>
                    G：預估當日庫存數</td>
            </tr>
            <tr valign="baseline" class="tbl_row">
                <td>
                    B：預估每日新進件</td>
                <td>
                    G'：預估當日庫存數（調整後，暫存）</td>
            </tr>
            <tr valign="baseline" class="tbl_row">
                <td>
                    C：預估每日新件調整</td>
                <td>
                    H：檢核欄位</td>
            </tr>
            <tr valign="baseline" class="tbl_row">
                <td>
                    D1：預估每日掛補量</td>
                <td>
                    J：系統建議or人員調整採購到貨量</td>
            </tr>
            <tr valign="baseline" class="tbl_row">
                <td>
                    D2：預估每日毀補量</td>
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
