<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType007Add.aspx.cs" Inherits="CardType_CardType007Add" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>�פJ���س]�w�s�W</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
    function doRadio(rad) {
        if (rad == document.getElementById("adrtSmallTotal")) {
            document.getElementById("trCardType1").style.display = "table-row";
        }
        else {
            document.getElementById("trCardType1").style.display = "none";
        }
    }

    function doLoad() {
        if (true == document.getElementById("adrtSmallTotal").checked) {
            document.getElementById("trCardType1").style.display = "table-row";
        }
        else {
            document.getElementById("trCardType1").style.display = "none";
        }
    }
    //-->
</script>

<body onload="doLoad();">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr valign="baseline">
                    <td colspan="2" style="height: 20px">&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="height: 26px">
                        <span style="font-size: 20px"><font class="PageTitle">�פJ���س]�w�s�W</font></span>
                    </td>
                    <td align="right" style="height: 26px; width: 292px; font-family: Times New Roman;">
                        <asp:Button ID="btnAdd1" runat="server" Text="�T�w" OnClick="btnAdd1_Click" CssClass="btn" />&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="����" Style="margin-left: 11px;" OnClick="btnCancel_Click" CausesValidation="False"
                            CssClass="btn" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table style="width: 100%;" cellspacing="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 18%; height: 26px;">
                                    <span><span class="style1">*</span>���O�G</span></td>
                                <td style="width: 90%;">&nbsp;<input type="radio" name="radioType" runat="server" onclick="doRadio(this);"
                                    id="adrtSmallTotal" checked="true" value="1" /><span>�p�p��</span>
                                    <input type="radio" name="radioType" runat="server" onclick="doRadio(this);" id="adrtNextMonth"
                                        value="2" /><span>����w��</span>
                                    <input id="adrtYear" name="radioType" runat="server" onclick="doRadio(this);" type="radio"
                                        value="3" /><span>�~�׹w��</span></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 18%; height: 26px;">
                                    <span><span class="style1">*</span>���A���ɮצW�١G</span></td>
                                <td style="width: 90%;" style="height: 26px">
                                    <asp:TextBox ID="txtFile_Name" runat="server" MaxLength="20" onkeyup="LimitLengthCheck(this, 20)" /><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFile_Name"
                                        ErrorMessage="�D���ɮצW�٤��ର��">*</asp:RequiredFieldValidator>
                                    <cc1:AjaxValidator ID="ajvFile_Name" runat="server" ControlToValidate="txtFile_Name"
                                        ErrorMessage="�D���ɮצW�٤w�g�s�b" OnOnAjaxValidatorQuest="ajvFile_Name_OnAjaxValidatorQuest">
�D���ɮצW�٤w�g�s�b</cc1:AjaxValidator>
                                </td>
                            </tr>
                            <tr valign="baseline" class="tbl_row">
                                <td colspan="2">
                                    <div id='trCardType1' style="display: table-row;">
                                        <table style="width: 100%;" cellspacing="0">
                                            <tr>
                                                <td style="width: 18%; height: 26px; text-align: right;" nowrap="true">
                                                    <span class="style1">*</span>�妸�G</td>
                                                <td style="width: 90%; height: 26px" nowrap="true">
                                                    <asp:DropDownList ID="dropMakeCardType_RID" runat="server" DataTextField="Text" DataValueField="Value">
                                                    </asp:DropDownList></td>
                                            </tr>
                                        </table>
                                    </div>

                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <input id="radType" runat="server" type="hidden" value="0" />
                        <asp:Button ID="btnAdd2" runat="server" Text="�T�w" OnClick="btnAdd1_Click" CssClass="btn" />
                        <asp:Button ID="btnCancel1" runat="server" Text="����" Style="margin-left: 11px;" OnClick="btnCancel_Click" CausesValidation="False"
                            CssClass="btn" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
