//******************************************************************
//*  作    者：JunWang
//*  功能說明：卡片版面送審檢核管理作業
//*  創建日期：2008-08-29
//*  修改日期：2008-08-29
//*  修改記錄：
//*            □2008-08-29
//*              1.創建 王俊
//*******************************************************************
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

public partial class CardType_CardType003 : PageBase
{
    CardType003BL ctAudit = new CardType003BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbAudit.PageSize = GlobalStringManager.PageSize;
        UrctrlCardTypeSelect1.CardTypeAll = true;

        if (!IsPostBack)
        {
            dropSendCheck_Status.DataSource = ctAudit.getParamSendCheck().Tables[0];
            dropSendCheck_Status.DataBind();
            dropSendCheck_Status.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));


            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    UrctrlCardTypeSelect1.CardType = ((Dictionary<string, object>)Session["Condition"])["CardType_RID"].ToString();
                }
                gvpbAudit.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gvpbAudit.BindData();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType003Add.aspx?ActionType=Add&Serial_Number="+ txtSerial_Number.Text.Trim()+ "&CardType=" + UrctrlCardTypeSelect1.CardType);
    }
    #endregion
    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbAudit_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("CardType_RID", UrctrlCardTypeSelect1.CardType);
        inputs.Add("txtSerial_Number", txtSerial_Number.Text);
        inputs.Add("txtBegin_Date", txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", txtFinish_Date.Text);
        inputs.Add("dropSendCheck_Status", dropSendCheck_Status.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlPersoProject = null;

        try
        {
            dstlPersoProject = ctAudit.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlPersoProject != null)//如果查到了資料
            {
                e.Table = dstlPersoProject.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }

    }
 

    protected void gvpbAudit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[2].Text != "&nbsp;")
            {
                e.Row.Cells[2].Text = Convert.ToDateTime(e.Row.Cells[2].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                e.Row.Cells[3].Text = ctAudit.getParam_Name(e.Row.Cells[3].Text);
                if (e.Row.Cells[2].Text == "1900/01/01")
                {
                    e.Row.Cells[2].Text = "";
                }
                if (e.Row.Cells[4].Text == "1900/01/01")
                {
                    e.Row.Cells[4].Text = "";
                }
            }
        }
    }
    #endregion
}
