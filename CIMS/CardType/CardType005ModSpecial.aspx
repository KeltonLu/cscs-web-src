<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="CardType005ModSpecial.aspx.cs"
    Inherits="CardType_CardType005ModSpecial" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Perso廠與卡種特殊設定修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<script language="javascript" type="text/javascript">	
<!--
    function delConfirm() {
        if (true == document.getElementById("chkDel").checked) {
            return confirm('確認刪除此筆訊息?');
        }
    }
    //-->
</script>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">&nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">Perso廠與卡種特殊設定修改/刪除</font>
                    </td>
                    <td align="right">&nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                        Text="確定" OnClientClick="return delConfirm();" />&nbsp;
                        <asp:Button ID="btnCancel1" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table id="errorMessage" style="display: none">
                            <tr>
                                <td>
                                    <font class="error_message">資料保存成功！</font>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" class="tbl_row" border="0" cellpadding="0" cellspacing="0">
                            <tr class="tbl_row" valign="baseline">
                                <td width="10%" align="right">卡種：</td>
                                <td>
                                    <asp:Label ID="lbCardType" runat="server"></asp:Label></td>
                                <td align="center">&nbsp;</td>
                                <td width="30%">&nbsp;
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td width="10%" align="right">&nbsp;</td>
                                <td>
                                    <asp:RadioButtonList ID="rablNumber_Percentage" runat="server" RepeatDirection="Horizontal"
                                        OnSelectedIndexChanged="rablNumber_Percentage_SelectedIndexChanged" AutoPostBack="True">
                                        <asp:ListItem>比率</asp:ListItem>
                                        <asp:ListItem>數量</asp:ListItem>
                                    </asp:RadioButtonList></td>
                                <td align="center">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />
                                </td>
                                <td width="30%">&nbsp;
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="left" colspan="4">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                <tr id="trGrid" style="display: table-row">
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvPerso_CardType" runat="server" AutoGenerateColumns="False"
                                                            ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                                                            EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                            OnOnSetDataSource="gvPerso_CardType_OnSetDataSource" OnRowDataBound="gvPerso_CardType_RowDataBound"
                                                            PreviousPageText="<">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" Visible="False" />
                                                            <Columns>
                                                                <asp:BoundField HeaderText="優先級" DataField="Priority">
                                                                    <ItemStyle Width="20%"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Perso廠">
                                                                    <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="hlModify" runat="server"></asp:HyperLink>

                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Value">
                                                                    <ItemStyle HorizontalAlign="Left" Width="10%"></ItemStyle>
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="刪除">
                                                                    <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        &nbsp;<asp:Button ID="btnDelete" runat="server" Text="刪除" class="btn" CausesValidation="False" OnCommand="btnDelete_Command"></asp:Button>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <SelectedRowStyle HorizontalAlign="Center" />
                                                        </cc1:GridViewPageingByDB>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <%--<asp:RadioButton ID="radPercentage" runat="server" Checked="True" GroupName="Percentage_Number"
                                        Text="比率" onclick="doPopDelSpecailDetail();" /><asp:RadioButton ID="radNumber" runat="server" GroupName="Percentage_Number"
                                            Text="數量" onclick="doPopDelSpecailDetail();" />--%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2">&nbsp;<asp:Button ID="Button1" class="btn" runat="server" OnClick="Button1_Click"
                        Text="新增設定明細" />&nbsp;
                        <asp:Button ID="btnAddSpecial" runat="server" CausesValidation="False" OnClick="btnAddSpecial_Click"
                            Text="Button" Style="display: none;" />
                        <asp:Button ID="btnDelSpecial" runat="server" CausesValidation="False" OnClick="btnDelSpecial_Click"
                            Text="Button" Style="display: none;" />
                        <asp:Button ID="btnIsChecked_rad" runat="server" CausesValidation="False" OnClick="btnIsChecked_rad_Click"
                            Text="Button" Style="display: none;" /></td>
                </tr>
                <tr valign="baseline">
                    <td colspan="2" align="right">
                        <asp:HiddenField ID="hidIsModif" runat="server" Value="0" />
                        &nbsp; &nbsp; &nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" OnClientClick="return delConfirm();" />&nbsp;
                        <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" class="btn" OnClick="btnCancel1_Click"
                            Text="取消" />&nbsp;&nbsp;
                    </td>
                </tr>
            </table>

            <script type="text/javascript" language="javascript">
                //function doPopSpecailDetail() {

                //    var b = document.all.rablNumber_Percentage.length
                //    var a = document.getElementById("rablNumber_Percentage").cells.length;
                //    //alert(b);
                //    //alert(a);

                //    for (var i = 0; i < a; i++) {
                //        var ss = "rablNumber_Percentage_" + i;
                //        var aa = document.getElementById(ss).value;
                //        var bb = document.getElementById(ss);

                //        if (document.getElementById(ss).checked) //註意checked不能写成Checked，要不然不成功
                //        {

                //            if (aa == "比率") {
                //                var aa = window.showModalDialog('CardType005ModSpecial_Percentage.aspx?type=add', '', 'dialogHeight:450px;dialogWidth:600px;');
                //                if (aa != undefined) { __doPostBack('btnAddSpecial', ''); }
                //            }
                //            else {
                //                var aa = window.showModalDialog('CardType005ModSpecial_Number.aspx?type=add', '', 'dialogHeight:450px;dialogWidth:600px;');
                //                if (aa != undefined) { __doPostBack('btnAddSpecial', ''); }
                //            }
                //            break;
                //        }

                //    }
                //}

                function doPopDelSpecailDetail() {
                    //if(document.getElementsByName("Percentage_Number")[0].checked)
                    //{
                    if (document.getElementById("hidIsModif").value == "1") {
                        if (true == confirm('系統將刪除當前畫面的所有資料，是否繼續？')) {
                            __doPostBack('btnDelSpecial', '');
                        }
                        else {
                            __doPostBack('btnIsChecked_rad', '');
                        }
                    }
                    //}
                }


                function EnableDel(flag) {
                    if (flag == "1") {
                        document.getElementById("trGrid").style.display = "none";
                    }
                    else {
                        document.getElementById("trGrid").style.display = "";
                    }
                }
            </script>

        </div>
    </form>
</body>
</html>
