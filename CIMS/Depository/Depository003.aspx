<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository003.aspx.cs" Inherits="Depository_Depository003" EnableEventValidation="false" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>入庫單查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">入庫作業</font>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table border="0" width="100%" cellpadding="0" cellspacing="2">
                            <tr valign="baseline">
                                <td style="width: 15%;" align="right">入庫流水編號：
                                </td>
                                <td align="left" colspan="3">
                                    <asp:TextBox ID="txtStock_RIDYear" Style="ime-mode: disabled;" onkeyup="Stock_RIDYearChange()" onblur="Stock_RIDYearChange()"
                                        runat="server" Width="100px" MaxLength="8"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID1" Style="ime-mode: disabled;" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="txtChange1()" runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID2" Style="ime-mode: disabled;" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="txtChange2()" runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID3" Style="ime-mode: disabled;" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="CheckNum('txtStock_RID3',2)" runat="server" Width="20px" MaxLength="2"></asp:TextBox>&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtStock_RIDYear"
                                        ErrorMessage="流水編號格式不對" ValidationExpression="\d{8}">流水編號格式不對</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="4">

                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                            </td>
                                        </tr>
                                    </table>


                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td style="width: 15%;" align="right">空白卡廠：
                                </td>
                                <td style="width: 15%;" align="left">
                                    <asp:DropDownList DataTextField="Factory_ShortName_cn" DataValueField="RID" ID="dropBlank_Factory_RID" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 15%;" align="right">Perso廠：
                                </td>
                                <td align="left">
                                    <asp:DropDownList DataTextField="Factory_ShortName_cn" DataValueField="RID" ID="dropPerso_Factory_RID" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td style="width: 15%;" align="right">訂貨日期：
                                </td>
                                <td align="left" colspan="3">
                                    <asp:TextBox ID="txtOrder_DateFrom" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtOrder_DateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtOrder_DateTo" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtOrder_DateTo')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    &nbsp;&nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="結束日期必須大於開始日期" ControlToCompare="txtOrder_DateFrom" ControlToValidate="txtOrder_DateTo" Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                    &nbsp;&nbsp;入庫日期：
                                    <asp:TextBox ID="txtIncome_DateFrom" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_DateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtIncome_DateTo" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_DateTo')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期" ControlToCompare="txtIncome_DateFrom" ControlToValidate="txtIncome_DateTo" Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator></td>
                            </tr>
                            <tr valign="baseline">
                                <td style="width: 15%;" align="right">訂單結案狀態：
                                </td>
                                <td style="width: 15%;" align="left">
                                    <asp:DropDownList ID="dropStatus" runat="server">
                                        <asp:ListItem Value="">全部</asp:ListItem>
                                        <asp:ListItem Value="Y">已結案</asp:ListItem>
                                        <asp:ListItem Value="N">未結案</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td colspan="2">
                                    <asp:RadioButtonList ID="radlOrderType" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="1">訂單</asp:ListItem>
                                        <asp:ListItem Value="2">訂單流水編號 </asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td>

                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                            <tr class="tbl_row">
                                <td align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" Height="25px" />&nbsp;&nbsp;
                                    <asp:Button ID="btnAdd" CssClass="btn" runat="server" Text="Auto匯入" CausesValidation="False" OnClick="btnAdd_Click" Height="25px" />&nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnAddMans" CausesValidation="False" runat="server" Text="人工新增" Height="25px" OnClick="btnAddMans_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbImportStock" runat="server" OnOnSetDataSource="gvpbImportStock_OnSetDataSource" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbImportStock_RowDataBound" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" AllowPaging="True" AllowSorting="True" DataKeyNames="Stock_RID" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Button ID="btnDel" runat="server" CssClass="btn" Text="刪除" OnClientClick="return CheckDelete();" OnClick="btnDel_Click" />
                                                </HeaderTemplate>
                                                <ItemStyle Width="4%" />
                                                <ItemTemplate><asp:CheckBox ID="chkDel" runat="server" /></ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="入庫列印編號" SortExpression="Report_RID">
                                                <ItemStyle Width="10%" />
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hlReport_RID" runat="server"></asp:HyperLink>

                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="入庫流水編號" SortExpression="Stock_RID">
                                                <ItemStyle Width="10%" />
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hlStock_RID" runat="server"></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                                <ItemStyle Width="6%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Income_Date" HeaderText="入庫日期">
                                                <ItemStyle Width="6%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Stock_Number" HeaderText="進貨量" DataFormatString="{0:N0}" HtmlEncode="False">
                                                <ItemStyle Width="6%" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Blemish_Number" HeaderText="瑕疵量" DataFormatString="{0:N0}" HtmlEncode="False">
                                                <ItemStyle Width="6%" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Income_Number" HeaderText="入庫量" DataFormatString="{0:N0}" HtmlEncode="False">
                                                <ItemStyle Width="6%" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Serial_Number" HeaderText="卡片批號">
                                                <ItemStyle Width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Perso_Factory_NAME" HeaderText="Perso廠">
                                                <ItemStyle Width="6%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Blank_Factory_NAME" HeaderText="空白卡廠">
                                                <ItemStyle Width="6%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="WAFER_NAME" HeaderText="晶片名稱">
                                                <ItemStyle Width="6%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="SendCheck_Status" HeaderText="批號狀態">
                                                <ItemStyle Width="6%" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="結案狀態">
                                                <ItemStyle Width="6%" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCase_Status" runat="server"></asp:Label>


                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
            </table>

            <script>
                document.getElementById("txtStock_RID1").disabled = true;
                document.getElementById("txtStock_RID2").disabled = true;
                document.getElementById("txtStock_RID3").disabled = true;
                txtChange1();
                txtChange2();
                Stock_RIDYearChange();



                function Stock_RIDYearChange() {
                    if (document.getElementById("txtStock_RIDYear").value != "") {

                        document.getElementById("txtStock_RID1").disabled = false;
                    }
                    else {
                        document.getElementById("txtStock_RID1").value = "";
                        document.getElementById("txtStock_RID2").value = "";
                        document.getElementById("txtStock_RID3").value = "";
                        document.getElementById("txtStock_RID1").disabled = true;
                        document.getElementById("txtStock_RID2").disabled = true;
                        document.getElementById("txtStock_RID3").disabled = true;
                    }
                }

                function txtChange1() {
                    CheckNum('txtStock_RID1', 2)
                    if (document.getElementById("txtStock_RID1").value != "") {
                        document.getElementById("txtStock_RID2").disabled = false;
                    }
                    else {
                        document.getElementById("txtStock_RID2").value = "";
                        document.getElementById("txtStock_RID3").value = "";
                        document.getElementById("txtStock_RID2").disabled = true;
                        document.getElementById("txtStock_RID3").disabled = true;
                    }
                }

                function txtChange2() {
                    CheckNum('txtStock_RID2', 2)
                    if (document.getElementById("txtStock_RID2").value != "") {
                        document.getElementById("txtStock_RID3").disabled = false;
                    }
                    else {
                        document.getElementById("txtStock_RID3").value = "";
                        document.getElementById("txtStock_RID3").disabled = true;
                    }
                }

                function ChecktxtStock_RID(obj) {
                    var num = obj.value;
                    if (num != "") {
                        if (num.length == 1) {
                            obj.value = 0 + num;
                        }
                    }
                    txtChange1();
                    txtChange2();
                }

                // Legend 2017/05/17 刪除前檢核
                function CheckDelete()
                {
                    if (confirm("是否刪除訊息"))
                    {
                        var gv = document.getElementById("gvpbImportStock"); 
                        var checkCnt = 0;
                        
                        for (i = 1; i < gv.rows.length; i++)
                        {
                            var objCheckBox = gv.rows(i).cells(0);
                            if (objCheckBox.getElementsByTagName("input")[0].checked)
                            {
                                checkCnt++;
                            }
                        }

                        if (checkCnt == 0)
                        {
                            alert("請至少點選一筆資料");
                            return false;
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
