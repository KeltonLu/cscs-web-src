<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType001Edit.aspx.cs"
    Inherits="CardType_CardType001Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡種群組編輯</title>
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
                if(CheckClientValidate())
                {
                    if(document.getElementById('IsNew').value != "insert")
                    {
                        if(document.getElementById('chkDel').checked)  
                        {
                            return confirm('是否刪除此筆訊息');
                        }  
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }    
            function returnform()
            {
                window.location="cardtype001.aspx?con=1";
            }     
            </script>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">
                            <asp:Label ID="lblTitle" runat="server"></asp:Label></font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" OnClientClick="return ConfirmDel();"
                            class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" type="button" value="取消" class="btn" onclick="returnform();" />                        
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr>
                                <td>
                                    <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" width="10%">
                                                <span class="style1">*</span>用途：
                                            </td>
                                            <td width="90%">
                                                <asp:DropDownList ID="drpParam_Name" runat="server" DataTextField="param_name" DataValueField="param_code"
                                                    AutoPostBack="True" OnSelectedIndexChanged="drpParam_Name_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" width="10%">
                                                <span class="style1">*</span>群組名稱：
                                            </td>
                                            <td width="90%">
                                                <asp:TextBox ID="txtGroup_Name" runat="server" onkeyup=" LimitLengthCheck(this, 30)"
                                                    MaxLength="30" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtGroup_Name"
                                                    ErrorMessage="群組名稱不能為空">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td width="15%" align="right">
                                                &nbsp;</td>
                                            <td colspan="3">
                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                    <ContentTemplate>
                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td align="center">
                                                                    可選擇卡種</td>
                                                                <td>
                                                                </td>
                                                                <td align="center">
                                                                    已選擇卡種</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:ListBox ID="LbLeft" SelectionMode="multiple" runat="server" Height="120px" Width="250px">
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
                                                                        Width="250px"></asp:ListBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <tr id="trDel" runat="server" class="tbl_row">
                                            <td>
                                            </td>
                                            <td colspan="3">
                                                <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />
                                            </td>
                                        </tr>
                                        <tr valign="baseline">
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td align="right" colspan="3">
                                                &nbsp;<input id="Hid_Code" type="hidden" runat="server" />
                                                <asp:HiddenField ID="IsNew" runat="server" />
                                                <input id="Hid_Name" type="hidden" runat="server" />
                                                <asp:Button ID="btnEditD" Text="確定" class="btn" runat="server" OnClientClick="return ConfirmDel();"
                                                    OnClick="btnEdit_Click" />
                                                &nbsp;&nbsp;
                                                <input id="btnCancelD" type="button" value="取消" class="btn" onclick="returnform();" />                                                
                                            </td>
                                        </tr>
                                    </table>
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
