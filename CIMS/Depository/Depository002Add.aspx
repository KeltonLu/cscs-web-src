<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository002Add.aspx.cs"
    Inherits="Depository_Depository002Add" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>採購下單放行作業新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">採購下單放行作業新增</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnOK" runat="server" class="btn" Text="確定" OnClick="btnOK_Click"
                            Visible="False" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnExcel" Height="24" runat="server" Text="匯出EXCEL格式" OnClick="btnExcel_Click"
                            class="btn" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCommit" Height="24" runat="server" Text="提交" class="btn" OnClick="btnCommit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" height="24" type="button" value="取消" class="btn" onclick="returnform();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbORDERFORMDETAIL" runat="server" AllowPaging="True"
                                                DataKeyNames="" OnOnSetDataSource="gvpbORDERFORMDETAIL_OnSetDataSource" AllowSorting="True"
                                                AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbORDERFORMDETAIL_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="false" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="卡種">
                                                        <ItemStyle Width="6%" />
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="Space_Short_RID" runat="server"></asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="number" HeaderText="數量" SortExpression="Number">
                                                        <ItemStyle Width="10%" HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="budget_name" HeaderText="預算" />
                                                    <asp:BoundField DataField="agreement_name" HeaderText="合約" />
                                                    <%--  <asp:BoundField DataField="Base_Price" HeaderText="單價（含稅）" >
                                                            <itemstyle width="10%"　HorizontalAlign="Right" />
                                                        </asp:BoundField>--%>
                                                    <asp:BoundField DataField="Base_Price" HeaderText="合約單價(含稅)">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Change_Unitprice" HeaderText="調整後單價(含稅)">
                                                        <ItemStyle ForeColor="Red" HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Fore_Delivery_Date" HeaderText="預訂交貨日" />
                                                    <asp:BoundField DataField="Wafer_Name" HeaderText="晶片名稱" />
                                                    <asp:TemplateField HeaderText="緊急程度">
                                                        <ItemStyle Width="8%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblis_exigence" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="factory_shortname_cn" HeaderText="交貨地點" />
                                                    <asp:TemplateField HeaderText="刪除">
                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                        <ItemTemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif" OnCommand="btnDelete_Command"></asp:ImageButton>
                                                        </ItemTemplate>
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
                    <td align="left" colspan="2">&nbsp;
                        <input id="Button2" onclick="var aa = window.showModalDialog('Depository002Detaila.aspx?ActionType=Add', '', 'dialogHeight:600px;dialogWidth:600px;'); if (aa != undefined) { ImtBind(); }"
                            type="button" value="新增訂單明細" class="btn" />
                        <asp:Button Style="display: none;" ID="btnAddDetail" runat="server" Text="Button" OnClick="btnAddDetail_Click"
                            CausesValidation="False" />
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnOKD" runat="server" class="btn" Text="確定" OnClick="btnOK_Click"
                            Enabled="False" Visible="False" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnExcelD" Height="24" runat="server" Text="匯出EXCEL格式" OnClick="btnExcel_Click"
                            class="btn" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCommitD" Height="24" runat="server" Text="提交" class="btn" OnClick="btnCommit_Click" />
                        &nbsp;&nbsp;
                        <input id="Button1" type="button" height="24" value="取消" class="btn" onclick="returnform();" />
                    </td>
                </tr>
            </table>
        </div>

        <script type="text/javascript">
            function ImtBind() {
                __doPostBack('btnAddDetail', '');
            }
            function returnform() {
                window.location = "Depository002.aspx?con=1";
            }
        </script>

    </form>
</body>
</html>
