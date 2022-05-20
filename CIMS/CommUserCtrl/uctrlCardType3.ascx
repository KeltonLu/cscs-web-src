<%@ Control Language="C#" AutoEventWireup="true" CodeFile="uctrlCardType3.ascx.cs" Inherits="CommUserCtrl_uctrlCardType3" %>
<Script language="javascript">
    function Choice()
    {
        var chkGroup = document.getElementById("UctrlCardType3_1_rdoGroup");
        var chkCard = document.getElementById("UctrlCardType3_1_rdoCard");
        if (chkGroup.checked)
        {
            document.getElementById("CardArea").style.display="none";
        }
        if (chkCard.checked)
        {
            document.getElementById("CardArea").style.display="table-row";
        }
    }
</Script>
<table width="100%" border="0" cellpadding="0" cellspacing="0">
    <tr valign="baseline">
        <td align="right" style="width: 80px; height: 25px;">
            <font color=red>*</font>年：</td>
        <td style="width: 8%; height: 25px;">
            <asp:DropDownList ID="dropYear" runat="server">
            </asp:DropDownList></td>
        <td align="right" style="width: 8%; height: 25px;">
            </td>
        <td align="left" style="height: 25px">
            </td>
    </tr>
    <tr valign="baseline">
        <td align="right" style="width: 80px; height: 25px">
            <span style="color: #ff0000">*</span>選擇方式：</td>
        <td align="left" colspan="3" style="height: 25px">
            &nbsp;<asp:RadioButton ID="rdoGroup" runat="server" GroupName="ChoiceType" Text="群組" Checked="True" OnCheckedChanged="rdoGroup_CheckedChanged" AutoPostBack="True" />
            <asp:RadioButton ID="rdoCard" runat="server" GroupName="ChoiceType" Text="卡種" OnCheckedChanged="rdoCard_CheckedChanged" AutoPostBack="True"/></td>
    </tr>
    <tr valign="baseline">
        <td align="right" style="width: 80px; height: 25px;">
            <span style="color: #ff0000">*</span>用途：
        </td>
        <td style="width: 8%; height: 25px;">
            <asp:DropDownList ID="dropCard_Purpose" runat="server" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList></td>
        <td align="right" style="width: 8%; height: 25px;">
            <span style="color: #ff0000">*</span>群組名稱：</td>
        <td align="left" style="height: 25px"> 
            <asp:DropDownList ID="dropCard_Group" runat="server" OnSelectedIndexChanged="dropCard_Group_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList></td>
    </tr>
    <tr valign="baseline" height="20px">
        <td colspan="4" style="height: 7px">&nbsp;</td>
    </tr>
    <tr valign="baseline">
        <td style="width: 80px"></td>
        <td colspan="3">
        <asp:Panel ID="plBox" runat="server"  Width="125px" Visible="False">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center">
                        <asp:Label ID="lblLeftName" runat="server" Text="可選擇卡種"></asp:Label></td>
                    <td></td>
                    <td align="center">
                        <asp:Label ID="lblRightName" runat="server" Text="已選擇卡種"></asp:Label></td>
                </tr>
                <tr>
                    <td style="height: 120px">
                        <asp:ListBox ID="LbLeft" SelectionMode="multiple" runat="server" Height="120px" Width="250px"></asp:ListBox>
                    </td>
                    <td align="center" style="width:50px; height: 120px;">
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnSelect" runat="server" Text=">" OnClick="btnSelect_Click" /><br />
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnRemove" runat="server" Text="<" OnClick="btnRemove_Click" /><br />
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnSelectAll" runat="server" Text=">>" OnClick="btnSelectAll_Click" /><br />
	                    <asp:Button Width="30px" CssClass="btn" CausesValidation="false" ID="btnRemoveAll" runat="server" Text="<<" OnClick="btnRemoveAll_Click" />
	                </td>
                    <td style="height: 120px">
                        <asp:ListBox ID="LbRight" SelectionMode="multiple" runat="server" Height="120px" Width="250px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        </td>
        
    </tr>

</table>


