<%@ Page Language="C#" AutoEventWireup="true" CodeFile="header.aspx.cs" Inherits="header" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>未命名頁面</title>

    <script language="javascript" type="text/javascript" src="JS/CommJS.js"></script>

</head>
<body bgcolor="#8E9574" marginheight="0" marginwidth="0" rightmargin="0" leftmargin="0"
    bottommargin="0" topmargin="0">
    <form id="form1" runat="server">
        <div>
            <table width="1024px" height="77px" border="0" cellpadding="0" cellspacing="0" >
                <tr >
                    <td>
                        <font size="2" face="Arial">登入名稱:<asp:Label ID="lblUserName" runat="server" Text=""></asp:Label></font>
                    </td>
                    <td align="right">
                        <a target="ViewFrame" href="none.aspx">
                            <img border="0" src="Images/TF_Home.gif"></a><a target="_top" href="Login.aspx"><img
                                border="0" src="Images/TF1.jpg" width="57" height="19">
                            </a>
                    </td>
                </tr>
                <tr style="height: 56px;" valign="middle">
                    <td bgcolor="#006558" align="left" style="height: 56px">
                        <img border="0" height="56px" src="Images/Logo.bmp" hspace="0" vspace="0">
                    </td>
                    <td align="right" style="height: 56px;width:197px; background-color: #00948c;">
                        <img border="0" height="56px" src="Images/LogoB.bmp" hspace="0" vspace="0">
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
