<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report024.aspx.cs" Inherits="Report_Report024" %>
<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>換卡預測月報表</title>
    
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    
    <script language="javascript" type="text/javascript">	
    <!--       
        function exportExcel()
        {            
            // 打開列印介面
            window.open('Report024Report.aspx?RID=' +  document.getElementById("dropFactoryRID").value+'&date='+ document.getElementById("dropYear").value + document.getElementById("dropMonth").value,'_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=800,height=650');
            
        }        
        
    //-->
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0">
            </asp:ScriptManager>
    <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1" border="0">
				<tr style="height:10px">
					<td>&nbsp;
						
					</td>
				</tr>
				<tr valign="baseline">
					<td>
						<font class="PageTitle">換卡預測月報表</font>
					</td>
				</tr>
				<tr width="100%">
					<td>
						<table width="100%" border="0" cellpadding="0" cellspacing="2" class="tbl_row">
						  <tr align="left" valign="bottom">
                            <td width="6%"><div align="right">年：                              </div></td>
                            <td width="6%">                              
                            <asp:DropDownList style="POSITION: relative" id="dropYear" runat="server">
                                <asp:ListItem>1998</asp:ListItem>
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
                            </td>
                            <td width="4%"><div align="right">月：</div></td>
                            <td width="16%">
                            <asp:DropDownList style="POSITION: relative" id="dropMonth" runat="server">
                                    <asp:ListItem Value="01">1</asp:ListItem>
                                    <asp:ListItem Value="02">2</asp:ListItem>
                                    <asp:ListItem Value="03">3</asp:ListItem>
                                    <asp:ListItem Value="04">4</asp:ListItem>
                                    <asp:ListItem Value="05">5</asp:ListItem>
                                    <asp:ListItem Value="06">6</asp:ListItem>
                                    <asp:ListItem Value="07">7</asp:ListItem>
                                    <asp:ListItem Value="08">8</asp:ListItem>
                                    <asp:ListItem Value="09">9</asp:ListItem>
                                    <asp:ListItem>10</asp:ListItem>
                                    <asp:ListItem>11</asp:ListItem>
                                    <asp:ListItem>12</asp:ListItem>
                                </asp:DropDownList></td>
                            <td width="8%"><div align="right">Perso廠：</div></td>
                            <td width="60%"><asp:DropDownList style="POSITION: relative" id="dropFactoryRID" runat="server">
                                </asp:DropDownList></td>
                          </tr>
							<tr valign="bottom">
								<td colspan="6" align="right">
									<input id="btnExport"  class="btn" style="position: relative" type="button" value="查詢"  onserverclick="btnExport_ServerClick" runat="server"/>
&nbsp;								</td>
							</tr>
					  </table>
					</td>
				</tr>
				<tr valign="top">
                    <td style="height: 415px">
                        <div style="overflow:auto;">
                            <rsweb:reportviewer id="ReportView1" runat="server" processingmode="Remote"
                                width="100%" ShowBackButton="True" ShowFindControls="False" ShowParameterPrompts="False" Visible="False"></rsweb:reportviewer>
                        </div>
                    </td>
                </tr>
		</table>
    </div>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" Style="position: relative" Width="136px" />
    </form>
</body>
</html>
