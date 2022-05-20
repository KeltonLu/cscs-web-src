<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="CardType004Add.aspx.cs"
    Inherits="CardType_CardType004Add" %>
<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>代製項目新增</title>
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
                <tr valign="baseline" style="height: 20">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td class="PageTitle">
                        代製項目新增
                    </td>
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnPage('CardType004.aspx')" type="button"
                            value="取消" /></td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="0" border="0" id="TABLE1">
                            <tr class="tbl_row" valign="baseline">
                                <td width="15%" align="right" style="height: 24px">
                                    <span style="color: #ff0000">*<span style="color: #000000">代製</span></span>項目：</td>
                                <td style="width: 15%; height: 24px;">
                                    <asp:TextBox ID="txtProject_Name" runat="server" MaxLength='30' onkeyup="LimitLengthCheck(this, 30)"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtProject_Name"
                                        ErrorMessage="Perso項目不能為空">*</asp:RequiredFieldValidator></td>
                                <td align="center" style="width: 15%; height: 24px;">
                                    <input id="adrtNormal" runat="server" name="radioNormal_Special" onclick="doRadio(this);"
                                        type="radio" value="1" checked="true" />一般&nbsp; &nbsp;<input id="adrtSpecial" runat="server"
                                            value="2" name="radioNormal_Special" onclick="doRadio(this);" type="radio" />特別</td>
                                <td style="height: 24px">
                                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span style="color: #ff0000">*<span style="color: #000000; background-color: #e4e4e4">Perso廠：</span></span></td>
                                <td colspan="3">
                                    <asp:DropDownList ID="dropFactory_RID" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropFactory_RID_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="dropFactory_RID"
                                        ErrorMessage="Perso廠不能為空">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" id='trCardType' style="display: table-row" valign="baseline">
                                <td colspan="4">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="right" style="width: 15%">
                                                        單價變化表：
                                                    </td>
                                                    <td colspan="3">
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
                                                    <td style="width: 15%">
                                                    </td>
                                                    <td colspan="3">
                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td align="center" style="height: 15px">
                                                                    <asp:Label ID="lblLeftName" runat="server" Text="可選製程 "></asp:Label></td>
                                                                <td style="height: 15px">
                                                                </td>
                                                                <td align="center" style="height: 15px">
                                                                    <asp:Label ID="lblRightName" runat="server" Text="已選擇製程"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:ListBox ID="LbLeft" runat="server" Height="120px" SelectionMode="multiple" Width="150px">
                                                                    </asp:ListBox>
                                                                </td>
                                                                <td align="center" style="width: 50px">
                                                                    <asp:Button ID="btnSelect" runat="server" CausesValidation="false" CssClass="btn"
                                                                        OnClick="btnSelect_Click" Text=">" Width="30px" /><br />
                                                                    <asp:Button ID="btnRemove" runat="server" CausesValidation="false" CssClass="btn"
                                                                        OnClick="btnRemove_Click" Text="<" Width="30px" /><br />
                                                                    <asp:Button ID="btnSelectAll" runat="server" CausesValidation="false" CssClass="btn"
                                                                        OnClick="btnSelectAll_Click" Text=">>" Width="30px" /><br />
                                                                    <asp:Button ID="btnRemoveAll" runat="server" CausesValidation="false" CssClass="btn"
                                                                        OnClick="btnRemoveAll_Click" Text="<<" Width="30px" />
                                                                </td>
                                                                <td>
                                                                    <asp:ListBox ID="LbRight" runat="server" Height="120px" SelectionMode="multiple"
                                                                        Width="150px"></asp:ListBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr class="tbl_row" id='trCardType1' style="display: table-row" valign="baseline">
                                <td align="right">
                                    單價：
                                </td>
                                <td colspan="3" align="left">
                                                        <%--Dana 20161021 最大長度由9改為10 --%>
                                    <asp:TextBox ID="txtUnit_Price" onfocus="DelDouhao(this)" onkeyup="CheckAmt('txtUnit_Price',4,4)" style="ime-mode:disabled;text-align: right"  onblur=" CheckAmt('txtUnit_Price',4,4);value=GetValue(this.value)" runat="server" MaxLength="10"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    <span style="color: #ff0000"><span style="color: #000000">備註：</span><span style="color: #000000;
                                        background-color: #e4e4e4"></span></span></td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="50px" Width="400px"></asp:TextBox></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2" id="txtFactory_RID">
                        <asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnPage('CardType004.aspx')" type="button"
                            value="取消" /></td>
                </tr>
            </table>
            <script language="javascript" type="text/javascript">	
<!--
    function doRadio(rad)
    {
        // Legend 2016/10/28 調整display: 由 table-row 改為 table-row
        if (rad == document.getElementById("adrtNormal"))
        {
            document.getElementById("trCardType").style.display="table-row";
            document.getElementById("trCardType1").style.display="none";
        }
        else
        {
           document.getElementById("trCardType").style.display="none";
           document.getElementById("trCardType1").style.display="table-row";
        }
    } 
    
   function returnPage(Url)
    {
       window.location=Url+"?Con=1&List=radProject_Name";
    }        
//-->
</script>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
