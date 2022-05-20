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

public partial class BasicInfo_BASEINFO_007 : PageBase
{
    BaseInfo007BL BaseInfo007BL = new BaseInfo007BL();
    bool isExists = false;
    List<DateTime> listObj = null;
    //ArrayList al = new ArrayList();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        calWorkDate.SelectedDates.Clear();
        if (!IsPostBack)
        {
            //ViewState["OldDate"] = al;
            Session.Add("SelectedDates", null);
            FillDropDate();
            QueryWork(DateTime.Now);

            if (btnSubmit1.Visible == false)
                calWorkDate.Enabled = false;
        }
    }

    protected void calWorkDate_SelectionChanged(object sender, EventArgs e)
    {
        string Is_Check = BaseInfo007BL.Is_Check(calWorkDate.SelectedDate);
        if (Is_Check != "")//判斷是否日結
        {
            if (Session["SelectedDates"] != null)
            {
                listObj = (List<DateTime>)Session["SelectedDates"];
            }
            else
            {
                listObj = new List<DateTime>();
            }

            calWorkDate.SelectedDates.Clear();

            for (int i = 0; i < listObj.Count; i++)
            {
                this.calWorkDate.SelectedDates.Add(listObj[i]);
            }

            Session["SelectedDates"] = listObj;
            Session["TempDate"] = listObj;
            ShowMessage(Is_Check + "是日結日期");
        }
        else
        {
            this.hidIsModif.Value = "1";//是否修改 是=1，否=0
            CalSelectChanged();
        }
    }

    private void CalSelectChanged()
    {
        if (Session["SelectedDates"] != null)
        {
            listObj = (List<DateTime>)Session["SelectedDates"];
        }
        else
        {
            listObj = new List<DateTime>();
        }
        DateTime selectedDate = new DateTime(this.calWorkDate.SelectedDate.Year, this.calWorkDate.SelectedDate.Month, this.calWorkDate.SelectedDate.Day);

        //al = (ArrayList)ViewState["OldDate"];
        //al.Add(selectedDate);
        //ViewState["OldDate"] = al;
        for (int i = 0; i < listObj.Count; i++)
        {
            if (listObj[i] == selectedDate)
            {

                listObj.RemoveAt(i);
                isExists = true;
                break;
            }
        }

        if (!isExists)
        {
            listObj.Add(selectedDate);
        }

        calWorkDate.SelectedDates.Clear();

        for (int i = 0; i < listObj.Count; i++)
        {
            this.calWorkDate.SelectedDates.Add(listObj[i]);
        }

        Session["SelectedDates"] = listObj;
        Session["TempDate"] = listObj;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        List<DateTime> SelectedDates = (List<DateTime>)Session["SelectedDates"];
       
        try
        {
            //修改後本月是休息日的保存
            //if (SelectedDates != null && SelectedDates.Count != 0)
            //{
            BaseInfo007BL.Save(SelectedDates, calWorkDate);
                ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
            //}
            //修改後本月全是工作日的保存
            //if (SelectedDates != null && SelectedDates.Count == 0)
            //{
            //    BaseInfo007BL.ChangeSave(calWorkDate);
            //    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
            //}
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

        QueryWork(calWorkDate.VisibleDate);
        this.hidIsModif.Value = "0";
        this.hidOkOrCancel.Value = "0";
    }

    protected void calWorkDate_DayRender(object sender, DayRenderEventArgs e)
    {
        if (e.Day.IsOtherMonth)
        {
            e.Cell.Controls.Clear();
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DateTime Date_Time = Convert.ToDateTime(Convert.ToDateTime(this.ddlYear.Text + "-" + this.ddlMonth.Text + "-01"));
        ChangeDate();
        QueryWork(Date_Time);
    }

    //點擊上一月及下一月時的查詢
    private void QueryWork(DateTime ReconvertDate_Time)
    {
        calWorkDate.SelectedDates.Clear();
        DateTime Date_Time = ReconvertDate_Time;
        listObj = new List<DateTime>();

        string datatime = ReconvertDate_Time.ToString("yyyyMM");
        DataRow dr;
        DataSet dsQueryByDate = BaseInfo007BL.QueryByDate(datatime);
        if (dsQueryByDate.Tables[0].Rows.Count != 0)
        {
            int dsQueryByDateCount = dsQueryByDate.Tables[0].Rows.Count;
            for (int i = 0; i < dsQueryByDateCount; i++)
            {
                dr = dsQueryByDate.Tables[0].Rows[i];
                Date_Time = Convert.ToDateTime(dr["Date_Time"]);
                this.calWorkDate.SelectedDates.Add(Date_Time);

                listObj.Add(Date_Time);

                Session["SelectedDates"] = listObj;
            }
        }
        else
        {
            Session.Remove("SelectedDates");
        }

        calWorkDate.VisibleDate = Date_Time;
        lblCurrDate.Text = Date_Time.ToString("yyyy年MM月");
    }
    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        CancelDate();
    }

    private void CancelDate()
    {
        DateTime Date_Time = Convert.ToDateTime(lblCurrDate.Text);
        QueryWork(Date_Time);
        string strYear = lblCurrDate.Text.Substring(0, 4);
        string strMonth = lblCurrDate.Text.Substring(5, 2);
        ddlYear.Text = strYear;
        ddlMonth.Text = strMonth;
        this.hidIsModif.Value = "0";
        this.hidOkOrCancel.Value = "0";
    }
    //下一年
    protected void lbtnNextYear_Click(object sender, EventArgs e)
    {
        ChangeDate("Year", "Next");
        this.lblCurrDate.Text = this.ddlYear.Text + "年" + this.ddlMonth.Text + "月";
    }
    //下一月
    protected void lbtnNextMonth_Click(object sender, EventArgs e)
    {
        ChangeDate("Month", "Next");
        this.lblCurrDate.Text = this.ddlYear.Text + "年" + this.ddlMonth.Text + "月";
    }
    //上一月
    protected void lbtnPrevMonth_Click(object sender, EventArgs e)
    {
        ChangeDate("Month", "Prev");
        this.lblCurrDate.Text = this.ddlYear.Text + "年" + this.ddlMonth.Text + "月";
    }
    //上一年
    protected void lbtnPrevYear_Click(object sender, EventArgs e)
    {
        ChangeDate("Year", "Prev");
        this.lblCurrDate.Text = this.ddlYear.Text + "年" + this.ddlMonth.Text + "月";
    }
    /// <summary>
    /// 查詢日期變化事件
    /// </summary>
    /// <param name=""></param> 
    /// <param name=""></param>
    private void ChangeDate()
    {
        string temp = this.hidOkOrCancel.Value;//確定或則取消，確定=1，取消=0
        List<DateTime> SelectedDates = (List<DateTime>)Session["SelectedDates"];
        if (temp == "1")
        {
            //if (SelectedDates != null && SelectedDates.Count != 0)
            //{
            BaseInfo007BL.Save(SelectedDates, calWorkDate);
                ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
            //}
            //if (SelectedDates != null && SelectedDates.Count == 0)
            //{
            //    BaseInfo007BL.ChangeSave(calWorkDate);
            //    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
            //}
        }
        else
        {
            calWorkDate.SelectedDates.Clear();
            Session.Remove("SelectedDates");
        }
        this.hidIsModif.Value = "0";
        this.hidOkOrCancel.Value = "0";
    }


    /// <summary>
    /// 日期變化事件
    /// </summary>
    /// <param name="IsMonthOrYear">是否月改變還是年改變</param> 
    /// <param name="IsNextOrPrev">是否上一年月或下一年月</param>
    private void ChangeDate(string IsMonthOrYear, string IsNextOrPrev)
    {
        string temp = this.hidOkOrCancel.Value;//確定或則取消，確定=1，取消=0

        DateTime Date_Time = Convert.ToDateTime(Convert.ToDateTime(this.ddlYear.Text + "-" + this.ddlMonth.Text + "-01"));
        List<DateTime> SelectedDates = (List<DateTime>)Session["SelectedDates"];
        try
        {
            if (IsMonthOrYear == "Month" && IsNextOrPrev == "Prev")
            {
                if (temp == "1")
                {
                    //if (SelectedDates != null && SelectedDates.Count != 0)
                   // {
                    BaseInfo007BL.Save(SelectedDates, calWorkDate);
                        ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    //if (SelectedDates != null && SelectedDates.Count == 0)
                    //{
                    //    BaseInfo007BL.ChangeSave(calWorkDate);
                    //    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    calWorkDate.VisibleDate = Date_Time.AddMonths(-1);
                }
                else
                {
                    calWorkDate.VisibleDate = Date_Time.AddMonths(-1);
                    calWorkDate.SelectedDates.Clear();
                    Session.Remove("SelectedDates");
                }
                UpdataDDL("Month");//當前設定日期隨上一月年或下一月年變動
            }
            else if (IsMonthOrYear == "Month" && IsNextOrPrev == "Next")
            {
                if (temp == "1")
                {
                    //if (SelectedDates != null && SelectedDates.Count != 0)
                    //{
                    BaseInfo007BL.Save(SelectedDates, calWorkDate);
                        ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    //if (SelectedDates != null && SelectedDates.Count == 0)
                    //{
                    //    BaseInfo007BL.ChangeSave(calWorkDate);
                    //    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    calWorkDate.VisibleDate = Date_Time.AddMonths(1);
                }
                else
                {
                    calWorkDate.VisibleDate = Date_Time.AddMonths(1);
                    calWorkDate.SelectedDates.Clear();
                    Session.Remove("SelectedDates");
                }
                UpdataDDL("Month");//當前設定日期隨上一月年或下一月年變動
            }
            else if (IsMonthOrYear == "Year" && IsNextOrPrev == "Prev")
            {
                if (temp == "1")
                {
                    //if (SelectedDates != null && SelectedDates.Count != 0)
                    //{
                    BaseInfo007BL.Save(SelectedDates, calWorkDate);
                        ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    //if (SelectedDates != null && SelectedDates.Count == 0)
                    //{
                    //    BaseInfo007BL.ChangeSave(calWorkDate);
                    //    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    calWorkDate.VisibleDate = Date_Time.AddYears(-1);
                }
                else
                {
                    calWorkDate.VisibleDate = Date_Time.AddYears(-1);
                    calWorkDate.SelectedDates.Clear();
                    Session.Remove("SelectedDates");
                }
                UpdataDDL("Year");//當前設定日期隨上一月年或下一月年變動
            }
            else if (IsMonthOrYear == "Year" && IsNextOrPrev == "Next")
            {
                if (temp == "1")
                {
                    //if (SelectedDates != null && SelectedDates.Count != 0)
                    //{
                    BaseInfo007BL.Save(SelectedDates, calWorkDate);
                        ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    //if (SelectedDates != null && SelectedDates.Count == 0)
                    //{
                    //    BaseInfo007BL.ChangeSave(calWorkDate);
                    //    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                    //}
                    calWorkDate.VisibleDate = Date_Time.AddYears(1);
                }
                else
                {
                    calWorkDate.VisibleDate = Date_Time.AddYears(1);
                    calWorkDate.SelectedDates.Clear();
                    Session.Remove("SelectedDates");
                }
                UpdataDDL("Year");//當前設定日期隨上一月年或下一月年變動
            }


        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

        QueryWork(calWorkDate.VisibleDate);
        this.hidIsModif.Value = "0";
        this.hidOkOrCancel.Value = "0";
    }
    #endregion

    #region 資料綁定
    private void FillDropDate()
    {
        // 2020改為2029 add judy 2018/05/03
        for (int y = 2008; y < 2029; y++)
        {//填充年下拉列表
            this.ddlYear.Items.Add(y.ToString());
        }
        for (int m = 1; m < 13; m++)
        {//填充月下拉列表
            if (m < 10)
            {
                this.ddlMonth.Items.Add("0" + m.ToString());
            }
            else
            {
                this.ddlMonth.Items.Add(m.ToString());
            }
        }
        string strYear = calWorkDate.TodaysDate.ToString("yyyy");
        string strMonth = calWorkDate.TodaysDate.ToString("MM");
        ddlYear.Text = strYear;
        ddlMonth.Text = strMonth;
        this.lblCurrDate.Text = this.ddlYear.Text + "年" + this.ddlMonth.Text + "月";
    }
    protected void UpdataDDL(string IsYearOrMonth)
    {
        string strYear = calWorkDate.VisibleDate.ToString("yyyy");
        string strMonth = calWorkDate.VisibleDate.ToString("MM");
        if (Convert.ToInt32(strYear) < 2008 && IsYearOrMonth == "Year")
        {
            ShowMessage("無當前時間的資料");
            calWorkDate.VisibleDate = calWorkDate.VisibleDate.AddYears(1);
            return;
        }
        else if (Convert.ToInt32(strYear) > 2028 && IsYearOrMonth == "Year")  // 2019改為2028 add judy 2018/05/03
        {
            ShowMessage("無當前時間的資料");
            calWorkDate.VisibleDate = calWorkDate.VisibleDate.AddYears(-1);
            return;
        }
        else if (Convert.ToInt32(strYear) < 2008 && IsYearOrMonth == "Month")
        {
            ShowMessage("無當前時間的資料");
            calWorkDate.VisibleDate = calWorkDate.VisibleDate.AddMonths(1);
            return;
        }
        else if (Convert.ToInt32(strYear) > 2028 && IsYearOrMonth == "Month")  // 2019改為2028 add judy 2018/05/03
        {
            ShowMessage("無當前時間的資料");
            calWorkDate.VisibleDate = calWorkDate.VisibleDate.AddMonths(-1);
            return;
        }
        ddlYear.Text = strYear;
        ddlMonth.Text = strMonth;
    }
    #endregion

}
