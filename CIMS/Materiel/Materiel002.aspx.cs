using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Materiel_Materiel002 : PageBase
{
    Materiel002BL bl = new Materiel002BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbCardExponent.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            if (SetConData())
                gvpbCardExponent.BindData();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCardExponent.BindData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Materiel002Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
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
    protected void gvpbCardExponent_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtName", txtName.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlCardExponent = null;

        try
        {
            dstlCardExponent = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlCardExponent != null)//如果查到了資料
            {
                String safeType = null;

                dstlCardExponent.Tables[0].Columns.Add("Unit_Price1", Type.GetType("System.String"));
                dstlCardExponent.Tables[0].Columns.Add("Wear_Rate1", Type.GetType("System.String"));
                foreach (DataRow dmRow in dstlCardExponent.Tables[0].Rows)
                {
                    // 安全庫存
                    safeType = dmRow["Safe_Type"].ToString();
                    if (safeType.Equals(GlobalString.SafeType.storage))
                    {
                        dmRow["Safe_Type"] = "最低安全庫存 " +
                            ((int)dmRow["Safe_Number"]).ToString("N0") + "（張）";
                    }
                    else if (safeType.Equals(GlobalString.SafeType.days))
                    {
                        dmRow["Safe_Type"] = "安全天數 " + dmRow["Safe_Number"] + "（天）";
                    }

                    // 單價
                    dmRow["Unit_Price1"] = ((decimal)dmRow["Unit_Price"]).ToString("N2");
                    // 損耗率
                    dmRow["Wear_Rate1"] = ((int)dmRow["Wear_Rate"]).ToString("N0") + "%";
                    string Billing_Type = dmRow["Billing_Type"].ToString();
                    if (Billing_Type.Equals("1"))
                    {
                        dmRow["Billing_Type"] = GlobalString.Billing_Type.Card;

                    }
                    else if (Billing_Type.Equals("2"))
                    {
                        dmRow["Billing_Type"] = GlobalString.Billing_Type.Blank;

                    }
                }

                e.Table = dstlCardExponent.Tables[0];//要綁定的資料表
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
    protected void gvpbCardExponent_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    #endregion

}
