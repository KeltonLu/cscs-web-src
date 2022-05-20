<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType002Img.aspx.cs" Inherits="CardType_CardType002Img" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>圖片預覽</title>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="1" style="width: 100%">
                <tr valign="baseline" style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">圖片預覽</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table border="0" cellpadding="0" cellspacing="2" class="tbl_row" width="100%">
                            <tr valign="baseline">
                                <td align="center" style="width: 100%">
                                    <asp:Image ID="imgFileUrl" ImageUrl="../images/NoPic.jpg" runat="server"/>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="center">
                                    <input id="btnSubmit" class="btn" onclick="window.close();" type="button" value="確定" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
