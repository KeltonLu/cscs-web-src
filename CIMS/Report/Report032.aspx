<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report032.aspx.cs" Inherits="Report_Report032" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>紙品物料月耗用預測報表</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

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
                    <td>
                        <font class="PageTitle">紙品物料月耗用預測報表</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                                    <tr valign="baseline">
                                        <td width="10%" align="right">
                                            <font color="red">*</font>物料類別：</td>
                                        <td style="width: 18%">
                                            <asp:DropDownList ID="dropMaterialType" runat="server" OnSelectedIndexChanged="dropMaterialType_SelectedIndexChanged"
                                                AutoPostBack="True">
                                                <asp:ListItem>信封</asp:ListItem>
                                                <asp:ListItem>寄卡單</asp:ListItem>
                                                <asp:ListItem>DM</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right">
                                            品名：</td>
                                        <td align="left" style="width: 50%">
                                            <asp:DropDownList ID="dropMaterial_RID" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 10%">
                                            Perso廠：</td>
                                        <td style="width: 18%">
                                            <asp:DropDownList ID="dropFactory" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right">
                                            <font color="red">*</font>依據過去X月的月平均：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropAction" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <table cellpadding="2" width="100%" border="0">
                            <tr class="tbl_row">
                                <td colspan="2">
                                    A：預估每月新申請件耗用量</td>
                                <td colspan="2">
                                    D：預估該月換卡耗用量</td>
                            </tr>
                            <tr class="tbl_row">
                                <td colspan="2">
                                    B：預估每月掛補耗用量</td>
                                <td colspan="2">
                                    E：預估該月總耗用量</td>
                            </tr>
                            <tr class="tbl_row">
                                <td colspan="4">
                                    C：預估每月毀補耗用量</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td colspan="8" align="right">
                        <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                    </td>
                </tr>       
                <tr valign="top">
                    <td style="height: 415px">
                        <div style="overflow:auto;">
                            <rsweb:reportviewer id="ReportView1" runat="server" processingmode="Remote"
                                width="100%" ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False"></rsweb:reportviewer>
                        </div>
                    </td>
                </tr>         
            </table>
        </div>
       
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
