<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="CardType0041Add.aspx.cs" Inherits="CardType_CardType0041Add" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>製程新增 </title>
       <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<script language="javascript" type="text/javascript">	
<!--
    function ExistFactory_RID_isok()
    {
        if (true==confirm('資料庫中已有此製程及對應Perso廠資料，是否新增？'))
                       {
                            __doPostBack('btnIsOk',''); 
                       }
//                    else
//                       {
//                            __doPostBack('btnIsCheck',''); 
//                       }
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
                        <font class="PageTitle">製程新增 </font>
                    </td>
                    <td align="right">
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit1" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            Text="確定" />&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnPage('CardType004.aspx')" type="button"
                            value="取消" />&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%; height: 22px;">
                                    <span class="style1">*</span>Perso廠：
                                </td>
                                <td style="width: 15%; height: 22px;">
                                    <asp:DropDownList ID="dropFactory" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right" style="width: 15%">
                                    <span class="style1">*</span>製程：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" onkeyup="LimitLengthCheck(this, 30)"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                                        ErrorMessage="製程不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td align="right" style="width: 15%">
                                    <span class="style1">*</span>單價：
                                </td>
                                <td>
                                                        <%--Dana 20161021 最大長度由9改為10 --%>
                                    <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="CheckAmt('txtPrice',4,4);value=GetValue(this.value)" onkeyup="CheckAmt('txtPrice',4,4)" ID="txtPrice" runat="server" MaxLength="10"></asp:TextBox>
                                </td>
                            </tr>
                            <tr valign="baseline">
                                <td>
                                    <div align="right">
                                        <span style="font-size: 10pt; color: #ff0000">*</span>使用期間：
                                    </div>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUse_Date_Begin" runat="server" MaxLength="10" onfocus="WdatePicker()" Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_Begin')})" src="../images/calendar.gif" />~
                                    <asp:TextBox ID="txtUse_Date_End" runat="server" MaxLength="10" onfocus="WdatePicker()" Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtUse_Date_End')})" src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtUse_Date_Begin"
                                        ErrorMessage="使用期間（起）不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtUse_Date_End"
                                        ErrorMessage="使用期間（迄）不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtUse_Date_Begin"
                                        ControlToValidate="txtUse_Date_End" ErrorMessage="使用期間（迄）必須大於（起）" Operator="GreaterThanEqual">*</asp:CompareValidator></td>
                            </tr>
                            <tr class="tbl_row" >
                                <td width="10%">
                                    <div align="right">
                                        備註：
                                    </div>
                                </td>
                                <td width="60%">
                                    <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="50px" Width="400px"></asp:TextBox></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        <asp:Button ID="btnIsOk" runat="server" OnClick="btnIsOk_Click" Text="Button" style="display: none;" />
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSubmit2" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
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