<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository015Mod.aspx.cs" Inherits="Depository_Depository015Mod" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>物料採購作業修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>    
    
    <script language="javascript" type="text/javascript">	      
        
        function delConfirm()
        {
            // add by tree CommJS.js中沒有CheckClientValidate的方法導致儲存時未選擇刪除時出錯
            if (true == document.getElementById("chkDelete").checked)
            {
                return confirm('確認刪除此物料採購單訊息?');
            }
            //else
            //{
            //    return CheckClientValidate();
            //}
        }        
        
        function ImtBind()
        {                
            __doPostBack('btnBind',''); 
        }
   
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ScriptManager id="ScriptManager1" runat="server">
            </asp:ScriptManager>
    <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
            border="0">
            <tr style="height: 20px">
                <td colspan="2">&nbsp;
                    
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">物料採購作業修改/刪除</font>
                </td>
                <td align="right">
                    <asp:Button id="btnSubmit" onclick="btnSubmit_Click" runat="server" CssClass="btn" Text="確定"></asp:Button>&nbsp;&nbsp;
                    <asp:Button id="btnCancel" onclick="btnCancel_Click" runat="server" CssClass="btn" Text="取消"></asp:Button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr class="tbl_row">
                            <td width="10%" align="right" style="height: 15px" >
                                採購單號：
                            </td>
                            <td style="height: 15px" >
                                &nbsp;<asp:Label ID="lblPurchaseOrder_RID" runat="server" Style="position: relative"
                                    Text="Label" Width="74px"></asp:Label></td>
                            <td width="10%" align="right" style="height: 15px" >
                                採購日期：
                            </td>
                            <td style="height: 15px" >
                                <asp:Label ID="lblPurchaseDate" runat="server" Style="position: relative" Text="Label" Width="77px"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand" OnItemCreated="Repeater1_ItemCreated" OnItemDataBound="Repeater1_ItemDataBound">
                    <ItemTemplate>
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr class="tbl_row">
                            <td  align="right" style="width: 120px; height: 15px">
                                品名：                            </td>
                            <td style="height: 15px" >                                
                                <asp:Label ID="lblName" text='<% # Eval("Name")%>' runat="server" Style="position: relative"  Width="67px"></asp:Label></td>
                            <td  align="right" style="height: 15px">
                                單價：                            </td>
                            <td style="height: 15px" >
                                <asp:Label ID="lblUnitPrice" text='<% # Eval("Unit_Price")%>' runat="server" Style="position: relative" 
                                    Width="50px"></asp:Label></td>
                            <td  align="right" style="height: 15px">
                                採購數量：                            </td>
                            <td style="height: 15px" >
                                <asp:Label ID="lblPurchaseNumber" text='<% # Eval("Total_Num")%>'  runat="server" Style="position: relative" 
                                    Width="47px"></asp:Label>
                            </td>
                            <td  align="right" style="height: 15px">
                                採購金額：                            </td>
                            <td style="height: 15px" >
                                <asp:Label ID="lblPrice" text='<% # Eval("Total_Price")%>' runat="server" Style="position: relative"  Width="49px"></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="8">
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr class="tbl_row">
                                        <td width="10%" align="right">
                                            送貨Perso廠：                                        </td>
                                        <td width="10%" align="center">
                                            <asp:Label ID="lblFactory1" text='<% # Eval("Factory1")%>' runat="server" Style="position: relative" 
                                                Width="113px"></asp:Label></td>
                                        <td width="15%" align="center">
                                            <asp:Label ID="lblFactory2" text='<% # Eval("Factory2")%>' runat="server" Style="position: relative"
                                                Width="113px"></asp:Label>
                                        </td>
                                        <td width="15%" style="text-align: center">&nbsp;                                      
                                            <asp:Label ID="lblFactory3" text='<% # Eval("Factory3")%>' runat="server" Style="position: relative" 
                                                Width="113px"></asp:Label></td>
                                        <td width="15%" style="text-align: center">&nbsp;                                      
                                           <asp:Label ID="lblFactory4" text='<% # Eval("Factory4")%>' runat="server" Style="position: relative" 
                                                Width="113px"></asp:Label></td>
                                        <td width="15%" style="text-align: center">&nbsp;                                      
                                            <asp:Label ID="lblFactory5" text='<% # Eval("Factory5")%>' runat="server" Style="position: relative" 
                                                Width="113px"></asp:Label></td>
                                    </tr>
                                    <tr class="tbl_row">
                                        <td style="height: 14px" align="right">
                                            送貨數量：                                        </td>
                                        <td style="height: 14px" align="center">
                                            <asp:Label ID="lblNumber1" text='<% # Eval("Number1")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label>
                                        </td>
                                        <td style="height: 14px" align="center">
                                            <asp:Label ID="lblNumber2" text='<% # Eval("Number2")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label>
                                        </td>
                                        <td style="height: 14px; text-align: center;" align="right">
                                            <asp:Label ID="lblNumber3" text='<% # Eval("Number3")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label>&nbsp;                                      </td>
                                        <td style="height: 14px; text-align: center;" align="right">
                                            <asp:Label ID="lblNumber4" text='<% # Eval("Number4")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label>&nbsp;                                      </td>
                                        <td style="height: 14px; text-align: center;" align="right">
                                            <asp:Label ID="lblNumber5" text='<% # Eval("Number5")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label>&nbsp;                                      </td>
                                    </tr>
                                    <tr class="tbl_row">
                                        <td align="right">
                                            交貨日：                                        </td>
                                        <td style="text-align: center">
                                            <asp:Label ID="lblDeliveryDate1" text='<% # Eval("Delivery_Date1")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label></td>
                                        <td style="text-align: center">
                                            <asp:Label ID="lblDeliveryDate2" text='<% # Eval("Delivery_Date2")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label></td>
                                        <td style="text-align: center">&nbsp;                                      
                                            <asp:Label ID="lblDeliveryDate3" text='<% # Eval("Delivery_Date3")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label></td>
                                        <td style="text-align: center">&nbsp;                                      
                                            <asp:Label ID="lblDeliveryDate4" text='<% # Eval("Delivery_Date4")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label></td>
                                        <td style="text-align: center">&nbsp;                                      
                                            <asp:Label ID="lblDeliveryDate5" text='<% # Eval("Delivery_Date5")%>' runat="server" Style="position: relative" 
                                                Width="100px"></asp:Label></td>
                                    </tr>
                              </table>                            </td>
                        </tr>
                        <tr class="tbl_row">
                            <td align="right" style="width: 120px">
                                結案日期：                            </td>
                            <td>
                                <asp:Label ID="lblCaseDate" text='<% # Eval("Case_Date")%>' runat="server" Style="position: relative" 
                                    Width="126px"></asp:Label></td>
                            <td colspan="6">&nbsp;                            </td>
                        </tr>
                        <tr class="tbl_row">
                          <td align="right" style="width: 120px">SAP單號：</td>
                          <td>
                              <asp:Label ID="lblSAP" text='<% # Eval("SAP_Serial_Number")%>' runat="server" Style="position: relative"  Width="126px"></asp:Label></td>
                          <td colspan="6">&nbsp;</td>
                        </tr>
                        <tr class="tbl_row">
                          <td align="right" style="width: 120px; height: 15px;">請款日期： </td>
                          <td style="height: 15px">
                              <asp:Label ID="lblAskDate" text='<% # Eval("Ask_Date")%>'  runat="server" Style="position: relative" 
                                  Width="126px"></asp:Label></td>
                          <td colspan="6" style="height: 15px">&nbsp;</td>
                        </tr>
                        <tr class="tbl_row">
                          <td align="right" style="width: 120px">出帳日期：</td>
                          <td>
                              <asp:Label ID="lblPayDate" text='<% # Eval("Pay_Date")%>' runat="server" Style="position: relative"  Width="126px"></asp:Label></td>
                          <td colspan="6">&nbsp;</td>
                        </tr>
                        <tr class="tbl_row">
                            <td align="right" style="width: 120px">
                                備註：                            </td>
                            <td>
                                <asp:Label ID="lblComment" runat="server" text='<% # Eval("Comment")%>' Style="position: relative" 
                                    Width="125px"></asp:Label></td>
                          <td colspan="6" align="right">
                              &nbsp;<asp:Button ID="btnUpdateDetail" runat="server" class="btn" Style="left: 1px;
                                  position: relative" Text="修改" OnCommand="btnUpdateDetail_Command"/>&nbsp;&nbsp;
                              <asp:Button ID="btnDeleteDetail" runat="server" class="btn" Style="position: relative"
                                  Text="刪除" OnCommand="btnDeleteDetail_Command"/>
                          </td>
                        </tr>
                    </table>
                    </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>            
            <tr class="tbl_row" >
            <td colspan="2">
                <asp:CheckBox ID="chkDelete" runat="server" Font-Size="Small" Style="position: relative"
                    Text="刪除" />&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <input id="hidPurchaseOrder_RID" runat="server" style="position: relative" type="hidden" />
                    <asp:Button ID="btnBind" runat="server" CausesValidation="False" OnClick="btnBind_Click"
                        Style="position: relative" Visible="False" />
                     <asp:Button id="btnSubmit1" onclick="btnSubmit_Click" runat="server" CssClass="btn" OnClientClick="return delConfirm();" Text="確定"></asp:Button>&nbsp;&nbsp;
                    <asp:Button id="btnCancel1" onclick="btnCancel_Click" runat="server" CssClass="btn" Text="取消"></asp:Button>
                </td>
            </tr>
        </table>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" Style="position: relative" Width="149px" />
    </form>
</body>
</html>
