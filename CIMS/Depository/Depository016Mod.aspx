<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository016Mod.aspx.cs"
    Inherits="Depository_Depository016Mod" EnableSessionState="true" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>物料庫存異動作業修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">	
    <!--
        function exportExcel()
        {
            // 打開列印介面
            window.open('Depository016Report.aspx?Transaction_ID=' + document.getElementById("hidTransaction_ID").value,'_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=750,height=850');
        }
        
        function delConfirm()
        {
            if (true == document.getElementById("chkDelete").checked)
            {
                return confirm('確認刪除此物料庫存異動單訊息?');
            }
            
            return true;
            
        }        
        
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
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr valign="baseline">
                    <td colspan="2" height="20px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td class="PageTitle">
                        物料庫存異動作業修改/刪除
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" Text="確定" Height="25px" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnExport" runat="server" CssClass="btn" OnClientClick="exportExcel();"
                            Text="匯出Excel格式" Height="25px" />
                        &nbsp; &nbsp; &nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="btn" OnClick="btnCancel_Click"
                            Text="取消" CausesValidation="False" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">
                                    日期：
                                </td>
                                <td>
                                    <asp:Label ID="lbDate" runat="server" Text="Label"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    異動單號：
                                </td>
                                <td>
                                    <asp:Label ID="lbID" runat="server" Text="Label"></asp:Label>
                                    <input id="hidTransaction_ID" runat="server" type="hidden" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" width="100%">
                        <table id="queryResult" width="100%" cellpadding="0" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbMaterielStocksTransaction" runat="server" AllowPaging="True"
                                        AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                        EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="gvpbMaterielStocksTransaction_OnSetDataSource"
                                        OnRowDataBound="gvpbMaterielStocksTransaction_RowDataBound" PreviousPageText="<"
                                        SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="品名" SortExpression="Materiel_Name">
                                                <itemtemplate>
                                                    <asp:HyperLink id="hlName" runat="server" ></asp:HyperLink>
                                                </itemtemplate>
                                                <itemstyle horizontalalign="Left" width="29%" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="PARAM_Name" HeaderText="異動項目">
                                                <itemstyle horizontalalign="Left" width="28%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Transaction_Amount" HeaderText="數量">
                                                <itemstyle horizontalalign="Right" width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Factory_Name" HeaderText="Perso廠">
                                                <itemstyle horizontalalign="Left" width="28%" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="刪除">
                                                <itemtemplate>
                                                    <asp:ImageButton id="ibtnDelete" runat="server" Height="18px" CausesValidation="false" OnCommand="btnDelete_Command" ImageUrl="~/images/trash_0021.gif" __designer:wfdid="w1"></asp:ImageButton> 
                                                </itemtemplate>
                                                <itemstyle horizontalalign="Center" width="5%" />
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
                        <input id="btnAdd" onclick="var aa=window.showModalDialog('Depository016Detail.aspx?ActionType=Add&Index=-1','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}"
                            type="button" value="新增明細" class="btn" />
                        <asp:Button ID="btnBind" runat="server" OnClick="btnBind_Click" Visible="False" CausesValidation="False" /></td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" width="100%">
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td style="width: 15%">
                                    &nbsp;
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkDelete" runat="server" Font-Size="Small" Text="刪除" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:Button ID="btnSubmit1" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" Text="確定" Height="25px" />
                        &nbsp;&nbsp;&nbsp;<asp:Button ID="btnExport1" runat="server" CssClass="btn" OnClientClick="exportExcel();"
                            Text="匯出Excel格式" Height="25px" />
                        &nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel1" runat="server" CssClass="btn" OnClick="btnCancel_Click"
                            Text="取消" CausesValidation="False" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
