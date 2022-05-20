<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository009.aspx.cs" Inherits="Depository_Depository009" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>物料年度預算控管作業</title>
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
                        <font class="PageTitle">物料年度預算控管作業</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="tbl_row" width="100%" cellpadding="0" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    <span class="style1">*</span>年度：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBeginTime" onblur="CheckNumWithNoId(this,4);" onkeyup="CheckNum('txtBeginTime',4)" runat="server"
                                        MaxLength="4"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBeginTime"
                                        ErrorMessage="年度不能為空！">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtBeginTime"
                                        ErrorMessage="年度必須為4位整數" ValidationExpression="\d{4}">*</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" colspan="2">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />&nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline" runat="server" id="trList">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbYearBudget" runat="server" AllowPaging="True"
                                                OnOnSetDataSource="gvpbYearBudget_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                OnRowDataBound="gvpbYearBudget_RowDataBound" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="param_name" HeaderText="類別">
                                                        <itemstyle horizontalalign="Left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField>
                                                        <itemstyle horizontalalign="Center" />
                                                        <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
<asp:TextBox onblur="CheckNumWithNoId(this,9);value=GetValue(this.value)" style="IME-MODE: disabled; TEXT-ALIGN: right" id="txtbudget" onfocus="DelDouhao(this)" onkeyup="CheckNumWithNoId(this,9)" runat="server" maxlength="11" AutoPostBack="True" OnTextChanged="txtbudget_update" __designer:wfdid="w5"></asp:TextBox> 
</itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Used" DataFormatString="{0:N2}" HtmlEncode="False">
                                                        <itemstyle horizontalalign="Right" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="剩餘預算">
                                                        <itemstyle horizontalalign="Right" width="25%" />
                                                        <itemtemplate>
<asp:Label id="lblRemainBudget" runat="server"  __designer:wfdid="w3"></asp:Label> 
</itemtemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnCommit" runat="server" Text="確定" Height="25px" class="btn" OnClick="btnCommit_Click" />
                        &nbsp;
                        <asp:Button ID="btnExcel" runat="server" Text="匯出EXCEL格式" Height="25px" class="btn" OnClick="btnExcel_Click" />
                        &nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="取消" Height="25px" class="btn" OnClick="btnCancel_Click" />
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
