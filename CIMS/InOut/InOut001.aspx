<%@ Page Language="C#" EnableEventValidation="false"
    AutoEventWireup="true" CodeFile="InOut001.aspx.cs" Inherits="InOut_InOut001" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>小計檔匯入匯出</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">	
    <!--
    function ImtBind()
    {                
        __doPostBack('btnBind','');
    }
    
    function loadRadio()
    {
         if(true == document.getElementById("localIn").checked)
         {
             document.getElementById("deleteTable").style.display="none";
             document.getElementById("serviceInTable").style.display="none";
             document.getElementById("localInTable").style.display="";
         }
         
          if(true == document.getElementById("serviceIn").checked)
         {
             document.getElementById("deleteTable").style.display="none";
             document.getElementById("localInTable").style.display="none";
             document.getElementById("serviceInTable").style.display="";
         }
     
          if(true == document.getElementById("delete").checked)
         {
             document.getElementById("serviceInTable").style.display="none";
             document.getElementById("localInTable").style.display="none";
             document.getElementById("deleteTable").style.display="";
         }
     }
    //-->
    </script>
</head>
<body bgcolor="#ffffff" class="body">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            
            <table width="100%" cellspacing="0">
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">小計檔匯入匯出</font></td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <input id="delete" runat="server" name="action" type="radio" onclick="loadRadio();"
                            checked />刪除已匯入資料
                        <input id="serviceIn" runat="server" name="action" type="radio" onclick="loadRadio();" />匯入伺服器資料
                        <input id="localIn" runat="server" name="action" type="radio" onclick="loadRadio();" />匯入本機資料
                        <asp:Button ID="btnBind" runat="server" CausesValidation="False" OnClick="btnBind_Click"
                            Visible="False" /></td>
                </tr>
            </table>
            <table id="deleteTable" width="100%" cellspacing="0">
                <tr class="tbl_row" valign="baseline">
                    <td align="right" width="15%" style="height: 26px">
                        <span class="style1">*</span>匯入日期：</td>
                    <td style="height: 26px">
                        <asp:TextBox ID="txtBegin_Date1" runat="server" MaxLength="10" onfocus="WdatePicker()"
                            Width="100px" AutoPostBack="True" OnTextChanged="txtBegin_Date1_TextChanged"></asp:TextBox>
                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date1')})" src="../images/calendar.gif" />
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td align="right">
                        批次：</td>
                    <td>
                        <asp:DropDownList ID="dropMakeCardTypeDel" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropMakeCardTypeDel_SelectedIndexChanged">
                        </asp:DropDownList></td>
                </tr>
                <tr class="tbl_row" valign="baseline">
                    <td align="right">
                        檔名：</td>
                    <td>
                        <asp:DropDownList ID="dropImport_FileNameDel" runat="server">
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td style="height: 26px">
                    </td>
                    <td align="right" style="height: 26px">
                        <asp:Button ID="btnDelImport" runat="server" Text="刪除已匯入資料" class="btn" OnClick="btnDelImport_Click" />&nbsp;</td>
                </tr>
            </table>
            <table id="serviceInTable" style="display: none" width="100%" cellspacing="0">
                <tr class="tbl_row">
                    <td align="right" width="15%">
                        <span class="style1">*</span>匯入日期：</td>
                    <td>
                        <asp:TextBox ID="txtBegin_Date2" runat="server" MaxLength="10" onfocus="WdatePicker()"
                            Width="100px"></asp:TextBox>
                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date2')})" src="../images/calendar.gif" />
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td align="right" width="15%" style="height: 26px">
                        <span class="style1">*</span>檔案下傳日期：</td>
                    <td style="height: 26px">
                        <asp:TextBox ID="txtBegin_Date3" runat="server" MaxLength="10" onfocus="WdatePicker()"
                            Width="100px" AutoPostBack="True" OnTextChanged="txtBegin_Date3_TextChanged"></asp:TextBox>
                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date3')})" src="../images/calendar.gif" />
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td align="right">
                        <span class="style1">*</span>檔名：</td>
                    <td>
                        <asp:DropDownList ID="dropImport_FileName" runat="server">
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td style="height: 26px" align="right">
                    </td>
                    <td style="height: 26px" align="right">
                        <asp:Button ID="btnImport" runat="server" Text="開始匯入" class="btn" OnClick="btnImport_Click" />&nbsp;</td>
                </tr>
            </table>
            <table id="localInTable" style="display: none" width="100%" cellspacing="0">
                <tr class="tbl_row">
                    <td align="right" width="15%">
                        <span class="style1">*</span>匯入日期：</td>
                    <td>
                        <asp:TextBox ID="txtBegin_Date4" runat="server" MaxLength="10" onfocus="WdatePicker()"
                            Width="100px"></asp:TextBox>
                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txtBegin_Date4')})" src="../images/calendar.gif" />
                    </td>
                </tr>
                <tr class="tbl_row">
                    <td align="right">
                        <span class="style1">*</span>批次：</td>
                    <td>
                        <asp:DropDownList ID="dropMakeCardType" runat="server">
                        </asp:DropDownList><tr class="tbl_row">
                            <td align="right">
                                <span class="style1">*</span>匯入資料：</td>
                            <td>
                                <asp:FileUpload ID="FileUpd" runat="server" />
                                </td>
                        </tr>
                        <tr>
                            <td style="height: 26px">
                            </td>
                            <td align="right" style="height: 26px">
                                <asp:Button ID="btnImportUp" runat="server" Text="開始匯入" class="btn" OnClick="btnImportUp_Click" />&nbsp;</td>
                        </tr>
            </table>
        </div>
    </form>
</body>
</html>
