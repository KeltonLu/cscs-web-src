<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType0041CardAdd.aspx.cs"
    Inherits="CardType_CardType0041CardAdd" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>代製項目與卡種設定新增 </title>
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
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">代製項目與卡種設定新增 </font>
                    </td>
                    <td align="right">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnAdd" runat="server" CssClass="btn" Text="確定"
                            OnClick="btnAdd_Click" />&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('CardType0041Card.aspx')" type="button"
                            value="取消" />&nbsp;
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td colspan="2">
                      
                                <table width="100%" class="tbl_row" cellpadding="1" cellspacing="0" border="0">
                                    <tr valign="baseline">
                                        <td align="right" style="width: 15%; height: 22px;">
                                            <span class="style1">*</span>Perso廠：
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropFactory_RID" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropFactory_RID_SelectedIndexChanged">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr valign="baseline">
                                        <td align="right" style="width: 15%">
                                            <span class="style1">*</span>代製項目：
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropPersoProject_RID" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr valign="baseline">
                                        <td align="right">
                                            <span style="font-size: 10pt; color: #ff0000">*</span>使用期間：
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtUse_Date_Begin" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                                Width="80px" AutoPostBack="True" OnTextChanged="txtUse_Date_Begin_TextChanged"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_Begin')})" src="../images/calendar.gif" />~<asp:Label
                                                ID="lblUse_Date_End" runat="server" Text="9999/12/31"></asp:Label>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUse_Date_Begin"
                                                ErrorMessage="使用期間不能為空">*</asp:RequiredFieldValidator>
                                    </tr>
                                    <tr valign="baseline">
                                        <td colspan="2">
                                            <table width="100%" class="tbl_row" cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td>
                                                        <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            備註：
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="50px" Width="400px"></asp:TextBox></td>
                                    </tr>
                                </table>
                          
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        <asp:Button ID="btnAdd1" runat="server" CssClass="btn" OnClick="btnAdd_Click" Text="確定" />&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('CardType0041Card.aspx')"
                            type="button" value="取消" />&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
