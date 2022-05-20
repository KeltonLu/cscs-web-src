//******************************************************************
//*  作    者：FangBao
//*  功能說明：系統管理模組
//*  創建日期：2008-11-24
//*  修改日期：2008-11-24 12:00
//*  修改記錄：
//*            □2008-11-24
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

public partial class SysInfo_SysInfo001 : PageBase
{
    SysInfo001BL bl = new SysInfo001BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbBudget.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            DataSet dstDrop = bl.LoadDropData();
            if (dstDrop != null)
            {
                dropParam_Code.DataTextField = "PARAM_NAME";
                dropParam_Code.DataValueField = "PARAM_CODE";
                dropParam_Code.DataSource = dstDrop.Tables[0];
                dropParam_Code.DataBind();
                dropParam_Code.Items.Insert(0, new ListItem("全部", ""));

                dropUserID.DataTextField = "UserName";
                dropUserID.DataValueField = "UserID";
                dropUserID.DataSource = dstDrop.Tables[1];
                dropUserID.DataBind();
                dropUserID.Items.Insert(0, new ListItem("全部", ""));

            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbBudget.BindData();
    }
    protected void gvpbBudget_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtOperate_DateFrom", txtOperate_DateFrom.Text);
        inputs.Add("txtOperate_DateTo", txtOperate_DateTo.Text);
        inputs.Add("dropParam_Code", dropParam_Code.SelectedValue);
        inputs.Add("dropUserID", dropUserID.SelectedValue);
        

        DataSet dstlAH = null;

        try
        {
            dstlAH = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlAH != null)//如果查到了資料
            {
                e.Table = dstlAH.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }
    protected void gvpbBudget_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
}
