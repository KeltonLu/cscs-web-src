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


public partial class BaseInfo_BaseInfo006_CardParam : PageBase
{
    BaseInfo006BL bizLogic = new BaseInfo006BL();
    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbParameter.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
                gvpbParameter.BindData();
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        DataTable dtblParam = new DataTable();
        dtblParam.Columns.Add("RID");
        dtblParam.Columns.Add("Param_Name");
        dtblParam.Columns.Add("Param_Comment");

        for (int i = 0; i < gvpbParameter.Rows.Count; i++)
        {
            TextBox txtParam_Name = (TextBox)gvpbParameter.Rows[i].FindControl("txtParam_Name");
            TextBox txtParam_Comment = (TextBox)gvpbParameter.Rows[i].FindControl("txtParam_Comment");
            Label lbParam = (Label)gvpbParameter.Rows[i].FindControl("lbParam");

            if (StringUtil.IsEmpty(txtParam_Name.Text.Trim()))
            {
                ShowMessage(gvpbParameter.Rows[i].Cells[0].Text + "的參數值不能為空");
                return;
            }
            if (lbParam.Text == "%")
            {
                try
                {
                    Convert.ToDecimal(txtParam_Name.Text.Trim());
                }
                catch
                {
                    ShowMessage(gvpbParameter.Rows[i].Cells[0].Text + "的參數值必須為數字");
                    return;
                }
            }
            else
            {
                try
                {
                    Convert.ToInt32(txtParam_Name.Text.Trim());
                }
                catch
                {
                    ShowMessage(gvpbParameter.Rows[i].Cells[0].Text + "的參數值必須為整數");
                    return;
                }
            }

            DataRow drowParam = dtblParam.NewRow();
            drowParam["RID"] = gvpbParameter.DataKeys[i].Value.ToString();
            drowParam["Param_Name"] = txtParam_Name.Text.Trim() + lbParam.Text;
            drowParam["Param_Comment"] = txtParam_Comment.Text;

            dtblParam.Rows.Add(drowParam);
        }
    

        try
        {
            bizLogic.ParamEdit(dtblParam);
            gvpbParameter.BindData();
            ShowMessage("保存成功");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    #endregion

    #region 列表資料綁定
    protected void gvpbParameter_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtPARAM_NAME", "");
        inputs.Add("ParamType_Code", GlobalString.ParameterType.CardParam);

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
    protected void gvpbParameter_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblParameter = (DataTable)gvpbParameter.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblParameter.Rows.Count == 0)
                return;

            TextBox txtParam_Name = (TextBox)e.Row.FindControl("txtParam_Name");
            TextBox txtParam_Comment = (TextBox)e.Row.FindControl("txtParam_Comment");
            Label lbParam = (Label)e.Row.FindControl("lbParam");

            txtParam_Name.Text = dtblParameter.Rows[e.Row.RowIndex]["Param_Name"].ToString().Substring(0, dtblParameter.Rows[e.Row.RowIndex]["Param_Name"].ToString().Length - 1);
            txtParam_Comment.Text = dtblParameter.Rows[e.Row.RowIndex]["Param_Comment"].ToString();
            lbParam.Text = dtblParameter.Rows[e.Row.RowIndex]["Param_Name"].ToString().Substring(dtblParameter.Rows[e.Row.RowIndex]["Param_Name"].ToString().Length - 1);

        }
    }
    #endregion
}
