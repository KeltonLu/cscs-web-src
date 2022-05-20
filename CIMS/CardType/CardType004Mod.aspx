<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="CardType004Mod.aspx.cs"
    Inherits="CardType_CardType004Mod" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>代製項目修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	

        
        function delConfirm()
        {
            if (true == document.getElementById("chkDel").checked)
            {
                return confirm('確認刪除此筆訊息?');
            }
            else
            {
                return CheckClientValidate();
            }
        } 
        
        function returnPage(Url)
        {
            window.location=Url+"?Con=1&List=radProject_Name";
        }        

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
                    <td class="PageTitle">
                        代製項目修改/刪除
                    </td>
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnPage('CardType004.aspx')" type="button"
                            value="取消" /></td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td colspan="2">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                            <tr valign="baseline">
                                <td style="width: 15%" align="right">
                                    代製項目編號：</td>
                                <td colspan="3">
                                    <asp:Label ID="lblProject_Code" runat="server"></asp:Label></td>
                            </tr>
                            <tr valign="baseline">
                                <td style="width: 15%" align="right">
                                    代製項目：</td>
                                <td style="width: 15%" align="left">
                                    <asp:Label ID="lblProject_Name" runat="server" ></asp:Label></td>
                                <td style="width: 15%" align="right">
                                    類別：</td>
                                <td align="left">
                                    <asp:Label ID="lblNormal_SpecialName" runat="server" ></asp:Label></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    Perso廠：</td>
                                <td colspan="3" align="left">
                                    <asp:Label ID="lblFactory_ShortName_CN" runat="server" ></asp:Label></td>
                            </tr>
                            <tr id="tr1" runat="server" visible="false" valign="baseline">
                                <td colspan="4">
                                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                                        <tr>
                                            <td colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 15%" align="right">
                                                單價變化表：</td>
                                            <td align="left">
                                                <cc1:GridViewPageingByDB ID="gvpbPrice" runat="server" OnOnSetDataSource="gvpbPrice_OnSetDataSource"
                                                    AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                    PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                    ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                    EditPageUrl="#">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <Columns>
                                                        <asp:BoundField DataField="Price" HeaderText="單價" DataFormatString="{0:N2}" HtmlEncode="False">
                                                            <itemstyle width="30%" horizontalalign="Right" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Qujian" HeaderText="使用期間" />
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                        <table cellspacing="0" cellpadding="0" border="1" style="width: 100%; border-collapse: collapse;">
                                                            <tr style="color: Black; background-color: #B9BDAA; font-size: Small;">
                                                                <th scope="col" style="width: 30%;">
                                                                    單價</th>
                                                                <th scope="col">
                                                                    使用期間</th>
                                                            </tr>
                                                        </table>
                                                    </EmptyDataTemplate>
                                                </cc1:GridViewPageingByDB>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                已選擇製程：
                                            </td>
                                            <td colspan="3">
                                                <asp:Label ID="lblStepName" runat="server" ></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="tr2" runat="server" visible="false" valign="baseline">
                                <td align="right">
                                    單價：</td>
                                <td colspan="3" align="left">
                                    <asp:Label ID="lblUnit_Price" runat="server" ></asp:Label></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    備註：</td>
                                <td colspan="3" align="left">
                                    <asp:Label ID="lblComment" runat="server" ></asp:Label></td>
                            </tr>
                            <tr id="trDel" runat="server" class="tbl_row">
                                <td>
                                </td>
                                <td colspan="3">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        &nbsp;<asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnPage('CardType004.aspx')" type="button"
                            value="取消" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
