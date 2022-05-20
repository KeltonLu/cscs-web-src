<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report014.aspx.cs" Inherits="Report_Report014" %>

<%@ Register Src="../CommUserCtrl/urctrlCardNameSelect.ascx" TagName="urctrlCardNameSelect"
    TagPrefix="uc1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>廠商製卡卡數明細表</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript">
        function CheckReg()
        {
            var vSDate = document.getElementById("txtDate_Time").value;
            var vEDate = document.getElementById("txtDate_Time2").value;
            if (vSDate == "")
            {
                alert("請輸入卡片耗用日期起！");
                return false;
            }
            if (vEDate == "")
            {
                alert("請輸入卡片耗用日期迄！");
                return false;
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager id="ScriptManager1" runat="server" AsyncPostBackTimeout="0"></asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="height: 19px">
                        <font class="PageTitle">月份製損率報表</font></td>
                </tr>
                <tr>
                    <td style="height: 121px">
                    <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0" style="height: 121px">
                        <tr>
                            <td width="10%" align =right >
                                <span style="color: #ff0000"><span style="color: #000000">Perso廠：</span></span></td>
                            <td colspan="3">
                                <asp:DropDownList ID="dropFactory" runat="server">
                                </asp:DropDownList><span style="color: #ff0000"></span></td>
                        </tr>
                       <tr>
                            <td align=right  width="10%">
                                <span style="color: #ff0000">*<span style="color: #000000">日期：</span></span></td>
                            <td colspan="3">
                                <%--Legend 2017/02/04 將長度由8改為10--%>
                                <asp:TextBox ID="txtDate_Time" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                    Width="80px"></asp:TextBox><img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDate_Time')})" src="../images/calendar.gif" />～<asp:TextBox
                                    ID="txtDate_Time2" runat="server" MaxLength="10" onfocus="WdatePicker()" Width="80px"></asp:TextBox>
                                <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDate_Time2')})" src="../images/calendar.gif" /><span style="color: #ff0000"></span></td>
                        </tr>
                        <tr>
                            <td colspan="4">
                            <asp:UpdatePanel id="UpdatePanel1" runat="server">
                               <contenttemplate>
                               <uc1:urctrlCardNameSelect ID="UrctrlCardNameSelect1" runat="server" IsShowAllCard="true" />
                               </contenttemplate>
                            </asp:UpdatePanel>            
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 15px"></td>
                            <td style="height: 15px"></td>
                            <td style="height: 15px"></td>
                            <td style="height: 15px" align =right >
                                <asp:Button ID="btnQuery" runat="server" class="btn" OnClick="btnQuery_Click" Text="查詢" /></td>
                        </tr>
                    </table>
                        <asp:Label ID="Label1" runat="server" ForeColor="Red" Text="Label"></asp:Label></td>
                </tr>
                <tr valign="top">
                    <td style="height: 450px">
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
