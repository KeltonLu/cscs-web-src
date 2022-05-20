<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo006_MatAdd.aspx.cs"
    Inherits="BaseInfo_BaseInfo006_MatAdd" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>參數設定新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                <ContentTemplate>
                    <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                        <tr>
                            <td colspan="2" style="height: 20px">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <font class="PageTitle">
                                    <asp:Label ID="lbTitle" runat="server"></asp:Label></font>
                            </td>
                            <td align="right">
                                <asp:Button ID="btnAdd" runat="server" Text="確定" OnClick="btnAdd_Click" CssClass="btn" />
                                &nbsp;
                                <input class="btn" onclick="window.location='<%=strURLPath%>&Con=1';" id="btnCancel"
                                    type="button" value="取消" />
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table width="100%" cellspacing="0">
                                    <tr class="tbl_row">
                                        <td align="right">
                                            <span class="style1">*</span>合併計算：</td>
                                        <td>
                                            &nbsp;
                                            <asp:RadioButton ID="radIs_Merge" runat="server" GroupName="rbgIs_Merge" Text="是" />&nbsp;
                                            <asp:RadioButton ID="radIs_Merge_N" runat="server" Checked="True" GroupName="rbgIs_Merge" Text="否" />
                                        </td>
                                    </tr>
                                    <tr class="tbl_row">
                                        <td align="right">
                                            <span class="style1">*</span>參數名稱：</td>
                                        <td>
                                            <asp:TextBox ID="txtParam_Name" runat="server" MaxLength="50" onkeyup=" LimitLengthCheck(this, 50)" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtParam_Name"
                                                ErrorMessage="參數值不能為空">*</asp:RequiredFieldValidator>
                                            <input id="hdParamType_Code" runat="server" type="hidden" />
                                            <input id="hdParamType_Name" runat="server" type="hidden" />
                                        </td>
                                    </tr>
                                    <tr class="tbl_row">
                                        <td align="right">
                                            備註：</td>
                                        <td>
                                            <asp:TextBox ID="txtParam_Comment" runat="server" Height="95px" Width="400px" TextMode="MultiLine" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <asp:Button ID="btnAdd1" runat="server" Text="確定" OnClick="btnAdd_Click" CssClass="btn" />
                                &nbsp;
                                <input class="btn" onclick="window.location='<%=strURLPath%>&Con=1';" id="btnCancel1"
                                    type="button" value="取消" />
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                ShowSummary="False" />
        </div>
    </form>
</body>
</html>
