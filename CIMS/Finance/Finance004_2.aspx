<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance004_2.aspx.cs" Inherits="Finance_Finance004_2" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>庫存成本查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript">
	function doSearch()
	{
		document.getElementById("queryResult").style.display="";
	}	 
	function exportExcel()
    {
        document.getElementById("queryResult").style.display="";      
        // 打開打印介面
        window.open('Finance004_2Print.aspx?month='+ document.getElementById("dropMonth").value +'&year='+ document.getElementById("dropYear").value +'&Group_RID=' + document.getElementById("dropGroup").value+'&DateFrom='+document.getElementById("hidDateFrom").value+'&DateTo='+document.getElementById("hidDateTo").value+'&time='+document.getElementById("HidTime").value,'_blank','status=no,menubar=no,location=no,scrollbars=yes,resizable=yes,width=1024,height=700');	            
    }  
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600">
            </asp:ScriptManager>
            <table width="100%" cellpadding="2" cellspacing="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">月庫存成本查詢</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="bottom">

                                <td align="right" style="width: 5%">
                                    <span class="style1">*</span>日期年月：</td>
                                <td width="46%">
                                    <label>
                                        <asp:DropDownList ID="dropYear" runat="server" Style="position: relative">
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="dropMonth" runat="server" Style="position: relative">
                                            <asp:ListItem>1</asp:ListItem>
                                            <asp:ListItem>2</asp:ListItem>
                                            <asp:ListItem>3</asp:ListItem>
                                            <asp:ListItem>4</asp:ListItem>
                                            <asp:ListItem>5</asp:ListItem>
                                            <asp:ListItem>6</asp:ListItem>
                                            <asp:ListItem>7</asp:ListItem>
                                            <asp:ListItem>8</asp:ListItem>
                                            <asp:ListItem>9</asp:ListItem>
                                            <asp:ListItem>10</asp:ListItem>
                                            <asp:ListItem>11</asp:ListItem>
                                            <asp:ListItem>12</asp:ListItem>
                                        </asp:DropDownList></label></td>
                            </tr>
                            <tr align="left" valign="bottom">
                                <td colspan="4">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                                <tr>
                                                    <td width="14%" align="right">
                                                        <span class="style1">*</span>用途：</td>
                                                    <td colspan="3">
                                                        <asp:DropDownList Style="position: relative" ID="dropUse" runat="server" AutoPostBack="True"
                                                            OnSelectedIndexChanged="dropUse_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                        <span class="style1">*</span>群組：
                                                        <asp:DropDownList Style="position: relative" ID="dropGroup" runat="server">
                                                        </asp:DropDownList>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    <input id="hidDateTo" runat="server" style="position: relative" type="hidden" />
                                    <input id="hidDateFrom" runat="server" style="position: relative" type="hidden" />
                                    <input id="HidTime" runat="server" style="position: relative" type="hidden" />
                                    <asp:Button Style="position: relative" ID="btnSearch" OnClick="btnSearch_Click" runat="server"
                                        CssClass="btn" Text="查詢"></asp:Button>                                     
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="2" id="queryResult" runat="server">
                            <tr class="tbl_blank">
                                <td colspan="19" align="center" class="xl22">
                                    <span class="tbl_blank">
                                        <asp:Label ID="lblTitle" runat="server" Style="position: relative" Text="Label"></asp:Label></span></td>
                            </tr>
                            <tr class="tbl_blank">
                                <td colspan="19" align="left" class="xl22">
                                    <span class="tbl_blank">日期區間：<asp:Label ID="lblDate" runat="server" Style="position: relative"
                                        Text="Label"></asp:Label></span></td>
                            </tr>
                            <tr valign="baseline">
                                <td style="width: 1184px">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tr valign="baseline">
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbStockCost" runat="server" AllowPaging="True" OnOnSetDataSource="gvpbStockCost_OnSetDataSource"
                                                            FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                            OnRowDataBound="gvpbStockCost_RowDataBound" SortAscImageUrl="~/images/asc.gif"
                                                            SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                            EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                        </cc1:GridViewPageingByDB>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                
                <tr>
                    <td class="tbl_row">
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" id="result2" runat="server">
                            <tr align="center">
                                <td colspan="2" width="60%">
                                    <asp:Label ID="lblTitle1" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="xl23">
                                    <asp:Label ID="lblLast_W_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right">
                                    <asp:Label ID="lblLast_W_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr id="result3"  runat="server">
                                <td class="xl23">
                                    <asp:Label ID="lblS_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right">
                                    <asp:Label ID="lblS_Numbers" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr id="result4"  runat="server">
                                <td class="xl23">
                                    <asp:Label ID="lblF_Num" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lblF_Numbers" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr id="result5"  runat="server">
                                <td class="xl23">
                                    <asp:Label ID="lblUseOutNum" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                <td align="right">
                                    <asp:Label ID="lblUseOutNumber" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                             <tr>
                                <td>
                                    <asp:Label ID="lblXH_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right" class="xl24 style2">
                                    <asp:Label ID="lblXH_Numer" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblTZ_Num" runat="server" Style="position: relative" Text="0"></asp:Label>&nbsp;</td>
                                <td align="right" class="xl24 style2">
                                    <asp:Label ID="lblTZ_Numer" runat="server" Style="position: relative" Text="0"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="xl23" style="height: 16px">
                                    <asp:Label ID="lblBack_Num" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                <td align="right" style="height: 16px">
                                    <asp:Label ID="lblBack_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td style="height: 16px">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="xl23" style="height: 15px">
                                    <asp:Label ID="lblP_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right" style="height: 15px">
                                    <asp:Label ID="lblP_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td style="height: 15px">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="xl23">
                                    <asp:Label ID="lblT_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right">
                                    <asp:Label ID="lblT_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="xl27" style="height: 15px">
                                    <asp:Label ID="lblA_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right" style="height: 15px">
                                    <asp:Label ID="lblA_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td style="height: 15px">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="xl23">
                                    <asp:Label ID="lblW_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right">
                                    <asp:Label ID="lblW_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblD_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td align="right" class="xl24 style2">
                                    <asp:Label ID="lblD_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                           
                        </table>
                    </td>
                </tr>               
                
                <tr class="tbl_title">
                    <td colspan="19" align="right" class="tbl_row">
                        <asp:Button ID="btnReport" OnClick="btnReport_click" runat="server" CssClass="btn"
                            Text="匯出Excel格式"></asp:Button></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" Style="position: relative" Width="150px" />
    </form>
</body>
</html>
