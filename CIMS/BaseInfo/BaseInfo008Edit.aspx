<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo008Edit.aspx.cs" Inherits="BaseInfo_BaseInfo008Edit" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>卡片材質基本檔修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script type="text/javascript">
     <!--
    function doDelConfirm()
            {
                  if(document.getElementById('chkDel').checked)
                    {
                        return confirm("是否刪除此筆訊息");
                    }
                  else
                    {
                        return CheckClientValidate();
                    }
            } 
        //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">卡片材質基本檔修改/刪除</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEditUp" OnClick="btnEditUp_click" runat="server" Text="確定" CssClass="btn"
                            OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('BaseInfo008.aspx')" type="button"
                            value="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="15%">
                                    材質編號：
                                </td>
                                <td>
                                    <asp:Label ID="lblMaterial_Code" runat="server" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="15%">
                                    <span class="style1">*</span>材質名稱：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMaterial_Name" onkeyup="LimitLengthCheck(this, 30)" runat="server"
                                        MaxLength="30" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMaterial_Name"
                                        ErrorMessage="材質名稱不能為空">*</asp:RequiredFieldValidator>
                                </td>
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
                             <tr valign="baseline">
                                <td>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkDel" runat="server" />刪除
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnEditDown" OnClick="btnEditDown_click" runat="server" Text="確定"
                            CssClass="btn" OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('BaseInfo008.aspx')" type="button"
                            value="取消" />
                    </td>
                </tr>
            </table>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                ShowSummary="False" />
        </div>
    </form>
</body>
</html>
