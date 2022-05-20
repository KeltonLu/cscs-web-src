<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance004_1.aspx.cs" Inherits="Finance_Finance004_1" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>已入庫未出帳明細查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript">	
    <!--
//        function doSearch()
//        {
//		    document.getElementById("trResult").style.display="table-row";		
//	    }
//	    function doSearch1()
//        {
//		    document.getElementById("trResult1").style.display="table-row";		
//	    }
        function exportExcel()
        {            
            // 打開列印介面
            window.open('Finance004_1Print.aspx?RID=' +  document.getElementById("dropFactoryRID").value+'&month='+ document.getElementById("dropMonth").value +'&Group_RID=' + document.getElementById("dropGroup").value+'&DateFrom='+document.getElementById("hidDateFrom").value+'&DateTo='+document.getElementById("hidDateTo").value,'_blank','status=no,menubar=no,location=no,scrollbars=yes,resizable=yes,width=1000,height=650');
            if(document.getElementById("dropFactoryRID").value=="all")
            {
               document.getElementById("trResult").style.display="table-row";
               document.getElementById("trResult1").style.display="table-row";	
            }
        }        
        
    //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table width="100%" align="center" cellpadding="0" cellspacing="1" border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">已入庫未出帳明細查詢</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="bottom">
                                <td width="11%" align="right">
                                    <span class="style1">*</span>空白卡廠：</td>
                                <td style="height: 24px; width: 13%;">
                                    &nbsp;<asp:DropDownList ID="dropFactoryRID" runat="server" Style="position: relative">
                                    </asp:DropDownList></td>
                                <td align="right" style="width: 9%">
                                    <span class="style1">*</span>日期年月：</td>
                                <td style="width: 13%">
                                    &nbsp;<asp:DropDownList ID="dropYear" runat="server" Style="position: relative">
                                    </asp:DropDownList>&nbsp;<asp:DropDownList ID="dropMonth" runat="server" Style="position: relative">
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
                                    </asp:DropDownList>
                                </td>
                                <td width="38%" align="left">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table cellpadding="0" width="100%" border="0">
                                                <tr>
                                                    <td>
                                                        <span class="style1">*</span>用途：&nbsp;<asp:DropDownList ID="dropUse" runat="server"
                                                            Style="position: relative" AutoPostBack="True" OnSelectedIndexChanged="dropUse_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                        <span class="style1">*</span>群組：
                                                        <asp:DropDownList ID="dropGroup" runat="server" Style="position: relative">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="5">
                                    <input id="hidDateTo" runat="server" style="position: relative" type="hidden" />
                                    <input id="hidDateFrom" runat="server" style="position: relative" type="hidden" />
                                    &nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                                        Style="position: relative" Text="查詢" />&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trResult" runat="server" >
                    <td>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td colspan="5">
                                    <table width="100%" cellpadding="0" cellspacing="1">
                                        <tr align="center">
                                            <td class="xl41 " style="height: 19px">
                                                <asp:Label ID="lblTitle" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                        </tr>
                                        <tr align="center">
                                            <td align="left" class="xl41 " style="height: 15px">
                                            </td>
                                        </tr>
                                        <tr valign="baseline">
                                            <td >
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                    <ContentTemplate>
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                            <tr valign="baseline">
                                                                <td colspan="9">
                                                                    <cc1:GridViewPageingByDB ID="gvpbIncome" runat="server" Width="100%" AllowPaging="True"
                                                                        OnOnSetDataSource="gvpbIncome_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                                        OnRowDataBound="gvpbIncome_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                                        ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                                        EditPageUrl="#">
                                                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                            PreviousPageText="&lt;" />
                                                                        <RowStyle CssClass="GridViewRow" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="版面簡稱">
                                                                                <edititemtemplate>
<asp:TextBox id="TextBox1" runat="server" __designer:wfdid="w4"></asp:TextBox>
</edititemtemplate>
                                                                                <itemstyle horizontalalign="Left" width="10%" />
                                                                                <itemtemplate>
<asp:Label id="lblName" runat="server" __designer:wfdid="w1"></asp:Label> 
</itemtemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="空白卡廠">
                                                                                <edititemtemplate>
<asp:TextBox id="TextBox2" runat="server" __designer:wfdid="w6"></asp:TextBox>
</edititemtemplate>
                                                                                <itemstyle horizontalalign="Left" width="10%" />
                                                                                <itemtemplate>
<asp:Label id="lblFactory" runat="server" __designer:wfdid="w5"></asp:Label>
</itemtemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號">
                                                                                <itemstyle horizontalalign="Left" width="10%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="Income_Number" HeaderText="進貨作業數量 " DataFormatString="{0:N0}"
                                                                                HtmlEncode="False">
                                                                                <itemstyle horizontalalign="Right" width="10%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="進貨作業日期 ">
                                                                                <edititemtemplate>
<asp:TextBox id="TextBox5" runat="server" __designer:wfdid="w3"></asp:TextBox>
</edititemtemplate>
                                                                                <itemstyle horizontalalign="Left" width="10%" />
                                                                                <itemtemplate>
<asp:Label id="lblIncome_Date" runat="server" __designer:wfdid="w2"></asp:Label>
</itemtemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="Real_Ask_Number" HeaderText="請款數量" DataFormatString="{0:N0}"
                                                                                HtmlEncode="False">
                                                                                <itemstyle horizontalalign="Right" width="10%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField>
                                                                                <edititemtemplate>
<asp:TextBox id="TextBox3" runat="server" __designer:wfdid="w10"></asp:TextBox>
</edititemtemplate>
                                                                                <itemstyle horizontalalign="Right" width="10%" />
                                                                                <itemtemplate>
<asp:Label id="lblUnitPrice" runat="server" __designer:wfdid="w9"></asp:Label>
</itemtemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField>
                                                                                <edititemtemplate>
<asp:TextBox id="TextBox4" runat="server" __designer:wfdid="w12"></asp:TextBox>
</edititemtemplate>
                                                                                <itemstyle horizontalalign="Right" width="10%" />
                                                                                <itemtemplate>
<asp:Label id="lblPrice" runat="server" __designer:wfdid="w11"></asp:Label>
</itemtemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="Comment" HeaderText="備註">
                                                                                <itemstyle horizontalalign="Left" width="20%" />
                                                                            </asp:BoundField>
                                                                        </Columns>
                                                                    </cc1:GridViewPageingByDB>
                                                                </td>
                                                            </tr>
                                                            <tr class="tbl_title">
                                                                <td class="xl37" width="10%">
                                                                    合計</td>
                                                                <td class="xl37" width="10%">
                                                                    &nbsp;</td>
                                                                <td class="xl37" width="10%">
                                                                    &nbsp;</td>
                                                                <td class="xl37" width="10%">
                                                                    &nbsp;</td>
                                                                <td class="xl37" width="10%">
                                                                    &nbsp;</td>
                                                                <td class="xl37" width="10%">
                                                                    &nbsp;</td>
                                                                <td class="xl37" width="10%">
                                                                    &nbsp;</td>
                                                                <td class="xl37" width="10%" align="right">
                                                                    <asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label></td>
                                                                <td class="xl37" width="20%">
                                                                    &nbsp;</td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trResult1" runat="server" >
                                <td colspan="5" class="tbl_row">
                                    <table cellpadding="0" cellspacing="1">
                                        <tr>
                                            <td width="144" colspan="2" class="tbl_row" style="height: 19px">
                                                <asp:Label ID="lblLast_T_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                            <td width="72" class="xl24" style="height: 19px">
                                            </td>
                                            <td width="156" colspan="2" align="right" style="height: 19px">
                                                <asp:Label ID="lblLast_T_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td colspan="2">
                                                <asp:Label ID="lblP_Num" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                            <td class="xl24" style="height: 19px">
                                            </td>
                                            <td colspan="2" align="right" class="xl30" style="height: 19px">
                                                <asp:Label ID="lblP_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td colspan="2">
                                                <asp:Label ID="lblU_Num" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                            <td class="xl24">
                                            </td>
                                            <td colspan="2" align="right" class="xl30">
                                                <asp:Label ID="lblU_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td colspan="2">
                                                <asp:Label ID="lblD_Num" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                            <td class="xl24" style="height: 19px">
                                            </td>
                                            <td colspan="2" align="right" class="xl32" style="height: 19px">
                                                <asp:Label ID="lblD_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                        </tr>
                                        <tr class="tbl_row">
                                            <td colspan="2">
                                                <asp:Label ID="lblT_Num" runat="server" Style="position: relative" Text="Label"></asp:Label></td>
                                            <td class="xl24">
                                            </td>
                                            <td colspan="2" align="right" class="xl27">
                                                <asp:Label ID="lblT_Number" runat="server" Style="position: relative" Text="Label"></asp:Label>&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5" align="right">
                                    <asp:Button ID="btnReport" OnClick="btnReport_click" runat="server" Text="匯出Excel格式"
                                        CssClass="btn"></asp:Button>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" Style="position: relative" Width="136px" />
    </form>
</body>
</html>
