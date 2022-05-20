//******************************************************************
//*  作    者：Minghuige
//*  功能說明：DM種類基本檔查詢頁面
//*  創建日期：2008-08-20
//*  修改日期：2008-08-20 12:00
//*  修改記錄：
//*            □2008-08-20
//*              1.創建 Minghuige
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

public partial class Materiel_Materiel003 : PageBase
{
    Materiel003BL bl = new Materiel003BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbDM.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //從Seesion中獲取已保存的查詢條件            
            if (SetConData())
                gvpbDM.BindData();
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbDM.BindData();
    }

    /// <summary>
    /// 轉向新增預算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Materiel003Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
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
    protected void gvpbDM_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtName", txtName.Text);
        Dictionary<string, object> paramMap = new Dictionary<string, object>();
        //保存查詢條件
        Session["Condition"] = inputs;        
        DataSet dstlDM = null;
        try
        {
            dstlDM = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            String safeType = null;
            foreach (DataRow dmRow in dstlDM.Tables[0].Rows)
            {
                safeType = dmRow["Safe_Type"].ToString();
                if (safeType.Equals(GlobalString.TextType.storage))
                {
                    dmRow["Safe_Type"] = "最低安全庫存 " + StringUtil.SpecAddComma(Convert.ToString(dmRow["Safe_Number"])) +"（張）";
                }
                else if (safeType.Equals(GlobalString.TextType.days))
                {
                    dmRow["Safe_Type"] = "安全天數 " + dmRow["Safe_Number"] + "（天）";
                }
                else if (safeType.Equals(GlobalString.TextType.over))
                {
                    dmRow["Safe_Type"] = "用完為止";
                }
                dmRow["Rate"] = dmRow["Wear_Rate"] + "%";
                dmRow["Price"] = StringUtil.SpecDecimalAddComma(Convert.ToString(dmRow["Unit_Price"]));
                string Billing_Type = dmRow["Billing_Type"].ToString();
                if(Billing_Type.Equals("1")){
                    dmRow["Billing_Type"] = GlobalString.Billing_Type.Card;

                }
                else if (Billing_Type.Equals("2"))
                {
                    dmRow["Billing_Type"] = GlobalString.Billing_Type.Blank;
                
                }
        
            }
            if (dstlDM != null)//如果查到了資料
            {
                e.Table = dstlDM.Tables[0];//要綁定的資料表
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
    protected void gvpbDM_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    #endregion

}
