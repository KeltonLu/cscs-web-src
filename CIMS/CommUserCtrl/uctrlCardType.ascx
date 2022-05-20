<%@ Control Language="C#" AutoEventWireup="true" CodeFile="uctrlCardType.ascx.cs" Inherits="CommUserCtrl_uctrlCardType" %>
<table width="100%" border="0" cellpadding="0" cellspacing="0">
    <tr valign="baseline">
        <td align="right" width="15%">
            用途：
        </td>
        <td width="15%">
            <asp:DropDownList ID="dropCard_Purpose" runat="server" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList></td>
        <td width="15%" align="right">群組名稱：</td>
        <td align="left"> 
            <asp:DropDownList ID="dropCard_Group" runat="server" OnSelectedIndexChanged="dropCard_Group_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList></td>
    </tr>
    <tr valign="baseline" height="20px">
        <td colspan="4">&nbsp;</td>
    </tr>
    <tr valign="baseline">
        <td></td>
        <td colspan="3">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center">
                        <asp:Label ID="lblLeftName" runat="server" Text="可選擇卡種"></asp:Label></td>
                    <td></td>
                    <td align="center">
                        <asp:Label ID="lblRightName" runat="server" Text="已選擇卡種"></asp:Label></td>
                </tr>
                <tr>
                    <td>
                        <asp:ListBox ID="LbLeft" SelectionMode="multiple" runat="server" Height="120px" Width="250px"></asp:ListBox>
                    </td>
                    <td align="center" style="width:50px">
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnSelect" runat="server" Text=">" OnClick="btnSelect_Click" /><br />
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnRemove" runat="server" Text="<" OnClick="btnRemove_Click" /><br />
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnSelectAll" runat="server" Text=">>" OnClick="btnSelectAll_Click" /><br />
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnRemoveAll" runat="server" Text="<<" OnClick="btnRemoveAll_Click" />
	                </td>
                    <td>
                        <asp:ListBox ID="LbRight" SelectionMode="multiple" runat="server" Height="120px" Width="250px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>