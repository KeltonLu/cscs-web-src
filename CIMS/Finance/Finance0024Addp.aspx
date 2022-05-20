<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0024Addp.aspx.cs"
    Inherits="Finance_Finance0024Addp" %>
    
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>例外代製項目</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 5px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">例外代製項目</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" type="button" value="取消" class="btn" onclick="window.close();" />
                    </td>
                </tr>
                <tr style="width: 100%">
                    <td colspan="2" style="width: 100%">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table class="tbl_row" cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr class="tbl_row" style="width: 100%">
                                        <td align="right" style="width: 50%">
                                            <font color="red">*</font>日期：
                                        </td>
                                        <td style="width: 50%">
                                            <asp:TextBox ID="txtProject_Date" onfocus="WdatePicker()" runat="server" Width="80px"
                                                MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtProject_Date')})"
                                                    src="../images/calendar.gif" align="absmiddle" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" >
                                            <font color="red">*</font>用途：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" OnSelectedIndexChanged="dropCard_Purpose_RID_SelectedIndexChanged"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" >
                                            <font color="red">*</font>群組：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropCard_Group_RID" runat="server" >
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" >
                                            <font color="red">*</font>Perso廠：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropFactory" runat="server" ></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" >
                                            <font color="red">*</font>例外代製項目：</td>
                                        <td align="left">
                                            <asp:TextBox ID="txtName" runat="server" MaxLength="30"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right" >
                                            <font color="red">*</font>數量：
                                        </td>
                                        <td>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                            <asp:TextBox ID="txtNumber" onfocus="DelDouhao(this)" Style="ime-mode: disabled;
                                                text-align: right" onblur="CheckNum('txtNumber',9);value=GetValue(this.value)" onkeyup="CheckNum('txtNumber',9)"
                                                runat="server" MaxLength="11" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumber"
                                                ErrorMessage="數量不能為空！">*</asp:RequiredFieldValidator></td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right" >
                                            <font color="red">*</font>單價：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBase_price" runat="server" Style="ime-mode: disabled; text-align: right"
                                                size="10" onfocus="DelDouhao(this)" onblur="CheckAmt('txtBase_price',4,2);value=GetValue(this.value)" onkeyup="CheckAmt('txtBase_price',4,2)"/>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="middle">
                                        <td align="right" >
                                            備註：
                                        </td>
                                        <td>
                                            <input id="HidRID" type="hidden" runat="server" />
                                            <asp:TextBox ID="txtComment" runat="server" MaxLength="30" onkeyup=" LimitLengthCheck(this, 30)" Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEditD" runat="server" Text="確定" class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancelD" type="button" value="取消" class="btn" onclick="window.close();" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
