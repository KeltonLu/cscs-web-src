<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SysInfo002Mod.aspx.cs" Inherits="SysInfo_SysInfo002Mod" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>警訊功能 </title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                            <tr>
                            <td class="PageTitle">
                                警訊功能修改
                            </td>
                            <td align="right">
                                <asp:Button CssClass="btn" ID="btnEdit1" runat="server" Text="確定" OnClick="btnEdit1_Click" />&nbsp;&nbsp;
                                <input class="btn" onclick="returnform('SysInfo002.aspx')" id="Button2" type="button"
                                    value="取消" />
                            </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td style="width: 15%" align="right">
                                    作業名稱：
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblItem_Name" runat="server" ></asp:Label></td>
                            </tr>
                            <tr valign="baseline" align="right">
                                <td>
                                    警訊條件：
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblCondition" runat="server" ></asp:Label></td>
                            </tr>
                            <tr valign="baseline" align="right">
                                <td>
                                    警訊信息：
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblWarning_Content" runat="server" ></asp:Label></td>
                            </tr>
                            <tr valign="baseline" align="right">
                                <td>
                                    類別：
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkSystem_Show" runat="server" Text="系統" />
                                    <asp:CheckBox ID="chkMail_Show" runat="server" Text="Mail" /></td>
                            </tr>
                            <tr align="right">
                                <td>
                                    接收人員：
                                </td>
                                <td align="left">
                                    <cc1:MoveListBox ID="mlbUser" runat="server" Width="137px" Height="180px" DataTextField="UserName"
                                        DataValueField="UserID">
                                    </cc1:MoveListBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnEdit2" runat="server" Text="確定" OnClick="btnEdit1_Click"  />&nbsp;&nbsp;
                        <input class="btn" onclick="returnform('SysInfo002.aspx')" id="Button3" type="button"
                            value="取消" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
