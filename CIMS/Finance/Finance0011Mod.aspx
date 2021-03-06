<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0011Mod.aspx.cs" Inherits="Finance_Finance0011Mod" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款放行作業修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function delConfirm()
        {
             if (true == document.getElementById("chkDel").checked)
                {
                    return confirm('確認刪除此筆訊息?');
                }
        }        
 -->
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">請款放行作業修改/刪除</font>
                    </td>
                    <td align="right">
                        &nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" OnClientClick="return delConfirm();" />
                        <asp:Button ID="btnPrint1" runat="server" class="btn" OnClick="btnPrint1_Click" Text="列印" />&nbsp;<asp:Button
                            ID="btnOK1" runat="server" class="btn" OnClick="btnOK1_Click" Text="提交" />
                        <asp:Button ID="btnPass1" runat="server" class="btn" OnClick="btnPass2_Click" Text="放行" />
                        <asp:Button ID="btnUntread1" runat="server" class="btn" OnClick="btnUntread1_Click"
                            Text="退回" />&nbsp;<asp:Button ID="btnCancel1" runat="server" CausesValidation="False"
                                class="btn" OnClick="btnCancel_Click" Text="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        SAP單號：<asp:TextBox ID="txtSAP_Serial_Number" runat="server" style="ime-mode:disabled;" MaxLength="15" onblur="LimitLengthCheck(this,15)" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSAP_Serial_Number"
                            ErrorMessage="SAP單號不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtSAP_Serial_Number"
                            ErrorMessage="SAP單號只能輸入數字及字母" ValidationExpression="([A-Z]|[a-z]|[0-9])*">*</asp:RegularExpressionValidator>
                        </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        請款日期：<asp:TextBox ID="txtAsk_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtAsk_Date')})" src="../images/calendar.gif" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%">
                            <tr class="tbl_row">
                                <td colspan="16">
                                    <cc1:GridViewPageingByDB ID="gvpbRequisitionWork" runat="server" AllowPaging="False"
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
                                                <itemstyle horizontalalign="left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="含稅單價">
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 10 改為 11 --%>
                                                    <asp:TextBox onfocus="DelDouhao(this)"  onblur="CheckAmtWithNoId(this,5,4);value=GetValue(this.value)" 
                                                        id="txtUnit_Price" style="ime-mode:disabled;text-align: right"
                                                        onkeyup="CheckAmtWithNoId(this,5,4)" MaxLength="11" 
                                                        OnTextChanged="txtUnit_Price_TextChanged" 
                                                        runat="server" AutoPostBack="True" Width="100px" Enabled="false">
                                                    </asp:TextBox> 
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Unit_Price_No" HeaderText="未稅單價" />
                                            <asp:TemplateField HeaderText="實際請款數量">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                    <asp:TextBox onfocus="DelDouhao(this)"  onblur="CheckNumWithNoId1(this,9);value=GetValue(this.value)" MaxLength="11"
                                                        id="txtRequisition_Count" style="ime-mode:disabled;text-align: right" 
                                                        onkeyup="CheckNumWithNoId1(this,9)" runat="server" 
                                                        OnTextChanged="txtRequisition_Count_TextChanged" 
                                                        AutoPostBack="True" Width="100px">
                                                    </asp:TextBox>
                                                    <asp:Label id="lblSumRequisition_Count" 
                                                        runat="server">
                                                    </asp:Label> 
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="含稅總金額">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                    <asp:TextBox onfocus="DelDouhao(this)"  onblur="CheckNumWithNoId1(this,9);value=GetValue(this.value)" id="txtFineMoney" MaxLength="11" 
                                                        onkeyup="CheckNumWithNoId1(this,9)" runat="server" style="ime-mode:disabled;text-align: right" 
                                                        OnTextChanged="txtFineMoney_TextChanged" AutoPostBack="True" 
                                                        Width="100px">
                                                    </asp:TextBox>
                                                    <asp:Label id="lblSumContanUnit_Price" runat="server"></asp:Label> 
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="未稅總金額" />
                                            <asp:BoundField HeaderText="遲交天數">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                            </asp:BoundField>
                                            <%--<asp:BoundField DataField="Check_ID" HeaderText="發票號碼" />--%>
                                            <asp:TemplateField HeaderText="發票號碼">
                                                <itemtemplate>
                                                <asp:CheckBox id="cbInvoiceNumber" runat="server"></asp:CheckBox> <asp:Label id="lbInvoiceNumber" runat="server"></asp:Label>
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="備註">
                                                <itemtemplate>
                                                    <asp:TextBox id="txtComment" runat="server"></asp:TextBox> 
                                                </itemtemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="16" style="height: 27px" class="tbl_row">
                                    <asp:TextBox ID="txtInvoiceNumber" runat="server" MaxLength="12"></asp:TextBox>
                                    <asp:Button ID="btnInvoiceNumber" runat="server" class="btn" Text="填寫發票號碼" OnClick="btnInvoiceNumber_Click"/></td>
                            </tr>
                            <tr>
                                <td colspan="16" class="tbl_row">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        &nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" OnClientClick="return delConfirm();" />
                        <asp:Button ID="btnPrint2" runat="server" class="btn" OnClick="btnPrint1_Click" Text="列印" />&nbsp;<asp:Button
                            ID="btnOK2" runat="server" class="btn" OnClick="btnOK1_Click" Text="提交" />
                        <asp:Button ID="btnPass2" runat="server" class="btn" OnClick="btnPass2_Click" Text="放行" />
                        <asp:Button ID="btnUntread2" runat="server" class="btn" OnClick="btnUntread1_Click"
                            Text="退回" />&nbsp;<asp:Button ID="btnCancel2" runat="server" CausesValidation="False"
                                class="btn" OnClick="btnCancel_Click" Text="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="HiddenField_SAP_RID" runat="server" />
    </form>
</body>
</html>
