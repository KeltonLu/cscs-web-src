<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository003Add.aspx.cs" Inherits="Depository_Depository003Add" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>入庫人工新增</title>
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
                     <font class="PageTitle">入庫作業-人工新增</font>
                </td>
            </tr>
            <tr>
                <td>
                     <asp:UpdatePanel id="UpdatePanel2" runat="server">
                    <contenttemplate>
                    <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                        <tr>
                            <td>
                                <cc1:GridViewPageingByDB ID="gvpbImportStock" runat="server" OnOnSetDataSource="gvpbImportStock_OnSetDataSource" AutoGenerateColumns="False" FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" OnRowDataBound="gvpbImportStock_RowDataBound" ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                    <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                    <AlternatingRowStyle CssClass="GridViewAlter" />
                                    <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;" PreviousPageText="&lt;" />
                                    <RowStyle CssClass="GridViewRow" />
                                    <Columns>
                                        <asp:BoundField DataField="Card_Purpose_NAME" HeaderText="用途" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Card_Group_NAME" HeaderText="群組" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="卡種" >
                                            <itemstyle />
                                                <itemtemplate>
                                                    <asp:HyperLink id="hlCardType" runat="server"></asp:HyperLink>
                                                </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Stock_Number" HeaderText="進貨量" >
                                            <itemstyle width="6%" HorizontalAlign="Right"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Blemish_Number" HeaderText="瑕疵量">
                                            <itemstyle width="6%" HorizontalAlign="right"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Sample_Number" HeaderText="抽樣卡數">
                                            <itemstyle width="6%" HorizontalAlign="right"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Income_Number" HeaderText="入庫量" >
                                            <itemstyle width="6%" HorizontalAlign="right"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Income_Date" HeaderText="入庫日期" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Serial_Number" HeaderText="卡片批號" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Perso_Factory_NAME" HeaderText="Perso廠" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Blank_Factory_NAME" HeaderText="空白卡廠" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Wafer_NAME" HeaderText="晶片名稱" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Check_Type" HeaderText="驗貨方式" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SendCheck_Status" HeaderText="批號狀態" >
                                            <itemstyle width="6%" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="刪除" >
                                            <itemstyle horizontalalign="Center" width="100px" />
                                            <itemtemplate>
                                                <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDelete_Command"></asp:ImageButton>
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
                <td align="left">
                     <input id="Button1" onclick="var aa=window.showModalDialog('Depository003AddDetail.aspx?ActionType=Add','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}" type="button" value="新增入庫明細" class="btn" />
                </td>
            </tr>
             <tr>
                <td align="right">
                    <asp:Button ID="btnBind" runat="server" OnClick="btnBind_Click" Visible="False" />
                    <asp:Button CssClass="btn" ID="btnSubmit" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                    <input id="btnCancel" class="btn" onclick="returnform('Depository003.aspx')" type="button"
                        value="取消" />
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
