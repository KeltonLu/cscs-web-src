<%@ Page Language="C#" AutoEventWireup="true" CodeFile="None.aspx.cs" Inherits="None" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>警訓功能</title>
    <link href="css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">

        function setHiddenValue()
        {
            document.getElementById("hidIsSubmit").value = "Y";
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager id="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel id="UpdatePanel1" runat="server">
            <contenttemplate>
            <table border="0" cellpadding="0" cellspacing="2" style="width: 100%">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <font class="PageTitle">警訊功能</font>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <cc1:GridViewPageingByDB ID="gvpbWarning" runat="server" AllowPaging="True" 
                                        DataKeyNames="RID" OnOnSetDataSource="gvpbWarning_OnSetDataSource" AllowSorting="True" AutoGenerateColumns="False"
                                        FirstPageText="<<" LastPageText=">>" NextPageText=">" PreviousPageText="<" CssClass="GridView" SortAscImageUrl="~/images/asc.gif"
                                        SortDescImageUrl="~/images/desc.gif" ColumnsHeaderText="" EditButtonImageUrl=""
                                        EditButtonText="編輯" EditCellWidth="48px" EditPageUrl="#">
                                        <HeaderStyle BackColor="#B9BDAA" Font-Size="Small" ForeColor="Black" />
                                        <AlternatingRowStyle CssClass="GridViewAlter" />
                                        <PagerSettings Visible="False" FirstPageText="&lt;&lt;" LastPageText="&gt;&gt;" NextPageText="&gt;"
                                            PreviousPageText="&lt;" />
                                        <RowStyle CssClass="GridViewRow" />
                                        <Columns>
                                            <asp:BoundField DataField="Warning_Content" HeaderText="警訊內容 ">
                                                <itemstyle width="90%" horizontalalign="Left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="不顯示此提示">
                                                <itemtemplate>
<asp:CheckBox id="cbIs_Show" runat="server" __designer:wfdid="w7" ></asp:CheckBox> 
</itemtemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </cc1:GridViewPageingByDB>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnSubmit" runat="server" Text="確定"  CssClass="btn" OnClientClick="setHiddenValue();" OnClick="btnSubmit_Click"/>
                        <input id="hidIsSubmit" runat="server" type="hidden" value="N" />
                    </td>
                </tr>
            </table>
            </contenttemplate>
        </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
