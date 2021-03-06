using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Depository_Depository002Query : PageBase
{
    Depository002QBL depManager = new Depository002QBL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbORDERFORMDETAIL.PageSize = GlobalStringManager.PageSize;
        UrctrlCardTypeSelect.CardTypeAll = true;

        if (!IsPostBack)
        {
            Session["D2Q"] = null;

            depManager.dropCaseStatusBind(dropCase_Status);
            dropCase_Status.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            depManager.dropBudgetBind(dropBudget);
            dropBudget.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            depManager.dropAgreementBind(dropAgreement);
            dropAgreement.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            depManager.dropBlankFactoryBind(dropBlankFactory);
            dropBlankFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            depManager.dropPersoFactoryBind(dropFactory_ShortName_CN);
            dropFactory_ShortName_CN.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                SetConData();
                gvpbORDERFORMDETAIL.BindData();
            }
        }
    }

    protected void query_Click(object sender, EventArgs e)
    {
        gvpbORDERFORMDETAIL.BindData();
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["D2Q"];
        if (dt != null && dt.Rows.Count > 0)
        {
            Response.Write("<script>window.open('Depository002MPrint.aspx?OFDRID=" + txtBID.Text + txtMID.Text + txtEID.Text + "&Fore_Delivery_BDate=" + txtFore_Delivery_BDate.Text + "&Fore_Delivery_EDate=" + txtFore_Delivery_EDate.Text + "&Order_Date_FROM=" + txtVALID_DATE_FROM.Text + "&Order_Date_TO=" + txtVALID_DATE_TO.Text + "&Agreement=" + dropAgreement.SelectedValue + "&Budget=" + dropBudget.SelectedValue + "&Factory_ShortName_CN=" + dropFactory_ShortName_CN.SelectedValue + "&BlankFactory=" + dropBlankFactory.SelectedValue + "&Case_Status=" + dropCase_Status.SelectedValue + "&cardtype=" + UrctrlCardTypeSelect.CardType + "&Isdetail=" + radltype.SelectedValue + "');</script>");
        }
        else
        {
            ShowMessage("沒有可列印資料！");
        }
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbORDERFORMDETAIL_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtBRID", txtBID.Text);
        inputs.Add("txtMRID", txtMID.Text);
        inputs.Add("txtERID", txtEID.Text);
        inputs.Add("txtFore_Delivery_BDate", txtFore_Delivery_BDate.Text);
        inputs.Add("txtFore_Delivery_EDate", txtFore_Delivery_EDate.Text);
        inputs.Add("txtOrder_Date_FROM", txtVALID_DATE_FROM.Text);
        inputs.Add("txtOrder_Date_TO", txtVALID_DATE_TO.Text);
        inputs.Add("dropAgreement", dropAgreement.SelectedValue);
        inputs.Add("dropBudget", dropBudget.SelectedValue);
        inputs.Add("dropFactory_ShortName_CN", dropFactory_ShortName_CN.SelectedValue);
        inputs.Add("dropBlankFactory", dropBlankFactory.SelectedValue);
        inputs.Add("dropCase_Status", dropCase_Status.SelectedValue);
        inputs.Add("cardtype", UrctrlCardTypeSelect.CardType);
        inputs.Add("isdetail", radltype.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlDep = null;

        try
        {
            dstlDep = depManager.List2(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlDep != null)//如果查到了資料
            {
                e.Table = dstlDep.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數

                Session["D2Q"] = dstlDep.Tables[0];
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
    protected void gvpbORDERFORMDETAIL_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtb = (DataTable)gvpbORDERFORMDETAIL.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                if (e.Row.Cells[2].Text != "" && e.Row.Cells[2].Text != "&nbsp;")
                {
                    e.Row.Cells[2].Text = Convert.ToInt32(e.Row.Cells[2].Text).ToString("N0");
                }



                Label lblis_exigence = null;
                lblis_exigence = (Label)e.Row.FindControl("lblis_exigence");

                if (dtb.Rows[e.Row.RowIndex]["is_exigence"].ToString() == "1")
                {
                    lblis_exigence.Text = "Urgent";
                }
                else if (dtb.Rows[e.Row.RowIndex]["is_exigence"].ToString() == "2")
                {
                    lblis_exigence.Text = "Normal";
                }

                Label lblCase_Status = null;
                lblCase_Status = (Label)e.Row.FindControl("lblCase_Status");



                // edit by Ian Huang start
                //add chaoma start
                //if (Convert.ToDecimal(dtb.Rows[e.Row.RowIndex]["Unit_Price"].ToString()) != Convert.ToDecimal(dtb.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString()))
                //{
                //    e.Row.Cells[8].ForeColor = System.Drawing.Color.Red;
                //}

                if (e.Row.Cells[8].Text.Trim() == e.Row.Cells[9].Text.Trim())
                {
                    e.Row.Cells[9].Text = "";
                }
                else
                {
                    e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
                }

                //add chaoma end
                // edit by Ian Huang end

                // add by Ian Huang start
                // cell[4] 入庫日
                if ("1900/01/01" == DateTime.Parse(e.Row.Cells[4].Text).ToString("yyyy/MM/dd"))
                {
                    e.Row.Cells[4].Text = "";
                }
                else
                {
                    e.Row.Cells[4].Text = DateTime.Parse(e.Row.Cells[4].Text).ToString("yyyy/MM/dd");
                }

                // cells[5] 入庫量
                String cells3 = "0";
                if (e.Row.Cells[3].Text.Replace("&nbsp;", "").Replace(",", "") != "")
                {
                    cells3 = e.Row.Cells[3].Text.Replace("&nbsp;", "").Replace(",", "");
                }
                e.Row.Cells[5].Text = (float.Parse("0" + e.Row.Cells[2].Text.Replace("&nbsp;", "").Replace(",", "")) - float.Parse(cells3)).ToString("N0");

                //
                if (dtb.Rows[e.Row.RowIndex]["case_status1"].ToString() == "Y")
                {
                    lblCase_Status.Text = "已結案";

                    if (e.Row.Cells[3].Text != "" && e.Row.Cells[3].Text != "&nbsp;")
                    {
                        e.Row.Cells[3].Text = "0";
                    }

                }
                else if (dtb.Rows[e.Row.RowIndex]["case_status1"].ToString() == "N")
                {
                    lblCase_Status.Text = "未結案";

                    if (e.Row.Cells[3].Text != "" && e.Row.Cells[3].Text != "&nbsp;")
                    {
                        e.Row.Cells[3].Text = Convert.ToInt32(e.Row.Cells[3].Text).ToString("N0");
                    }

                }
                // cells[12] 交貨剩餘天數
                if (e.Row.Cells[11].Text != "" && e.Row.Cells[11].Text != "&nbsp;")
                {
                    TimeSpan ts = DateTime.Parse(e.Row.Cells[11].Text) - DateTime.Now.Date;

                    string strDay = "";

                    if (0 > ts.Days)
                    {
                        strDay = "(" + (-1 * ts.Days).ToString() + ")";
                        e.Row.Cells[12].ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        strDay = ts.Days.ToString();
                    }

                    e.Row.Cells[12].Text = strDay; //(ts.Days > 0 ? ts.Days + 1 : ts.Days).ToString();
                }
                if (dtb.Rows[e.Row.RowIndex]["case_status1"].ToString() == "Y")
                {
                    e.Row.Cells[12].Text = "";
                }
                // add by Ian Huang end
            }
            catch
            {
                return;
            }
        }
    }
    #endregion
}
