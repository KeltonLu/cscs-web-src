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
using Microsoft.Practices.EnterpriseLibrary.Data;

public partial class Finance_Finance0023 : PageBase
{
    Finance0023BL FinanceBL = new Finance0023BL();

    DataTable FinanceSession = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        FinanceSession.Columns.Clear();
        FinanceSession.Columns.Add("Group_Name");
        FinanceSession.Columns.Add("Factory_ShortName_CN");
        FinanceSession.Columns.Add("Date_Time");
        FinanceSession.Columns.Add("Finance");
        FinanceSession.Columns.Add("Price");
        FinanceSession.Columns.Add("Name");
        FinanceSession.Columns.Add("Number");
        FinanceSession.Columns.Add("Replace_Name");
        FinanceSession.Columns.Add("Replace_Number");

        gvpbFinance.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //this.txtBDate_Time.Text = DateTime.Now.ToShortDateString();
            //this.txtEDate_Time.Text = DateTime.Now.ToShortDateString();
            dropCard_Purpose_RIDBind();
            dropCard_Group_RIDBind();
            dropFactoryBind();
        }
    }

    //用途變更
    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            dropCard_Group_RIDBind();
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //查詢
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        if (txtEDate_Time.Text != "" && txtBDate_Time.Text != "")
        {
            if (Convert.ToDateTime(txtEDate_Time.Text) >= Convert.ToDateTime(txtBDate_Time.Text))
            {
            }
            else
            {
                ShowMessage("卡片耗用起止日期有誤，請重新選擇！");
                return;
            }
        }
        else
        {
            txtEDate_Time.Text = "";
            txtBDate_Time.Text = "";
        }

        gvpbFinance.BindData();
    }

    //匯出Excel
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        DataTable dtFinance = (DataTable)ViewState["FinanceSession"];
        if (dtFinance != null && dtFinance.Rows.Count > 0)
        {
            try
            {
                string strTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                FinanceBL.Export(dtFinance,strTime);
                Response.Write("<script>window.open('Finance0023print.aspx?Time="+strTime+"&inputs=1&Comment=2&Fine=3','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        else
        {
            ShowMessage("沒有可列印資料！");
        }
    }

    protected void gvpbFinance_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropCard_Purpose_RID", dropCard_Purpose_RID.SelectedValue);
        inputs.Add("dropCard_Group_RID", dropCard_Group_RID.SelectedValue);
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtBDate_Time", txtBDate_Time.Text);
        inputs.Add("txtEDate_Time", txtEDate_Time.Text);
        inputs.Add("txtFinance", txtFinance.Text);

        //保存查詢條件
        ViewState["Finance"] = inputs;
        ViewState["FinanceSession"] = null;

        DataTable dstlFinance = null;

        try
        {
            // 取代製費用明細記錄。
            dstlFinance = FinanceBL.SearchSubTotal(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            if (dstlFinance != null)//如果查到了資料
            {
                e.Table = dstlFinance;//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
            if (intRowCount == 0)
                btnExcelD.Visible = false;
            else
                btnExcelD.Visible = true;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    protected void gvpbFinance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblFinance = (DataTable)gvpbFinance.DataSource;
        DataRow datarow = null;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                // 第一行全部顯示
                if (e.Row.RowIndex != 0)
                {
                    // 後面的控制顯示
                    if (dtblFinance.Rows[e.Row.RowIndex]["Group_Name"].ToString().Trim() != "合計" &&
                        dtblFinance.Rows[e.Row.RowIndex]["Group_Name"].ToString().Substring(dtblFinance.Rows[e.Row.RowIndex]["Group_Name"].ToString().Length - 2, 2).Trim() != "合計")
                    {
                        // 明細行處理
                        //替換前版面為空
                        if (e.Row.Cells[7].Text.Trim() == "&nbsp;")
                        {
                            e.Row.Cells[7].Text = "";
                        }
                        //非第一行記錄並且用途群組為空
                        if (e.Row.Cells[0].Text == dtblFinance.Rows[e.Row.RowIndex - 1]["Group_Name"].ToString())
                        {
                            e.Row.Cells[0].Text = "";
                        }
                        else
                        {
                            datarow = FinanceSession.NewRow();
                            datarow["Group_Name"] = e.Row.Cells[0].Text;
                            datarow["Factory_ShortName_CN"] = e.Row.Cells[1].Text;
                            datarow["Date_Time"] = e.Row.Cells[2].Text;
                            datarow["Finance"] = e.Row.Cells[3].Text;
                            datarow["Price"] = e.Row.Cells[4].Text;
                            //
                            datarow["Name"] = e.Row.Cells[7].Text;
                            datarow["Number"] = e.Row.Cells[8].Text;
                            datarow["Replace_Name"] = e.Row.Cells[5].Text;
                            datarow["Replace_Number"] = e.Row.Cells[6].Text;
                            //
                            FinanceSession.Rows.Add(datarow);

                            ViewState["FinanceSession"] = FinanceSession;
                            return;
                        }

                        //非第一行記錄並且用廠商為空
                        if (e.Row.Cells[1].Text == dtblFinance.Rows[e.Row.RowIndex - 1]["Factory_ShortName_CN"].ToString())
                        {
                            e.Row.Cells[1].Text = "";
                        }
                        else
                        {
                            datarow = FinanceSession.NewRow();
                            datarow["Group_Name"] = e.Row.Cells[0].Text;
                            datarow["Factory_ShortName_CN"] = e.Row.Cells[1].Text;
                            datarow["Date_Time"] = e.Row.Cells[2].Text;
                            datarow["Finance"] = e.Row.Cells[3].Text;
                            datarow["Price"] = e.Row.Cells[4].Text;
                            //
                            datarow["Name"] = e.Row.Cells[7].Text;
                            datarow["Number"] = e.Row.Cells[8].Text;
                            datarow["Replace_Name"] = e.Row.Cells[5].Text;
                            datarow["Replace_Number"] = e.Row.Cells[6].Text;
                            //
                            FinanceSession.Rows.Add(datarow);

                            ViewState["FinanceSession"] = FinanceSession;
                            return;
                        }

                        //非第一行記錄並且用時間為空
                        if (e.Row.Cells[2].Text == dtblFinance.Rows[e.Row.RowIndex - 1]["Date_Time"].ToString())
                        {
                            e.Row.Cells[2].Text = "";
                        }
                        else
                        {
                            datarow = FinanceSession.NewRow();
                            datarow["Group_Name"] = e.Row.Cells[0].Text;
                            datarow["Factory_ShortName_CN"] = e.Row.Cells[1].Text;
                            datarow["Date_Time"] = e.Row.Cells[2].Text;
                            datarow["Finance"] = e.Row.Cells[3].Text;
                            datarow["Price"] = e.Row.Cells[4].Text;
                            //
                            datarow["Name"] = e.Row.Cells[7].Text;
                            datarow["Number"] = e.Row.Cells[8].Text;
                            datarow["Replace_Name"] = e.Row.Cells[5].Text;
                            datarow["Replace_Number"] = e.Row.Cells[6].Text;
                            //
                            FinanceSession.Rows.Add(datarow);

                            ViewState["FinanceSession"] = FinanceSession;
                            return;
                        }

                        //非第一行記錄並且代製項目為空
                        if (e.Row.Cells[3].Text == dtblFinance.Rows[e.Row.RowIndex - 1]["Project_Name"].ToString())
                        {
                            e.Row.Cells[3].Text = "";
                        }
                        else
                        {
                            datarow = FinanceSession.NewRow();
                            datarow["Group_Name"] = e.Row.Cells[0].Text;
                            datarow["Factory_ShortName_CN"] = e.Row.Cells[1].Text;
                            datarow["Date_Time"] = e.Row.Cells[2].Text;
                            datarow["Finance"] = e.Row.Cells[3].Text;
                            datarow["Price"] = e.Row.Cells[4].Text;
                            //
                            datarow["Name"] = e.Row.Cells[7].Text;
                            datarow["Number"] = e.Row.Cells[8].Text;
                            datarow["Replace_Name"] = e.Row.Cells[5].Text;
                            datarow["Replace_Number"] = e.Row.Cells[6].Text;
                            //
                            FinanceSession.Rows.Add(datarow);

                            ViewState["FinanceSession"] = FinanceSession;
                            return;
                        }

                        //非第一行記錄並且代製價格為空
                        if (e.Row.Cells[4].Text == dtblFinance.Rows[e.Row.RowIndex - 1]["Price"].ToString())
                        {
                            e.Row.Cells[4].Text = "";
                        }
                        else
                        {
                            datarow = FinanceSession.NewRow();
                            datarow["Group_Name"] = e.Row.Cells[0].Text;
                            datarow["Factory_ShortName_CN"] = e.Row.Cells[1].Text;
                            datarow["Date_Time"] = e.Row.Cells[2].Text;
                            datarow["Finance"] = e.Row.Cells[3].Text;
                            datarow["Price"] = e.Row.Cells[4].Text;
                            //
                            datarow["Name"] = e.Row.Cells[7].Text;
                            datarow["Number"] = e.Row.Cells[8].Text;
                            datarow["Replace_Name"] = e.Row.Cells[5].Text;
                            datarow["Replace_Number"] = e.Row.Cells[6].Text;
                            //
                            FinanceSession.Rows.Add(datarow);

                            ViewState["FinanceSession"] = FinanceSession;
                            return;
                        }
                    }
                    else
                    {
                        // 合計行處理
                        e.Row.Cells[1].Text = "";
                        e.Row.Cells[2].Text = "";
                        e.Row.Cells[3].Text = "";
                        e.Row.Cells[4].Text = "";
                        e.Row.Cells[5].Text = "";
                        e.Row.Cells[7].Text = "";
                    }
                }

                datarow = FinanceSession.NewRow();
                datarow["Group_Name"] = e.Row.Cells[0].Text;
                datarow["Factory_ShortName_CN"] = e.Row.Cells[1].Text;
                datarow["Date_Time"] = e.Row.Cells[2].Text;
                datarow["Finance"] = e.Row.Cells[3].Text;
                datarow["Price"] = e.Row.Cells[4].Text;
                //
                datarow["Name"] = e.Row.Cells[7].Text;
                datarow["Number"] = e.Row.Cells[8].Text;
                datarow["Replace_Name"] = e.Row.Cells[5].Text;
                datarow["Replace_Number"] = e.Row.Cells[6].Text;
                //
                FinanceSession.Rows.Add(datarow);

                ViewState["FinanceSession"] = FinanceSession;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    #region 初始化下拉框
    /// <summary>
    /// Perso卡廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        dropFactory.Items.Clear();

        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataValueField = "RID";
        dropFactory.DataSource = FinanceBL.getFactory();
        dropFactory.DataBind();

        dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_Purpose_RIDBind()
    {
        dropCard_Purpose_RID.DataTextField = "PARAM_NAME";
        dropCard_Purpose_RID.DataValueField = "Param_Code";
        dropCard_Purpose_RID.DataSource = FinanceBL.getParam_Finance();
        dropCard_Purpose_RID.DataBind();

        //dropCard_Purpose_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_Group_RIDBind()
    {
        dropCard_Group_RID.Items.Clear();

        dropCard_Group_RID.DataTextField = "GROUP_NAME";
        dropCard_Group_RID.DataValueField = "RID";
        dropCard_Group_RID.DataSource = FinanceBL.getCardGroup(dropCard_Purpose_RID.SelectedValue);
        dropCard_Group_RID.DataBind();

        dropCard_Group_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }
    #endregion 初始化下拉框
}
