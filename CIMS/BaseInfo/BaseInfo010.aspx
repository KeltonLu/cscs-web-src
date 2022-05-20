<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo010.aspx.cs" Inherits="BaseInfo_BaseInfo010" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡片管理設定-庫存公式設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">卡片管理設定-庫存公式設定</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:GridViewPageingByDB ID="gvpbFormula" runat="server" AllowPaging="True" OnOnSetDataSource="gvpbFormula_OnSetDataSource"
                            AllowSorting="false" CssClass="GridView" AutoGenerateColumns="False" OnRowDataBound="gvpbFormula_RowDataBound"
                            ColumnsHeaderText="">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <RowStyle CssClass="GridViewRow" />
                            <Columns>
                                <asp:TemplateField HeaderText="參數名稱 ">
                                    <itemstyle horizontalalign="Left" />
                                    <itemtemplate>
                                    <asp:TextBox id="txtParam_Name" runat="server" onkeyup="LimitLengthCheck(this, 30)" MaxLength="30"></asp:TextBox> 
                                   </itemtemplate>
                                </asp:TemplateField>                                   
                                <asp:TemplateField HeaderText="狀態項目 ">
                                    <itemstyle horizontalalign="Left" />
                                    <itemtemplate>
                            <table id="tbStatus" runat="server" />                                                     
                        </itemtemplate>
                                </asp:TemplateField>
                            </Columns>
                        </cc1:GridViewPageingByDB>
                    </td>
                </tr>
                
                <tr>
                    <td align="right">
                        <asp:Button ID="btnWatch" runat="server" OnClick="btnWatchClick" Text="公式預覽" CssClass="btn" />
                    </td>
                </tr>
            </table>
            <table class="GridView" id="result" runat="server"  border="1" cellspacing="0" style="width:100%;border-collapse:collapse;" visible="false">
            
            <tr style="color:Black;background-color:#B9BDAA;font-size:Small;"  >
            <th style="height: 16px">參數名稱</th>
            <th style="height: 16px">公式</th></tr>
            </table>
            <table style="width:100%;border-collapse:collapse;">
                <tr>
                    <td align="right">
                        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_click" Text="確定" CssClass="btn" />
                        &nbsp;&nbsp;
                        <input class="btn" onclick="returnform('BaseInfo010.aspx')" id="btnCancel" type="button"
                            value="取消" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
