<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository011Add.aspx.cs"
    Inherits="Depository_Depository011Add" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>卡片庫存移轉新增</title>
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
    
     /**
     * 驗證輸入數字
     *
     * @param TextName
     * @param NumLen
     * return 正確數字
     */
    function AtGridCheckNum(obj,NumLen)
    {
        var CardNum = obj.value;
                    
        var patten=new RegExp(/^\d*$/); 
        var IsCal=false;
        
        if(!patten.test(CardNum))
        {
            IsCal=true;
        }
            
        var CardNum = CardNum.replace(/[^0-9]/g,'');
        
        if(CardNum.length>NumLen)
        {
            IsCal=true;
            CardNum=CardNum.substring(0,NumLen);
        }
        
        if(IsCal)
        {
            obj.value=CardNum;
        }
        
        return CardNum;
    }

//-->
</script>

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
                    <td class="PageTitle" colspan="2">
                        卡片庫存移轉新增</td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="0" border="0" id="TABLE1">
                            <tr class="tbl_row" valign="baseline">
                                <td width="15%" align="right">
                                    <span style="color: #ff0000">*</span>日期：</td>
                                <td>
                                    <asp:TextBox ID="txtMove_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtMove_Date')})" src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMove_Date"
                                        ErrorMessage="日期必須輸入">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td colspan="2">
                                    <div id="123">
                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                            <tr valign="baseline">
                                                <td>
                                                    <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr valign="baseline">
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbCardTypeStocksMove" runat="server" AllowPaging="false"
                                                OnOnSetDataSource="gvpbCardTypeStocksMove_OnSetDataSource" AllowSorting="false"
                                                AutoGenerateColumns="False" CssClass="GridView" OnRowDataBound="gvpbCardTypeStocksMove_RowDataBound"
                                                SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:BoundField DataField="Name" HeaderText="卡種">
                                                        <itemstyle horizontalalign="Left" width="22%" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="庫存數量">
                                                        <itemstyle width="27%" />
                                                        <itemtemplate>
                                                    <asp:Label runat="server" ID = "lbDepository_Stocks_Num"></asp:Label>
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="數量">
                                                        <itemstyle width="7%" />
                                                        <itemtemplate>
                                                        <%--Dana 20161021 最大長度由 9 改為 11 --%>
                                                    <asp:TextBox runat="server" ID = "txtMove_Number" onfocus="DelDouhao(this)" onblur="AtGridCheckNum(this,9);value=GetValue(this.value)"
                                        onkeyup="AtGridCheckNum(this,9)" MaxLength="11" style="ime-mode:disabled;text-align: right"></asp:TextBox>                                               
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="轉出Perso廠">
                                                        <itemstyle width="22%" />
                                                        <itemtemplate>
                                                        <asp:DropDownList runat="server" ID = "drpFrom_Factory"></asp:DropDownList>                                                    
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="轉入Perso廠">
                                                        <itemstyle width="22%" />
                                                        <itemtemplate>
                                                        <asp:DropDownList runat="server" ID = "drpTo_Factory" DataTextField="Factory_ShortName_CN" DataValueField="RID"></asp:DropDownList>                                                    
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="">
                                                        <itemstyle width="12%" />
                                                        <itemtemplate>
                                                        <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDelete_Command"></asp:ImageButton>
                                                       
                                                </itemtemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="left">
                        <asp:Button CssClass="btn" ID="btnDetailAdd" runat="server" Text="新增明細" OnClick="btnDetailAdd_Click" />
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        <asp:Button CssClass="btn" ID="btnSubmit" runat="server" Text="確定" OnClick="btnSubmit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('Depository011.aspx')" type="button"
                            value="取消" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
