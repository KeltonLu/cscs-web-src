<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo006_CardParam.aspx.cs"
    Inherits="BaseInfo_BaseInfo006_CardParam" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>參數設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">參數設定</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbParameter" runat="server" AllowPaging="True" OnOnSetDataSource="gvpbParameter_OnSetDataSource"
                                                AllowSorting="True" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<"
                                                CssClass="GridView" AutoGenerateColumns="False" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" OnRowDataBound="gvpbParameter_RowDataBound"
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" DataKeyNames="RID">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="ParamType_Name" HeaderText="參數名稱" SortExpression="ParamType_Name" >
                                                    <itemstyle width="20%" HorizontalAlign="Left"/>
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="參數值">
                                                        <itemstyle width="10%" HorizontalAlign="Center" />
                                                        <itemtemplate>
<asp:TextBox id="txtParam_Name" style="ime-mode:disabled;text-align: right" runat="server" __designer:wfdid="w3" MaxLength="7" width="50px"></asp:TextBox> <asp:Label id="lbParam" runat="server"  __designer:wfdid="w4"></asp:Label>
</itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="備註">
                                                        <itemstyle width="70%" HorizontalAlign="Center" />
                                                        <itemtemplate>
<asp:TextBox id="txtParam_Comment" runat="server" __designer:wfdid="w2" width="600px" MaxLength="500"></asp:TextBox>
</itemtemplate>
                                                    </asp:TemplateField>
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
                    <td colspan="2" align="right">
                        <asp:Button ID="btnAdd" runat="server" Text="確定" OnClick="btnAdd_Click" CssClass="btn" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
