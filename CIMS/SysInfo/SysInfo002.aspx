<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SysInfo002.aspx.cs" Inherits="SysInfo_SysInfo002" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>警訊功能 </title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">警訊功能</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbWarning" runat="server" AllowPaging="True" DataKeyNames="RID"
                                        OnOnSetDataSource="gvpbWarning_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                        OnRowDataBound="gvpbWarning_RowDataBound" SortAscImageUrl="~/images/asc.gif"
                                        SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                        EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:HyperLinkField DataTextField="Item_Name" HeaderText="作業名稱 " SortExpression="Item_Name"
                                                DataNavigateUrlFields="RID" DataNavigateUrlFormatString="SysInfo002Mod.aspx?ActionType=Edit&amp;RID={0}">
                                                <itemstyle horizontalalign="Left" width="15%" />
                                            </asp:HyperLinkField>
                                            <asp:BoundField DataField="Condition" HeaderText="警訊條件 ">
                                                <itemstyle width="16%" horizontalalign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Warning_Content" HeaderText="警訊信息 ">
                                                <itemstyle horizontalalign="Left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="警訊類別">
                                                <itemstyle width="10%" horizontalalign="Left" />
                                                <itemtemplate>
                                                <asp:Label id="lblWarnType" runat="server"></asp:Label> 
                                                
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="接收人員">
                                                <itemstyle width="20%" horizontalalign="Left" />
                                                <itemtemplate>
                                                <asp:Label id="lblUsers" runat="server"></asp:Label> 
                                                
</itemtemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
