<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo007.aspx.cs" Inherits="BasicInfo_BASEINFO_007" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>未命名頁面</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
    <%--    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
--%>

    <script language="javascript" type="text/javascript">
    function doConfirm()
    {
    if(document.getElementById("hidIsModif").value == "1")
    {
            if (true==confirm('確認保存?'))
            {
                document.getElementById("hidOkOrCancel").value = "1";
            }
            else
            {
                document.getElementById("hidOkOrCancel").value = "0";
            }
      }
    }
    </script>

</head>
<body bgcolor="#ffffff" class="body">
    <form id="Form1" name="form1" method="post" action="" runat="server">
    <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
            border="0">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">營業日資料查詢</font>
                </td>
            </tr>
            <tr>
                <td>
                    <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                        <tr>
                            <td align="right" width="15%">
                                設定日期：</td>
                            <td width="4%">
                                <asp:DropDownList ID="ddlYear" runat="server">
                                </asp:DropDownList></td>
                            <td width="1%">
                                年</td>
                            <td width="3%">
                                <asp:DropDownList ID="ddlMonth" runat="server">
                                </asp:DropDownList></td>
                            <td width="1%">
                                月</td>
                            <td width="10">
                            </td>
                        </tr>
                        <tr valign="bottom">
                            <td colspan="6" align="right">
                                &nbsp;<asp:Button ID="btnQuery" runat="server" class="btn" Text="查詢" OnClick="btnQuery_Click" OnClientClick="return doConfirm();" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="left" colspan="2">
                    &nbsp;</td>
                <td align="center" colspan="7">
                    &nbsp; &nbsp;&nbsp;
                </td>
                <td align="right" colspan="1">
                    &nbsp;</td>
                <td align="right" colspan="1">
                </td>
            </tr>
            <tr style="background-color:#B9BDAA;">
                <td align="left" colspan="1">
                    <asp:LinkButton ID="lbtnPrevYear" runat="server" style="TEXT-DECORATION: none" OnClick="lbtnPrevYear_Click" OnClientClick="return doConfirm();">《</asp:LinkButton>
                    </td>
                <td align="right" colspan="1">
                    <asp:LinkButton ID="lbtnPrevMonth" runat="server" style="TEXT-DECORATION: none" OnClick="lbtnPrevMonth_Click" OnClientClick="return doConfirm();"><</asp:LinkButton></td>
                <td align="center" colspan="8">
                    <asp:Label ID="lblCurrDate" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
                <td align="right" colspan="1">
                    <asp:LinkButton ID="lbtnNextMonth" runat="server" style="TEXT-DECORATION: none" OnClick="lbtnNextMonth_Click" OnClientClick="return doConfirm();">></asp:LinkButton></td>
                <td align="center" colspan="1">
                    &nbsp; &nbsp; &nbsp; &nbsp;
                    <asp:LinkButton ID="lbtnNextYear" runat="server" style="TEXT-DECORATION: none" OnClick="lbtnNextYear_Click" OnClientClick="return doConfirm();">》</asp:LinkButton></td>
                <td align="left" colspan="1">
                    &nbsp;</td>
            </tr>
            <tr>
                <td align="right" colspan="13" valign="top">
                    <asp:Calendar ID="calWorkDate" runat="server" OnDayRender="calWorkDate_DayRender"
                        OnSelectionChanged="calWorkDate_SelectionChanged" CellPadding="0" CellSpacing="0" 
                        BackColor="White" BorderColor="Silver" Font-Names="Verdana" Font-Size="9pt"
                        ForeColor="Black" Height="230px" Width="100%" Font-Bold="False" ShowNextPrevMonth="False" ShowTitle="False">
                        <SelectedDayStyle BackColor="Red" ForeColor="Black" Font-Bold="True" />
                        <TodayDayStyle BackColor="Transparent" />
                        <WeekendDayStyle BackColor="White" BorderStyle="Solid" ForeColor="Black" Font-Bold="False" />
                        <OtherMonthDayStyle ForeColor="Silver" Width="0px" Wrap="False" BorderColor="Silver" />
                        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" VerticalAlign="Middle"
                            HorizontalAlign="Left" />
                        <DayHeaderStyle Font-Bold="True" Font-Size="8pt" BorderStyle="Solid" BorderWidth="2px" />
                        <TitleStyle BackColor="DarkGray" BorderColor="White" BorderWidth="2px" Font-Bold="True"
                            Font-Size="12pt" ForeColor="Black" />
                        <DayStyle BorderStyle="Solid" BorderWidth="2px" />
                    </asp:Calendar>
                </td>
            </tr>
        </table>
        
        <table name="queryResult" width="100%" border="0">
            <tr>
                <td colspan="6" align="right">
                    &nbsp;
                    <asp:Button ID="btnSubmit1" class="btn" runat="server" Text="確定" OnClick="btnSubmit1_Click" />
                    &nbsp;&nbsp; &nbsp;<asp:Button ID="btnCancel" class="btn" runat="server" Text="取消"
                        OnClick="btnCancel_Click" /></td>
            </tr>
        </table>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HiddenField ID="hidOkOrCancel" runat="server" />
        <asp:HiddenField ID="hidIsModif" runat="server" Value="0" />
    </form>
</body>
</html>
