//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-08-27
//*  修改日期：2008-08-27 14:00
//*  修改記錄：
//*            □2008-08-27
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

public partial class BaseInfo_BaseInfo003 : PageBase
{
    BaseInfo003BL csManager = new BaseInfo003BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbFACTORY.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpbFACTORY.BindData();
        }
    }

    /// <summary>
    /// 廠商新增
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("BaseInfo003Edit.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvpbFACTORY.BindData();
    }


    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbFACTORY_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtFactory_ShortName_CN", txtFactory_ShortName_CN.Text);
        inputs.Add("chkIs_Blank", chkIs_Blank.Checked);
        inputs.Add("chkIs_Perso", chkIs_Perso.Checked);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlFactory = null;

        try
        {
            dstlFactory = csManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlFactory != null)//如果查到了資料
            {
                e.Table = dstlFactory.Tables[0];//要綁定的資料表
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
    protected void gvpbFACTORY_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblcszl = (DataTable)gvpbFACTORY.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                Label lbcszlID = null;

                lbcszlID = (Label)e.Row.FindControl("lblFACTORY_TYPE");

                //載入信息
                DataSet dstFactoryInfo = csManager.LoadFactoryInfoByRID(dtblcszl.Rows[e.Row.RowIndex]["RID"].ToString());

                if (dstFactoryInfo.Tables[0] != null)
                {
                    foreach (DataRow drowCslb in dstFactoryInfo.Tables[0].Rows)
                    {
                        if (drowCslb["Is_Blank"].ToString() == "Y")
                        {
                            lbcszlID.Text += GlobalString.CSLB.BlankFactory + "<br>";
                        }
                        if (drowCslb["Is_Perso"].ToString() == "Y")
                        {
                            lbcszlID.Text += GlobalString.CSLB.PersoFactory + "<br>";
                        }
                    }
                }

                e.Row.Cells[0].Attributes.CssStyle.Add("cursor", "hand");
                e.Row.Cells[0].Attributes["OnClick"] = "window.location='BaseInfo003Edit.aspx?ActionType=Edit&RID=" + dstFactoryInfo.Tables[0].Rows[e.Row.RowIndex]["RID"].ToString() + "'";                
            }
            catch
            {
                return;
            }
        }
    }
    #endregion
  
}
