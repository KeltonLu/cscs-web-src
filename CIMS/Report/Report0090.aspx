<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report0090.aspx.cs" Inherits="Report_Report0090" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>廠商卡片庫存日報表</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript">
        function CheckReg()
        {
            var vDate = document.getElementById("txtDate_Time").value;
            if (vDate == "")
            {
                alert("請輸入日期！");
                return false;
            }
            return true;
        }
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
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="height: 19px">
                        <font class="PageTitle">廠商卡片庫存日報表</font></td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    <font color="red">*</font>日期：</td>
                                <td style="width: 11%">
                                    <asp:TextBox ID="txtDate_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtDate_Time')})"
                                            src="../images/calendar.gif" align="absmiddle">
                                </td>
                                <td align="right" style="width: 13%">
                                    <font color="red">*</font>Perso廠：</td>
                                <td style="width: 17%">
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    </td>
                                <td align="left" style="width: 17%">
                                </td>
                                <td align="right">
                                    </td>
                                <td align="left">
                                </td>
                            </tr>
                             <tr valign="baseline">
                                <td width="10%" align="right">
                                    <font color="red">*</font>用途：</td>
                                <td style="width: 10%">
                                   <asp:DropDownList Style="position: relative" ID="dropUse" runat="server" OnSelectedIndexChanged="dropUse_SelectedIndexChanged"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                                </td>
                                <td align="right" style="width: 13%">
                                    <font color="red">*</font>群組：</td>
                                <td style="width: 17%">
                                    <asp:DropDownList Style="position: relative" ID="dropGroup" runat="server">
                                </asp:DropDownList>
                                </td>
                                <td align="right">
                                    </td>
                                <td align="left" style="width: 17%">
                                </td>
                                <td align="right">
                                    </td>
                                <td align="left">
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="8" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                </td>
                            </tr>
                        </table>
                        <asp:Label ID="Label1" runat="server" ForeColor="Red" Width="194px"></asp:Label></td>
                </tr>
                <tr valign="top">
                    <td style="height: 415px">
                        <div style="overflow:auto;">
                             <rsweb:reportviewer id="ReportView" runat="server" processingmode="Remote"
                                width="100%" ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False"></rsweb:reportviewer>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        &nbsp;</td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
