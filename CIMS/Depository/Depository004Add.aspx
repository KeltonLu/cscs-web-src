<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository004Add.aspx.cs"
    Inherits="Depository_Depository004Add" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退貨作業新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script type="text/javascript">
    function doRepeat(){                
       var isConfirm = document.getElementById("hidConfirm").value;
       var stockNum = document.getElementById("hidStockNum").value;
       if(isConfirm == "1"){
          if(confirm("入庫流水編號："+stockNum+"已經有退貨記錄，確定要再退貨嗎？")){
                document.getElementById("hidConfirm").value="2";
                  __doPostBack("btnSubmit","");
          } 
       }
    }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:HiddenField ID="hidConfirm" runat="server" />
            <asp:HiddenField ID="hidStockNum" runat="server" />
            <asp:HiddenField ID="HidRowId" runat="server" Value="0" />
            <table width="100%" cellpadding="0" cellspacing="1" border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="title_one">退貨作業新增</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="15%" align="right" style="height: 48px">
                                    入庫流水編號：
                                </td>
                                <td width="20%" style="height: 48px">
                                    <asp:TextBox ID="txtStock_RIDYear" onkeyup="Stock_RIDYearChange()" onblur="Stock_RIDYearChange()"
                                        runat="server" Width="100px" MaxLength="8"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID1" onblur="ChecktxtStock_RID(this)" onkeyup="txtChange1()"
                                        runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID2" onblur="ChecktxtStock_RID(this)" onkeyup="txtChange2()"
                                        runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID3" onblur="ChecktxtStock_RID(this)" onkeyup="CheckNum('txtStock_RID3',2)"
                                        runat="server" Width="20px" MaxLength="2"></asp:TextBox>&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtStock_RIDYear"
                                        ErrorMessage="流水編號格式不對" ValidationExpression="\d{8}" EnableTheming="True" Enabled="False">流水編號格式不對</asp:RegularExpressionValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtStock_RID1"
                                        ErrorMessage="流水編號AA格式不對" ValidationExpression="\d{2}">*</asp:RegularExpressionValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtStock_RID2"
                                        ErrorMessage="流水編號BB格式不對" ValidationExpression="\d{2}">*</asp:RegularExpressionValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtStock_RID3"
                                        ErrorMessage="流水編號CC格式不對" ValidationExpression="\d{2}">*</asp:RegularExpressionValidator>
                                </td>
                                <td align="right" width="10%" style="height: 48px">
                                    入庫日期：
                                </td>
                                <td align="left" valign="baseline" style="height: 48px">
                                    <asp:TextBox ID="txtStock_DateFrom" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtStock_DateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtStock_DateTo" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtStock_DateTo')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期"
                                        ControlToCompare="txtStock_DateFrom" ControlToValidate="txtStock_DateTo" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right" style="height: 48px">
                                    Perso廠商：
                                </td>
                                <td align="left" style="height: 48px">
                                    <asp:DropDownList DataTextField="Factory_ShortName_cn" DataValueField="RID" ID="dropPerso_Factory_RID"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6">
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
                                <td colspan="6" align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" OnClick="btnSearch_Click" runat="server"
                                        Text="查詢" />
                                    &nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnCancel" OnClick="btnCancel_Click" runat="server"
                                        Text="取消" />
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
                        <cc1:GridViewPageingByDB ID="gvpbDepository" runat="server" OnOnSetDataSource="gvpbDepository_OnSetDataSource"
                            AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                            PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbDepository_RowDataBound"
                            ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                            EditPageUrl="#" AllowPaging="True" AllowSorting="True" DataKeyNames="Stock_RID">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" />
                            <RowStyle CssClass="GridViewRow" />
                            <Columns>
                                <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號">
                                    <itemstyle width="10%" horizontalalign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                    <itemstyle width="10%" horizontalalign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Income_Number" HeaderText="入庫量" DataFormatString="{0:N0}"
                                    HtmlEncode="False">
                                    <itemstyle horizontalalign="Right" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="入庫日期">
                                    <itemstyle width="10%" />
                                    <itemtemplate>
                                        <asp:Label id="lblIncome_Date" runat="server" ></asp:Label>
                                    
</itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="退貨量">
                                    <itemstyle width="30%" />
                                    <itemtemplate>
                                        <asp:TextBox id="txtCancel_number" runat="server" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)" onkeyup="CheckNumWithNoId(this,9)"></asp:TextBox>                                
                                    
</itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="備註">
                                    <itemstyle width="20%" />
                                    <itemtemplate>
<asp:TextBox id="txtComment" runat="server" MaxLength="100" __designer:wfdid="w2"></asp:TextBox> 
</itemtemplate>
                                </asp:TemplateField>
                            </Columns>
                        </cc1:GridViewPageingByDB>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button CssClass="btn" Text="確定" ID="btnSubmit" OnClick="btnSubmit_Click" runat="server" />
                    </td>
                </tr>
            </table>

            <script type="text/javascript">
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
