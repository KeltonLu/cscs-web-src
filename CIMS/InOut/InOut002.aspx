<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="InOut002.aspx.cs"
    Inherits="InOut_InOut002" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>廠商庫存資料匯入</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">	
    <!--
            function delConfirm()
            {
                if (true==confirm('轉換畫面，系統將不保存當前畫面的所有修改，是否繼續？'))
                   {
                        __doPostBack('btnIsCheckTure',''); 
                   }
                else
                   {
                        __doPostBack('btnIsCheck',''); 
                   }
            }  
            
            
            function displayOK()
              {  
                var obj   =   document.getElementById('addModify');
                if(obj != null)
                {
                    obj.style.display = ''; 
                }
              }  
              
              
              function displayNO()
              {  
                var obj   =   document.getElementById('addModify');
                if(obj != null)
                {
                    obj.style.display = 'none'; 
                }
              }  
              
//               function displayYiDongNo()
//               {  
//                var obj   =   document.getElementById('Button1');
//                if(obj != null)
//                 {
//                    //obj.style.display = 'none'; 
//                     obj.disabled="disabled"
//                 }
//               } 
    //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table width="100%" cellspacing="0">
                <tr style="height: 10px">
                    <td style="width: 157px">
                    </td>
                    <td style="width: 805px">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td width="20%">
                        <font class="PageTitle">廠商庫存資料匯入</font></td>
                    <td style="width: 805px">
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td colspan="2" style="height: 22px">
                        <asp:RadioButton ID="RadioButton1" runat="server" Text="匯入廠商異動 " AutoPostBack="True"
                            Checked="True" GroupName="a" OnCheckedChanged="RadioButton1_CheckedChanged" />
                        <asp:RadioButton ID="RadioButton2" runat="server" Text="刪除已匯入資料" AutoPostBack="True"
                            GroupName="a" OnCheckedChanged="RadioButton2_CheckedChanged" />
                        <asp:RadioButton ID="RadioButton3" runat="server" AutoPostBack="True" GroupName="a"
                            OnCheckedChanged="RadioButton3_CheckedChanged" Text="人工輸入" /></td>
                </tr>
                <tr class="tbl_row" style="font-size: 10pt">
                    <td width="100%" colspan="2">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td align="right" width="15%">
                                    <span style="color: #ff0000">*</span>匯入日期：</td>
                                <td style="width: 779px">
                                    <asp:TextBox ID="txtBegin_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                                        Width="100px"></asp:TextBox>
                                    <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date')})" src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBegin_Date"
                                        ErrorMessage="匯入日期不能为空">*</asp:RequiredFieldValidator></td>
                            </tr>
                            <tr class="tbl_row" style="font-size: 10pt">
                                <td align="right">
                                    <span style="color: #ff0000">*</span>PERSO廠：</td>
                                <td style="width: 779px">
                                    <asp:DropDownList ID="dropFactory" runat="server" Width="88px">
                                    </asp:DropDownList></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="Panel1" runat="server" Width="100%">
                <table id="tb1" cellspacing="0" width="100%">
                    <tr class="tbl_row">
                        <td align="right" width="15%">
                            <span class="style1">*</span>檔案匯入：</td>
                        <td style="width: 715px">
                            <asp:FileUpload ID="FileUpd" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 146px; height: 26px;">
                        </td>
                        <td align="right" >
                            &nbsp;<asp:Button ID="btnIMPORT" runat="server" class="btn" Text="匯入" OnClick="btnIMPORT_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height: 138px">
                            &nbsp;<cc1:GridViewPageingByDB ID="gvpbFactory_Change_Import" runat="server"
                                AllowSorting="True" AutoGenerateColumns="False" ColumnsHeaderText="" CssClass="GridView"
                                EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                                SortDescImageUrl="~/images/desc.gif" OnOnSetDataSource="gvpbFactory_Change_Import_OnSetDataSource" OnRowDataBound="gvpbFactory_Change_Import_RowDataBound">
                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                <RowStyle CssClass="GridViewRow" HorizontalAlign="Center" />
                                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                    PreviousPageText="&lt;" Visible="False" />
                                <Columns>
                                    <asp:BoundField DataField="Space_Short_Name" HeaderText="卡種">
                                        <itemstyle horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Status_Name" HeaderText="狀況">
                                        <itemstyle horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Number" HeaderText="數量" >
                                        <itemstyle horizontalalign="Right" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:GridViewPageingByDB>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="Panel2" runat="server" Width="100%" Visible="False">
                <table width="100%" id="tb3" cellpadding="0" cellspacing="0" border="0">
                    <tr class="tbl_row">
                        <td align="right" width="80%">
                            <asp:Button ID="btnDisFactoryChange" runat="server" OnClick="btnDisFactoryChange_Click"
                                Text="Button" style="display: none;" /></td>
                        <td align="right" style="height: 24px">
                            <asp:Button ID="btnSearch" runat="server" class="btn" Text="查詢" OnClick="btnSearch_Click" />
                            <input id="Button1" class="btn" onclick="doPopSpecailDetail();" type="button" value="新增異動" runat="server" />&nbsp;
                        </td>
                    </tr>
                    <tr style="height: 20px">
                        <td colspan="2" style="height: 20px">
                            &nbsp;
                        </td>
                    </tr>
                    <tr style="height: 20px">
                        <td colspan="2">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                        <tr id="trGrid" style="display: table-row">
                                            <td>
                                                <cc1:GridViewPageingByDB ID="gvSearch_Factory_Change_Import" runat="server" AutoGenerateColumns="False"
                                                    ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                                                    EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                                    OnOnSetDataSource="gvSearch_Factory_Change_Import_OnSetDataSource" PreviousPageText="<"
                                                    OnRowDataBound="gvSearch_Factory_Change_Import_RowDataBound">
                                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                                    <RowStyle CssClass="GridViewRow" />
                                                    <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                        PreviousPageText="&lt;" Visible="False" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="卡種">
                                                            <itemstyle horizontalalign="Center" width="45%"></itemstyle>
                                                            <itemtemplate>
                                                            <asp:HyperLink id="hlModify" runat="server"></asp:HyperLink>
                                                        
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="狀況" DataField="Status_Name" />
                                                        <asp:BoundField DataField="Number" HeaderText="數量" SortExpression="Number">
                                                            <itemstyle horizontalalign="Right" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="刪除">
                                                            <itemstyle horizontalalign="Center" width="5%" />
                                                            <itemtemplate>
                                                            <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDeleteItem" Height="18px" ImageUrl="~/images/trash_0021.gif" OnCommand="btnDeleteItem_Command"></asp:ImageButton>
                                                            
</itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Perso_Factory_RID" HeaderText="Perso_Factory_RID" />
                                                        <asp:BoundField DataField="Date_Time" HeaderText="Date_Time" />
                                                        <asp:BoundField HeaderText="Param_Code" DataField="Param_Code" />
                                                        <asp:BoundField DataField="RID" HeaderText="RID" />
                                                    </Columns>
                                                    <SelectedRowStyle HorizontalAlign="Center" />
                                                </cc1:GridViewPageingByDB>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 15px">
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="1" style="width: 717px">
                        </td>
                        <td colspan="2">
                            <table width="100%" cellpadding="0" cellspacing="2" border="0" id="result2">
                                <tr>
                                    <td id="addModify" style="display: none">
                                        &nbsp;
                                        <asp:Button ID="btnAddFactory_Chage_Import" runat="server" CausesValidation="False"
                                            OnClick="btnAddFactory_Chage_Import_Click" Text="Button" style="display: none;" />
                                        <asp:Button ID="btnIsCheck" runat="server" CausesValidation="False" OnClick="btnIsCheck_Click"
                                            Text="Button" style="display: none;"/>
                                        <asp:Button ID="btnIsCheckTure" runat="server" CausesValidation="False" Text="Button"
                                            style="display: none;" OnClick="btnIsCheckTure_Click" /></td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        &nbsp;
                                        <asp:Button ID="btnReset" runat="server" class="btn" Text="取消" OnClick="btnReset_Click"
                                            Visible="False" />&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table id="tb2" cellspacing="0" width="100%" runat="server">
                <tr>
                    <td>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnDelete" runat="server" class="btn" Text="刪除已匯入資料" OnClick="btnDelete_Click" />
                    </td>
                </tr>
            </table>

            <script type="text/javascript" language="javascript">
             
             function doPopSpecailDetail()
             {
                var aa=window.showModalDialog('InOut002Detail.aspx?type=add&Date='+document.getElementById("txtBegin_Date").value+'&Factory='+document.getElementById("dropFactory").value+'','','dialogHeight:450px;dialogWidth:600px;');
                if(aa!=undefined){__doPostBack('btnDisFactoryChange','');} 
             }
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
