<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance004_4.aspx.cs" Inherits="Finance_Finance004_4" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>晶片資本攤銷</title>
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
        window.open('Finance004_4Print.aspx?time='+document.getElementById("hdTime").value,'_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=650');
    }    
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
       <table width="100%" align="center" cellpadding="0" cellspacing="1" border="0">
            <tr style="height: 10px">
                <td>&nbsp;
                    
                </td>
            </tr>
            <tr valign="baseline">
                <td>
                    <font class="PageTitle">晶片資本攤銷</font>
                </td>
            </tr>
            <tr width="100%">
                <td>
                    <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                        <tr  valign="bottom">
                            <td width="15%" align="right"><span class="style1">*</span>日期年月：</td>
                            <td width="25%"><asp:DropDownList style="POSITION: relative" id="dropYear" runat="server">
                                </asp:DropDownList>
                              <asp:DropDownList style="POSITION: relative" id="dropMonth" runat="server">
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
                                </asp:DropDownList></td>
                            <td width="25%" align="right">&nbsp;</td>
                            <td width="25%">&nbsp;</td>
                        </tr>
                        <tr valign="bottom">
                            <td colspan="4" align="right">
                                <asp:Button style="POSITION: relative" id="btnSearch" onclick="btnSearch_Click" runat="server" CssClass="btn" Text="查詢"></asp:Button>
&nbsp;                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="queryResult" style="display:none">
            <td>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="baseline">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbStockCostSNumber" runat="server" AllowPaging="True" OnOnSetDataSource="gvpbStockCostSNumber_OnSetDataSource" FirstPageText="<<" LastPageText=">>"
                                                NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbStockCostSNumber_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" ShowHeader="False">
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
            <tr>
                            <td colspan="14" align="right">
                                <input type="hidden" id="hdTime" runat="server" />
                                <asp:Button id="btnReport" onclick="btnReport_click" runat="server" Text="匯出Excel格式" CssClass="btn"></asp:Button>
                            </td>
                        </tr>
        </table>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" Style="position: relative" Width="146px" />
    </form>
</body>
</html>
