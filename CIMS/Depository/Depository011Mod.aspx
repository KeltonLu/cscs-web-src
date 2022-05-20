<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository011Mod.aspx.cs"
    Inherits="Depository_Depository011Mod" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>卡片庫存移轉修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function exportExcel()
        {
            // 打開列印介面
            window.open('Depository011Report.aspx?Move_ID=' + document.getElementById("hidMove_ID").value,'_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');
        }
        
        function delConfirm()
        {
            if (true == document.getElementById("chkDel").checked)
            {
                return confirm('確認刪除此卡片庫存移轉單訊息?');
            }
            else
            {
                return CheckClientValidate();
            }
        }    
            
        /**
         * 驗證輸入數字
         *
         * @param TextName
         * @param NumLen
         * return 正確數字
         */
        function AtGridCheckNum(obj,NumLen)
        {
            var CardNum = obj.value;
                        
            var patten=new RegExp(/^\d*$/); 
            var IsCal=false;
            
            if(!patten.test(CardNum))
            {
                IsCal=true;
            }
                
            var CardNum = CardNum.replace(/[^0-9]/g,'');
            
            if(CardNum.length>NumLen)
            {
                IsCal=true;
                CardNum=CardNum.substring(0,NumLen);
            }
            
            if(IsCal)
            {
                obj.value=CardNum;
            }
            
            return CardNum;
        }

 //-->
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline">
                    <td colspan="2" height="20px">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td class="PageTitle" style="height: 27px">
                        卡片庫存移轉修改/刪除
                    </td>
                    <td align="right" style="height: 27px">
                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" Height="25px" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnExport" runat="server" CssClass="btn" OnClientClick="exportExcel();"
                            Text="匯出Excel格式" Height="25px" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('Depository011.aspx')" type="button"
                            value="取消" /></td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="0" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" width="15%">
                                    日期：</td>
                                <td width="85%">
                                    <asp:Label ID="lblMove_Date" runat="server"></asp:Label>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    移轉單號：</td>
                                <td>
                                    <asp:Label ID="lblMove_ID" runat="server"></asp:Label>
                                    <input id="hidMove_ID" type="hidden" runat="server" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td colspan="2" align="right" bgcolor="#FFFFFF" style="height: 2px">
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td colspan="2" width="100%">
                                    <table width="100%">
                                        <tr valign="baseline">
                                            <td>
                                                <cc1:GridViewPageingByDB ID="gvpbCardTypeStocksMove" runat="server" OnOnSetDataSource="gvpbCardTypeStocksMove_OnSetDataSource"
                                                    AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                    PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbCardTypeStocksMove_RowDataBound"
                                                    SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                                                    EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <Columns>
                                                        <asp:BoundField DataField="Name" HeaderText="卡種">
                                                            <itemstyle horizontalalign="Left" width="29%" />
                                                        </asp:BoundField>
                                                             <asp:TemplateField HeaderText="移轉後庫存數">
                                                                <itemstyle width="17%" />
                                                                <itemtemplate>
                                                                    <asp:Label runat="server" ID = "lbStocks_Num"></asp:Label>
                                                                </itemtemplate>
                                                           </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="數量">
                                                            <itemstyle width="7%" />
                                                            <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                                <asp:TextBox runat="server" ID = "txtMove_Number" onfocus="DelDouhao(this)" onblur="AtGridCheckNum(this,9);value=GetValue(this.value)"
                                                                    onkeyup="AtGridCheckNum(this,9)" MaxLength="11" style="ime-mode:disabled;text-align: right">
                                                                </asp:TextBox>
                                                            </itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="轉出Perso廠" DataField="From_Factory_Name">
                                                            <itemstyle horizontalalign="Left" width="29%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField HeaderText="轉入Perso廠" DataField="To_Factory_Name">
                                                            <itemstyle horizontalalign="Left" width="29%" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="刪除">
                                                            <itemstyle horizontalalign="Center" width="6%" />
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
                            <tr class="tbl_row" valign="baseline">
                                <td colspan="2" align="right" bgcolor="#FFFFFF" style="height: 2px">
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    &nbsp;
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        &nbsp;<asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" Height="25px" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button
                                ID="btnExport1" runat="server" CssClass="btn" OnClientClick="exportExcel();"
                                Text="匯出Excel格式" Height="25px" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('Depository011.aspx')" type="button"
                            value="取消" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
