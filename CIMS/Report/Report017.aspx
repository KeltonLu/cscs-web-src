<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report017.aspx.cs" Inherits="Report_Report017" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType5.ascx" TagName="uctrlCardType5" TagPrefix="uc5" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc2" %>
<%@ Register Src="../CommUserCtrl/urctrlCardNameSelect.ascx" TagName="urctrlCardNameSelect"
    TagPrefix="uc1" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>�C��o�d�Ʋέp��</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>



    <script language="javascript" type="text/javascript">
        function CheckReg()
        {
            var vSDate = document.getElementById("txtDate_Time").value;
            var vEDate = document.getElementById("txtDate_Time2").value;
            if (vSDate == "")
            {
                alert("�п�J�d���ӥΤ���_�I");
                return false;
            }
            if (vEDate == "")
            {
                alert("�п�J�d���ӥΤ�����I");
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
            &nbsp;<table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr valign="baseline">
                    <td style="height: 19px">
                        <font class="PageTitle">�C��o�d�Ʋέp��</font></td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td style="height: 15px" colspan="4">
                                    <uc5:uctrlCardType5 ID="UctrlCardType5_1" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 15px"></td>
                                <td style="height: 15px"></td>
                                <td style="height: 15px"></td>
                                <td style="height: 15px" align="right">
                                    <asp:Button ID="btnQuery" runat="server" class="btn" OnClick="btnQuery_Click" Text="�d��" /></td>
                            </tr>
                        </table>
                        <asp:Label ID="Label1" runat="server" ForeColor="Red" Text="Label"></asp:Label></td>
                </tr>
                <tr valign="top">
                    <td style="height: 450px">
                        <div style="overflow:auto;">
                            <rsweb:ReportViewer ID="ReportView" runat="server" ProcessingMode="Remote" Width="100%"
                                ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False">
                            </rsweb:ReportViewer>
                        </div> 
                    </td>
                </tr>
                <tr>
                    <td align="right">&nbsp;</td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
