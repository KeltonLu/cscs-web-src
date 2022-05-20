<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="BaseInfo001Edit.aspx.cs" Inherits="BaseInfo_BaseInfo001Edit" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>未命名頁面</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <style>
        input.file
        {
            background-color: #b5dcb8;
            color: #5b4223;
            font-size: 111px;
            font-weight: bold;
            letter-spacing: 11px;
            padding: 11px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                        <tr>
                            <td>&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                                    <td class="PageTitle">
                                        <asp:Label ID="lbTitle" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClientClick="return ConfirmDel();" OnClick="btnSubmit1_Click" />&nbsp;&nbsp;
                            <input class="btn" onclick="returnform('BaseInfo001.aspx')" id="Button2" type="button" value="取消" />
                                    </td>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr class="tbl_title">
                                        <td align="center">主預算簽呈
                                        </td>
                                    </tr>
                                    <tr class="tbl_row">
                                        <td>
                                            <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                                <tr>
                                                    <td width="70%">
                                                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                                            <tr valign="baseline">
                                                                <td align="right" width="14.5%">
                                                                    <span class="style1">*</span>簽呈文號：
                                                                </td>
                                                                <td width="85%" colspan="3">
                                                                    <asp:TextBox ID="txtBUDGET_ID" onblur="LimitLengthCheck(this,30);" onkeyup="LimitLengthCheck(this,30);" runat="server" MaxLength="30"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBUDGET_ID"
                                                                        ErrorMessage="簽呈文號不能為空">*</asp:RequiredFieldValidator>
                                                                    <cc1:AjaxValidator ID="ajvBudgetID" runat="server" ControlToValidate="txtBUDGET_ID" ErrorMessage="簽呈文號已經存在" OnOnAjaxValidatorQuest="AjaxValidator1_OnAjaxValidatorQuest">簽呈文號已經存在</cc1:AjaxValidator></td>
                                                            </tr>
                                                            <tr valign="baseline">
                                                                <td align="right">
                                                                    <span class="style1">*</span>預算名稱：
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:TextBox ID="txtBUDGET_NAME" onblur="LimitLengthCheck(this,30);" onkeyup="LimitLengthCheck(this,30);" runat="server" MaxLength='30'></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtBUDGET_NAME"
                                                                        ErrorMessage="預算名稱不能為空">*</asp:RequiredFieldValidator>
                                                                    <cc1:AjaxValidator ID="ajvBudgetName" runat="server" ControlToValidate="txtBUDGET_NAME" ErrorMessage="預算名稱已經存在" OnOnAjaxValidatorQuest="ajvBudgetName_OnAjaxValidatorQuest">預算名稱已經存在</cc1:AjaxValidator>
                                                                </td>
                                                            </tr>
                                                            <tr valign="baseline">
                                                                <td align="right">
                                                                    <span class="style1">*</span>上傳簽呈影像：
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:FileUpload ID="fludFileUpload" runat="server" Width="200px" />&nbsp;&nbsp;
                                                        <asp:Button ID="btnUpload" runat="server" CssClass="btn" Text="上傳" OnClick="btnUpload_Click" CausesValidation="False" />
                                                                    <asp:HyperLink ID="HyperLink" runat="server" Target="_blank"></asp:HyperLink></td>
                                                            </tr>
                                                            <tr valign="baseline">
                                                                <td align="right">
                                                                    <span class="style1">*</span>金額：
                                                                </td>
                                                                <td colspan="3">
                                                                    <%--Dana 20161021 最大長度由13改為16 --%>
                                                                    <asp:TextBox ID="txtCard_Price" onfocus="DelDouhao(this)" onkeyup="CalCardAmt();" Style="ime-mode: disabled; text-align: right" onblur=" CheckAmt('txtCard_Price',10,2);value=GetValue(this.value)" runat="server" MaxLength="16"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtCard_Price"
                                                                        ErrorMessage="金額不能為空">*</asp:RequiredFieldValidator>
                                                                </td>
                                                            </tr>
                                                            <tr valign="baseline">
                                                                <td align="right">卡數：
                                                                </td>
                                                                <td colspan="3">
                                                                    <%--Dana 20161021 最大長度由9改為11 --%>
                                                                    <asp:TextBox ID="txtCard_Num" onfocus="DelDouhao(this)" onkeyup="CalCardNum();" Style="ime-mode: disabled; text-align: right" onblur="CheckNum('txtCard_Num',9);value=GetValue(this.value)" runat="server" MaxLength="11"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr valign="baseline">
                                                                <td align="right">
                                                                    <span class="style1">*</span>有效期間：
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:TextBox ID="txtVALID_DATE_FROM" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_FROM')})" src="../images/calendar.gif" align="absmiddle">
                                                                    ~<asp:TextBox ID="txtVALID_DATE_TO" onfocus="WdatePicker()" runat="server" Width="80px" MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtVALID_DATE_TO')})" src="../images/calendar.gif" align="absmiddle">
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="有效期間（起）不能為空" ControlToValidate="txtVALID_DATE_FROM">*</asp:RequiredFieldValidator>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="有效期間（迄）不能為空" ControlToValidate="txtVALID_DATE_TO">*</asp:RequiredFieldValidator>
                                                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtVALID_DATE_FROM"
                                                                        ControlToValidate="txtVALID_DATE_TO" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                                                        Type="Date">*</asp:CompareValidator></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <asp:Image ID="imgIMG_FILE_URL" ImageUrl="../images/NoPic.jpg" runat="server" Height="100px" Width="100px" Visible="False" /></td>
                                                </tr>
                                                <tr>
                                                    <td>

                                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                                                </td>
                                                            </tr>
                                                        </table>

                                                    </td>
                                                    <td>&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                     <ContentTemplate>
                                        <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                            <tr><td>
                                                <tr valign="baseline">
                                                    <td>
                                                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                                            <tr class="tbl_title">
                                                                <td align="center">追加預算簽呈
                                                                </td>
                                                            </tr>
                                                            <tr class="tbl_row">
                                                                <td>

                                                                    <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                                                        <tr id='trGrid' style="display: table-row">
                                                                            <td>
                                                                                <cc1:GridViewPageingByDB ID="gvpbBudgetAppend" runat="server" OnOnSetDataSource="gvpbBudgetAppend_OnSetDataSource" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbBudgetAppend_RowDataBound" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                                                                    <RowStyle CssClass="GridViewRow" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="BUDGET_ID" HeaderText="簽呈文號" SortExpression="BUDGET_ID">
                                                                                            <ItemStyle Width="20%" />
                                                                                        </asp:BoundField>
                                                                                        <asp:BoundField DataField="Budget_Name" HeaderText="預算名稱">
                                                                                            <ItemStyle Width="20%" />
                                                                                        </asp:BoundField>
                                                                                        <asp:BoundField DataField="Card_Price" HeaderText="金額" DataFormatString="{0:N2}" HtmlEncode="False">
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Right" />
                                                                                        </asp:BoundField>
                                                                                        <asp:BoundField DataField="Card_Num" HeaderText="卡數" DataFormatString="{0:N0}" HtmlEncode="False">
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Right" />
                                                                                        </asp:BoundField>
                                                                                        <asp:BoundField DataField="VALID_DATE_FROM" HeaderText="有效期間（起）" SortExpression="VALID_DATE">
                                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                                                        </asp:BoundField>
                                                                                        <asp:BoundField DataField="VALID_DATE_TO" HeaderText="有效期間（迄）" SortExpression="VALID_DATE">
                                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                                                        </asp:BoundField>
                                                                                        <asp:TemplateField HeaderText="簽呈影像檔">
                                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                                                            <ItemTemplate>
                                                                                                <asp:HyperLink ID="hlinkImg" runat="server" __designer:wfdid="w2"></asp:HyperLink>
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="刪除">
                                                                                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                            <ItemTemplate>
                                                                                                <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif" OnCommand="btnDelete_Command"></asp:ImageButton>

                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="修改">
                                                                                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                            <ItemTemplate>
                                                                                                <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnUpdate" Height="18px" ImageUrl="~/images/pattern_1.gif" OnCommand="ibtnUpdate_Command"></asp:ImageButton>

                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                    </Columns>

                                                                                </cc1:GridViewPageingByDB>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Button ID="btnAddBudget" CausesValidation="false" CssClass="btn" runat="server" Text="追加預算" OnClick="btnAddBudget_Click" />
                                                                                <%-- Legend 20161024 將 Visible="false" 改為 style="display: none;" --%>
                                                                                <asp:Button ID="btnBudgetAppend" style="display: none;" runat="server" Text="Button" OnClick="btnBudgetAppend_Click" CausesValidation="False" /></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr valign="baseline">
                                                    <td>
                                                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                                            <tr class="tbl_title">
                                                                <td align="center" colspan="2">&nbsp;
                                                                </td>
                                                            </tr>
                                                            <tr class="tbl_row">
                                                                <td>
                                                                    <table cellpadding="0" width="100%" cellspacing="2" border="0">
                                                                        <tr valign="baseline">
                                                                            <td align="right" width="15%" style="height: 24px">總金額：
                                                                            </td>
                                                                            <td width="15%" style="height: 24px">
                                                                                <asp:TextBox ID="txtTOTAL_CARD_AMT" runat="server" ReadOnly="True" MaxLength="16"></asp:TextBox></td>
                                                                            <td width="15%" align="right" style="height: 24px">總卡數：
                                                                            </td>
                                                                            <td align="left" style="height: 24px">
                                                                                <asp:TextBox ID="txtTOTAL_CARD_NUM" runat="server" ReadOnly="True" MaxLength="9"></asp:TextBox></td>
                                                                        </tr>
                                                                        <tr id="trResult" runat="server">
                                                                            <td align="right">異動原因： 
                                                                            </td>
                                                                            <td colspan="3">
                                                                                <asp:TextBox ID="txtReason" runat="server" Height="95px" Width="448px" TextMode="MultiLine"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td></td>
                                                                            <td colspan="3" id='tdDel' style="display: none">
                                                                                <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                            <td align="right">
                                                <input id="hdCardAmt" runat="server" type="hidden" style="width: 1px" />
                                                <input id="hdCardNum" runat="server" type="hidden" style="width: 1px" />
                                                <input id="hdRID" runat="server" type="hidden" />
                                                <input id="hdAppCardAmt" value="0" runat="server" type="hidden" />
                                                <input id="hdAppCardNum" value="0" runat="server" type="hidden" />
                                                <asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClientClick="return ConfirmDel();" OnClick="btnSubmit1_Click" />&nbsp;&nbsp;
                                    <input class="btn" onclick="returnform('BaseInfo001.aspx')" id="Button3" type="button" value="取消" />
                                        </tr>
                                           </td></tr> 
                                        </table>
                                     </ContentTemplate>
                                 </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
               


            <script type="text/javascript">
                function AddBudgetAppend()
                {
                    if (document.getElementById("hdRID").value != "")
                    {
                        document.getElementById("chkDel").checked = false;
                    }
                    document.getElementById("trGrid").style.display = "table-row";
                    __doPostBack('btnBudgetAppend', '');
                }

                function CalCardAmt()
                {
                    var CardAmt = CheckAmt('txtCard_Price', 10, 2);

                    var hdCardAmt = Number(document.getElementById("hdAppCardAmt").value);

                    var totalCarAMT = Number(CardAmt) + hdCardAmt + "";

                    var arrayNum = totalCarAMT.split('.');

                    if (arrayNum.length == 1)
                    {
                        totalCarAMT += ".00";
                    }
                    else (arrayNum.length == 2)
                    {
                        if (arrayNum[1] == "" || arrayNum[1] == undefined)
                        {
                            arrayNum[1] = "00";
                        }
                        else
                        {
                            if (arrayNum[1].length == 1)
                            {
                                arrayNum[1] += "0";
                            }
                            else if (arrayNum[1].length > 1)
                            {
                                arrayNum[1] = arrayNum[1].substring(0, 2);
                            }
                        }
                        totalCarAMT = arrayNum[0] + "." + arrayNum[1];
                    }

                    document.getElementById("txtTOTAL_CARD_AMT").value = GetValue(totalCarAMT);
                }

                function CalCardNum()
                {
                    var CardNum = CheckNum('txtCard_Num', 9);

                    var hdCardNum = Number(document.getElementById("hdAppCardNum").value);

                    var totalCarNum = Number(CardNum) + hdCardNum;

                    document.getElementById("txtTOTAL_CARD_NUM").value = GetValue(totalCarNum);
                }

                function CalDelNum(DelAmt, DelNum)
                {
                    document.getElementById("hdAppCardAmt").value = Number(document.getElementById("hdAppCardAmt").value) - Number(DelAmt);
                    document.getElementById("hdAppCardNum").value = Number(document.getElementById("hdAppCardNum").value) - Number(DelNum);

                    CalCardAmt();
                    CalCardNum();
                }

                function EnableDel(flag)
                {

                    if (document.getElementById("hdRID").value != "")
                    {
                        document.getElementById("tdDel").style.display = "table-row";
                        document.getElementById('chkDel').checked = false;
                    }
                    document.getElementById("trGrid").style.display = "none";
                }

                function ConfirmDel()
                {
                    if (document.getElementById('chkDel').checked)
                    {
                        return confirm("是否刪除此筆訊息");
                    }
                    else
                    {
                        return CheckClientValidate();
                    }
                }


            </script>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
