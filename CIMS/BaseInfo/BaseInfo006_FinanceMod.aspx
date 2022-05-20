<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo006_FinanceMod.aspx.cs"
    Inherits="BaseInfo_BaseInfo006_FinanceMod" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>參數設定修改畫面</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script type="text/javascript">
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
    </script>

</head>
<body>
    <form id="form2" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr>
                    <td colspan="2" style="height: 20px">
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">
                            <asp:Label ID="lbTitle" runat="server"></asp:Label></font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" OnClick="btnEdit_Click" CssClass="btn"
                            OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;
                        <input class="btn" onclick="window.location='<%=strURLPath%>&Con=1';" id="btnCancel"
                            type="button" value="取消" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" cellspacing="0">
                            <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    <span class="style1">*</span>參數名稱：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:TextBox ID="txtParamType_Name" Enabled="false" runat="server"></asp:TextBox>
                                    <input id="hdParamType_Code" runat="server" type="hidden" /></td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    <span class="style1">*</span>參數值：</td>
                                <td>
                                    <asp:TextBox ID="txtParam_Name" runat="server" MaxLength="50" onkeyup=" LimitLengthCheck(this, 50)" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtParam_Name"
                                        ErrorMessage="參數值不能為空">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    備註：</td>
                                <td>
                                    <asp:TextBox ID="txtParam_Comment" runat="server" Height="95px" Width="400px" TextMode="MultiLine" />
                                </td>
                            </tr>
                            <tr id="trDel" runat="server" class="tbl_row">
                                <td width="10%">
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnEdit1" runat="server" Text="確定" OnClick="btnEdit_Click" CssClass="btn"
                            OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;
                        <input class="btn" onclick="window.location='<%=strURLPath%>&Con=1';" id="btnCancel1"
                            type="button" value="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
