<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository004.aspx.cs" Inherits="Depository_Depository004" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退貨作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table width="100%" cellpadding="0" cellspacing="1" border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">退貨作業</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="15%" align="right">
                                    入庫流水編號：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCancel_RIDYear" onkeyup="Cancel_RIDYearChange()" onblur="Cancel_RIDYearChange()"
                                        runat="server" Width="100px" MaxLength="8"></asp:TextBox>-
                                    <asp:TextBox ID="txtCancel_RID1" onblur="ChecktxtCancel_RID(this)" onkeyup="txtChange1()"
                                        runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtCancel_RID2" onblur="ChecktxtCancel_RID(this)" onkeyup="txtChange2()"
                                        runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtCancel_RID3" onblur="ChecktxtCancel_RID(this)" onkeyup="CheckNum('txtCancel_RID3',2)"
                                        runat="server" Width="20px" MaxLength="2"></asp:TextBox>&nbsp;&nbsp; &nbsp;&nbsp;
                                </td>
                                <td align="right">
                                    退貨日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCancel_DateFrom" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtCancel_DateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtCancel_DateTo" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtCancel_DateTo')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期"
                                        ControlToCompare="txtCancel_DateFrom" ControlToValidate="txtCancel_DateTo" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right">
                                    PERSO廠：
                                </td>
                                <td align="left">
                                    <asp:DropDownList DataTextField="Factory_ShortName_cn" DataValueField="RID" ID="dropPerso_Factory_Search"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="6" align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" OnClick="btnSearch_Click" runat="server"
                                        Text="查詢" />&nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" OnClick="btnAdd_Click" runat="server" Text="新增" CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table id="Table1" width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbCancel" runat="server" OnOnSetDataSource="gvpbCancel_OnSetDataSource"
                                                AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbCancel_RowDataBound"
                                                ColumnsHeaderText="" AllowSorting="True" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                EditPageUrl="#" AllowPaging="True" DataKeyNames="Stock_RID" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="退貨列印編號" SortExpression="Report_RID">
                                                        <itemstyle width="10%" />
                                                        <itemtemplate>
                                        <asp:HyperLink id="hlReport_RID" runat="server" ></asp:HyperLink>
                                    
</itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:HyperLinkField DataTextField="Stock_RID" HeaderText="入庫流水編號" SortExpression="Stock_RID"
                                                        DataNavigateUrlFields="RID" DataNavigateUrlFormatString="Depository004Edit.aspx?ActionType=Edit&amp;RID={0}">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                                        <itemstyle width="10%" horizontalalign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Cancel_number" HeaderText="退貨量" DataFormatString="{0:N0}"
                                                        HtmlEncode="False">
                                                        <itemstyle horizontalalign="Right" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="退貨日期">
                                                        <itemstyle width="15%" />
                                                        <itemtemplate>
                                        <asp:Label id="lblCancel_Date" runat="server" ></asp:Label>
                                    
</itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Comment" HeaderText="備註" HtmlEncode="False">
                                                        <itemstyle horizontalalign="Left" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>

            <script type="text/javascript">
                document.getElementById("txtCancel_RID1").disabled=true;
                document.getElementById("txtCancel_RID2").disabled=true;
                document.getElementById("txtCancel_RID3").disabled=true;
                txtChange1();
                txtChange2();
                Cancel_RIDYearChange();
                
                
                
                function Cancel_RIDYearChange()
                {
                    if(document.getElementById("txtCancel_RIDYear").value!="")
                    {
                        
                        document.getElementById("txtCancel_RID1").disabled=false;
                    }
                    else
                    {
                        document.getElementById("txtCancel_RID1").value="";
                        document.getElementById("txtCancel_RID2").value="";
                        document.getElementById("txtCancel_RID3").value="";
                        document.getElementById("txtCancel_RID1").disabled=true;
                        document.getElementById("txtCancel_RID2").disabled=true;
                        document.getElementById("txtCancel_RID3").disabled=true;
                    }
                }
                
                function txtChange1()
                {
                    CheckNum('txtCancel_RID1',2)
                    if(document.getElementById("txtCancel_RID1").value!="")
                    {
                        document.getElementById("txtCancel_RID2").disabled=false;
                    }
                    else
                    {
                        document.getElementById("txtCancel_RID2").value="";
                        document.getElementById("txtCancel_RID3").value="";
                        document.getElementById("txtCancel_RID2").disabled=true;
                        document.getElementById("txtCancel_RID3").disabled=true;
                    }
                }
                
                function txtChange2()
                {
                    CheckNum('txtCancel_RID2',2)
                    if(document.getElementById("txtCancel_RID2").value!="")
                    {
                        document.getElementById("txtCancel_RID3").disabled=false;
                    }
                    else
                    {
                        document.getElementById("txtCancel_RID3").value="";
                        document.getElementById("txtCancel_RID3").disabled=true;
                    }
                }
                
                function ChecktxtCancel_RID(obj)
                {
                    var num=obj.value;
//                    if(num!="")
//                    {
//                        if(num.length==1)
//                        {
//                            obj.value=0+num;
//                        }
//                    }
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
