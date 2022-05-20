<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report013.aspx.cs" Inherits="Report_Report013" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>�t�ӻs�d�d�Ʃ��Ӫ�</title>
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
                alert("�п�J����_�I");
                return false;
            }
            if (vEDate == "")
            {
                alert("�п�J������I");
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
                        <font class="PageTitle">�t�Ӫ��Ʈw�s��</font></td>
                </tr>
                <tr>
                    <td style="height: 121px">
                    <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0" style="height: 121px">
                        <tr>
                            <td width="100px" align =right >
                                <span style="color: #ff0000">*</span>����G</td>
                            <td width="250px">
                                <%--Legend 2017/02/04 �N���ץ�8�אּ10--%>
                                <asp:TextBox ID="txtDate_Time" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                    Width="80px"></asp:TextBox>
                                <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDate_Time')})" src="../images/calendar.gif" />��<asp:TextBox
                                    ID="txtDate_Time2" runat="server" MaxLength="10" onfocus="WdatePicker()" Width="80px"></asp:TextBox>
                                <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtDate_Time2')})" src="../images/calendar.gif" /></td>
                            <td width="80px">
                                <span style="color: #ff0000">*</span>Perso�t�G</td>
                            <td>
                                <asp:DropDownList ID="dropFactory" runat="server">
                                </asp:DropDownList></td>
                        </tr>
                       <tr>
                            <td align=right >
                                <span style="color: #ff0000">*</span>��ܤ��e</td>
                            <td>
                                <asp:RadioButton ID="rdoType" runat="server" GroupName="groupRadio" Text="�������O" Checked="True" />
                                <asp:DropDownList ID="ddrType" runat="server">
                                    <asp:ListItem Value="1">�H�ʡBDM</asp:ListItem>
                                    <asp:ListItem Value="2">�H�d��</asp:ListItem>
                                </asp:DropDownList></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:RadioButton ID="rdoSerial" runat="server" Text="��@�~�W" GroupName="groupRadio" />
                                <asp:DropDownList ID="ddrSerial" runat="server">
                                </asp:DropDownList></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td style="height: 15px"></td>
                            <td style="height: 15px"></td>
                            <td style="height: 15px"></td>
                            <td style="height: 15px" align =right >
                                <asp:Button ID="btnQuery" runat="server" class="btn" OnClick="btnQuery_Click" Text="�d��" /></td>
                        </tr>
                    </table>
                        </td>
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
