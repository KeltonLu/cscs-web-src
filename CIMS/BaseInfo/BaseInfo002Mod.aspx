<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo002Mod.aspx.cs" Inherits="BaseInfo_BaseInfo002Mod" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>合約作業修改/刪除</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr style="height: 20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                            <td>
                                <font class="PageTitle">合約作業修改/刪除</font>
                            </td>
                            <td align="right">
                                <asp:Button CssClass="btn" ID="btnSubmitUp" runat="server" OnClientClick="return ConfirmDel();"
                                    Text="確定" OnClick="btnSubmitUp_Click" />&nbsp;&nbsp;
                                <input id="Button1" class="btn" onclick="returnform('BaseInfo002.aspx')" type="button"
                                    value="取消" />
                            </td>
                        </table>
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table cellpadding="0" width="100%" cellspacing="2" border="0">
                            <tr class="tbl_row">
                                <td>
                                    <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                        <tr>
                                            <td width="70%">
                                                <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                                    <tr valign="baseline">
                                                        <td align="right" style="width: 20%">
                                                            <span class="style1">*</span>合約編號：
                                                        </td>
                                                        <td style="width: 25%" align="left">
                                                            <asp:TextBox onkeyup="LimitLengthCheck(this,20)" ID="txtAgreement_Code" runat="server"
                                                                MaxLength="20" Enabled="False"></asp:TextBox>
                                                        </td>
                                                        <td style="width: 15%" align="right">
                                                            <span class="style1">*</span>合約名稱：
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox onkeyup="LimitLengthCheck(this,30)" ID="txtAgreement_Name" runat="server"
                                                                MaxLength="30"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtAgreement_Name"
                                                                ErrorMessage="合約名稱不能為空">*</asp:RequiredFieldValidator></td>
                                                    </tr>
                                                    <tr valign="baseline">
                                                        <td align="right">
                                                            <span class="style1">*</span>上傳合約影像檔：
                                                        </td>
                                                        <td colspan="3" align="left">
                                                            &nbsp;<asp:FileUpload ID="fludFileUpload" runat="server" Width="200px" />
                                                            <asp:Button ID="btnUpload" runat="server" CssClass="btn" Text="上傳" OnClick="btnUpload_Click"
                                                                CausesValidation="False" />
                                                            <asp:HyperLink ID="HyperLink" runat="server" Target="_blank"></asp:HyperLink></td>
                                                    </tr>
                                                    <tr valign="baseline">
                                                        <td align="right">
                                                            <span class="style1">*</span>空白卡廠：
                                                        </td>
                                                        <td colspan="3" align="left">
                                                            <asp:DropDownList ID="dropFactory_RID" DataTextField="Factory_ShortName_cn" DataValueField="RID"
                                                                runat="server">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr valign="baseline">
                                                        <td align="right">
                                                            卡數：
                                                        </td>
                                                        <td colspan="3" align="left">
                                                        <%--Dana 20161021 最大長度由9改為11 --%>
                                                            <asp:TextBox ID="txtCard_Number" onkeyup="CheckNum('txtCard_Number',9)" onfocus="DelDouhao(this)"
                                                                Style="ime-mode: disabled; text-align: right" onblur="value=GetValue(this.value)"
                                                                runat="server" MaxLength="11"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr valign="baseline">
                                                        <td align="right">
                                                            <span class="style1">*</span>合約時間：
                                                        </td>
                                                        <td colspan="3" align="left">
                                                            <asp:TextBox ID="txtBegin_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                                                MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtBegin_Time')})"
                                                                    src="../images/calendar.gif" align="absmiddle" id="IMG1" runat="server">
                                                            ~<asp:TextBox ID="txtEnd_Time" onfocus="WdatePicker()" runat="server" Width="80px"
                                                                MaxLength="10"></asp:TextBox>&nbsp;<img onclick="WdatePicker({el:$dp.$('txtEnd_Time')})"
                                                                    src="../images/calendar.gif" align="absmiddle" id="IMG2" runat="server">
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="合約時間（起）不能為空"
                                                                ControlToValidate="txtBegin_Time">*</asp:RequiredFieldValidator>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="合約時間（迄）不能為空"
                                                                ControlToValidate="txtEnd_Time">*</asp:RequiredFieldValidator>
                                                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtBegin_Time"
                                                                ControlToValidate="txtEnd_Time" ErrorMessage="有效期間迄必須大於起" Operator="GreaterThanEqual"
                                                                Type="Date">*</asp:CompareValidator></td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td>
                                                <asp:Image ID="imgIMG_File_URL" ImageUrl="../images/NoPic.jpg" runat="server" Height="100px"
                                                    Width="100px" Visible="False" />
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
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                    <tr>
                                        <td>
                                            <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                                
                                                <tr class="tbl_row">
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbCardPrice" runat="server" OnOnSetDataSource="gvpbCardPrice_OnSetDataSource"
                                                            AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                            PreviousPageText="<" OnRowDataBound="gvpbCardPrice_RowDataBound"
                                                            ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                            EditPageUrl="#">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="卡種組合">
                                                                    <itemstyle width="20%" />
                                                                    <itemtemplate>
<asp:LinkButton id="lbtnCardName" runat="server" CausesValidation="false" OnCommand="lbtnCardName_Click" ></asp:LinkButton>
</itemtemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Type" HeaderText="計價方式"></asp:BoundField>
                                                                <asp:TemplateField HeaderText="刪除">
                                                                    <itemstyle horizontalalign="Center" width="100px" />
                                                                    <itemtemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDeleteCard" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDeleteCard_Command"></asp:ImageButton>
                                                        
</itemtemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <table cellspacing="0" cellpadding="0" border="1" style="width: 100%;border-collapse: collapse;">
                                                                    <tr style="color: Black; background-color: #B9BDAA; font-size: Small;">
                                                                        <th scope="col" style="width: 20%;">
                                                                            卡種組合</th>
                                                                        <th scope="col">
                                                                            計價方式</th>
                                                                        <th scope="col" width="100px">刪除
                                                                        </th>
                                                                    </tr>
                                                                </table>
                                                            </EmptyDataTemplate>
                                                        </cc1:GridViewPageingByDB>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Button ID="btnCardPriceAdd" CssClass="btn" runat="server" Text="新增卡種組合" OnClick="btnCardPriceAdd_Click"
                                                            CausesValidation="False" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table cellpadding="0" width="100%" cellspacing="0" border="0">
                                                
                                                <tr class="tbl_row">
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbBak" runat="server" OnOnSetDataSource="gvpbBak_OnSetDataSource"
                                                            AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                            PreviousPageText="<" OnRowDataBound="gvpbBak_RowDataBound"
                                                            ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                            EditPageUrl="#">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                                PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="備援合約編號">
                                                                    <itemtemplate>
<asp:LinkButton id="lbAgreement_Code" runat="server" CausesValidation="false" OnCommand="lbAgreement_Code_Click" ></asp:LinkButton> 
</itemtemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Agreement_Name" HeaderText="合約名稱">
                                                                    <itemstyle width="20%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Factory_Name" HeaderText="廠商名稱">
                                                                    <itemstyle width="20%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="刪除">
                                                                    <itemstyle horizontalalign="Center" width="100px" />
                                                                    <itemtemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDeleteBak" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDeleteBak_Command"></asp:ImageButton>
                                                        
</itemtemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <table cellspacing="0" cellpadding="0" border="1" style="width: 100%;border-collapse: collapse;">
                                                                    <tr style="color: Black; background-color: #B9BDAA; font-size: Small;">
                                                                        <th scope="col">
                                                                            備援合約編號</th>
                                                                        <th scope="col" width="20%">
                                                                            合約名稱</th>
                                                                        <th scope="col" width="20%">
                                                                            廠商名稱
                                                                        </th>
                                                                        <th scope="col" width="100px">刪除
                                                                        </th>
                                                                    </tr>
                                                                </table>
                                                            </EmptyDataTemplate>
                                                        </cc1:GridViewPageingByDB>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Button ID="btnBakAdd" CssClass="btn" runat="server" Text="新增備援合約" OnClick="btnBakAdd_Click"
                                                            CausesValidation="False" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="right">
                                    異動原因：</td>
                                <td align="left">
                                    <asp:TextBox ID="txtReason" runat="server" Height="50px" TextMode="MultiLine" Width="400px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td align="left" id='tdDel' style="display :none">
                                    <asp:CheckBox ID="chkDel" runat="server" Text="刪除" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnSubmitDn" OnClientClick="return ConfirmDel();"
                            runat="server" Text="確定" OnClick="btnSubmitUp_Click" />&nbsp;&nbsp;
                        <input id="btnCancel" class="btn" onclick="returnform('BaseInfo002.aspx')" type="button"
                            value="取消" />
                    </td>
                </tr>
            </table>

            <script>
            function EnableDel(flag)
            {
          
                if(flag)
                {
                    document.getElementById("tdDel").style.display ="table-row";
                    document.getElementById('chkDel').checked=false;
                }
                else
                {
                    document.getElementById("tdDel").style.display ="none";
                    document.getElementById('chkDel').checked=false;
                }
            }
            
		    function ConfirmDel()
            {
                if(CheckClientValidate())
                {
                    if(document.getElementById('chkDel').checked)  
                    {
                        return confirm("是否刪除此筆訊息");
                    }
                }
                else
                {
                    return false;
                }
               
                return true;
            }
            
            function openwindow(value)
            {   
  　            window.open("BaseInfo002Image.asp?imgdir="+value,"newwindow","height=277,width=600,top=10,left=10,toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,status=no");   
            }  
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
