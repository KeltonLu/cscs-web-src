<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QReport.aspx.cs" Inherits="FileUpload_QReport" %>

<!DOCTYPE html>
<script runat="server">
</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <table>
        <tr>
        <td>Connect String&nbsp;</td>
        <td style="width: 700px" colspan="2"><asp:TextBox ID="txtConnect" runat="server" Width="100%"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="height: 305px">SQL Command&nbsp;</td>
            <td style="width: 600px; height: 305px">
                <asp:TextBox ID="txtCommand" runat="server" Height="303px" Width="95%" TextMode="MultiLine"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnQuery"  runat="server" Text="Query" OnClick="btnQuery_Click" />
                <asp:Button ID="btnExecute" runat="server" Text="Execute" OnClick="btnExecute_Click"/>
                <asp:Button ID="btnExcel" runat="server" Text="Excel" OnClick="btnExcel_Click"/>
                <asp:Button ID="btnCsv" runat="server" Text="CSV" OnClick="btnCsv_Click"/>
            </td>
        </tr>
        <tr>
            <td colspan="3" >
                <asp:Label runat="server" ID="label"></asp:Label>
            </td>
        </tr>
        </table>
    </div>
          <asp:GridView ID="gridViewResult" runat="server"></asp:GridView>
    </form>
</body>
</html>
