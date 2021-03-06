<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository002Detail.aspx.cs"
    Inherits="Depository_Depository002Detail" %>

<%@ Register Src="../CommUserCtrl/urctrlCardTypeSelect.ascx" TagName="urctrlCardTypeSelect"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>採購下單明細</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 5px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">採購下單明細</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" type="button" value="取消" class="btn" onclick="window.close();" />
                        
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table class="tbl_row" cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr runat="server" id="trDRID">
                                        <td style="width: 20%" align="right">
                                            訂單流水編號：</td>
                                        <td align="left">
                                            <asp:TextBox ID="txtOrderForm_Detail_RID" runat="server" ReadOnly="true"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%" align="right">
                                            <font color="red">*</font>用途：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" OnSelectedIndexChanged="dropCard_Purpose_RID_SelectedIndexChanged"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>群組：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropCard_Group_RID" runat="server" OnSelectedIndexChanged="dropCard_Group_RID_SelectedIndexChanged"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>卡種：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropSpace_Short_RID" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropSpace_Short_RID_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right" style="width: 20%">
                                            <font color="red">*</font>數量：
                                        </td>
                                        <td>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                            <asp:TextBox ID="txtNumber" onfocus="DelDouhao(this)" Style="ime-mode: disabled;
                                                text-align: right" onblur="value=GetValue(this.value);txtNumber.onchange();" onkeyup="CheckNum('txtNumber',9)"
                                                runat="server" MaxLength="11" OnTextChanged="txtNumber_TextChanged" AutoPostBack="True" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumber"
                                                ErrorMessage="數量不能為空！">*</asp:RequiredFieldValidator></td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>預算：
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="dropBudget_RID" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>合約：
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="dropAgreement_RID" runat="server" OnSelectedIndexChanged="drpAgreement_RID_SelectedIndexChanged"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>空白卡廠：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtFactory_shortname_cn" runat="server" size="30" ReadOnly />
                                        </td>
                                    </tr>
                                    <!--add by chaoma start-->
                                    <tr class="tbl_row" valign="baseline">
                                        <td style="width: 20%" align="right" >
                                            <font color="red">*</font>合約單價(含稅)：
                                        </td>
                                        <td>
                                            <%--<asp:TextBox ID="txtBase_price" runat="server" Style="ime-mode: disabled;
                                                text-align: right" size="10" MaxLength="13"
                                                ReadOnly="true" />--%>
                                            <asp:Label runat="server" ID="lblBase_price"  Style="ime-mode: disabled;
                                                text-align: right" size="10" MaxLength="13"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>調整後單價(含稅)：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtChange_Unitprice" runat="server" Style="ime-mode: disabled;
                                                text-align: right" size="10" MaxLength="13" onblur="javascript:if(this.value != document.getElementById('lblBase_price').innerHTML){this.style.color='Red';}else{ this.style.color='black';}"
                                                onkeyup="CheckAmt('txtChange_Unitprice',10,2)" />
                                        </td>
                                    </tr>
                                    <!--add by chaoma end-->
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>預訂交貨日：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtFore_Delivery_Date" onfocus="WdatePicker()" runat="server" Width="80px"
                                                MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtFore_Delivery_Date')})"
                                                    src="../images/calendar.gif" align="absmiddle" />
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>晶片名稱：
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="dropWafer_Name" AutoPostBack="true" DataTextField="text" DataValueField="value"
                                                runat="server" AppendDataBoundItems="True" OnSelectedIndexChanged="dropWafer_Name_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>晶片容量：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtWafer_Capacity" runat="server" Enabled="false"></asp:TextBox>K
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            緊急程度：
                                        </td>
                                        <td>
                                            <asp:RadioButtonList ID="radlIs_Exigence" runat="server" RepeatColumns="2">
                                                <asp:ListItem Value="1">Urgent</asp:ListItem>
                                                <asp:ListItem Selected="True" Value="2">Normal</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>交貨地點：
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="dropFactory_shorname_cn" DataTextField="text" DataValueField="value"
                                                runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="middle">
                                        <td align="right">
                                            備註：
                                        </td>
                                        <td><input id="HidRID" type="hidden" runat="server" />
                                            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="50px" Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                    <td align="right">
                        
                        <asp:Button ID="btnEditD" runat="server" Text="確定" class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancelD" type="button" value="取消" class="btn" onclick="window.close();" />
                        
                    </td>
                </tr>
            </table>
            <script language="javascript" type="text/javascript">
            if(document.getElementById('txtChange_Unitprice').value != document.getElementById('lblBase_price').innerHTML)
            {
                document.getElementById('txtChange_Unitprice').style.color='Red';
            }
            else
            {
                document.getElementById('txtChange_Unitprice').style.color='black';
            }            
            </script>
        </div>
    </form>
</body>
</html>
