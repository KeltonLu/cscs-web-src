<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo006_Finance.aspx.cs"
    Inherits="BaseInfo_BaseInfo006_Finance" %>    
<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>代製費用帳務異動項目設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

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
                        <font class="PageTitle">
                            <asp:Label ID="lbTitle" runat="server"></asp:Label></font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" width="15%">
                                    參數值：</td>
                                <td width="90%">
                                    <asp:TextBox ID="txtPARAM_NAME" MaxLength="30" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="right">
                                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />&nbsp;&nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
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
                                            <cc1:gridviewpageingbydb id="gvpbParameter" runat="server" allowpaging="True" ononsetdatasource="gvpbParameter_OnSetDataSource"
                                                allowsorting="True" firstpagetext="<<" lastpagetext=">>" nextpagetext=">" previouspagetext="<"
                                                cssclass="GridView" autogeneratecolumns="False" sortascimageurl="~/images/asc.gif"
                                                sortdescimageurl="~/images/desc.gif" columnsheadertext="" onrowdatabound="gvpbParameter_RowDataBound">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>                                                    
                                                    <asp:TemplateField HeaderText="參數值" SortExpression="Param_Name">
                                                        <itemstyle horizontalalign="Left" width="15%" />
                                                        <itemtemplate>
                                                            <asp:HyperLink id="hlParam_Name" runat="server"></asp:HyperLink>
                                                        </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Param_Comment" HeaderText="備註" SortExpression="Param_Comment" />
                                                    <asp:BoundField DataField="Is_Delete" HeaderText="可否刪除" />
                                                </Columns>
                                            </cc1:gridviewpageingbydb>
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
