<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository005Imp.aspx.cs" Inherits="Depository_Depository005Imp" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>再入庫作業匯入</title>
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
                     <font class="PageTitle">再入庫作業匯入</font>
                </td>
            </tr>
            <tr>
			    <td>
			        <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
			            <tr>
			                <td>
			                    <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
			                        <tr class="tbl_row">
                                        <td style="width: 141px; text-align: right">
                                          <span class="style1" style="color:Red">*</span>匯入資料：</td>
			                            <td style="width: 825px">
                                            <asp:FileUpload ID="FileUpd" runat="server" />
                                            <asp:Button CssClass="btn" ID="btnImport" runat="server" Text="開始匯入" OnClick="btnImport_Click" /></td>
			                        </tr>
			                        <tr>
                                        <td colspan="2">
			                                <asp:UpdatePanel id="UpdatePanel2" runat="server">
                                            <contenttemplate>
                                            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                                                <tr>
                                                    <td>
                                                        <cc1:GridViewPageingByDB ID="gvpbImportStock" runat="server" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" DataKeyNames="Stock_RID" OnOnSetDataSource="gvpbImportStock_OnSetDataSource" OnRowDataBound="gvpbImportStock_RowDataBound">
                                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                                            <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                                            <RowStyle CssClass="GridViewRow" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="入庫流水編號 " >
                                                                    <itemstyle width="6%" />
                                                                        <itemtemplate>
                                                                            <asp:HyperLink id="hlDetailRID" runat="server"></asp:HyperLink>
                                                                        
</itemtemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Space_Short_Name" HeaderText="版面簡稱" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField HeaderText="原退貨量" DataFormatString="{0:N2}" HtmlEncode="False" DataField="Cancel_Number">
                                                                    <itemstyle width="6%" HorizontalAlign="Right"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField HeaderText="再進貨量" DataField="Restock_Number">
                                                                    <itemstyle width="6%" HorizontalAlign="Right"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Blemish_Number" HeaderText="瑕疵量" DataFormatString="{0:N2}" HtmlEncode="False">
                                                                    <itemstyle width="6%" HorizontalAlign="Right"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Sample_Number" HeaderText="抽樣卡數" DataFormatString="{0:N2}" HtmlEncode="False">
                                                                    <itemstyle width="6%" HorizontalAlign="Right"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Reincome_Number" HeaderText="再入庫量" DataFormatString="{0:N2}" HtmlEncode="False">
                                                                    <itemstyle width="6%" HorizontalAlign="Right"/>
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Reincome_Date" HeaderText="再入庫日期" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Serial_Number" HeaderText="卡片批號" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Perso_Factory_Name" HeaderText="Perso廠商" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Blank_Factory_Name" HeaderText="空白卡廠" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Wafer_Name" HeaderText="晶片名稱" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Check_Type" HeaderText="驗貨方式" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Comment" HeaderText="備註" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="SendCheck_Status" HeaderText="批號狀態(送審狀態)" >
                                                                    <itemstyle width="6%" />
                                                                </asp:BoundField>                                                                
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
                                        <td align="right" style="width: 141px">
                                        </td>
			                            <td align="right" style="width: 825px">
                                            <asp:Button ID="btnBind" runat="server"  Visible="False" OnClick="btnBind_Click" />
			                                <asp:Button CssClass="btn" ID="btnSubmit" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                                            <input id="btnCancel" class="btn" onclick="returnform('Depository005.aspx')" type="button"
                                                value="取消" />
			                            </td>
			                        </tr>
			                    </table>
			                </td>
			            </tr>
			        </table>
			    </td>
			</tr>
        </table>
        <script>
            function ImtBind()
            {
                __doPostBack('btnBind',''); 
            }
        </script>
    </div>
    </form>
</body>
</html>
