<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report031.aspx.cs" Inherits="Report_Report031" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>晶片規格變化查詢</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript">	
    <!--       
        function exportExcel()
        {            
            // 打開列印介面
            window.open('Report031Report.aspx?DateFrom='+ document.getElementById("txtBeginDate").value +'&DateTo='+ document.getElementById("txtEndDate").value,'_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=800,height=650');
            
        }        
        
    //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">晶片規格變化查詢</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="tbl_row" width="100%" border="0" cellpadding="0" cellspacing="2">
                            <tr>
                                <td colspan="6">
                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" RightMaxLenght="10" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table class="tbl_row" width="100%" border="0" cellpadding="0" cellspacing="2">
                            <tr class="tbl_row" valign="baseline">
                                <td width="15%" align="right">
                                    晶片名稱+容量：</td>
                                <td colspan="2">
                                    <asp:DropDownList Style="position: relative" ID="dropWafer" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td width="15%" align="right">
                                    日期區間：</td>
                                <td colspan="2">
                                    進貨日>=<asp:TextBox ID="txtBeginDate" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBeginDate')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                    使用日起<=<asp:TextBox ID="txtEndDate" onfocus="WdatePicker()" runat="server" Width="100px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEndDate')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="7">
                                    <asp:Button ID="btnSearch" OnClick="btnSearch_Click" runat="server" Text="查詢" CssClass="btn">
                                    </asp:Button></td>
                            </tr>
                        </table>
                        <table name="queryResult" id="queryResult" width="100%">
                            <tr valign="baseline">
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tr valign="baseline">
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbWafer" runat="server" AllowPaging="True" OnOnSetDataSource="gvpbWafer_OnSetDataSource"
                                                            AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>"
                                                            NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbWafer_RowDataBound"
                                                            SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                            EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <Columns>
                                                                <asp:BoundField DataField="Name" HeaderText="卡種">
                                                                    <itemstyle horizontalalign="Left" width="20%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Income_Date" HeaderText="最早進貨日">
                                                                    <itemstyle horizontalalign="Left" width="20%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Wafer" HeaderText="晶片名稱+容量">
                                                                    <itemstyle horizontalalign="Left" width="20%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Begin_Date" HeaderText="使用日 起">
                                                                    <itemstyle horizontalalign="Left" width="20%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="End_Date" HeaderText="使用日 迄">
                                                                    <itemstyle horizontalalign="Left" width="20%" />
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
                            <tr>
                                <td align="Right">
                                    <asp:Button ID="btnExport" runat="server" CssClass="btn" OnClientClick="exportExcel();"
                                        Text="匯出Excel格式" /></td>
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
