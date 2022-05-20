<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MainFrame.aspx.cs" Inherits="MainFrame" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡片庫存管理系統</title>
    <link href="css/Appmenu.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="JS/CommJS.js"></script>

    <link href="css/Main.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript">
        function GoPage(url)
        {
            if (url.length > 6)
            {
                url = url.substring(6);
            }
            document.getElementById("ViewFrame").src = url;
        }
        function ShowWaitDiv(show)
        {
            if (show == '1')
            {
                document.getElementById("WaitDiv").style.visibility = "visible";
            }
            else
            {
                document.getElementById("WaitDiv").style.visibility = "hidden";
            }
        }
    </script>

</head>
<body bgcolor="#8E9574" marginheight="0" marginwidth="0" rightmargin="0" leftmargin="0"
    bottommargin="0" topmargin="0" style="font-size: 8pt; overflow: scroll; overflow-x: auto; overflow-y: auto;">
    <form id="form1" runat="server">
        <div>
            <table width="1000px" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <table width="1000px" height="75px" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <font size="2" face="Arial">登入名稱:<asp:Label ID="lblUserName" runat="server" Text=""></asp:Label></font>
                                </td>
                                <td align="right">
                                    <a target="ViewFrame" href="none.aspx">
                                        <img border="0" src="Images/TF_Home.gif"></a><a target="_top" href="Login.aspx"><img
                                            border="0" src="Images/TF1.jpg" width="57" height="19">
                                        </a>
                                </td>
                            </tr>
                            <tr style="height: 56px;" valign="middle">
                                <td bgcolor="#006558" align="left" style="height: 56px">
                                    <img border="0" height="56px" src="Images/Logo.bmp" hspace="0" vspace="0">
                                </td>
                                <td align="right" style="height: 56px; width: 197px; background-color: #00948c;">
                                    <img border="0" height="56px" src="Images/LogoB.bmp" hspace="0" vspace="0">
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" width="1000px" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="width: 12px">&nbsp;</td>
                                <td>
                                    <asp:Menu ID="MnuMain" runat="server" Orientation="Horizontal" DynamicBottomSeparatorImageUrl="~/images/Menu_Line.gif"
                                        DynamicPopOutImageUrl="~/images/Menu_E.gif" StaticPopOutImageUrl="~/images/Menu_E1.gif"
                                        EnableTheming="False" EnableViewState="False">
                                        <StaticMenuItemStyle BorderColor="#B8C8BB" BorderStyle="Solid" BorderWidth="1px"
                                            ForeColor="White" HorizontalPadding="3px" Font-Size="8pt" ItemSpacing="0px" VerticalPadding="2px"
                                            Width="120px" Height="20px" />
                                        <DynamicMenuItemStyle Font-Size="8pt" HorizontalPadding="0px" ItemSpacing="0px" VerticalPadding="2px"
                                            BorderColor="#B8C8BB" BorderStyle="None" BorderWidth="1px" ForeColor="Black" />
                                        <DataBindings>
                                            <asp:MenuItemBinding DataMember="MenuLevel1" TextField="FunctionName" NavigateUrlField="FunctionUrl" />
                                            <asp:MenuItemBinding DataMember="MenuLevel2" TextField="FunctionName" NavigateUrlField="FunctionUrl" />
                                            <asp:MenuItemBinding DataMember="MenuLevel3" TextField="FunctionName" NavigateUrlField="FunctionUrl" />
                                            <asp:MenuItemBinding DataMember="MenuLevel4" NavigateUrlField="FunctionUrl" TextField="FunctionName" />
                                        </DataBindings>
                                        <DynamicMenuStyle BackColor="#F5F8FF" BorderColor="#B8C8BB" BorderStyle="Solid" BorderWidth="1px"
                                            CssClass="MenuItem" HorizontalPadding="0px" VerticalPadding="2px" />
                                        <DynamicHoverStyle BackColor="#F5F8FF" BorderColor="#8E9574" BorderStyle="Solid"
                                            BorderWidth="1px" />
                                        <StaticHoverStyle BackColor="#B8C8BB" BorderColor="#8E9574" />
                                        <StaticMenuStyle HorizontalPadding="0px" />
                                    </asp:Menu>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table border="0" width="1000px" height="80%" id="table1" cellpadding="0" cellspacing="0">
                <tr valign="top">
                    <td style="width: 12px">&nbsp;</td>
                    <td>
                        <iframe name="ViewFrame" id="ViewFrame" scrolling="yes" frameborder="0" width="988px"
                            src="none.aspx" height="100%"></iframe>
                    </td>
                </tr>
            </table>
        </div>
        <div id="WaitDiv" style="z-index: 1; left: 150px; visibility: hidden; width: 400px; position: absolute; top: 250px; height: 115px">
            <table bgcolor="lightcyan" width="300" style="border-right: #353299 1px solid; border-top: #353299 1px solid; border-left: #353299 1px solid; border-bottom: #353299 1px solid">
                <tbody>
                    <tr>
                        <td align="center">
                            <img alt="Report is being generated" src="images/Waiting.gif">
                        </td>
                    </tr>
                    <tr>
                        <td align="center">資料處理中...等一下喔
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <script>
            //alert(document.body.clientHeight);
            //alert(document.getElementById("table1").height);
        </script>

    </form>
</body>
</html>
