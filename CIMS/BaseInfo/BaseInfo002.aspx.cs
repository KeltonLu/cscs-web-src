//******************************************************************
//*  作    者：FangBao
//*  功能說明：合約查詢管理
//*  創建日期：2008-09-16
//*  修改日期：2008-09-16 12:00
//*  修改記錄：
//*            □2008-09-16
//*              1.創建 鮑方
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

public partial class BaseInfo_BaseInfo002 : PageBase
{
    BaseInfo002BL BL = new BaseInfo002BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        UrctrlCardTypeSelect.CardTypeAll = true;
        this.gvpbAgreement.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            txtAgreement_Code.Focus();

            Session.Remove("Agreement");

            dropFactory_RID.DataSource = BL.GetFacotry();
            dropFactory_RID.DataBind();
            dropFactory_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            UrctrlCardTypeSelect.CardTypeAll = true;


            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    UrctrlCardTypeSelect.CardType = ((Dictionary<string, object>)Session["Condition"])["UrctrlCardTypeSelect"].ToString();
                }
                gvpbAgreement.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (!StringUtil.IsEmpty(txtCard_Number1.Text) && !StringUtil.IsEmpty(txtCard_Number2.Text))
        {
            if (int.Parse(txtCard_Number1.Text.Replace(",","")) > int.Parse(txtCard_Number2.Text.Replace(",","")))
            {
                ShowMessage("卡數迄不能小於卡數起");
                return;
            }
        }
        gvpbAgreement.BindData();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("BaseInfo002Add.aspx?ActionType=Add");
    }

    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbAgreement_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtAgreement_Code", txtAgreement_Code.Text);
        inputs.Add("txtAgreement_Name", txtAgreement_Name.Text);
        inputs.Add("dropFactory_RID", dropFactory_RID.SelectedValue);
        inputs.Add("txtCard_Number1", txtCard_Number1.Text);
        inputs.Add("txtCard_Number2", txtCard_Number2.Text);
        inputs.Add("txtBegin_Time", txtBegin_Time.Text);
        inputs.Add("txtEnd_Time", txtEnd_Time.Text);
        inputs.Add("UrctrlCardTypeSelect", UrctrlCardTypeSelect.CardType);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlBudget = null;

        try
        {
            dstlBudget = BL.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlBudget != null)//如果查到了資料
            {
                e.Table = dstlBudget.Tables[0];//要綁定的資料表
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
    protected void gvpbAgreement_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblAgreement = (DataTable)gvpbAgreement.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblAgreement.Rows.Count == 0)
                return;

            HyperLink hlAgreement_Code = (HyperLink)e.Row.FindControl("hlAgreement_Code");
            Label lbAgreement_Code = (Label)e.Row.FindControl("lbAgreement_Code");
            Label lbAgreement_Name = (Label)e.Row.FindControl("lbAgreement_Name");
            Label lbCard_Number = (Label)e.Row.FindControl("lbCard_Number");
            Label lbCard_Number_r = (Label)e.Row.FindControl("lbCard_Number_r");
            Label lbTime = (Label)e.Row.FindControl("lbTime");
            Label lbFactory_Name = (Label)e.Row.FindControl("lbFactory_Name");

            hlAgreement_Code.Text += dtblAgreement.Rows[e.Row.RowIndex]["Agreement_Code"].ToString().Trim();
            hlAgreement_Code.NavigateUrl = "BaseInfo002Mod.aspx?ActionType=Edit&RID=" + dtblAgreement.Rows[e.Row.RowIndex]["RID"].ToString().Trim();
            lbAgreement_Name.Text += dtblAgreement.Rows[e.Row.RowIndex]["Agreement_Name"].ToString().Trim() + "<br>";
            lbCard_Number.Text += int.Parse(dtblAgreement.Rows[e.Row.RowIndex]["Card_Number"].ToString()).ToString("N0") + "<br>";
            lbCard_Number_r.Text += int.Parse(dtblAgreement.Rows[e.Row.RowIndex]["Remain_Card_Num"].ToString()).ToString("N0") + "<br>";
            lbTime.Text += Convert.ToDateTime(dtblAgreement.Rows[e.Row.RowIndex]["Begin_Time"].ToString()).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + " ~ " + Convert.ToDateTime(dtblAgreement.Rows[e.Row.RowIndex]["End_Time"].ToString()).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "<br>";
            lbFactory_Name.Text += dtblAgreement.Rows[e.Row.RowIndex]["Factory_ShortName_CN"].ToString() + "<br>";

            DataTable dtbl = BL.GetListDetail(dtblAgreement.Rows[e.Row.RowIndex]["Agreement_Code"].ToString());
            foreach (DataRow drow in dtbl.Rows)
            {
                lbAgreement_Code.Text += drow["Agreement_Code"].ToString().Trim() + "<br>";
                lbAgreement_Name.Text += drow["Agreement_Name"].ToString().Trim() + "<br>";
                lbCard_Number.Text += int.Parse(drow["Card_Number"].ToString()).ToString("N0") + "<br>";
                lbCard_Number_r.Text += int.Parse(drow["Remain_Card_Num"].ToString()).ToString("N0") + "<br>";
                lbTime.Text += Convert.ToDateTime(drow["Begin_Time"].ToString()).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + " ~ " + Convert.ToDateTime(drow["End_Time"].ToString()).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "<br>";
                lbFactory_Name.Text += drow["Factory_ShortName_CN"].ToString() + "<br>";
            }


        }
    }
    #endregion
}
