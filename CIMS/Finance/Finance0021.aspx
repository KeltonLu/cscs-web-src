<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Finance0021.aspx.cs"
    Inherits="Finance_Finance0021" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>請款及總行會計付款</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function delConfirm()
        {
            
                return confirm('確認刪除此筆訊息?');
            
        }  
        
        function Click(RID)
        {
            __doPostBack('Button2',RID); 
        }
              
        function ClickDel(RID)
        {
            if(document.getElementById("hidIsEdit").value=="2")
            {
                alert("無刪除權限，無法刪除");
                return;
            }
            if(delConfirm())
                __doPostBack('Button1',RID); 
        }
 //-->
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="0"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">請款及總行會計付款</font>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr valign="baseline">
                                        <td align="right" width="15%">
                                            用途：</td>
                                        <td>
                                            <asp:DropDownList ID="dropCard_Purpose" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged">
                                            </asp:DropDownList>群組：&nbsp;<asp:DropDownList ID="dropCard_Group" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right" id="budgetid">
                                            Perso厰：</td>
                                        <td>
                                            <asp:DropDownList ID="dropFactory" runat="server">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr valign="baseline">
                                        <td align="right">
                                            SAP單號：</td>
                                        <td>
                                            <asp:TextBox ID="txtSAP_Serial_Number" runat="server" MaxLength="15" onblur="LimitLengthCheck(this,15);" style="ime-mode:disabled;" onkeyup="LimitLengthCheck(this,15)"></asp:TextBox></td>
                                        <td align="right">
                                            卡片耗用日期：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBegin_Date2" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                                Width="80px"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date2')})" src="../images/calendar.gif" />
                                            ~ &nbsp;<asp:TextBox ID="txtFinish_Date2" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                                Width="80px"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date2')})" src="../images/calendar.gif" />
                                            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="txtBegin_Date2"
                                                ControlToValidate="txtFinish_Date2" ErrorMessage="卡片耗用日期迄必須大於起" Operator="GreaterThanEqual"
                                                Type="Date">*</asp:CompareValidator>
                                        </td>
                                    </tr>
                                    <tr valign="baseline">
                                        <td align="right">
                                            請款日期：</td>
                                        <td>
                                            <asp:TextBox ID="txtBegin_Date1" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                                Width="80px"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date1')})" src="../images/calendar.gif" />
                                            ~ &nbsp;<asp:TextBox ID="txtFinish_Date1" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                                Width="80px"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date1')})" src="../images/calendar.gif" />
                                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date1"
                                                ControlToValidate="txtFinish_Date1" ErrorMessage="請款日期迄必須大於起" Operator="GreaterThanEqual"
                                                Type="Date">*</asp:CompareValidator>
                                        </td>
                                        <td align="right">
                                            出帳日期：</td>
                                        <td>
                                            <asp:TextBox ID="txtBegin_Date3" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                                Width="80px"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date3')})" src="../images/calendar.gif" />
                                            ~ &nbsp;<asp:TextBox ID="txtFinish_Date3" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                                Width="80px"></asp:TextBox>
                                            <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date3')})" src="../images/calendar.gif" />
                                            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToCompare="txtBegin_Date3"
                                                ControlToValidate="txtFinish_Date3" ErrorMessage="出帳日期迄必須大於起" Operator="GreaterThanEqual"
                                                Type="Date">*</asp:CompareValidator>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td align="right">
                        <input id="hidIsEdit" runat="server" type="hidden" />
                        <asp:Button style="display: none;" ID="Button2" runat="server" OnClick="Button2_Click" Text="Button" />
                        <asp:Button style="display: none;" ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
                        &nbsp; &nbsp;&nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                            Text="查詢" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnAdd" runat="server" CssClass="btn" OnClick="btnAdd_Click" Text="新增" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Label ID="lbMsg" Visible="false" runat="server" Text="查無資料" ForeColor="red"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                <tr id="trGrid" style="display: table-row">
                                                    <td>
                                                        <asp:GridView Width="100%" ID="gvSAP" runat="server" CssClass="GridView" OnRowDataBound="gvSAP_RowDataBound">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" Visible="False" />
                                                            <SelectedRowStyle HorizontalAlign="Center" />
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <input id="Button1" class="btn" type="button" value='列印' onclick='Click(<%#  DataBinder.Eval(Container.DataItem,"RID") %>)' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <input id="Button1" class="btn" type="button" value='刪除' onclick='ClickDel(<%#  DataBinder.Eval(Container.DataItem,"RID") %>)' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
