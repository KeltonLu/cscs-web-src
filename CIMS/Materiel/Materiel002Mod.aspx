<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Materiel002Mod.aspx.cs"
    Inherits="Materiel_Materiel002Mod" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>寄卡單種類基本檔修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
        function doRadio(rad)
        {
            if (rad == document.getElementById("adrtSafe_Type2"))
            {
                document.getElementById("adrtSafe_Type2").checked = true;
                document.getElementById("txtSafe_Number2").disabled = false;
                document.getElementById("adrtSafe_Type1").checked = false;
                document.getElementById("txtSafe_Number1").disabled = true;
                document.getElementById("txtSafe_Number1").value = "";
            }
            else
            {
                document.getElementById("adrtSafe_Type1").checked = true;
                document.getElementById("txtSafe_Number1").disabled = false;
                document.getElementById("adrtSafe_Type2").checked = false;
                document.getElementById("txtSafe_Number2").disabled = true;
                document.getElementById("txtSafe_Number2").value = "";
            }
        }   
        
        function checkRadio()
        {
            if(document.getElementById("adrtSafe_Type1").checked)
            {
                document.getElementById("adrtSafe_Type1").checked = true;
                document.getElementById("txtSafe_Number1").disabled = false;
                document.getElementById("adrtSafe_Type2").checked = false;
                document.getElementById("txtSafe_Number2").disabled = true;
                document.getElementById("txtSafe_Number2").value = "";
            }
            else
            {
                document.getElementById("adrtSafe_Type2").checked = true;
                document.getElementById("txtSafe_Number2").disabled = false;
                document.getElementById("adrtSafe_Type1").checked = false;
                document.getElementById("txtSafe_Number1").disabled = true;
                document.getElementById("txtSafe_Number1").value = "";
            }
         }     
        
        function doDelConfirm()
            {
                  if(document.getElementById('chkDel').checked)
                    {
                        return confirm("是否刪除此筆訊息");
                    }
                  else
                    {
                        return CheckClientValidate();
                    }
            }   
            
        function delConfirm()
        {
            if (true==confirm('寄卡單已被使用，是否確定刪除？'))
               {
                    __doPostBack('Button1',''); 
               }
        }      
 //-->
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline" height="20px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td class="PageTitle">
                        寄卡單種類基本檔修改/刪除
                    </td>
                    <td align="right">
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" style="display: none;" />
                        <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click"
                            OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('Materiel002.aspx')" type="button"
                            value="取消" /></td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td colspan="2">
                        <table cellpadding="0" width="100%" cellspacing="0" border="0">
                            <tr class="tbl_row" valign="baseline">
                                <td width="15%" align="right">
                                    品名編號：</td>
                                <td with="85%">
                                    <asp:HiddenField ID="hidRID" runat="server" />
                                    <asp:HiddenField ID="hidRCU" runat="server" />
                                    <asp:HiddenField ID="hidRCT" runat="server" />
                                    <asp:Label ID="lblSerial_Number" runat="server"></asp:Label>
                            </tr>
                            <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                <td align="right">
                                    <span style="color: #ff0000">*</span>品名：</td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" MaxLength='30' onkeyup=" LimitLengthCheck(this, 30)"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="品名不能為空">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                <td align="right">
                                    <span style="color: #ff0000">*</span>單價：</td>
                                <td>
                                                        <%--Dana 20161021 最大長度由 7 改為 8 --%>
                                    <asp:TextBox ID="txtUnit_Price" runat="server" onfocus="DelDouhao(this)" onblur="CheckAmt('txtUnit_Price',4,2);value=GetValue(this.value)"
                                        onkeyup="CheckAmt('txtUnit_Price',4,2)" MaxLength="8" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                            runat="server" ControlToValidate="txtUnit_Price" ErrorMessage="單價不能為空">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                <td align="right">
                                    <span style="color: #ff0000">*</span>耗損率：</td>
                                <td>
                                    <asp:TextBox ID="txtWear_Rate" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtWear_Rate',3)" runat="server" MaxLength="3" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>%
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtWear_Rate"
                                        ErrorMessage="耗損率不能為空">*</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtWear_Rate"
                                        ErrorMessage="耗損率不能超過100" Operator="LessThanEqual" ValueToCompare="100" Type="Integer">*</asp:CompareValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline" style="font-size: 10pt">
                                <td align="right">
                                    <span style="color: #ff0000">*</span>安全存量：</td>
                                <td>
                                    <input id="adrtSafe_Type1" name="radioSafeType" runat="server" type="radio" onclick="doRadio(this);"
                                        checked="true" />最低安全庫存
                                                        <%--Dana 20161021 最大長度由 6 改為 7 --%>
                                    <asp:TextBox ID="txtSafe_Number1" runat="server" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtSafe_Number1',6)" MaxLength="7" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>
                                    <input id="adrtSafe_Type2" type="radio" name="radioSafeType" runat="server" onclick="doRadio(this);" />安全天數
                                    <asp:TextBox ID="txtSafe_Number2" runat="server" onfocus="DelDouhao(this)" onblur="value=GetValue(this.value)"
                                        onkeyup="CheckNum('txtSafe_Number2',3)" MaxLength="3" Style="ime-mode: disabled;
                                        text-align: right"></asp:TextBox>天</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    帳務類別：
                                </td>
                                <td>
                                    <input id="adrtCard" runat="server" checked="true" name="radioUse_Stop" type="radio"
                                        value="1" />信用卡帳&nbsp; &nbsp;
                                    <input id="adrtBlank" runat="server" name="radioUse_Stop" type="radio" value="2" />銀行帳
                                </td>
                            </tr>
                            <%-- add by linhuanhuang start --%>
                            <tr>
                                <td align="right">
                                    使用到期日期：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMaturityDate" onfocus="WdatePicker()" runat="server" Width="80px"
                                        MaxLength="10"></asp:TextBox>&nbsp;
                                    <img onclick="WdatePicker({el:$dp.$('txtMaturityDate')})" src="../images/calendar.gif"
                                        align="absmiddle">
                                </td>
                            </tr>
                            <%-- add by linhuanhuang end --%>
                            <tr class="tbl_row" valign="baseline">
                                <td colspan="2" align="right" bgcolor="#FFFFFF" style="height: 2px">
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td colspan="2">
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                        <tr valign="baseline">
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="left" colspan="2">
                                    <table width="100%" cellpadding="0" cellspacing="2" border="0" bgcolor="#ffffff">
                                        <tr class="tbl_row" valign="baseline">
                                            <td style="width: 15%" align="right">
                                                寄卡單圖檔：
                                            </td>
                                            <td style="width: 70%">
                                                &nbsp;<asp:FileUpload ID="fludFileUpload" runat="server" Width="200px" />
                                                <asp:Button ID="btnUpload" runat="server" CausesValidation="False" CssClass="btn"
                                                    OnClick="btnUpload_Click" Text="上傳" />
                                                <asp:HyperLink ID="HyperLink" Visible="false" runat="server" Target="_blank"></asp:HyperLink></td>
                                            <td style="width: 15%">
                                            </td>
                                        </tr>
                                        <tr class="tbl_row" valign="baseline">
                                            <td colspan="3">
                                                <cc1:GridViewPageingByDB ID="grvbImg" runat="server" AutoGenerateColumns="False"
                                                    ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                                                    EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText="<"
                                                    OnOnSetDataSource="grvbImg_OnSetDataSource" OnRowDataBound="grvbImg_RowDataBound"
                                                    PreviousPageText=">" ShowHeader="False">
                                                    <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&lt;"
                                                        PreviousPageText="&gt;" Visible="False" />
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <itemstyle width="15%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="圖片">
                                                            <itemstyle horizontalalign="Left" width="70%" />
                                                            <itemtemplate>
                                                                <asp:HyperLink id="hlFile_Name" runat="server"></asp:HyperLink> 
                                                                </itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="刪除">
                                                            <itemstyle horizontalalign="Center" width="15%" />
                                                            <itemtemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDeleteImg" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDeleteImg_Command"></asp:ImageButton>
                                                            
</itemtemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                </cc1:GridViewPageingByDB>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td width="12%" align="right">
                                    &nbsp;
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="right" colspan="2">
                        <input id="radSafe_Type" runat="server" type="hidden" value="0" /><input id="txtSafe_Number"
                            runat="server" type="hidden" value="0" /><asp:Button CssClass="btn" ID="btnSubmit2"
                                runat="server" Text="確定" OnClick="btnSubmit_Click" OnClientClick="return doDelConfirm();" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('Materiel002.aspx')" type="button"
                            value="取消" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
