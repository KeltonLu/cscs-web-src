<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository002Query.aspx.cs"
    Inherits="Depository_Depository002Query" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>採購下單查詢</title>
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
                        <font class="PageTitle">採購下單查詢</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td align="right" style="width: 15%; text-align: right;">
                                    訂單流水編號：</td>
                                <td style="width: 37%">
                                    <asp:TextBox ID="txtBID" runat="server" MaxLength="8" Width="100px" Style="ime-mode: disabled;"
                                        onkeyup="BIDChange()" onblur="BIDChange()" />&nbsp;-&nbsp;
                                    <asp:TextBox ID="txtMID" runat="server" MaxLength="2" Width="30px" Style="ime-mode: disabled;"
                                        onblur="Check(this)" onkeyup="MIDChange()" />&nbsp;-&nbsp;
                                    <asp:TextBox ID="txtEID" runat="server" MaxLength="2" Width="30px" Style="ime-mode: disabled;"
                                        onblur="Check(this)" onkeyup="CheckNum('txtEID',2)" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtBID"
                                        ErrorMessage="流水編號格式不正確!" ValidationExpression="\d{8}">*</asp:RegularExpressionValidator></td>
                                <td style="width: 10%" align="right">
                                    預算：
                                </td>
                                <td colspan="2" style="width: 150px">
                                    <asp:DropDownList ID="dropBudget" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    訂購日：
                                </td>
                                <td style="width: 37%">
                                    <asp:TextBox ID="txtVALID_DATE_FROM" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_FROM')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtVALID_DATE_TO" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_TO')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    &nbsp;
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtVALID_DATE_FROM"
                                        ControlToValidate="txtVALID_DATE_TO" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td style="width: 15%" align="right">
                                    合約：
                                </td>
                                <td colspan="2" style="width: 150px">
                                    <asp:DropDownList ID="dropAgreement" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" style="width: 6%">
                                    預訂交貨日：
                                </td>
                                <td style="width: 37%">
                                    <asp:TextBox ID="txtFore_Delivery_BDate" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtFore_Delivery_BDate')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtFore_Delivery_EDate" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtFore_Delivery_EDate')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    &nbsp;
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="txtFore_Delivery_BDate"
                                        ControlToValidate="txtFore_Delivery_EDate" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right">
                                    交貨地點：
                                </td>
                                <td colspan="2" style="width: 150px">
                                    <asp:DropDownList ID="dropFactory_ShortName_CN" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    空白卡廠：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropBlankFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    訂單結案狀態：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropCase_Status" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="radltype" runat="server" RepeatColumns="2">
                                        <asp:ListItem Selected="True">訂單</asp:ListItem>
                                        <asp:ListItem>訂單流水編號</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="5">
                                    <uc1:urctrlCardTypeSelect ID="UrctrlCardTypeSelect" runat="server" />
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="10" align="right">
                                    <asp:Button class="btn" runat="server" ID="query" Text="查詢" OnClick="query_Click" />&nbsp;&nbsp;
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
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table id="Table1" width="100%">
                                                <tr>
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbORDERFORMDETAIL" runat="server" AllowPaging="True" OnOnSetDataSource="gvpbORDERFORMDETAIL_OnSetDataSource" AllowSorting="True"
                                                            AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                            PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbORDERFORMDETAIL_RowDataBound"
                                                            SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                            EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <Columns>
                                                                <asp:BoundField DataField="OrderForm_Detail_RID" HeaderText="訂單流水編號" />
                                                                <asp:BoundField DataField="Space_Short_RID" HeaderText="卡種" />
                                                                <asp:BoundField DataField="Number" HeaderText="數量">
                                                                    <itemstyle width="4%" horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SumTotal" HeaderText="已下單未到貨數">
                                                                    <itemstyle width="4%" horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="IDate" HeaderText="入庫日" />
                                                                <asp:BoundField HeaderText="入庫量" />
                                                                <asp:BoundField DataField="Budget_Name" HeaderText="預算" />
                                                                <asp:BoundField DataField="Agreement_Name" HeaderText="合約" />
                                                                <asp:BoundField DataField="Change_Unitprice" HeaderText="合約單價(含稅)">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Unit_Price" HeaderText="調整後單價(含稅)">
                                                                    <itemstyle forecolor="Red" horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="fsc" HeaderText="空白卡廠" />
                                                                <asp:BoundField DataField="Fore_Delivery_Date" HeaderText="預訂交貨日" DataFormatString="{0:N0}" />
                                                                <asp:BoundField HeaderText="交貨剩餘天數" >
                                                                    <itemstyle wrap="False" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="入庫結案狀態">
                                                                    <itemtemplate>
                                                            <asp:Label id="lblCase_Status" runat="server"/>                                            
                                                        
</itemtemplate>
                                                                    <itemstyle width="8%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Wafer_Name" HeaderText="晶片名稱" />
                                                                <asp:TemplateField HeaderText="緊急程度">
                                                                    <itemtemplate>
                                                            <asp:Label id="lblis_exigence" runat="server"/>                                            
                                                        
</itemtemplate>
                                                                    <itemstyle width="8%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="factory_shortname_cn" HeaderText="交貨地點" />
                                                                <asp:BoundField DataField="Comment" HeaderText="備註" />
                                                            </Columns>
                                                        </cc1:GridViewPageingByDB>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnExcel" runat="server" class="btn" OnClick="btnExcel_Click" Text="匯出EXCEL格式" />&nbsp;&nbsp;
                    </td>
                </tr>
            </table>

            <script language="javascript" type="text/javascript">            
            CheckNum('txtBID',8);
            document.getElementById("txtMID").disabled=true;
            document.getElementById("txtEID").disabled=true;            
            MIDChange();
            BIDChange();
            
            function BIDChange()
            {
                if(document.getElementById("txtBID").value!="")
                {
                    
                    document.getElementById("txtMID").disabled=false;
                    document.getElementById("txtEID").disabled=false;
                }
                else
                {
                    document.getElementById("txtEID").value="";                    
                    document.getElementById("txtEID").disabled=true; 
                    document.getElementById("txtMID").value="";                    
                    document.getElementById("txtMID").disabled=true;                      
                }
            }
            
            function MIDChange()
            {
                CheckNum('txtMID',2);
                if(document.getElementById("txtMID").value!="")
                {
                    document.getElementById("txtEID").disabled=false;
                }
                else
                {
                    document.getElementById("txtEID").value="";                    
                    document.getElementById("txtEID").disabled=true;                    
                }
            }
            
            function Check(obj)
            {
                var num=obj.value;
                if(num!="")
                {
                    if(num.length==1)
                    {
                        obj.value=0+num;
                    }
                }
                MIDChange();
            }
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
