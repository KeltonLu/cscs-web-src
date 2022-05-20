<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0011Add.aspx.cs" Inherits="Finance_Finance0011Add" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>請款放行作業新增</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="0"
                border="0">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">請款放行作業新增</font>
                    </td>
                </tr>
                <tr>
                    <td class="tbl_row">
                        <input type="radio" runat="server" name="function" id="fuctionR1" value="1" onclick="show('buttonRow1')"
                            checked="true" />拆單作業
                        <input type="radio" runat="server" name="function" id="functionR2" value="2" onclick="show('buttonRow2')" />請款作業
                    </td>
                </tr>
                <tr width="100%">
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0" id="div">
                            <tr valign="baseline">
                                <td align="right" width="15%">
                                    用途：</td>
                                <td width="50%">
                                    <asp:DropDownList ID="dropCard_Purpose" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropCard_Purpose_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    &nbsp; &nbsp;&nbsp;&nbsp;&nbsp; 群組名稱：&nbsp;<asp:DropDownList ID="dropCard_Group"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td align="right" width="9%" id="budgetid">
                                    版面簡稱：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" Width="90px"></asp:TextBox></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    空白卡厰：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropBlankFactory" runat="server">
                                    </asp:DropDownList></td>
                                <td align="right" id="td1">
                                    請款狀態：
                                </td>
                                <td id="td2">
                                    <asp:DropDownList ID="dropState" runat="server">
                                        <asp:ListItem Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="1">未請款</asp:ListItem>
                                        <asp:ListItem Value="2">已請款</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    進貨作業期間：</td>
                                <td>
                                    <asp:TextBox ID="txtBegin_Date" runat="server" onfocus="WdatePicker()" MaxLength="10"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    ~ &nbsp;<asp:TextBox ID="txtFinish_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="80px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtFinish_Date')})" src="../images/calendar.gif" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Date"
                                        ControlToValidate="txtFinish_Date" ErrorMessage="進貨作業期間日期迄必須大於起" Operator="GreaterThanEqual"
                                        Type="Date">*</asp:CompareValidator>
                                </td>
                                <td align="right">
                                    預算簽呈文號：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBUDGET_ID" runat="server" MaxLength="30"></asp:TextBox></td>
                            </tr>
                            <tr valign="baseline">
                                <td align="right">
                                    合約編號：</td>
                                <td>
                                    <asp:TextBox ID="txtAgreement_Code" runat="server" MaxLength="20" onblur="LimitLengthCheck(this,20)"
                                        onkeyup="LimitLengthCheck(this,20)"></asp:TextBox></td>
                                <td align="right">
                                    訂單流水編號：</td>
                                <td>
                                    <asp:TextBox ID="txtStock_RIDYear" runat="server" MaxLength="8" onblur="Stock_RIDYearChange()"
                                        onkeyup="Stock_RIDYearChange()" Style="ime-mode: disabled" Width="100px"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID1" runat="server" MaxLength="2" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="txtChange1()" Style="ime-mode: disabled" Width="40px"></asp:TextBox>-
                                    <asp:TextBox ID="txtStock_RID2" runat="server" MaxLength="2" onblur="ChecktxtStock_RID(this)"
                                        onkeyup="txtChange2()" Style="ime-mode: disabled" Width="40px"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="bottom" id="buttonRow1" class="tbl_row">
                    <td align="right">
                        &nbsp;<asp:Button ID="btnSearch1" runat="server" CssClass="btn" Text="查詢" OnClick="btnSearch1_Click" />
                        &nbsp;&nbsp; &nbsp;<asp:Button ID="btnCancel1" runat="server" CausesValidation="False"
                            class="btn" OnClick="btnCancel1_Click" Text="取消" />
                    </td>
                </tr>
                <tr valign="bottom" id="buttonRow2" class="tbl_row">
                    <td align="right">
                        &nbsp;<asp:Button ID="btnSearch2" runat="server" CssClass="btn" Text="查詢" OnClick="btnSearch2_Click" />
                        &nbsp;&nbsp; &nbsp;<asp:Button ID="btnCancel2" runat="server" CausesValidation="False"
                            class="btn" OnClick="btnCancel1_Click" Text="取消" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <table id="queryResult1" width="100%" style="display:none">
                            <tr>
                                <td colspan="12" style="width: 100%">
                                    <cc1:GridViewPageingByDB ID="gvpbSplitWork" runat="server" CssClass="GridView" Width="1px"
                                        SortDescImageUrl="~/images/desc.gif" SortAscImageUrl="~/images/asc.gif" PreviousPageText="<"
                                        NextPageText=">" LastPageText=">>" FirstPageText="<<" EditPageUrl="#" EditCellWidth="48px"
                                        EditButtonText="編輯" EditButtonImageUrl="" ColumnsHeaderText="" AutoGenerateColumns="False"
                                        OnOnSetDataSource="gvpbSplitWork_OnSetDataSource" OnRowDataBound="gvpbSplitWork_RowDataBound">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                                <itemstyle horizontalalign="Left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="空白卡厰" DataField="Factory_ShortName_CN"></asp:BoundField>
                                            <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號"></asp:BoundField>
                                            <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號"></asp:BoundField>
                                            <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號 "></asp:BoundField>
                                            <asp:BoundField DataField="Operate_Type" HeaderText="進貨作業" HtmlEncode="False">
                                                <itemstyle horizontalalign="Left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <%--add chaoma by 201005515-0 start--%>
                                            <asp:BoundField DataField="Number" HeaderText="訂單數量 " DataFormatString="{0:N0}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="RIC_Number" HeaderText="入庫數量 " DataFormatString="{0:N0}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <%--add chaoma end--%>
                                            <asp:BoundField DataField="Income_Date" HeaderText="日期">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Unit_Price" HeaderText="含稅單價" DataFormatString="{0:N4}" HtmlEncode="False" >
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="未稅單價 " DataField="Unit_Price1" DataFormatString="{0:N4}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="拆單數量">
                                                <itemstyle horizontalalign="Right" />
                                                <itemtemplate>
<asp:TextBox id="txtSplit" runat="server" Width="45px" __designer:wfdid="w5" onblur="CheckNumWithNoId(this,2)" style="ime-mode:disabled;text-align: right" MaxLength="2" onkeyup="CheckNumWithNoId(this,2)"></asp:TextBox>
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Comment" HeaderText="備註" />
                                            <asp:BoundField DataField="SAP_ID" />
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="14" style="height: 26px">
                                    &nbsp;<asp:Button ID="btnSubmit1" runat="server" class="btn" Text="確定" OnClick="btnSubmit1_Click" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="queryResult2" width="100%" style="display:none">
                            <tr>
                                <td colspan="13">
                                    <cc1:GridViewPageingByDB ID="gvpbRequisitionWork" runat="server" CssClass="GridView"
                                        Width="1px" SortDescImageUrl="~/images/desc.gif" SortAscImageUrl="~/images/asc.gif"
                                        PreviousPageText="<" NextPageText=">" LastPageText=">>" FirstPageText="<<" EditPageUrl="#"
                                        EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl="" ColumnsHeaderText=""
                                        AutoGenerateColumns="False" OnOnSetDataSource="gvpbRequisitionWork_OnSetDataSource"
                                        OnRowDataBound="gvpbRequisitionWork_RowDataBound">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" Visible="False" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:BoundField DataField="NAME" HeaderText="版面簡稱">
                                                <itemstyle horizontalalign="Left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="空白卡厰" DataField="Factory_ShortName_CN"></asp:BoundField>
                                            <asp:BoundField DataField="Budget_ID" HeaderText="預算簽呈文號"></asp:BoundField>
                                            <asp:BoundField DataField="Agreement_Code" HeaderText="合約編號"></asp:BoundField>
                                            <asp:BoundField DataField="Stock_RID" HeaderText="入庫流水編號 "></asp:BoundField>
                                            <asp:BoundField DataField="Operate_Type" HeaderText="進貨作業" HtmlEncode="False">
                                                <itemstyle horizontalalign="Left"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <%--add chaoma by 201005515-0 start--%>
                                            <asp:BoundField DataField="Number" HeaderText="訂單數量 " DataFormatString="{0:N0}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="RIC_Number" HeaderText="入庫數量 " DataFormatString="{0:N0}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <%--add chaoma end--%>
                                            <asp:BoundField DataField="Income_Date" HeaderText="日期">
                                                <itemstyle horizontalalign="Right"></itemstyle>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Unit_Price" HeaderText="含稅單價" DataFormatString="{0:N4}" HtmlEncode="False">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="未稅單價 ">
                                                <itemstyle horizontalalign="Right" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="是否請款">
                                                <itemtemplate>
&nbsp;<asp:CheckBox id="cbISRequisition" runat="server" __designer:wfdid="w8"></asp:CheckBox>
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="發票號碼">
                                                <itemtemplate>
<asp:CheckBox id="cbInvoiceNumber" runat="server" __designer:wfdid="w11"></asp:CheckBox> <asp:Label id="lbInvoiceNumber" runat="server" __designer:wfdid="w10"></asp:Label>
</itemtemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Comment" HeaderText="備註" />
                                            <asp:BoundField DataField="SAP_ID" />
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="14" style="height: 27px">
                                    <asp:TextBox ID="txtInvoiceNumber" runat="server" MaxLength="12"></asp:TextBox>
                                    <asp:Button ID="btnInvoiceNumber" runat="server" class="btn" Text="填寫發票號碼" OnClick="btnInvoiceNumber_Click" /></td>
                            </tr>
                            <tr>
                                <td align="right" colspan="14" style="height: 26px">
                                    &nbsp;<asp:Button ID="btnSubmit2" runat="server" class="btn" Text="確定" OnClick="btnSubmit2_Click" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <script language="javascript" type="text/javascript">
            function show(row)
            {
                if(row=='buttonRow1')
                {			   	
                    document.getElementById('td1').style.display = "none";
                    document.getElementById('td2').style.display = "none";
                    document.getElementById('queryResult1').style.display = "none";
                    document.getElementById('queryResult2').style.display = "none";
                    document.getElementById("buttonRow1").style.display = "";
                    document.getElementById("buttonRow2").style.display = "none";
                    
                }else if (row=='buttonRow2')
                {
                    document.getElementById('td1').style.display = "";
                    document.getElementById('td2').style.display = "";
                    document.getElementById('queryResult1').style.display = "none";
                    document.getElementById('queryResult2').style.display = "none";
                    document.getElementById("buttonRow1").style.display = "none";
                    document.getElementById("buttonRow2").style.display = "";
                }
            }
           
            function doQuery()
            {            
               if(true == document.getElementById("fuctionR1").checked)
               {
                    document.getElementById('td1').style.display = "none";
                    document.getElementById('td2').style.display = "none";
                    document.getElementById('queryResult1').style.display = "";
                    document.getElementById('queryResult2').style.display = "none";
                    document.getElementById("buttonRow1").style.display = "";
                    document.getElementById("buttonRow2").style.display = "none";
               }
               else
               {
                    document.getElementById('td1').style.display = "";
                    document.getElementById('td2').style.display = "";
                    document.getElementById('queryResult1').style.display = "none";
                    document.getElementById('queryResult2').style.display = "";
                    document.getElementById("buttonRow1").style.display = "none";
                    document.getElementById("buttonRow2").style.display = "";
               }
           }
           
            function Stock_RIDYearChange()
            {
                if(document.getElementById("txtStock_RIDYear").value!="")
                {
                    document.getElementById("txtStock_RID1").disabled=false;
                }
                else
                {
                    document.getElementById("txtStock_RID1").value="";
                    document.getElementById("txtStock_RID2").value="";
                    document.getElementById("txtStock_RID1").disabled=true;
                    document.getElementById("txtStock_RID2").disabled=true;
                }
            }
                
            function txtChange1()
            {
                CheckNum('txtStock_RID1',2)
                if(document.getElementById("txtStock_RID1").value!="")
                {
                    document.getElementById("txtStock_RID2").disabled=false;
                }
                else
                {
                    document.getElementById("txtStock_RID2").value="";
                    document.getElementById("txtStock_RID2").disabled=true;
                }
            }
                
            function txtChange2()
            {
                CheckNum('txtStock_RID2',2)
            }
            
            function ChecktxtStock_RID(obj)
            {
                var num=obj.value;
                if(num!="")
                {
                    if(num.length==1)
                    {
                        obj.value=0+num;
                    }
                }
                txtChange1();
                txtChange2();
            }
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
