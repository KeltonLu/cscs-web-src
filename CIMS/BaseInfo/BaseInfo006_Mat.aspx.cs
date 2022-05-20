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

public partial class BaseInfo_BaseInfo006_Mat : PageBase
{
    BaseInfo006BL bizLogic = new BaseInfo006BL();
    public string strType = "";

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbParameter.PageSize = GlobalStringManager.PageSize;
        strType = Request.QueryString["Type"];

        if (!IsPostBack)
        {
            if (strType == "1")
                lbTitle.Text = "代碼管理-卡片用途群組";
            else if (strType == "2")
                lbTitle.Text = "代碼設定-合約級距";
            else if (strType == "3")
                lbTitle.Text = "紙品物料設定-庫存原因設定";

            //從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpbParameter.BindData();
        }

    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbParameter.BindData();
    }

    /// <summary>
    /// 轉向新增預算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        // Legend 2017/05/15 注釋此段, 因調整 #7 
        // 基本設定→紙品物料設定→庫存原因設定→增加顯示參數編號09 參數名稱:移轉入→”新增”選項需移除判斷邏輯,因條件邏輯僅能新增至04
        //ShowMessage("請注意，庫存原因已有參數編號01到04的資料，無法新增!");
        //return;

        string strURL = "";

        

        if (strType == "1")
            strURL += "&Type=1";
        else if (strType == "2")
            strURL += "&Type=2";
        else if (strType == "3")
            strURL += "&Type=3";

        Response.Redirect(string.Concat("BaseInfo006_MatAdd.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD, strURL));
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbParameter_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {

        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtPARAM_NAME", txtPARAM_NAME.Text);
        if (strType == "1")
            inputs.Add("ParamType_Code", GlobalString.ParameterType.Use);
        else if (strType == "2")
            inputs.Add("ParamType_Code", GlobalString.ParameterType.CardType);
        else if (strType == "3")
            inputs.Add("ParamType_Code", GlobalString.ParameterType.MatType1);

        //保存查詢條件
        Session["Condition"] = inputs;
        DataSet dstlParam = null;
        try
        {
            dstlParam = bizLogic.list(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlParam != null)//如果查到了資料
            {
                e.Table = dstlParam.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }

    #endregion

    protected void gvpbParameter_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblParameter = (DataTable)gvpbParameter.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblParameter.Rows.Count == 0)
                return;

            HyperLink hlParam_Name = (HyperLink)e.Row.FindControl("hlParam_Name");

            hlParam_Name.Text = dtblParameter.Rows[e.Row.RowIndex]["Param_Code"].ToString().PadLeft(2,'0');
            hlParam_Name.NavigateUrl = "BaseInfo006_MatMod.aspx?Type=" + strType + "&ActionType=Edit&RID=" + dtblParameter.Rows[e.Row.RowIndex]["RID"].ToString().Trim();

        }

    }
}
