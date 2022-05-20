//******************************************************************
//*  作    者：lantaosu
//*  功能說明：庫存成本明細查詢
//*  創建日期：2008-12-10
//*  修改日期：2008-12-11 12:00
//*  修改記錄：
//*            □2008-12-11
//*              1.創建 蘇斕濤
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

public partial class Finance_Finance004_2 : PageBase
{
    Finance004_5BL bl = new Finance004_5BL();
    Finance004_1BL bl1 = new Finance004_1BL();

    bool isSecondBing = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbStockCost.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //初始化頁面
            this.queryResult.Visible = false;
            this.result2.Visible = false;

            DataSet Year = bl.GetYearList();
            dropYear.DataSource = Year;
            dropYear.DataValueField = "Year";
            dropYear.DataTextField = "Year";
            dropYear.DataBind();
            dropYear.SelectedValue = DateTime.Now.Year.ToString();
            dropMonth.SelectedValue = DateTime.Now.Month.ToString();

            DataSet Use = bl.GetUseList();
            dropUse.DataSource = Use;
            dropUse.DataValueField = "Param_Code";
            dropUse.DataTextField = "Param_Name";
            dropUse.DataBind();

            DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
            dropGroup.DataSource = Group;
            dropGroup.DataValueField = "RID";
            dropGroup.DataTextField = "Group_Name";
            dropGroup.DataBind();

            btnReport.Visible = false;
        }
    }

    /// <summary>
    /// 查詢該用途的所有卡穜群組，放於【群組】下拉框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropUse_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
        dropGroup.DataSource = Group;
        dropGroup.DataValueField = "RID";
        dropGroup.DataTextField = "Group_Name";
        dropGroup.DataBind();
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string StartDate = null;
        string EndDate = null;
        string[] Date = new string[2];

        //檢查頁面
        if (dropGroup.SelectedValue.Trim() == "")
        {
            ShowMessage("群組不能為空");
            return;
        }

        if (int.Parse(dropYear.SelectedItem.Text + dropMonth.SelectedValue.PadLeft(2, '0')) < 201508)
        {
            ShowMessage("只能執行2015/08之後的查詢");
            return;
        }

        //獲取帳務起迄日
        Date = bl1.GetDateFromAndDateTo(dropYear.SelectedItem.Text, dropMonth.SelectedValue);
        if (Date == null)
        {
            ShowMessage("卡片成本帳務期間未設定");
            return;
        }

        StartDate = Date[0];
        EndDate = Date[1];
        ViewState["DateFrom"] = StartDate;
        ViewState["DateTo"] = EndDate;
        //獲取上個帳務起迄日
        string[] beginDate = new string[2];
        string strYear;
        string strMonth;
        int intMonth = (Convert.ToInt32(dropMonth.SelectedValue.ToString()) - 1);
        if (intMonth == 0)
        {
            strYear = Convert.ToString(Convert.ToInt32(dropYear.SelectedItem.Text.ToString()) - 1);
            strMonth = "12";
        }
        else
        {
            strYear = dropYear.SelectedItem.Text.ToString();
            strMonth = intMonth.ToString();
        }

        //取得該日期區間内的第一個未日結日
        string strUnCheckDate = bl1.GetUncheckDate(StartDate, EndDate);
        if (strUnCheckDate != "")
        {
            EndDate = strUnCheckDate;   //未日結就捉到最後有日結的日期           
            ViewState["DateTo"] = EndDate;
            //ShowMessage(strUnCheckDate + "後未日結");
            //return;
        }

        lblTitle.Text = dropMonth.SelectedValue + "月份" + dropGroup.SelectedItem.Text + "庫存成本明細表";
        lblDate.Text = StartDate.ToString() + "~" + EndDate.ToString();

        //查詢資料
        gvpbStockCost.BindData();

        if (gvpbStockCost.DataSource != null)
        {
            this.queryResult.Visible = true;
            this.result2.Visible = true;
            btnReport.Visible = true;

            //show Report庫存殘值調整
            Show_Report(StartDate.ToString(), EndDate.ToString());

            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doSearch();", true);
        }
        else
        {
            ShowMessage("查無資料");
            this.queryResult.Visible = false;
            this.result2.Visible = false;
            btnReport.Visible = false;
        }
    }

    public DataTable CreateTable()
    {
        DataTable dtMSC = new DataTable();
        dtMSC.Columns.Add("版面簡稱", Type.GetType("System.String"));
        dtMSC.Columns.Add("期初庫存數", Type.GetType("System.String"));
        dtMSC.Columns.Add("期初未稅單價", Type.GetType("System.String"));
        dtMSC.Columns.Add("進貨數", Type.GetType("System.String"));
        dtMSC.Columns.Add("消耗卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("製成卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("耗損卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("銷毀卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("調整卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("期末庫存數", Type.GetType("System.String"));
        dtMSC.Columns.Add("未稅單價", Type.GetType("System.String"));
        dtMSC.Columns.Add("期初庫存金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("進貨金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("消耗卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("製成卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("耗損卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("銷毀卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("調整卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("單價調整金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("期末庫存未稅金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("Income_Date", Type.GetType("System.Int32"));
        dtMSC.Columns.Add("CardType_RID", Type.GetType("System.String"));
        return dtMSC;
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbStockCost_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;
        string DateFrom = ViewState["DateFrom"].ToString();
        string DateTo = ViewState["DateTo"].ToString();

        hidDateFrom.Value = ViewState["DateFrom"].ToString();
        hidDateTo.Value = ViewState["DateTo"].ToString();

        DataSet dstlStockCost = null;

        try
        {
            string strDateTo = bl.GetNextWorkDay(DateTo);                //取這個日期區間之後的第一個工作日
            string strDateFrom = bl.GetFirstWorkDay(DateFrom, DateTo);   //起日  //取這個日期區間的第一個工作日 

            //捉 進貨 
            dstlStockCost = bl.List(strDateFrom, strDateTo, dropGroup.SelectedValue, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            //
            dstlStockCost = bl.List_New(strDateFrom, strDateTo);

            DataTable dtResult = GenResultTable(dstlStockCost);

            //傳參數，添加報表資料
            ViewState["StockCost"] = dtResult;

            if (dtResult != null && dtResult.Rows.Count > 0)//如果查到了資料
            {
                e.Table = dtResult;//要綁定的資料表
                e.RowCount = dtResult.Rows.Count;
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
    protected void gvpbStockCost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //將負數項轉換為括號加（）
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text.Contains("-"))
                {
                    e.Row.Cells[i].Text = "(" + e.Row.Cells[i].Text.Substring(1, e.Row.Cells[i].Text.Length - 1) + ")";
                    e.Row.Cells[i].ForeColor = System.Drawing.Color.Red;
                }
            }
            if (e.Row.Cells[0].Text == "合計")
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                    e.Row.Cells[i].Font.Bold = true;
            }
        }
    }
    #endregion

    /// <summary>
    /// 匯出報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        this.queryResult.Visible = true;
        this.result2.Visible = true;

        int i = 0;
        DataTable dt = (DataTable)ViewState["StockCost"];
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");

        HidTime.Value = time;

        //千分位
        foreach (DataRow dr in dt.Rows)
        {
            for (i = 1; i < dt.Columns.Count; i++)
                dr[i] = dr[i].ToString().Replace(",", "");
        }
        //將表格中的資料存入RPT_Finance004_3中
        bl.AddReport_New(dt, time);

        Session["Last_W_Number"] = lblLast_W_Number.Text.Replace(",", "");
        Session["S_Number"] = lblS_Numbers.Text.Replace(",", "");
        Session["F_Number"] = lblF_Numbers.Text.Replace(",", "");
        Session["Back_Number"] = lblBack_Number.Text.Replace(",", "");
        Session["P_Number"] = lblP_Number.Text.Replace(",", "");
        Session["T_Number"] = lblT_Number.Text.Replace(",", "");
        Session["A_Number"] = lblA_Number.Text.Replace(",", "");
        Session["W_Number"] = lblW_Number.Text.Replace(",", "");
        Session["D_Number"] = lblD_Number.Text.Replace(",", "");
        Session["UseOutNumber"] = lblUseOutNumber.Text.Replace(",", "");
        Session["lblXH_Numer"] = lblXH_Numer.Text.Replace(",", "");
        Session["lblTZ_Numer"] = lblTZ_Numer.Text.Replace(",", "");
        Session["lblLast_YM"] = lblLast_W_Num.Text.Substring(0, 7);

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "exportExcel();", true);

        this.queryResult.Visible = true;

        this.result2.Visible = true;

        if (dropGroup.SelectedItem.Text != "晶片信用卡")
        {
            this.result3.Visible = false;
            this.result4.Visible = false;
            this.result5.Visible = true;
        }
        else
        {
            this.result3.Visible = true;
            this.result4.Visible = true;
            this.result5.Visible = false;
        }
    }

    /// <summary>
    /// 產生數據方法！
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private DataTable GenResultTable(DataSet dstlMonthStockCost)
    {
        int number = 0;
        int i = 0;
        int j = 0;

        string DateFrom = ViewState["DateFrom"].ToString();
        string DateTo = ViewState["DateTo"].ToString();

        hidDateFrom.Value = ViewState["DateFrom"].ToString();
        hidDateTo.Value = ViewState["DateTo"].ToString();

        //200908IR刪除本月期末金額
        bl.DELEndPrice(dropYear.SelectedItem.Text, dropMonth.SelectedItem.Text, dropUse.SelectedItem.Value, dropGroup.SelectedItem.Value);

        //將進貨資料排序 :  cardtype_rid,income_date
        DataRow[] drowStockCost = dstlMonthStockCost.Tables[0].Select("", "cardtype_rid,income_date");

        try
        {
            #region 取得 各個卡種在選定日期區間之内的 各種狀況的卡種的數量
            DataSet CardNumber = bl.GetCardNumber(DateFrom, DateTo, dropGroup.SelectedValue);

            DataTable dtbl = new DataTable();
            DataRow dr3 = null;
            dtbl.Columns.Add("CardType_RID", Type.GetType("System.String"));
            dtbl.Columns.Add("消耗卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("製成卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("耗損卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("銷毀卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("調整卡數", Type.GetType("System.String"));

            //先新增CardType_RID 資料行 
            foreach (DataRow drow in CardNumber.Tables[0].Rows)
            {
                if (dtbl.Select("CardType_RID='" + drow["CardType_RID"].ToString().Trim() + "'").Length > 0)
                {
                    continue;
                }
                else
                {
                    dr3 = dtbl.NewRow();
                    dr3["CardType_RID"] = drow["CardType_RID"].ToString().Trim();
                    dtbl.Rows.Add(dr3);
                }
            }
            #endregion

            #region 將相同CardType_RID  各種狀況的卡種的數量 寫成同一行
            foreach (DataRow dr1 in dtbl.Rows)
            {
                DataSet dset = bl.GetExpression(GlobalString.Expression.Made_RID.ToString()); //製成卡數："1"
                number = 0;
                for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                {
                    if (CardNumber.Tables[0].Select("Status_RID='" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                    {
                        j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID='" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + "'  and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());

                        if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            number += j;
                        else
                            number -= j;
                    }
                }
                dr1["製成卡數"] = number.ToString();

                DataSet dset1 = bl.GetExpression(GlobalString.Expression.Waste_RID.ToString());//耗損卡數："3"
                number = 0;
                for (i = 0; i < dset1.Tables[0].Rows.Count; i++)
                {
                    if (CardNumber.Tables[0].Select("Status_RID='" + dset1.Tables[0].Rows[i]["Type_RID"].ToString() + "'  and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                    {
                        j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID='" + dset1.Tables[0].Rows[i]["Type_RID"].ToString() + "'  and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());

                        if (dset1.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            number += j;
                        else
                            number -= j;
                    }
                }
                dr1["耗損卡數"] = number.ToString();

                dr1["消耗卡數"] = Convert.ToInt32(dr1["製成卡數"]) + Convert.ToInt32(dr1["耗損卡數"]);

                if (CardNumber.Tables[0].Select("Status_Code='13' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                    dr1["調整卡數"] = Convert.ToInt32(CardNumber.Tables[0].Select("Status_Code='13' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());
                else
                    dr1["調整卡數"] = "0";

                if (CardNumber.Tables[0].Select("Status_Code='12' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                    dr1["銷毀卡數"] = Convert.ToInt32(CardNumber.Tables[0].Select("Status_Code='12' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());
                else
                    dr1["銷毀卡數"] = "0";
            }

            #endregion

            DataTable dtMSC = this.CreateTable();
            DataRow drMSC = null;

            //期初的日結日期：
            string Date_Time = "";
            //Date_Time = bl.GetLastWorkDay(DateFrom);//該日期區間的前一個工作日
            Date_Time = bl.GetFirstWorkDay(DateFrom, DateTo);//該日期區間内的第一個工作日

            //期末的日結日期
            string strLastCheckDate = bl.GetNextWorkDay(DateTo);//取這個日期區間之后的第一個工作日

            string strActualDate = GlobalStringManager.ActualDate;

            #region 捉取所有資料-期初庫存數量
            DataSet LastEndPrice = new DataSet();
            LastEndPrice = bl.GetLastEndPrice(dropYear.SelectedItem.Value, dropMonth.SelectedItem.Value, dropUse.SelectedItem.Value, dropGroup.SelectedItem.Value);
            foreach (DataRow drLastEndPrice in LastEndPrice.Tables[0].Rows)    //期初
            {
                drMSC = dtMSC.NewRow();
                drMSC["版面簡稱"] = drLastEndPrice["CARD_Name"].ToString();
                drMSC["CardType_RID"] = drLastEndPrice["CARDTYPE_RID"].ToString();
                drMSC["期初庫存數"] = drLastEndPrice["EndNumber"].ToString();
                drMSC["期初未稅單價"] = drLastEndPrice["BeginPrice"].ToString();
                drMSC["未稅單價"] = drLastEndPrice["BeginPrice"].ToString();
                drMSC["期初庫存金額"] = drLastEndPrice["EndPrice"].ToString();
                drMSC["Income_Date"] = drLastEndPrice["Income_Date"].ToString();

                drMSC["進貨數"] = "0";
                drMSC["消耗卡數"] = "0";
                drMSC["製成卡數"] = "0";
                drMSC["耗損卡數"] = "0";
                drMSC["銷毀卡數"] = "0";
                drMSC["調整卡數"] = "0";
                drMSC["期末庫存數"] = "0";

                drMSC["進貨金額"] = "0";
                drMSC["消耗卡金額"] = "0";
                drMSC["製成卡金額"] = "0";
                drMSC["耗損卡金額"] = "0";
                drMSC["銷毀卡金額"] = "0";
                drMSC["調整卡金額"] = "0";
                drMSC["單價調整金額"] = "0";

                dtMSC.Rows.Add(drMSC);
            }
            #endregion

            #region 捉 進貨 資料 數量
            foreach (DataRow drMonthStockCost in drowStockCost)    //進貨
            {
                #region 進貨
                //進貨數  Income_Date 需在月份區間 & 
                //加入判斷，如果入庫日期小於上線日，則不認為是進貨！
                string strIncome_Date = Convert.ToDateTime(drMonthStockCost["Income_Date"].ToString().Trim()).ToString("yyyy/MM/dd");
                if (DateFrom.CompareTo(strIncome_Date) <= 0 && strIncome_Date.CompareTo(DateTo) <= 0 && strIncome_Date.CompareTo(strActualDate) >= 0)
                {
                    //本期進貨                   
                    drMSC = dtMSC.NewRow();

                    // Legend 2017/12/13 查詢清單, 版面簡稱前增加卡片編號欄位CARD TYPE(3碼) AFFINITY (4碼) PHOTO(4碼)
                    // 版面名稱
                    string strName = "";
                    //drMSC["版面簡稱"] = drMonthStockCost["Name"].ToString();
                    strName = bl.GetCardTypeNOAndName(drMonthStockCost["CardType_RID"].ToString());
                    drMSC["版面簡稱"] = strName;
                    drMSC["CardType_RID"] = drMonthStockCost["CardType_RID"].ToString();
                    drMSC["進貨數"] = drMonthStockCost["Number"].ToString();

                    string strUnit_Price = "0.0";
                    if (drMonthStockCost["Unit_Price"].ToString().Trim() != "")
                    {
                        strUnit_Price = drMonthStockCost["Unit_Price"].ToString().Trim();
                    }

                    #region 捉未稅單價

                    if (strIncome_Date.CompareTo(strActualDate) < 0 || drMonthStockCost["Operate_RID"].ToString().Trim() == "0")
                    {
                        drMSC["未稅單價"] = strUnit_Price;
                    }
                    else
                    {
                        if (!StringUtil.IsEmpty(drMonthStockCost["Operate_Type"].ToString()))
                        {
                            DataSet UnitPrice1 = bl.GetUnitPrice(drMonthStockCost["Operate_RID"].ToString(), drMonthStockCost["Operate_Type"].ToString(), strLastCheckDate.Split(' ')[0]);
                            if (UnitPrice1.Tables[0].Rows.Count > 0)
                            {
                                string strReal_Ask_Number = UnitPrice1.Tables[0].Rows[0]["Real_Ask_Number"].ToString().Trim();
                                decimal dReal_Ask_Number = 0;
                                if (strReal_Ask_Number != "")
                                {
                                    dReal_Ask_Number = Convert.ToDecimal(strReal_Ask_Number);
                                }

                                if (dReal_Ask_Number == 0)//未請款
                                {
                                    if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡" || dropGroup.SelectedItem.Text == "DEBIT")
                                    {
                                        if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
                                        {
                                            drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim();
                                        }
                                        else
                                        {
                                            drMSC["未稅單價"] = "0.0000";
                                        }
                                    }
                                    else
                                    {
                                        if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
                                        {
                                            decimal unit_price = Convert.ToDecimal(UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"]) / 1.05M;
                                            drMSC["未稅單價"] = unit_price.ToString("N4");
                                        }
                                        else
                                        {
                                            drMSC["未稅單價"] = "0.0000";
                                        }
                                    }
                                }
                                else//已請款
                                {
                                    if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡" || dropGroup.SelectedItem.Text == "DEBIT")
                                    {
                                        if (UnitPrice1.Tables[0].Rows[0]["Unit_Price"].ToString().Trim() != "")
                                        {
                                            drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price"].ToString().Trim();
                                        }
                                        else
                                        {
                                            drMSC["未稅單價"] = "0.0000";
                                        }
                                    }
                                    else
                                    {
                                        if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim() != "")
                                        {
                                            drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim();
                                        }
                                        else
                                        {
                                            drMSC["未稅單價"] = "0.0000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                drMSC["未稅單價"] = "0.0000";
                            }
                        }
                        else
                        {
                            drMSC["未稅單價"] = "0.0000";
                        }
                    }
                    #endregion

                    drMSC["Income_Date"] = Convert.ToInt32(strIncome_Date.Substring(0, 4) +
                                                           strIncome_Date.Substring(5, 2) +
                                                           strIncome_Date.Substring(8, 2));

                    drMSC["期初庫存數"] = "0";
                    drMSC["期初未稅單價"] = "0.0000";
                    drMSC["期初庫存金額"] = "0";
                    drMSC["消耗卡數"] = "0";
                    drMSC["製成卡數"] = "0";
                    drMSC["耗損卡數"] = "0";
                    drMSC["銷毀卡數"] = "0";
                    drMSC["調整卡數"] = "0";
                    drMSC["期末庫存數"] = "0";
                    drMSC["消耗卡金額"] = "0";
                    drMSC["製成卡金額"] = "0";
                    drMSC["耗損卡金額"] = "0";
                    drMSC["銷毀卡金額"] = "0";
                    drMSC["調整卡金額"] = "0";
                    drMSC["單價調整金額"] = "0";

                    dtMSC.Rows.Add(drMSC);
                }
                #endregion
            }

            #endregion

            //將期初+進貨 資料排序 :  cardtype_rid,income_date
            DataRow[] drowMSC = dtMSC.Select("", "CardType_RID,Income_Date");

            DataRow dr = null;

            foreach (DataRow drow in drowMSC)
            {
                #region 計算數量
                #region 製成卡數,耗損卡數,消耗卡數,銷毀卡數,調整卡數
                // edit by Ian Huang 帳務管理模組/卡片成本/-月庫存成本查詢、庫存成本查詢 start
                //製成卡數,耗損卡數,消耗卡數,銷毀卡數,調整卡數.
                if (dtbl.Select("CardType_RID='" + drow["CardType_RID"].ToString() + "'").Length > 0)
                {
                    dr = dtbl.Select("CardType_RID='" + drow["CardType_RID"].ToString() + "'")[0];

                    number = Convert.ToInt32(drow["期初庫存數"].ToString()) + Convert.ToInt32(drow["進貨數"].ToString());

                    if (dropGroup.SelectedItem.Text == "晶片信用卡")
                    {
                        drow["消耗卡數"] = "0";
                        if (number > 0 && dr["耗損卡數"].ToString() != "" && Convert.ToInt32(dr["耗損卡數"].ToString()) > 0)
                        {
                            drow["耗損卡數"] = bl.GetCardCount(number, dr, "耗損卡數").ToString();
                            number -= Convert.ToInt32(drow["耗損卡數"]);
                        }
                        else
                        {
                            drow["耗損卡數"] = "0";
                        }

                        if (number > 0 && dr["製成卡數"].ToString() != "" && Convert.ToInt32(dr["製成卡數"].ToString()) > 0)
                        {
                            drow["製成卡數"] = bl.GetCardCount(number, dr, "製成卡數").ToString();
                            number -= Convert.ToInt32(drow["製成卡數"]);
                        }
                        else
                        {
                            drow["製成卡數"] = "0";
                        }
                    }
                    else
                    {
                        drow["製成卡數"] = "0";
                        drow["耗損卡數"] = "0";
                        if (number > 0 && dr["消耗卡數"].ToString() != "" && Convert.ToInt32(dr["消耗卡數"].ToString()) > 0)
                        {
                            drow["消耗卡數"] = bl.GetCardCount(number, dr, "消耗卡數").ToString();
                            number -= Convert.ToInt32(drow["消耗卡數"]);
                        }
                        else
                        {
                            drow["消耗卡數"] = "0";
                        }
                    }

                    if (number > 0 && dr["銷毀卡數"].ToString() != "" && Convert.ToInt32(dr["銷毀卡數"].ToString()) > 0)
                    {
                        drow["銷毀卡數"] = bl.GetCardCount(number, dr, "銷毀卡數").ToString();
                        number -= Convert.ToInt32(drow["銷毀卡數"]);
                    }
                    else
                    {
                        drow["銷毀卡數"] = "0";
                    }

                    if (number > 0 && dr["調整卡數"].ToString() != "" && Convert.ToInt32(dr["調整卡數"].ToString()) > 0)
                    {
                        drow["調整卡數"] = bl.GetCardCount(number, dr, "調整卡數").ToString();
                        number -= Convert.ToInt32(drow["調整卡數"]);
                    }
                    else
                    {
                        drow["調整卡數"] = "0";
                    }
                }
                else
                {
                    drow["消耗卡數"] = "0";
                    drow["製成卡數"] = "0";
                    drow["耗損卡數"] = "0";
                    drow["銷毀卡數"] = "0";
                    drow["調整卡數"] = "0";
                }
                // edit by Ian Huang 帳務管理模組/卡片成本/-月庫存成本查詢、庫存成本查詢 end
                #endregion

                //期末庫存數                
                if (dropGroup.SelectedItem.Text == "晶片信用卡")
                    drow["期末庫存數"] = Convert.ToString(Convert.ToInt32(drow["期初庫存數"].ToString()) + Convert.ToInt32(drow["進貨數"].ToString()) - Convert.ToInt32(drow["製成卡數"].ToString()) - Convert.ToInt32(drow["耗損卡數"].ToString()) - Convert.ToInt32(drow["銷毀卡數"].ToString()) - Convert.ToInt32(drow["調整卡數"].ToString()));
                else
                    drow["期末庫存數"] = Convert.ToString(Convert.ToInt32(drow["期初庫存數"].ToString()) + Convert.ToInt32(drow["進貨數"].ToString()) - Convert.ToInt32(drow["消耗卡數"].ToString()) - Convert.ToInt32(drow["銷毀卡數"].ToString()) - Convert.ToInt32(drow["調整卡數"].ToString()));
                #endregion

                #region 計算金額
                //進貨金額                
                drow["進貨金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drow["進貨數"].ToString()) * Convert.ToDecimal(drow["未稅單價"].ToString()), MidpointRounding.AwayFromZero));

                //銷毀卡金額,調整卡金額                
                drow["銷毀卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drow["銷毀卡數"].ToString()) * Convert.ToDecimal(drow["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                drow["調整卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drow["調整卡數"].ToString()) * Convert.ToDecimal(drow["未稅單價"].ToString()), MidpointRounding.AwayFromZero));

                //製成卡金額,耗損卡金額,消耗卡金額                
                if (dropGroup.SelectedItem.Text != "晶片信用卡")
                {
                    drow["製成卡金額"] = "0.00";
                    drow["耗損卡金額"] = "0.00";
                    if (drow["期末庫存數"].ToString() == "0")
                    {
                        //200907CR消耗卡的金額=期初庫存金額+進貨金額-銷毀卡金額+調整卡金額
                        drow["消耗卡金額"] = Convert.ToString(Convert.ToDecimal(drow["期初庫存金額"]) + Convert.ToDecimal(drow["進貨金額"]) - Convert.ToDecimal(drow["銷毀卡金額"]) + Convert.ToDecimal(drow["調整卡金額"]));
                    }
                    else
                    {
                        drow["消耗卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drow["消耗卡數"].ToString()) * Convert.ToDecimal(drow["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                    }
                }
                else
                {
                    drow["製成卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drow["製成卡數"].ToString()) * Convert.ToDecimal(drow["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                    drow["消耗卡金額"] = "0.00";

                    if (drow["期末庫存數"].ToString() == "0")
                    {
                        //200907CR耗損卡的金額=期初庫存金額+進貨金額-製成卡金額-銷毀卡金額+調整卡金額
                        drow["耗損卡金額"] = Convert.ToString(Convert.ToDecimal(drow["期初庫存金額"]) + Convert.ToDecimal(drow["進貨金額"]) - Convert.ToDecimal(drow["製成卡金額"]) - Convert.ToDecimal(drow["銷毀卡金額"]) + Convert.ToDecimal(drow["調整卡金額"]));
                    }
                    else
                    {
                        drow["耗損卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drow["耗損卡數"].ToString()) * Convert.ToDecimal(drow["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                    }
                }

                //期末庫存未(含)稅金額                
                drow["期末庫存未稅金額"] = Convert.ToString(Convert.ToDecimal(drow["期初庫存金額"]) + Convert.ToDecimal(drow["進貨金額"]) - Convert.ToDecimal(drow["製成卡金額"]) - Convert.ToDecimal(drow["耗損卡金額"]) - Convert.ToDecimal(drow["消耗卡金額"]) - Convert.ToDecimal(drow["銷毀卡金額"]) + Convert.ToDecimal(drow["調整卡金額"]));

                //單價調整金額                
                if (Convert.ToInt32(drow["期初庫存數"]) != 0 && (Convert.ToDecimal(drow["期初未稅單價"].ToString()) != Convert.ToDecimal(drow["未稅單價"].ToString())))
                {
                    drow["單價調整金額"] = Convert.ToString(Convert.ToDecimal(drow["期初庫存金額"].ToString()) - Convert.ToDecimal(drow["期末庫存未稅金額"].ToString()));
                }
                else
                    drow["單價調整金額"] = "0.00";

                #endregion

                //200907CR月報表儲存之月底庫存金額
                if (drow["期末庫存數"].ToString() != "0")
                {
                    bl.AddEndPrice(drow["CardType_RID"].ToString(), dropYear.SelectedItem.Text + dropMonth.SelectedItem.Text.PadLeft(2, '0'), drow["未稅單價"].ToString(), drow["期末庫存數"].ToString(),
                                   drow["期末庫存未稅金額"].ToString(), drow["Income_Date"].ToString(), dropUse.SelectedItem.Value, dropGroup.SelectedItem.Value);
                }
            }

            //排序
            dtMSC.DefaultView.Sort = "版面簡稱,Income_Date ";

            DataTable dtResult = dtMSC.DefaultView.ToTable();

            dtResult.Columns.Remove("Income_Date");
            dtResult.Columns.Remove("CardType_RID");

            #region 資料表中的合計行
            if (dtResult.Rows.Count > 0)
            {
                drMSC = dtResult.NewRow();
                drMSC["版面簡稱"] = "合計";
                int total = 0;
                decimal total1 = 0;
                for (j = 0; j < dtResult.Rows.Count; j++)
                {
                    if (dtResult.Rows[j][1].ToString().Trim() != "")
                        total += Convert.ToInt32(dtResult.Rows[j][1].ToString().Trim());
                }
                drMSC[1] = total.ToString();

                for (i = 3; i < 10; i++)
                {
                    total = 0;
                    for (j = 0; j < dtResult.Rows.Count; j++)
                    {
                        if (dtResult.Rows[j][i].ToString().Trim() != "")
                            total += Convert.ToInt32(dtResult.Rows[j][i].ToString().Trim());
                    }
                    drMSC[i] = total.ToString();
                }

                for (i = 11; i < dtResult.Columns.Count; i++)
                {
                    total1 = 0;
                    for (j = 0; j < dtResult.Rows.Count; j++)
                    {
                        if (dtResult.Rows[j][i].ToString().Trim() != "")
                            total1 += Convert.ToDecimal(dtResult.Rows[j][i].ToString().Trim());
                    }
                    drMSC[i] = total1.ToString();
                }
                drMSC[2] = drMSC[10] = "";
                dtResult.Rows.Add(drMSC);
            }

            // Legend 2017/02/04 添加判斷 當   drMSC  不為null時, 才可賦值
            if (drMSC != null)
            {
                //(YYYY/MM/DD起)- (YYYY/MM/DD迄)製成卡金額
                lblS_Numbers.Text = Convert.ToDecimal(drMSC["製成卡金額"].ToString()).ToString("N2");
                //(YYYY/MM/DD起)- (YYYY/MM/DD迄)耗損卡金額
                lblF_Numbers.Text = Convert.ToDecimal(drMSC["耗損卡金額"].ToString()).ToString("N2");
                //(YYYY/MM/DD起)- (YYYY/MM/DD迄)消耗卡金額
                lblUseOutNumber.Text = Convert.ToDecimal(drMSC["消耗卡金額"].ToString()).ToString("N2");
                //(YYYY/MM)委管庫存成本
                lblW_Number.Text = Convert.ToDecimal(drMSC["期末庫存未稅金額"].ToString()).ToString("N2");

                //銷毀卡，調整卡
                lblXH_Numer.Text = Convert.ToDecimal(drMSC["銷毀卡金額"].ToString()).ToString("N2");
                lblTZ_Numer.Text = Convert.ToDecimal(drMSC["調整卡金額"].ToString()).ToString("N2");
            }

            #endregion

            #region 設置表格標題
            if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡" || dropGroup.SelectedItem.Text == "DEBIT")
            {
                dtResult.Columns[2].ColumnName = "期初含稅單價";
                dtResult.Columns[10].ColumnName = "含稅單價";
                dtResult.Columns[11].ColumnName = "期初庫存金額(含稅)";
                dtResult.Columns[19].ColumnName = "期末庫存含稅金額";
            }
            else
            {
                dtResult.Columns[2].ColumnName = "期初未稅單價";
                dtResult.Columns[10].ColumnName = "未稅單價";
                dtResult.Columns[11].ColumnName = "期初庫存金額(未稅)";
                dtResult.Columns[19].ColumnName = "期末庫存未稅金額";
            }

            //顯示欄位           
            if (dropGroup.SelectedItem.Text != "晶片信用卡")
            {
                dtResult.Columns.Remove("製成卡數");
                dtResult.Columns.Remove("製成卡金額");
                dtResult.Columns.Remove("耗損卡數");
                dtResult.Columns.Remove("耗損卡金額");

                lblUseOutNum.Visible = true;
                lblUseOutNumber.Visible = true;
                lblS_Num.Visible = false;
                lblS_Numbers.Visible = false;
                lblF_Num.Visible = false;
                lblF_Numbers.Visible = false;
            }
            else
            {
                dtResult.Columns.Remove("消耗卡數");
                dtResult.Columns.Remove("消耗卡金額");

                lblUseOutNum.Visible = false;
                lblUseOutNumber.Visible = false;
                lblS_Num.Visible = true;
                lblS_Numbers.Visible = true;
                lblF_Num.Visible = true;
                lblF_Numbers.Visible = true;
            }
            #endregion

            #region 千分位
            foreach (DataRow dr1 in dtResult.Rows)
            {
                if (dr1[0].ToString() == "合計")
                {
                    if (dropGroup.SelectedItem.Text != "晶片信用卡")
                    {
                        dr1[1] = Convert.ToInt32(dr1[1].ToString()).ToString("N0");
                        for (i = 3; i < 8; i++)
                            dr1[i] = Convert.ToInt32(dr1[i].ToString()).ToString("N0");
                        for (i = 9; i <= dtResult.Columns.Count - 1; i++)
                            dr1[i] = Convert.ToDecimal(dr1[i].ToString()).ToString("N2");
                    }
                    else
                    {
                        dr1[1] = Convert.ToInt32(dr1[1].ToString()).ToString("N0");
                        for (i = 3; i < 9; i++)
                            dr1[i] = Convert.ToInt32(dr1[i].ToString()).ToString("N0");
                        for (i = 10; i <= dtResult.Columns.Count - 1; i++)
                            dr1[i] = Convert.ToDecimal(dr1[i].ToString()).ToString("N2");
                    }
                }
                else
                {
                    if (dropGroup.SelectedItem.Text != "晶片信用卡")
                    {
                        dr1[1] = Convert.ToInt32(dr1[1].ToString()).ToString("N0");
                        dr1[2] = Convert.ToDecimal(dr1[2].ToString()).ToString("N4");
                        dr1[8] = Convert.ToDecimal(dr1[8].ToString()).ToString("N4");
                        for (i = 3; i < 8; i++)
                            dr1[i] = Convert.ToInt32(dr1[i].ToString()).ToString("N0");
                        for (i = 9; i <= dtResult.Columns.Count - 1; i++)
                            dr1[i] = Convert.ToDecimal(dr1[i].ToString()).ToString("N2");
                    }
                    else
                    {
                        dr1[1] = Convert.ToInt32(dr1[1].ToString()).ToString("N0");
                        dr1[2] = Convert.ToDecimal(dr1[2].ToString()).ToString("N4");
                        dr1[9] = Convert.ToDecimal(dr1[9].ToString()).ToString("N4");
                        for (i = 3; i < 9; i++)
                            dr1[i] = Convert.ToInt32(dr1[i].ToString()).ToString("N0");
                        for (i = 10; i <= dtResult.Columns.Count - 1; i++)
                            dr1[i] = Convert.ToDecimal(dr1[i].ToString()).ToString("N2");
                    }
                }
            }
            #endregion

            //傳參數，添加報表數據
            Session["MonthStockCost"] = dtResult.Select("", "版面簡稱");

            return dtResult;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
            return null;
        }
    }

    private void Show_Report(string StartDate, string EndDate)
    {
        string strWN = null;

        lblTitle1.Text = dropGroup.SelectedItem.Text + "庫存殘值調整";

        //(YYYY/MM)委管庫存成本

        lblLast_W_Num.Text = DateTime.Parse(dropYear.SelectedValue + "/" + Convert.ToInt32(dropMonth.SelectedItem.Text)).AddMonths(-1).ToString("yyyy/MM") + "委管庫存成本";
        int month = (Convert.ToInt32(dropMonth.SelectedItem.Text) - 1);
        if (month == 0)
            strWN = bl.GetStockCostWNumber(Convert.ToString(Convert.ToInt32(dropYear.SelectedItem.Text) - 1) + "12", dropGroup.SelectedValue);
        else
        {
            if (month > 9)
                strWN = bl.GetStockCostWNumber(dropYear.SelectedItem.Text + month.ToString(), dropGroup.SelectedValue);
            else
                strWN = bl.GetStockCostWNumber(dropYear.SelectedItem.Text + "0" + month.ToString(), dropGroup.SelectedValue);
        }
        lblLast_W_Number.Text = Convert.ToDecimal(strWN).ToString("N2");

        //(YYYY/MM/DD起)- (YYYY/MM/DD迄)製成卡金額
        lblS_Num.Text = StartDate + "-" + EndDate + "製成卡金額";

        //(YYYY/MM/DD起)- (YYYY/MM/DD迄)耗損卡金額
        lblF_Num.Text = StartDate + "-" + EndDate + "耗損卡金額";

        //(YYYY/MM/DD起)- (YYYY/MM/DD迄)消耗卡金額
        lblUseOutNum.Text = StartDate + "-" + EndDate + "消耗卡金額";

        //(MM)月初迴轉金額
        lblBack_Num.Text = dropMonth.SelectedValue + "月初迴轉金額";
        if (month == 0)
            lblBack_Number.Text = bl.GetStockUnpayTNumber(Convert.ToString(Convert.ToInt32(dropYear.SelectedItem.Text) - 1) + "12", dropGroup.SelectedValue);
        else
        {
            if (month > 9)
                lblBack_Number.Text = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + month.ToString(), dropGroup.SelectedValue);
            else
                lblBack_Number.Text = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + "0" + month.ToString(), dropGroup.SelectedValue);
        }
        lblBack_Number.Text = Convert.ToDecimal(lblBack_Number.Text).ToString("N2");

        //(MM)月出帳付款金額
        lblP_Num.Text = dropMonth.SelectedValue + "月出帳付款金額";
        month = Convert.ToInt32(dropMonth.SelectedItem.Text);
        if (month > 9)
            lblP_Number.Text = bl.GetStockUnpayPNumber(dropYear.SelectedItem.Text + month.ToString(), dropGroup.SelectedValue);
        else
            lblP_Number.Text = bl.GetStockUnpayPNumber(dropYear.SelectedItem.Text + "0" + month.ToString(), dropGroup.SelectedValue);
        lblP_Number.Text = Convert.ToDecimal(lblP_Number.Text).ToString("N2");

        //(MM)月底提列金額
        lblT_Num.Text = dropMonth.SelectedValue + "月底提列金額";
        if (month > 9)
            lblT_Number.Text = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + month.ToString(), dropGroup.SelectedValue);
        else
            lblT_Number.Text = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + "0" + month.ToString(), dropGroup.SelectedValue);
        lblT_Number.Text = Convert.ToDecimal(lblT_Number.Text).ToString("N2");

        //(YYYY/MM)帳上庫存成本
        lblA_Num.Text = dropYear.SelectedValue + "/" + dropMonth.SelectedItem.Text + "帳上庫存成本";
        if (dropGroup.SelectedItem.Text == "晶片信用卡")
        {//上月委管庫存成本-本月製成卡金額-本月耗損卡金額+本月初迴轉金額(负值)+本月出帳付款金額+本月底提例金額
            lblA_Number.Text = Convert.ToDecimal(Convert.ToDecimal(lblLast_W_Number.Text) - Convert.ToDecimal(lblS_Numbers.Text) - Convert.ToDecimal(lblF_Numbers.Text) - Convert.ToDecimal(lblBack_Number.Text) + Convert.ToDecimal(lblP_Number.Text) + Convert.ToDecimal(lblT_Number.Text)).ToString("N2");
        }
        else
        {//上月委管庫存成本-本月消耗卡金額金額+本月初迴轉金額(负值)+本月出帳付款金額+本月底提例金額            
            lblA_Number.Text = Convert.ToDecimal(Convert.ToDecimal(lblLast_W_Number.Text) - Convert.ToDecimal(lblUseOutNumber.Text) - Convert.ToDecimal(lblBack_Number.Text) + Convert.ToDecimal(lblP_Number.Text) + Convert.ToDecimal(lblT_Number.Text)).ToString("N2");
        }

        //(YYYY/MM)委管庫存成本
        lblW_Num.Text = dropYear.SelectedValue + "/" + dropMonth.SelectedItem.Text + "委管庫存成本";

        //算
        if (lblXH_Numer.Text == "")
            lblXH_Numer.Text = "0";
        if (lblTZ_Numer.Text == "")
            lblTZ_Numer.Text = "";
        decimal declblA_Number = Convert.ToDecimal(lblA_Number.Text.Replace(",", "")) - Convert.ToDecimal(lblXH_Numer.Text.Replace(",", "")) - Convert.ToDecimal(lblTZ_Numer.Text.Replace(",", ""));
        lblA_Number.Text = declblA_Number.ToString("N2");

        //差異數
        lblD_Num.Text = "差異數";
        lblD_Number.Text = Convert.ToDecimal(Convert.ToDecimal(lblA_Number.Text) - Convert.ToDecimal(lblW_Number.Text)).ToString("N2");

        //銷毀卡
        lblXH_Num.Text = StartDate + "-" + EndDate + "銷毀卡金額";

        //調整卡
        lblTZ_Num.Text = StartDate + "-" + EndDate + "調整卡金額";

        //計算完差異數後，重新綁定GRIDVIEW
        isSecondBing = true;
        //gvpbMonthStockCost.BindData();
        //lblW_Number.Text = lblA_Number.Text;//更新庫存成本為帳上庫存成本
        //lblD_Number.Text  = "0.00";

        bl1.AddbillCycle(Convert.ToDateTime(StartDate).ToString("yyyyMMdd"), Convert.ToDateTime(EndDate).ToString("yyyyMMdd"), dropYear.SelectedItem.Text + dropMonth.SelectedItem.Text.PadLeft(2, '0'));
        //將本月的製成卡金額、耗損卡金額、帳上庫存成本、委管庫存成本新增至庫存成本檔
        if (month > 9)
            bl.Add(Convert.ToDecimal(lblS_Numbers.Text.Replace(",", "")), Convert.ToDecimal(lblF_Numbers.Text.Replace(",", "")), Convert.ToDecimal(lblA_Number.Text.Replace(",", "")), Convert.ToDecimal(lblW_Number.Text.Replace(",", "")), StartDate, EndDate, dropYear.SelectedItem.Text + month.ToString(), dropGroup.SelectedValue);
        else
            bl.Add(Convert.ToDecimal(lblS_Numbers.Text.Replace(",", "")), Convert.ToDecimal(lblF_Numbers.Text.Replace(",", "")), Convert.ToDecimal(lblA_Number.Text.Replace(",", "")), Convert.ToDecimal(lblW_Number.Text.Replace(",", "")), StartDate, EndDate, dropYear.SelectedItem.Text + "0" + month.ToString(), dropGroup.SelectedValue);

        //將負數項轉換為括號加（）
        if (lblD_Number.Text.Contains("-"))
        {
            lblD_Number.Text = "(" + lblD_Number.Text.Substring(1, lblD_Number.Text.Length - 1) + ")";
            lblD_Number.ForeColor = System.Drawing.Color.Red;
        }
        if (lblBack_Number.Text.Contains("-"))
        {
            lblBack_Number.Text = "(" + lblBack_Number.Text + ")";
        }
        lblBack_Number.ForeColor = System.Drawing.Color.Red;
    }

}
