//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-09-05
//*  修改日期：2008-09-05 11:30
//*  修改記錄：
//*            □2008-08-29
//*              1.創建 潘秉奕
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Depository_Depository002 : PageBase
{
    Depository002BL depManager = new Depository002BL();

    LoginManager lm = new LoginManager();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbORDERFORM.PageSize = GlobalStringManager.PageSize;
            
        if (!IsPostBack)
        {
            btnAdd.Visible = false;

            Session.Remove("htFactory");
            Session.Remove("detail");
            Session.Remove("delRID");
            Session.Remove("delBlank");
            Session.Remove("drid");
            if (Session["detail"] != null)
                Session.Remove("detail");
            if (Session["monitory"]!=null)
                Session.Remove("monitory");
            depManager.dropPassStatusBind(drpPass_Status);
            drpPass_Status.Items.Insert(0, new ListItem("全部", ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                SetConData();
                gvpbORDERFORM.BindData();
            }

            if (CanUseAction(GlobalString.UserRoleConfig.Commit) && CanUseAction(GlobalString.UserRoleConfig.Add) && CanUseAction(GlobalString.UserRoleConfig.Edit))//經辦登陸（提交、新增和修改權限）
            {
                btnAdd.Visible = true;
            }
            else if (CanUseAction(GlobalString.UserRoleConfig.Pass) && CanUseAction(GlobalString.UserRoleConfig.Reject))//主管登陸(放行和退回權限)
            {
                btnAdd.Visible = false;
            }
            else if (CanUseAction(GlobalString.UserRoleConfig.Add))//判斷新增權限
            {
                btnAdd.Visible = true;
            }
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvpbORDERFORM.BindData();       
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Depository002Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbORDERFORM_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtOrderForm_RID_B", txtOrderForm_RID_B.Text);
        inputs.Add("txtOrderForm_RID_E", txtOrderForm_RID_E.Text);
        inputs.Add("txtOrder_Date_FROM", txtOrder_Date_FROM.Text);
        inputs.Add("txtOrder_Date_TO", txtOrder_Date_TO.Text);
        inputs.Add("txtPass_Date_FROM", txtPass_Date_FROM.Text);
        inputs.Add("txtPass_Date_TO", txtPass_Date_TO.Text);
        inputs.Add("drpPass_Status", drpPass_Status.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlDep = null;

        try
        {
            dstlDep = depManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlDep != null)//如果查到了資料
            {
                e.Table = dstlDep.Tables[0];//要綁定的資料表
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
    protected void gvpbORDERFORM_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtb = (DataTable)gvpbORDERFORM.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                if (e.Row.Cells[2].Text == "1900/01/01")
                    e.Row.Cells[2].Text = "";
            }
            catch
            {
                return;
            }
        }
    }
    #endregion
    
}
