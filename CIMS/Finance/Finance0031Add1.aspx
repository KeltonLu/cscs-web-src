<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Finance0031Add1.aspx.cs"
    Inherits="Finance_Finance0031Add1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款及總行會計付款資訊新增</title>
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
                        <font class="PageTitle">物料請款帳務-請款及總行會計付款資訊新增</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                        <table id="queryResult" width="100%">
                            <tr class="tbl_list_row_2">
                                <td colspan="5">
                                    <cc1:GridViewPageingByDB ID="gvpbRequisition_Money" runat="server" AllowPaging="True"
                                        AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                        EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                                        SortDescImageUrl="~/images/desc.gif" OnOnSetDataSource="gvpbRequisition_Money_OnSetDataSource" OnRowDataBound="gvpbRequisition_Money_RowDataBound">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <Columns>
                                            <asp:BoundField DataField="Materiel_Type_Name" HeaderText="請款項目" />
                                            <asp:TemplateField HeaderText="出帳金額 ">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 10 改為 12 --%>
<asp:TextBox id="txtPay_Money" runat="server" __designer:wfdid="w18" MaxLength="12" onblur="CheckAmtWithNoId(this,7,2);value=GetValue(this.value);" onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onkeyup="CheckAmtWithNoId(this,7,2)"></asp:TextBox> 
<asp:Label id="lblPay_Money" runat="server" Text="Label" __designer:wfdid="w19"></asp:Label> 
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="SAP單號 ">
                                                <itemtemplate>
<asp:TextBox id="txtSAP_ID" runat="server" MaxLength="15" __designer:wfdid="w20" onblur="LimitLengthCheck(this,15);" style="ime-mode:disabled;" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox> 
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="請款日期">
                                                <itemtemplate>
<asp:TextBox id="txtAsk_Date" onfocus="WdatePicker()" runat="server" __designer:wfdid="w21"></asp:TextBox> 
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="出帳日期 ">
                                                <itemtemplate>
<asp:TextBox id="txtPay_Date" onfocus="WdatePicker()" runat="server" __designer:wfdid="w22"></asp:TextBox> 
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
                    <asp:Button ID="btnIsSave" runat="server" CausesValidation="False" OnClick="btnIsSave_Click"
            Text="Button" style="display: none;"/>
                        &nbsp; &nbsp;&nbsp;
                        <asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />
                        &nbsp;
                        <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp; &nbsp;</td>
                </tr>
            </table>
        </div>

    </form>
</body>
</html>
