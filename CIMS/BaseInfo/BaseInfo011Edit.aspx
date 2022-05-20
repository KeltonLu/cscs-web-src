<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo011Edit.aspx.cs" Inherits="BaseInfo_BaseInfo011Edit" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>庫存類別修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <script type="text/javascript">
        function ConfirmDel()
        {
            if(document.getElementById('chkDel').checked)
            {
                return confirm("是否刪除此筆訊息");
            }else
            {
                return CheckClientValidate();
            }
        }       
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
                        <font class="PageTitle">庫存類別修改/刪除</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEditUp" OnClick="btnEditUp_click" runat="server" Text="確定" CssClass="btn" />&nbsp;&nbsp;
                        <input id="btnCancelUp" class="btn" onclick="returnform('BaseInfo011.aspx')" type="button"
                            value="取消" />&nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="15%">
                                    <span class="style1"></span>類別編號：
                                </td>
                                <td>
                                    <asp:Label ID="txtStatus_Code" runat="server" Text="Label"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="15%">
                                    <span class="style1">*</span>類別名稱：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtStatus_Name" onkeyup="LimitLengthCheck(this, 10)" runat="server"
                                        MaxLength="10" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtStatus_Name"
                                        ErrorMessage="類別名稱不能為空">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" width="15%">
                                 </td>
                                <td width="85%">
                                    
                                </td>
                            </tr>
                             <tr class="tbl_row">
                                <td align="right" width="15%">
                                    <span class="style1">*</span>廠商庫存異動檔  
                                                                匯入對庫存增減：
                                </td>
                                <td width="85%">
                                    <asp:UpdatePanel id="UpdatePanel2" runat="server">
                                    <contenttemplate>
                                    <table>
                                        <tr>
                                            <td>
                                                 <asp:DropDownList ID="dropOperate" runat="server">
                                                    <asp:ListItem Value="">無</asp:ListItem>
                                                    <asp:ListItem Value="+">+</asp:ListItem>
                                                    <asp:ListItem Value="-">-</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>是否可修改</td>
                                            <td>
                                                <asp:RadioButtonList ID="radlIs_UptDepository" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="radlIs_UptDepository_SelectedIndexChanged">
                                                    <asp:ListItem Text="是" Value="Y" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="否" Value="N"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                    </table>
                                    </contenttemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" width="15%">
                                    <span class="style1">*</span>是否顯示在公式中：</td>
                                <td width="85%">
                                    <asp:RadioButtonList ID="radlIs_Display" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Text="是" Value="Y" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="否" Value="N"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="middle">
                                <td align="right" width="15%">
                                    <span class="style2"></span>備註：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtComment"  runat="server" TextMode="MultiLine" Height="50px" Width="400px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trDel" runat="server" class="tbl_row">
                                <td width="15%">
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnEditDown" OnClick="btnEditDown_click" runat="server" Text="確定"  CssClass="btn"/>&nbsp;&nbsp;
                        <input id="btnCancelDown" class="btn" onclick="returnform('BaseInfo011.aspx')" type="button"
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
