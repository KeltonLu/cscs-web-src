<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository012.aspx.cs" Inherits="Depository_Depository012" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>當日監控作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js">

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">當日監控作業</font></td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="15%" valign="baseline">
                                    <span class="style1">*</span>查詢日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCheckDate" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtCheckDate')})" src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCheckDate"
                                        ErrorMessage="查詢日期不能為空">*</asp:RequiredFieldValidator>
                                </td>
                                <td align="right" valign="baseline">
                                    Perso廠：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropPerso" runat="server" DataTextField="Factory_ShortName_CN"
                                        DataValueField="RID" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="15%" valign="baseline">
                                    CARD TYPE：
                                </td>
                                <td valign="baseline">
                                    <asp:TextBox runat="server" ID="txtCardtype" onblur="CheckNum('txtCardtype',3)" onkeyup="CheckNum('txtCardtype',3)"
                                        MaxLength="3" Width="30px" />
                                    &nbsp;&nbsp;AFFINITY：&nbsp<asp:TextBox runat="server" onblur="CheckNum('txtAffinity',4)" ID="txtAffinity" onkeyup="CheckNum('txtAffinity',4)"
                                        MaxLength="4" Width="40px" />
                                    &nbsp;&nbsp;PHOTO：&nbsp;<asp:TextBox runat="server" ID="txtPhoto" onblur="CheckNum('txtPhoto',2)" onkeyup="CheckNum('txtPhoto',2)"
                                        MaxLength="2" Width="20px" />
                                </td>
                                <td align="right" valign="baseline">
                                    版面簡稱：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtName" onkeyup="LimitLengthCheck(this, 30)" MaxLength="30" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="15%" valign="baseline">
                                    安全庫存月數小於等於：
                                </td>
                                <td valign="baseline">
                                    <asp:TextBox runat="server" ID="txtMonth" MaxLength="4" onblur="CheckAmt('txtMonth',2,1)" onkeyup="CheckAmt('txtMonth',2,1)" Style="ime-mode: disabled;
                                        text-align: right" />
                                   </td>
                                <td colspan="2" valign="baseline">
                                    <asp:RadioButtonList ID="radSaveType" runat="server" RepeatDirection="Horizontal"
                                        AutoPostBack="True">
                                        <asp:ListItem Text="當日" Value="0" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="前十日" Value="1"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" align="right">
                                    <asp:Button Text="計算" OnClick="btnCalculate_Click" ID="btnCalculate" runat="server"
                                        CssClass="btn" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table id="Table2" width="100%">
                            <tr>
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbCalculate" runat="server" AllowPaging="False" OnOnSetDataSource="gvpbCalculate_OnSetDataSource"
                                        AllowSorting="False" AutoGenerateColumns="False" CssClass="GridView" OnRowDataBound="gvpbCalculate_RowDataBound"
                                        ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                        EditPageUrl="#">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="">
                                                <itemstyle width="10px" />
                                                <itemtemplate>
                                                    <asp:CheckBox ID = "checkBuy" runat="server" />                                
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="rowId" HeaderText="序號" />
                                            <asp:BoundField DataField="persoName" HeaderText="Perso廠">
                                                <itemstyle width="80px" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="cardType" HeaderText="CARD TYPE" />
                                            <asp:BoundField DataField="affinity" HeaderText="AFFINITY" />
                                            <asp:BoundField DataField="photo" HeaderText="PHOTO" />
                                            <asp:BoundField DataField="name" HeaderText="版面簡稱" />
                                            <asp:BoundField DataField="stockNumber" HeaderText="可用庫存量">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="not_Income" HeaderText="已下單未到貨數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nextMonth_CNumber" HeaderText="該月換卡耗用量">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="next6Months_CNumber" HeaderText="次6月換卡數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="next12Months_CNumber" HeaderText="次12月換卡數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="halfYear_GBNumber" HeaderText="前半年月平均掛補數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="halfYear_HBNumber" HeaderText="前半年月平均毀補數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="today_NewNumber" HeaderText="當日新卡數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="tenDay_NewNumber" HeaderText="前十日平均新卡數">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="採購量">
                                                <itemstyle width="80px" horizontalalign="Center" />
                                                <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                    <asp:TextBox id="txtBuy_number" runat="server" onfocus="DelDouhao(this)" MaxLength="11" onblur="CheckNumWithNoId(this,9);value=GetValue(this.value);" onkeyup="CheckNumWithNoId(this,9)" width= "80px" style="ime-mode:disabled;text-align: right"></asp:TextBox>                                
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="expectMonth_Today" HeaderText="預計可用月數（當日）">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="expectMonth_Ten" HeaderText="預計可用月數（前十日）">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Button Text="匯出Excel格式" CausesValidation="false" Height="25px" CssClass="btn" runat="server" ID="btnReport" OnClick="btnReport_click" />
                                    &nbsp;&nbsp;
                                    <asp:Button Text="採購下單" CausesValidation="false" Height="25px" CssClass="btn" ID="btnSubmit" OnClick="btnSubmit_click" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
