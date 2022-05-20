<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡片庫存管理系統</title>
</head>
<body style="text-align: center" background="Images/LOgin_B_r5_c3.gif" marginheight="0" marginwidth="0" rightmargin="0" leftmargin="0" bottommargin="0" topmargin="0">
    <form id="form1" runat="server">
        <div>
            <table width="100%" height="100%" cellpadding="0" cellspacing="0" border="0">
                <tr height="100px">
                    <td>&nbsp;</td>
                </tr>
                <tr valign="top">
                    <td align="left">
                        <table background="Images/Login_New.gif" width="1008" height="203" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td width="3%">&nbsp;
                                </td>
                                <td width="8%">
                                    <p align="right">
                                        <span><font size="2" color="#ffffff">用戶：</font></span>
                                </td>
                                <td width="10%">
                                    <asp:TextBox ID="txtID" runat="server"></asp:TextBox>
                                </td>
                                <td width="8%">
                                    <p align="right">
                                        <span><font size="2" color="#ffffff">密碼：</font></span>
                                    </p>
                                </td>
                                <td width="15%">
                                    <asp:TextBox TextMode="Password" ID="txtPwd" runat="server"></asp:TextBox>
                                </td>
                                <td width="1%">&nbsp;
                                </td>
                                <td width="10%">
                                    <asp:ImageButton ImageUrl="images/login.gif" ID="ibtnLogin" runat="server" OnClick="ibtnLogin_Click" />
                                </td>
                                <td>&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
