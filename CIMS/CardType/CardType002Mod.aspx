<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType002Mod.aspx.cs" Inherits="CardType_CardType002Mod" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡種基本資料修改</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">
    function delConfirm()
        {
            if (true == document.getElementById("chkDelete").checked)
            {
               var answer =  confirm('確認刪除此筆訊息?');
               if(answer ==true)
               {
                  return true;
               }
               else
               { 
                  document.getElementById("chkDelete").checked = false;
                  return false;
               }
            }
            else
            {
                return CheckClientValidate();
            }
        } 	
</script>

<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline">
                    <td align="right" colspan="2" style="height: 20px">
                    </td>
                </tr>
                <tr valign="baseline">
                    <td style="width: 35%">
                        <font class="PageTitle">卡種基本資料維護作業修改/刪除</font></td>
                    <td align="right">
                        <asp:Button ID="btnSubmit1" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            Text="確定" />
                        &nbsp;&nbsp;<asp:Button ID="btnCancel1" runat="server" CausesValidation="False" CssClass="btn"
                            OnClick="btnCancel_Click" Text="取消" />
                    </td>
                </tr>
                <tr valign="baseline" class="tbl_row">
                    <td colspan="2">
                        <table border="0" cellpadding="0" cellspacing="0" id="TABLE1" width="100%">
                            <tr class="tbl_row" valign="baseline">
                                <td align="right" style="width: 15%">
                                    <span style="color: #ff0000; background-color: #e4e4e4">*</span>使用狀態：</td>
                                <td align="left">
                                    <input id="adrtUse" runat="server" checked="true" name="radioUse_Stop" type="radio"
                                        value="1" />使用&nbsp; &nbsp;
                                    <input id="adrtStop" runat="server" name="radioUse_Stop" type="radio" value="2" />停用
                                    <asp:HiddenField ID="hdRID" runat="server" />
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span style="color: #ff0000">*</span>CARD TYPE：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTYPE" MaxLength="3" Style="ime-mode: disabled; "
                                        runat="server" Width="30px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtTYPE"
                                        ErrorMessage="CARD TYPE必須輸入">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtTYPE"
                                        ErrorMessage="CARD TYPE必須為三位數字" ValidationExpression="\d{3}">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span style="color: #ff0000; background-color: #e4e4e4">*</span>AFFINITY：</td>
                                <td>
                                    <asp:TextBox ID="txtAFFINITY" MaxLength="4" Style="ime-mode: disabled;" runat="server" Width="40px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtAFFINITY"
                                        ErrorMessage="AFFINITY必須輸入">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtAFFINITY"
                                        ErrorMessage="AFFINITY必須為四位數字" ValidationExpression="\d{4}">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span style="color: #ff0000; background-color: #e4e4e4">*</span>PHOTO：</td>
                                <td>
                                    <asp:TextBox ID="txtPHOTO" MaxLength="2" Style="ime-mode: disabled;"
                                        runat="server" Width="20px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtPHOTO"
                                        ErrorMessage="PHOTO必須輸入">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtPHOTO"
                                        ErrorMessage="PHOTO必須為二位數字" ValidationExpression="\d{2}">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    BIN：</td>
                                <td>
                                    <asp:TextBox ID="txtBIN" MaxLength="6" Style="ime-mode: disabled; text-align: right"
                                        runat="server" Width="60px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtBIN"
                                        ErrorMessage="BIN必須為六位以內數字" ValidationExpression="\d{0,6}">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    <span style="color: #ff0000; background-color: #e4e4e4">*</span>版面簡稱：</td>
                                <td>
                                    <asp:TextBox ID="txtName" onkeyup="LimitLengthCheck(this, 30)" MaxLength="30" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtName"
                                        ErrorMessage="版面簡稱必須輸入">*</asp:RequiredFieldValidator>
                                    <cc1:AjaxValidator ID="ajvCardTypeName" runat="server" ClientValidationFunction="AjaxValidatorFunction"
                                        ControlToValidate="txtName" ErrorMessage="版面簡稱已經存在" OnOnAjaxValidatorQuest="AjaxValidator_CardType_Name_OnAjaxValidatorQuest"></cc1:AjaxValidator></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline" height="25px">
                                <td align="right">
                                    換卡版面：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropChange_Space_RID" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline" height="25px">
                                <td align="right">
                                    替換卡版面：</td>
                                <td>
                                    <asp:DropDownList ID="dropReplace_Space_RID" runat="server">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    起始日期：</td>
                                <td>
                                    <asp:Label ID="lblBegin_Time" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    終止日期：</td>
                                <td>
                                    <asp:TextBox ID="txtEnd_Time" runat="server" Width="80px" onfocus="WdatePicker()" MaxLength="10"></asp:TextBox>&nbsp;
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtEnd_Time')})" src="../images/calendar.gif" /></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    特殊材質：</td>
                                <td>
                                    <asp:Label ID="lblMateriel" runat="server"></asp:Label></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    信封：</td>
                                <td>
                                    <asp:Label ID="lblEnvelope" runat="server"></asp:Label>
                                    <asp:HiddenField ID="hdEnvelope_RID" runat="server" /></td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="right">
                                    寄卡單：</td>
                                <td>
                                    <asp:Label ID="lblCARD_EXPONENT" runat="server"></asp:Label>
                                    <asp:HiddenField ID="hdExponent_RID" runat="server" /></td>
                            </tr>
                             <tr valign="baseline" class="tbl_row">
                                <td align="right">
                                    列印方式：</td>
                                <td>
                                    正面&nbsp;&nbsp;<asp:TextBox ID="txtPrint_Cover" runat="server" MaxLength="20" onkeyup="LimitLengthCheck(this, 20)" Width="250px"></asp:TextBox>
                                    背面&nbsp;&nbsp;<asp:TextBox ID="txtPrint_Back" runat="server" MaxLength="20" onkeyup="LimitLengthCheck(this, 20)" Width="250px"></asp:TextBox>
                                    OCR碼數&nbsp;&nbsp; <asp:TextBox style="ime-mode:disabled;text-align: right" onblur="CheckNum('txtOCR',2)" onkeyup="CheckNum('txtOCR',2)" ID="txtOCR" runat="server" MaxLength="2" Width="30px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="tbl_row">
                                <td align="right">
                                    備註：</td>
                                <td>
                                    <asp:TextBox ID="txtComment" runat="server"  Width="600px" Height="50px" TextMode="MultiLine"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="left" colspan="2">
                                    <table width="100%">
                                        <tr valign="baseline">
                                            <td>
                                                <cc1:GridViewPageingByDB ID="grvbPersoFactory" runat="server" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                                    EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                                    FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                    PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" Visible="False" />
                                                    <Columns>
                                                        <asp:BoundField DataField="FactoryName" HeaderText="Perso廠">
                                                            <itemstyle horizontalalign="Center" width="30%" />
                                                            <headerstyle horizontalalign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField HeaderText="製程" DataField="Project_StepName">
                                                            <itemstyle horizontalalign="Center" width="30%"/>
                                                            <headerstyle horizontalalign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField HeaderText="總金額" DataField="AmtTotal" DataFormatString="{0:N4}" HtmlEncode="False">
                                                            <itemstyle horizontalalign="Right" width="40%" />
                                                            <headerstyle horizontalalign="Center" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                    <table cellspacing="0" cellpadding="0" border="1" style="width: 100%;border-collapse: collapse;">
                                                        <tr style="color: Black; background-color: #B9BDAA; font-size: Small;">
                                                            <th scope="col" style="width: 30%;" align="center">
                                                                Perso廠</th>
                                                            <th scope="col" style="width: 30%;" align="center">
                                                                製程</th>
                                                            <th scope="col" style="width: 40%;" align="center">總金額
                                                                </th>
                                                        </tr>
                                                    </table>
                                                </EmptyDataTemplate>
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                </cc1:GridViewPageingByDB>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr valign="baseline" bgcolor="#ffffff">
                                <td align="right" colspan="2" style="height: 2px">
                                </td>
                            </tr>
                            <tr class="tbl_row" valign="baseline">
                                <td align="left" colspan="2">
                                    <table width="100%">
                                        <tr valign="baseline">
                                            <td>
                                                <cc1:GridViewPageingByDB ID="grvbCardTypeGroup" runat="server" AllowPaging="True"
                                                    AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                                    EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                                    FirstPageText="<<" LastPageText=">>" NextPageText=">" OnOnSetDataSource="grvbCardTypeGroup_OnSetDataSource"
                                                    PreviousPageText="<" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                    OnRowDataBound="grvbCardTypeGroup_RowDataBound">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" Visible="False" />
                                                    <Columns>
                                                        <asp:BoundField HeaderText="用途" DataField="Param_Name">
                                                            <itemstyle horizontalalign="Center" width="50%" />
                                                            <headerstyle horizontalalign="Center" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="群組">
                                                            <itemstyle horizontalalign="Center" width="50%" />
                                                            <headerstyle horizontalalign="Center" />
                                                            <itemtemplate>
                                                                <asp:DropDownList id="dropCard_Group" runat="server">
                                                                </asp:DropDownList>
                                                            
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
                                <td align="left" colspan="2">
                                    <table width="100%" cellpadding="0" cellspacing="2" border="0" bgcolor="#ffffff">
                                        <tr class="tbl_row" valign="baseline">
                                            <td style="width: 15%" align="right">
                                                卡片圖檔：
                                            </td>
                                            <td style="width: 70%">
                                                <asp:FileUpload ID="fludFileUpload" runat="server" Width="200px"/>
                                                <asp:Button ID="btnUpload" runat="server" CausesValidation="False" CssClass="btn"
                                                    OnClick="btnUpload_Click" Text="上傳" />
                                            </td>
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
                        </table>
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr valign="baseline">
                    <td align="left">
                        <asp:CheckBox ID="chkDelete" runat="server" Text="删除" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSubmit2" runat="server" CssClass="btn" OnClick="btnSubmit_Click"
                            OnClientClick="return delConfirm();" Text="確定" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" CssClass="btn"
                            OnClick="btnCancel_Click" Text="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
