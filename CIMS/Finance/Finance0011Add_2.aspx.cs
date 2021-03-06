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
public partial class Finance_Finance0011Add_2 : PageBase
{
    Finance0011BL Finance0011BL = new Finance0011BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork"];
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs = (Dictionary<string, object>)Session["Condition"];
            if (dtRequisitionWork != null)//如果查到了資料
            {
                //刪除沒有資料的空余行
                DataRow[] drowsDEL = dtRequisitionWork.Select("是否請款 is null ");
                foreach (DataRow dr in drowsDEL)
                {
                    dtRequisitionWork.Rows.Remove(dr);
                }

                // 添加合計列
                // add chaoma by 201005515-0 start
                DataRow drNew = dtRequisitionWork.NewRow();
                //drNew["Name"] = "合計";
                drNew["Name"] = "罰款金額";
                drNew["Comment"] = "";
                dtRequisitionWork.Rows.Add(drNew);
                // 添加罰款列
                drNew = dtRequisitionWork.NewRow();
                //drNew["Name"] = "罰款金額";
                drNew["Name"] = "合計";
                drNew["Comment"] = "";
                dtRequisitionWork.Rows.Add(drNew);
                // add chaoma end
                
                // 添加輔助列
                if (dtRequisitionWork.Columns.Contains("Income_Number1") == false)
                {
                    dtRequisitionWork.Columns.Add(new DataColumn("Income_Number1", Type.GetType("System.Int32")));
                }
                Session["dtRequisitionWork"] = dtRequisitionWork;
                gvpbRequisitionWork.BindData();
            }
        }
    }

    /// <summary>
    /// 綁定請款訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbRequisitionWork_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork"];
        e.Table = dtRequisitionWork;//要綁定的資料表
        e.RowCount = dtRequisitionWork.Rows.Count;//查到的行數
    }

    /// <summary>
    /// 行綁定請款訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbRequisitionWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork"];
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // 實際請款數量
            TextBox txtRequisition_Count = (TextBox)e.Row.FindControl("txtRequisition_Count");
            txtRequisition_Count.Attributes.Add("Num", e.Row.RowIndex.ToString());

            // 含稅單價
            TextBox txtUnit_Price = (TextBox)e.Row.FindControl("txtUnit_Price");
            txtUnit_Price.Attributes.Add("Num",e.Row.RowIndex.ToString());

            // 罰款金額
            TextBox txtFineMoney = (TextBox)e.Row.FindControl("txtFineMoney");
            txtFineMoney.Attributes.Add("Num", e.Row.RowIndex.ToString());

            // 備註
            TextBox txtComment = (TextBox)e.Row.FindControl("txtComment");

            // 罰款金額、合計行時，“含稅單價”和“實際請款數量”不顯示。
            if (e.Row.Cells[0].Text.Trim() == "罰款金額" || 
                e.Row.Cells[0].Text.Trim()=="合計")
            {
                txtUnit_Price.Visible = false;
                txtRequisition_Count.Visible = false;
            }

            // 合計時，“備註”不顯示
            if (e.Row.Cells[0].Text == "合計")
            {
                txtComment.Visible = false;
            }

            if (e.Row.Cells[0].Text != "罰款金額")
            {
                txtFineMoney.Visible = false;
            }
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
                if (Convert.ToDateTime(dtRequisitionWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) != "1900/01/01")
                //if (dtRequisitionWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00")
                {
                    //TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(e.Row.Cells[7].Text).Ticks);
                    TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(e.Row.Cells[8].Text).Ticks);
                    TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(dtRequisitionWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString()).Ticks);
                    TimeSpan ts = ts1 - ts2;
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                    if (ts.Days < 0)
                    //    //e.Row.Cells[13].Text = "0";
                    //    e.Row.Cells[14].Text = "0";
                        e.Row.Cells[14].Text = "<font color='red'>" + "(" + ts.Days.ToString("N0").Replace("-", "") + ")" + " </font>";
                    else
                        //e.Row.Cells[13].Text = ts.Days.ToString("N0");
                        e.Row.Cells[14].Text = ts.Days.ToString("N0");
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 End
                }
                else
                    //e.Row.Cells[13].Text = "0";
                    e.Row.Cells[14].Text = "0";

            }else if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "再入庫";
                //e.Row.Cells[13].Text = "0";
                e.Row.Cells[14].Text = "0";
            }else if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "<font color='red'>退貨 </font>";
                e.Row.Cells[0].Text = "<font color='red'>" + e.Row.Cells[0].Text + " </font>";
                //e.Row.Cells[6].Text = "<font color='red'>-" + e.Row.Cells[6].Text + " </font>";
                e.Row.Cells[7].Text = "<font color='red'>-" + e.Row.Cells[7].Text + " </font>";
                e.Row.Cells[1].Text = "<font color='red'>" + e.Row.Cells[1].Text + " </font>";
                //e.Row.Cells[13].Text = "0";
                e.Row.Cells[14].Text = "0";
            }
            // add chaoma end

            // 顯示明細內容
            if (e.Row.Cells[0].Text != "罰款金額" && e.Row.Cells[0].Text != "合計")
            {
                // 相同入庫\再入庫\退貨單的數量只顯示一次
                if (e.Row.RowIndex != 0)
                {
                    if (dtRequisitionWork.Rows[e.Row.RowIndex - 1]["Operate_Type"].ToString() ==
                        dtRequisitionWork.Rows[e.Row.RowIndex]["Operate_Type"].ToString() &&
                        dtRequisitionWork.Rows[e.Row.RowIndex - 1]["Operate_RID"].ToString() ==
                        dtRequisitionWork.Rows[e.Row.RowIndex]["Operate_RID"].ToString()
                    )
                    {
                        // add chaoma by 201005515-0 start
                        e.Row.Cells[6].Text = "";
                        e.Row.Cells[7].Text = "";
                        // add chaoma end
                    }
                }

                // 含稅單價
                txtUnit_Price.Text = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString()).ToString("N4");

                // 未稅單價
                // add chaoma by 201005515-0 start
                //e.Row.Cells[9].Text = Convert.ToDecimal(Math.Round(Decimal.Parse(txtUnit_Price.Text.Trim().Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                e.Row.Cells[10].Text = Convert.ToDecimal(Math.Round(Decimal.Parse(txtUnit_Price.Text.Trim().Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");


                // 實際請款數量
                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Operate_Type"].ToString() == "3")
                {
                    txtRequisition_Count.Text = "-"+Convert.ToInt32(dtRequisitionWork.Rows[e.Row.RowIndex]["Income_Number"].ToString()).ToString("N0");    
                }
                else
                {
                    txtRequisition_Count.Text = Convert.ToInt32(dtRequisitionWork.Rows[e.Row.RowIndex]["Income_Number"].ToString()).ToString("N0");    
                }

                // 含稅總金額
                Label lblSumContanUnit_Price = (Label)e.Row.FindControl("lblSumContanUnit_Price");
                lblSumContanUnit_Price.Text = Convert.ToDecimal(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Replace(",", ""))).ToString("N0") + ".00";//金額取整后保留小數點兩位

                // 未稅總金額
                //e.Row.Cells[12].Text = Convert.ToDecimal(Decimal.Parse(e.Row.Cells[9].Text.Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Replace(",", ""))).ToString("N0") + ".00";//金額取整后保留小數點兩位
                e.Row.Cells[13].Text = Convert.ToDecimal(Decimal.Parse(e.Row.Cells[10].Text.Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Replace(",", ""))).ToString("N0") + ".00";//金額取整后保留小數點兩位

                // 備註
                txtComment.Text = dtRequisitionWork.Rows[e.Row.RowIndex]["Comment"].ToString().Trim();

                #region Legend 2017/07/07 當 Unit_Price&Change_UnitPrice 都不為null時才比對

                decimal decUnit_Price = 0;
                decimal decChange_UnitPrice = 0;

                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"] != null && dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString() != "")
                {
                    decUnit_Price = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString());
                }

                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Change_UnitPrice"] != null && dtRequisitionWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString() != "")
                {
                    decChange_UnitPrice = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString());
                }

                #endregion
                if (decUnit_Price != decChange_UnitPrice)
                {
                    ((TextBox)e.Row.Cells[9].Controls[1]).ForeColor = System.Drawing.Color.Red;
                } 

                // add chaoma end
            }
            
            // 計算合計金額、數量
            if (e.Row.Cells[0].Text == "合計")
            {
                // 實際請款數量
                int intSumRequisition_Count = 0;
                // 含稅總金額
                Decimal decSumContanUnit_Price = 0;
                // 未稅總金額
                Decimal decSumNotContanUnit_Price = 0;
                for (int intLoop = 0; intLoop < this.gvpbRequisitionWork.Rows.Count; intLoop++)
                {
                    if (this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text != "罰款金額" &&
                        this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text != "合計")
                    {
                        // 實際請款數量
                        txtRequisition_Count = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtRequisition_Count");
                        intSumRequisition_Count += Convert.ToInt32(txtRequisition_Count.Text.Replace(",",""));

                        // 含稅單價
                        txtUnit_Price = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtUnit_Price");
                        // 含稅總金額
                        decSumContanUnit_Price += Math.Round(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Replace(",", "")), MidpointRounding.AwayFromZero);


                        // 未稅總金額
                        // add chaoma by 201005515-0 start
                        //decSumNotContanUnit_Price += Math.Round(Decimal.Parse(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Replace(",", "")), MidpointRounding.AwayFromZero);
                        decSumNotContanUnit_Price += Math.Round(Decimal.Parse(this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text.Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Replace(",", "")), MidpointRounding.AwayFromZero);
                        // add chaoma end
                    }
                }

                ((Label)e.Row.FindControl("lblSumRequisition_Count")).Text= intSumRequisition_Count.ToString("N0");
                ((Label)e.Row.FindControl("lblSumContanUnit_Price")).Text = decSumContanUnit_Price.ToString("N0")+".00";
                // add chaoma by 201005515-0 start
                //e.Row.Cells[12].Text = decSumNotContanUnit_Price.ToString("N0")+".00";
                e.Row.Cells[13].Text = decSumNotContanUnit_Price.ToString("N0") + ".00";
                // add chaoma end
            }
        }
    }

    /// <summary>
    /// 計算合計數量、含稅總金額、未稅總金額
    /// </summary>
    protected void getSumNumberMoney()
    {
        // 實際請款數量
        int intSumRequisition_Count = 0;
        // 含稅總金額
        Decimal dblSumContanUnit_Price = 0;
        // 未稅總金額
        Decimal dblSumNotContanUnit_Price = 0;
        for (int intLoop = 0; intLoop < this.gvpbRequisitionWork.Rows.Count; intLoop++)
        {
            if (this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text != "罰款金額" &&
                this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text != "合計")
            {
                // 實際請款數量
                TextBox txtRequisition_Count = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtRequisition_Count");
                // 含稅單價
                TextBox txtUnit_Price = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtUnit_Price");
                // add chaoma by 201005515-0 start
                if (txtUnit_Price.Text.Trim() != "")
                {
                    //this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                    this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                }
                else {
                    //this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text = "0.0000";
                    this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text = "0.0000";
                }
                // add chaoma end
                if (txtRequisition_Count.Text.Trim() != "")
                {
                    intSumRequisition_Count += Convert.ToInt32(txtRequisition_Count.Text.Replace(",", ""));
                    if (txtUnit_Price.Text.Trim() != "")
                    {
                        // 含稅總金額
                        Label lblSumContanUnit_Price = (Label)this.gvpbRequisitionWork.Rows[intLoop].FindControl("lblSumContanUnit_Price");
                        lblSumContanUnit_Price.Text = Convert.ToDecimal(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0") + ".00";//金額取整后保留小數點兩位
                        //dblSumContanUnit_Price += Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""));
                        dblSumContanUnit_Price += Convert.ToDecimal(lblSumContanUnit_Price.Text.Trim().Replace(",", ""));
                        // 未稅總金額
                        // add chaoma by 201005515-0 start
                        //this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text = Convert.ToDecimal(Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0") + ".00";//金額取整后保留小數點兩位
                        this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text = Convert.ToDecimal(Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0") + ".00";//金額取整后保留小數點兩位
                        //dblSumNotContanUnit_Price +=Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""));
                        //dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text.Trim().Replace(",", ""));
                        dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text.Trim().Replace(",", ""));
                        // add chaoma end
                    }
                }
            }
            else if (this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text == "罰款金額")
            {
                TextBox txtFineMoney = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtFineMoney");
                //屏蔽下方代碼將將"罰款金額"從合計中排除
                //if (txtFineMoney.Text.Trim() != "")
                //{
                //    dblSumContanUnit_Price += Convert.ToDecimal(txtFineMoney.Text.Trim().Replace(",", ""));
                //}
                //if (this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text.Trim() != "")
                //{
                //    dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text.Trim().Replace(",", ""));
                //}
            }
            else if (this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text == "合計")
            {
                ((Label)this.gvpbRequisitionWork.Rows[intLoop].FindControl("lblSumRequisition_Count")).Text = intSumRequisition_Count.ToString("N0");
                ((Label)this.gvpbRequisitionWork.Rows[intLoop].FindControl("lblSumContanUnit_Price")).Text = dblSumContanUnit_Price.ToString("N0")+".00";
                // add chaoma by 201005515-0 start
                //this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text = dblSumNotContanUnit_Price.ToString("N0") + ".00";
                this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text = dblSumNotContanUnit_Price.ToString("N0") + ".00";
                // add chaoma end
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
        Response.Redirect("Finance0011Add.aspx?Con=2");
    }

    /// <summary>
    /// 含稅單價修改時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtUnit_Price_TextChanged(object sender, EventArgs e)
    {
        try
        {
            TextBox txtUnit_Price = (TextBox)sender;
            int intRowIndex = Convert.ToInt32(txtUnit_Price.Attributes["Num"].ToString().Replace(",", ""));
            if (txtUnit_Price.Text.Trim() == "")
            {
                ShowMessage("含稅單價不能為空。");
                return;
            }

            if (Convert.ToDouble(txtUnit_Price.Text.Replace(",", "")) < 0)
            {
                ShowMessage("含稅單價不能小於零。");
                return;
            }

            // 重新計算“合計”行的“實際請款數量”、“含稅總金額”、“未稅總金額”
            getSumNumberMoney();
        }
        catch { }
    }

    /// <summary>
    /// 罰款金額改變時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtFineMoney_TextChanged(object sender, EventArgs e)
    {
        try
        {
            TextBox txtFineMoney = (TextBox)sender;
            if (txtFineMoney.Text.Trim() == "")
                txtFineMoney.Text = "0";
            if (Convert.ToInt32(txtFineMoney.Text.Replace(",", "")) > 0)
            {
                txtFineMoney.Text = "-" + txtFineMoney.Text.Replace(",", "");
            }
            int intRowIndex = Convert.ToInt32(txtFineMoney.Attributes["Num"]);
            // add chaoma by 201005515-0 start
            //this.gvpbRequisitionWork.Rows[intRowIndex].Cells[12].Text = Convert.ToDecimal(Math.Round(Decimal.Parse(txtFineMoney.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N2");
            this.gvpbRequisitionWork.Rows[intRowIndex].Cells[13].Text = Convert.ToDecimal(Math.Round(Decimal.Parse(txtFineMoney.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N2");
            // add chaoma end

            getSumNumberMoney();
        }
        catch (Exception ex) {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 實際請款數量修改時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtRequisition_Count_TextChanged(object sender, EventArgs e)
    {
        try
        {
            DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork"];
            TextBox txtRequistion_Count = (TextBox)sender;
            int intRowIndex = Convert.ToInt32(txtRequistion_Count.Attributes["Num"].ToString());

            if (txtRequistion_Count.Text.Trim() == "" || txtRequistion_Count.Text.Trim() == "0")//IR-請款數量不能為空不能等於零
            {
                ShowMessage("實際請款數量不能為空,不能為零。");
                return;
            }

            if (dtRequisitionWork.Rows[intRowIndex]["Operate_Type"].ToString() == "1" ||
                dtRequisitionWork.Rows[intRowIndex]["Operate_Type"].ToString() == "2")
            {
                if (Convert.ToInt32(txtRequistion_Count.Text.Trim().Replace(",", "")) < 0)
                {
                    ShowMessage("實際請款數量不能小於零。");
                    return;
                }
            }
            else
            {
                if (Convert.ToInt32(txtRequistion_Count.Text.Trim().Replace(",", "")) > 0)
                {
                    txtRequistion_Count.Text = "-" + txtRequistion_Count.Text.Replace(",", "");
                }
            }

            getSumNumberMoney();
        }
        catch { }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        string strType = "1";//暫存
        AskMoneySubmitOROK(strType);
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOK1_Click(object sender, EventArgs e)
    {
        string strType = "3";//提交
        AskMoneySubmitOROK(strType);
        string[] arg = new string[1];
        Warning.SetWarning(GlobalString.WarningType.BlankCardMoney, arg);
    }

    /// <summary>
    /// 保存或提交請款SAP單
    /// </summary>
    /// <param name="strType"></param>
    private void AskMoneySubmitOROK(string strType)
    {
        CARD_TYPE_SAP ctsModel = new CARD_TYPE_SAP();
        DataTable dtRequisitionWork = (DataTable)this.Session["dtRequisitionWork"];

        try
        {
            this.btnSubmit1.Enabled = false;
            this.btnOK1.Enabled = false;
            this.btnOK2.Enabled = false;
            this.btnSubmit2.Enabled = false;

            #region 檢查頁面修改訊息正確性
            // SAP單號檢查
            if (StringUtil.IsEmpty(this.txtSAP_Serial_Number.Text.Trim()))
            {
                ShowMessage("SAP單號不能為空，請輸入。");
                this.btnSubmit1.Enabled = true;
                this.btnOK1.Enabled = true;
                this.btnOK2.Enabled = true;
                this.btnSubmit2.Enabled = true;
                return;
            }

            //if (this.txtSAP_Serial_Number.Text.Trim().Length!=15)
            //{
            //    ShowMessage("SAP單號必須是15位字符。");
            //    this.btnSubmit1.Enabled = true;
            //    this.btnOK1.Enabled = true;
            //    this.btnOK2.Enabled = true;
            //    this.btnSubmit2.Enabled = true;
            //    return;
            //}

            // SAP單號重復性檢查
            if (!Finance0011BL.ConstrainSAP_ID(this.txtSAP_Serial_Number.Text.Trim()))
            {
                ShowMessage("SAP單號不能重復，請重新輸入。");
                this.btnSubmit1.Enabled = true;
                this.btnOK1.Enabled = true;
                this.btnOK2.Enabled = true;
                this.btnSubmit2.Enabled = true;
                return;
            }

            //*************準備SAP單訊息 start*************
            ctsModel.SAP_Serial_Number = this.txtSAP_Serial_Number.Text.Trim();
            ctsModel.Pass_Status = strType;//暫存
            TextBox txtFineMoney = (TextBox)this.gvpbRequisitionWork.Rows[this.gvpbRequisitionWork.Rows.Count - 1].FindControl("txtFineMoney");
            if (StringUtil.IsEmpty(txtFineMoney.Text))
            {
                ctsModel.Fine = 0;
            }
            else
            {
                ctsModel.Fine = Convert.ToInt32(txtFineMoney.Text.Trim().Replace(",", ""));
            }
            TextBox txtComment = (TextBox)this.gvpbRequisitionWork.Rows[this.gvpbRequisitionWork.Rows.Count - 1].FindControl("txtComment");
            ctsModel.Comment = txtComment.Text;
            //*************準備SAP單訊息 end*************

            //*************準備SAP單明細訊息 start*************
            int intOldSum = 0;
            int intUpdateSum = 0;
            for (int intLoop = 0; intLoop < this.gvpbRequisitionWork.Rows.Count - 2; intLoop++)
            {
                // 已稅單價
                TextBox txtUnit_Price = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtUnit_Price");
                if (StringUtil.IsEmpty(txtUnit_Price.Text))
                {
                    ShowMessage("已稅單價不能為空。");
                    this.btnSubmit1.Enabled = true;
                    this.btnOK1.Enabled = true;
                    this.btnOK2.Enabled = true;
                    this.btnSubmit2.Enabled = true;
                    return;
                }
                if (Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) < 0)
                {
                    ShowMessage("已經稅單價不能小於0。");
                    this.btnSubmit1.Enabled = true;
                    this.btnOK1.Enabled = true;
                    this.btnOK2.Enabled = true;
                    this.btnSubmit2.Enabled = true;
                    return;
                }

                // 實際數量
                TextBox txtRequisition_Count = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtRequisition_Count");
                if (StringUtil.IsEmpty(txtRequisition_Count.Text) || txtRequisition_Count.Text.Trim() == "0")//IR-請款數量不能為空不能等於零
                {
                    ShowMessage("實際請款數量不能為空，不能為零。");
                    this.btnSubmit1.Enabled = true;
                    this.btnOK1.Enabled = true;
                    this.btnOK2.Enabled = true;
                    this.btnSubmit2.Enabled = true;
                    return;
                }

                // 入庫、再入庫
                if (dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "1" ||
                    dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "2")
                {
                    if (Convert.ToDecimal(txtRequisition_Count.Text.Replace(",", "")) < 0)
                    {
                        ShowMessage("實際請款數量不能小於0。");
                        this.btnSubmit1.Enabled = true;
                        this.btnOK1.Enabled = true;
                        this.btnOK2.Enabled = true;
                        this.btnSubmit2.Enabled = true;
                        return;
                    }
                }
                // 退貨
                else if (dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "3")
                {
                    if (Convert.ToDecimal(txtRequisition_Count.Text.Replace(",", "")) > 0)
                    {
                        ShowMessage("退貨的實際請款數量不能大於0。");
                        this.btnSubmit1.Enabled = true;
                        this.btnOK1.Enabled = true;
                        this.btnOK2.Enabled = true;
                        this.btnSubmit2.Enabled = true;
                        return;
                    }
                }

                // 每一筆的開始
                // add chaoma by 201005515-0 start
                //if (this.gvpbRequisitionWork.Rows[intLoop].Cells[6].Text.Trim() != "")
                if (this.gvpbRequisitionWork.Rows[intLoop].Cells[7].Text.Trim() != "")
                // add chaoma end
                {
                    // 檢查修改後的請款數量總和是否和修改前的相等
                    if (intOldSum != intUpdateSum)
                    {
                        ShowMessage("拆分後的實際請款數量之和不等於拆分前的數量。");
                        this.btnSubmit1.Enabled = true;
                        this.btnOK1.Enabled = true;
                        this.btnOK2.Enabled = true;
                        this.btnSubmit2.Enabled = true;
                        return;
                    }

                    intOldSum = 0;
                    intUpdateSum = 0;

                    if (dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "3")
                    {
                        intOldSum -= Convert.ToInt32(dtRequisitionWork.Rows[intLoop]["Income_Number"].ToString());
                    }
                    else
                    {
                        intOldSum += Convert.ToInt32(dtRequisitionWork.Rows[intLoop]["Income_Number"].ToString());
                    }

                    intUpdateSum += Convert.ToInt32(txtRequisition_Count.Text.Replace(",", ""));
                }
                else
                {
                    if (dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "3")
                    {
                        intOldSum -= Convert.ToInt32(dtRequisitionWork.Rows[intLoop]["Income_Number"].ToString());
                    }
                    else
                    {
                        intOldSum += Convert.ToInt32(dtRequisitionWork.Rows[intLoop]["Income_Number"].ToString());
                    }
                    intUpdateSum += Convert.ToInt32(txtRequisition_Count.Text.Replace(",", ""));
                }

                // 備註
                TextBox txtCommentRow = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtComment");
                dtRequisitionWork.Rows[intLoop]["Unit_Price"] = txtUnit_Price.Text.Replace(",", "");
                dtRequisitionWork.Rows[intLoop]["Unit_Price1"] = Math.Round(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4);
                dtRequisitionWork.Rows[intLoop]["Income_Number1"] = Math.Abs(Int32.Parse(txtRequisition_Count.Text.Replace(",", "")));
                dtRequisitionWork.Rows[intLoop]["Comment"] = txtCommentRow.Text + " ";
            }
            // 檢查修改後的請款數量總和是否和修改前的相等
            if (intOldSum != intUpdateSum)
            {
                ShowMessage("拆分後的實際請款數量之和不等於拆分前的數量。");
                this.btnSubmit1.Enabled = true;
                this.btnOK1.Enabled = true;
                this.btnOK2.Enabled = true;
                this.btnSubmit2.Enabled = true;
                return;
            } 

            //*************準備SAP單明細訊息 end*************
            #endregion 檢查頁面修改訊息正確性

            // 有退貨記錄時，
            // 該筆退貨拆分後的每一筆的單價在退貨記錄關聯的進貨記錄及進貨記錄的拆分記錄中都有對應的，
            // 且退貨數量不能大於進貨數量
            DataTable dtCheck = dtRequisitionWork.Copy();
            foreach (DataRow drCheck in dtCheck.Rows)
            {
                if (drCheck["Name"].ToString() != "罰款金額" &&
                    drCheck["Name"].ToString() != "合計")
                {
                    drCheck["Income_Number"] = drCheck["Income_Number1"].ToString();    
                }
            }

            if (Finance0011BL.CheckSplitCancel(dtCheck) == false)
            {
                ShowMessage("無對應的入庫、再入庫拆分記錄");
                this.btnSubmit1.Enabled = true;
                this.btnOK1.Enabled = true;
                this.btnOK2.Enabled = true;
                this.btnSubmit2.Enabled = true;
                return;
            }

            // 保存SAP單訊息
            Finance0011BL.SaveSAP(ctsModel, dtRequisitionWork);
            string strTime = DateTime.Now.ToString("yyyyMMddhhmmss");
            Finance0011BL.ADD_CARD_YEAR_FORCAST_PRINTS(dtRequisitionWork, strTime);
            // 打開列印窗口
            Response.Write("<script>window.open('Finance0011Add_2Print.aspx?Time=" + strTime + "&SAP_Serial_Number=" + txtSAP_Serial_Number.Text.Trim() +
                                "&Comment=" +Server.UrlEncode(txtComment.Text) +
                                "&Fine=" + txtFineMoney.Text +
                                "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Finance0011Add.aspx?Con=2");
        }
        catch (Exception ex)
        {
            this.btnSubmit1.Enabled = true;
            this.btnOK1.Enabled = true;
            this.btnOK2.Enabled = true;
            this.btnSubmit2.Enabled = true;
            ShowMessage(ex.Message);
        }
    }
}
