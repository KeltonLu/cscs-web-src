<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository001.aspx.cs" Inherits="Depository_Depository001" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡片採購歷程</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">卡片採購歷程</font>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                            <tr valign="baseline">
                                <td style="width: 15%;" align="right">
                                    入庫流水編號：
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtStock_RIDYear" Style="ime-mode: disabled;" onkeyup="Stock_RIDYearChange()"
                                        onblur="Stock_RIDYearChange()" runat="server" Width="100px" MaxLength="8"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID1" Style="ime-mode: disabled;" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="txtChange1()" runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID2" Style="ime-mode: disabled;" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="txtChange2()" runat="server" Width="20px" MaxLength="2"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID3" Style="ime-mode: disabled;" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="CheckNum('txtStock_RID3',2)" runat="server" Width="20px" MaxLength="2"></asp:TextBox>&nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtStock_RIDYear"
                                        ErrorMessage="流水編號格式不對" ValidationExpression="\d{8}">*</asp:RegularExpressionValidator>
                                </td>
                                <td style="width: 15%;" align="right">
                                    日期區間：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtIncome_DateFrom" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_DateFrom')})"
                                            src="../images/calendar.gif" align="absmiddle">~
                                    <asp:TextBox ID="txtIncome_DateTo" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_DateTo')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="結束日期必須大於開始日期"
                                        ControlToCompare="txtIncome_DateFrom" ControlToValidate="txtIncome_DateTo" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" align="right">
                                    <asp:Button ID="btnSearch" CssClass="btn" runat="server" Text="查詢" OnClick="btnSearch_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="lbMsg" Visible="false" runat="server" Text="查無資料" ForeColor="red"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="width:50%" valign="top">
                                                 <asp:TreeView ID="tvOrderForm" runat="server" ShowLines="True" ExpandDepth="1">
                                                </asp:TreeView>
                                            </td>
                                            <td  valign="top">
                                                 <iframe name="OrderDetail" src="Depository001Detail.aspx" id="OrderDetail" marginheight="0" marginwidth="0" scrolling="auto" frameborder="0" width="100%"
                                                    onload="this.height=this.contentWindow.document.body.scrollHeight"></iframe>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
             <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        </div>

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

       

    </form>
</body>
</html>
