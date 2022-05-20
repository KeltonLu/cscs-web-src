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

public partial class BaseInfo_BaseInfo008 : PageBase
{
    BaseInfo008BL bizLogic = new BaseInfo008BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbMateriel.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpbMateriel.BindData();
        }

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbMateriel.BindData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("BaseInfo008Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion

    protected void gvpbMateriel_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtName", txtName.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlMateriel = null;

        try
        {
            dstlMateriel = bizLogic.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);           
            if (dstlMateriel != null)//如果查到了資料
            {
                e.Table = dstlMateriel.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }


}
