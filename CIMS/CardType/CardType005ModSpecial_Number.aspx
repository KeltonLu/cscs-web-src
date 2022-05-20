<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType005ModSpecial_Number.aspx.cs" Inherits="CardType_CardType005ModSpecial_Number" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>特殊設定明細</title>
    <META HTTP-EQUIV="pragma" CONTENT="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1" border="0">
				<tr style="height:5px">
					<td colspan="2">
						&nbsp;
					</td>
				</tr>
				<tr valign="baseline">
					<td>
						<font class="PageTitle">特殊設定明細</font>
					</td>
					<td align="right">
                        &nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />
                        &nbsp;&nbsp; &nbsp;<input id="Button1" class="btn" onclick="window.close();" type="button"
                            value="取消" />&nbsp;
					</td>
				</tr>
				<tr>
					<td colspan="2">
					<!--
						<table id="errorMessage" style="display:none">
							<tr>
								<td>
									<font class="error_message">資料保存成功！</font>
								</td>
							</tr>
						</table>
					-->
						<table cellpadding="0" width="100%" cellspacing="2" border="0">
							<tr>
							    <td>
							        <table class="tbl_row" cellpadding="0" width="100%" cellspacing="2" border="0">							                                                
                                        <tr class="tbl_row" valign="baseline">
								            <td align="right" width="100px">
									            <span class="style1">*</span>Perso廠：
								            </td>
							                <td>
                                                &nbsp;<asp:DropDownList ID="dropFactory" runat="server" Width="88px">
                                                </asp:DropDownList></td>
							            </tr>                                        
							            <tr id="tr2" class="tbl_row" valign="baseline">
								            <td align="right">
									            <span class="style1">*</span>數量：
								            </td>
							                <td>
                                                &nbsp;<asp:TextBox ID="txtNumberValue" runat="server" Width="100px" MaxLength="9"></asp:TextBox><asp:RegularExpressionValidator
                                                    ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtNumberValue"
                                                    ErrorMessage="數量欄位请输入为非负整数的數值!" ValidationExpression="^(0|[1-9][0-9]*)$" SetFocusOnError="True">*</asp:RegularExpressionValidator></td>
							            </tr>                                                                                                                
							        </table>
							    </td>
							</tr>
						</table>
					</td>
				</tr>
				<tr valign="baseline">
				    <td>
				        &nbsp;
				    </td>
					<td align="right">
                        &nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />
                        &nbsp;&nbsp; &nbsp;<input id="Button2" class="btn" onclick="window.close();" type="button"
                            value="取消" />&nbsp;
					</td>
				</tr>
			</table>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
