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
using CIMSClass.Business;

public partial class InOut_InOut003 : PageBase
{
    InOut003BL BL = new InOut003BL();
    InOut007BL BL007 = new InOut007BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        gvCompare.NoneData = "";
        gvTotalCompare.NoneData = "";
        gvFaReplaceCompare.NoneData = "";
        btnSub.Visible = false;
        btnWafer.Visible = false;
        //btnMaterial.Visible = false;
        if (!IsPostBack)
        {
            //取最後一次日結日期
            lbDate.Text = ((DateTime)BL.getLastSurplusDate()).ToString("yyyy/MM/dd");

            //取最後一次日結日期的下一工作日
            if (lbDate.Text != "")
            {
                txt_Date.Text = ((DateTime)BL.getLastSurplusDateNext(Convert.ToDateTime(this.lbDate.Text))).ToString("yyyy/MM/dd");
            }
        }
    }

    /// <summary>
    /// 廠商異動訊息和系統訊息比對，將不相符合的記錄顯示出來
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCompare_Click(object sender, EventArgs e)
    {
        CompareDateSurplus(0);
    }

    /// <summary>
    /// 日結對比及日結處理
    /// </summary>
    /// <param name="intType"></param>
    private void CompareDateSurplus(int intType)
    {
        bool blDaySurplusStart = false;

        try
        {
            this.gvCompare.DataSource = null;
            this.gvTotalCompare.DataSource = null;
            this.gvCompare.DataBind();
            this.gvTotalCompare.DataBind();
            this.gvFaReplaceCompare.DataSource = null;
            this.gvFaReplaceCompare.DataBind();

            if (!BL.DaySurplusStart())
            {
                ShowMessage("日結處理已經開始，不能重復開始日結處理");
                return;
            }
            blDaySurplusStart = true;

            //檢查日結日期是否正確
            BL.CheckDateTime(Convert.ToDateTime(txt_Date.Text));
        
            ////200908CR比對替換前與替換后的廠商異動信息 ADD by 楊昆 2009/09/01 start
            //DataTable dtFaReplaceCompare = new DataTable();           
            //BL007.GetCompareFactoryReplace(Convert.ToDateTime(txt_Date.Text), ref dtFaReplaceCompare);
            //if (dtFaReplaceCompare.Rows.Count > 0)
            //{
            //    // 替換前與替換后的廠商異動不符信息
            //    gvFaReplaceCompare.DataSource = dtFaReplaceCompare.DefaultView;
            //    gvFaReplaceCompare.DataBind();
            //}
            ////200908CR比對替換前與替換后的廠商異動信息 ADD by 楊昆 2009/09/01 end

            //比對廠商匯入資料和系統對應項目
            DataTable dtFaSysCompare = new DataTable();
            DataTable dtSumCompare = new DataTable();
            BL.Compare(Convert.ToDateTime(txt_Date.Text),
                    Convert.ToDateTime(lbDate.Text), 
                    ref dtFaSysCompare, ref dtSumCompare);

            if (dtFaSysCompare.Rows.Count > 0)
            {
                // 廠商異動和系統異動不符記錄
                gvCompare.DataSource = dtFaSysCompare.DefaultView;
                gvCompare.DataBind();
            }

            if (dtSumCompare.Rows.Count > 0)
            {
                // 廠商結余匯總和系統結余匯總不符記錄
                gvTotalCompare.DataSource = dtSumCompare.DefaultView;
                gvTotalCompare.DataBind();
            }
           


            // 日結處理
            if (intType == 1)
            {
                //200908CR比對替換前與替換后的廠商異動信息 modified  by 楊昆 2009/09/01 start
                if (dtSumCompare.Rows.Count > 0 || dtFaSysCompare.Rows.Count > 0)//|| dtFaReplaceCompare.Rows.Count>0
                //200908CR比對替換前與替換后的廠商異動信息 modified  by 楊昆 2009/09/01 end
                {
                    BL.DaySurplusEnd();
                    ShowMessage("系統記錄資訊與廠商匯入資訊不合，不能日結");
                    return;
                }

                // 系統記錄資訊與廠商匯入資訊合,進行日結處理
                string strAlert = BL.DaySurplus(Convert.ToDateTime(txt_Date.Text));
                BL.DaySurplusEnd();

                //取最後一次日結日期
                lbDate.Text = ((DateTime)BL.getLastSurplusDate()).ToString("yyyy/MM/dd");

                //取最後一次日結日期的下一工作日
                if (lbDate.Text != "")
                {
                    txt_Date.Text = ((DateTime)BL.getLastSurplusDateNext(Convert.ToDateTime(this.lbDate.Text))).ToString("yyyy/MM/dd");
                }
                ShowMessage("日結成功！" + strAlert);
            }
            else {
                //200908CR比對替換前與替換后的廠商異動信息 modified  by 楊昆 2009/09/01 start
                if (dtSumCompare.Rows.Count > 0 || dtFaSysCompare.Rows.Count > 0)//|| dtFaReplaceCompare.Rows.Count > 0
                //200908CR比對替換前與替換后的廠商異動信息 modified  by 楊昆 2009/09/01 end
                {
                    //ShowMessage("系統記錄資訊與廠商匯入資訊不合，不能日結");
                    //return;
                }
                else {
                    BL.DaySurplusEnd();
                    ShowMessage("系統記錄資訊與廠商匯入資訊符合，比對成功。");
                    return;
                }
            }
            BL.DaySurplusEnd();
        }
        catch (Exception ex)
        {
            if (blDaySurplusStart)
                BL.DaySurplusEnd();
            ShowMessage(ex.Message);   
        }
    }

    /// <summary>
    /// 系統日結處理。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCheckDate_Click(object sender, EventArgs e)
    {
        CompareDateSurplus(1);
    }

    /// <summary>
    /// 取消日結處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelCheckDate_Click(object sender, EventArgs e)
    {
        bool blDaySurplusStart = false;
        try
        {
            //if (Convert.ToDateTime(txt_Date.Text).ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd"))
            //{
            //    ShowMessage("只能取消當日日結");
            //   return;
            //}

            // 檢查輸入日期是否可以取消日結
            if (BL.CancelCheck(Convert.ToDateTime(txt_Date.Text)))
            {
                if (!BL.DaySurplusStart())
                {
                    ShowMessage("日結處理已經開始，不能重復開始日結處理");
                    return;
                }
                blDaySurplusStart = true;

                // 取消日結
                BL.CancelDaySurplus(Convert.ToDateTime(txt_Date.Text));
                BL.DaySurplusEnd();

                //取最後一次日結日期
                lbDate.Text = ((DateTime)BL.getLastSurplusDate()).ToString("yyyy/MM/dd");

                //取最後一次日結日期的下一工作日
                if (lbDate.Text != "")
                {
                    txt_Date.Text = ((DateTime)BL.getLastSurplusDateNext(Convert.ToDateTime(this.lbDate.Text))).ToString("yyyy/MM/dd");
                }
                ShowMessage("取消日結成功");
            }
        }
        catch (Exception ex)
        {
            if (blDaySurplusStart)
                BL.DaySurplusEnd();
            ShowMessage(ex.Message);
        }
    }
    #endregion

    protected void btnSub_Click(object sender, EventArgs e)
    {
        try
        {
           
            InOut001BL BL001 = new InOut001BL();
            BL001.AddReplaceImp();
            ShowMessage("同步數據成功");
            ShowWait("0");
        }
        catch
        {
            ShowWait("0");
            ShowMessage("同步數據失敗");
        }
    }
    protected void btnWafer_Click(object sender, EventArgs e)
    {
        try
        {

            DateTime startday = Convert.ToDateTime("2009-10-26");
            //DateTime endday = Convert.ToDateTime("2009-10-26");
            DateTime endday = BL.getWAFERCARDTYPEDate();
            int t = Int32.Parse((endday - startday).Days.ToString());
            for (int m = 0; m <= t; m++)
            {
                BL.CancelWAFERCARDTYPE(endday.AddDays(-m));
            }
            //BL.DEL_Wafer_Uselog();

            for (int i = 0; i <= t; i++)
            {
                BL.DoWaferUsedLog(startday.AddDays(i));
            }
            ShowMessage("重算成功");         
            ShowWait("0");
        }
        catch
        {
            ShowWait("0");
            ShowMessage("重算失敗");
        }

    }

    protected void btnMaterial_Click(object sender, EventArgs e)
    {
        try
        {

            DateTime startday = Convert.ToDateTime("2009-09-01");
            DateTime endday = Convert.ToDateTime("2009-11-09");
           
            int t = Int32.Parse((endday - startday).Days.ToString());
            InOut000BL BL000 = new InOut000BL();            

            for (int i = 0; i <= t; i++)
            {
                BL000.SaveMaterielUsedCount(startday.AddDays(i));
            }
            ShowMessage("重算成功");
            ShowWait("0");
        }
        catch
        {
            ShowWait("0");
            ShowMessage("重算失敗");
        }

    }
}
