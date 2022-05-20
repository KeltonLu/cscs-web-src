<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo042.aspx.cs" Inherits="BaseInfo_BaseInfo042" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>使用者維護</title>
    <link href="../css/Global.css"  rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%" >
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">使用者維護</font>
                </td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr valign="baseline">
                            <td style="width:15%" align="right">
                                使用者員編：
                            </td>
                            <td style="width:15%" align="left">
                                <asp:TextBox ID="txtUserID" runat="server" Width="100px" ></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtUserID"
                                    Display="Dynamic" ErrorMessage="使用者員編必須是英文或數字" ValidationExpression="^(\w*|\d*)$"></asp:RegularExpressionValidator>
                            </td>
                            <td style="width:15%" align="right">
                                使用者姓名：</td>
                            <td style="width:15%" align="left">
                                <asp:TextBox ID="txtUserName" runat="server" Width="100px" MaxLength="128"></asp:TextBox>
                            </td>
                            <td style="width:15%" align="right"> 
                                角色：
                            </td>
                            <td align="left"> 
                                <asp:DropDownList ID="dropRole" runat="server" DataTextField="RoleName" DataValueField="RoleID">
                                </asp:DropDownList>
                            </td>
                            
                        </tr>
                        <tr>
                            <td align="right" colspan="6">
                                <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增"  OnClick="btnAdd_Click" Height="25px" />
                                <asp:Button CssClass="btn"  ID="btnLDAP" runat="server" Text="LDAP資料同步" Height="25px" OnClick="btnLDAP_Click" Width="100px" />&nbsp;&nbsp;
                                <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" Height="25px" />&nbsp;&nbsp;
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel id="UpdatePanel1" runat="server">
                    <contenttemplate>
                    <table width="100%">
                        <tr>
                            <td>
                                 <cc1:GridViewPageingByDB ID="gvpbUsers" runat="server" AllowPaging="True"
                                    DataKeyNames="UserID" OnOnSetDataSource="gvpbUsers_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbUsers_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                         <asp:TemplateField HeaderText="使用者員編" SortExpression="UserID">
                                            <itemstyle width="20%" />
                                            <itemtemplate>
                                                <asp:HyperLink id="hlUserID" runat="server"></asp:HyperLink>
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="UserName" HeaderText="使用者姓名" SortExpression="UserName">
                                            <itemstyle width="30%" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="角色" >
                                            <itemstyle width="15%" />
                                            <itemtemplate>
                                                <asp:Label id="lblRoleName" runat="server"></asp:Label> 
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Email" HeaderText="電子郵件" SortExpression="Email" />
                                        <asp:TemplateField HeaderText="刪除" >
                                            <itemstyle horizontalalign="Center" width="100px" />
                                            <itemtemplate>
                                                <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="ibtnDelete_Command"></asp:ImageButton>
                                            
</itemtemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </cc1:GridViewPageingByDB>
                            </td>
                        </tr>
                    </table>
                    </contenttemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
