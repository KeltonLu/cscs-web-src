<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="InOut005.aspx.cs" Inherits="InOut_InOut004" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>次月換卡預測表匯入</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>

<script language="javascript" type="text/javascript">	
<!--
    function doRadio(rad)
    {
        if (rad == document.getElementById("adrtInftp"))
        {
            document.getElementById("trInyear").style.display="table-row";
            document.getElementById("trIn2").style.display="table-row";
            document.getElementById("trIn1").style.display="none";
            document.getElementById("trSearch1").style.display="none";
            document.getElementById("trSearch2").style.display="none";
            document.getElementById("trSearch3").style.display="none";
            document.getElementById("trSearch4").style.display="none";
            document.getElementById("trSearch5").style.display="none";
        }
        if (rad == document.getElementById("adrtIn"))
        {
            document.getElementById("trIn1").style.display="table-row";
            document.getElementById("trIn2").style.display="table-row";
             document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearch1").style.display="none";
            document.getElementById("trSearch2").style.display="none";
            document.getElementById("trSearch3").style.display="none";
            document.getElementById("trSearch4").style.display="none";
            document.getElementById("trSearch5").style.display="none";
        }
       if (rad == document.getElementById("adrtSearch"))
        {
           document.getElementById("trIn1").style.display="none";
           document.getElementById("trIn2").style.display="none";           
            document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearch1").style.display="table-row";
            document.getElementById("trSearch2").style.display="table-row";
            document.getElementById("trSearch3").style.display="table-row";
            document.getElementById("trSearch4").style.display="table-row";
            document.getElementById("trSearch5").style.display="table-row";                
        }
    } 
    
    function loadRadio()
    {
      if(true == document.getElementById("adrtIn").checked)
     {
           document.getElementById("trIn1").style.display="table-row";
            document.getElementById("trIn2").style.display="table-row";
             document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearch1").style.display="none";
            document.getElementById("trSearch2").style.display="none";
            document.getElementById("trSearch3").style.display="none";
            document.getElementById("trSearch4").style.display="none";
            document.getElementById("trSearch5").style.display="none";
           
     }
           else if(true == document.getElementById("adrtSearch").checked)
     {
           document.getElementById("trIn1").style.display="none";
           document.getElementById("trIn2").style.display="none";           
           document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearch1").style.display="table-row";
            document.getElementById("trSearch2").style.display="table-row";
            document.getElementById("trSearch3").style.display="table-row";
            document.getElementById("trSearch4").style.display="table-row";
            document.getElementById("trSearch5").style.display="table-row";            
     }

        else if(true == document.getElementById("adrtInftp").checked)
     {
           document.getElementById("trInyear").style.display="table-row";
           document.getElementById("trIn2").style.display="table-row";           
          document.getElementById("trIn1").style.display="none";
            document.getElementById("trSearch1").style.display="none";
            document.getElementById("trSearch2").style.display="none";
            document.getElementById("trSearch3").style.display="none";
            document.getElementById("trSearch4").style.display="none";
            document.getElementById("trSearch5").style.display="none";            
     }
    } 
      
    //調用後台方法，將正確的資料導入DB！
    function ImtBind()
    {                
        __doPostBack('btnErrImp','');
    }    
             
//-->
</script>

<body onload="loadRadio();">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr style="height: 20px">
                    <td>
                        &nbsp;</td>
                </tr>
                <tr valign="baseline" style="font-size: 12pt">
                    <td class="PageTitle" style="height: 19px;" colspan="2">
                        次月換卡預測表匯入
                    </td>
                </tr>
                <tr class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td style="height: 19px" colspan="2">
                        <input id="adrtInftp" runat="server" name="radioIn_Search" onclick="doRadio(this);"
                            type="radio" value="3" checked="true" />
                        匯入伺服器資料 &nbsp;
                        <input id="adrtIn" runat="server" name="radioIn_Search" onclick="doRadio(this);"
                            type="radio" value="1" /><span style="color: #000000;">匯入本機資料</span><input id="adrtSearch"
                                runat="server" name="radioIn_Search" onclick="doRadio(this);" type="radio" value="2" />查詢
                        <asp:Button ID="btnErrImp" runat="server" CausesValidation="False" OnClick="btnBind_Click"
                            Visible="False" /></td>
                </tr>
                <tr class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td colspan="2" style="height: 19px">
                        &nbsp;
                    </td>
                </tr>
                <tr id="trIn1" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td style="height: 19px" colspan="2">
                        <span style="color: #ff0000">*</span>匯入資料：&nbsp;<asp:FileUpload ID="FileUpd" runat="server" /></td>
                </tr>
                <tr class="tbl_row" id='trInyear' style="display: table-row">
                    <td colspan="3" style="height: 26px;">
                        <span class="style1" style="color: #000000"><span style="font-size: 9pt; color: #ff0000">
                            *</span>匯入年：</span><asp:DropDownList ID="ddlYear" runat="server">
                            </asp:DropDownList>月：<asp:DropDownList ID="ddlMonth" runat="server">
                            </asp:DropDownList></td>
                </tr>
                <tr id="trIn2" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td align="right" style="height: 19px" colspan="2">
                        <asp:Button ID="btnImport" runat="server" CssClass="btn" OnClick="btnImport_Click"
                            Text="開始匯入" /></td>
                </tr>
                <tr id="trSearch1" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td colspan="2" style="height: 19px">
                        <span style="color: #ff0000; background-color: #e4e4e4">*</span>換卡年：<asp:DropDownList
                            ID="dropChange_Year" runat="server">
                        </asp:DropDownList>換卡月：<asp:DropDownList ID="dropChange_Month" runat="server">
                        </asp:DropDownList>
                        版面簡稱：<asp:TextBox ID="txtName" runat="server" MaxLength="30" Width="64px"></asp:TextBox>耗用版面：<asp:TextBox
                            ID="txtDeplete" runat="server" MaxLength="30" Width="64px"></asp:TextBox></td>
                </tr>
                <tr class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td colspan="2" style="height: 5px">
                    </td>
                </tr>
                <tr id="trSearch2" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td colspan="2" style="height: 19px">
                        &nbsp;Perso廠：<asp:DropDownList ID="dropFactory_Name" runat="server" DataTextField="Factory_ShortName_CN"
                            DataValueField="RID">
                        </asp:DropDownList></td>
                </tr>
                <tr id="trSearch3" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td align="right" colspan="2" style="height: 19px">
                        <asp:Button ID="btnSearch" runat="server" CssClass="btn" OnClick="btnSearch_Click"
                            Text="查詢" /></td>
                </tr>
                <tr id="trSearch4" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td colspan="2" style="height: 19px">
                        <cc1:GridViewPageingByDB ID="gvpbChangeCard" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                            ColumnsHeaderText="" CssClass="GridView" EditButtonImageUrl="" EditButtonText="編輯"
                            EditCellWidth="48px" EditPageUrl="#" FirstPageText="<<" LastPageText=">>" NextPageText=">"
                            OnOnSetDataSource="gvpbChangeCard_OnSetDataSource" PreviousPageText="<" SortAscImageUrl="~/images/asc.gif"
                            SortDescImageUrl="~/images/desc.gif" Width="1px" OnRowDataBound="gvpbChangeCard_RowDataBound">
                            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                            <AlternatingRowStyle CssClass="GridViewAlter" />
                            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                PreviousPageText="&lt;" Visible="False" />
                            <RowStyle CssClass="GridViewRow" />
                            <Columns>
                                <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠">
                                    <itemstyle horizontalalign="Left" />
                                    <headerstyle horizontalalign="Center" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="卡片編號" />
                                <asp:BoundField DataField="TYPE" />
                                <asp:BoundField DataField="AFFINITY" />
                                <asp:BoundField DataField="PHOTO" />
                                <asp:BoundField DataField="Name" HeaderText="版面簡稱" HtmlEncode="False">
                                    <itemstyle horizontalalign="Left" />
                                    <headerstyle horizontalalign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="耗用卡版面" HeaderText="耗用版面" />
                                <asp:BoundField DataField="Number" HeaderText="換卡預測數量">
                                    <itemstyle horizontalalign="Right" />
                                    <headerstyle horizontalalign="Center" />
                                </asp:BoundField>
                            </Columns>
                        </cc1:GridViewPageingByDB>
                    </td>
                </tr>
                <tr id="trSearch5" class="tbl_row" style="font-size: 12pt" valign="baseline">
                    <td align="right" colspan="2" style="height: 19px">
                        <asp:Button ID="btnExport" runat="server" CausesValidation="False" CssClass="btn"
                            Text="匯出EXCEL表格" Visible="False" OnClick="btnExport_Click" /></td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
