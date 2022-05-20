<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository005.aspx.cs" Inherits="Depository_Depository005" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>再入庫作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">再入庫作業</font>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table border="0" width="100%" cellpadding="0" cellspacing="2">
                            <tr>
                                <td style="width: 15%;" align="right">
                                    入庫流水編號：
                                </td>
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
                                        ErrorMessage="流水編號格式不對" ValidationExpression="\d{8}">*</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr>
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
                            <tr>
                                <td style="width: 15%;" align="right">
                                    Perso廠：</td>
                                <td align="left" colspan="3">
                                    &nbsp;<asp:DropDownList DataTextField="Factory_ShortName_cn" DataValueField="RID"
                                        ID="dropPerso_Factory_RID" runat="server">
                                    </asp:DropDownList>
                                    &nbsp;</td>
                            </tr>
                            <tr>
                                <td style="width: 15%;" align="right">
                                    入庫日期：</td>
                                <td align="left" colspan="3">
                                    &nbsp;<asp:TextBox ID="txtIncome_DateFrom" onfocus="WdatePicker()" runat="server"
                                        Width="100px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_DateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtIncome_DateTo" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_DateTo')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期"
                                        ControlToCompare="txtIncome_DateFrom" ControlToValidate="txtIncome_DateTo" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator></td>
                            </tr>
                            <tr>
                                <td align="right" colspan="4">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" Height="25px"
                                        OnClick="btnSearch_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnAdd" CssClass="btn" runat="server" Text="匯入" CausesValidation="False"
                                        Height="25px" OnClick="btnAdd_Click" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:GridViewPageingByDB ID="gvpbImportStock" runat="server" AutoGenerateColumns="False"
                            FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                            ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                            EditPageUrl="#" AllowPaging="True" AllowSorting="True" DataKeyNames="Stock_RID"
                            OnRowDataBound="gvpbImportStock_RowDataBound" OnOnSetDataSource="gvpbImportStock_OnSetDataSource"
                            SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" />
                            <RowStyle CssClass="GridViewRow" />
                            <Columns>
                                <asp:TemplateField HeaderText="再入庫列印編號" SortExpression="Report_RID">
                                    <itemstyle width="10%" />
                                    <itemtemplate>
                                        <asp:HyperLink id="hlReport_RID" runat="server" ></asp:HyperLink>
                                    
</itemtemplate>
                                </asp:TemplateField>
                                <asp:HyperLinkField DataTextField="Stock_RID" HeaderText="入庫流水編號" SortExpression="Stock_RID"
                                    DataNavigateUrlFields="RID,Stock_RID,Report_RID" DataNavigateUrlFormatString="Depository005Mod.aspx?ActionType=Edit&amp;RID1={0}&amp;RID={1}&amp;ID={2}">
                                    <itemstyle horizontalalign="Left" width="10%" />
                                </asp:HyperLinkField>
                                <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                    <itemstyle width="6%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Reincome_Date" HeaderText="再入庫日期">
                                    <itemstyle width="6%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Restock_Number" HeaderText="再進貨量" DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                    <itemstyle width="6%" horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Blemish_Number" HeaderText="瑕疵量" DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                    <itemstyle width="6%" horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Reincome_Number" HeaderText="再入庫量" DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                    <itemstyle width="6%" horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Serial_Number" HeaderText="卡片批號">
                                    <itemstyle width="10%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Perso_Factory_NAME" HeaderText="Perso廠商">
                                    <itemstyle width="6%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Blank_Factory_NAME" HeaderText="空白卡廠">
                                    <itemstyle width="6%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="WAFER_NAME" HeaderText="晶片名稱">
                                    <itemstyle width="6%" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="備註" DataField="Comment" />
                                <asp:BoundField DataField="SendCheck_Status" HeaderText="批號狀態">
                                    <itemstyle width="6%" />
                                </asp:BoundField>
                            </Columns>
                        </cc1:GridViewPageingByDB>
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
