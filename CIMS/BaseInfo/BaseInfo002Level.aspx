<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo002Level.aspx.cs" Inherits="BaseInfo_BaseInfo002Level" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>級距</title>
    <META HTTP-EQUIV="pragma" CONTENT="no-cache">
    <link href="../css/Global.css"  rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%" >
            <tr>
                <td>&nbsp;</td>
            </tr>
             <tr>
                <td>
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%" >
                         <td class="PageTitle">
                            級距編輯
                        </td>
                        <td align="right">
                            <asp:Button CssClass="btn" ID="btnSubmitUp" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                            <input id="Button3" class="btn" type="button" value="取消" onclick="window.close();" />
                        </td>
                    </table>
                </td>
               
            </tr>
            <tr>
                <td>
                    <table cellpadding="0" width="100%" cellspacing="2" border="0">
                        <tr class="tbl_title">
                            <td align="center">
									級距編輯
							</td>
                        </tr>
                        <tr class="tbl_row">
						    <td>
						        <table cellpadding="0" width="100%" cellspacing="0" border="0" id="tbLevel" runat="server">
			                        <tr valign="baseline">
			                            <td align="right" width="30%"><span class="style1">*</span>級距最小數量：</td>
			                            <td align="left">
                                                                    <%--Dana 20161021 最大長度由9改為11 --%>
			                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtLevel_Min',9)" ID="txtLevel_Min" runat="server" MaxLength="11"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtLevel_Min"
                                                ErrorMessage="級距最小數量不能為空">*</asp:RequiredFieldValidator></td>
			                        </tr>
			                        <tr valign="baseline">
			                            <td align="right"><span class="style1">*</span>級距最大數量：</td>
			                            <td align="left">
                                                                    <%--Dana 20161021 最大長度由9改為11 --%>
			                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="if(value.toUpperCase()=='MAX'){value=999999999}CheckNum('txtLevel_Max',9);value=GetValue(this.value)" onkeyup="" ID="txtLevel_Max" runat="server" MaxLength="11"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLevel_Max"
                                                ErrorMessage="級距最大數量不能為空">*</asp:RequiredFieldValidator>
                                            </td>
			                        </tr>
						        </table>
						    </td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
                <td align="right">
                    <asp:Button CssClass="btn" ID="btnSubmitDn" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                    <input id="Button1" class="btn" type="button" value="取消" onclick="window.close();" />
                </td>
            </tr>
        </table>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
