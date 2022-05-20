<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0013.aspx.cs" Inherits="Finance_Finance0013" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>帳務查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="0"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">帳務查詢</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0" id="div">
                            <tr valign="baseline">
                                <td align="right">
                                    用途：</td>
                                <td>
                                    <asp:DropDownList ID="dropCard_Purpose" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    群組名稱：&nbsp;<asp:DropDownList ID="dropCard_Group" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    版面簡稱：</td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" onkeyup="LimitLengthCheck(this, 30)" MaxLength="30"></asp:TextBox></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" width="10%">
                                    空白卡廠：</td>
                                <td width="40%">
                                    <asp:DropDownList ID="dropBlankFactory" runat="server">
                                    </asp:DropDownList></td>
                                <td align="right" width="15%" id="budgetid">
                                    放行狀態：</td>
                                <td>
                                    <asp:DropDownList ID="dropPass_Status" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="1">暫存</asp:ListItem>
                                        <asp:ListItem Value="2">退回</asp:ListItem>
                                        <asp:ListItem Value="3">待放行</asp:ListItem>
                                        <asp:ListItem Value="4">已放行</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    請款狀態：</td>
                                <td>
                                    <asp:DropDownList ID="dropState" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="1">未請款</asp:ListItem>
                                        <asp:ListItem Value="2">已請款</asp:ListItem>
                                    </asp:DropDownList></td>
                                <td align="right" id="td1">
                                    出帳狀態：</td>
                                <td id="td2">
                                    <asp:DropDownList ID="dropIs_Finance" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="Y">已出帳</asp:ListItem>
                                        <asp:ListItem Value="N">未出帳</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    進貨作業日期：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    &nbsp;&nbsp; ~
                                    <asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>&nbsp;
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />&nbsp;
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="進貨作業日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right">
                                    入庫流水編號：</td>
                                <td align="left" colspan="3">
                                    <asp:TextBox ID="txtStock_RIDYear" Style="ime-mode: disabled;" onkeyup="Stock_RIDYearChange()"
                                        onblur="Stock_RIDYearChange()" runat="server" Width="100px" MaxLength="8"></asp:TextBox>-
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
                                <td align="right">
                                    預算簽呈文號：</td>
                                <td>
                                    <asp:TextBox ID="txtBUDGET_ID" runat="server" onkeyup="LimitLengthCheck(this, 30)" MaxLength="30"></asp:TextBox></td>
                                <td align="right">
                                    合約編號：</td>
                                <td>
                                    <asp:TextBox ID="txtAgreement_Code" runat="server" MaxLength="20" onblur="LimitLengthCheck(this,20)"
                                        onkeyup="LimitLengthCheck(this,20)"></asp:TextBox></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="bottom" id="buttonRow1" class="tbl_row">
                    <td align="right">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSearch1" runat="server" CssClass="btn" OnClick="btnSearch1_Click"
                            Text="查詢" />
                        &nbsp;
                        <asp:Button ID="btnCancel1" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table id="queryResult1" width="100%">
                            <tr>
                                <td align="left" colspan="18" style="height: 26px">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tr>
                                                    <td colspan="16">
                                                        <cc1:GridViewPageingByDB ID="gvpbFinance" runat="server" OnOnSetDataSource="gvpbFinance_OnSetDataSource" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>"
                                                            NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbFinance_RowDataBound"
                                                            SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                            EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" Visible="False" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <Columns>
                                                                <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號 " />
                                                                <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                                                    <itemstyle horizontalalign="Left" />
                                                                    <headerstyle horizontalalign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="空白卡厰" />
                                                                <asp:BoundField DataField="Operate_Type" HeaderText="進貨作業" HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Left" />
                                                                    <headerstyle horizontalalign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Fore_Delivery_Date" HeaderText="預訂交貨日期" />
                                                                <asp:BoundField DataField="Income_Date" HeaderText="進貨作業日期 ">
                                                                    <itemstyle horizontalalign="Left" />
                                                                    <headerstyle horizontalalign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Delay_Days" HeaderText="遲交天數" DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="RIC_Number" HeaderText="數量 " DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Unit_Price" HeaderText="含稅單價" DataFormatString="{0:N4}"
                                    HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Unit_Price1" HeaderText="未稅單價 " DataFormatString="{0:N4}"
                                    HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField HeaderText="含稅總金額 " DataFormatString="{0:N2}" HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField HeaderText="未稅總金額 " DataFormatString="{0:N2}" HtmlEncode="False">
                                                                    <itemstyle horizontalalign="Right" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Ask_Date" HeaderText="請款日期" />
                                                                <asp:BoundField DataField="Pay_Date" HeaderText="出帳日期" />
                                                                <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號" />
                                                                <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號" />
                                                                <asp:BoundField DataField="SAP_ID" HeaderText="SAP單號" />
                                                                <asp:BoundField DataField="Check_ID" HeaderText="發票號碼" />
                                                                <%-- add by chaoma start --%>
                                                                <asp:BoundField DataField="Comment" HeaderText="備註" />
                                                                <%-- add by chaoma end --%>
                                                                <asp:BoundField DataField="Operate_RID" HeaderText="Operate_RID" />
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
                                <td align="right" colspan="18" style="height: 26px">
                                    <asp:Button ID="btnExport" runat="server" CausesValidation="False" CssClass="btn"
                                        OnClick="btnExport_Click" Text="匯出EXCEL表格" Visible="False" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <script>
                document.getElementById("txtStock_RID1").disabled=true;
                document.getElementById("txtStock_RID2").disabled=true;
                document.getElementById("txtStock_RID3").disabled=true;
                txtChange1();
                txtChange2();
                Stock_RIDYearChange();
                
                
                
                function Stock_RIDYearChange()
                {
                    if(document.getElementById("txtStock_RIDYear").value!="")
                    {
                        
                        document.getElementById("txtStock_RID1").disabled=false;
                    }
                    else
                    {
                        document.getElementById("txtStock_RID1").value="";
                        document.getElementById("txtStock_RID2").value="";
                        document.getElementById("txtStock_RID3").value="";
                        document.getElementById("txtStock_RID1").disabled=true;
                        document.getElementById("txtStock_RID2").disabled=true;
                        document.getElementById("txtStock_RID3").disabled=true;
                    }
                }
                
                function txtChange1()
                {
                    CheckNum('txtStock_RID1',2)
                    if(document.getElementById("txtStock_RID1").value!="")
                    {
                        document.getElementById("txtStock_RID2").disabled=false;
                    }
                    else
                    {
                        document.getElementById("txtStock_RID2").value="";
                        document.getElementById("txtStock_RID3").value="";
                        document.getElementById("txtStock_RID2").disabled=true;
                        document.getElementById("txtStock_RID3").disabled=true;
                    }
                }
                
                function txtChange2()
                {
                    CheckNum('txtStock_RID2',2)
                    if(document.getElementById("txtStock_RID2").value!="")
                    {
                        document.getElementById("txtStock_RID3").disabled=false;
                    }
                    else
                    {
                        document.getElementById("txtStock_RID3").value="";
                        document.getElementById("txtStock_RID3").disabled=true;
                    }
                }
                
                function ChecktxtStock_RID(obj)
                {
                    var num=obj.value;
                    if(num!="")
                    {
                        if(num.length==1)
                        {
                            obj.value=0+num;
                        }
                    }
                    txtChange1();
                    txtChange2();
                }
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
