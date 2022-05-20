<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository003CaseClose.aspx.cs" Inherits="Depository_Depository003CaseClose" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>入庫單結案</title>
     <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                        <tr>
                            <td>
                                 <font class="PageTitle">入庫資料結案</font>
                            </td>
                            <td align="right" style="height: 24px">
                                <asp:Button CssClass="btn" ID="btnSubmitUp" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                                <input id="Button2" class="btn" onclick="returnform('Depository003.aspx?Con=1')" type="button"
                                    value="取消" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
             <tr>
			    <td>
			        <asp:UpdatePanel id="UpdatePanel2" runat="server">
                    <contenttemplate>
			        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
			            <tr>
			                <td>
			                     <cc1:GridViewPageingByDB ID="gvpbImportStock" runat="server" OnOnSetDataSource="gvpbImportStock_OnSetDataSource" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbImportStock_RowDataBound" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" DataKeyNames="OrderForm_Detail_RID">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                        <asp:BoundField DataField="OrderForm_Detail_RID" HeaderText="訂單流水編號" >
                                        </asp:BoundField>
                                        <asp:BoundField DataField="name" HeaderText="版面簡稱" >
                                        </asp:BoundField>
                                        <asp:BoundField DataField="number" HeaderText="原訂購卡數" DataFormatString="{0:N0}" HtmlEncode="False">
                                            <itemstyle HorizontalAlign="Right"/>
                                        </asp:BoundField>
                                         <asp:TemplateField HeaderText="累計入庫卡數">
                                            <itemstyle HorizontalAlign="Right"/>
                                            <itemtemplate>
                                                <asp:Label id="lbNumber" runat="server"></asp:Label> 
                                            
</itemtemplate>
                                         </asp:TemplateField>
                                        <asp:TemplateField HeaderText="是否結案">
                                            <itemtemplate>
<asp:CheckBox id="cbCase_Status" runat="server" __designer:wfdid="w2" Text="是"></asp:CheckBox> 
</itemtemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </cc1:GridViewPageingByDB>
			                </td>
			            </tr>
			        </table>
			        </contenttemplate>
			        </asp:UpdatePanel>
			    </td>
			</tr>
			<tr>
                <td align="right" style="height: 24px">
                    <asp:Button CssClass="btn" ID="btnSubmitDn" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                    <input id="btnCancel" class="btn" onclick="returnform('Depository003.aspx?Con=1')" type="button"
                        value="取消" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
