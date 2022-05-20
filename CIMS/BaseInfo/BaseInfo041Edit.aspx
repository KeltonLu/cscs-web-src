<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseInfo041Edit.aspx.cs"
    Inherits="BaseInfo_BaseInfo041Edit" %>

<%@ Register Assembly="ControlLibrary" Namespace="ControlLibrary" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>角色權限設定</title>
    <link href="../css/Global.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="../JS/CommJS.js"></script>

    <script language="javascript" type="text/javascript">
    function FindCheckBox(table)
    {
        // 一個tale中只有一行遍歷獲取返回checkbox
        if (table != null && table.rows != null && table.rows.length > 0)
        {
           for(var i=0;i<table.rows[0].cells.length;i++)
           {
               // 獲取table中的標籤
               var cell = table.rows[0].cells[i];

               // 判斷是否為checkbox，是則返回判斷是否為checkbox
              if(cell.childNodes.length>0 && cell.childNodes[0].nodeName=='INPUT' && cell.childNodes[0].type=='checkbox')
              {
                    return cell.childNodes[0];
              }
           } 
        }
      return null; 
    }
    
    function FindParentCheckBox(Obj)
    {
        var returnValue=null;
        try
        {
            // 獲取該筆的tabe，根據table遍歷獲取返回checkbox
            returnValue=FindCheckBox(Obj.parentNode.parentNode.parentNode.parentNode.parentNode.previousSibling);
        }
        catch(e){}
        return returnValue;
    }
    
    function SetTree(obj, _value,e)
    {
        if (obj == null)
        {
            var eventobj = e|| window.event;
            //var che= event.srcElement;
            var che = eventobj.srcElement ? eventobj.srcElement:eventobj.target;

            if(che.nodeName=='INPUT' && che.type=='checkbox')
            {
                // 根據checkbox 獲取div例：trvFunctionn11Nodes
                // checkbox對象：<input name="trvFunctionn11CheckBox" id="trvFunctionn11CheckBox" type="checkbox" CHECKED="checked" value="on"/>
                var t = che.parentNode.parentNode.parentNode.parentNode;

                // 獲取div對象：trvFunctionn11Nodes
                var chds=t.nextSibling;
                if(chds!=null)
                {
                    if(chds.nodeName=='DIV')
                    {
                        for(var i=0;i<chds.childNodes.length;i+=2)
                        {
                            // add by tree 20160929 VS2015,IE11用childNodes獲取不到table改用children獲取
                            //var temp=FindCheckBox(chds.childNodes[i]);
                            var temp = FindCheckBox(chds.children[i]);
                            if(temp!=null)
                            {
                                SetTree(temp,che.checked,e);
                            } 
                        }
                    }
                }
                if(che.checked)
                {
                    var tp=FindParentCheckBox(che);
                 
                    while(tp!=null)
                    {
                        tp.checked=true;
                        tp=FindParentCheckBox(tp);
                    }
                }
            }
        } 
       else
       {
      
            obj.checked=_value;
            var t=obj.parentNode.parentNode.parentNode.parentNode;
            var chds=t.nextSibling;
         
            if(chds!=null)
            {
                for(var i=0;i<chds.childNodes.length;i+=2)
                {
                    // add by tree 20160929 VS2015,IE11用childNodes獲取不到table改用children獲取
                    //var temp=FindCheckBox(chds.childNodes[i]);
                    var temp = FindCheckBox(chds.children[i]);

                    if(temp!=null)
                    {
                        SetTree(temp,_value,e);
                    } 
                }
            } 
        }  
    }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                            <td class="PageTitle">
                                角色權限設定
                            </td>
                            <td align="right">
                                <asp:Button CssClass="btn" ID="btnSubmit1" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                                <input id="btnCancel" class="btn" onclick="returnform('BaseInfo041.aspx')" type="button"
                                    value="取消" />
                            </td>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%;">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                            <tr id="trRole" runat="server">
                                <td style="width: 100%" bgcolor="#B9BDAA">
                                    角色編號：
                                    <asp:Label ID="lblRoleID" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%" bgcolor="#B9BDAA">
                                    <font color="red">*</font><strong>角色名稱：</strong>
                                    <asp:TextBox ID="txtRoleName" MaxLength="50" onblur="LimitLengthCheck(this,50);"
                                        onkeyup="LimitLengthCheck(this,50);" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtRoleName"
                                        Display="Dynamic" ErrorMessage="角色名稱不可為空白&nbsp;">*</asp:RequiredFieldValidator>&nbsp;
                                    <cc1:AjaxValidator ID="AjaxValidatorRoleName" runat="server" ControlToValidate="txtRoleName"
                                        OnOnAjaxValidatorQuest="AjaxValidatorRoleName_OnAjaxValidatorQuest" ErrorMessage="角色名稱不能重複">角色名稱不能重複</cc1:AjaxValidator></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 90%; background-color: #e4e4e4">
                        <asp:TreeView ID="trvFunction" runat="server" ShowCheckBoxes="All" ShowLines="True"
                            Width="100%" OnTreeNodeDataBound="trvFunction_TreeNodeDataBound" CssClass="TreeView"
                            ExpandDepth="1">
                            <NodeStyle ForeColor="Black" CssClass="TreeView" />
                            <LeafNodeStyle CssClass="TreeView" />
                        </asp:TreeView>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%;" align="right">
                        <asp:Button CssClass="btn" ID="btnSubmit2" runat="server" Text="確定" OnClick="btnSubmit_Click" />&nbsp;&nbsp;
                        <input id="btnCancel1" class="btn" onclick="returnform('BaseInfo041.aspx')" type="button"
                            value="取消" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </form>
</body>
</html>
