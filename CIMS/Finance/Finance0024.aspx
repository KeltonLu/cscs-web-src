<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Finance0024.aspx.cs"
    Inherits="Finance_Finance0024" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>代製費用</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">
    function doRadio(rad)
    {
        if (rad == document.getElementById("adrtIn"))
        {
            document.getElementById("trperso").style.display="table-row";               
            document.getElementById("trfileupd").style.display="table-row";
            document.getElementById("trym").style.display="none"; 
            document.getElementById("trcardgroup").style.display="none";
            document.getElementById("trbtnin").style.display="table-row"; 
            document.getElementById("trbtnsave").style.display="table-row";                                   
            document.getElementById("trbtndel").style.display="none"; 
            document.getElementById("trsearchld").style.display="none";                                   
            document.getElementById("trsearchfz").style.display="none";   
            document.getElementById("DataInGrid").style.display="table-row";     
            document.getElementById("SearchProjectGrid").style.display="none";  
            document.getElementById("FinanceGrid").style.display="none";                       
        }
    
        if (rad == document.getElementById("adrtDel"))
        {
            document.getElementById("trperso").style.display="table-row";               
            document.getElementById("trfileupd").style.display="none";
            document.getElementById("trym").style.display="table-row"; 
            document.getElementById("trcardgroup").style.display="none";
            document.getElementById("trbtnin").style.display="none"; 
            document.getElementById("trbtnsave").style.display="none";                                   
            document.getElementById("trbtndel").style.display="table-row"; 
            document.getElementById("trsearchld").style.display="none";                                   
            document.getElementById("trsearchfz").style.display="none";   
            document.getElementById("DataInGrid").style.display="none";     
            document.getElementById("SearchProjectGrid").style.display="none";  
            document.getElementById("FinanceGrid").style.display="none"; 
        }
        if (rad == document.getElementById("adrtInHand"))
        {
            document.getElementById("trperso").style.display="table-row";               
            document.getElementById("trfileupd").style.display="none";
            document.getElementById("trym").style.display="table-row"; 
            document.getElementById("trcardgroup").style.display="table-row";
            document.getElementById("trbtnin").style.display="none"; 
            document.getElementById("trbtnsave").style.display="none";                                   
            document.getElementById("trbtndel").style.display="none"; 
            document.getElementById("trsearchld").style.display="table-row";                                   
            document.getElementById("trsearchfz").style.display="none";  
            document.getElementById("DataInGrid").style.display="none";     
            document.getElementById("SearchProjectGrid").style.display="table-row";  
            document.getElementById("FinanceGrid").style.display="none";                           
        }
        if (rad == document.getElementById("adrtInMove"))
        {
            document.getElementById("trperso").style.display="table-row";               
            document.getElementById("trfileupd").style.display="none";
            document.getElementById("trym").style.display="table-row"; 
            document.getElementById("trcardgroup").style.display="table-row";
            document.getElementById("trbtnin").style.display="none"; 
            document.getElementById("trbtnsave").style.display="none";                                   
            document.getElementById("trbtndel").style.display="none"; 
            document.getElementById("trsearchld").style.display="none";                                   
            document.getElementById("trsearchfz").style.display="table-row";    
            document.getElementById("DataInGrid").style.display="none";     
            document.getElementById("SearchProjectGrid").style.display="none";  
            document.getElementById("FinanceGrid").style.display="table-row";                            
        }
    } 
    
    function showbtn()
    {
        document.getElementById("trbtnsave").style.display="table-row";    
    }
    
    function onLoaddoRadio()
    {
        if (document.getElementById("adrtIn").checked)
        {
            doRadio(document.getElementById("adrtIn"));
        }
        else if (document.getElementById("adrtDel").checked)
        {
            doRadio(document.getElementById("adrtDel"));
        }
        else if (document.getElementById("adrtInHand").checked)
        {
            doRadio(document.getElementById("adrtInHand"));
        }
        else if (document.getElementById("adrtInMove").checked)
        {
            doRadio(document.getElementById("adrtInMove"));
        }
    }     
    function ImtBind()
    {
        __doPostBack('AddP',''); 
    }  
    function ImtBind2()
    {
        __doPostBack('AddF',''); 
    }  
</script>

<body >
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table width="100%" cellspacing="0">
            <tr style="height: 20px">
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <font class="PageTitle">代製費用</font></td>
            </tr>
            <tr class="tbl_row">
                <td>
                    <input name="action" id="adrtIn" runat="server" type="radio" checked="true" onclick="doRadio(this);" />匯入特殊代製項目
                    <input name="action" id="adrtDel" runat="server" type="radio" onclick="doRadio(this);" />刪除特殊代製項目
                    <input name="action" id="adrtInHand" runat="server" type="radio" onclick="doRadio(this);" />人工輸入例外代製項目
                    <input name="action" id="adrtInMove" runat="server" type="radio" onclick="doRadio(this);" />代製費用帳務異動
                </td>
            </tr>
        </table>
        <table width="100%" id="tb1" cellpadding="0" cellspacing="0" border="0">
            <tr id="trym" class="tbl_row" valign="baseline" style="display: none">
                <td align="right" width="15%" style="height: 24px">
                    <span class="style1">*</span>年：
                </td>
                <td style="height: 24px" colspan="3">
                    <asp:DropDownList ID="dropYear" runat="server">
                        <asp:ListItem>1999</asp:ListItem>
                        <asp:ListItem>2000</asp:ListItem>
                        <asp:ListItem>2001</asp:ListItem>
                        <asp:ListItem>2002</asp:ListItem>
                        <asp:ListItem>2003</asp:ListItem>
                        <asp:ListItem>2004</asp:ListItem>
                        <asp:ListItem>2005</asp:ListItem>
                        <asp:ListItem>2006</asp:ListItem>
                        <asp:ListItem>2007</asp:ListItem>
                        <asp:ListItem>2008</asp:ListItem>
                        <asp:ListItem>2009</asp:ListItem>
                        <asp:ListItem>2010</asp:ListItem>
                        <asp:ListItem>2011</asp:ListItem>
                        <asp:ListItem>2012</asp:ListItem>
                        <asp:ListItem>2013</asp:ListItem>
                        <asp:ListItem>2014</asp:ListItem>
                        <asp:ListItem>2015</asp:ListItem>
                        <asp:ListItem>2016</asp:ListItem>
                        <asp:ListItem>2017</asp:ListItem>
                        <asp:ListItem>2018</asp:ListItem>
                        <asp:ListItem>2019</asp:ListItem>
                        <asp:ListItem>2020</asp:ListItem>
                        <asp:ListItem>2021</asp:ListItem>                      
                        <asp:ListItem>2022</asp:ListItem>
                        <asp:ListItem>2023</asp:ListItem>
                        <asp:ListItem>2024</asp:ListItem>
                        <asp:ListItem>2025</asp:ListItem>
                        <asp:ListItem>2026</asp:ListItem>
                        <asp:ListItem>2027</asp:ListItem>
                        <asp:ListItem>2028</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                    <span class="style1">*</span>月：
                    <asp:DropDownList ID="dropMonth" runat="server">
                        <asp:ListItem>1</asp:ListItem>
                        <asp:ListItem>2</asp:ListItem>
                        <asp:ListItem>3</asp:ListItem>
                        <asp:ListItem>4</asp:ListItem>
                        <asp:ListItem>5</asp:ListItem>
                        <asp:ListItem>6</asp:ListItem>
                        <asp:ListItem>7</asp:ListItem>
                        <asp:ListItem>8</asp:ListItem>
                        <asp:ListItem>9</asp:ListItem>
                        <asp:ListItem>10</asp:ListItem>
                        <asp:ListItem>11</asp:ListItem>
                        <asp:ListItem>12</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trperso" class="tbl_row" valign="baseline" style="display: table-row">
                <td align="right" width="15%">
                    <span class="style1">*</span>Perso廠：</td>
                <td colspan="3">
                    <asp:DropDownList ID="dropFactory" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trfileupd" class="tbl_row" valign="baseline" style="display: table-row">
                <td align="right">
                    <span class="style1">*</span>匯入：</td>
                <td colspan="3">
                    <asp:FileUpload ID="FileUpd" runat="server" />
                </td>
            </tr>
            <tr id="trcardgroup" class="tbl_row" valign="baseline" style="display: none">
                <td align="right" width="13%">
                    <span class="style1">*</span>用途：</td>
                <td style="width: 18%">
                    <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
                <td align="left">
                    群組：
                    <asp:DropDownList ID="dropCard_Group_RID" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trbtnin" style="display: table-row">
                <td align="right" colspan="4">
                    <asp:Button ID="btnDataIn" CssClass="btn" runat="server" Text="開始匯入" OnClick="btnDataIn_Click" />
                </td>
            </tr>
            <tr valign="baseline" style="display: none" id="DataInGrid">
                <td colspan="3">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr id="trGrid" style="display: table-row">
                                    <td>
                                        <cc1:GridViewPageingByDB ID="gvbFinanceDataIn" runat="server" AutoGenerateColumns="False"
                                            ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                                            EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                            OnOnSetDataSource="gvbFinanceDataIn_OnSetDataSource" PreviousPageText="<" OnRowDataBound="gvbFinanceDataIn_RowDataBound">
                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                            <RowStyle CssClass="GridViewRow" />
                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                PreviousPageText="&lt;" Visible="False" />
                                            <Columns>
                                                <asp:BoundField HeaderText="作業日期" DataField="Project_Date" />                                                
                                                <asp:TemplateField HeaderText="特殊代製項目">
                                                    <itemstyle horizontalalign="Center"></itemstyle>
                                                    <itemtemplate>                                                        
                                                        <asp:Label id="lblProject_Name" runat="server"/> 
                                                    </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Number" HeaderText="數量" SortExpression="Number" DataFormatString="{0:N0}" HtmlEncode="False">
                                                    <itemstyle horizontalalign="Right" />
                                                </asp:BoundField>
                                            </Columns>
                                            <SelectedRowStyle HorizontalAlign="Center" />
                                        </cc1:GridViewPageingByDB>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr id="trbtnsave" style="display: table-row">
                <td align="right" colspan="4">
                    <asp:Button ID="btnSave" CssClass="btn" Enabled="false" runat="server" Text="確定" OnClick="btnSave_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" CssClass="btn" runat="server" Text="取消" OnClick="btnCancel_Click" />
                </td>
            </tr>
            <tr id="trbtndel" style="display: none">
                <td align="right" colspan="4">
                    <asp:Button ID="btnDeldata" CssClass="btn" runat="server" Text="刪除已匯入資料" OnClick="btnDeldata_Click" />
                </td>
            </tr>
            <tr id="trsearchld" valign="baseline" style="display: none">
                <td align="right" colspan="4">
                    <asp:Button ID="btnQuery" CssClass="btn" runat="server" Text="查詢" OnClick="btnQuery_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="AddP" CssClass="btn" runat="server" Text="新增例外代製項目" Visible="false" OnClick="AddP_Click"/>
                    <input id="btnAddP" onclick="var aa=window.showModalDialog('Finance0024Addp.aspx?ActionType=Add','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}"
                        type="button" value="新增例外代製項目" class="btn" runat="server" visible="true" />
                </td>
            </tr>
            <tr valign="baseline" style="display: none" id="SearchProjectGrid">
                <td colspan="3">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr id="tr2" style="display: table-row">
                                    <td>
                                        <cc1:GridViewPageingByDB ID="gvbProject" runat="server" AutoGenerateColumns="False"
                                            ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                                            EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                            OnOnSetDataSource="gvbProject_OnSetDataSource" PreviousPageText="<" OnRowDataBound="gvbProject_RowDataBound">
                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                            <RowStyle CssClass="GridViewRow" />
                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                PreviousPageText="&lt;" Visible="False" />
                                            <Columns>
                                                <asp:BoundField HeaderText="用途" DataField="Param_Name" />
                                                <asp:BoundField HeaderText="群組" DataField="Group_Name" />
                                                <asp:BoundField HeaderText="日期" DataField="Project_Date" />
                                                <asp:TemplateField HeaderText="例外代製項目">
                                                    <itemstyle horizontalalign="Center" width="45%"></itemstyle>
                                                    <itemtemplate>
                                                        <asp:HyperLink id="hlModify" runat="server"></asp:HyperLink>
                                                    </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Number" HeaderText="數量" SortExpression="Number" DataFormatString="{0:N0}" HtmlEncode="False">
                                                    <itemstyle horizontalalign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Unit_Price" HeaderText="單價" SortExpression="Unit_Price" DataFormatString="{0:N2}" HtmlEncode="False">
                                                    <itemstyle horizontalalign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="備註" DataField="Comment" />
                                                <asp:TemplateField HeaderText="刪除">
                                                    <itemstyle horizontalalign="Center" width="100px" />
                                                    <itemtemplate>
                                                        <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDelete" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDelete_Command"></asp:ImageButton>
                                                    </itemtemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <SelectedRowStyle HorizontalAlign="Center" />
                                        </cc1:GridViewPageingByDB>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr id="trsearchfz" valign="baseline" style="display: none">
                <td align="right" colspan="4">
                    <asp:Button Height="25" ID="btnQuery2" CssClass="btn" runat="server" Text="查詢" OnClick="btnQuery2_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="AddF" Height="25" CssClass="btn" runat="server" Text="新增罰款&折讓" Visible="false" OnClick="AddF_Click"/>
                    <input id="btnAddF" Height="25" onclick="var aa=window.showModalDialog('Finance0024Addf.aspx?ActionType=Add','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind2();}"
                        type="button" value="新增罰款&折讓" class="btn" runat="server" visible="true" />
                </td>
            </tr>
            <tr style="height: 20px">
                <td>
                    &nbsp;</td>
            </tr>
            <tr valign="top" style="display: none" id="FinanceGrid">
                <td colspan="3">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr id="tr3" style="display: table-row">
                                    <td>
                                        <cc1:GridViewPageingByDB ID="gvbFinance" runat="server" AutoGenerateColumns="False"
                                            ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                                            EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                                            OnOnSetDataSource="gvbFinance_OnSetDataSource" PreviousPageText="<" OnRowDataBound="gvbFinance_RowDataBound">
                                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                            <AlternatingRowStyle CssClass="GridViewAlter" />
                                            <RowStyle CssClass="GridViewRow" />
                                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                PreviousPageText="&lt;" Visible="False" />
                                            <Columns>
                                                <asp:BoundField HeaderText="用途" DataField="Param_Name" />
                                                <asp:BoundField HeaderText="群組" DataField="Group_Name" />
                                                <asp:BoundField HeaderText="Perso廠" DataField="Factory_shortname_cn" />                                                
                                                <asp:TemplateField HeaderText="年月">
                                                    <itemstyle horizontalalign="Center"></itemstyle>
                                                    <itemtemplate>                                                        
                                                        <asp:Label id="lblProject_Date" runat="server"/> 
                                                    </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="項目">
                                                    <itemstyle horizontalalign="Center" width="45%"></itemstyle>
                                                    <itemtemplate>
                                                        <asp:HyperLink id="hlModify2" runat="server"></asp:HyperLink>
                                                    </itemtemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Price" HeaderText="金額" SortExpression="Price" DataFormatString="{0:N2}" HtmlEncode="False">
                                                    <itemstyle horizontalalign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="備註" DataField="Comment" />
                                                <asp:TemplateField HeaderText="刪除">
                                                    <itemstyle horizontalalign="Center" width="100px" />
                                                    <itemtemplate>
                                                        <asp:ImageButton CausesValidation="false" runat="server" ID="ibtnDeleteItem" Height="18px" ImageUrl="~/images/trash_0021.gif"  OnCommand="btnDeleteItem_Command"></asp:ImageButton>
                                                    </itemtemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <SelectedRowStyle HorizontalAlign="Center" />
                                        </cc1:GridViewPageingByDB>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
