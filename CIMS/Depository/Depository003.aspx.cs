//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片入庫查詢
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 12:00
//*  修改記錄：
//*            □2008-09-02
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

public partial class Depository_Depository003 : PageBase
{
    Depository003BL BL = new Depository003BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        // 20170426 修改true-->false
        UctrlCardType.ChangeName = false;
        //UctrlCardType.RightMaxLenght = 10;
        this.gvpbImportStock.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            Session.Remove("dtblImp");

            txtStock_RIDYear.Focus();

            DataSet dstFactoryData = BL.GetFactoryData();

            dropBlank_Factory_RID.DataSource = dstFactoryData.Tables[0];
            dropBlank_Factory_RID.DataBind();
            dropBlank_Factory_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            dropPerso_Factory_RID.DataSource = dstFactoryData.Tables[1];
            dropPerso_Factory_RID.DataBind();
            dropPerso_Factory_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    UctrlCardType.SetRightItem = (DataTable)(((Dictionary<string, object>)Session["Condition"])["UctrlCardType"]);
                }
                gvpbImportStock.BindData();
            }
            else
                Session.Remove("Condition");

            
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //if (UctrlCardType.GetRightItem.Rows.Count == 0)
        //{
        //    ShowMessage("請選擇版面簡稱");
        //}
        gvpbImportStock.BindData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage("今天已經日結，不可新增入庫信息");
            return;
        }
        Response.Redirect(string.Concat("Depository003ImpAuto.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

    protected void btnAddMans_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage("今天已經日結，不可新增入庫信息");
            return;
        }
        Response.Redirect(string.Concat("Depository003Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

    /// <summary>
    /// 多筆刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDel_Click(object sender, EventArgs e)
    {
        string strStock_RID = "";

        foreach (GridViewRow gvRow in gvpbImportStock.Rows)
        {   
            if ((gvRow.Cells[0].FindControl("chkDel") as CheckBox).Checked)
            {
                strStock_RID += (gvRow.Cells[2].FindControl("hlStock_RID") as HyperLink).Text.Trim() + ";";
            }
        }
        strStock_RID = strStock_RID.Trim(';');

        if (string.IsNullOrEmpty(strStock_RID))
        {
            ShowMessage("請至少點選一筆資料");
        }
        else
        {
            BL.MultiDelete(strStock_RID.Trim(';'));
            ShowMessage("刪除成功");
            gvpbImportStock.BindData();
        }
        return;
    }

    #endregion

    #region 列表資料綁定.
    protected void gvpbImportStock_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtStock_RIDYear", txtStock_RIDYear.Text.Trim());
        inputs.Add("txtStock_RID1", txtStock_RID1.Text.Trim());
        inputs.Add("txtStock_RID2", txtStock_RID2.Text.Trim());
        inputs.Add("txtStock_RID3", txtStock_RID3.Text.Trim());
        inputs.Add("UctrlCardType", UctrlCardType.GetRightItem);
        inputs.Add("dropBlank_Factory_RID", dropBlank_Factory_RID.SelectedValue.Trim());
        inputs.Add("dropPerso_Factory_RID", dropPerso_Factory_RID.SelectedValue.Trim());
        inputs.Add("txtOrder_DateFrom", txtOrder_DateFrom.Text.Trim());
        inputs.Add("txtOrder_DateTo", txtOrder_DateTo.Text.Trim());
        inputs.Add("txtIncome_DateFrom", txtIncome_DateFrom.Text.Trim());
        inputs.Add("txtIncome_DateTo", txtIncome_DateTo.Text.Trim());
        inputs.Add("dropStatus", dropStatus.SelectedValue.Trim());
        inputs.Add("radlOrderType", radlOrderType.SelectedValue.Trim());


        //保存查詢條件
        Session["Condition"] = inputs;

        //if (UctrlCardType.GetRightItem.Rows.Count == 0)
        //{
        //    gvpbImportStock.DataSource = null;
        //    gvpbImportStock.DataBind();
        //    return;
        //}

        DataSet dstlBudget = null;

        try
        {
            dstlBudget = BL.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlBudget != null)//如果查到了資料
            {
                e.Table = dstlBudget.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 因添加 刪除 欄位, 因此 GridView的列索引調整
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbImportStock_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblImport = (DataTable)gvpbImportStock.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblImport.Rows.Count == 0)
                return;

            try
            {
                Label lblStatus = null;

                lblStatus = (Label)e.Row.FindControl("lblCase_Status");

                CheckBox chkDel = e.Row.FindControl("chkDel") as CheckBox;

                // 結案,請款, 刪除按鈕不可用
                if (dtblImport.Rows[e.Row.RowIndex]["Case_Status"].ToString() == "Y" || dtblImport.Rows[e.Row.RowIndex]["Is_AskFinance"].ToString() == "Y" || dtblImport.Rows[e.Row.RowIndex]["Is_Check"].ToString() == "Y")
                {
                    chkDel.Enabled = false;
                }

                if (dtblImport.Rows[e.Row.RowIndex]["Case_Status"].ToString() == "Y")
                {
                    lblStatus.Text = "已結案";
                }
                else
                {
                    lblStatus.Text = "未結案";
                }

                if (dtblImport.Rows[e.Row.RowIndex]["Stock_RID"].ToString().Substring(8, 4) == "9999")
                {
                    lblStatus.Text = "已結案";
                }
                HyperLink hlReportID = null;
                hlReportID = (HyperLink)e.Row.FindControl("hlReport_RID");
                hlReportID.Text = dtblImport.Rows[e.Row.RowIndex]["Report_RID"].ToString();
                hlReportID.NavigateUrl = "#";
                hlReportID.Attributes.Add("onclick", "window.open('Depository003ImpPrint.aspx?RID=" + dtblImport.Rows[e.Row.RowIndex]["RID"].ToString()+"','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');");

                HyperLink hlStock_RID = null;
                hlStock_RID = (HyperLink)e.Row.FindControl("hlStock_RID");
                hlStock_RID.Text = dtblImport.Rows[e.Row.RowIndex]["Stock_RID"].ToString();
                hlStock_RID.NavigateUrl = "#";
                hlStock_RID.Attributes.Add("onclick", "window.location.href = 'Depository003Mod.aspx?ActionType=Edit&amp;RID=" + dtblImport.Rows[e.Row.RowIndex]["Stock_RID"].ToString() + "';");

                if (e.Row.Cells[12].Text != "&nbsp;")
                {
                    e.Row.Cells[12].Text = e.Row.Cells[12].Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");
                }

                if (e.Row.Cells[4].Text != "&nbsp;")
                {
                    e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
            }
            catch
            {
                return;
            }
        }
    }
    #endregion
}
