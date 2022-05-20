<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo050.aspx.cs" Inherits="BaseInfo_BaseInfo050" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>帳號解鎖功能</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr class="PageTitle">
                    <td class="PageTitle">帳號解鎖功能
                    </td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td style="width: 15%" align="right">解鎖帳號： 
                    </td>
                    <td align="left" style="width: 10%">
                        <asp:TextBox ID="txtUsersID" runat="server" MaxLength="64"></asp:TextBox>
                    </td>
                    <td style="padding-left: 25px">
                        <asp:Button CssClass="btn" runat="server" ID="btnDeblocking" Text="解鎖" OnClick="btnDeblocking_Click" />
                    </td>
                </tr>

            </table>
        </div>
    </form>
</body>
</html>
