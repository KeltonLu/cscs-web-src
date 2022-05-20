//******************************************************************
//*  作    者：JunWang
//*  功能說明：卡片在入庫查詢
//*  創建日期：2008-09-22
//*  修改日期：2008-09-22 12:00
//*  修改記錄：
//*            □2008-09-22
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
public partial class Depository_Depository005 :PageBase
{
    Depository005BL BL =new Depository005BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        UctrlCardType.ChangeName = true;
        this.gvpbImportStock.PageSize = GlobalStringManager.PageSize;        

        if (!IsPostBack)
        {
            Session.Remove("dtblImp");

            txtStock_RIDYear.Focus();

            DataSet dstFactoryData = BL.GetFactoryData();

            dropPerso_Factory_RID.DataSource = dstFactoryData.Tables[0];
            dropPerso_Factory_RID.DataBind();
            dropPerso_Factory_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    UctrlCardType.SetRightItem = (DataTable)(((Dictionary<string, object>)Session["Condition"])["UctrlCardType"]);
                }
                gvpbImportStock.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage("今天已經日結，不可增加再入庫信息");
            return;
        }

        Response.Redirect(string.Concat("Depository005Imp.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbImportStock.BindData();
    }
     #endregion

    #region 列表資料綁定.
    protected void gvpbImportStock_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtStock_RIDYear", txtStock_RIDYear.Text.Trim());
        inputs.Add("txtStock_RID1", txtStock_RID1.Text.Trim());
        inputs.Add("txtStock_RID2", txtStock_RID2.Text.Trim());
        inputs.Add("txtStock_RID3", txtStock_RID3.Text.Trim());
        inputs.Add("UctrlCardType", UctrlCardType.GetRightItem);
        inputs.Add("dropPerso_Factory_RID", dropPerso_Factory_RID.SelectedValue.Trim());
        inputs.Add("txtIncome_DateFrom", txtIncome_DateFrom.Text.Trim());
        inputs.Add("txtIncome_DateTo", txtIncome_DateTo.Text.Trim());


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

    protected void gvpbImportStock_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblImport = (DataTable)gvpbImportStock.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblImport.Rows.Count == 0)
                return;

            try
            {
                HyperLink hlReportID = null;
                hlReportID = (HyperLink)e.Row.FindControl("hlReport_RID");
                hlReportID.Text = dtblImport.Rows[e.Row.RowIndex]["Report_RID"].ToString();
                hlReportID.NavigateUrl = "#";
                hlReportID.Attributes.Add("onclick", "window.open('Depository005ImpPrint.aspx?RID=" + dtblImport.Rows[e.Row.RowIndex]["RID"].ToString() + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=800,height=650');");
                                
                if (e.Row.Cells[12].Text != "&nbsp;")
                {
                    e.Row.Cells[12].Text = e.Row.Cells[12].Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");
                }

                if (e.Row.Cells[3].Text != "&nbsp;")
                {
                    e.Row.Cells[3].Text = Convert.ToDateTime(e.Row.Cells[3].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
            }
            catch
            {
                return;
            }
        }
    }
    #endregion
}
