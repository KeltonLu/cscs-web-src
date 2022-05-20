<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeFile="Depository013.aspx.cs"
    Inherits="Depository_Depository013" %>

<%@ Register Src="../CommUserCtrl/uctrlCardType.ascx" TagName="uctrlCardType" TagPrefix="uc1" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>每月監控作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js">

    </script>

    <script type="text/javascript">
    <!--
    function doRadio(rad)
        {        
          if (rad.value == "Type1")
            {  
                      
                document.getElementById("Type2").checked = false; 
                document.getElementById("tr_input").style.display = "";
                document.getElementById("tr_uctrl").style.display = "none";
            }
            else
            {            
                document.getElementById("Type1").checked  = false;          
                document.getElementById("tr_input").style.display = "none";
                document.getElementById("tr_uctrl").style.display = "";
            }
        }
        
   function CheckRio()
   {
        if(document.getElementById("Type1").checked)
        {
            document.getElementById("tr_input").style.display = "";
            document.getElementById("tr_uctrl").style.display = "none";
        }
        else
        {
            document.getElementById("tr_input").style.display = "none";
            document.getElementById("tr_uctrl").style.display = "";
        }
   }     
        //-->
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">每月監控作業</font></td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" width="100%" class="tbl_row" cellspacing="2" border="0">
                            <tr>
                                <td align="right" style="width: 15%">
                                    Perso廠：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropPerso" runat="server" DataTextField="Factory_ShortName_CN"
                                        DataValueField="RID" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    預計月份：
                                </td>
                                <td>
                                    <asp:DropDownList ID="dropMonthCol" runat="server" />&nbsp;&nbsp;預估過去X月的月平均 ：
                                    <asp:DropDownList ID="dropMonth" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    卡種選擇方式：
                                </td>
                                <td>
                                    <asp:RadioButton ID="Type1" runat="server" onclick="doRadio(this);" Text="CARD TYPE/AFFINITY/PHOTO/版面簡稱" />
                                    <asp:RadioButton ID="Type2" runat="server" onclick="doRadio(this);" Text="用途-->群組-->卡種" />
                                </td>
                            </tr>
                            <tr id="tr_input" runat="server">
                                <td align="right" width="15%">
                                    CARDTYPE：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtCardtype" onblur="CheckNum('txtCardtype',3)" onkeyup="CheckNum('txtCardtype',3)"
                                        MaxLength="3" Width="30px" />
                                    &nbsp;&nbsp;AFFINITY：&nbsp<asp:TextBox runat="server" ID="txtAffinity" onblur="CheckNum('txtAffinity',4)"
                                        onkeyup="CheckNum('txtAffinity',4)" MaxLength="4" Width="40px" />
                                    &nbsp;&nbsp;PHOTO：&nbsp;<asp:TextBox runat="server" ID="txtPhoto" onblur="CheckNum('txtPhoto',2)"
                                        onkeyup="CheckNum('txtPhoto',2)" MaxLength="2" Width="20px" />
                                    版面簡稱：<asp:TextBox runat="server" ID="txtName" onkeyup="LimitLengthCheck(this, 30)"
                                        MaxLength="30" />
                                </td>
                            </tr>
                            <tr style="display: none;" id="tr_uctrl" runat="server">
                                <td colspan="2">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <uc1:uctrlCardType ID="UctrlCardType" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="right">
                                    <asp:Button Text="預計" OnClick="btnCalculate_Click" ID="btnCalculate" runat="server"
                                        CssClass="btn" />
                                    <asp:Button style="display: none;" ID="btnDrawTable" runat="server" Text="Button" OnClick="btnDrawTable_Click"
                                        CausesValidation="False" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="Table2" width="100%">
                            <tr>
                                <td class="tbl_row">
                                    C：預估每月新件調整</td>
                                <td class="tbl_row">
                                    J：系統建議or人員調整採購量</td>
                            </tr>
                            <tr>
                                <td class="tbl_row">
                                    H：檢核欄位</td>
                                <td class="tbl_row">
                                    L：調整後檢核欄位</td>
                            </tr>
                            <tr>
                                <td class="tbl_row">
                                    X：預估過去X月的月平均（X1、X2、X3）</td>
                                <td class="tbl_row">
                                    &nbsp;</td>
                            </tr>
                            <tr>
                                <td colspan="2" id="tdResult" runat="server">
                                    <span style='color: #ff0000' id="sp001" visible="false" runat="server">查無資料</span>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2">
                                    <asp:Button Text="匯出Excel格式" Height="25px" CssClass="btn" runat="server" ID="btnReport"
                                        OnClick="btnReport_click" />
                                    &nbsp;&nbsp;
                                    <asp:Button Text="採購下單" Height="25px" CssClass="btn" ID="btnSubmit" OnClick="btnSubmit_click"
                                        runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <script type="text/javascript">
            function changeTable()
            {  
                           
                __doPostBack('btnDrawTable',''); 
                
            }
            </script>

        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
