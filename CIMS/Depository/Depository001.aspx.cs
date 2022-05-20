//******************************************************************
//*  作    者：FangBao
//*  功能說明：案件歷程查詢 
//*  創建日期：2008-11-24
//*  修改日期：2008-11-24 12:00
//*  修改記錄：
//*            □2008-11-24
//*              1.創建 鮑方
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

public partial class Depository_Depository001 :PageBase
{
    Depository001BL bl = new Depository001BL();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        TreeMainBind();
    }

    private void TreeMainBind()
    {

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtIncome_DateFrom", txtIncome_DateFrom.Text.Trim());
        inputs.Add("txtIncome_DateTo", txtIncome_DateTo.Text.Trim());

        DataSet dstlTreeData = null;

        tvOrderForm.Nodes.Clear();

        try
        {
            dstlTreeData = bl.GetTreeData(inputs);
           

            if (dstlTreeData != null)//如果查到了資料
            {
                string strStockID = txtStock_RIDYear.Text + txtStock_RID1.Text + txtStock_RID2.Text + txtStock_RID3.Text;

                if (!StringUtil.IsEmpty(strStockID))
                {
                    DataRow[] drowRoots = null;
                    if (strStockID.Length == 8)
                        drowRoots = dstlTreeData.Tables[0].Select("SID like '" + strStockID + "%'");
                    else
                        drowRoots = dstlTreeData.Tables[0].Select("SID='" + strStockID.Substring(0,10) + "'");

                    foreach (DataRow drowRoot in drowRoots)
                    {
                        TreeNode rootnode = new TreeNode();
                        rootnode.Value = drowRoot["SID"].ToString();
                        rootnode.Text = drowRoot["Name"].ToString();
                        rootnode.SelectAction = TreeNodeSelectAction.None;
                        rootnode.ImageUrl = "~/images/file.gif";
                        tvOrderForm.Nodes.Add(rootnode);
                        TreeOrderDetailBind(rootnode.Value, dstlTreeData.Tables[1], rootnode);
                    }
                }
                else
                {
                    foreach (DataRow drowRoot in dstlTreeData.Tables[0].Rows)
                    {
                        TreeNode rootnode = new TreeNode();
                        rootnode.Value = drowRoot["SID"].ToString();
                        rootnode.Text = drowRoot["Name"].ToString();
                        rootnode.SelectAction = TreeNodeSelectAction.None;
                        rootnode.ImageUrl = "~/images/file.gif";
                        tvOrderForm.Nodes.Add(rootnode);
                        TreeOrderDetailBind(rootnode.Value, dstlTreeData.Tables[1], rootnode);
                    }
                }
            }
            if (tvOrderForm.Nodes.Count == 0)
                lbMsg.Visible = true;
            else
                lbMsg.Visible = false;
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    private void TreeOrderDetailBind(string strSID, DataTable dtblMain, TreeNode node)
    {
         DataRow[] drowsDetail=null;
        string strStockID = txtStock_RIDYear.Text + txtStock_RID1.Text + txtStock_RID2.Text + txtStock_RID3.Text;
        if (!StringUtil.IsEmpty(strStockID))
        {
            if (strSID.Length == 10)
            {
                if (strStockID.Length < 12)
                    drowsDetail = dtblMain.Select("PID='" + strSID + "' and SID like '" + strStockID + "%'");
                else
                    drowsDetail = dtblMain.Select("PID='" + strSID + "' and SID='" + strStockID.Substring(0, 12)+"'");
            }
            else if (strSID.Length == 12)
            {
                if (strStockID.Length < 14)
                    drowsDetail = dtblMain.Select("PID='" + strSID + "' and SID like '" + strStockID + "%'");
                else
                    drowsDetail = dtblMain.Select("PID='" + strSID + "' and SID='" + strStockID.Substring(0, 14) + "'");
            }
            else
            {
                if (strStockID.Length < 14)
                    drowsDetail = dtblMain.Select("PID='" + strSID + "' and SID like '" + strStockID + "%'");
                else
                    drowsDetail = dtblMain.Select("PID='" + strSID + "' and SID like '" + strStockID.Substring(0, 14) + "%'","name desc");
            }
           
        }
        else
        {
            drowsDetail = dtblMain.Select("PID='" + strSID + "'");
        }
        
        foreach (DataRow dr in drowsDetail)
        {
            TreeNode treenode = new TreeNode();
            treenode.Value = dr["SID"].ToString().Trim();
            treenode.Text = dr["Name"].ToString().Trim();
            
            treenode.ImageUrl = "~/images/file.gif";
            if (treenode.Value.Length == 14)
            {
                treenode.NavigateUrl = "Depository001Detail.aspx?Type=1&ID=" + treenode.Value;
                treenode.Target = "OrderDetail";
            }
            else if (treenode.Value.Length == 16)
            {
                treenode.NavigateUrl = "Depository001Detail.aspx?Type=3&ID=" + treenode.Value;
                treenode.Target = "OrderDetail";
            }
            else if (treenode.Value.Length == 18)
            {
                treenode.NavigateUrl = "Depository001Detail.aspx?Type=2&ID=" + treenode.Value;
                treenode.Target = "OrderDetail";
            }
            else
            {
                treenode.SelectAction = TreeNodeSelectAction.None;
            }

            node.ChildNodes.Add(treenode);

            TreeOrderDetailBind(treenode.Value, dtblMain, treenode);
        }
    }

    //private void TreeSTOCKBind(string strSID, DataTable dtblMain, TreeNode node)
    //{
    //    DataRow[] drowsDetail = dtblMain.Select("PID='" + strSID + "'");
    //    foreach (DataRow dr in drowsDetail)
    //    {
    //        TreeNode treenode = new TreeNode();
    //        treenode.Text = dr["SID"].ToString().Trim();
    //        treenode.Value = dr["Name"].ToString().Trim();
    //        treenode.SelectAction = TreeNodeSelectAction.None;
    //        node.ChildNodes.Add(treenode);

    //        TreeSTOCKBind(treenode.Value, dtblMain, treenode);
    //    }
    //}
}
