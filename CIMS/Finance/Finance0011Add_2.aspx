<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0011Add_2.aspx.cs"
    Inherits="Finance_Finance0011Add_2" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款放行作業新增</title>
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
                    <td style="height: 23px">
                        <font class="PageTitle">請款放行作業新增</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <asp:Button ID="btnOK1" runat="server" class="btn" OnClick="btnOK1_Click"
                            Text="提交" />&nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel1"
                            runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <span class="style1">*</span> SAP單號：
                        <asp:TextBox ID="txtSAP_Serial_Number" runat="server" style="ime-mode:disabled;" MaxLength="15" onblur="LimitLengthCheck(this,15)" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSAP_Serial_Number"
                            ErrorMessage="SAP單號不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtSAP_Serial_Number"
                            ErrorMessage="SAP單號只能輸入數字及字母" ValidationExpression="([A-Z]|[a-z]|[0-9])*">*</asp:RegularExpressionValidator></td>
                </tr>
                <tr>
                    <td colspan="2">
                     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                        <table width="100%">
                            <tr>
                                <td colspan="16">
                                    <cc1:GridViewPageingByDB ID="gvpbRequisitionWork" runat="server"
                                        AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>"
                                        NextPageText=">" PreviousPageText="<" CssClass="GridView" SortAscImageUrl="~/images/asc.gif"
                                        SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                        EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" OnOnSetDataSource="gvpbRequisitionWork_OnSetDataSource"
                                        OnRowDataBound="gvpbRequisitionWork_RowDataBound" Font-Underline="False">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                                <itemstyle horizontalalign="Left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="空白卡厰" DataField="Factory_ShortName_CN"></asp:BoundField>
                                            <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號"></asp:BoundField>
                                            <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號"></asp:BoundField>
                                            <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號 "></asp:BoundField>
                                            <asp:BoundField DataField="Operate_Type" HeaderText="進貨作業" HtmlEncode="False">
                                                <itemstyle horizontalalign="Left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <%--add chaoma by 201005515-0 start--%>
                                            <asp:BoundField DataField="Number" HeaderText="訂單數量 " DataFormatString="{0:N0}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="RIC_Number" HeaderText="入庫數量 " DataFormatString="{0:N0}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <%--add chaoma end--%> 
                                            <asp:BoundField DataField="Income_Date" HeaderText="日期">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="含稅單價">
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 10 改為 11 --%>
                                                    <asp:TextBox onfocus="DelDouhao(this)"  onblur="CheckAmtWithNoId(this,5,4);value=GetValue(this.value)" 
                                                        id="txtUnit_Price"  style="ime-mode:disabled;text-align: right" MaxLength="11"
                                                        onkeyup="CheckAmtWithNoId(this,5,4)" 
                                                        OnTextChanged="txtUnit_Price_TextChanged" 
                                                        runat="server" AutoPostBack="True" Width="100px" __designer:wfdid="w21" Enabled="false">
                                                    </asp:TextBox> 
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="未稅單價" />
                                            <asp:TemplateField HeaderText="實際請款數量">
                                                 <itemstyle horizontalalign="Right"></itemstyle>
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                    <asp:TextBox onfocus="DelDouhao(this)"  onblur="CheckNumWithNoId1(this,9);value=GetValue(this.value)" 
                                                        id="txtRequisition_Count" style="ime-mode:disabled;text-align: right" MaxLength="11"
                                                        onkeyup="CheckNumWithNoId1(this,9)" runat="server" 
                                                        OnTextChanged="txtRequisition_Count_TextChanged" 
                                                        AutoPostBack="True" Width="100px" __designer:wfdid="w29">
                                                    </asp:TextBox>
                                                    <asp:Label id="lblSumRequisition_Count" 
                                                        runat="server" __designer:wfdid="w30">
                                                    </asp:Label> 
                                                
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="含稅總金額">
                                                 <itemstyle horizontalalign="Right"></itemstyle>
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                    <asp:TextBox onfocus="DelDouhao(this)" onblur="CheckNumWithNoId1(this,9);value=GetValue(this.value)" id="txtFineMoney" 
                                                        onkeyup="CheckNumWithNoId1(this,9)" runat="server" style="ime-mode:disabled;text-align: right" MaxLength="11"
                                                        OnTextChanged="txtFineMoney_TextChanged" AutoPostBack="True" 
                                                        Width="90px">
                                                    </asp:TextBox>
                                                    <asp:Label id="lblSumContanUnit_Price" runat="server"></asp:Label> 
                                                
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="未稅總金額" />
                                            <asp:BoundField HeaderText="遲交天數" >
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Check_ID" HeaderText="發票號碼" />
                                            <asp:TemplateField HeaderText="備註">
                                                <itemtemplate>
                                                    <asp:TextBox id="txtComment" runat="server" __designer:wfdid="w25"></asp:TextBox> 
                                                
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
                    <td align="right" colspan="2">
                        <asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <asp:Button ID="btnOK2" runat="server" class="btn" OnClick="btnOK1_Click"
                            Text="提交" />&nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel2"
                            runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
