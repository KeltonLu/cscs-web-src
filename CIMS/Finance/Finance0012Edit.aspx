<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0012Edit.aspx.cs"
    Inherits="Finance_Finance0012Edit" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>總行會計付款修改</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
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
                        <font class="PageTitle">總行會計付款修改</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            OnClientClick="return delConfirm();" Text="確定" />
                        &nbsp;&nbsp;<asp:Button ID="btnCancel1" runat="server" CausesValidation="False" class="btn"
                            OnClick="btnCancel1_Click" Text="取消" />
                    </td>
                </tr>
                <tr width="100%" valign="baseline">
                    <td colspan="2">
                        SAP單號：<asp:Label ID="lblSAP_Serial_Number" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr width="100%" id="tr1">
                    <td colspan="2">
                        <cc1:GridViewPageingByDB ID="gvpbRequisitionWork" runat="server" OnOnSetDataSource="gvpbRequisitionWork_OnSetDataSource"
                            AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                            PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbRequisitionWork_RowDataBound"
                            SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                            EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" />
                            <RowStyle CssClass="GridViewRow" />
                            <Columns>
                                <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                    <itemstyle horizontalalign="Left" />
                                    <headerstyle horizontalalign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="空白卡厰" />
                                <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號" />
                                <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號" />
                                <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號 " />
                                <asp:BoundField DataField="Operate_Type" HeaderText="進貨作業" HtmlEncode="False">
                                    <itemstyle horizontalalign="Left" />
                                    <headerstyle horizontalalign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Income_Number" HeaderText="數量 " DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Income_Date" HeaderText="日期">
                                    <itemstyle horizontalalign="Right" />
                                    <headerstyle horizontalalign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Unit_Price" HeaderText="含稅單價 " DataFormatString="{0:N4}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Unit_Price_No" HeaderText="未稅單價 " DataFormatString="{0:N4}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="實際請款數量">
                                    <itemstyle horizontalalign="Right" />
                                    <itemtemplate>
<asp:Label id="lblSumRequisition_Count" runat="server" __designer:wfdid="w2"></asp:Label> 
</itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="含稅總金額 ">
                                    <itemstyle horizontalalign="Right" />
                                    <itemtemplate>
<asp:Label id="lblContanUnit_Price" runat="server"></asp:Label><%--<asp:TextBox id="txtSumContanUnit_Price" runat="server" Width="80px"></asp:TextBox>--%> 
</itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="未稅總金額 ">
                                    <itemstyle horizontalalign="Right" />
                                    <itemtemplate>
<asp:Label id="lblNOContanUnit_Price" runat="server" __designer:wfdid="w3"></asp:Label> 
</itemtemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="遲交天數" >
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Check_Serial_Number" HeaderText="發票號碼" />
                                <asp:TemplateField HeaderText="備註">
                                    <itemtemplate>
<%--<asp:TextBox id="txtComment" runat="server" __designer:wfdid="w7"></asp:TextBox>--%><asp:Label id="lblComment" runat="server" __designer:wfdid="w8"></asp:Label> 
</itemtemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Unit_Price_No" HeaderText="未稅單價 " DataFormatString="{0:N4}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Comment" HeaderText="備註" />
                                <asp:BoundField DataField="Real_Ask_Number" HeaderText="實際請款數量" DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Operate_RID" HeaderText="Operate_RID" />
                                <asp:BoundField DataField="含稅總金額" HeaderText="含稅總金額 " DataFormatString="{0:N2}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="未稅總金額" HeaderText="未稅總金額 " DataFormatString="{0:N2}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Operate_Type" HeaderText="Operate_Type" />
                            </Columns>
                        </cc1:GridViewPageingByDB>
                    </td>
                </tr>
                <tr id="tr2">
                    <td colspan="2">
                        <table width="100%" cellspacing="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="20%">
                                    實際含稅請款金額：
                                </td>
                                <td>
                                    <asp:Label ID="lblSumUnit_Price" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" valign="baseline">
                                    <span class="style1">*</span> 實際含稅付款金額：
                                </td>
                                <td>
                                                        <%--Dana 20161021 最大長度由 10 改為 14 --%>
                                    <asp:TextBox ID="txtReal_Pay_Money" runat="server"  onfocus="DelDouhao(this)"
                                    onblur="CheckAmt('txtReal_Pay_Money',9,2);this.value=GetValue(this.value)" OnTextChanged="txtReal_Pay_Money_TextChanged"
                                        MaxLength="14" style="ime-mode:disabled;text-align: right" AutoPostBack="True"
                                        onkeyup="CheckAmt('txtReal_Pay_Money',9,2)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtReal_Pay_Money"
                                        ErrorMessage="實際含稅付款金額不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="height: 17px">
                                    含稅差異數：
                                </td>
                                <td style="height: 17px">
                                    <asp:Label ID="lblDiffUnit_Price" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    實際未稅請款金額：
                                </td>
                                <td>
                                    <asp:Label ID="lblUnit_PriceNO" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span> 實際未稅付款金額：
                                </td>
                                <td>
                                                        <%--Dana 20161021 最大長度由 10 改為 14 --%>
                                    <asp:TextBox ID="txtReal_Pay_Money_No" runat="server" onfocus="DelDouhao(this)" onblur="CheckAmtWithNoId(this,9,2);this.value=GetValue(this.value)" AutoPostBack="True" OnTextChanged="txtReal_Pay_Money_No_TextChanged"
                                        MaxLength="14" style="ime-mode:disabled;text-align: right"
                                        onkeyup="CheckAmtWithNoId(this,9,2)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtReal_Pay_Money_No"
                                        ErrorMessage="實際未稅付款金額不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    未稅差異數：
                                </td>
                                <td>
                                    <asp:Label ID="lblDiffUnit_PriceNO" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span class="style1">*</span> 出帳日：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>&nbsp; &nbsp;<img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})"
                                            src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtBegin_Date"
                                        ErrorMessage="出帳日不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        &nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />
                        &nbsp;&nbsp;<asp:Button ID="btnCancel2" runat="server" CausesValidation="False" class="btn"
                            OnClick="btnCancel1_Click" Text="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="HiddenField_SAP_RID" runat="server" />
        <asp:HiddenField ID="HiddenField_Ask_Date" runat="server" />
        <asp:HiddenField ID="HiddenField_Pay_Date" runat="server" />
        <asp:HiddenField ID="HiddenField_Is_Finance" runat="server" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
