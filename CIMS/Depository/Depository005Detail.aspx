<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository005Detail.aspx.cs" Inherits="Depository_Depository005Detail" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>再入庫作業修改</title>
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
                     <font class="PageTitle">再入庫作業修改 </font>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button CssClass="btn" ID="btnSubmitUp" runat="server" Text="確定" OnClick="btnSubmitDn_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <input id="Button2" class="btn" type="button" value="取消" onclick="window.close();" /></td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                        <tr>
                            <td style="width:20%" align="right">
                                入庫流水編號：</td>
                            <td align="left">
                                <asp:Label ID="lblOrderForm_Detail_RID" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">版面簡稱：</td>
                            <td align="left">
                                <asp:Label ID="lblSpace_Short_Name" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">
                                原退貨量：</td>
                            <td align="left">
                                <asp:Label ID="lblTotalNum" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">
                                再進貨量：</td>
                            <td align="left">
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="caltxtIncome_Number('txtRestock_Number',9)" MaxLength="11" ID="txtRestock_Number" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtRestock_Number"
                                    ErrorMessage="再進貨量不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right">瑕疵量：</td>
                            <td align="left">
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="caltxtIncome_Number('txtBlemish_Number',9)" MaxLength="11" ID="txtBlemish_Number" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBlemish_Number"
                                    ErrorMessage="瑕疵量不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right">抽樣卡數：</td>
                            <td align="left">
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                <asp:TextBox onfocus="DelDouhao(this)" style="ime-mode:disabled;text-align: right" onblur="value=GetValue(this.value)" onkeyup="caltxtIncome_Number('txtSample_Number',9)" MaxLength="11" ID="txtSample_Number" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSample_Number"
                                    ErrorMessage="抽樣卡數不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right">
                                再入庫量：</td>
                            <td align="left">
                                <asp:TextBox ID="lblIncome_Number"  style="ime-mode:disabled;text-align: right" runat="server" ReadOnly="true"></asp:TextBox>
                        </tr>
                        <tr>
                            <td align="right">
                                再入庫日期：</td>
                            <td align="left">
                                <asp:Label ID="lblReincome_Date" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">卡片批號：</td>
                            <td align="left">
                                <asp:TextBox ID="txtSerial_Number" runat="server" MaxLength="25"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtSerial_Number"
                                    ErrorMessage="卡片批號不能為空">*</asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td align="right" style="height: 43px">Perso廠商：</td>
                            <td align="left" style="height: 43px">
                                <asp:Label ID="lblPerso_Factory_Name" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">空白卡廠：</td>
                            <td align="left">
                                <asp:Label ID="lblBlank_Factory_Name" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">晶片名稱：</td>
                            <td align="left">
                                <asp:Label ID="lblWafer_Name" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">驗貨方式：</td>
                            <td align="left">
                                <asp:RadioButtonList ID="rblCheck_Type" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True" Value="1">抽驗</asp:ListItem>
                                    <asp:ListItem Value="2">全驗</asp:ListItem>
                                </asp:RadioButtonList></td>
                        </tr>
                        <tr>
                            <td align="right">備註：</td>
                            <td align="left">
                                <asp:TextBox ID="txtComment" runat="server" Height="50px" Width="300px" TextMode="MultiLine"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td align="right">
                                批號狀態(送審狀態)：
                            </td>
                            <td align="left">
                                &nbsp;<asp:Label ID="lblSendCheck_Status" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button CssClass="btn" ID="btnSubmitDn" runat="server" Text="確定" OnClick="btnSubmitDn_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <input id="Button1" class="btn" type="button" value="取消" onclick="window.close();" /></td>
            </tr>
        </table>
        </contenttemplate>
        </asp:UpdatePanel>
        <script>
        
           
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
                
                if(document.getElementById("txtRestock_Number").value!="")
                {
                    num1 = Number(document.getElementById("txtRestock_Number").value.replace(/,/g,''));
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
                
                document.getElementById("lblIncome_Number").value=num4;
            }
        </script>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>

