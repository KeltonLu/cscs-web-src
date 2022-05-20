<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType0041Mod.aspx.cs"
    Inherits="CardType_CardType0041Mod" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>製程修改/刪除 </title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function doRadio(rad)
        {
            if (rad == document.getElementById("adrtNormal"))
            {
                document.getElementById("trCardType").style.display="table-row";
            }
            else
            {
               document.getElementById("trCardType").style.display="none";
            }
        }        
        
        function doLoad()
        {
            if (true == document.getElementById("adrtNormal").checked)
            {
                document.getElementById("trCardType").style.display="table-row";
            }
            else
            {
                document.getElementById("trCardType").style.display="none";
            }
        }
        
        function delConfirm()
        {
            if(document.getElementById('chkDel').checked)
            {
                return confirm("是否刪除此筆訊息");
            }
          else
            {
                return CheckClientValidate();
            }
        } 
        
        function returnPage(Url)
        {
            window.location=Url+"?Con=1&List=radStep";
        }        
 //-->
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">製程修改/刪除 </font>
                    </td>
                    <td align="right">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit1" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" Text="確定" />&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('CardType004.aspx')" type="button"
                            value="取消" />&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%; height: 22px;">
                                    Perso廠：
                                </td>
                                <td>
                                    <asp:Label ID="lbFactory" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    製程：
                                </td>
                                <td>
                                    <asp:Label ID="lbName" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" style="width: 15%">
                                    <span class="style1">*</span>單價：
                                </td>
                                <td>
                                                        <%--Dana 20161021 最大長度由9改為10 --%>
                                    <asp:TextBox ID="txtPrice" runat="server" MaxLength="10" onblur=" CheckAmt('txtPrice',4,4);value=GetValue(this.value)"
                                        onfocus="DelDouhao(this)" onkeyup="CheckAmt('txtPrice',4,4)" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td width="10%">
                                    <div align="right">
                                        <span style="font-size: 10pt; color: #ff0000">*</span>價格期間：
                                    </div>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUse_Date_Begin" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_Begin')})" src="../images/calendar.gif" />~
                                    <asp:TextBox ID="txtUse_Date_End" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_End')})" src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtUse_Date_Begin"
                                        ErrorMessage="使用期間（起）不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtUse_Date_End"
                                        ErrorMessage="使用期間（迄）不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" valign="top">
                                    被指定代製項目：</td>
                                <td>
                                    <asp:GridView ID="gvPerso_Project" runat="server" AutoGenerateColumns="False" Width="200px" CssClass="GridView">
                                        <Columns>
                                            <asp:BoundField DataField="Project_Name" HeaderText="代製項目" />
                                        </Columns>
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                    </asp:GridView>
                                   
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right" valign="top">
                                    備註：</td>
                                <td>
                                    <label>
                                        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="50px" Width="400px"></asp:TextBox></label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit2" runat="server"
                            CssClass="btn" OnClick="btnSubmit_Click" OnClientClick="return delConfirm();"
                            Text="確定" />&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnPage('CardType004.aspx')" type="button"
                            value="取消" />&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
