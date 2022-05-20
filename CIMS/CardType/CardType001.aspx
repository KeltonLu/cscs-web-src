<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType001.aspx.cs" Inherits="CardType_CardType001" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡種群組維護作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">卡種群組維護作業</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td width="10%" align="right">
                                    用途：
                                </td>
                                <td width="90%">
                                    <asp:DropDownList ID="drpParam_Name" runat="server" DataTextField="param_name" DataValueField="param_code">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="6" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button Text="新增" ID="btnAdd" class="btn" runat="server" OnClick="btnAdd_Click" />                                    
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table id="queryResult" width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbCARDGROUP" runat="server" AllowPaging="True" DataKeyNames="RID"
                                                OnOnSetDataSource="gvpbCARDGROUP_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                OnRowDataBound="gvpbCARDGROUP_RowDataBound" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Param_Name" HeaderText="用途" SortExpression="Param_Name" />
                                                    <asp:HyperLinkField DataTextField="Group_Name" HeaderText="群組名稱" SortExpression="Group_Name"
                                                        DataNavigateUrlFields="RID" DataNavigateUrlFormatString="CardType001Edit.aspx?ActionType=Edit&RID={0}">
                                                        <itemstyle horizontalalign="Left" width="65%" />
                                                    </asp:HyperLinkField>
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
