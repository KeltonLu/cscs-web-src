<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Warning.aspx.cs" Inherits="Warning" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>警告</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="css/right_css.css" rel="stylesheet" type="text/css" />
		<style type="text/css">
        <!--
        .jg_text {
	        font-size: 16px;
	        font-weight: bold;
	        letter-spacing: 10px;
	        text-indent: 15px;
        }

        .ts_bk {
	        border: 1px solid #1A417B;
        }

        .all_button {
	        font-size: 12px;
	        font-weight: normal;
	        color: #426464;
	        border: 1px solid #4D7777;
	        background-image:url(img/anniu_bg.gif);
	        height: 20px;
	        width:auto;
	        padding-left:2px;
	        padding-right:2px;
	        padding-top:2px;
	        padding-bottom:2px;
	        letter-spacing: 3px;
	        font-family: "宋体";
        }
        -->
        </style>
	</HEAD>
	<body >
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="760" border="0" align="center">
			     <tr>
                      <td ><br /><br /></td>
                 </tr>  
			     <tr>
                        <td align="center" valign="top">
                            <table width="650" border="0" cellpadding="0" cellspacing="0" bgcolor="#FCFDFD" class="ts_bk">
                              
                              <tr>
                                <td height="29" colspan="2" background="images/ts_top1_bg.gif">&nbsp;</td>
                                </tr>
                              <tr>
                                <td width="278" height="45" align="right" valign="bottom" background="images/ts_top1_bg.gif"><img src="images/ts.gif" width="40" height="38" /></td>
                                <td width="370" height="45" align="left" valign="bottom" background="images/ts_top1_bg.gif" class="jg_text">訊息提示</td>
                              </tr>
                              <tr>
                                <td height="50" colspan="2" align="center" valign="middle">
                                    <span id="Span1">
                                        <asp:label id="lblError" runat="server" ForeColor="#0000C0" Font-Size="12px"></asp:label>
                                    </span>
                                </td>
                              </tr>
                              <tr>
                                <td height="50" colspan="2" align="center" valign="middle">&nbsp;</td>
                              </tr>
                              <tr>
                                <td colspan="2" align="center" valign="middle">
                                  <input id="btnOK" class="all_button" type="button" value=" 確 定 " name="btnOK" runat="server">
                                  <input id="showdetail" class="all_button" type="button" value="顯示詳細 ▼" name="showdetail" runat="server" onserverclick="showdetail_ServerClick">
                               </td>
                              </tr>
                              <tr>
                                <td height="20" colspan="2" align="center" valign="middle"></td>
                              </tr>
                              <tr id="detail" runat="server">
                                    <td colspan="2" align="center" valign="middle">
                                        <asp:textbox id="txtMessage" runat="server" Width="400px" TextMode="MultiLine"></asp:textbox>
                                    </td>
                              </tr>
                              <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                              </tr>
                              <tr>
                                <td height="32" colspan="2" background="images/ts_bot_bg.gif">&nbsp;</td>
                              </tr>
                            </table>
                        </td>
                  </tr>
			</table>
		</form>
	</body>
</HTML>
