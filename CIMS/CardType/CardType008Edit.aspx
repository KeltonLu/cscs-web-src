<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType008Edit.aspx.cs"
    Inherits="CardType_CardType008Edit" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>晶片基本資料檔編輯</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>

            <script type="text/javascript">
            function ConfirmDel()
            {
                if(document.getElementById('IsNew').value == "insert")
                {
                    return CheckClientValidate();
                }
                else
                {                    
                    if(document.getElementById('chkDel').checked)  
                    {
                        return confirm('是否刪除此筆訊息');
                    }     
                    else
                    {
                        return CheckClientValidate();
                    }                  
                }
            }        
            </script>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table width="100%" align="center" cellpadding="0" cellspacing="2" border="0">
                <tr style="height: 10px">
                    <td colspan="2">
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">
                            <asp:Label ID="lblTitle" runat="server" /></font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" class="btn" OnClientClick="return ConfirmDel();"
                            OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="取消" class="btn" CausesValidation="False"
                            OnClick="btnCancel_Click" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    <span class="style1" runat="server" id="wn">*</span>晶片名稱：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtWafer_Name" runat="server" onkeyup=" LimitLengthCheck(this, 20)"
                                        size="15" MaxLength="20" />&nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                            runat="server" ControlToValidate="txtWafer_Name" ErrorMessage="晶片名稱不能為空！">*</asp:RequiredFieldValidator>
                                    <cc1:AjaxValidator ID="AjaxValidatorName" runat="server" ControlToValidate="txtWafer_Name"
                                        Display="Dynamic" ErrorMessage="晶片名稱重複" OnOnAjaxValidatorQuest="AjaxValidatorName_OnAjaxValidatorQuest"
                                        ClientValidationFunction="AjaxValidatorFunction"></cc1:AjaxValidator>
                                    <asp:CheckBox ID="chkIs_Using" runat="server" Text="使用中" Checked="false" />
                                </td>
                                <td width="20%" align="right">
                                    <span class="style1">*</span>晶片容量（EEPROM）：</td>
                                <td>
                                                        <%--Dana 20161021 最大長度由 4 改為 5 --%>
                                    <asp:TextBox ID="txtWafer_Capacity" Style="ime-mode: disabled; text-align: right"
                                        onblur="value=GetValue(this.value)" onkeyup="CheckNum('txtWafer_Capacity',4)"
                                        runat="server" size="15" MaxLength="5" />K
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtWafer_Capacity"
                                        ErrorMessage="晶片容量不能為空！">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    <span class="style1">*</span>品牌：</td>
                                <td>
                                    <asp:TextBox ID="txtMark" runat="server" size="15" MaxLength="10" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtMark"
                                        ErrorMessage="品牌不能為空！">*</asp:RequiredFieldValidator></td>
                                <td width="20%" align="right">
                                    <span class="style1">*</span>晶片供應商：</td>
                                <td>
                                    <asp:TextBox ID="txtWafer_Factory" runat="server" onkeyup=" LimitLengthCheck(this, 15)"
                                        size="15" MaxLength="15" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtWafer_Factory"
                                        ErrorMessage="晶片供應商不能為空！">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    接觸式介面：</td>
                                <td>
                                    <asp:TextBox ID="txtInterface" runat="server" size="15" MaxLength="10" />
                                </td>
                                <td width="20%" align="right">
                                    非接觸式介面：</td>
                                <td>
                                    <asp:TextBox ID="txtProtocle" runat="server" size="15" MaxLength="10" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    晶片型號：</td>
                                <td>
                                    <asp:TextBox ID="txtWafer_Type" runat="server" size="15" onkeyup=" LimitLengthCheck(this, 30)" />
                                </td>
                                <td width="20%" align="right">
                                    OS：</td>
                                <td>
                                    <asp:TextBox ID="txtOS" runat="server" size="15" onkeyup=" LimitLengthCheck(this, 10)" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right" style="height: 21px">
                                    JAVA 版本：</td>
                                <td style="height: 21px">
                                    <asp:TextBox ID="txtJava_Version" runat="server" size="15" onkeyup=" LimitLengthCheck(this, 10)" />
                                </td>
                                <td width="20%" align="right" style="height: 21px">
                                    Crypto功能：</td>
                                <td style="height: 21px">
                                    <asp:TextBox ID="txtCrypto_Cap" runat="server" size="15" onkeyup=" LimitLengthCheck(this, 20)" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    ROM容量：</td>
                                <td>
                                                        <%--Dana 20161021 最大長度由 4 改為 5 --%>
                                    <asp:TextBox ID="txtROM_Capacity" Style="ime-mode: disabled; text-align: right" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtROM_Capacity',4)" runat="server" size="15" MaxLength="5" />K
                                </td>
                                <td width="20%" align="right">
                                    初次發卡時間：</td>
                                <td>
                                    <asp:TextBox ID="txtFirst_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtFirst_Time')})"
                                            src="../images/calendar.gif" align="absmiddle" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    參考單價：</td>
                                <td>
                                                        <%--Dana 20161021 最大長度由 8 改為 9 --%>
                                    <asp:TextBox ID="txtUnit_Price" Style="ime-mode: disabled; text-align: right" size="15"
                                        onblur="CheckAmt('txtUnit_Price',5,2);value=GetValue(this.value)" onkeyup="CheckAmt('txtUnit_Price',5,2)"
                                        MaxLength="9" runat="server" />
                                </td>
                                <td width="20%" align="right">
                                    出廠預設應用程式：</td>
                                <td>
                                    <asp:TextBox ID="txtPre_Program" runat="server" size="15" onkeyup=" LimitLengthCheck(this, 100)" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" style="width: 15%">
                                    選擇空白卡廠：</td>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    &nbsp;</td>
                                <td colspan="3">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="center">
                                                        可選擇空白卡廠</td>
                                                    <td>
                                                    </td>
                                                    <td align="center">
                                                        已選擇空白卡廠</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:ListBox ID="LbLeft" SelectionMode="multiple" runat="server" Height="120px" Width="150px">
                                                        </asp:ListBox>
                                                    </td>
                                                    <td align="center" style="width: 50px">
                                                        <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnSelect" runat="server"
                                                            Text=">" OnClick="btnSelect_Click" /><br />
                                                        <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnRemove" runat="server"
                                                            Text="<" OnClick="btnRemove_Click" /><br />
                                                        <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnSelectAll"
                                                            runat="server" Text=">>" OnClick="btnSelectAll_Click" /><br />
                                                        <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnRemoveAll"
                                                            runat="server" Text="<<" OnClick="btnRemoveAll_Click" />
                                                    </td>
                                                    <td>
                                                        <asp:ListBox ID="LbRight" SelectionMode="multiple" runat="server" Height="120px"
                                                            Width="150px"></asp:ListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    選擇晶片卡種：</td>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td colspan="4">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType1" runat="server" SetWidth="250"></uc1:uctrlCardType>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    備註1：</td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtComment_One" runat="server" TextMode="MultiLine" Width="400px"
                                        Height="50" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    備註2：</td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtComment_Second" runat="server" TextMode="MultiLine" Width="400px"
                                        Height="50" />
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="15%" align="right">
                                    備註3：</td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtComment_Third" runat="server" TextMode="MultiLine" Width="400px"
                                        Height="50" />
                                </td>
                            </tr>
                            <tr class="tbl_row" id="trDel" runat="server">
                                <td width="15%" align="right">
                                    &nbsp;</td>
                                <td colspan="3">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" colspan="4">
                                    <asp:HiddenField ID="IsNew" runat="server" />
                                    &nbsp;
                                    <asp:Button ID="btnEditD" Text="確定" class="btn" runat="server" OnClientClick="return ConfirmDel();"
                                        OnClick="btnEditD_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnCancelD" Text="取消" class="btn" runat="server" CausesValidation="False"
                                        OnClick="btnCancelD_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
