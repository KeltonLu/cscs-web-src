<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo042Edit.aspx.cs" Inherits="BaseInfo_BaseInfo042Edit" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>未命名頁面</title>
    <link href="../css/Global.css"  rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%" >
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">使用者資料維護</font>
                </td>
            </tr>
            <tr class="tbl_row">
                <td>
                   
                    <table width="100%" border="0">
                        <tr>
                            <td align="right" width="15%" >
                                <font color="red">*</font>使用者員編：
                            </td>
                            <td align="left">
                                <asp:TextBox id="txtUserID" onkeyup="UserIDChange()"  runat="server" MaxLength="64"></asp:TextBox> 
                                <asp:Button  CssClass="btn" id="btnLoad" onclick="btnLoad_Click" runat="server" Text="重新讀取LDAP資料"></asp:Button> 
                                <asp:RegularExpressionValidator id="RegularExpressionValidator_UserID" runat="server" ValidationGroup="All" ErrorMessage="使用者員編必須是英文字母或數字" Display="Dynamic" ControlToValidate="txtUserID" ValidationExpression="^(\w|\d){1,64}$">*</asp:RegularExpressionValidator> 
                                <asp:RequiredFieldValidator id="RequiredFieldValidator_UserID" runat="server" ErrorMessage="使用者員編不能為空" Display="Dynamic" ControlToValidate="txtUserID">*</asp:RequiredFieldValidator> 
                                <cc1:AjaxValidator id="ajvUserID" runat="server" ErrorMessage="使用者員編已經存在" ControlToValidate="txtUserID" ClientValidationFunction="AjaxValidatorFunction" OnOnAjaxValidatorQuest="AjaxValidator_UserID_OnAjaxValidatorQuest">使用者員編已經存在</cc1:AjaxValidator>
                                </td>
                       </tr>
                       <tr>
                            <td align="right" >
                                <asp:Label id="Label_UserName" runat="server" ><font color="red">*</font>使用者姓名：</asp:Label>
                            </td>
                            <td align="left" >
                                <asp:TextBox id="txtUserName" runat="server" MaxLength="128"  Enabled="false"></asp:TextBox>
                                </td>
                        </tr>
                        <tr>
                            <td align="right" >
                                <font color="red">*</font>電子郵件：
                            </td>
                            <td align="left" >
                                <asp:TextBox id="txtEMail" runat="server" MaxLength="100" Enabled="false"></asp:TextBox>
                                </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <asp:UpdatePanel id="UpdatePanel1" runat="server">
                    <contenttemplate>
                    <table border="0" width="100%">
	                    <tr>
	                        <td width="15%" align="right" >  
                                角色：
                            </td>
                            <td align="left">
                                <cc1:MoveListBox ID="mlbRole" runat="server" Width="137px" Height="180px" DataTextField="RoleName" DataValueField="RoleID" >
                                </cc1:MoveListBox>
                            </td>
                        </tr>
	                </table>
	                </contenttemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button CssClass="btn" ID="btnSubmit" runat="server" Text="確定" OnClientClick="return CheckValide()" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                    <input id="btnCancel" class="btn" onclick="returnform('BaseInfo042.aspx')" type="button"
                        value="取消" />
                </td>
            </tr>
        </table>
        <script>
            function UserIDChange()
            {
                document.getElementById("txtUserName").value="";
                document.getElementById("txtEMail").value="";
            }
        
            function CheckValide()
            {
                if(!CheckClientValidate())
                {
                    return false;
                }
                if(document.getElementById("txtEMail").value=="")
                {
                    alert("電子郵件不能為空");
                    return false;
                }
                return true;
            }
        </script>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
