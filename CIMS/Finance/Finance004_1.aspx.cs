//******************************************************************
//*  作    者：lantaosu
//*  功能說明：已入庫未請款明細查詢
//*  創建日期：2008-11-21
//*  修改日期：2008-11-25 12:00
//*  修改記錄：
//*            □2008-11-25
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


public partial class Finance_Finance004_1 : PageBase
{
    Finance004_1BL bl = new Finance004_1BL();    
    decimal Total = 0;
    decimal TotalMonth = 0;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbIncome.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //初始化頁面
            this.trResult.Visible = false;
            this.trResult1.Visible = false;

            DataSet CooperateBlankList = bl.GetCooperateBlankList();
            dropFactoryRID.DataSource = CooperateBlankList;
            dropFactoryRID.DataValueField = "RID";
            dropFactoryRID.DataTextField = "Factory_ShortName_CN";
            dropFactoryRID.DataBind();
            ListItem li = new ListItem("全部", "");
            dropFactoryRID.Items.Insert(0,li);
            

            DataSet Year = bl.GetYearList();
            dropYear.DataSource = Year;
            dropYear.DataValueField = "Year";
            dropYear.DataTextField = "Year";
            dropYear.DataBind();
            dropYear.SelectedValue = DateTime.Now.Year.ToString();

            dropMonth.SelectedValue = DateTime.Now.Month.ToString();

            DataSet Use=bl.GetUseList();
            dropUse.DataSource=Use;
            dropUse.DataValueField = "Param_Code";
            dropUse.DataTextField = "Param_Name";
            dropUse.DataBind();            

            DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
            dropGroup.DataSource = Group;
            dropGroup.DataValueField = "RID";
            dropGroup.DataTextField = "Group_Name";
            dropGroup.DataBind();
        }

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

        if (dropGroup.SelectedValue.Trim() == "")
        {
            ShowMessage("群組不能為空");
            return;
        }

        if (int.Parse(dropYear.SelectedItem.Text + dropMonth.SelectedValue.PadLeft(2, '0')) < 200905)
        {
            ShowMessage("只能執行2009/05之後的查詢");
            return;
        }

        //獲取帳務起迄日
        Date = bl.GetDateFromAndDateTo(dropYear.SelectedItem.Text, dropMonth.SelectedValue);
        if(Date==null){
            ShowMessage("卡片成本帳務期間未設定");
            return;
        }
        StartDate = Date[0];
        EndDate = Date[1];
        ViewState["DateFrom"] = StartDate;
        ViewState["DateTo"] = EndDate;

        //取得該日期區間内的第一個未日結日
        string strUnCheckDate = bl.GetUncheckDate(StartDate, EndDate);
        if (strUnCheckDate != "")
        {
            ShowMessage(strUnCheckDate + "後未日結");
            return;
        }
        else
        {
            int intmonth = Convert.ToInt32(dropMonth.SelectedItem.Text) - 1;
            if (intmonth == 0) intmonth = 12;
            lblTitle.Text = dropMonth.SelectedValue + "月份" + dropGroup.SelectedItem.Text + "已入庫未出帳控管表";
            lblLast_T_Num.Text = intmonth.ToString() + "月底提列金額";
            lblP_Num.Text = "減" + dropMonth.SelectedValue + "月已出帳付款金額";
            lblU_Num.Text = dropMonth.SelectedValue + "月已入庫金額";
            lblD_Num.Text = dropMonth.SelectedValue + "月差異數金額";
            lblT_Num.Text = dropMonth.SelectedValue + "月底提列金額";

            gvpbIncome.BindData();

            if (gvpbIncome.DataSource != null)
            {
                this.btnReport.Visible = true;
                gvpbIncome.Visible = true;
                //合計
                lblTotal.Text = Total.ToString("N0")+".00";

                //if (dropFactoryRID.SelectedValue == "")
                //{
                    //(MM)月底提列金額
                    DataSet dsTN = null;
                    int month = (Convert.ToInt32(dropMonth.SelectedItem.Text) - 1);
                    if (month == 0)
                        dsTN = bl.GetStockUnpayTNumber(Convert.ToString(Convert.ToInt32(dropYear.SelectedItem.Text) - 1) + "12",this.dropGroup.SelectedValue,dropFactoryRID.SelectedValue);
                    else
                    {
                        if (month > 9)
                            dsTN = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + month.ToString(), this.dropGroup.SelectedValue,this.dropFactoryRID.SelectedValue);
                        else
                            dsTN = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + "0" + month.ToString(), this.dropGroup.SelectedValue, this.dropFactoryRID.SelectedValue);
                    }
                    if (dsTN.Tables[0].Rows.Count > 0)
                        lblLast_T_Number.Text = Convert.ToDecimal(dsTN.Tables[0].Rows[0][0].ToString()).ToString("N2");
                    else
                        lblLast_T_Number.Text = "0.00";

                    //減(MM)月已出帳付款金額
                    DataTable dtPayMoney = bl.GetSumPayMoney(StartDate, EndDate, dropGroup.SelectedValue).Tables[0];
                    if (dropGroup.SelectedItem.Text == "DEBIT" || dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                        if (dtPayMoney.Rows.Count > 0 && dtPayMoney.Rows[0]["Sum_Money"].ToString() != "")
                            lblP_Number.Text = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Money"].ToString()).ToString("N2");
                        else
                            lblP_Number.Text = "0.00";
                    else
                        if (dtPayMoney.Rows.Count > 0 && dtPayMoney.Rows[0]["Sum_Money_No"].ToString() != "")
                            lblP_Number.Text = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Money_No"].ToString()).ToString("N2");
                        else
                            lblP_Number.Text = "0.00";

                    //罰款金額
                    decimal dFine = 0;
                    if (dtPayMoney.Rows.Count > 0)
                    {
                        if (ViewState["type"].ToString() == "1")
                        {
                            dFine = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Fine"].ToString());
                        }
                        else
                        {
                            dFine = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Fine"].ToString()) / 1.05M;
                        }
                    }

                    //加(MM)月已入庫金額
                    double dU_Number = 0.00;
                    //dU_Number = Convert.ToDouble(TotalMonth);//20090824IR
                    if (ViewState["type"].ToString() == "1")
                    {
                        dU_Number = (bl.GetSumStockMoney(StartDate, EndDate, dropGroup.SelectedValue) + bl.GetSumRestockMoney(StartDate, EndDate, dropGroup.SelectedValue) - bl.GetSumCancelMoney(StartDate, EndDate, dropGroup.SelectedValue));
                    }
                    else
                    {
                        // dU_Number = (bl.GetSumStockMoney(StartDate, EndDate, dropGroup.SelectedValue) + bl.GetSumRestockMoney(StartDate, EndDate, dropGroup.SelectedValue) - bl.GetSumCancelMoney(StartDate, EndDate, dropGroup.SelectedValue)) / 1.05;
                        dU_Number = (bl.GetSumStockMoneyNO(StartDate, EndDate, dropGroup.SelectedValue) + bl.GetSumRestockMoneyNO(StartDate, EndDate, dropGroup.SelectedValue) - bl.GetSumCancelMoneyNO(StartDate, EndDate, dropGroup.SelectedValue));
                    }
                    //dU_Number = Convert.ToDouble(TotalMonth) + Convert.ToDouble(lblP_Number.Text.ToString()) - Convert.ToDouble(lblLast_T_Number.Text.ToString());
                    lblU_Number.Text = dU_Number.ToString("N0") + ".00";
                    //查詢出帳日在日期期間内的請款SAP單明細，得到SAP單請款明細
                    DataSet SAP = bl.GetSapDetail(StartDate, EndDate, dropGroup.SelectedValue);
                    decimal SUM_Price = 0;
                    decimal SUM_Price_No = 0;
                    foreach (DataRow dr in SAP.Tables[0].Rows)
                    {
                        if (dr["Operate_Type"].ToString() == "1" || dr["Operate_Type"].ToString() == "2")
                        {
                            //SUM_Price_No += Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString());
                            //SUM_Price += Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString());
                            SUM_Price_No += Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString()), MidpointRounding.AwayFromZero);
                            SUM_Price += Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString()), MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            //SUM_Price_No -= Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString());
                            //SUM_Price -= Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString());
                            SUM_Price_No -= Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString()), MidpointRounding.AwayFromZero);
                            SUM_Price -= Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString()), MidpointRounding.AwayFromZero);
                        }
                    }
                    //減(MM)月差異數金額
                    if (dropGroup.SelectedItem.Text == "DEBIT" || dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                        //lblD_Number.Text = Convert.ToDecimal(SUM_Price - Convert.ToDecimal(lblP_Number.Text) + dFine).ToString("N2");
                        lblD_Number.Text = Convert.ToDecimal(SUM_Price - Convert.ToDecimal(lblP_Number.Text) ).ToString("N2");
                    else
                        //lblD_Number.Text = Convert.ToDecimal(SUM_Price_No - Convert.ToDecimal(lblP_Number.Text) + dFine).ToString("N2");
                        lblD_Number.Text = Convert.ToDecimal(SUM_Price_No - Convert.ToDecimal(lblP_Number.Text)).ToString("N2");
                     
                    //等於(MM)月底提列金額
                    //lblT_Number.Text = Convert.ToDecimal(Convert.ToDecimal(lblLast_T_Number.Text) - Convert.ToDecimal(lblP_Number.Text) + Convert.ToDecimal(lblU_Number.Text) + Convert.ToDecimal(lblD_Number.Text)).ToString("N2");
                    lblT_Number.Text = Convert.ToDecimal(Convert.ToDecimal(lblLast_T_Number.Text) - Convert.ToDecimal(lblP_Number.Text) + Convert.ToDecimal(lblU_Number.Text) - Convert.ToDecimal(lblD_Number.Text)).ToString("N2");

                    //檢查"(MM)月底提列金額"是否與該月未(含)稅總金額之合計相等
                    if (Convert.ToDecimal(lblT_Number.Text.Replace(",", "")) != Convert.ToDecimal(lblTotal.Text.Replace(",", "")))
                        ShowMessage("提列金額不符");

                    //儲存至Stock_Unpay檔
                    string strBlank_Factory_RID = this.dropFactoryRID.SelectedValue;
                    if (strBlank_Factory_RID == "") strBlank_Factory_RID = "0";
                    bl.AddbillCycle(Convert.ToDateTime(StartDate).ToString("yyyyMMdd"),Convert.ToDateTime(EndDate).ToString("yyyyMMdd"), dropYear.SelectedItem.Text + dropMonth.SelectedItem.Text.PadLeft(2, '0'));   
                    if (Convert.ToInt32(dropMonth.SelectedItem.Text) > 9)                        
                        bl.Add(Convert.ToDecimal(lblP_Number.Text.Replace(",", "")), Convert.ToDecimal(lblU_Number.Text.Replace(",", "")), Convert.ToDecimal(lblD_Number.Text.Replace(",", "")), Convert.ToDecimal(lblT_Number.Text.Replace(",", "")), dropGroup.SelectedValue, dropYear.SelectedItem.Text + dropMonth.SelectedItem.Text, strBlank_Factory_RID);
                    else
                        bl.Add(Convert.ToDecimal(lblP_Number.Text.Replace(",", "")), Convert.ToDecimal(lblU_Number.Text.Replace(",", "")), Convert.ToDecimal(lblD_Number.Text.Replace(",", "")), Convert.ToDecimal(lblT_Number.Text.Replace(",", "")), dropGroup.SelectedValue, dropYear.SelectedItem.Text + "0" + dropMonth.SelectedItem.Text, strBlank_Factory_RID);

                //}
                //將負數項轉換為括號加（）
                if (lblD_Number.Text.Contains("-"))
                {
                    lblD_Number.Text = "(" + lblD_Number.Text.Substring(1, lblD_Number.Text.Length - 1) + ")";
                    lblD_Number.ForeColor = System.Drawing.Color.Red;
                }
                //lblU_Number.Text = Convert.ToDecimal(lblU_Number.Text).ToString("N0") + ".00";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doSearch();", true);
                this.trResult.Visible = true;
                this.trResult1.Visible = true;
                //if (dropFactoryRID.SelectedValue == "")
                //{
                //    //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doSearch1();", true);
                //    this.trResult1.Visible = true;
                //}
                //else
                //{
                //    this.trResult1.Visible = false;
                //}
            }
            else
            {

                //this.trResult.Visible = false;
                //this.trResult1.Visible = false;
                //ShowMessage("查無資料");
                #region 若查不到已入庫未出帳的資料，下方提列金額仍需計算顯示
                
                this.btnReport.Visible = true;

                //合計
                lblTotal.Text = "0.00";

                //if (dropFactoryRID.SelectedValue == "")
                //{
                //(MM)月底提列金額
                DataSet dsTN = null;
                int month = (Convert.ToInt32(dropMonth.SelectedItem.Text) - 1);
                if (month == 0)
                    dsTN = bl.GetStockUnpayTNumber(Convert.ToString(Convert.ToInt32(dropYear.SelectedItem.Text) - 1) + "12", this.dropGroup.SelectedValue, dropFactoryRID.SelectedValue);
                else
                {
                    if (month > 9)
                        dsTN = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + month.ToString(), this.dropGroup.SelectedValue, this.dropFactoryRID.SelectedValue);
                    else
                        dsTN = bl.GetStockUnpayTNumber(dropYear.SelectedItem.Text + "0" + month.ToString(), this.dropGroup.SelectedValue, this.dropFactoryRID.SelectedValue);
                }
                if (dsTN.Tables[0].Rows.Count > 0)
                    lblLast_T_Number.Text = Convert.ToDecimal(dsTN.Tables[0].Rows[0][0].ToString()).ToString("N2");
                else
                    lblLast_T_Number.Text = "0.00";

                //減(MM)月已出帳付款金額
                DataTable dtPayMoney = bl.GetSumPayMoney(StartDate, EndDate, dropGroup.SelectedValue).Tables[0];
                if (dropGroup.SelectedItem.Text == "DEBIT" || dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                    if (dtPayMoney.Rows.Count > 0 && dtPayMoney.Rows[0]["Sum_Money"].ToString() != "")
                        lblP_Number.Text = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Money"].ToString()).ToString("N2");
                    else
                        lblP_Number.Text = "0.00";
                else
                    if (dtPayMoney.Rows.Count > 0 && dtPayMoney.Rows[0]["Sum_Money_No"].ToString() != "")
                        lblP_Number.Text = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Money_No"].ToString()).ToString("N2");
                    else
                        lblP_Number.Text = "0.00";

                //罰款金額
                decimal dFine = 0;
                if (dtPayMoney.Rows.Count > 0)
                {
                    if (ViewState["type"].ToString() == "1")
                    {
                        dFine = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Fine"].ToString());
                    }
                    else
                    {
                        dFine = Convert.ToDecimal(dtPayMoney.Rows[0]["Sum_Fine"].ToString()) / 1.05M;
                    }
                }

                //加(MM)月已入庫金額              
                lblU_Number.Text = "0.00";
                //查詢出帳日在日期期間内的請款SAP單明細，得到SAP單請款明細
                DataSet SAP = bl.GetSapDetail(StartDate, EndDate, dropGroup.SelectedValue);
                decimal SUM_Price = 0;
                decimal SUM_Price_No = 0;
                foreach (DataRow dr in SAP.Tables[0].Rows)
                {
                    if (dr["Operate_Type"].ToString() == "1" || dr["Operate_Type"].ToString() == "2")
                    {
                        //SUM_Price_No += Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString());
                        //SUM_Price += Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString());
                        SUM_Price_No += Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString()),MidpointRounding.AwayFromZero);
                        SUM_Price += Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString()),MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        //SUM_Price_No -= Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString());
                        //SUM_Price -= Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString());
                        SUM_Price_No -= Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price_No"].ToString()),MidpointRounding.AwayFromZero);
                        SUM_Price -= Math.Round(Convert.ToInt32(dr["Real_Ask_Number"].ToString()) * Convert.ToDecimal(dr["Unit_Price"].ToString()),MidpointRounding.AwayFromZero);
                    }
                }
                //減(MM)月差異數金額
                if (dropGroup.SelectedItem.Text == "DEBIT" || dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                    lblD_Number.Text = Convert.ToDecimal(SUM_Price - Convert.ToDecimal(lblP_Number.Text)).ToString("N2");//+ dFine
                else
                    lblD_Number.Text = Convert.ToDecimal(SUM_Price_No - Convert.ToDecimal(lblP_Number.Text)).ToString("N2");// + dFine

                //等於(MM)月底提列金額
                lblT_Number.Text = Convert.ToDecimal(Convert.ToDecimal(lblLast_T_Number.Text) - Convert.ToDecimal(lblP_Number.Text) + Convert.ToDecimal(lblU_Number.Text) - Convert.ToDecimal(lblD_Number.Text)).ToString("N2");
                              

                //儲存至Stock_Unpay檔
                string strBlank_Factory_RID = this.dropFactoryRID.SelectedValue;
                if (strBlank_Factory_RID == "") strBlank_Factory_RID = "0";
                bl.AddbillCycle(Convert.ToDateTime(StartDate).ToString("yyyyMMdd"), Convert.ToDateTime(EndDate).ToString("yyyyMMdd"), dropYear.SelectedItem.Text + dropMonth.SelectedItem.Text.PadLeft(2, '0'));
                if (Convert.ToInt32(dropMonth.SelectedItem.Text) > 9)
                    bl.Add(Convert.ToDecimal(lblP_Number.Text.Replace(",", "")), Convert.ToDecimal(lblU_Number.Text.Replace(",", "")), Convert.ToDecimal(lblD_Number.Text.Replace(",", "")), Convert.ToDecimal(lblT_Number.Text.Replace(",", "")), dropGroup.SelectedValue, dropYear.SelectedItem.Text + dropMonth.SelectedItem.Text, strBlank_Factory_RID);
                else
                    bl.Add(Convert.ToDecimal(lblP_Number.Text.Replace(",", "")), Convert.ToDecimal(lblU_Number.Text.Replace(",", "")), Convert.ToDecimal(lblD_Number.Text.Replace(",", "")), Convert.ToDecimal(lblT_Number.Text.Replace(",", "")), dropGroup.SelectedValue, dropYear.SelectedItem.Text + "0" + dropMonth.SelectedItem.Text, strBlank_Factory_RID);

                //}
                //將負數項轉換為括號加（）
                if (lblD_Number.Text.Contains("-"))
                {
                    lblD_Number.Text = "(" + lblD_Number.Text.Substring(1, lblD_Number.Text.Length - 1) + ")";
                    lblD_Number.ForeColor = System.Drawing.Color.Red;
                }
                this.trResult.Visible = true;
                this.trResult1.Visible = true;
                #endregion
            }
        } 
 
    }



    #region 列表數據綁定
    /// <summary>
    /// GridView數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbIncome_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;        
        string RID;
        string DateFrom=ViewState["DateFrom"].ToString();
        hidDateFrom.Value = ViewState["DateFrom"].ToString();
        string DateTo=ViewState["DateTo"].ToString();
        hidDateTo.Value = ViewState["DateTo"].ToString();
       

        RID = dropFactoryRID.SelectedValue;
        if (RID == "")
        {
            RID = "";
            DataSet dsF = bl.GetCooperateBlankList();
            foreach (DataRow dr in dsF.Tables[0].Rows)
            {
                RID += dr["RID"].ToString() + ",";
            }
            RID = RID.Substring(0, (RID.Length - 1));
        }   

        DataSet dstlIncome = null;
        //20150807 增加 DEBIT
        if (dropGroup.SelectedItem.Text == "DEBIT" || dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
        {
            gvpbIncome.Columns[6].HeaderText = "含稅單價";
            gvpbIncome.Columns[7].HeaderText = "含稅總金額";
            ViewState["type"] = "1";
        }
        else
        {
            gvpbIncome.Columns[6].HeaderText = "未稅單價";
            gvpbIncome.Columns[7].HeaderText = "未稅總金額";
            ViewState["type"] = "2";
        }
        

        try
        {
            dstlIncome = bl.List(RID,DateFrom,DateTo,dropGroup.SelectedValue, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            //if (dstlIncome!= null && dstlIncome.Tables[0].Rows.Count>0)//如果查到了資料
            if (dstlIncome != null )//如果查到了資料
            {
                e.Table = dstlIncome.Tables[0];//要綁定的資料表
                e.RowCount = dstlIncome.Tables[0].Rows.Count;                
            }            
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    /// <summary>
    /// GridView列數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbIncome_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtb = (DataTable)gvpbIncome.DataSource;
        DataSet dsName;
        DataSet dsFactory;
        Label Name = null;
        Label Factory = null;
        Label UnitPrice = null;
        Label Price = null;
        Label Income_Date = null;
        Hashtable ht = new Hashtable();
        DateTime DateFrom = DateTime.Parse(ViewState["DateFrom"].ToString());
        DateTime DateTo = DateTime.Parse(ViewState["DateTo"].ToString());

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {  
                if (dtb != null && dtb.Rows.Count > 0)
                {
                    Name = (Label)e.Row.FindControl("lblName");
                    dsName = bl.GetCardName(dtb.Rows[e.Row.RowIndex]["Space_Short_RID"].ToString());
                    Name.Text = dsName.Tables[0].Rows[0][0].ToString();

                    Factory = (Label)e.Row.FindControl("lblFactory");
                    dsFactory = bl.GetFactoryShortCName(dtb.Rows[e.Row.RowIndex]["Blank_Factory_RID"].ToString());
                    Factory.Text = dsFactory.Tables[0].Rows[0][0].ToString();

                    UnitPrice = (Label)e.Row.FindControl("lblUnitPrice");
                    Price = (Label)e.Row.FindControl("lblPrice");

                    Income_Date = (Label)e.Row.FindControl("lblIncome_Date");
                    Income_Date.Text = dtb.Rows[e.Row.RowIndex]["Income_Date"].ToString().Split(' ')[0];
                   
                    string strUnit_Price = dtb.Rows[e.Row.RowIndex]["Unit_Price"].ToString();
                    string strUnit_Price_No = dtb.Rows[e.Row.RowIndex]["Unit_Price_No"].ToString();
                    string strUnit_Price_Order = dtb.Rows[e.Row.RowIndex]["Unit_Price_Order"].ToString();
                    string strReal_Ask_Number = dtb.Rows[e.Row.RowIndex]["Real_Ask_Number"].ToString();
                    string strIncome_Number = dtb.Rows[e.Row.RowIndex]["Income_Number"].ToString();

                    decimal dUnit_Price = 0;
                    if (strUnit_Price != "")
                    {
                        dUnit_Price = Convert.ToDecimal(strUnit_Price);
                    }

                    decimal dUnit_Price_No = 0;
                    if (strUnit_Price_No != "")
                    {
                        dUnit_Price_No = Convert.ToDecimal(strUnit_Price_No);
                    }

                    decimal dUnit_Price_Order = 0;
                    if (strUnit_Price_Order != "")
                    {
                        dUnit_Price_Order = Convert.ToDecimal(strUnit_Price_Order);
                    }

                    decimal dReal_Ask_Number = 0;
                    if (strReal_Ask_Number != "")
                    {
                        dReal_Ask_Number = Convert.ToDecimal(strReal_Ask_Number);
                    }

                    decimal dIncome_Number = 0;
                    if (strIncome_Number != "")
                    {
                        dIncome_Number = Convert.ToDecimal(strIncome_Number);
                    }

                    //單價 
                    decimal dUnit_Price_Real = 0;
                    //數量(用於計算 含稅總金額 = 單價*數量)
                    decimal dNumber = 0;

                    if (dReal_Ask_Number == 0)//未請款
                    {
                        if (ViewState["type"].ToString() == "2")
                        {
                            dUnit_Price_Order = dUnit_Price_Order / 1.05M;
                        }
                        dNumber = dIncome_Number;
                        dUnit_Price_Real = Convert.ToDecimal(dUnit_Price_Order.ToString("F4"));
                    }
                    else//已請款
                    {
                        dNumber = dReal_Ask_Number;
                        //20150807 加入DEBIT 卡，用含稅計算
                        if (dropGroup.SelectedItem.Text == "DEBIT" || dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                        {
                            dUnit_Price_Real = dUnit_Price;
                        }
                        else
                        {
                            dUnit_Price_Real = dUnit_Price_No;
                        }
                    }
                    e.Row.Cells[5].Text = dNumber.ToString("N0");
                    UnitPrice.Text = dUnit_Price_Real.ToString("N4");

                    decimal dc = dUnit_Price_Real * dNumber;
                    //Price.Text = dc.ToString("N2");

                    //if (dtb.Rows[e.Row.RowIndex]["Type"].ToString() == "3") Total -= dc;
                    //else Total += dc; 
                   
                   
                    //Price.Text = dc.ToString("N0")+".00";
                    Price.Text = (Math.Round(dc, MidpointRounding.AwayFromZero)).ToString("N2");
                    if (dtb.Rows[e.Row.RowIndex]["Type"].ToString() == "3")
                    {
                        Total -= Convert.ToDecimal(Price.Text);
                    }
                    else
                    {
                        Total += Convert.ToDecimal(Price.Text);
                    }
                    DateTime IncomeDate;
                    if (Income_Date.Text != null && Income_Date.Text != "")
                    {
                        IncomeDate = DateTime.Parse(Income_Date.Text.ToString().Trim());
                        if (IncomeDate >= DateFrom && IncomeDate <= DateTo)
                        {
                            if (dtb.Rows[e.Row.RowIndex]["Type"].ToString() == "3")
                            {
                                TotalMonth -= Convert.ToDecimal(Price.Text);
                            }
                            else
                            {
                                TotalMonth += Convert.ToDecimal(Price.Text);
                            }
                        }
                                             
                    }
                   
                   


                    //若Type=='3',表示為退貨記錄，則需將該行的"進貨作業數量","請款數量"和"未稅總金額"加上括號，用紅色字顯示
                    if (dtb.Rows[e.Row.RowIndex]["Type"].ToString() == "3")
                    {

                        if (UnitPrice.Text != "&nbsp;")
                        {
                            UnitPrice.Text = "(" + UnitPrice.Text + ")";
                            UnitPrice.ForeColor = System.Drawing.Color.Red;
                        }
                        if (Price.Text != "&nbsp;")
                        {
                            Price.Text = "(" + Price.Text + ")";
                            Price.ForeColor = System.Drawing.Color.Red;
                        }

                        if (e.Row.Cells[5].Text != "&nbsp;")
                        {
                            e.Row.Cells[5].Text = "(" + e.Row.Cells[5].Text + ")";
                            e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        //將負數項轉換為括號加（）
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 2; i < e.Row.Cells.Count; i++)
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
    /// 查詢該用途的所有卡穜群組，放於【群組】下拉框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropUse_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (dropUse.SelectedValue != "")
        {
            DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
            dropGroup.DataSource = Group;
            dropGroup.DataValueField = "RID";
            dropGroup.DataTextField = "Group_Name";
            dropGroup.DataBind();
        }
        else
        {
            dropGroup.DataSource = "";
            dropGroup.DataBind();
            ListItem li2 = new ListItem("全部", "");
            dropGroup.Items.Insert(0, li2);
        }
    }

    /// <summary>
    /// 匯出報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        this.trResult.Visible = true;
        this.trResult1.Visible = true;
        //if (dropFactoryRID.SelectedValue == "")
        //{
        //    this.trResult1.Visible = true;
        //}
        //else
        //{
        //    this.trResult1.Visible = false;
        //}

        Session["Total"] = lblTotal.Text.Replace(",", "");
        Session["Last_T_Number"] = lblLast_T_Number.Text.Replace(",","");
        Session["P_Number"] = lblP_Number.Text.Replace(",","");
        Session["U_Number"] = lblU_Number.Text.Replace(",","");
        Session["D_Number"] = lblD_Number.Text.Replace(",","");
        Session["T_Number"] = lblT_Number.Text.Replace(",","");

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "exportExcel();", true);
    }
}
