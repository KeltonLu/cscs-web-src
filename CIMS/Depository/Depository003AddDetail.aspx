<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository003AddDetail.aspx.cs" Inherits="Depository_Depository003AddDetail" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>入庫單明細</title>
     <META HTTP-EQUIV="pragma" CONTENT="no-cache">
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    
    <base target="_self">
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
       <asp:UpdatePanel id="UpdatePanel2" runat="server">
        <contenttemplate>
        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                        <tr>
                            <td>
                                 <font class="PageTitle">入庫單明細</font>
                            </td>    
                            <td align="right">
                                <asp:Button CssClass="btn" ID="btnSubmitUp" runat="server" Text="確定" OnClick="btnSubmitDn_Click" />&nbsp;&nbsp;
                                <input id="Button2" class="btn" type="button" value="取消" onclick="window.close();" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                        <tr>
                            <td style="width:15%" align="right"><font color="red">*</font>用途：</td>
                            <td align="left">
                                <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" OnSelectedIndexChanged="dropCard_Purpose_RID_SelectedIndexChanged" AutoPostBack="True">
                               </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>群組：</td>
                            <td align="left">
                               <asp:DropDownList ID="dropCard_Group_RID" runat="server" OnSelectedIndexChanged="dropCard_Group_RID_SelectedIndexChanged" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>卡種：</td>
                            <td align="left">
                                <asp:DropDownList ID="dropSpace_Short_RID" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropSpace_Short_RID_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>進貨量：</td>
                            <td align="left">
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="caltxtIncome_Number('txtStock_Number',9)" MaxLength="11" ID="txtStock_Number" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtStock_Number"
                                    ErrorMessage="進貨量不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>瑕疵量：</td>
                            <td align="left">
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="caltxtIncome_Number('txtBlemish_Number',9)" MaxLength="11" ID="txtBlemish_Number" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtBlemish_Number"
                                    ErrorMessage="瑕疵量不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>抽樣卡數：</td>
                            <td align="left">
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="caltxtIncome_Number('txtSample_Number',9)" MaxLength="11" ID="txtSample_Number" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtSample_Number"
                                    ErrorMessage="抽樣卡數不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>入庫量：</td>
                            <td align="left">
                                <asp:TextBox ID="txtIncome_Number"  style="ime-mode:disabled;text-align: right" runat="server" ReadOnly="true"></asp:TextBox></tr>
                        <tr>
                            <td align="right"><font color="red">*</font>入庫日期：</td>
                            <td align="left">
                                <asp:TextBox ID="txtIncome_Date" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtIncome_Date')})" src="../images/calendar.gif" align="absmiddle">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtIncome_Date"
                                    ErrorMessage="入庫日期不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right">卡片批號：</td>
                            <td align="left">
                                <asp:TextBox ID="txtSerial_Number" runat="server" onkeyup="LimitLengthCheck(this,25)" onblur="LimitLengthCheck(this,25);CheckSerialNum()" MaxLength="25"></asp:TextBox>&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>Perso廠：</td>
                            <td align="left">
                                <asp:DropDownList DataValueField="RID" DataTextField="Factory_ShortName_CN" ID="dropPerso_Factory_RID" runat="server">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>空白卡廠：</td>
                            <td align="left">
                                <asp:DropDownList DataValueField="RID" DataTextField="Factory_ShortName_CN" ID="dropBlank_Factory_RID" runat="server">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>晶片名稱：</td>
                            <td align="left">
                                <asp:DropDownList DataValueField="RID" DataTextField="WAFER_NAME" ID="dropWafer_RID" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"><font color="red">*</font>驗貨方式：</td>
                            <td align="left">
                                <asp:RadioButtonList ID="rblCheck_Type" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True" Value="1">抽驗</asp:ListItem>
                                    <asp:ListItem Value="2">全驗</asp:ListItem>
                                </asp:RadioButtonList></td>
                        </tr>
                        <tr>
                            <td align="right">備註：</td>
                            <td align="left">
                                <asp:TextBox ID="txtComment" runat="server" Height="50px" Width="300px" TextMode="MultiLine"></asp:TextBox>
                                </td>
                        </tr>
                        <tr>
                            <td align="right">批號狀態：</td>
                            <td align="left">
                                 <asp:TextBox ID="txtSendCheck_Status" runat="server" ReadOnly="true"></asp:TextBox></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button ID="btnCalNum" style="display: none;"  runat="server" Text="Button" CausesValidation="false" OnClick="btnCalNum_Click" />
                    <asp:Button CssClass="btn" ID="btnSubmitDn" runat="server" Text="確定" OnClick="btnSubmitDn_Click" />&nbsp;&nbsp;
                    <input id="Button1" class="btn" type="button" value="取消" onclick="window.close();" /></td>
            </tr>
        </table>
        </contenttemplate>
        </asp:UpdatePanel>
    </div>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
     <script>
            function CheckSerialNum()
            {
                __doPostBack('btnCalNum',''); 
            }
            
            function caltxtIncome_Number(txtid,len)
            {
                CheckNum(txtid,len);
                calIncome_Number();
            }
            
            function calIncome_Number()
            {
                var num1=0;
                var num2=0;
                var num3=0;
                var num4=0;
                
                if(document.getElementById("txtStock_Number").value!="")
                {
                    num1 = Number(document.getElementById("txtStock_Number").value.replace(/,/g,''));
                }
                
                if(document.getElementById("txtBlemish_Number").value!="")
                {
                    num2 = Number(document.getElementById("txtBlemish_Number").value.replace(/,/g,''));
                }
                
                if(document.getElementById("txtSample_Number").value!="")
                {
                    num3 = Number(document.getElementById("txtSample_Number").value.replace(/,/g,''));
                }
                
                num4=GetValue(num1-num2-num3);
                
                document.getElementById("txtIncome_Number").value=num4;
            }
        </script>
</body>
</html>
