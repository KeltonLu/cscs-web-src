<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InOut003.aspx.cs" Inherits="InOut_InOut003" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡片庫存日結作業</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table width="100%" cellspacing="0">
                <tr style="height: 10px">
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">卡片庫存日結作業</font></td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        最後日結日期：<asp:Label ID="lbDate" runat="server" Text="Label"></asp:Label></td>
                </tr>
                <tr class="tbl_row">
                    <td>
                        <span class="style1">*</span>日期：
                        <asp:TextBox ID="txt_Date" runat="server" MaxLength="10" onfocus="WdatePicker()"
                            Width="100px"></asp:TextBox>
                        <img align="absMiddle" onclick="WdatePicker({el:$dp.$('txt_Date')})" src="../images/calendar.gif" />
                    </td>
                </tr>
            </table>
            <table id="tb1" cellspacing="0" width="100%">
                <tr>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div align="right">                            
                            <asp:Button ID="btnCompare" runat="server" Text="比對" class="btn" OnClick="btnCompare_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCheckDate" runat="server" Text="日結" class="btn" OnClick="btnCheckDate_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancelCheckDate" runat="server" Text="取消日結" class="btn" OnClick="btnCancelCheckDate_Click" /></div>
                    </td>
                </tr>
                <tr>
                <td>
                <div align="right">    
                <asp:Button ID="btnSub" runat="server" Text="同步替換前與替換后小計檔" class="btn" OnClick="btnSub_Click" OnClientClick="top.ShowWaitDiv('1')" />
                            &nbsp;&nbsp;
                    <asp:Button ID="btnWafer" runat="server" Text="重算晶片規格變化表" class="btn"  OnClientClick="top.ShowWaitDiv('1')" OnClick="btnWafer_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnMaterial" runat="server" Text="重算物料消耗" class="btn"  OnClientClick="top.ShowWaitDiv('1')" OnClick="btnMaterial_Click"  />
                    &nbsp;&nbsp;
                </div>
                </td>
                </tr>
            </table>
            <cc1:GridViewPageingByDB ID="gvCompare" runat="server" SortDescImageUrl="~/images/desc.gif"
                SortAscImageUrl="~/images/asc.gif" PreviousPageText="<" NextPageText=">" LastPageText=">>"
                FirstPageText="<<" EditPageUrl="#" EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl=""
                CssClass="GridView" ColumnsHeaderText="" AutoGenerateColumns="False" AllowSorting="True"
                AllowPaging="True">
                <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                <AlternatingRowStyle CssClass="GridViewAlter" />
                <RowStyle CssClass="GridViewRow" HorizontalAlign="Center" />
                <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                    PreviousPageText="&lt;" Visible="False" />
                <Columns>
                    <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="卡種" DataField="Name" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Type" HeaderText="項目" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Factory_Number" HeaderText="廠商數量" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="System_Number" HeaderText="系統數量" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                </Columns>
            </cc1:GridViewPageingByDB>
                 <cc1:GridViewPageingByDB ID="gvFaReplaceCompare" runat="server" SortDescImageUrl="~/images/desc.gif"
            SortAscImageUrl="~/images/asc.gif" PreviousPageText="<" NextPageText=">" LastPageText=">>"
            FirstPageText="<<" EditPageUrl="#" EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl=""
            CssClass="GridView" ColumnsHeaderText="" AutoGenerateColumns="False" AllowSorting="True"
            AllowPaging="True">
            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
            <AlternatingRowStyle CssClass="GridViewAlter" />
            <RowStyle CssClass="GridViewRow" HorizontalAlign="Center" />
            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                PreviousPageText="&lt;" Visible="False" />
            <Columns>
                    <asp:BoundField DataField="Factory_ShortName_CN" HeaderText="Perso廠" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                    <asp:BoundField HeaderText="卡種" DataField="Name" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>                   
                     <asp:BoundField HeaderText="庫存類別" DataField="Status_Name" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>                   
                    <asp:BoundField DataField="ReplaceNumber" HeaderText="替換前版面廠商異動檔數量" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Number" HeaderText="替換后版面廠商異動檔數量" >
                        <itemstyle horizontalalign="Left" />
                        <headerstyle horizontalalign="Left" />
                    </asp:BoundField>
                </Columns>
        </cc1:GridViewPageingByDB>
        </div>
        <cc1:GridViewPageingByDB ID="gvTotalCompare" runat="server" SortDescImageUrl="~/images/desc.gif"
            SortAscImageUrl="~/images/asc.gif" PreviousPageText="<" NextPageText=">" LastPageText=">>"
            FirstPageText="<<" EditPageUrl="#" EditCellWidth="48px" EditButtonText="編輯" EditButtonImageUrl=""
            CssClass="GridView" ColumnsHeaderText="" AutoGenerateColumns="False" AllowSorting="True"
            AllowPaging="True" >
            <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
            <AlternatingRowStyle CssClass="GridViewAlter" />
            <RowStyle CssClass="GridViewRow" HorizontalAlign="Center" />
            <PagerSettings FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                PreviousPageText="&lt;" Visible="False" />
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="卡種" >
                    <itemstyle horizontalalign="Left" />
                    <headerstyle horizontalalign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Factory_Number" HeaderText="廠商total數量" >
                    <itemstyle horizontalalign="Left" />
                    <headerstyle horizontalalign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="System_Number" HeaderText="系統total數量" >
                    <itemstyle horizontalalign="Left" />
                    <headerstyle horizontalalign="Left" />
                </asp:BoundField>
            </Columns>
        </cc1:GridViewPageingByDB>
   
    </form>
</body>
</html>
