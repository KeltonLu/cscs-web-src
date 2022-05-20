//******************************************************************
//*  作    者：FangBao
//*  功能說明：使用者資料維護的查詢頁面
//*  創建日期：2008-07-31
//*  修改日期：2008-07-31 12:00
//*  修改記錄：
//*            □2008-07-31
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

public partial class BaseInfo_BaseInfo042 : PageBase
{
    BaseInfo042BL umManager = new BaseInfo042BL();//用戶管理邏輯
    BaseInfo041BL rmManager = new BaseInfo041BL();//角色管理邏輯

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbUsers.PageSize = GlobalStringManager.PageSize;
        gvpbUsers.Columns[4].Visible = false;

        if (!IsPostBack)
        {
            txtUserID.Focus();

            //角色下拉框綁定
            dropRole.DataSource = umManager.GetPageDatas().Tables[0];
            dropRole.DataBind();
            dropRole.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //查詢條件
            if (SetConData())
                gvpbUsers.BindData();
        }

        btnAdd.Visible = false;
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbUsers.BindData();//查詢並顯示資料
    }

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("BaseInfo042Edit.aspx?ActionType=Add");
    }


    /// <summary>
    /// 刪除某條用戶信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ibtnDelete_Command(object sender, CommandEventArgs e)
    {
        string strUserID = e.CommandArgument.ToString();
        try
        {
            umManager.Delete(strUserID);
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
            gvpbUsers.BindData();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    #endregion

    #region 欄位/資料補充說明
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbUsers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblRole = (DataTable)gvpbUsers.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ibtnButton = null;
            Label lblRoleName = null;
            HyperLink hlUserID = (HyperLink)e.Row.FindControl("hlUserID");

            hlUserID.Text = dtblRole.Rows[e.Row.RowIndex]["UserID"].ToString().Trim();
            //hlUserID.NavigateUrl = "BaseInfo042Edit.aspx?ActionType=Edit&ID=" + HttpUtility.UrlEncode(dtblRole.Rows[e.Row.RowIndex]["UserID"].ToString().Trim());


            string strUserID = dtblRole.Rows[e.Row.RowIndex]["UserID"].ToString();

            // 刪除的邦定事件
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnButton.CommandArgument = strUserID;
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            if (hlUserID.Text == "admin")
                ibtnButton.Visible = false;


            //角色綁定
            lblRoleName = (Label)e.Row.FindControl("lblRoleName");
            DataSet dstlRole = rmManager.SearchRole(strUserID);


            if (dstlRole.Tables[0] != null)
            {
                foreach (DataRow drowRole in dstlRole.Tables[0].Rows)
                {
                    lblRoleName.Text += drowRole["RoleName"].ToString() + "<br>";
                }
            }
        }
    }

    /// <summary>
    /// GridView綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbUsers_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtUserID", txtUserID.Text);
        inputs.Add("txtUserName", txtUserName.Text);
        inputs.Add("dropRole", dropRole.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlUser = null;

        try
        {
            dstlUser = umManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

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
    #endregion

    protected void btnLDAP_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet dstUser = LADPCheckManager.GetLDAPAuth();

            umManager.AddLDAP(dstUser);

            ShowMessage("導入成功");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
}
