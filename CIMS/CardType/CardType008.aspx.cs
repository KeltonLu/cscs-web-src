//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 9:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 潘秉奕
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

public partial class CardType_CardType008 : PageBase
{
    CardType008BL ctManager = new CardType008BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbWAFER.PageSize = GlobalStringManager.PageSize;

        urctrlCardTypeGroupSelect.CardTypeAll = true;

        if (!IsPostBack)
        {
            ctManager.dropFactoryBind(drpFactory_shortname_cn);
            drpFactory_shortname_cn.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                SetConData();
                gvpbWAFER.BindData();
            }
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvpbWAFER.BindData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType008Edit.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbWAFER_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtWafer_Name", txtWafer_Name.Text);

        //inputs.Add("txtWafer_Capacity", txtWafer_Capacity.Text);
        inputs.Add("txtBG", txtBG.Text.Replace(",", ""));
        inputs.Add("txtEnd", txtEnd.Text.Replace(",", ""));

        inputs.Add("txtMark", txtMark.Text);
        inputs.Add("txtWafer_Factory", txtWafer_Factory.Text);
        inputs.Add("drpFactory_shortname_cn", drpFactory_shortname_cn.SelectedValue);
        inputs.Add("UrctrlCardTypeSelect", urctrlCardTypeGroupSelect.CardType);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlFactory = null;

        try
        {
            dstlFactory = ctManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlFactory != null)//如果查到了資料
            {
                e.Table = dstlFactory.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbWAFER_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblwafer = (DataTable)gvpbWAFER.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                Label lblFACTORY_NAME = null;

                Label lblIS_USING = null;

                Label lblWafer_Capacity = null;

                Label lblROM_Capacity = null;

                lblROM_Capacity = (Label)e.Row.FindControl("lblROM_Capacity");

                lblWafer_Capacity = (Label)e.Row.FindControl("lblWafer_Capacity");

                lblFACTORY_NAME = (Label)e.Row.FindControl("lblFACTORY_NAME");

                lblIS_USING = (Label)e.Row.FindControl("lblIS_USING");

                if (dtblwafer.Rows[e.Row.RowIndex]["ROM_Capacity"].ToString() != "")
                {
                    lblROM_Capacity.Text += dtblwafer.Rows[e.Row.RowIndex]["ROM_Capacity"].ToString() + "K";
                }

                if (dtblwafer.Rows[e.Row.RowIndex]["Wafer_Capacity"].ToString() != "")
                {
                    lblWafer_Capacity.Text += dtblwafer.Rows[e.Row.RowIndex]["Wafer_Capacity"].ToString() + "K";
                }

                if (dtblwafer.Rows[e.Row.RowIndex]["is_using"].ToString() == "Y")
                {
                    lblIS_USING.Text = "使用中";
                }
                else if (dtblwafer.Rows[e.Row.RowIndex]["is_using"].ToString() == "N")
                {
                    lblIS_USING.Text = "未使用";
                }
                if (e.Row.Cells[12].Text != "&nbsp;")
                {
                    e.Row.Cells[12].Text = Convert.ToDateTime(e.Row.Cells[12].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
                //載入信息
                DataSet dstFactoryInfo = ctManager.LoadFactoryByWRID(dtblwafer.Rows[e.Row.RowIndex]["RID"].ToString());

                if (dstFactoryInfo.Tables[0] != null)
                {
                    foreach (DataRow drowCslb in dstFactoryInfo.Tables[0].Rows)
                    {
                        lblFACTORY_NAME.Text += drowCslb["factory_shortname_cn"].ToString() + "<br>";
                    }
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
