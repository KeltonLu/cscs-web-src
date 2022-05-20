//******************************************************************
//*  作    者：WangxiaoYan
//*  功能說明：卡種狀況設定頁面
//*  創建日期：2008-10-7
//*  修改日期： 
//*  修改記錄：
//*            □2008-10-7
//*              1.創建 王曉燕
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

public partial class BaseInfo_BaseInfo011 : PageBase
{
    BaseInfo011BL bizLogic = new BaseInfo011BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbCardTypeStatus.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpbCardTypeStatus.BindData();
        }
       
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCardTypeStatus.BindData();
    }

    /// <summary>
    /// 轉向新增預算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("BaseInfo011Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion  

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardTypeStatus_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtStatus_Name", txtStatus_Name.Text);
        //保存查詢條件
        Session["Condition"] = inputs;
        DataSet dstlCardTypeStatus = null;
        try
        {
            dstlCardTypeStatus = bizLogic.list(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlCardTypeStatus != null)//如果查到了資料
            {
                e.Table = dstlCardTypeStatus.Tables[0];//要綁定的資料表
                
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
  
    }        
    
    #endregion

    protected void gvpbCardTypeStatus_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCardTypeStatus = (DataTable)gvpbCardTypeStatus.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblCardTypeStatus.Rows.Count == 0)
                return;

            if (e.Row.Cells[2].Text == " ")
            {
                e.Row.Cells[2].Text = "無";
            }
        }
    }
}
