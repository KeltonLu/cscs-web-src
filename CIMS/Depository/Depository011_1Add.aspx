<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository011_1Add.aspx.cs"
    Inherits="Depository_Depository011_1Add" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>物料庫存移轉作業新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">	
    <!--
    function ImtBind()
    {                
        __doPostBack('btnBind',''); 
    }
    //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="1" style="width: 100%">
                <tr valign="baseline" height="20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">物料庫存移轉作業新增</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            Text="確定" />
                        &nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="btn" OnClick="btnCancel_Click"
                            Text="取消" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table class="tbl_row" width="100%" cellpadding="0" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td width="10%" align="right">
                                    <span class="style1">*</span>日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMove_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" runat="server" ID= "img" onclick="WdatePicker({el:$dp.$('txtMove_Date')})" src="../images/calendar.gif" />                                    
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table width="100%">
                            <tr valign="baseline">
                                <td>
                                        <cc1:GridViewPageingByDB ID="gvpbMaterielStocksMove" runat="server" AllowPaging="True"
                                        AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                        EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvpbMaterielStocksMove_OnSetDataSource"
                                        OnRowDataBound="gvpbMaterielStocksMove_RowDataBound" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                                        SortDescImageUrl="~/images/desc.gif">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="品名" SortExpression="Materiel_Name">
                                                <itemstyle horizontalalign="Left" width="29%" />
                                                <itemtemplate>
<asp:HyperLink id="hlName" runat="server" Text='<%# Eval("Materiel_Name") %>' NavigateUrl="Depository011_1Detail.aspx"></asp:HyperLink> 
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Move_Number" HeaderText="數量">
                                                <itemstyle horizontalalign="Right" width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="From_Factory_Name" HeaderText="轉出Perso廠">
                                                <itemstyle horizontalalign="Left" width="28%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="To_Factory_Name" HeaderText="轉入Perso廠">
                                                <itemstyle horizontalalign="Left" width="28%" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="刪除">
                                                <itemstyle horizontalalign="Center" width="5%" />
                                                <itemtemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDelete_Command"></asp:ImageButton>
                                                            
</itemtemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="left" colspan="2">
                        <asp:Button id="detailButton" class="btn"  onclick="detailButton_click"
                            Text="新增明細" runat="server" />
                        <asp:Button ID="btnBind" runat="server" CausesValidation="False" OnClick="btnBind_Click"
                            Visible="False" />
                        <asp:HiddenField ID="hidMoveId" runat="server" /></td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnSubmit1" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            Text="確定" />
                        &nbsp;<asp:Button ID="btnCancel1" runat="server" CssClass="btn" OnClick="btnCancel_Click"
                            Text="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
