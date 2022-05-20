<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance004_3.aspx.cs" Inherits="Finance_Finance004_3" %>

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
        window.open('Finance004_3Print.aspx?date='+ document.getElementById("txtDate").value +'&Group_RID=' + document.getElementById("dropGroup").value+'&time='+document.getElementById("HidTime").value,'_blank','status=no,menubar=no,location=no,scrollbars=yes,resizable=yes,width=1024,height=600');
        	            
    }  
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table width="100%" cellpadding="2" cellspacing="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">庫存成本查詢</font>
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="bottom">
                                <td width="14%" align="right">
                                    <span class="style1">*</span>Perso廠：</td>
                                <td width="28%">
                                    <asp:DropDownList Style="position: relative" ID="dropFactoryRID" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td width="12%" align="right">
                                    <span class="style1">*</span>日期：</td>
                                <td width="46%">
                                    <label>
                                        <asp:TextBox ID="txtDate" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                        <%--<img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDate')})" src="../images/calendar.gif" />--%>
                                    </label>
                                </td>
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
                                    &nbsp;
                                    <input id="HidTime" runat="server" style="position: relative" type="hidden" />
                                    <asp:Button Style="position: relative" ID="btnSearch" OnClick="btnSearch_Click" runat="server"
                                        CssClass="btn" Text="查詢"></asp:Button>
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="queryResult" runat="server">
                    <td>
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
