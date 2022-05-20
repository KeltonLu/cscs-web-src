<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository015Detail.aspx.cs"
    Inherits="Depository_Depository015Detail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>物料採購訂單明細</title>
    <meta http-equiv="pragma" content="no-cache" />
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <base target="_self" />
</head>
<body bgcolor="#ffffff" class="body">
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px" valign="baseline">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">物料採購訂單明細</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmitUp" runat="server" CssClass="btn" OnClick="btnSubmitDn_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <input id="Button2" class="btn" onclick="window.close();" type="button" value="取消" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table cellpadding="2" cellspacing="0" width="100%" style="background-color: white">
                            <tr class="tbl_row" valign="baseline">
                                <td width="10%" align="right">
                                    品名：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropName" runat="server" Style="position: relative" OnSelectedIndexChanged="dropName_SelectedIndexChanged"
                                        AutoPostBack="True">
                                    </asp:DropDownList></td>
                                <td align="right">
                                    單價：
                                </td>
                                <td>
                                    <asp:Label ID="lblUnitPrice" runat="server" Style="position: relative"></asp:Label>&nbsp;</td>
                                <td align="right">
                                    採購數量：
                                </td>
                                <td>
                                    <asp:Label ID="lblTotalNumber" runat="server" Style="position: relative"></asp:Label></td>
                                <td align="right">
                                    採購金額：
                                </td>
                                <td>
                                    <asp:Label ID="lblTotalPrice" runat="server" Style="position: relative"></asp:Label>&nbsp;</td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="8">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr class="tbl_row">
                                            <td width="10%" align="right">
                                                送貨Perso廠：
                                            </td>
                                            <td style="height: 22px; width: 175px;">
                                                <asp:DropDownList ID="dropFactoryRID1" runat="server" Style="position: relative">
                                                </asp:DropDownList></td>
                                            <td style="height: 22px; width: 175px;">
                                                <asp:DropDownList ID="dropFactoryRID2" runat="server" Style="position: relative">
                                                </asp:DropDownList></td>
                                            <td style="height: 22px; width: 175px;">
                                                <asp:DropDownList ID="dropFactoryRID3" runat="server" Style="position: relative">
                                                </asp:DropDownList></td>
                                            <td style="height: 22px; width: 175px;">
                                                <asp:DropDownList ID="dropFactoryRID4" runat="server" Style="position: relative">
                                                </asp:DropDownList></td>
                                            <td style="height: 22px">
                                                <asp:DropDownList ID="dropFactoryRID5" runat="server" Style="position: relative">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td style="height: 24px" align="right">
                                                送貨數量：
                                            </td>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                            <td style="width: 175px; height: 24px;">
                                                <asp:TextBox ID="txtNumber1" style="ime-mode:disabled;text-align: right" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtNumber1',9)" runat="server" OnTextChanged="txtNumber1_TextChanged"
                                                    AutoPostBack="True" MaxLength="11"></asp:TextBox></td>
                                            <td style="width: 175px; height: 24px;">
                                                <asp:TextBox ID="txtNumber2" style="ime-mode:disabled;text-align: right" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtNumber2',9)" runat="server" OnTextChanged="txtNumber1_TextChanged"
                                                    AutoPostBack="True" MaxLength="11"></asp:TextBox></td>
                                            <td style="width: 175px; height: 24px;">
                                                <asp:TextBox ID="txtNumber3" style="ime-mode:disabled;text-align: right" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtNumber3',9)" runat="server" OnTextChanged="txtNumber1_TextChanged"
                                                    AutoPostBack="True" MaxLength="11"></asp:TextBox></td>
                                            <td style="width: 175px; height: 24px;">
                                                <asp:TextBox ID="txtNumber4" style="ime-mode:disabled;text-align: right" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtNumber4',9)" runat="server" OnTextChanged="txtNumber1_TextChanged"
                                                    AutoPostBack="True" MaxLength="11"></asp:TextBox></td>
                                            <td style="height: 24px">
                                                <asp:TextBox ID="txtNumber5" style="ime-mode:disabled;text-align: right" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtNumber5',9)" runat="server" OnTextChanged="txtNumber1_TextChanged"
                                                    AutoPostBack="True" MaxLength="11"></asp:TextBox></td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td align="right">
                                                交貨日：
                                            </td>
                                            <td style="width: 175px">
                                                <asp:TextBox ID="txtDelivery_Date1" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox><img
                                                    align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDelivery_Date1')})" src="../images/calendar.gif" /></td>
                                            <td style="width: 175px">
                                                <asp:TextBox ID="txtDelivery_Date2" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox><img
                                                    align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDelivery_Date2')})" src="../images/calendar.gif" /></td>
                                            <td style="width: 175px">
                                                <asp:TextBox ID="txtDelivery_Date3" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox><img
                                                    align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDelivery_Date3')})" src="../images/calendar.gif" /></td>
                                            <td style="width: 175px">
                                                <asp:TextBox ID="txtDelivery_Date4" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox><img
                                                    align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDelivery_Date4')})" src="../images/calendar.gif" /></td>
                                            <td>
                                                <asp:TextBox ID="txtDelivery_Date5" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox><img
                                                    align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDelivery_Date5')})" src="../images/calendar.gif" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    結案日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCase_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox><img
                                        align="absMiddle" onclick="WdatePicker({el:$dp.$('txtCase_Date')})" src="../images/calendar.gif" />
                                </td>
                                <td colspan="6">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    備註：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtComment" Width="400" Height="100" runat="server" Style="position: relative" TextMode="MultiLine"></asp:TextBox></td>
                                <td colspan="6">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnSubmitDn" OnClick="btnSubmitDn_Click" runat="server" Text="確定"
                            CssClass="btn"></asp:Button>
                        &nbsp;&nbsp;
                        <input id="Button1" class="btn" onclick="window.close();" type="button" value="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hidPurchaseDate" runat="server" />
        <asp:HiddenField ID="hidActionType" runat="server" />
        <asp:HiddenField ID="HidIndex" runat="server" />
        <br />
    </form>
</body>
</html>
