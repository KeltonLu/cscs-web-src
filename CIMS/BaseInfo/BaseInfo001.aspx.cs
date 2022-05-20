//******************************************************************
//*  作    者：FangBao
//*  功能說明：預算管理作業查詢頁面
//*  創建日期：2008-07-31
//*  修改日期：2008-07-31 12:00
//*  修改記錄：
//*            □2008-07-31
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
using System.Web.UI.HtmlControls;
using System.Text;


public partial class BaseInfo_BaseInfo001 : PageBase
{
    BaseInfo001BL bmManager = new BaseInfo001BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbBudget.PageSize = GlobalStringManager.PageSize;

        UrctrlCardTypeSelect.CardTypeAll = true;

        if (!IsPostBack)
        {
            Session.Remove("BudgetAppend");
            Session.Remove("BudgetID");

            txtBUDGET_ID.Focus();

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    UrctrlCardTypeSelect.CardType = ((Dictionary<string, object>)Session["Condition"])["UrctrlCardTypeSelect"].ToString();
                }
                gvpbBudget.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbBudget.BindData();
    }

    /// <summary>
    /// 轉向新增預算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("BaseInfo001Edit.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion

    #region 欄位/資料補充說明
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbBudget_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtBUDGET_ID", txtBUDGET_ID.Text);
        inputs.Add("txtBudget_Name", txtBudget_Name.Text);
        inputs.Add("txtVALID_DATE_FROM", txtVALID_DATE_FROM.Text);
        inputs.Add("txtVALID_DATE_TO", txtVALID_DATE_TO.Text);
        inputs.Add("UrctrlCardTypeSelect", UrctrlCardTypeSelect.CardType);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlBudget = null;

        try
        {
            dstlBudget = bmManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

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
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbBudget_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblbudget = (DataTable)gvpbBudget.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblbudget.Rows.Count == 0)
                return;

            try
            {
                Label lbBUDGET_ID = (Label)e.Row.FindControl("lbBUDGET_ID");
                Label lbBUDGET_NAME = (Label)e.Row.FindControl("lbBUDGET_NAME");
                Label lbCard_Price = (Label)e.Row.FindControl("lbCard_Price");
                Label lbRemain_Card_Price = (Label)e.Row.FindControl("lbRemain_Card_Price");
                Label lbCard_Num = (Label)e.Row.FindControl("lbCard_Num");
                Label lbRemain_Card_Num = (Label)e.Row.FindControl("lbRemain_Card_Num");
                HyperLink hlBUDGET_ID = (HyperLink)e.Row.FindControl("hlBUDGET_ID");


                lbBUDGET_NAME.Text = dtblbudget.Rows[e.Row.RowIndex]["BUDGET_NAME"].ToString() + "<br>";
                lbCard_Price.Text = Convert.ToDecimal(dtblbudget.Rows[e.Row.RowIndex]["Card_Price"]).ToString("N2") + "<br>";
                lbRemain_Card_Price.Text = Convert.ToDecimal(dtblbudget.Rows[e.Row.RowIndex]["Remain_Card_Price"]).ToString("N2") + "<br>";
                lbCard_Num.Text = Convert.ToInt32(dtblbudget.Rows[e.Row.RowIndex]["Card_Num"]).ToString("N0") + "<br>";
                lbRemain_Card_Num.Text = Convert.ToInt32(dtblbudget.Rows[e.Row.RowIndex]["Remain_Card_Num"]).ToString("N0") + "<br>";
                hlBUDGET_ID.Text = dtblbudget.Rows[e.Row.RowIndex]["BUDGET_ID"].ToString();
                hlBUDGET_ID.NavigateUrl = "BaseInfo001Edit.aspx?ActionType=Edit&RID=" + dtblbudget.Rows[e.Row.RowIndex]["RID"].ToString().Trim();

                //載入預算附加信息
                DataSet dstBudgetInfo = bmManager.LoadBudgetInfoByRID(dtblbudget.Rows[e.Row.RowIndex]["RID"].ToString());

                if (dstBudgetInfo.Tables[0] != null)
                {
                    foreach (DataRow drowBudgetAppend in dstBudgetInfo.Tables[0].Rows)
                    {
                        lbBUDGET_ID.Text += drowBudgetAppend["BUDGET_ID"].ToString() + "<br>";
                        lbBUDGET_NAME.Text += drowBudgetAppend["BUDGET_NAME"].ToString() + "<br>";
                        lbCard_Price.Text += Convert.ToDecimal(drowBudgetAppend["Card_Price"]).ToString("N2") + "<br>";
                        lbRemain_Card_Price.Text += Convert.ToDecimal(drowBudgetAppend["Remain_Card_Price"]).ToString("N2") + "<br>";
                        lbCard_Num.Text += Convert.ToInt32(drowBudgetAppend["Card_Num"]).ToString("N0") + "<br>";
                        lbRemain_Card_Num.Text += Convert.ToInt32(drowBudgetAppend["Remain_Card_Num"]).ToString("N0") + "<br>";
                    }
                }

                ////轉向預算修改
                //e.Row.Attributes["OnClick"] = "window.location='BudgetEdit.aspx?ActionType=Edit&RID=" + dtblbudget.Rows[e.Row.RowIndex]["RID"].ToString() + "'";
                //e.Row.Attributes.CssStyle.Add("cursor", "hand");
            }
            catch
            {
                return;
            }
        }
    }
    #endregion

}
