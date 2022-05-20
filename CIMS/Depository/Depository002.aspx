<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository002.aspx.cs" Inherits="Depository_Depository002" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>採購下單作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
        
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">採購下單作業</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    訂單編號：
                                </td>
                                <td width="35%">
                                    <asp:TextBox ID="txtOrderForm_RID_B" runat="server" size="8" MaxLength="8" Style="ime-mode: disabled;" onkeyup="OrderForm_RID_BChange()" onblur="OrderForm_RID_BChange()"/>&nbsp;-
                                    <asp:TextBox ID="txtOrderForm_RID_E" runat="server" size="2" MaxLength="2" Style="ime-mode: disabled;" onblur="CheckOrderForm_RID_E(this)"/><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                    ControlToValidate="txtOrderForm_RID_B" ErrorMessage="流水編號格式不正確!" ValidationExpression="\d{8}">*</asp:RegularExpressionValidator>
                                </td>
                                <td width="10%" align="right">
                                    訂購日：
                                </td>
                                <td align="left" style="height: 24px">
                                    <asp:TextBox ID="txtOrder_Date_FROM" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtOrder_Date_FROM')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtOrder_Date_TO" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtOrder_Date_TO')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    &nbsp;
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtOrder_Date_FROM"
                                        ControlToValidate="txtOrder_Date_TO" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator></td>
                            </tr>
                            <tr valign="baseline">                                
                                <td width="10%" align="right">
                                    放行日：
                                </td>
                                <td align="left" style="height: 24px">
                                    <asp:TextBox ID="txtPass_Date_FROM" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtPass_Date_FROM')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtPass_Date_TO" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtPass_Date_TO')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    &nbsp;
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="txtPass_Date_FROM"
                                        ControlToValidate="txtPass_Date_TO" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td width="10%" align="right">
                                    放行狀態：
                                </td>
                                <td width="35%">
                                    <asp:DropDownList ID="drpPass_Status" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="6" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button Text="新增" ID="btnAdd" class="btn" runat="server" OnClick="btnAdd_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="height: 15px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table id="queryResult" width="100%" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table id="Table1" width="100%">
                                                <cc1:GridViewPageingByDB ID="gvpbORDERFORM" runat="server" AllowPaging="True" DataKeyNames="RID"
                                                    OnOnSetDataSource="gvpbORDERFORM_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                    FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                    OnRowDataBound="gvpbORDERFORM_RowDataBound" SortAscImageUrl="~/images/asc.gif"
                                                    SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                    EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <Columns>
                                                        <asp:HyperLinkField DataTextField="OrderForm_RID" HeaderText="訂單編號" SortExpression="OrderForm_RID"
                                                            DataNavigateUrlFields="OrderForm_RID" DataNavigateUrlFormatString="Depository002Mod.aspx?ActionType=Edit&RID={0}">
                                                            <itemstyle horizontalalign="Left" width="25%" />
                                                        </asp:HyperLinkField>
                                                        <asp:BoundField DataField="Order_Date" HeaderText="訂購日" SortExpression="Order_Date"/>
                                                        <asp:BoundField DataField="Pass_Date" HeaderText="放行日" SortExpression="Pass_Date" />
                                                        <asp:BoundField DataField="Param_Name" HeaderText="放行狀態" SortExpression="Param_Name" />
                                                    </Columns>
                                                </cc1:GridViewPageingByDB>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <script language="javascript" type="text/javascript">
            document.getElementById("txtOrderForm_RID_E").disabled=true;
            CheckNum('txtOrderForm_RID_E',2);
            CheckNum('txtOrderForm_RID_B',8);
            OrderForm_RID_BChange();
            
            function OrderForm_RID_BChange()
            {
                if(document.getElementById("txtOrderForm_RID_B").value!="")
                {
                    
                    document.getElementById("txtOrderForm_RID_E").disabled=false;
                }
                else
                {
                    document.getElementById("txtOrderForm_RID_E").value="";                    
                    document.getElementById("txtOrderForm_RID_E").disabled=true;                   
                }
            }
            
            function CheckOrderForm_RID_E(obj)
            {
                var num=obj.value;
                if(num!="")
                {
                    if(num.length==1)
                    {
                        obj.value=0+num;
                    }
                }
                CheckNum('txtOrderForm_RID_E',2);
            }
        </script>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
