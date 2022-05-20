//******************************************************************
//*  作    者：LantaoSu
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-11-11
//*  修改日期：2008-11-20 10:30
//*  修改記錄：
//*            □2008-11-20
//*              1.創建 蘇斕濤
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

public partial class Depository_Depository009 : PageBase
{
    Depository009BL depManager = new Depository009BL();

    static int Add = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //預設為當前系統日期
            txtBeginTime.Text = System.DateTime.Now.ToString("yyyy");
            btnCommit.Visible = false;
            trList.Visible = false;

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                SetConData();                
            }
        }

        if (Add == 1)
        {
            btnCommit.Visible = true;
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {        
        try
        {            
            gvpbYearBudget.BindData();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
        Add = 1;
        btnCommit.Visible = true;
        trList.Visible = true;
        txtBeginTime.Enabled = false;
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCommit_Click(object sender, EventArgs e)
    {
        //保持頁面修改，并更新資料
        DataTable dt = (DataTable)ViewState["YearBudget"];        

        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (Convert.ToDecimal(dr["RemainBudget"].ToString()) < 0)
                {
                    ShowMessage("剩餘預算為負數，請重新填寫預算");
                    return;
                }
                //added by Even.Cheng on 20090108
                if (Convert.ToDecimal(dr["Budget"].ToString()) != 0)
                {
                    if (Convert.ToDecimal(dr["RemainBudget"].ToString()) / Convert.ToDecimal(dr["Budget"].ToString()) < 0.1M)
                    {
                        ShowMessage(GetMaterielName(dr["Materiel_Type"].ToString()) + "項目年度剩餘預算小於1/10");
                        string[] arg = new string[1];
                        arg[0] = GetMaterielName(dr["Materiel_Type"].ToString());
                        Warning.SetWarning(GlobalString.WarningType.LowSafeLevel, arg);
                    }
                }
                //end add
            }

            depManager.Save(dt);

            Add = 0;

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository009.aspx?con=1");
        }
        else
        {
            ShowMessage("沒有可保存的資料！");
        }
    }

    //added by Even.Cheng on 20090108
    public string GetMaterielName(string strMaterielType)
    {
        string strMaterielName = "";
        switch (strMaterielType)
        {
            case GlobalString.MaterielType.EXPONENT_CARD: 
                strMaterielName = "寄卡單（卡）"; break;

            case GlobalString.MaterielType.EXPONENT_BANK:
                strMaterielName = "寄卡單（銀）"; break;

            case GlobalString.MaterielType.ENVELOPE_CARD:
                strMaterielName = "信封（卡）"; break;

            case GlobalString.MaterielType.ENVELOPE_BANK:
                strMaterielName = "信封（銀）"; break;

            case GlobalString.MaterielType.DM_CARD:
                strMaterielName = "DM（卡）"; break;

            case GlobalString.MaterielType.DM_BANK:
                strMaterielName = "DM（銀）"; break;

            case GlobalString.MaterielType.POSTAGE_CARD:
                strMaterielName = "郵資費（卡）"; break;

            case GlobalString.MaterielType.POSTAGE_BANK:
                strMaterielName = "郵資費（銀）"; break;

            case GlobalString.MaterielType.EXPENSE_CARD:
                strMaterielName = "代製費用（卡）"; break;

            case GlobalString.MaterielType.EXPENSE_BANK:
                strMaterielName = "代製費用（銀）"; break;
        }
        return strMaterielName;
    }
    //end add

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        btnCommit.Visible = false;
        txtBeginTime.Enabled = true;
        trList.Visible = false;
        txtBeginTime.Text = System.DateTime.Now.ToString("yyyy");
        Add = 0;
    }

    /// <summary>
    /// 匯出EXCEL格式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        DataTable dtb = (DataTable)ViewState["YearBudget"]; 
        if (dtb != null && dtb.Rows.Count > 0)
        {
            Response.Write("<script>window.open('Depository009Print.aspx?year=" + txtBeginTime.Text + "&used1=" + dtb.Rows[0]["used"] + "&used2=" + dtb.Rows[1]["used"] + "&used3=" + dtb.Rows[2]["used"] + "&used4=" + dtb.Rows[3]["used"] + "&used5=" + dtb.Rows[4]["used"] + "&used6=" + dtb.Rows[5]["used"] + "&used7=" + dtb.Rows[6]["used"] + "&used8=" + dtb.Rows[7]["used"] + "&used9=" + dtb.Rows[8]["used"] + "&used10=" + dtb.Rows[9]["used"] + "');</script>");
        }
        else
        {
            ShowMessage("沒有可列印的資料！");
        }
    }

    /// <summary>
    /// 當預算欄位改變時，變動相應其他欄位
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtbudget_update(object sender, EventArgs e)
    {
        string rowId = ((TextBox)sender).Attributes["key"];
        DataTable dtblBudget = (DataTable)ViewState["YearBudget"];        
        DataRow drowBudget = dtblBudget.Rows[Convert.ToInt32(rowId)];
        decimal used = Convert.ToDecimal(drowBudget["Used"]);
        ((TextBox)sender).Text = ((TextBox)sender).Text.Replace(",", "");
        int budget=0;
        try
        {
            budget = Convert.ToInt32(((TextBox)sender).Text);
        }
        catch { ((TextBox)sender).Text = "0"; }

        ((TextBox)sender).Text = budget.ToString("N0");

        decimal remian_budget = Convert.ToDecimal(budget) - used;
        drowBudget["Budget"] = budget;
        drowBudget["RemainBudget"] = remian_budget;
        if (((Label)gvpbYearBudget.Rows[int.Parse(rowId)].FindControl("lblRemainBudget")).Text.ToString() != "")
        {
            ((Label)gvpbYearBudget.Rows[int.Parse(rowId)].FindControl("lblRemainBudget")).Text = remian_budget.ToString("N2");
        }
        ViewState["YearBudget"] = dtblBudget;
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbYearBudget_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    { 
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtBeginTime", txtBeginTime.Text);        

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlDep = null;

        gvpbYearBudget.Columns[1].HeaderText = txtBeginTime.Text + "年度預算";
        gvpbYearBudget.Columns[2].HeaderText = txtBeginTime.Text + "年已耗用";

        try
        {
            dstlDep = depManager.List(inputs);

            if (dstlDep != null)//如果查到了資料
            {                
                e.Table = dstlDep.Tables[0];//要綁定的資料表
                e.RowCount = dstlDep.Tables[0].Rows.Count;
                ViewState["YearBudget"] = dstlDep.Tables[0];
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
    protected void gvpbYearBudget_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtb = (DataTable)gvpbYearBudget.DataSource;

        TextBox budgeTxt = null;
        Label remainLB = null;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {                
                budgeTxt = (TextBox)e.Row.FindControl("txtbudget");

                if (e.Row.Cells[2].Text != "&nbsp;")
                    e.Row.Cells[2].Text = Convert.ToDecimal(e.Row.Cells[2].Text).ToString("N2");
                
                if (dtb != null && dtb.Rows.Count > 0)
                {                    
                    budgeTxt.Attributes.Add("key", e.Row.RowIndex.ToString());
                    try
                    {
                        budgeTxt.Text = Convert.ToInt32(dtb.Rows[e.Row.RowIndex]["Budget"].ToString()).ToString("N0");
                    }
                    catch
                    {
                        budgeTxt.Text = "0";
                    }

                    remainLB = (Label)e.Row.FindControl("lblRemainBudget");
                    try
                    {
                        remainLB.Text = Convert.ToDecimal(dtb.Rows[e.Row.RowIndex]["RemainBudget"].ToString()).ToString("N2");
                    }
                    catch
                    {
                        remainLB.Text = "0.00";
                    }
                }
                else
                {
                    budgeTxt.Text = "0";
                    budgeTxt.Attributes.Add("key", e.Row.RowIndex.ToString());
                    remainLB = (Label)e.Row.FindControl("lblRemainBudget");
                    remainLB.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    #endregion
}
