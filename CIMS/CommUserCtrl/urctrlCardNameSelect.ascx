<%@ Control Language="C#" AutoEventWireup="true" CodeFile="urctrlCardNameSelect.ascx.cs" Inherits="CommUserCtrl_urctrlCardNameSelect" %>
<table width="100%" border="0" cellpadding="0" cellspacing="0">
    <tr valign="baseline">
        <td align="right" width="10%" style="height: 24px">
            用途：
        </td>
        <td style="height: 24px">
            <asp:DropDownList ID="dropCard_Purpose" runat="server" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList>
        群組名稱：
            <asp:DropDownList ID="dropCard_Group" runat="server" OnSelectedIndexChanged="dropCard_Group_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList>
            版面簡稱：
            <asp:DropDownList ID="dropCard_Type" runat="server">
            </asp:DropDownList>
            </td>
    </tr>
</table>