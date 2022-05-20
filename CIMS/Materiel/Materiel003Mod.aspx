<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Materiel003Mod.aspx.cs"
    Inherits="Materiel_Materiel003Mod" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>DM種類基本檔修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">
    function doDelConfirm() {
        if (document.getElementById('chkDel').checked) {
            return confirm("是否刪除此筆訊息");
        }
        else {
            return CheckClientValidate();
        }
    }
    function onStorage() {
        document.getElementById("txtStorageNum").disabled = "";
        document.getElementById("txtDays").disabled = "true";
        document.getElementById("txtDays").value = "";
    }
    function onDays() {
        document.getElementById("txtStorageNum").disabled = "true";
        document.getElementById("txtStorageNum").value = "";
        document.getElementById("txtDays").disabled = "";
    }
    function onOver() {
        document.getElementById("txtStorageNum").disabled = "true";
        document.getElementById("txtStorageNum").value = "";
        document.getElementById("txtDays").disabled = "true";
        document.getElementById("txtDays").value = "";
    }
    function setDisabled() {
        document.getElementById("UctrlCardType_dropCard_Purpose").disabled = "true";
        document.getElementById("UctrlCardType_dropCard_Group").disabled = "true";
        document.getElementById("UctrlCardType_LbLeft").disabled = "true";
        document.getElementById("UctrlCardType_LbRight").disabled = "true";
        document.getElementById("UctrlCardType_btnSelectAll").disabled = "true";
        document.getElementById("UctrlCardType_btnSelect").disabled = "true";
        document.getElementById("UctrlCardType_btnRemove").disabled = "true";
        document.getElementById("UctrlCardType_btnRemoveAll").disabled = "true";
    }
    function setEnabled() {
        document.getElementById("UctrlCardType_dropCard_Purpose").disabled = "";
        document.getElementById("UctrlCardType_dropCard_Group").disabled = "";
        document.getElementById("UctrlCardType_LbLeft").disabled = "";
        document.getElementById("UctrlCardType_LbRight").disabled = "";
        document.getElementById("UctrlCardType_btnSelectAll").disabled = "";
        document.getElementById("UctrlCardType_btnSelect").disabled = "";
        document.getElementById("UctrlCardType_btnRemove").disabled = "";
        document.getElementById("UctrlCardType_btnRemoveAll").disabled = "";
    }

</script>

<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="PageTitle">DM種類基本檔修改/刪除
                    </td>
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('Materiel003.aspx')" type="button"
                            value="取消" /></td>
                </tr>
                <tr class="tbl_row">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="0" border="0">
                            <tr class="tbl_row">
                                <td width="12%" align="right">品名編號：</td>
                                <td width="1">
                                    <asp:HiddenField ID="hidRID" runat="server" />
                                    <asp:HiddenField ID="hidRCU" runat="server" />
                                    <asp:HiddenField ID="hidRCT" runat="server" />
                                    <asp:Label ID="lblSerial_Number" runat="server"></asp:Label>
                            </tr>
                            <tr class="tbl_row">
                                <td width="12%" align="right">
                                    <span class="style1">*</span>品名：</td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" onkeyup=" LimitLengthCheck(this, 30)" MaxLength='30'></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="品名不能為空">*</asp:RequiredFieldValidator>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;使用狀態：<input
                                            id="usingRadio" name="stopUse" runat="server" type="radio" value="N" />使用&nbsp;&nbsp;<input
                                                id="stopRadio" name="stopUse" runat="server" type="radio" value="Y" />停用
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">單價：</td>
                                <td>
                                    <%--Dana 20161021 最大長度由 7 改為 8 --%>
                                    <asp:TextBox ID="txtUnit_Price" Style="text-align: right" runat="server" MaxLength='8'
                                        onfocus="DelDouhao(this)" onblur="CheckAmt('txtUnit_Price',4,2);value=GetValue(this.value)"
                                        onkeyup="CheckAmt('txtUnit_Price',4,2)"></asp:TextBox>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;帳務類別：<input id="adrtCard" runat="server"
                                        checked="true" type="radio" name="accounting" value="1" />信用卡帳&nbsp;&nbsp;<input id="adrtBlank" runat="server"
                                            type="radio" name="accounting" value="2" />銀行帳
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    <span class="style1">*</span>耗損率：</td>
                                <td>
                                    <asp:TextBox ID="txtWear_Rate" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtWear_Rate',3)" runat="server" MaxLength="3" Style="ime-mode: disabled; text-align: right"></asp:TextBox>%
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtWear_Rate"
                                        ErrorMessage="耗損率不能為空">*</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtWear_Rate"
                                        ErrorMessage="耗損率不能超過100" Operator="LessThanEqual" ValueToCompare="100" Type="Integer">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    <span class="style1">*</span>有效期間：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBegin_Date')})"
                                            src="../images/calendar.gif" align="absmiddle"><asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                                                runat="server" ErrorMessage="有效期間（起）不能為空" ControlToValidate="txtBegin_Date">*</asp:RequiredFieldValidator>
                                    ~<asp:TextBox ID="txtEnd_Date" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEnd_Date')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="有效期間（迄）不能為空"
                                        ControlToValidate="txtEnd_Date">*</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtEnd_Date" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date"></asp:CompareValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td height="2" colspan="2" align="right" bgcolor="#FFFFFF"></td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">批次：</td>
                                <td align="left">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <cc1:MoveListBox ID="mlbMakeType" runat="server" Width="200px" Height="180px" DataTextField="Text"
                                                            DataValueField="Value">
                                                        </cc1:MoveListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td colspan="2" align="right" bgcolor="#FFFFFF" style="height: 2px"></td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    <input type="radio" id="TypeAll" runat="server" name="Card_Type_Link_Type" value="1"
                                        onclick="setDisabled();" />全部<br>
                                    <input type="radio" id="TypePart" runat="server" name="Card_Type_Link_Type" value="2"
                                        onclick="setEnabled();" />部分</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr class="tbl_row">
                                <td colspan="2">
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td height="2" colspan="2" align="right" bgcolor="#FFFFFF"></td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    <span class="style1">*</span>安全存量：</td>
                                <td>
                                    <%--Dana 20161021 最大長度由 6 改為 7 --%>
                                    <input id="storageNum" name="radioSafeType" onclick="onStorage();" runat="server"
                                        type="radio" value="1" checked />最低安全庫存<asp:TextBox ID="txtStorageNum" runat="server"
                                            Style="text-align: right" MaxLength='7' onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                            onkeyup="CheckNum('txtStorageNum',6)"></asp:TextBox>
                                    <br />
                                    <input id="daysNum" type="radio" onclick="onDays();" name="radioSafeType" runat="server"
                                        value="2" />安全天數<asp:TextBox ID="txtDays" runat="server" MaxLength='3' Style="text-align: right"
                                            onkeyup="CheckNum('txtDays')"></asp:TextBox>天<br />
                                    <input id="overNum" name="radioSafeType" onclick="onOver();" runat="server" type="radio"
                                        value="3" />用完為止
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td width="12%" align="right">&nbsp;
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClientClick="return doDelConfirm();"
                            OnClick="btnSubmit_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('Materiel003.aspx')" type="button"
                            value="取消" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
