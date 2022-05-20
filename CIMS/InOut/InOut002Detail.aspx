<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InOut002Detail.aspx.cs" Inherits="InOut_InOut002Detail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>廠商庫存異動匯入</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <base target="_self" />
    <script language="javascript" type="text/javascript">
        function CanNegativeNumCheck()
        {
            for (var i=0; i<document.getElementById("dropStatus").options.length; i++)
            {
                if (document.getElementById("dropStatus").options[i].selected)
                {
                    if (document.getElementById("dropStatus").options[i].text == '移轉' ||
                        document.getElementById("dropStatus").options[i].text == '調整')
                    {
                        return CheckNumWithNoId1(document.getElementById("txtNumber"),9);
                    }
                }
            }
            
            return CheckNum("txtNumber",9);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 5px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">異動明細</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit1" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <input id="Button1" class="btn" onclick="window.close();" type="button" value="取消" />&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr>
                                <td>
                                    <table class="tbl_row" cellpadding="0" width="100%" cellspacing="2" border="0">
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" width="100px">
                                                日期：
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBegin_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                                    Width="100px"></asp:TextBox>
                                                <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBegin_Date"
                                                    ErrorMessage="日期不能為空" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                                <asp:HiddenField ID="hidRowIndex" runat="server" />
                                                </td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" width="100px">
                                                Perso廠商：
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="dropFactory" runat="server" Width="88px">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" width="100px">
                                                <span class="style1">*</span> 用途：
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="dropCard_Purpose_RID_SelectedIndexChanged">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right">
                                                <span class="style1">*</span> 群組：
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="dropCard_Group_RID" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="dropCard_Group_RID_SelectedIndexChanged">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" style="height: 22px">
                                                <span class="style1">*</span> 卡種：
                                            </td>
                                            <td style="height: 22px">
                                                <asp:DropDownList ID="dropSpace_Short_RID" runat="server">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right">
                                                <span class="style1">*</span> 狀況：
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="dropStatus" runat="server" DataTextField="Status_Name"
                                                    DataValueField="RID">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td align="right" style="height: 24px">
                                                <span class="style1">*</span> 數量：
                                            </td>
                                            <td style="height: 24px">
                                                <asp:TextBox ID="txtNumber" onfocus="DelDouhao(this)" 
                                                Style="ime-mode: disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="CanNegativeNumCheck()"
                                                runat="server" MaxLength="11" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit2" runat="server" class="btn" OnClick="btnSubmit1_Click"
                            Text="確定" />&nbsp;&nbsp;
                        <input id="Button2" class="btn" onclick="window.close();" type="button" value="取消" />&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
