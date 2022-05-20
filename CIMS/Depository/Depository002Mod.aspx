<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="Depository002Mod.aspx.cs"
    Inherits="Depository_Depository002Mod" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>採購下單作業修改/删除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>

            <script type="text/javascript">
            function ImtBind()
            {
                __doPostBack('btnAddDetail',''); 
            }
            function ConfirmDel()
            {                 
                if(document.getElementById('chkDel').checked)  
                {
                    return confirm('是否刪除列表中的訊息');
                }                      
                return true;                
            }  
            function returnform()
            {
                window.location="Depository002.aspx?con=1";
            }   
            </script>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">
                            <asp:Label ID="lblTitle" runat="server"></asp:Label></font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" Height="24" runat="server" Text="確定" class="btn" OnClientClick="return ConfirmDel();"
                            OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnExcel" Height="24" runat="server" Text="匯出EXCEL格式" class="btn"
                            OnClick="btnExcel_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCommit" Height="24" runat="server" Text="提交" class="btn" OnClick="btnCommit_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnPass" Height="24" runat="server" Text="放行" class="btn" OnClick="btnPass_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnReturn" Height="24" runat="server" Text="退回" class="btn" OnClick="btnReturn_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" height="24" type="button" value="取消" class="btn" onclick="returnform();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr>
                                <td>
                                    <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" width="10%">
                                                訂單編號：
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="lblOrderForm_RID" runat="server"></asp:Label>
                                            </td>
                                            <td align="right" width="10%">
                                                結案狀態：
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="lblCase_Status" runat="server"></asp:Label>
                                            </td>
                                            <td width="20%" align="right">
                                                空白卡廠：</td>
                                            <td style="width: 20%">
                                                <asp:Label ID="lblFactory_ShortName_CN" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr valign="baseline">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbORDERFORMDETAIL" runat="server" AllowPaging="True"
                                                OnOnSetDataSource="gvpbORDERFORMDETAIL_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                OnRowDataBound="gvpbORDERFORMDETAIL_RowDataBound" SortAscImageUrl="~/images/asc.gif"
                                                SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                                EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="訂單流水編號">
                                                        <itemtemplate>
                                                                <asp:HyperLink id="OrderForm_Detail_RID" runat="server"></asp:HyperLink>
                                                            
</itemtemplate>
                                                        <itemstyle width="12%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="卡種">
                                                        <itemtemplate>
                                                            <asp:Label id="lblSpace_Short_RID" runat="server"/>                                            
                                                        
</itemtemplate>
                                                        <itemstyle width="8%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Number" HeaderText="數量">
                                                        <itemstyle width="10%" horizontalalign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Base_Price" HeaderText="合約單價(含稅)">
                                                        <itemstyle horizontalalign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Change_Unitprice" HeaderText="調整後單價(含稅)">
                                                        <itemstyle forecolor="Red" horizontalalign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Budget_Name" HeaderText="預算" />
                                                    <asp:BoundField DataField="Agreement_Name" HeaderText="合約" />
                                                    <asp:BoundField DataField="Fore_Delivery_Date" HeaderText="預訂交貨日" />
                                                    <asp:BoundField DataField="Wafer_Name" HeaderText="晶片名稱" />
                                                    <asp:TemplateField HeaderText="緊急程度">
                                                        <itemtemplate>
                                                            <asp:Label id="lblis_exigence" runat="server"/>                                            
                                                        
</itemtemplate>
                                                        <itemstyle width="8%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="factory_shortname_cn" HeaderText="交貨地點" />
                                                    <asp:TemplateField HeaderText="刪除">
                                                        <itemtemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDelete_Command"></asp:ImageButton>
                                                        
</itemtemplate>
                                                        <itemstyle horizontalalign="Center" width="100px" />
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
                <tr valign="baseline">
                    <td align="left">
                        &nbsp;<input id="Button2" onclick="var aa=window.showModalDialog('Depository002Detail.aspx?ActionType=Add','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}"
                            type="button" value="新增訂單明細" class="btn" runat="server" />
                        <asp:Button style="display: none;" ID="btnAddDetail" runat="server" Text="Button" OnClick="btnAddDetail_Click"
                            CausesValidation="False" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="tbl_row" runat="server" id="trdel">
                    <td colspan="2">
                        <asp:CheckBox ID="chkDel" Height="24" runat="server" Text="刪除整筆訂單" />
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <asp:Button ID="btnEditD" Height="24" runat="server" Text="確定" class="btn" OnClientClick="return ConfirmDel();"
                            OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnExcelD" Height="24" runat="server" Text="匯出EXCEL格式" class="btn"
                            OnClick="btnExcel_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCommitD" Height="24" runat="server" Text="提交" class="btn" OnClick="btnCommit_Click" />
                        &nbsp;
                        <asp:Button ID="btnPassD" Height="24" runat="server" Text="放行" class="btn" OnClick="btnPass_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnReturnD" Height="24" runat="server" Text="退回" class="btn" OnClick="btnReturn_Click" />
                        &nbsp;
                        <input id="Button1" height="24" type="button" value="取消" class="btn" onclick="returnform();" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
