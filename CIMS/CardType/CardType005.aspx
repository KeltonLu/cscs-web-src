<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType005.aspx.cs" Inherits="CardType_CardType005" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Perso廠與卡種設定表</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">Perso廠與卡種設定</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="tbl_row" width="100%" border="0" cellpadding="0" cellspacing="3">
                            <tr>
                                <td colspan="9">
                                    
                                            <table width="100%" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="right" style="width: 15%">
                                                        Perso廠：</td>
                                                    <td align="left" colspan="3">
                                                        <asp:DropDownList ID="dropFactory" runat="server">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="4" valign="top">
                                                        <uc1:uctrlCardType ID="uctrlCARDNAME" runat="server"></uc1:uctrlCardType>
                                                    </td>
                                                </tr>
                                            </table>
                                      
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" colspan="1" style="width: 147px">
                                </td>
                                <td colspan="8" align="right">
                                    &nbsp; &nbsp;
                                    <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn" OnClick="btnSearch_Click" Height="24px" />
                                    &nbsp;
                                    <asp:Button ID="btnAdd_Base" runat="server" Text="新增基本設定" CssClass="btn" OnClick="btnAdd_Base_Click" Height="24px" />
                                    &nbsp;
                                    <asp:Button ID="btnAdd_Special" runat="server" Text="新增特殊設定" CssClass="btn" OnClick="btnAdd_Special_Click" Height="24px" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="height: 15px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvPerso_CardType" runat="server" AllowPaging="True"
                                                AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                                DataKeyNames="RID" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvPerso_CardType_OnSetDataSource"
                                                OnRowDataBound="gvPerso_CardType_RowDataBound" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" OnSelectedIndexChanged="gvPerso_CardType_SelectedIndexChanged">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <RowStyle CssClass="GridViewRow" HorizontalAlign="Center" />
                                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" Visible="False" />
                                                <Columns>
                                                    <asp:BoundField DataField="RID" HeaderText="RID" />
                                                    <asp:BoundField DataField="Factory_RID" HeaderText="Factory_RID" />
                                                    <asp:BoundField DataField="TYPE" HeaderText="TYPE" />
                                                    <asp:BoundField DataField="AFFINITY" HeaderText="AFFINITY" />
                                                    <asp:BoundField DataField="PHOTO" HeaderText="PHOTO" />
                                                    <asp:BoundField DataField="CardType_RID" HeaderText="CardType_RID" />
                                                    <asp:TemplateField HeaderText="卡種 " SortExpression="CardType_RID">
                                                        <itemstyle horizontalalign="Center" width="100px" />
                                                        <itemtemplate>
                                                            <asp:HyperLink id="hlModify" runat="server"></asp:HyperLink>
                                                        
</itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="CardType_RID" HeaderText="卡片全名" SortExpression="CardType_RID" />
                                                    <asp:BoundField DataField="CardType_RID" HeaderText="小計Type" SortExpression="CardType_RID" />
                                                    <asp:HyperLinkField DataNavigateUrlFields="CardType_RID,RID,Factory_RID" DataNavigateUrlFormatString="CardType005ModBase.aspx?ActionType=Edit&amp;CardType_RID={0}&amp;RID={1}&amp;Factory_RID={2}"
                                                        DataTextField="B_FNAME" HeaderText="基本Perso廠" SortExpression="B_FNAME" />
                                                    <asp:BoundField DataField="S_FNAME" HeaderText="特殊Perso廠" HtmlEncode="False" SortExpression="S_FNAME">
                                                        <itemstyle horizontalalign="Center" width="15%" />
                                                        <headerstyle width="15%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="RATE_VALUE" HeaderText="比率 " HtmlEncode="False" SortExpression="RATE_VALUE">
                                                        <itemstyle horizontalalign="Center" width="8%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="NUM_VALUE" HeaderText="數量" SortExpression="NUM_VALUE">
                                                        <itemstyle horizontalalign="Center" width="16%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="PRIORITY" HeaderText="優先級" SortExpression="PRIORITY" />
                                                    <asp:BoundField DataField="Percentage_Number" HeaderText="Percentage_Number" />
                                                    <asp:BoundField DataField="B_Factory_ID" />
                                                    <asp:BoundField DataField="S_Factory_ID" />
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" style="height: 27px">
                        &nbsp; &nbsp;&nbsp;
                        <asp:Button ID="btnUpFile" runat="server" OnClick="btnUpFile_Click" Text="上傳設定表" CssClass="btn" Height="24px" />
                        <asp:Button ID="btnExport" runat="server" Text="匯出Txt格式" CssClass="btn" OnClick="btnExport_Click" Height="24px" /></td>
                </tr>
            </table>
        </div>
        <asp:Label ID="Label1" runat="server" Visible="False"></asp:Label>
    </form>
</body>
</html>
