<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Depository010.aspx.cs" Inherits="Depository_Depository010" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>物料庫存管理</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">	
    
        function doRadio(rad)
        {
            if (rad == document.getElementById("adrtMaterielIn"))
            {
                document.getElementById("MaterielDel").style.display="none";
                document.getElementById("MaterielIn").style.display="table-row";
                document.getElementById("MaterielInGrid").style.display="table-row";
                document.getElementById("MaterielSubmit").style.display="table-row";
                document.getElementById("RequiredFieldValidator1").enabled=false;
                document.getElementById("RequiredFieldValidator2").enabled=false;
                document.getElementById("RequiredFieldValidator3").enabled=true;
                document.getElementById("RequiredFieldValidator4").enabled=true;
            }
            else
            {
                document.getElementById("MaterielDel").style.display="table-row";
                document.getElementById("MaterielIn").style.display="none";
                document.getElementById("MaterielInGrid").style.display="none";
                document.getElementById("MaterielSubmit").style.display="none";
                document.getElementById("RequiredFieldValidator1").enabled=true;
                document.getElementById("RequiredFieldValidator2").enabled=true;
                document.getElementById("RequiredFieldValidator3").enabled=false;
                document.getElementById("RequiredFieldValidator4").enabled=false;
            }
        }        
        
        function onLoaddoRadio()
        {
            if (document.getElementById("adrtMaterielIn").checked)
            {
                doRadio(document.getElementById("adrtMaterielIn"));
            }
            else
            {
                doRadio(document.getElementById("adrtMaterielDel"));
            }
        } 
        function OnClickbtnSubmit()
        {
            document.getElementById("MaterielSubmit").style.display="none";
        }  
    
    </script>

</head>
<body onload="onLoaddoRadio();">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="1" style="width: 100%">
                <tr valign="baseline" height="20px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">物料庫存管理</font>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <table width="100%" class="tbl_row" cellpadding="0" cellspacing="2" border="0">
                            <tr valign="baseline">
                                <td align="left" colspan="3">
                                    <input id="adrtMaterielIn" runat="server" name="radioMaterielDelIn" onclick="doRadio(this);"
                                        type="radio" value="1" checked="true" />匯入物料庫存異動&nbsp; &nbsp;
                                    <input id="adrtMaterielDel" runat="server" value="2" name="radioMaterielDelIn" onclick="doRadio(this);"
                                        type="radio" />刪除匯入
                                </td>
                            </tr>
                            <tr valign="baseline" id="MaterielDel" style="display: none">
                                <td align="right" style="width: 43%">
                                    <span style="color: #ff0000">*</span>廠商結餘日期：
                                    <asp:TextBox ID="txtDate" runat="server" MaxLength="10" onfocus="WdatePicker()"></asp:TextBox>
                                    <img align="Middle" onclick="WdatePicker({el:$dp.$('txtDate')})" src="../images/calendar.gif" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDate"
                                        ErrorMessage="廠商結餘日期必須輸入">*</asp:RequiredFieldValidator>
                                </td>
                                <td style="width:3%"></td>
                                <td align="left">
                                    <span style="color: #ff0000">*</span>Perso廠商：
                                    <asp:DropDownList ID="dropFactoryDel" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="dropFactoryDel"
                                        ErrorMessage="Perso廠商必須輸入">*</asp:RequiredFieldValidator>
                                    <asp:Button CssClass="btn" ID="btnDelete" runat="server" Text="刪除匯入" OnClick="btnDelete_Click" />
                                </td>
                            </tr>
                            <tr valign="baseline" id="MaterielIn" style="display: table-row">
                                <td align="right" style="width: 43%">
                                    <span style="color: #ff0000">*</span>匯入資料：
                                    <asp:FileUpload ID="FileUpd" runat="server" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="FileUpd"
                                        ErrorMessage="匯入資料必須輸入">*</asp:RequiredFieldValidator>
                                </td>
                                <td style="width:3%"></td>
                                <td align="left">
                                    <span style="color: #ff0000">*</span>Perso廠商：
                                    <asp:DropDownList ID="dropFactoryIn" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="dropFactoryIn"
                                        ErrorMessage="Perso廠商必須輸入">*</asp:RequiredFieldValidator>
                                    <asp:Button CssClass="btn" ID="btnDataIn" runat="server" Text="開始匯入" OnClick="btnDataIn_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline" style="display: table-row" id="MaterielInGrid">
                    <td>
                        <table width="100%">
                            <tr valign="baseline">
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbPersoStockIn" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" FirstPageText="<<"
                                        LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" SortAscImageUrl="~/images/asc.gif"
                                        SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                        EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#" OnOnSetDataSource="gvpbPersoStockIn_OnSetDataSource" OnRowDataBound="gvpbPersoStockIn_RowDataBound">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:BoundField HeaderText="品名">
                                                <itemstyle horizontalalign="Left" width="25%" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="日期">
                                                <itemstyle horizontalalign="Left" width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="類別">
                                                <itemstyle horizontalalign="Left" width="10%" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="數量">
                                                <itemstyle horizontalalign="Right" width="10%" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="備註">
                                                <itemstyle width="45%" />
                                                <itemtemplate>
                                                        <asp:TextBox runat="server" ID="txtComment" MaxLength='100' width="300px">
                                                        </asp:TextBox>
                                                </itemtemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr valign="baseline" style="display: table-row" id="MaterielSubmit">
                    <td align="right">
                        <asp:Button CssClass="btn" ID="btnSubmit" runat="server" Text="確定" OnClick="btnSubmit_Click" OnClientClick="OnClickbtnSubmit();" CausesValidation="False" />
                        &nbsp;
                        <asp:Button CssClass="btn" ID="btnCancel" runat="server" Text="取消" CausesValidation="False" OnClick="btnCancel_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
