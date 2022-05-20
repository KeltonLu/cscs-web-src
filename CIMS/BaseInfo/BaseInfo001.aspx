<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo001.aspx.cs" Inherits="BaseInfo_BaseInfo001" %>
<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect" TagPrefix="uc2" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>預算簽呈查詢</title>
    <link href="../css/Global.css"  rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel id="UpdatePanel1" runat="server">
        <contenttemplate>
        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%" >
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">預算管理作業</font>
                </td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <table border="0" width="100%" cellpadding="0" cellspacing="2">
                        <tr valign="baseline">
                            <td style="width:15%; height: 24px;" align="right">
                                簽呈文號：
                            </td>
                            <td style="width:20%; height: 24px;" align="left">
                                <asp:TextBox ID="txtBUDGET_ID" runat="server"  MaxLength="30" ></asp:TextBox>&nbsp;
                            </td>
                            <td style="width:20%; height: 24px;" align="right">
                                預算名稱：
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtBudget_Name" runat="server"  MaxLength="30" ></asp:TextBox>&nbsp;
                            </td>
                           
                        </tr>
                        <tr >
                            <td colspan="4">
                                <uc2:urctrlCardTypeSelect ID="UrctrlCardTypeSelect" runat="server" />
                            </td>
                        </tr>
                       <tr valign="baseline">
                             <td align="right"> 
                                有效期間：
                            </td>
                            <td align="left" style="height: 24px"  colspan="3">
                                <asp:TextBox ID="txtVALID_DATE_FROM" Width="80px" onfocus="WdatePicker()" runat="server"  MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_FROM')})" src="../images/calendar.gif" align="absmiddle">~
                                 <asp:TextBox ID="txtVALID_DATE_TO" Width="80px" onfocus="WdatePicker()" runat="server"  MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_TO')})" src="../images/calendar.gif" align="absmiddle">
                                &nbsp;
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtVALID_DATE_FROM"
                                    ControlToValidate="txtVALID_DATE_TO" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                    Type="Date">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="4">
                                <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                                <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增"  OnClick="btnAdd_Click" CausesValidation="False" />
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
                    <table width="100%">
                        <tr>
                            <td>
                                 <cc1:GridViewPageingByDB ID="gvpbBudget" runat="server" AllowPaging="True"
                                    DataKeyNames="RID" OnOnSetDataSource="gvpbBudget_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbBudget_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="簽呈文號" SortExpression="BUDGET_ID">
                                            <itemstyle width="20%" />
                                            <itemtemplate>
                                                <asp:HyperLink id="hlBUDGET_ID" runat="server"></asp:HyperLink><br />
                                                <asp:Label id="lbBUDGET_ID" runat="server"></asp:Label>
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="預算名稱" >
                                            <itemstyle width="20%" />
                                            <itemtemplate>
                                                <asp:Label id="lbBUDGET_NAME" runat="server"></asp:Label> 
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="TOTAL_CARD_AMT" HeaderText="總金額" DataFormatString="{0:N2}" HtmlEncode="False" >
                                            <itemstyle width="8%"　HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TOTAL_CARD_NUM" HeaderText="總卡數" DataFormatString="{0:N0}" HtmlEncode="False" >
                                            <itemstyle width="8%" HorizontalAlign="Right"/>
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="預算金額" >
                                            <itemstyle width="8%"　HorizontalAlign="Right" />
                                            <itemtemplate>
                                                <asp:Label id="lbCard_Price" runat="server"></asp:Label> 
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="剩餘金額" >
                                            <itemstyle width="8%"　HorizontalAlign="Right" />
                                            <itemtemplate>
                                                <asp:Label id="lbRemain_Card_Price" runat="server"></asp:Label> 
                                            </itemtemplate>
                                        </asp:TemplateField>    
                                        <asp:TemplateField HeaderText="預算卡數" >
                                            <itemstyle width="8%"　HorizontalAlign="Right" />
                                            <itemtemplate>
                                                <asp:Label id="lbCard_Num" runat="server"></asp:Label> 
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="剩餘卡數" >
                                            <itemstyle width="8%"　HorizontalAlign="Right" />
                                            <itemtemplate>
                                                <asp:Label id="lbRemain_Card_Num" runat="server"></asp:Label> 
                                            </itemtemplate>
                                        </asp:TemplateField>   
                                        <asp:BoundField DataField="VALID_DATE" HeaderText="有效期間" >
                                            <itemstyle width="16%" HorizontalAlign="left"/>
                                        </asp:BoundField>
                                    </Columns>
                                </cc1:GridViewPageingByDB>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        </contenttemplate>
        </asp:UpdatePanel>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
