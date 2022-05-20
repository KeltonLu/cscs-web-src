<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Finance0024Addf.aspx.cs"
    Inherits="Finance_Finance0024Addf" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>代製費用帳務異動</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 5px">
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">代製費用帳務異動</font>
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEdit" runat="server" Text="確定" class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancel" type="button" value="取消" class="btn" onclick="window.close();" />
                    </td>
                </tr>
                <tr style="width: 100%">
                    <td colspan="2" style="width: 100%">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table class="tbl_row" cellpadding="0" width="100%" cellspacing="2" border="0">
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>用途：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropCard_Purpose_RID" runat="server" OnSelectedIndexChanged="dropCard_Purpose_RID_SelectedIndexChanged"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>群組：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropCard_Group_RID" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>Perso廠：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropFactory" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>年：</td>
                                        <td align="left">
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
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>月：</td>
                                        <td align="left">
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
                                    <tr>
                                        <td align="right">
                                            <font color="red">*</font>帳務異動項目：</td>
                                        <td align="left">
                                            <asp:DropDownList ID="dropParam_Code" runat="server"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr class="tbl_row" valign="baseline">
                                        <td align="right">
                                            <font color="red">*</font>金額：
                                        </td>
                                        <td>
                                                        <%--Dana 20161021 最大長度由 13 改為 14 --%>
                                            <asp:TextBox ID="txtPrice" runat="server" onfocus="DelDouhao(this)" Style="ime-mode: disabled;
                                                text-align: right" onblur="CheckAmt('txtPrice',9,2);value=GetValue(this.value)" onkeyup="CheckAmt('txtPrice',9,2)"
                                                size="10" MaxLength="14"/>
                                    </tr>
                                    <tr class="tbl_row" valign="middle">
                                        <td align="right">
                                            備註：
                                        </td>
                                        <td>
                                            <input id="HidRID" type="hidden" runat="server" />
                                            <asp:TextBox ID="txtComment" runat="server" MaxLength="30" onkeyup=" LimitLengthCheck(this, 30)" Width="400px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr valign="baseline">
                    <td>
                        &nbsp;
                    </td>
                    <td align="right">
                        <asp:Button ID="btnEditD" runat="server" Text="確定" class="btn" OnClick="btnEdit_Click" />
                        &nbsp;&nbsp;
                        <input id="btnCancelD" type="button" value="取消" class="btn" onclick="window.close();" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
