<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository004Edit.aspx.cs"
    Inherits="Depository_Depository004Edit" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退貨作業修改</title>
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
    <form id="form1" runat="server">
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
                        <font class="PageTitle">退貨作業修改/刪除</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" OnClick="btnEdit_Click" CssClass="btn"
                            OnClientClick="return doDelConfirm();" />
                        &nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="取消" OnClick="btnCancel_Click" CausesValidation="False"
                            CssClass="btn" />
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" cellspacing="0">
                            <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    入庫流水編號：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:Label ID="lblStork_Id" runat="server" />                                
                                </td>
                            </tr>
                              <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    版面簡稱：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:Label ID="lblName" runat="server" />                                
                                </td>
                            </tr>
                              <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    入庫量：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:Label ID="lblIncome_Number" runat="server" />                                
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    退貨日期：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:Label ID="lblCancelDate" runat="server" />                                
                                </td>
                            </tr>
                                <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    <span class="style1">*</span>退貨量：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:TextBox  ID="txtCancel_number" runat="server" onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="CheckNum('txtCancel_number',9);value=GetValue(this.value)" onkeyup="CheckNum('txtCancel_number',9)" MaxLength="11" />  
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCancel_number"
                                        ErrorMessage="退貨量必須輸入">*</asp:RequiredFieldValidator>                         
                                </td>
                            </tr>
                                   <tr class="tbl_row">
                                <td align="right" width="10%" style="height: 24px">
                                    備註：</td>
                                <td width="90%" style="height: 24px">
                                    <asp:TextBox  ID="txtComment" runat="server"  Height="80px" Width="400px" TextMode="MultiLine"/>                               
                                </td>
                            </tr>
                               <tr class="tbl_row" id="trDel">
                                <td align="right" width="10%" >
                                    </td>
                                <td width="90%">
                                    <asp:CheckBox  ID="chkDel" Text="刪除" runat="server" />                           
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnEdit1" runat="server" Text="確定" OnClick="btnEdit_Click" CssClass="btn"
                            OnClientClick="return doDelConfirm();" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancel1" runat="server" Text="取消" OnClick="btnCancel_Click" CausesValidation="False"
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
