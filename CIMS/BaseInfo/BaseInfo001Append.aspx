<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo001Append.aspx.cs" Inherits="BaseInfo_BaseInfo001Append" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>預算管理作業-追加預算</title>
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
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%" >
                        <td class="PageTitle">
                            預算管理作業-追加預算
                        </td>
                        <td align="right">
                            <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click"  />&nbsp;&nbsp;
                            <input class="btn" id="Button2" type="button" value="取消" onclick="window.close();" />
                        </td>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table cellpadding="0" width="100%" cellspacing="2" border="0">
                        
                        <tr class="tbl_row">
						    <td>
						        <table cellpadding="0" width="100%" cellspacing="0" border="0">
						            <tr>
						                <td width="80%">
						                    <table cellpadding="0" width="100%" cellspacing="0" border="0">
						                        <tr valign="baseline">
                                                    <td align="right" width="30%">
								                        <span class="style1">*</span>簽呈文號：
								                    </td>
								                    <td colspan="3">
                                                        <asp:TextBox ID="txtBUDGET_ID" onblur="LimitLengthCheck(this,30);" onkeyup="LimitLengthCheck(this,30);"  runat="server" ValidationGroup="All" MaxLength="30"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBUDGET_ID"
                                                            Display="Dynamic" ErrorMessage="簽呈文號不可為空&nbsp;" >*</asp:RequiredFieldValidator>
                                                        <cc1:AjaxValidator id="ajvBUDGET_ID" runat="server" ErrorMessage="簽呈文號已經存在" ControlToValidate="txtBUDGET_ID" ClientValidationFunction="AjaxValidatorFunction" OnOnAjaxValidatorQuest="AjaxValidator_BUDGET_ID_OnAjaxValidatorQuest">簽呈文號已經存在</cc1:AjaxValidator>                                                                
                                                    </td>
								                </tr>
						                        <tr valign="baseline">
                                                    <td align="right" width="30%">
								                       <span class="style1">*</span>追加預算名稱：
								                    </td>
								                    <td colspan="3">
                                                        <asp:TextBox ID="txtBudget_Name" onblur="LimitLengthCheck(this,30);" onkeyup="LimitLengthCheck(this,30);"  runat="server" ValidationGroup="All" MaxLength="30"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtBudget_Name"
                                                            Display="Dynamic" ErrorMessage="追加預算名稱不可為空&nbsp;" >*</asp:RequiredFieldValidator>
                                                        <cc1:AjaxValidator id="ajvBUDGET_Name" runat="server" ErrorMessage="追加預算名稱已經存在" ControlToValidate="txtBudget_Name" ClientValidationFunction="AjaxValidatorFunction" OnOnAjaxValidatorQuest="ajvBUDGET_Name_OnAjaxValidatorQuest" >追加預算名稱已經存在</cc1:AjaxValidator>
                                                    </td>
								                </tr>								                
								                <tr valign="baseline">
                                                    <td align="right" >
									                        <span class="style1">*</span>上傳簽呈影像：
								                    </td>
								                    <td colspan="3">
                                                        <asp:FileUpload ID="fludFileUpload" runat="server" Width="200px" />
                                                        <asp:Button ID="btnUpload" runat="server" CssClass="btn" Text="上傳" OnClick="btnUpload_Click" CausesValidation="False" />
								                        <asp:HyperLink ID="HyperLink" runat="server" Target="_blank"></asp:HyperLink></td>
								                </tr>
								                <tr valign="baseline">
                                                    <td align="right" >
									                        <span class="style1">*</span>有效期間（起）：
								                    </td>
								                    <td colspan="3">
								                        <asp:TextBox ID="txtVALID_DATE_FROM" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_FROM')})" src="../images/calendar.gif" align="absmiddle">
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtVALID_DATE_FROM"
                                                            ErrorMessage="有效期間（起）不能為空">*</asp:RequiredFieldValidator> 
                                                    </td>
								                </tr>
								                <tr valign="baseline">
								                    <td align="right">    
								                        <span class="style1">*</span>有效期間（迄）：
								                    </td>
								                    <td colspan="3">
								                        <asp:TextBox ID="txtVALID_DATE_TO" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_TO')})" src="../images/calendar.gif" align="absmiddle">   
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtVALID_DATE_TO"
                                                            ErrorMessage="有效期間（迄）不能為空">*</asp:RequiredFieldValidator>
                                                         <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtVALID_DATE_FROM"
                                                            ControlToValidate="txtVALID_DATE_TO" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                                            Type="Date">*</asp:CompareValidator>
                                                    </td>
								                </tr>
								                 <tr valign="baseline">
                                                    <td align="right" style="height: 24px" >
									                        <span class="style1">*</span>金額：
								                    </td>
								                    <td colspan="3" style="height: 24px">
                                                        <%--Dana 20161021 最大長度由13改為16 --%>
                                                            <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="CheckAmt('txtCard_Price',10,2);value=GetValue(this.value)" onkeyup="CheckAmt('txtCard_Price',10,2)" ID="txtCard_Price" runat="server" MaxLength="16"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtCard_Price"
                                                            ErrorMessage="金額不能為空">*</asp:RequiredFieldValidator>&nbsp;
                                                        
                                                    </td>
								                </tr>
								                <tr valign="baseline">
                                                    <td align="right" >
									                        卡數：
								                    </td>
								                    <td colspan="3">
                                                        <%--Dana 20161021 最大長度由9改為11 --%>
                                                            <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtCard_Num',9)" ID="txtCard_Num" runat="server" MaxLength="11"></asp:TextBox>
                                                    </td>
								                </tr>
						                    </table>
						                </td>
						                <td>
                                            <asp:Image ID="imgFileUrl" ImageUrl="../images/NoPic.jpg" runat="server" Height="100px" Width="100px" Visible="False" /></td>
						            </tr>
						        </table>
						    </td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
                <td align="right">
                    <input id="hdCardAmt" runat="server" type="hidden" style="width: 1px" />
                    <input id="hdCardNum" runat="server" type="hidden" style="width: 1px" />
                    <asp:Button CssClass="btn" ID="btnSubmit" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                    <input id="Button1" class="btn" type="button" value="取消" onclick="window.close();" />
            </tr>
        </table>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
