<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType007Mod.aspx.cs" Inherits="CardType_CardType007Mod" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>匯入項目設定修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
    function doRadio(rad) {
        //if (rad == document.getElementById("adrtSmallTotal"))
        //{
        //    document.getElementById("trCardType1").style.display="table-row";
        //}
        //else
        //{
        //   document.getElementById("trCardType1").style.display="none";
        //}
    }
    function doLoad() {
        //if (true == document.getElementById("adrtSmallTotal").checked)
        //{
        //   document.getElementById("trCardType1").style.display="table-row";
        //}
        //else
        //{
        //    document.getElementById("trCardType1").style.display="none";
        //}
    }

    //        function delConfirm()
    //        {
    //            if (true == document.getElementById("chkDel").checked)
    //            {
    //               return  confirm('確認刪除此筆訊息?');
    //            }
    //            else
    //            {
    //                return CheckClientValidate();
    //            }
    //        } 

    function delConfirm() {
        if (CheckClientValidate()) {
            if (document.getElementById('chkDel').checked) {
                return confirm('是否刪除此筆訊息');
            }
        }
        else {
            return false;
        }
    }

    //-->
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" style="width: 100%">
                <tr valign="baseline">
                    <td colspan="2" style="height: 20px">&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle"><span>匯入項目設定修改/刪除</span></font>
                    </td>
                    <td align="right" style="height: 24px; font-family: Times New Roman;">
                        <asp:Button ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit1_Click" OnClientClick="return delConfirm();"
                            CssClass="btn" />
                        <input id="Button1" type="button" style="margin-left: 11px;" value="取消" class="btn" onclick="returnform('cardtype007.aspx')" /></td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table width="100%" cellspacing="0">
                            <tr class="tbl_row">
                                <td align="right" style="width: 18%">
                                    <span><span class="style1">*</span>類別：</span></td>
                                <td width="90%">&nbsp;<asp:Label ID="lbParam_Name" runat="server"></asp:Label>
                                    <input type="radio" name="radioType" runat="server" onclick="doRadio(this);"
                                        checked="true" id="adrtSmallTotal" value="1" visible="false" /><span> </span>
                                    <input type="radio" name="radioType" runat="server" onclick="doRadio(this);" id="adrtNextMonth"
                                        value="2" visible="false" /><span></span><input id="adrtYear" name="radioType" runat="server" onclick="doRadio(this);" type="radio"
                                            value="3" visible="false" /><span></span></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 18%; height: 26px;">
                                    <span class="style1">*</span>伺服器檔案名稱：</td>
                                <td width="90%" style="height: 26px">
                                    <asp:TextBox ID="txtFile_Name" runat="server" MaxLength="20" onkeyup="LimitLengthCheck(this, 20)" /><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFile_Name"
                                        ErrorMessage="主機檔案名稱不能為空">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr valign="baseline" class="tbl_row" id='trCardType1' runat="server">
                                <td align="right" style="width: 18%; height: 26px; text-align: right;">
                                    <span class="style1">*</span>批次：</td>
                                <td style="height: 26px">
                                    <asp:DropDownList ID="dropMakeCardType_RID" runat="server" DataTextField="Text" DataValueField="Value">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="baseline" class="tbl_row">
                                <td align="right" style="width: 18%"></td>
                                <td width="90%">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <input id="radType" runat="server" type="hidden" value="0" />
                        <asp:Button ID="btnSubmit2" runat="server" Text="確定" OnClick="btnSubmit1_Click" OnClientClick="return delConfirm();"
                            CssClass="btn" />
                        <input id="Button2" type="button" value="取消" class="btn" style="margin-left: 11px;" onclick="returnform('cardtype007.aspx')" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
