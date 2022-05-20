<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType004.aspx.cs" Inherits="CardType_CardType004" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Perso項目基本檔</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline" height="20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">代製費用設定檔</font>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="0" border="0">
                            <tr valign="top">
                                <td colspan="2">
                                    &nbsp;<asp:RadioButton ID="radStep" runat="server" AutoPostBack="True" Checked="True"
                                        GroupName="PERSO_PROJECT" OnCheckedChanged="radStep_CheckedChanged" Text="製程設定" />
                                    <asp:RadioButton ID="radProject_Name" runat="server" AutoPostBack="True" GroupName="PERSO_PROJECT"
                                        OnCheckedChanged="radProject_Name_CheckedChanged" Text="代製項目設定 " /></td>
                                <td align="right" style="width: 70px">
                                </td>
                                <td align="left" style="width: 150px">
                                </td>
                                <td align="right" style="width: 70px">
                                </td>
                                <td align="left">
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" style="width: 82px">
                                    Perso廠：</td>
                                <td style="width: 255px">
                                    &nbsp;<asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList></td>
                                <td align="right" style="width: 70px">
                                    <asp:Label ID="lblProjectName" runat="server" Text="製程" ></asp:Label>：
                                </td>
                                <td align="left" style="width: 150px">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="20" onkeyup="LimitLengthCheck(this, 30)"></asp:TextBox>
                                </td>
                                <td align="right" style="width: 70px">
                                    <asp:Label ID="lblTime" runat="server" Text="價格期間" ></asp:Label>：
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right" colspan="7">
                                    &nbsp;<asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />
                                    &nbsp;
                                    <asp:Button CssClass="btn" ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click"
                                        CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="top">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbPersoProject" runat="server" AllowPaging="True"
                                                DataKeyNames="RID" OnOnSetDataSource="gvpbPersoProject_OnSetDataSource" AllowSorting="True"
                                                AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbPersoProject_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <Columns>
                                                    <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠">
                                                        <itemstyle horizontalalign="Left" width="25%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Project_Code" HeaderText="代製項目編號" >
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="Project_Name"  HeaderText="代製項目" DataNavigateUrlFields="RID"
                                                        DataNavigateUrlFormatString="CardType004Mod.aspx?ActionType=Edit&amp;RID={0}">
                                                        <itemstyle horizontalalign="Left" width="20%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Normal_Special" HeaderText="類別">
                                                        <itemstyle horizontalalign="Left" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="price1" HeaderText="單價" DataFormatString="{0:N4}" HtmlEncode="False">
                                                        <itemstyle horizontalalign="Right" width="10%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Use_Date_Begin" HeaderText="使用期間 起" />
                                                    <asp:BoundField DataField="Use_Date_End" HeaderText="使用期間 迄 " />
                                                </Columns>
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="top">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbPROJECT_STEP" runat="server" AllowPaging="True"
                                                DataKeyNames="RID" OnOnSetDataSource="gvpbPROJECT_STEP_OnSetDataSource" AllowSorting="True"
                                                AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                EditPageUrl="#" OnRowDataBound="gvpbPROJECT_STEP_RowDataBound">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <Columns>
                                                    <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠">
                                                        <itemstyle horizontalalign="Left" width="25%" />
                                                    </asp:BoundField>
                                                    <asp:HyperLinkField DataTextField="Name" HeaderText="製程" DataNavigateUrlFields="RID"
                                                        DataNavigateUrlFormatString="CardType0041Mod.aspx?ActionType=Edit&amp;RID={0}">
                                                        <itemstyle horizontalalign="Left" width="30%" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="Unit_Price" HeaderText="單價" DataFormatString="{0:N4}" HtmlEncode="False">
                                                        <itemstyle horizontalalign="Right" width="15%" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Use_Date_Begin" HeaderText="價格期間 起" />
                                                    <asp:BoundField DataField="Use_Date_End" HeaderText="價格期間 迄 " />
                                                </Columns>
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
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
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
