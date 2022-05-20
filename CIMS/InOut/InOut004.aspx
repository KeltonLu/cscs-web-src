<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="InOut004.aspx.cs"
    Inherits="InOut_InOut004" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>年度換卡預測表匯入</title>
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
                document.getElementById("trIn1").style.display="none";
                document.getElementById("trIn2").style.display="table-row";
                document.getElementById("trSearchName").style.display="none"; 
                document.getElementById("trSearch1").style.display="none";
                document.getElementById("trSearch2").style.display="none"; 
                document.getElementById("trSearch3").style.display="none";                                   
                document.getElementById("trSearch4").style.display="none"; 
                document.getElementById("trSearch5").style.display="none";                                   
                document.getElementById("trSearch6").style.display="none";                        
            }
    
        if (rad == document.getElementById("adrtIn"))
        {
            document.getElementById("trIn1").style.display="table-row";
            document.getElementById("trIn2").style.display="table-row";
            document.getElementById("trSearchName").style.display="none"; 
            document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearch1").style.display="none";
            document.getElementById("trSearch2").style.display="none"; 
            document.getElementById("trSearch3").style.display="none";                                   
            document.getElementById("trSearch4").style.display="none"; 
            document.getElementById("trSearch5").style.display="none";                                   
            document.getElementById("trSearch6").style.display="none";             
            
         }
        if (rad == document.getElementById("adrtSearch"))
        {
            document.getElementById("trIn1").style.display="none";
            document.getElementById("trIn2").style.display="none";
            document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearchName").style.display="table-row"; 
            document.getElementById("trSearch1").style.display="table-row";
            document.getElementById("trSearch2").style.display="table-row"; 
            document.getElementById("trSearch3").style.display="table-row";                                   
            document.getElementById("trSearch4").style.display="table-row"; 
            document.getElementById("trSearch5").style.display="table-row";                                   
            document.getElementById("trSearch6").style.display="table-row";                        
        }
    }  
    
    function loadRadio()
    {
   
      if(true == document.getElementById("adrtInftp").checked)
     {
            document.getElementById("trInyear").style.display="table-row";
            document.getElementById("trIn1").style.display="none";
            document.getElementById("trIn2").style.display="table-row";
            document.getElementById("trSearch1").style.display="none";
            document.getElementById("trSearch2").style.display="none"; 
            document.getElementById("trSearch3").style.display="none";                                   
            document.getElementById("trSearch4").style.display="none"; 
            document.getElementById("trSearch5").style.display="none";                                   
            document.getElementById("trSearch6").style.display="none"; 
            document.getElementById("trSearchName").style.display="none"; 
              if(true == document.getElementById("radAFFIN").checked)
              {
                   document.getElementById("txtAffinity_Code").disabled = false;
                   document.getElementById("UctrlCardType_dropCard_Purpose").disabled="true";
                   document.getElementById("UctrlCardType_dropCard_Group").disabled="true";
                   document.getElementById("UctrlCardType_LbLeft").disabled="true";
                   document.getElementById("UctrlCardType_LbRight").disabled="true";
                   document.getElementById("UctrlCardType_btnSelectAll").disabled="true";
                   document.getElementById("UctrlCardType_btnSelect").disabled="true";
                   document.getElementById("UctrlCardType_btnRemove").disabled="true";
                   document.getElementById("UctrlCardType_btnRemoveAll").disabled="true";       
              }
              else if(true == document.getElementById("radCard").checked)
              {
                document.getElementById("txtAffinity_Code").disabled = true;
                document.getElementById("UctrlCardType_dropCard_Purpose").disabled="";
                   document.getElementById("UctrlCardType_dropCard_Group").disabled="";
                   document.getElementById("UctrlCardType_LbLeft").disabled="";
                   document.getElementById("UctrlCardType_LbRight").disabled="";
                   document.getElementById("UctrlCardType_btnSelectAll").disabled="";
                   document.getElementById("UctrlCardType_btnSelect").disabled="";
                   document.getElementById("UctrlCardType_btnRemove").disabled="";
                   document.getElementById("UctrlCardType_btnRemoveAll").disabled="";           
              }            
           
     }
    
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
            document.getElementById("trSearch6").style.display="none"; 
             document.getElementById("trSearchName").style.display="none";  
              if(true == document.getElementById("radAFFIN").checked)
              {
                   document.getElementById("txtAffinity_Code").disabled = false;
                   document.getElementById("UctrlCardType_dropCard_Purpose").disabled="true";
                   document.getElementById("UctrlCardType_dropCard_Group").disabled="true";
                   document.getElementById("UctrlCardType_LbLeft").disabled="true";
                   document.getElementById("UctrlCardType_LbRight").disabled="true";
                   document.getElementById("UctrlCardType_btnSelectAll").disabled="true";
                   document.getElementById("UctrlCardType_btnSelect").disabled="true";
                   document.getElementById("UctrlCardType_btnRemove").disabled="true";
                   document.getElementById("UctrlCardType_btnRemoveAll").disabled="true";       
              }
              else if(true == document.getElementById("radCard").checked)
              {
                document.getElementById("txtAffinity_Code").disabled = true;
                document.getElementById("UctrlCardType_dropCard_Purpose").disabled="";
                   document.getElementById("UctrlCardType_dropCard_Group").disabled="";
                   document.getElementById("UctrlCardType_LbLeft").disabled="";
                   document.getElementById("UctrlCardType_LbRight").disabled="";
                   document.getElementById("UctrlCardType_btnSelectAll").disabled="";
                   document.getElementById("UctrlCardType_btnSelect").disabled="";
                   document.getElementById("UctrlCardType_btnRemove").disabled="";
                   document.getElementById("UctrlCardType_btnRemoveAll").disabled="";           
              }            
           
     }
           else if(true == document.getElementById("adrtSearch").checked)
     {
            document.getElementById("trIn1").style.display="none";
            document.getElementById("trIn2").style.display="none";
            document.getElementById("trInyear").style.display="none";
            document.getElementById("trSearchName").style.display="table-row"; 
            document.getElementById("trSearch1").style.display="table-row";
            document.getElementById("trSearch2").style.display="table-row"; 
            document.getElementById("trSearch3").style.display="table-row";                                   
            document.getElementById("trSearch4").style.display="table-row"; 
            document.getElementById("trSearch5").style.display="table-row";                                   
            document.getElementById("trSearch6").style.display="table-row";    
             if(true == document.getElementById("radAFFIN").checked)
              {
                document.getElementById("txtAffinity_Code").disabled = false;
                   document.getElementById("UctrlCardType_dropCard_Purpose").disabled="true";
                   document.getElementById("UctrlCardType_dropCard_Group").disabled="true";
                   document.getElementById("UctrlCardType_LbLeft").disabled="true";
                   document.getElementById("UctrlCardType_LbRight").disabled="true";
                   document.getElementById("UctrlCardType_btnSelectAll").disabled="true";
                   document.getElementById("UctrlCardType_btnSelect").disabled="true";
                   document.getElementById("UctrlCardType_btnRemove").disabled="true";
                   document.getElementById("UctrlCardType_btnRemoveAll").disabled="true";       
              }
              else if(true == document.getElementById("radCard").checked)
              {
                  document.getElementById("txtAffinity_Code").disabled = true;
                   document.getElementById("UctrlCardType_dropCard_Purpose").disabled="";
                   document.getElementById("UctrlCardType_dropCard_Group").disabled="";
                   document.getElementById("UctrlCardType_LbLeft").disabled="";
                   document.getElementById("UctrlCardType_LbRight").disabled="";
                   document.getElementById("UctrlCardType_btnSelectAll").disabled="";
                   document.getElementById("UctrlCardType_btnSelect").disabled="";
                   document.getElementById("UctrlCardType_btnRemove").disabled="";
                   document.getElementById("UctrlCardType_btnRemoveAll").disabled="";           
              }                
     }

    }
    
    function radCard_Affin(rad)
    {
      if(true == document.getElementById("radAFFIN").checked)
      {
        document.getElementById("txtAffinity_Code").disabled = false;
        document.getElementById("txtAffinity_Code").value = "";
           document.getElementById("UctrlCardType_dropCard_Purpose").disabled="true";
           document.getElementById("UctrlCardType_dropCard_Group").disabled="true";
           document.getElementById("UctrlCardType_LbLeft").disabled="true";
           document.getElementById("UctrlCardType_LbRight").disabled="true";
           document.getElementById("UctrlCardType_btnSelectAll").disabled="true";
           document.getElementById("UctrlCardType_btnSelect").disabled="true";
           document.getElementById("UctrlCardType_btnRemove").disabled="true";
           document.getElementById("UctrlCardType_btnRemoveAll").disabled="true";       
      }
      else if(true == document.getElementById("radCard").checked)
      {
        document.getElementById("txtAffinity_Code").disabled = true;
        document.getElementById("txtAffinity_Code").value = "";
           document.getElementById("UctrlCardType_dropCard_Purpose").disabled="";
           document.getElementById("UctrlCardType_dropCard_Group").disabled="";
           document.getElementById("UctrlCardType_LbLeft").disabled="";
           document.getElementById("UctrlCardType_LbRight").disabled="";
           document.getElementById("UctrlCardType_btnSelectAll").disabled="";
           document.getElementById("UctrlCardType_btnSelect").disabled="";
           document.getElementById("UctrlCardType_btnRemove").disabled="";
           document.getElementById("UctrlCardType_btnRemoveAll").disabled="";           
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
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
            <tr style="height: 20px">
                <td style="">
                    &nbsp;</td>
            </tr>
            <tr valign="baseline">
                <td colspan="3">
                    <font class="PageTitle">年度換卡預測表匯入</font></td>
            </tr>
            <tr class="tbl_row" valign="baseline">
                <td style="" colspan="3">
                    <input id="adrtInftp" runat="server" name="radioIn_Search" onclick="doRadio(this);"
                        type="radio" value="3" checked="true" />匯入伺服器資料
                    <input id="adrtIn" runat="server" name="radioIn_Search" onclick="doRadio(this);"
                        type="radio" value="1" />匯入本機資料
                    <input id="adrtSearch" runat="server" value="2" name="radioIn_Search" onclick="doRadio(this);"
                        type="radio" />查詢
                    <asp:Button ID="btnErrImp" runat="server" CausesValidation="False" OnClick="btnBind_Click"
                        Visible="False" /></td>
            </tr>
            <tr class="tbl_row" style="display: table-row">
                <td align="right" style="">
                </td>
                <td colspan="2">
                    &nbsp;</td>
            </tr>
            <tr class="tbl_row" id='trIn1' style="display: table-row;">
                <td align="left" colspan="3">
                    <span style="color: #ff0000">*</span>匯入資料：
                    <asp:FileUpload ID="FileUpd" runat="server" /></td>
            </tr>
            <tr class="tbl_row" id='trInyear' style="display: table-row">
                <td align="left" colspan="3">
                    <span style="color: #ff0000">*</span>匯入年： <span class="style1" style="color: #000000">
                        <span style="font-size: 9pt; color: #ff0000"></span></span>
                    <asp:DropDownList ID="ddlYear" runat="server">
                    </asp:DropDownList>月：<asp:DropDownList ID="ddlMonth" runat="server">
                    </asp:DropDownList>
            </tr>
            <tr class="tbl_row" id='trIn2' style="display: table-row">
                <td colspan="3" align="right">
                    <asp:Button CssClass="btn" ID="btnImport" runat="server" Text="開始匯入" OnClick="btnImport_Click" /></td>
            </tr>
            <tr class="tbl_row" valign="baseline" id='trSearch1' style="display: table-row">
                <td style="height: 22px">
                    &nbsp;
                </td>
                <td align="right" style="height: 22px width:10px;">
                    到期年月：</td>
                <td align="left" style="height: 22px; width: 1000px">
                    <asp:DropDownList ID="ddlYear1" runat="server">
                    </asp:DropDownList>
                    年<asp:DropDownList ID="ddlMonth1" runat="server">
                    </asp:DropDownList>
                    月 ～<asp:DropDownList ID="ddlYear2" runat="server">
                    </asp:DropDownList>
                    年
                    <asp:DropDownList ID="ddlMonth2" runat="server">
                    </asp:DropDownList>月</td>
            </tr>
            <tr class="tbl_row" id='trSearchName' style="display: table-row">
                <td style="height: 24px;">
                    &nbsp;
                </td>
                <td align="right" style="height: 24px;">
                    版面簡稱：</td>
                <td align="left" style="height: 24px">
                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" Width="64px"></asp:TextBox>
                    耗用版面：<asp:TextBox ID="txtDeplete" runat="server" MaxLength="30" Width="64px"></asp:TextBox></td>
            </tr>
            <tr class="tbl_row" id='trSearch2' style="display: table-row">
                <td align="left" style="">
                    <input id="radAFFIN" runat="server" name="radioAFFIN_Card" onclick="radCard_Affin(this);"
                        type="radio" value="1" checked="true" />
                </td>
                <td align="right" style="">
                    <span style="color: #000000">AFFINITY： </span>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtAffinity_Code" runat="server" onkeyup="CheckNum('txtAffinity_Code',10)"
                        Style="ime-mode: disabled; text-align: right" MaxLength="4" Width="64px" /></td>
            </tr>
            <tr class="tbl_row" id='trSearch3' style="display: table-row">
                <td>
                    <input id="radCard" runat="server" name="radioAFFIN_Card" type="radio" onclick="radCard_Affin(this);"
                        value="2" />
                </td>
                <td colspan="2" align="left" width="1000">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 50%;">
                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr class="tbl_row" id='trSearch4' style="display: table-row">
                <td colspan="3" style="height: 7px;" align="right">
                    <asp:Button CssClass="btn" ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" /></td>
            </tr>
            <tr class="tbl_row">
                <td colspan="3">
                    <asp:Label ID="lbMsg" Visible="false" runat="server" Text="查無資料" ForeColor="red"></asp:Label>
                </td>
            </tr>
            <tr class="tbl_row" id='trSearch5' style="display: table-row">
                <td colspan="3">
                    <cc1:GridViewPageingByDB ID="gvpbChangeCard" runat="server" AllowSorting="False"
                        FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                        SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText=""
                        EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#"
                        Width="1%" OnRowDataBound="gvpbChangeCard_RowDataBound">
                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                        <AlternatingRowStyle CssClass="GridViewAlter" />
                        <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                            PreviousPageText="&lt;" Visible="False" />
                        <RowStyle CssClass="GridViewRow" />
                    </cc1:GridViewPageingByDB>
                </td>
            </tr>
            <tr class="tbl_row" id='trSearch6' style="display: table-row">
                <td style="height: 23px;">
                </td>
                <td colspan="2" style="height: 24px;" align="right">
                    <asp:Button CssClass="btn" ID="btnToExcel" runat="server" Text="匯出EXCEL表格" OnClick="btnToExcel_Click" /></td>
            </tr>
        </table>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
