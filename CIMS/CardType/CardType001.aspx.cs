//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-08-29
//*  修改日期：2008-09-02 16:00
//*  修改記錄：
//*            □2008-08-29
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

public partial class CardType_CardType001 : PageBase
{
    CardType001BL ctManager = new CardType001BL();    

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbCARDGROUP.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            ctManager.dropCard_PurposeBind(drpParam_Name);
            drpParam_Name.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpbCARDGROUP.BindData();
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvpbCARDGROUP.BindData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType001Edit.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCARDGROUP_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("drpParam_Name", drpParam_Name.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlParam = null;

        try
        {
            dstlParam = ctManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlParam != null)//如果查到了資料
            {
                e.Table = dstlParam.Tables[0];//要綁定的資料表
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
    protected void gvpbCARDGROUP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    #endregion
}
