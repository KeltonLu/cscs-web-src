<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="BaseInfo002Card.aspx.cs"
    Inherits="BaseInfo_BaseInfo002Card" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡種/價格設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <style type="text/css">
    .Header
    {
        color:Black;background-color:#B9BDAA;font-size:Small;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
          
                    <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                        <tr style="height: 20px">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr valign="baseline">
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                                    <td>
                                        <font class="PageTitle">卡種/價格設定</font>
                                    </td>
                                    <td align="right">
                                        <asp:Button CssClass="btn" ID="btnSubmitUp" runat="server" Text="確定" OnClick="btnSubmitUp_Click" />&nbsp;&nbsp;
                                        <asp:Button CssClass="btn" ID="btnCancelUp" runat="server" Text="取消" CausesValidation="False"
                                            OnClick="btnCancelUp_Click" />
                                    </td>
                                </table>
                            </td>
                        </tr>
                        <tr class="tbl_row">
                            <td>
                                <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <span class="style1">*</span>卡種組合：
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtGroup_Name" onblur="LimitLengthCheck(this,30)" onkeyup="LimitLengthCheck(this,30)"
                                                MaxLength="30" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtGroup_Name"
                                                ErrorMessage="卡種組合不能為空">*</asp:RequiredFieldValidator>
                                            <cc1:AjaxValidator ID="AjaxValidator1" runat="server" ControlToValidate="txtGroup_Name"
                                                ErrorMessage="卡種組合不能重複" OnOnAjaxValidatorQuest="AjaxValidator1_OnAjaxValidatorQuest"></cc1:AjaxValidator></td>
                                    </tr>
                                    <!--<tr valign="baseline">
                                        <td align="right">
                                            選擇此合約卡種
                                        </td>
                                        <td>
                                        </td>
                                    </tr>-->
                                    <tr valign="baseline">
                                        <td align="left" colspan="2">
                                            <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                                <tr>
                                                    <td>
                                                        <uc1:uctrlCardType ID="UctrlCardType1" runat="server"  />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class="tbl_row">
                            <td>
                                <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                    <tr valign="baseline">
                                        <td style="width: 10%">
                                            &nbsp;</td>
                                        <td align="left" style="width: 80px">
                                            <asp:RadioButtonList ID="radlType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="radlType_SelectedIndexChanged">
                                                <asp:ListItem Value="1" Selected="True">基本價格</asp:ListItem>
                                                <asp:ListItem Value="2">級距價格</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td align="left" valign="top">
                                                        <%--Dana 20161021 最大長度由11改為16 --%>
                                            $<asp:TextBox ID="txtBase_Price" onfocus="DelDouhao(this)" Style="ime-mode: disabled;
                                                text-align: right" onblur="CheckAmt('txtBase_Price',8,2);value=GetValue(this.value)"
                                                onkeyup="CheckAmt('txtBase_Price',8,2)" MaxLength="16" runat="server" Width="90px">0.00</asp:TextBox></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class="tbl_row" id="trLevel" runat="server">
                            <td>
                                <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                    <tr>
                                        <td>
                                            <asp:GridView ID="gvpbCardType" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvpbCardType_RowDataBound"
                                                Width="100%">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Name" HeaderText="卡種">
                                                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="合約級距類別">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="dropParam_RID" runat="server" DataValueField="RID" DataTextField="Param_Name"
                                                                AutoPostBack="True" OnSelectedIndexChanged="dropParam_RID_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <table cellspacing="0" cellpadding="0" border="1" style="width: 100%;border-collapse: collapse;">
                                                        <tr style="color: Black; background-color: #B9BDAA; font-size: Small;">
                                                            <th scope="col" style="width: 20%;">
                                                                卡種</th>
                                                            <th scope="col">
                                                                合約級距類別</th>
                                                        </tr>
                                                    </table>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:GridView ID="gvpbLevel" runat="server" Width="100%" OnRowDataBound="gvpbLevel_RowDataBound"
                                                AutoGenerateColumns="False">
                                                <HeaderStyle BackColor="#B9BDAA" HorizontalAlign="Center" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <RowStyle CssClass="GridViewRow" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button CssClass="btn" ID="btnAddLevel" CausesValidation="false" runat="server"
                                                Text="+" OnClick="btnAddLevel_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            *級距=MAX代表最大
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
                        <tr class="tbl_row">
                            <td>
                                <cc1:GridViewPageingByDB ID="gvpbMaterial" runat="server" OnOnSetDataSource="gvpbMaterial_OnSetDataSource"
                                    AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                    PreviousPageText="<" OnRowDataBound="gvpbMaterial_RowDataBound"
                                    ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                    EditPageUrl="#">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                        PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="特殊材質">
                                            <itemstyle width="20%" />
                                            <itemtemplate>
<asp:LinkButton id="lbtnMaterial" runat="server" CausesValidation="false" OnCommand="lbtnMaterial_Click" __designer:wfdid="w2"></asp:LinkButton> 
</itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Base_Price" HeaderText="單價" DataFormatString="{0:N2}"
                                            HtmlEncode="False">
                                            <itemstyle horizontalalign="Right" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="刪除">
                                            <itemstyle horizontalalign="Center" width="100px" />
                                            <itemtemplate>
                                    <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDeleteMaterial" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDeleteMaterial_Command"></asp:ImageButton>
                                
</itemtemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <table cellspacing="0" cellpadding="0" border="1" style="width: 100%;border-collapse: collapse;">
                                            <tr style="color: Black; background-color: #B9BDAA; font-size: Small;">
                                                <th scope="col" style="width: 20%;">
                                                    特殊材質</th>
                                                <th scope="col">
                                                    單價</th>
                                                <th scope="col" style="width: 100px;">
                                                    刪除</th>
                                            </tr>
                                        </table>
                                    </EmptyDataTemplate>
                                </cc1:GridViewPageingByDB>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Button ID="btnMaterialAdd" CausesValidation="false" CssClass="btn" runat="server"
                                    Text="新增材質"  OnClick="btnMaterialAdd_Click" /><asp:Button  style="display: none;"  ID="btnMaterialBind" CausesValidation="false" 
                                        runat="server" Text="Button" OnClick="btnMaterialBind_Click" /></td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Button CssClass="btn" ID="btnSubmitDn" runat="server" Text="確定" OnClick="btnSubmitUp_Click" />&nbsp;&nbsp;
                                <asp:Button CssClass="btn" ID="btnCancelDn" runat="server" Text="取消" CausesValidation="False"
                                    OnClick="btnCancelUp_Click" />
                            </td>
                        </tr>
                    </table>
               

            <script type="text/javascript">
            function AddMaterial()
            {
                __doPostBack('btnMaterialBind',''); 
            }
            
            function AddLevel()
            {
                __doPostBack('btnLevelBind',''); 
            }
           
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
