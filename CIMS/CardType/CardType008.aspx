<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardType008.aspx.cs" Inherits="CardType_CardType008" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<%@ Register Src="../CommUserCtrl/urctrlCardTypeGroupSelect.ascx" TagName="urctrlCardTypeGroupSelect"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>晶片資料檔</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <table bgcolor="#ffffff" width="100%" align="center" cellpadding="0" cellspacing="1"
                border="0">
                <tr style="height: 10px">
                    <td>&nbsp;</td>
                </tr>
                <tr valign="baseline">
                    <td>
                        <font class="PageTitle">晶片資料檔</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="tbl_row" width="100%" border="0" cellpadding="0" cellspacing="2">
                            <tr>
                                <td colspan="4">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <tr class="tbl_row" valign="bottom">
                                                    <td align="right" style="width: 15%">晶片名稱：</td>
                                                    <td>
                                                        <asp:TextBox ID="txtWafer_Name" runat="server" MaxLength="20"></asp:TextBox>
                                                    </td>
                                                    <td align="right">晶片容量：</td>
                                                    <td>
                                                        <%--Dana 20161021 最大長度由 4 改為 5 --%>
                                                        <asp:TextBox ID="txtBG" Style="ime-mode: disabled; text-align: right" onblur="value=GetValue(this.value)"
                                                            size="15" MaxLength="5" onkeyup="CheckNum('txtBG',4)" runat="server"></asp:TextBox>K（起）<asp:TextBox
                                                                ID="txtEnd" Style="ime-mode: disabled; text-align: right" onblur="value=GetValue(this.value)"
                                                                size="15" MaxLength="5" runat="server" onkeyup="CheckNum('txtEnd',4)"></asp:TextBox>K（迄）
                                                        <%--Dana 20161025 此控件驗證有誤，未能正確識別分隔符 %>
                                                        <%--<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtEnd" ControlToValidate="txtBG"
                                                            ErrorMessage="晶片容量（起）不能大於（迄）" Operator="LessThanEqual" Type="Integer">*</asp:CompareValidator>--%>

                                                    </td>
                                                </tr>
                                                <tr valign="bottom" class="tbl_row">
                                                    <td align="right" style="width: 15%">品牌：</td>
                                                    <td>
                                                        <asp:TextBox ID="txtMark" runat="server" MaxLength="10"></asp:TextBox></td>
                                                    <td align="right">晶片供應商：</td>
                                                    <td>
                                                        <asp:TextBox ID="txtWafer_Factory" runat="server" MaxLength="15"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4" align="left">
                                                        <uc1:urctrlCardTypeGroupSelect ID="urctrlCardTypeGroupSelect" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr class="tbl_row" valign="bottom">
                                                    <td align="right" style="width: 15%">空白卡廠：</td>
                                                    <td colspan="3">
                                                        <asp:DropDownList ID="drpFactory_shortname_cn" runat="server">
                                                        </asp:DropDownList></td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr valign="bottom">
                                <td colspan="4" align="right">
                                    <asp:Button Text="查詢" ID="btnQuery" class="btn" runat="server" OnClick="btnQuery_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button Text="新增" ID="btnAdd" class="btn" runat="server" OnClick="btnAdd_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <table name="queryResult" id="queryResult" width="100%">
                                    <tr>
                                        <td>
                                            <cc1:GridViewPageingByDB ID="gvpbWAFER" runat="server" AllowPaging="True" DataKeyNames="RID"
                                                OnOnSetDataSource="gvpbWAFER_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                                FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView"
                                                OnRowDataBound="gvpbWAFER_RowDataBound" SortAscImageUrl="~/images/asc.gif" SortDescImageUrl="~/images/desc.gif"
                                                ColumnsHeaderText="" EditButtonImageUrl="" EditButtonText="編輯" EditCellWidth="48px"
                                                EditPageUrl="#">
                                                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                                <AlternatingRowStyle CssClass="GridViewAlter" />
                                                <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                                    PreviousPageText="&lt;" />
                                                <RowStyle CssClass="GridViewRow" />
                                                <Columns>
                                                    <asp:HyperLinkField DataTextField="Wafer_Name" HeaderText="晶片名稱" SortExpression="Wafer_Name"
                                                        DataNavigateUrlFields="RID" DataNavigateUrlFormatString="CardType008Edit.aspx?ActionType=Edit&RID={0}">
                                                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                                                    </asp:HyperLinkField>
                                                    <asp:TemplateField HeaderText="晶片容量" SortExpression="Wafer_Capacity">
                                                        <ItemStyle Width="4%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblWafer_Capacity" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Mark" HeaderText="品牌" SortExpression="Mark" />
                                                    <asp:BoundField DataField="Wafer_Factory" HeaderText="晶片供應商" SortExpression="Wafer_Factory" />
                                                    <asp:TemplateField HeaderText="空白卡廠">
                                                        <ItemStyle Width="8%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFACTORY_NAME" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Interface" HeaderText="接觸式介面" />
                                                    <asp:BoundField DataField="Protocle" HeaderText="非接觸式介面" />
                                                    <asp:BoundField DataField="Wafer_Type" HeaderText="晶片型號" SortExpression="Wafer_Type" />
                                                    <asp:BoundField DataField="OS" HeaderText="OS" />
                                                    <asp:BoundField DataField="Java_Version" HeaderText="JAVA 版本" />
                                                    <asp:BoundField DataField="Crypto_Cap" HeaderText="Crypto功能" />
                                                    <asp:TemplateField HeaderText="ROM容量" SortExpression="ROM_Capacity">
                                                        <ItemStyle Width="4%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblROM_Capacity" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="First_Time" HeaderText="初次發卡時間" SortExpression="First_Time" />
                                                    <asp:BoundField DataField="Unit_Price" SortExpression="Unit_Price" HeaderText="參考單價" DataFormatString="{0:N2}" HtmlEncode="False">
                                                        <ItemStyle Width="8%" HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Pre_Program" HeaderText="出廠預設應用程式" />
                                                    <asp:TemplateField HeaderText="使用狀態">
                                                        <ItemStyle Width="3%" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIs_Using" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </cc1:GridViewPageingByDB>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
