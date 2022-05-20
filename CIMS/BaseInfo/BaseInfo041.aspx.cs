//******************************************************************
//*  作    者：FangBao
//*  功能說明：使用者權限維護
//*  創建日期：2008-08-01
//*  修改日期：2008-08-01 12:00
//*  修改記錄：
//*            □2008-08-01
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
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Text;

public partial class BaseInfo_BaseInfo041 : PageBase
{
    BaseInfo041BL rmManager = new BaseInfo041BL(); //使用者權限維護邏輯


    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbdRoles.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            txtRoleID.Focus();

            //查詢條件
            if (SetConData())
                gvpbdRoles.BindData();
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbdRoles.BindData();
    }

    /// <summary>
    /// 刪除某條角色信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ibtnDelete_Command(object sender, CommandEventArgs e)
    {
        string strRoleID = e.CommandArgument.ToString();

        try
        {
            rmManager.Delete(strRoleID);

            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
        gvpbdRoles.BindData();
    }

    /// <summary>
    /// 新增按鈕處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("BaseInfo041Edit.aspx?ActionType=Add");
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// 綁定資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbdRoles_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtRoleID", txtRoleID.Text);
        inputs.Add("txtRoleName", txtRoleName.Text);


        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlUser = null;

        try
        {
            dstlUser = rmManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlUser != null)//如果查到了資料
            {
                e.Table = dstlUser.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 行資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbdRoles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblRole = (DataTable)gvpbdRoles.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ibtnButton = null;

            string strRoleID = dtblRole.Rows[e.Row.RowIndex]["RoleID"].ToString();

            // 刪除的邦定事件
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnButton.CommandArgument = strRoleID;
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
        }
    }
    #endregion


}
