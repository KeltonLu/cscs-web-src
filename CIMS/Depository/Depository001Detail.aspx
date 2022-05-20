<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository001Detail.aspx.cs"
    Inherits="Depository_Depository001Detail" %>

<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc2" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>預算簽呈明細</title>
    <meta http-equiv="pragma" content="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr class="tbl_row" valign="top">
                    <td>
                        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%" id="MainTb"
                            runat="server">
                            <tr class="tbl_title">
                                <td align="left" colspan="2">
                                    <asp:Label ID="lblTitle" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="trname" runat="server">
                                <td style="width: 20%">
                                    版面簡稱：</td>
                                <td>
                                    <asp:Label ID="lblname" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trReincome_Date" runat="server">
                                <td>
                                    再入庫日期：</td>
                                <td>
                                    <asp:Label ID="lblReincome_Date" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trCancel_Date" runat="server">
                                <td>
                                    退貨日期：</td>
                                <td>
                                    <asp:Label ID="lblCancel_Date" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trOrder_Date" runat="server">
                                <td>
                                    訂購日：</td>
                                <td>
                                    <asp:Label ID="lblOrder_Date" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trNumber" runat="server">
                                <td>
                                    訂購數量：</td>
                                <td>
                                    <asp:Label ID="lblNumber" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trStock_Number" runat="server">
                                <td>
                                    進貨量：</td>
                                <td>
                                    <asp:Label ID="lblStock_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trRestock_Number" runat="server">
                                <td>
                                    再入庫日期：</td>
                                <td>
                                    <asp:Label ID="lblRestock_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trBlemish_Number" runat="server">
                                <td>
                                    瑕疵量：</td>
                                <td>
                                    <asp:Label ID="lblBlemish_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trSample_Number" runat="server">
                                <td>
                                    抽樣卡數：</td>
                                <td>
                                    <asp:Label ID="lblSample_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trIncome_Number" runat="server">
                                <td>
                                    入庫量：</td>
                                <td>
                                    <asp:Label ID="lblIncome_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trCancel_Number" runat="server">
                                <td>
                                    退貨量：</td>
                                <td>
                                    <asp:Label ID="lblCancel_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trReincome_Number" runat="server">
                                <td>
                                    再入庫量：</td>
                                <td>
                                    <asp:Label ID="lblReincome_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trIncome_Date" runat="server">
                                <td>
                                    入庫日期：</td>
                                <td>
                                    <asp:Label ID="lblIncome_Date" runat="server"></asp:Label></td>
                            </tr>
                            
                            <tr id="trSerial_Number" runat="server">
                                <td>
                                    卡片批號：</td>
                                <td>
                                    <asp:Label ID="lblSerial_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trPerso_Factory_Name" runat="server">
                                <td>
                                    Perso廠</td>
                                <td>
                                    <asp:Label ID="lblPerso_Factory_Name" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trBlank_Factory_NAME" runat="server">
                                <td>
                                    空白卡廠：</td>
                                <td>
                                    <asp:Label ID="lblBlank_Factory_NAME" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trWafer_Name" runat="server">
                                <td>
                                    晶片名稱：</td>
                                <td>
                                    <asp:Label ID="lblWafer_Name" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trCase_Status" runat="server">
                                <td>
                                    批號狀態：</td>
                                <td>
                                    <asp:Label ID="lblCase_Status" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="tr18" runat="server">
                                <td>
                                    請款日：</td>
                                <td>
                                    <asp:Label ID="lblAsk_Date" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="tr19" runat="server">
                                <td>
                                    出帳日：</td>
                                <td>
                                    <asp:Label ID="lblPay_Date" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="tr20" runat="server">
                                <td>
                                    SAP單號：</td>
                                <td>
                                    <asp:Label ID="lblSAP_Serial_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="tr21" runat="server">
                                <td>
                                    發票號碼：</td>
                                <td>
                                    <asp:Label ID="lblCheck_Serial_Number" runat="server"></asp:Label></td>
                            </tr>
                            <tr id="trComment" runat="server">
                                <td>
                                    備註：</td>
                                <td>
                                    <asp:Label ID="lblComment" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                                        <tr>
                                            <td>
                                                <cc1:GridViewPageingByDB ID="gvpbBudget" runat="server" AutoGenerateColumns="False"
                                                    FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                    ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                    EditPageUrl="#">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <Columns>
                                                        <asp:BoundField DataField="Unit_Price" HeaderText="含稅單價" DataFormatString="{0:N4}" HtmlEncode="False">
                                                            <itemstyle width="50%" horizontalalign="Right" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Real_Ask_Number" HeaderText="請款數量" DataFormatString="{0:N0}" HtmlEncode="False">
                                                            <itemstyle width="50%" horizontalalign="Right" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </cc1:GridViewPageingByDB>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
