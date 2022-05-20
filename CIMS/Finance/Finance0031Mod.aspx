<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0031Mod.aspx.cs" Inherits="Finance_Finance0031Mod" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款及總行會計付款資訊修改</title>
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
                <tr style="height: 20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">物料請款帳務-請款及總行會計付款資訊修改</font>
                    </td>
                    <td align="right">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" OnClientClick="return delConfirm();" />
                        &nbsp;
                        <asp:Button ID="btnCancel1" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp; &nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr width="100%">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="15%">
                                    請款項目：</td>
                                <td>
                                    <asp:Label ID="lblAsk_Project" runat="server" Text="Label"></asp:Label></td>
                                <td align="right">
                                    <span class="style1">*</span>出帳金額：
                                </td>
                                <td>
                                    <asp:Label ID="lblPay_Money" runat="server" Text="Label"></asp:Label>
                                    <asp:TextBox ID="txtPay_Money" runat="server" onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="CheckAmt('txtPay_Money',7,2);value=GetValue(this.value)" onkeyup="CheckAmt('txtPay_Money',7,2)"></asp:TextBox></td>
                                <td colspan="2">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="style1">*</span>請款日期：</td>
                                <td>
                                    <asp:TextBox ID="txtAsk_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox><img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtAsk_Date')})"
                                            src="../images/calendar.gif" />
                                </td>
                                <td align="right">
                                    出帳日期：</td>
                                <td>
                                    <asp:TextBox ID="txtPay_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox><img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtPay_Date')})"
                                            src="../images/calendar.gif" />&nbsp;</td>
                                <td align="right" style="font-size: 10pt">
                                    <span style="color: #ff0000">*</span>SAP單號：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSAP_Serial_Number" runat="server" MaxLength="15" onblur="LimitLengthCheck(this,15);" style="ime-mode:disabled;" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox>
                                    </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td colspan="5">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="top">
                    <td colspan="2">
                        <cc1:GridViewPageingByDB ID="gvpbMATERIEL_PURCHASE_FORM" runat="server" AllowPaging="True"
                            AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                            EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                            FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                            SortDescImageUrl="~/images/desc.gif" OnOnSetDataSource="gvpbMATERIEL_PURCHASE_FORM_OnSetDataSource">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <RowStyle CssClass="GridViewRow" HorizontalAlign="Left" />
                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" Visible="False" />
                            <Columns>
                                <asp:BoundField DataField="PurchaseOrder_RID" HeaderText="採購單號" />
                                <asp:BoundField DataField="Material_Name" HeaderText="品名">
                                    <itemstyle horizontalalign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Unit_Price" HeaderText="單價 ">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Total_Num" HeaderText="數量 " />
                                <asp:BoundField DataField="Total_Price" HeaderText="金額 " />
                            </Columns>
                        </cc1:GridViewPageingByDB>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="Button1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" OnClientClick="return delConfirm();" />
                        &nbsp;
                        <asp:Button ID="Button2" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp; &nbsp;</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
