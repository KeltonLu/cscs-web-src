<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo003.aspx.cs" Inherits="BaseInfo_BaseInfo003" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>廠商資料設定</title>
    <link href="../css/Global.css"  rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>
    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
        <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1" border="0">
				<tr style="height:20px">
					<td>
						&nbsp;
					</td>
				</tr>
				<tr valign="baseline">
					<td>
						<font class="PageTitle">廠商資料設定</font>
					</td>
				</tr>
				<tr>
					<td>
						<table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
							<tr valign="baseline">
								<td width="10%" align="right">
									廠商簡稱：
								</td>
								<td width="30%">
									<asp:TextBox id="txtFactory_ShortName_CN" runat="server" size="15" maxlength="20"/>
								</td>
								<td width="10%" align="right">
									廠商類別：
								</td>
								<td width="80%">
									<asp:CheckBox id="chkIs_Blank" runat="server" Text="空白卡廠"/>
                                    <asp:CheckBox id="chkIs_Perso" runat="server" Text="Perso廠"/>
								</td>
							</tr>												
							<tr valign="baseline">
								<td colspan="6" align="right">
									<asp:Button Text="查詢" id="btnQuery"  class="btn" runat="server" OnClick="btnQuery_Click"/>&nbsp;&nbsp;
									<asp:Button Text="新增" id="btnAdd"  class="btn" runat="server" OnClick="btnAdd_Click"/>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr style="height:20px">
					<td>
						&nbsp;
					</td>
				</tr>
				<tr valign="top">
					<td>			
					    <asp:UpdatePanel id="UpdatePanel1" runat="server">
                        <contenttemplate>			
						<table id="queryResult" width="100%">
						    <cc1:GridViewPageingByDB ID="gvpbFACTORY" runat="server" AllowPaging="True"
                                    DataKeyNames="RID" OnOnSetDataSource="gvpbFACTORY_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbFACTORY_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                        <asp:HyperLinkField DataTextField="Factory_ID" HeaderText="廠商代號" SortExpression="FACTORY_ID" DataNavigateUrlFields="RID" DataNavigateUrlFormatString="BaseInfo003Edit.aspx?ActionType=Edit&RID={0}">
                                            <itemstyle horizontalalign="Left"　width="15%" />
                                        </asp:HyperLinkField>
                                        <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="廠商簡稱" SortExpression="Factory_ShortName_CN" />
                                        <asp:TemplateField HeaderText="廠商類別">
                                            <itemstyle width="15%" />
                                            <itemtemplate>
                                                <asp:Label id="lblFACTORY_TYPE" runat="server"/>                                            
                                            </itemtemplate>
                                        </asp:TemplateField>                
                                        <asp:BoundField DataField="CoLinkMan_Name" HeaderText="公司聯絡人" SortExpression="CoLinkMan_Name" />
                                        <asp:BoundField DataField="CoLinkMan_Mobil" HeaderText="聯絡人手機" SortExpression="CoLinkMan_Mobil" />
                                    </Columns>
                            </cc1:GridViewPageingByDB>													
						</table>
						</contenttemplate>
						</asp:UpdatePanel>
					</td>
				</tr>
			</table>
    </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
