//******************************************************************
//*  作    者：JunWang
//*  功能說明：請款放行作業邏輯 
//*  創建日期：2008-12-03
//*  修改日期：2008-12-03 9:00
//*  修改記錄：
//*            □2008-12-03
//*              1.創建 王俊
//*             2010/12/10  Ge.Song
//*                 RQ-2010-004324-000 空白卡請款-遲繳天數開放負數
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

public partial class Finance_Finance0011Add_1 : PageBase
{
    Finance0011BL Finance0011BL = new Finance0011BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable dtSplitWork = (DataTable)Session["dtSplitWork"];
            DataTable dtSplitWorkNew = dtSplitWork.Clone();

            if (dtSplitWork != null)//如果查到了資料
            {
                // 刪除沒有進行拆分的行
                DataRow[] drowsDEL = dtSplitWork.Select("拆單數量 is null ");
                foreach (DataRow dr in drowsDEL)
                {
                    dtSplitWork.Rows.Remove(dr);
                }

                // 按拆分數量復製相同的記錄
                foreach (DataRow drOld in dtSplitWork.Rows)
                {
                    for (int i = 0; i < Convert.ToInt16(drOld["拆單數量"]); i++)
                    {
                        DataRow drNew = dtSplitWorkNew.NewRow();
                        drNew.ItemArray = drOld.ItemArray;
                        dtSplitWorkNew.Rows.Add(drNew);
                    }
                }

                // 添加拆分列
                dtSplitWorkNew.Columns.Add(new DataColumn("Split_Num", Type.GetType("System.Int32")));
                // 對拆分記錄進行排序
                dtSplitWorkNew.DefaultView.Sort = "Factory_ShortName_CN,Stock_RID,Income_Date,Operate_Type,Operate_RID";
                // 將拆分訊息保存到Session
                Session["dtSplitWork"] = dtSplitWorkNew.DefaultView.ToTable();
                gvpbSplitWork.BindData();
            }
        }
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0011Add.aspx?Con=1");
    }

    /// <summary>
    /// 保存拆分記錄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        #region 檢查拆分記錄
        int intSum = 0;
        DataTable dtSplitWork = (DataTable)this.Session["dtSplitWork"];
        // 檢查也面拆分訊息的正確性,拆分後的數量之是否等於拆分前的數量
        for (int intRow = 0; intRow < this.gvpbSplitWork.Rows.Count; intRow++)
        {
            TextBox txtUnit_Price = (TextBox)this.gvpbSplitWork.Rows[intRow].FindControl("txtUnit_Price");
            TextBox txtRequisition_Count = (TextBox)this.gvpbSplitWork.Rows[intRow].FindControl("txtRequisition_Count");
            if (txtUnit_Price.Text.Trim() == "" || Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) < 0)
            {
                ShowMessage("含稅單價必須輸入，且不能小於零。");
                return;
            }

            if (txtRequisition_Count.Text.Trim() == "" || txtRequisition_Count.Text.Trim() == "0")//IR-請款數量不能為空不能等於零
            {
                ShowMessage("實際請款數量必須輸入，且不能等於零。");
                return;
            }

            // 保存含稅單價
            dtSplitWork.Rows[intRow]["Unit_Price"] = Decimal.Parse(txtUnit_Price.Text.Replace(",",""));
            // 保存未稅單價
            // add chaoma by 201005515-0 start
            //dtSplitWork.Rows[intRow]["Unit_Price1"] = Decimal.Parse(this.gvpbSplitWork.Rows[intRow].Cells[9].Text.Replace(",", ""));
            dtSplitWork.Rows[intRow]["Unit_Price1"] = Decimal.Parse(this.gvpbSplitWork.Rows[intRow].Cells[10].Text.Replace(",", ""));
            // add chaoma end
            // 保存拆分後的數量
            dtSplitWork.Rows[intRow]["Split_Num"] = Math.Abs(int.Parse(txtRequisition_Count.Text.Replace(",", "")));
            // 保存備註
            dtSplitWork.Rows[intRow]["Comment"] = ((TextBox)this.gvpbSplitWork.Rows[intRow].FindControl("txtComment")).Text.Trim();
            // add chaoma by 201005515-0 start
            //if (this.gvpbSplitWork.Rows[intRow].Cells[6].Text != "" &&
            //    this.gvpbSplitWork.Rows[intRow].Cells[6].Text != "&nbsp;")
            if (this.gvpbSplitWork.Rows[intRow].Cells[7].Text != "" &&
                this.gvpbSplitWork.Rows[intRow].Cells[7].Text != "&nbsp;")
            {
                if (intSum == 0)
                {
                    if (dtSplitWork.Rows[intRow]["Operate_Type"].ToString() == "3")
                    {
                        intSum = int.Parse("-" + dtSplitWork.Rows[intRow]["Income_Number"].ToString().Replace(",", ""));
                    }
                    else
                    {
                        intSum = int.Parse(dtSplitWork.Rows[intRow]["Income_Number"].ToString().Replace(",", ""));                        
                    }
                    intSum -= int.Parse(txtRequisition_Count.Text.Replace(",", ""));
                }
                else
                {
                    ShowMessage("拆分後的實際請款數量之和必須等於拆分前的數量。");
                    return;
                }
            }
            else
            {
                intSum -= int.Parse(txtRequisition_Count.Text.Replace(",", ""));
            }
            // add chaoma end
        }

        if (intSum != 0)
        {
            ShowMessage("拆分後的實際請款數量之和必須等於拆分前的數量。");
            return;
        }

        #endregion 檢查拆分記錄

        //增加
        try
        {
            // 有退貨記錄時，
            // 該筆退貨拆分後的每一筆的單價在退貨記錄關聯的進貨記錄及進貨記錄的拆分記錄中都有對應的，
            // 且退貨數量不能大於進貨數量
            DataTable dtCheck = dtSplitWork.Copy();
            foreach (DataRow drCheck in dtCheck.Rows)
            {
                drCheck["Income_Number"] = drCheck["Split_Num"].ToString();
            }

            // 有退貨記錄時，
            // 該筆退貨拆分後的每一筆的單價在退貨記錄關聯的進貨記錄及進貨記錄的拆分記錄中都有對應的，
            // 且退貨數量不能大於進貨數量
            if (Finance0011BL.CheckSplitCancel(dtCheck) == false)
            {
                ShowMessage("無對應的入庫、再入庫拆分記錄");
                return;
            }

            // 將拆分記錄保存入資料庫
            Finance0011BL.SaveSplit(dtSplitWork);
            // 保存拆分訊息成功
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Finance0011Add.aspx?Con=1");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 綁定拆分記錄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSplitWork_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtSplitWork = (DataTable)Session["dtSplitWork"];
        this.Session["SplitWorkSelectNum"] = 0;
        if (null != (DataTable)Session["dtSplitWork"])
        {
            e.Table = dtSplitWork;//要綁定的資料表
            e.RowCount = dtSplitWork.Rows.Count;//查到的行數    
            this.Session["SplitWorkSelectNum"] = e.RowCount;
        }
    }

    /// <summary>
    /// 行綁定拆分記錄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSplitWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtSplitWork = (DataTable)this.Session["dtSplitWork"];
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (int.Parse(this.Session["SplitWorkSelectNum"].ToString()) == 0)
                return;

            // 不是第一行
            if (e.Row.RowIndex != 0)
            {
                // 如果該行和上一行的RID相同，則不顯示數量。
                if (dtSplitWork.Rows[e.Row.RowIndex - 1]["Operate_Type"].ToString() ==
                    dtSplitWork.Rows[e.Row.RowIndex]["Operate_Type"].ToString() &&
                    dtSplitWork.Rows[e.Row.RowIndex - 1]["Operate_RID"].ToString() ==
                    dtSplitWork.Rows[e.Row.RowIndex]["Operate_RID"].ToString()
                    )
                {
                    // add chaoma by 201005515-0 start
                    //e.Row.Cells[6].Text = "";
                    e.Row.Cells[6].Text = "";
                    e.Row.Cells[7].Text = "";
                    // add chaoma by end
                }
            }

            // 入庫日期格式化
            // add chaoma by 201005515-0 start
            //if (e.Row.Cells[7].Text != "&nbsp;")
            //{
            //    e.Row.Cells[7].Text = Convert.ToDateTime(e.Row.Cells[7].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            //}
            if (e.Row.Cells[8].Text != "&nbsp;")
            {
                e.Row.Cells[8].Text = Convert.ToDateTime(e.Row.Cells[8].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "入庫";
                if (dtSplitWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00")
                {
                    //TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(e.Row.Cells[7].Text).Ticks);
                    TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(e.Row.Cells[8].Text).Ticks);
                    TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(dtSplitWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString()).Ticks);
                    TimeSpan ts = ts1 - ts2;
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                    if (ts.Days < 0)
                    //    //e.Row.Cells[13].Text = "0";
                        //    e.Row.Cells[14].Text = "0";
                        e.Row.Cells[14].Text = "<font color='red'>" + "(" + ts.Days.ToString("N0").Replace("-", "") + ")" + " </font>";
                    else
                    //    //e.Row.Cells[13].Text = ts.Days.ToString("N0");
                        e.Row.Cells[14].Text = ts.Days.ToString("N0");
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                }
                else
                    //e.Row.Cells[13].Text = "0";
                    e.Row.Cells[14].Text = "0";
            }
            else if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "再入庫";
                //e.Row.Cells[13].Text = "0";
                e.Row.Cells[14].Text = "0";
            }
            else if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "<font color='red'>退貨</font>";
                e.Row.Cells[0].Text = "<font color='red'>" + e.Row.Cells[0].Text + "</font>";
                e.Row.Cells[1].Text = "<font color='red'>" + e.Row.Cells[1].Text + "</font>";
                //if (e.Row.Cells[6].Text != "")
                //    e.Row.Cells[6].Text = "<font color='red'>-" + e.Row.Cells[6].Text + "</font>";
                //e.Row.Cells[13].Text = "0";
                if (e.Row.Cells[7].Text != "")
                    // Legend 2017/06/07 賦值部分  將 6 改為 7  
                    e.Row.Cells[7].Text = "<font color='red'>-" + e.Row.Cells[7].Text + "</font>";
                e.Row.Cells[14].Text = "0";
            }
            // add chaoma end
            // 含稅單價
            TextBox txtUnit_Price = (TextBox)e.Row.FindControl("txtUnit_Price");
            txtUnit_Price.Text = Convert.ToDecimal(dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString()).ToString("N4");
            txtUnit_Price.Attributes.Add("Num", e.Row.RowIndex.ToString());

            // 未稅單價
            // add chaoma by 201005515-0 start
            //e.Row.Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",","")) / Decimal.Parse("1.05"), 4)).ToString("N4");
            e.Row.Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
            // add chaoma end
            // 實際請款數量
            TextBox txtRequisition_Count = (TextBox)e.Row.FindControl("txtRequisition_Count");
            txtRequisition_Count.Attributes.Add("Num", e.Row.RowIndex.ToString());

            // 備註
            TextBox txtComment = (TextBox)e.Row.FindControl("txtComment");
            txtComment.Text = dtSplitWork.Rows[e.Row.RowIndex]["Comment"].ToString().Trim();

            // add chaoma by 201005515-0 start

            #region Legend 2017/07/07 當 Unit_Price&Change_UnitPrice 都不為null時才比對

            decimal decUnit_Price = 0;
            decimal decChange_UnitPrice = 0;

            if (dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"] != null && dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString() != "")
            {
                decUnit_Price = Convert.ToDecimal(dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString());
            }

            if (dtSplitWork.Rows[e.Row.RowIndex]["Change_UnitPrice"] != null && dtSplitWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString() != "")
            {
                decChange_UnitPrice = Convert.ToDecimal(dtSplitWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString());
            }

            #endregion
            if (decUnit_Price != decChange_UnitPrice)
            {
                ((TextBox)e.Row.Cells[9].Controls[1]).ForeColor = System.Drawing.Color.Red;
            }
              
            // add chaoma end

        }
    }
    
    /// <summary>
    /// 含稅單價變化變化時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtUnit_Price_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string RowIndex = ((TextBox)sender).Attributes["Num"].ToString();
            int intRowIndex = int.Parse(RowIndex);

            // 已稅單價
            TextBox txtUnit_Price = (TextBox)gvpbSplitWork.Rows[intRowIndex].FindControl("txtUnit_Price");
            if (txtUnit_Price.Text.Trim() == "")
                txtUnit_Price.Text = "0";

            // 計算未稅單價
            // add chaoma by 201005515-0 start
            //gvpbSplitWork.Rows[intRowIndex].Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",","")) / Decimal.Parse("1.05"), 4)).ToString("N4");
            gvpbSplitWork.Rows[intRowIndex].Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
            
            // 實際請款數量
            TextBox txtRequisition_Count = (TextBox)gvpbSplitWork.Rows[intRowIndex].FindControl("txtRequisition_Count");

            // 計算實際請款總金額
            if (txtRequisition_Count.Text.Trim() != "")
            {
                // 計算已稅總金額
                //gvpbSplitWork.Rows[intRowIndex].Cells[11].Text = Convert.ToDecimal(Decimal.Parse(txtUnit_Price.Text.Replace(",","")) * Int32.Parse(txtRequisition_Count.Text.Replace(",",""))).ToString("N2");
                gvpbSplitWork.Rows[intRowIndex].Cells[12].Text = Convert.ToDecimal(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) * Int32.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N2");
                // 計算未稅總金額
                //gvpbSplitWork.Rows[intRowIndex].Cells[12].Text = Convert.ToDecimal(Decimal.Parse(gvpbSplitWork.Rows[intRowIndex].Cells[9].Text.Replace(",","")) * Int32.Parse(txtRequisition_Count.Text.Replace(",",""))).ToString("N2");
                gvpbSplitWork.Rows[intRowIndex].Cells[13].Text = Convert.ToDecimal(Decimal.Parse(gvpbSplitWork.Rows[intRowIndex].Cells[10].Text.Replace(",", "")) * Int32.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N2");
            }
            // add chaoma end
        }
        catch { }
    }

    /// <summary>
    /// 實際請款數量變化時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtRequisition_Count_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string RowIndex = ((TextBox)sender).Attributes["Num"].ToString();
            int intRowIndex = int.Parse(RowIndex);

            // 已稅單價
            TextBox txtUnit_Price = (TextBox)gvpbSplitWork.Rows[intRowIndex].FindControl("txtUnit_Price");
            // 未稅單價
            // add chaoma by 201005515-0 start
            //string strUnit_Price1 = gvpbSplitWork.Rows[intRowIndex].Cells[9].Text.Replace(",","");
            string strUnit_Price1 = gvpbSplitWork.Rows[intRowIndex].Cells[10].Text.Replace(",", "");
            // add chaoma end
            // 實際請款數量
            TextBox txtRequisition_Count = (TextBox)gvpbSplitWork.Rows[intRowIndex].FindControl("txtRequisition_Count");

            DataTable dtSplitWork = (DataTable)this.Session["dtSplitWork"];
            if (txtRequisition_Count.Text.Trim() != "" &&
                txtUnit_Price.Text.Trim() != "")
            {
                // 退貨數量處理
                if (dtSplitWork.Rows[intRowIndex]["Operate_Type"].ToString() == "3")
                {
                    if (Int32.Parse(txtRequisition_Count.Text.Trim().Replace(",","")) > 0)
                    {
                        txtRequisition_Count.Text = "-" + txtRequisition_Count.Text;
                    }
                }
                // add chaoma by 201005515-0 start
                // 計算已稅總金額
                //gvpbSplitWork.Rows[intRowIndex].Cells[11].Text = Convert.ToDecimal(Decimal.Parse(txtUnit_Price.Text.Replace(",","")) * Int32.Parse(txtRequisition_Count.Text.Replace(",",""))).ToString("N2");
                gvpbSplitWork.Rows[intRowIndex].Cells[12].Text = Convert.ToDecimal(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) * Int32.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N2");
                // 計算未稅總金額
                //gvpbSplitWork.Rows[intRowIndex].Cells[12].Text = Convert.ToDecimal(Decimal.Parse(strUnit_Price1) * Int32.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N2");
                gvpbSplitWork.Rows[intRowIndex].Cells[13].Text = Convert.ToDecimal(Decimal.Parse(strUnit_Price1) * Int32.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N2");
                // add chaoma end
            }
        }
        catch { }
    }
}
