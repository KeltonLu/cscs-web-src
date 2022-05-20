//******************************************************************
//*  作    者：FangBao
//*  功能說明：群組權限各項設定
//*  創建日期：2008-08-01
//*  修改日期：2008-08-01 12:00
//*  修改記錄：
//*            □2008-08-01
//*              1.創建 鮑方
//*******************************************************************

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Text;
using System.Xml;


public partial class BaseInfo_BaseInfo041Edit : PageBase
{
    BaseInfo041BL rmManager = new BaseInfo041BL(); //使用者權限維護邏輯
    private List<string> OldData;  // 列表資料暫存


    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        trvFunction.Attributes.Add("onclick", "SetTree(null,null,event);");

        if (!IsPostBack)
        {
            string strRoleID = Request.QueryString["ID"];

            txtRoleName.Focus();

            if (!StringUtil.IsEmpty(strRoleID))
            {
                // 設置群組名稱
                ROLE rModel = rmManager.GetModel(strRoleID);
                if (rModel != null)
                {
                    txtRoleName.Text = rModel.RoleName;
                    trRole.Visible = true;
                    lblRoleID.Text = rModel.RoleID;
                }
                AjaxValidatorRoleName.QueryInfo = strRoleID;

                if (strRoleID == "CIMS001")
                {
                    btnSubmit1.Enabled = false;
                    btnSubmit2.Enabled = false;
                }
                //AjaxValidatorRoleName.Enabled = false;
            }
            else
                trRole.Visible = false;

            SetTreeView();
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        List<string> listOldData = (List<string>)ViewState["OldData"];
        Dictionary<string, string> dirAdd = new Dictionary<string, string>();
        Dictionary<string, string> dirDeleted = new Dictionary<string, string>();
        Dictionary<string, string> dirTotal = new Dictionary<string, string>();

        FindTreeChange(trvFunction.Nodes[0], listOldData, dirAdd, dirDeleted, dirTotal);

        try
        {
            string strRoleID = Request.QueryString["ID"];

            if (!StringUtil.IsEmpty(strRoleID))        //修改
            {
                rmManager.Updata(txtRoleName.Text, this.Request.QueryString["ID"].Trim(), dirAdd, dirDeleted, dirTotal);

                this.ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo041.aspx?Con=1");
            }
            else                            //增加
            {
                rmManager.Add(txtRoleName.Text.Trim(), dirAdd);

                this.ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo041Edit.aspx");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
   
    #endregion

    #region 欄位/資料補充說明
    /// <summary>
    /// 驗證角色名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AjaxValidatorRoleName_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {

        e.IsAllowSubmit = !rmManager.ContainsRole(e.QueryData.Trim(), e.QueryInfo);//驗證角色名稱是否已存在於資料庫
    }
    #endregion

    #region 樹型功能綁定

    /// <summary>
    /// 初始化TreeView
    /// </summary>
    private void SetTreeView()
    {
        DataSet dstTree = null;

        string strRoleID = Request.QueryString["ID"];

        if (!StringUtil.IsEmpty(strRoleID))
        {
            dstTree = rmManager.GetTreeData(strRoleID);
        }
        else
        {
            dstTree = rmManager.GetTreeData("-1");
        }


        for (int i = 0; i < dstTree.Tables.Count; i++)
        {
            for (int l = 0; l < dstTree.Tables[i].Columns.Count; l++)
            {
                dstTree.Tables[i].Columns[l].ColumnMapping = MappingType.Attribute;
            }
        }
        for (int i = 0; i < dstTree.Tables.Count - 3; i += 2)
        {
            dstTree.Relations.Add(dstTree.Tables[i].Columns["FUNCTIONID"], dstTree.Tables[i + 2].Columns["PARENTFUNCTIONID"]).Nested = true;
        }
        for (int i = 0; i < dstTree.Tables.Count - 1; i += 2)
        {
            dstTree.Relations.Add(dstTree.Tables[i].Columns["FUNCTIONID"], dstTree.Tables[i + 1].Columns["FUNCTIONID"]).Nested = true;
        }

        XmlDataSource xdsTree = new XmlDataSource();
        xdsTree.EnableCaching = false;
        xdsTree.Data = dstTree.GetXml();

        trvFunction.DataSource = xdsTree;
        if (!IsPostBack)
        {
            OldData = new List<string>();
        }
        trvFunction.DataBind();
        if (!IsPostBack)
        {
            ViewState.Add("OldData", OldData);
        }
    }

    /// <summary>
    /// Tree分支資料查找
    /// </summary>
    /// <param name="node"></param>
    /// <param name="listOldData"></param>
    /// <param name="dirAdd"></param>
    /// <param name="dirDeleted"></param>
    private bool FindTreeChange(TreeNode node, List<string> listOldData, Dictionary<string, string> dirAdd, Dictionary<string, string> dirDeleted, Dictionary<string, string> dirTotal)
    {
        if (node.ImageUrl == "~/images/dot.gif")
        {
            if (node.Checked)
            {
                if (!listOldData.Contains(node.Value))
                {
                    dirAdd.Add(node.Value.Trim(), node.Parent.Value.Trim());
                }

                dirTotal.Add(node.Value.Trim(), node.Parent.Value.Trim());
            }
            else
            {
                if (listOldData.Contains(node.Value))
                {
                    dirDeleted.Add(node.Value.Trim(), node.Parent.Value.Trim());
                }
            }

            return node.Checked;
        }
        else
        {
            bool bChecked = false;

            for (int index = 0; index < node.ChildNodes.Count; index++)
            {
                bChecked |= FindTreeChange(node.ChildNodes[index], listOldData, dirAdd, dirDeleted, dirTotal);
            }

            if (bChecked && node.Checked)
            {
                if (node.Parent != null && node.Value != "Root")
                    dirTotal.Add(node.Value.Trim(), node.Parent.Value.Trim());

                if (!listOldData.Contains(node.Value))
                {
                    if (node.Parent != null && node.Value != "Root")
                        dirAdd.Add(node.Value.Trim(), node.Parent.Value.Trim());
                }
            }
            else
            {
                if (listOldData.Contains(node.Value))
                {
                    if (node.Parent != null && node.Value != "Root")
                        dirDeleted.Add(node.Value.Trim(), node.Parent.Value.Trim());
                }
            }

            return bChecked;
        }
    }

    /// <summary>
    /// TreeView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void trvFunction_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
    {
        XmlElement xeNode = (XmlElement)e.Node.DataItem;
        e.Node.SelectAction = TreeNodeSelectAction.None;
        
        if (xeNode.Attributes["ACTIONID"] != null)
        {
            e.Node.Text = xeNode.Attributes["ACTIONNAME"].Value;
            e.Node.Value = xeNode.Attributes["ACTIONID"].Value;
            e.Node.ImageUrl = "~/images/dot.gif";
            if (xeNode.Attributes["ROLEID"].Value == "B")
            {
                if (!IsPostBack)
                {
                    OldData.Add(xeNode.Attributes["ACTIONID"].Value);
                }
                e.Node.Checked = true;

            }
        }
        else if (xeNode.Attributes["FUNCTIONID"] != null)
        {
            e.Node.Text = xeNode.Attributes["FUNCTIONNAME"].Value;
            e.Node.Value = xeNode.Attributes["FUNCTIONID"].Value;
            e.Node.ImageUrl = "~/images/show.gif";

            if (xeNode.Attributes["ROLEID"].Value.Trim() == "B")
            {
                if (!IsPostBack)
                {
                    OldData.Add(xeNode.Attributes["FUNCTIONID"].Value);
                }
                e.Node.Checked = true;
            }
        }
        else
        {
            e.Node.Text = "功能樹";
            e.Node.Value = "Root";
        }
    }
    #endregion

}
