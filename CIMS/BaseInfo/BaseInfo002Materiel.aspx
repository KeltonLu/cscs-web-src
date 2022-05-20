<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo002Materiel.aspx.cs" Inherits="BaseInfo_BaseInfo002Materiel" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>特殊材質編輯</title>
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
                            特殊材質編輯
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
									特殊材質編輯
							</td>
                        </tr>
                        <tr class="tbl_row">
						    <td>
						        <table cellpadding="0" width="100%" cellspacing="0" border="0">
			                        <tr valign="baseline">
                                        <td align="right" width="15%">
					                        <span class="style1">*</span>特殊材質：
					                    </td>
					                    <td colspan="3">
                                            <asp:DropDownList DataTextField="Material_Name" DataValueField="RID" ID="dropMaterial_RID" runat="server">
                                            </asp:DropDownList>                                           
                                        </td>
					                </tr>
			                        <tr valign="baseline">
                                        <td align="right">
					                       <span class="style1">*</span>單價：
					                    </td>
					                    <td colspan="3">
                                                        <%--Dana 20161021 最大長度由11改為13 --%>
                                            <asp:TextBox ID="txtBase_Price" runat="server" onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="CheckAmt('txtBase_Price',8,2);value=GetValue(this.value)" onkeyup="CheckAmt('txtBase_Price',8,2)" MaxLength="13"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBase_Price"
                                                ErrorMessage="單價不能為空!">*</asp:RequiredFieldValidator></td>
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
