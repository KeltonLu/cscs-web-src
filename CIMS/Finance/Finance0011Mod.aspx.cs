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

public partial class Finance_Finance0011Mod : PageBase
{
    Finance0011BL Finance0011BL = new Finance0011BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strSAP_Serial_Number = Request.QueryString["SAP_Serial_Number"];
            if (StringUtil.IsEmpty(strSAP_Serial_Number))
            {
                return;
            }
            btnSubmit1.Enabled = false;
            btnOK1.Enabled = false;
            btnPass1.Enabled = false;
            btnUntread1.Enabled = false;
            btnSubmit2.Enabled = false;
            btnOK2.Enabled = false;
            btnPass2.Enabled = false;
            btnUntread2.Enabled = false;
            chkDel.Visible = false;


            // SAP單號
            this.txtSAP_Serial_Number.Text = strSAP_Serial_Number;
            // 取SAP單訊息
            DataTable dtRequisitionWork_SAP = Finance0011BL.getSAP(strSAP_Serial_Number);
            // 保存SAP單訊息
            Session["dtRequisitionWork_SAP"] = dtRequisitionWork_SAP;
            Session["OldSAP_Serial_Number"] = dtRequisitionWork_SAP.Rows[0]["SAP_Serial_Number"].ToString();
            // add chaoma by 201005515-0 start
            //請款日期
            this.txtAsk_Date.Text = Convert.ToDateTime(dtRequisitionWork_SAP.Rows[0]["Ask_Date"].ToString()).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            // add chaoma end
            //獲得sap單的rid
            if (dtRequisitionWork_SAP.Rows.Count > 0)
            {
                HiddenField_SAP_RID.Value = dtRequisitionWork_SAP.Rows[0]["RID"].ToString();
                string strStatus = dtRequisitionWork_SAP.Rows[0]["Pass_Status"].ToString();

                if (Convert.ToDateTime(dtRequisitionWork_SAP.Rows[0]["Pay_date"]).ToString("yyyy-MM-dd") == "1900-01-01")
                {
                    if (strStatus == "1" || strStatus == "2")    //暫存和退回
                    {
                        btnSubmit1.Enabled = true;
                        btnSubmit2.Enabled = true;
                        chkDel.Visible = true;
                        btnOK1.Enabled = true;
                        btnOK2.Enabled = true;
                    }
                    else if (strStatus == "3")                  //待放行
                    {
                        btnUntread1.Enabled = true;
                        btnUntread2.Enabled = true;
                        btnPass1.Enabled = true;
                        btnPass2.Enabled = true;
                    }
                    else                                        //已放行
                    {
                        btnUntread1.Enabled = true;
                        btnUntread2.Enabled = true;
                    }
                }
            }
            
            // 取SAP單明細訊息
            DataTable dtRequisitionWork_SAPDetail = Finance0011BL.getSAPDetail(strSAP_Serial_Number);
            dtRequisitionWork_SAPDetail.Columns.Add(new DataColumn("Income_Number1", Type.GetType("System.Int32")));
            Session["dtRequisitionWork_SAPDetail"] = dtRequisitionWork_SAPDetail;
            gvpbRequisitionWork.BindData();
            if (btnSubmit1.Visible == false)
                chkDel.Visible = false;
        }
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
            int intRowIndex = Convert.ToInt32(txtUnit_Price.Attributes["Num"].ToString());
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
                if (txtUnit_Price.Text.Trim() != "")
                {
                    // add chaoma by 201005515-0 start
                    //this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                    this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                    // add chaoma end
                }
                else
                {
                    // add chaoma by 201005515-0 start
                    //this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text = "0.0000";
                    this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text = "0.0000";
                    // add chaoma end
                }

                if (txtRequisition_Count.Text.Trim() != "")
                {
                    intSumRequisition_Count += int.Parse(txtRequisition_Count.Text.Replace(",", ""));
                    if (txtUnit_Price.Text.Trim() != "")
                    {
                        // 含稅總金額
                        Label lblSumContanUnit_Price = (Label)this.gvpbRequisitionWork.Rows[intLoop].FindControl("lblSumContanUnit_Price");
                        lblSumContanUnit_Price.Text = Convert.ToDecimal(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",","")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",",""))).ToString("N0")+".00";
                        //dblSumContanUnit_Price += Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""));
                        dblSumContanUnit_Price += Convert.ToDecimal(lblSumContanUnit_Price.Text.Trim().Replace(",", ""));
                        // 未稅總金額
                        // add chaoma by 201005515-0 start
                        //this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text = Convert.ToDecimal(Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0")+".00";
                        this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text = Convert.ToDecimal(Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0") + ".00";
                        //dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""));
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
                ((Label)this.gvpbRequisitionWork.Rows[intLoop].FindControl("lblSumContanUnit_Price")).Text = dblSumContanUnit_Price.ToString("N0") + ".00";
                // add chaoma by 201005515-0 start
                //this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text = dblSumNotContanUnit_Price.ToString("N0") + ".00";
                this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text = dblSumNotContanUnit_Price.ToString("N0") + ".00";
                // add chaoma end
            }
        }
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
            //this.gvpbRequisitionWork.Rows[intRowIndex].Cells[12].Text =
            //    Convert.ToDecimal(Math.Round(Decimal.Parse(txtFineMoney.Text) / Decimal.Parse("1.05"), 4)).ToString("N2");
            this.gvpbRequisitionWork.Rows[intRowIndex].Cells[13].Text =
                Convert.ToDecimal(Math.Round(Decimal.Parse(txtFineMoney.Text) / Decimal.Parse("1.05"), 4)).ToString("N2");
            // add chaoma end
            getSumNumberMoney();
        }
        catch { }
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
            DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork_SAPDetail"];
            TextBox txtRequistion_Count = (TextBox)sender;
            if (txtRequistion_Count.Text.Trim() == "")
            {
                ShowMessage("實際請款數量不能為空。");
                return;
            }

            int intRowIndex = int.Parse(txtRequistion_Count.Attributes["Num"].ToString());
            if (dtRequisitionWork.Rows[intRowIndex]["Operate_Type"].ToString() == "1" ||
                dtRequisitionWork.Rows[intRowIndex]["Operate_Type"].ToString() == "2")
            {
                if (int.Parse(txtRequistion_Count.Text.Trim().Replace(",", "")) < 0)
                {
                    ShowMessage("實際請款數量不能小於零。");
                    return;
                }
            }
            else
            {
                if (int.Parse(txtRequistion_Count.Text.Trim().Replace(",", "")) > 0)
                {
                    txtRequistion_Count.Text = "-" + txtRequistion_Count.Text;
                }
            }

            getSumNumberMoney();
        }
        catch { }
    }

    /// <summary>
    /// 請款記錄綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbRequisitionWork_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork_SAPDetail"];
        e.Table = dtRequisitionWork;//要綁定的資料表
        e.RowCount = dtRequisitionWork.Rows.Count;//查到的行數
    }

    /// <summary>
    /// 保存或提交請款SAP單
    /// </summary>
    /// <param name="strType"></param>
    private void AskMoneySubmitOROK(string strType)
    {
        CARD_TYPE_SAP ctsModel = new CARD_TYPE_SAP();
        DataTable dtRequisitionWork = (DataTable)this.Session["dtRequisitionWork_SAPDetail"];

        try
        {
            #region 檢查頁面修改訊息正確性
            //*************準備SAP單訊息 start*************
            // add chaoma by 201005515-0 start
            // SAP單號檢查
            if (StringUtil.IsEmpty(this.txtSAP_Serial_Number.Text.Trim()))
            {
                ShowMessage("SAP單號不能為空，請輸入。");
                return;
            }

            // SAP單號重復性檢查
            if (this.txtSAP_Serial_Number.Text.ToString().Trim() != Session["OldSAP_Serial_Number"].ToString().Trim())
            {
                if (!Finance0011BL.ConstrainSAP_ID(this.txtSAP_Serial_Number.Text.Trim()))
                {
                    ShowMessage("SAP單號不能重復，請重新輸入。");
                    return;
                }
            }

            // 發票號不能和代制費用請款發票號相重復
            DataTable dtCheckAllCheckSerialNumber = null;
            dtCheckAllCheckSerialNumber = Finance0011BL.getAllCheckSerialNumber();

            Label lblInvoiceNumber = new Label();
           
            for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
            {
                lblInvoiceNumber = (Label)gvpbRequisitionWork.Rows[i].FindControl("lbInvoiceNumber");

                if (dtCheckAllCheckSerialNumber != null &&
                    dtCheckAllCheckSerialNumber.Rows.Count > 0)
                {
                    DataRow[] drCheckSerialNumber = dtCheckAllCheckSerialNumber.Select("Check_Serial_Number = '" + lblInvoiceNumber.Text.Trim() + "'");
                    if (drCheckSerialNumber.Length > 0)
                    {
                        ShowMessage("發票號不能和代製費用請款已經存在的發票號相同。");
                        return;
                    }
                }

                
            }


            // add chaoma end

            ctsModel.RID = Convert.ToInt32(((DataTable)this.Session["dtRequisitionWork_SAP"]).Rows[0]["RID"].ToString());
            ctsModel.Pass_Status = strType;//暫存or提交

            // add chaoma by 201005515-0 start
            ctsModel.Ask_Date = Convert.ToDateTime(this.txtAsk_Date.Text.ToString());
            ctsModel.SAP_Serial_Number = this.txtSAP_Serial_Number.Text.ToString();
            // add chaoma end

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
            ctsModel.Comment = txtComment.Text + " ";
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
                    return;
                }
                if (Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) < 0)
                {
                    ShowMessage("已經稅單價不能小於0。");
                    return;
                }

                // 實際數量
                TextBox txtRequisition_Count = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtRequisition_Count");
                if (StringUtil.IsEmpty(txtRequisition_Count.Text))
                {
                    ShowMessage("實際請款數量不能為空。");
                    return;
                }

                // 入庫、再入庫
                if (dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "1" ||
                    dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "2")
                {
                    if (Convert.ToDecimal(txtRequisition_Count.Text.Replace(",", "")) < 0)
                    {
                        ShowMessage("實際請款數量不能小於0。");
                        return;
                    }
                }
                // 退貨
                else if (dtRequisitionWork.Rows[intLoop]["Operate_Type"].ToString() == "3")
                {
                    if (Convert.ToDecimal(txtRequisition_Count.Text.Replace(",", "")) > 0)
                    {
                        ShowMessage("退貨的實際請款數量不能大於0。");
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
                dtRequisitionWork.Rows[intLoop]["Unit_Price_No"] = Math.Round(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) / decimal.Parse("1.05"), 4);
                dtRequisitionWork.Rows[intLoop]["Income_Number1"] = Math.Abs(Int32.Parse(txtRequisition_Count.Text.Replace(",", "")));
                dtRequisitionWork.Rows[intLoop]["Comment"] = txtCommentRow.Text + " ";
                // add chaoma by 201005515-0 start
                lblInvoiceNumber = (Label)gvpbRequisitionWork.Rows[intLoop].FindControl("lbInvoiceNumber");
                dtRequisitionWork.Rows[intLoop]["Check_ID"] = lblInvoiceNumber.Text;
                // add chaoma end
            }

            // 檢查修改後的請款數量總和是否和修改前的相等
            if (intOldSum != intUpdateSum)
            {
                ShowMessage("拆分後的實際請款數量之和不等於拆分前的數量。");
                return;
            }
            //*************準備SAP單明細訊息 end*************
            #endregion 檢查頁面修改訊息正確性

            // 有退貨記錄時，
            // 該筆退貨拆分後的每一筆的單價在退貨記錄關聯的進貨記錄及進貨記錄的拆分記錄中都有對應的，
            // 且退貨數量不能大於進貨數量
            DataTable dtCheck = dtRequisitionWork.Copy();
            for (int int1 = 0; int1 < dtRequisitionWork.Rows.Count - 2; int1++)
            {
                dtCheck.Rows[int1]["Income_Number"] = dtCheck.Rows[int1]["Income_Number1"].ToString();
            }

            if (Finance0011BL.CheckSplitCancel(dtCheck) == false)
            {
                ShowMessage("無對應的入庫、再入庫拆分記錄");
                return;
            }

            // 保存SAP單訊息
            Finance0011BL.UpdateSAP(ctsModel, dtRequisitionWork);

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Finance0011.aspx?Con=1");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 放行或退回操作
    /// </summary>
    /// <param name="strType"></param>
    private void AuditAskMoney(string strType)
    {
        try
        {
            Finance0011BL.Pass_Untread(Convert.ToInt32(this.HiddenField_SAP_RID.Value.ToString()), strType);

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Finance0011.aspx?Con=1");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 請款記錄行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbRequisitionWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork_SAPDetail"];
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // add chaoma by 201005515-0 start
            if (e.Row.Cells[14].Text != "&nbsp;" || int.Parse(dtRequisitionWork.Rows[e.Row.RowIndex]["stock_rid"].ToString().Substring(0, 8)) < 20080901)
            {
                // 發票CheckBox
                CheckBox cbInvoiceNumber = (CheckBox)e.Row.FindControl("cbInvoiceNumber");
                Label lbInvoiceNumber = (Label)e.Row.FindControl("lbInvoiceNumber");
                lbInvoiceNumber.Text = dtRequisitionWork.Rows[e.Row.RowIndex]["Check_ID"].ToString();
                lbInvoiceNumber.Visible = true;
            }

            // add chaoma end


            // 實際請款數量
            TextBox txtRequisition_Count = (TextBox)e.Row.FindControl("txtRequisition_Count");
            txtRequisition_Count.Attributes.Add("Num", e.Row.RowIndex.ToString());

            // 罰款金額、合計行時，“含稅單價”和“實際請款數量”不顯示。
            if (e.Row.Cells[0].Text.Trim() == "罰款金額" ||
                e.Row.Cells[0].Text.Trim() == "合計")
            {
                TextBox txtUnit_Price = (TextBox)e.Row.FindControl("txtUnit_Price");
                txtUnit_Price.Visible = false;

                TextBox txtRequisition_Count1 = (TextBox)e.Row.FindControl("txtRequisition_Count");
                txtRequisition_Count1.Visible = false;

                // add chaoma by 201005515-0 start
                CheckBox cbInvoiceNumber = (CheckBox)e.Row.FindControl("cbInvoiceNumber");
                cbInvoiceNumber.Visible = false;
                // add chaoma end
            }

            // 合計時，“備註”不顯示
            if (e.Row.Cells[0].Text == "合計")
            {
                TextBox txtComment = (TextBox)e.Row.FindControl("txtComment");
                txtComment.Visible = false;
            }

            TextBox txtFineMoney = (TextBox)e.Row.FindControl("txtFineMoney");
            txtFineMoney.Attributes.Add("Num", e.Row.RowIndex.ToString());

            if (e.Row.Cells[0].Text != "罰款金額")
            {
                //TextBox txtFineMoney = (TextBox)e.Row.FindControl("txtFineMoney");
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
            // add chaoma end
            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "入庫";
                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00")
                {
                    // add chaoma by 201005515-0 start
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
                    //    //e.Row.Cells[13].Text = ts.Days.ToString("N0");
                        e.Row.Cells[14].Text = ts.Days.ToString("N0");
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 End
                }
                else
                    //e.Row.Cells[13].Text = "0";
                    e.Row.Cells[14].Text = "0";
                     // add chaoma end
            }
            else if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "再入庫";
                // add chaoma by 201005515-0 start
                //e.Row.Cells[13].Text = "0";
                e.Row.Cells[14].Text = "0";
                // add chaoma end
            }
            else if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "<font color='red'>退貨 </font>";
                e.Row.Cells[0].Text = "<font color='red'>" + e.Row.Cells[0].Text + " </font>";
                // add chaoma by 201005515-0 start
                //e.Row.Cells[6].Text = "<font color='red'>-" + e.Row.Cells[6].Text + " </font>";
                // Legend 2017/06/07 賦值部分  將 6 改為 7
                // Legend 2017/08/15 將 "</font>"前面留一個空格, 否則替換時會報錯
                e.Row.Cells[7].Text = "<font color='red'>-" + e.Row.Cells[7].Text + " </font>";
                e.Row.Cells[1].Text = "<font color='red'>" + e.Row.Cells[1].Text + " </font>";
                //e.Row.Cells[13].Text = "0";
                e.Row.Cells[14].Text = "0";
                // add chaoma end
            }

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
                TextBox txtUnit_Price = (TextBox)e.Row.FindControl("txtUnit_Price");
                txtUnit_Price.Text = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString()).ToString("N4");
                txtUnit_Price.Attributes.Add("Num", e.Row.RowIndex.ToString());
                // add chaoma by 201005515-0 start
                //e.Row.Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Trim()) / decimal.Parse("1.05"), 4)).ToString("N4");
                e.Row.Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Trim()) / decimal.Parse("1.05"), 4)).ToString("N4");
                // add chaoma end
                // 實際請款數量
                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Operate_Type"].ToString() == "3")
                {
                    txtRequisition_Count.Text = "-" + Convert.ToInt32(dtRequisitionWork.Rows[e.Row.RowIndex]["Income_Number"].ToString()).ToString("N0");
                }
                else
                {
                    txtRequisition_Count.Text = Convert.ToInt32(dtRequisitionWork.Rows[e.Row.RowIndex]["Income_Number"].ToString()).ToString("N0");
                }

                // 含稅總金額
                Label lblSumContanUnit_Price = (Label)e.Row.FindControl("lblSumContanUnit_Price");
                lblSumContanUnit_Price.Text = Convert.ToDecimal(Decimal.Parse(txtUnit_Price.Text.Replace(",", "")) * int.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N0")+".00";

                // 未稅總金額
                // add chaoma by 201005515-0 start
                //e.Row.Cells[12].Text = Convert.ToDecimal(Decimal.Parse(e.Row.Cells[9].Text.Replace(",", "")) * int.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N0")+".00";
                e.Row.Cells[13].Text = Convert.ToDecimal(Decimal.Parse(e.Row.Cells[10].Text.Replace(",", "")) * int.Parse(txtRequisition_Count.Text.Replace(",", ""))).ToString("N0") + ".00";

                //當不是退貨的時候
                if (e.Row.Cells[5].Text != "<font color='red'>退貨 </font>")
                {
                    #region Legend 2017/06/07 當 Unit_Price&Change_UnitPrice 都不為null時才比對

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
                      
                }
                // add chaoma end
                // 備註
                TextBox txtComment = (TextBox)e.Row.FindControl("txtComment");
                txtComment.Text = dtRequisitionWork.Rows[e.Row.RowIndex]["Comment"].ToString().Trim();
            }

            // 罰款金額
            if (e.Row.Cells[0].Text == "罰款金額")
            {
                // 罰款金額
                //TextBox txtFineMoney = (TextBox)e.Row.FindControl("txtFineMoney");
                if (Convert.ToDouble(((DataTable)this.Session["dtRequisitionWork_SAP"]).Rows[0]["Fine"].ToString()) > 0)
                {
                    txtFineMoney.Text = "-" + Convert.ToDecimal(((DataTable)this.Session["dtRequisitionWork_SAP"]).Rows[0]["Fine"].ToString()).ToString("N0");
                }
                else
                {
                    txtFineMoney.Text = Convert.ToDecimal(((DataTable)this.Session["dtRequisitionWork_SAP"]).Rows[0]["Fine"].ToString()).ToString("N0");
                }

                // 备註
                TextBox txtComment = (TextBox)e.Row.FindControl("txtComment");
                txtComment.Text = ((DataTable)this.Session["dtRequisitionWork_SAP"]).Rows[0]["Comment"].ToString().Trim();

                // 罰款未稅金額
                if (txtFineMoney.Text.Trim() != "")
                {
                    // add chaoma by 201005515-0 start
                    //e.Row.Cells[12].Text = Convert.ToDecimal(Math.Round(Decimal.Parse(txtFineMoney.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N2");
                    e.Row.Cells[13].Text = Convert.ToDecimal(Math.Round(Decimal.Parse(txtFineMoney.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N2");
                    // add chaoma end
                }
            }

            // 計算合計金額、數量
            if (e.Row.Cells[0].Text == "合計")
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
                        txtRequisition_Count = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtRequisition_Count");
                        // 含稅單價
                        TextBox txtUnit_Price = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtUnit_Price");
                        if (txtUnit_Price.Text.Trim() != "")
                        {
                            // add chaoma by 201005515-0 start
                            //this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Replace(",","")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                            this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDecimal(txtUnit_Price.Text.Replace(",", "")) / Decimal.Parse("1.05"), 4)).ToString("N4");
                        }
                        else
                        {
                            //this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text = "0.0000";
                            this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text = "0.0000";
                            // add chaoma end
                        }

                        if (txtRequisition_Count.Text.Trim() != "")
                        {
                            intSumRequisition_Count += int.Parse(txtRequisition_Count.Text.Replace(",", ""));
                            if (txtUnit_Price.Text.Trim() != "")
                            {
                                // 含稅總金額
                                Label lblSumContanUnit_Price = (Label)this.gvpbRequisitionWork.Rows[intLoop].FindControl("lblSumContanUnit_Price");
                                lblSumContanUnit_Price.Text = Convert.ToDecimal(Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0")+".00";
                                //dblSumContanUnit_Price += Convert.ToDecimal(txtUnit_Price.Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""));
                                dblSumContanUnit_Price += Convert.ToDecimal(lblSumContanUnit_Price.Text.Trim().Replace(",", ""));
                                // 未稅總金額
                                // add chaoma by 201005515-0 start
                                //this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text = Convert.ToDecimal(Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0") + ".00";
                                this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text = Convert.ToDecimal(Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[10].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""))).ToString("N0") + ".00";
                                //dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[9].Text.Trim().Replace(",", "")) * Convert.ToInt32(txtRequisition_Count.Text.Trim().Replace(",", ""));
                                //dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text.Trim().Replace(",", ""));
                                dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[13].Text.Trim().Replace(",", ""));
                                // add chaoma end
                            }
                        }
                    }
                    else if (this.gvpbRequisitionWork.Rows[intLoop].Cells[0].Text == "罰款金額")
                    {
                        TextBox txtFineMoney1 = (TextBox)this.gvpbRequisitionWork.Rows[intLoop].FindControl("txtFineMoney");
                        //if (txtFineMoney1.Text.Trim() != "")
                        //{
                        //    dblSumContanUnit_Price += Convert.ToDecimal(txtFineMoney1.Text.Trim().Replace(",", ""));
                        //}
                        //if (this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text.Trim() != "")
                        //{
                        //    dblSumNotContanUnit_Price += Convert.ToDecimal(this.gvpbRequisitionWork.Rows[intLoop].Cells[12].Text.Trim().Replace(",", ""));
                        //}
                    }
                }

                ((Label)e.Row.FindControl("lblSumRequisition_Count")).Text = intSumRequisition_Count.ToString("N0");
                ((Label)e.Row.FindControl("lblSumContanUnit_Price")).Text = dblSumContanUnit_Price.ToString("N0") + ".00";
                // add chaoma by 201005515-0 start
                //e.Row.Cells[12].Text = dblSumNotContanUnit_Price.ToString("N0") + ".00";
                e.Row.Cells[13].Text = dblSumNotContanUnit_Price.ToString("N0") + ".00";
                // add chaoma end
            }
        }
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint1_Click(object sender, EventArgs e)
    {
        DataTable dtSplitWorkOld = (DataTable)Session["dtRequisitionWork_SAPDetail"];
        DataTable dtSplitWorkNew = dtSplitWorkOld.Clone();
        dtSplitWorkNew.Columns.Remove("Fore_Delivery_Date");

        DataColumn dclast = new DataColumn("未稅單價", typeof(string));
        DataColumn dclast1 = new DataColumn("實際請款數量", typeof(string));
        DataColumn dclast2 = new DataColumn("遲交天數", typeof(string));
        DataColumn dclast3 = new DataColumn("罰款金額", typeof(string));
        DataColumn dclast4 = new DataColumn("含稅總金額", typeof(string));
        DataColumn dclast5 = new DataColumn("未稅總金額", typeof(string));
        dtSplitWorkNew.Columns.Add(dclast);
        dtSplitWorkNew.Columns.Add(dclast1);
        dtSplitWorkNew.Columns.Add(dclast2);
        dtSplitWorkNew.Columns.Add(dclast3);
        dtSplitWorkNew.Columns.Add(dclast4);
        dtSplitWorkNew.Columns.Add(dclast5);
        int i = 0;
        foreach (GridViewRow GVdr in gvpbRequisitionWork.Rows)
        {


            //if (GVdr.Cells[0].Text == "合計")
            //{
            //    break;
            //}
            DataRow drNew = dtSplitWorkNew.NewRow();

            // add chaoma by 201005515-0 start
            if (GVdr.Cells[5].Text.Contains("入庫"))
            {

                //if (dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00" && dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString() != "&nbsp;" && dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString().Trim() != "" && GVdr.Cells[7].Text != "&nbsp;" && GVdr.Cells[7].Text.Trim() != "")
                if (dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00" && dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString() != "&nbsp;" && dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString().Trim() != "" && GVdr.Cells[8].Text != "&nbsp;" && GVdr.Cells[8].Text.Trim() != "")
                {
                    //TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(GVdr.Cells[7].Text).Ticks);
                    TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(GVdr.Cells[8].Text).Ticks);
                    TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(dtSplitWorkOld.Rows[i]["Fore_Delivery_Date"].ToString()).Ticks);
                    TimeSpan ts = ts1 - ts2;
                    if (ts.Days < 0)
                        drNew["Delay_Days"] = "0";
                    else
                        drNew["Delay_Days"] = ts.Days.ToString();
                }
                else
                    drNew["Delay_Days"] = "0";
            }
            else if (GVdr.Cells[5].Text.Contains("再入庫") || GVdr.Cells[5].Text.Contains("退貨"))
            {
                drNew["Delay_Days"] = "0";
            }

            drNew["NAME"] = GVdr.Cells[0].Text.Replace("<font color='red'>", "").Replace(" </font>", "");
            drNew["Factory_ShortName_CN"] = GVdr.Cells[1].Text.Replace("<font color='red'>", "").Replace(" </font>", "");
            drNew["Budget_ID"] = GVdr.Cells[2].Text;
            drNew["Agreement_Code"] = GVdr.Cells[3].Text;
            drNew["Stock_RID"] = GVdr.Cells[4].Text;
            drNew["Operate_Type"] = GVdr.Cells[5].Text.Replace("<font color='red'>", "").Replace(" </font>", "");
            //if (GVdr.Cells[6].Text.Replace("<font color='red'>", "").Replace(" </font>", "") != "-&nbsp;" && GVdr.Cells[6].Text.Replace("<font color='red'>", "").Replace(" </font>", "") != "&nbsp;" && GVdr.Cells[6].Text.Replace("<font color='red'>", "").Replace(" </font>", "") != "")
            if (GVdr.Cells[7].Text.Replace("<font color='red'>", "").Replace(" </font>", "") != "-&nbsp;" && GVdr.Cells[7].Text.Replace("<font color='red'>", "").Replace(" </font>", "") != "&nbsp;" && GVdr.Cells[7].Text.Replace("<font color='red'>", "").Replace(" </font>", "") != "")
            {
                //drNew["Income_Number"] = GVdr.Cells[6].Text.Replace("<font color='red'>", "").Replace(" </font>", "").Replace(",","");
                drNew["Income_Number"] = GVdr.Cells[7].Text.Replace("<font color='red'>", "").Replace(" </font>", "").Replace(",", "");
                drNew["Number"] = dtSplitWorkOld.Rows[i]["Number"].ToString();
            }
            else
            {
                drNew["Income_Number"] = 0;
                drNew["Number"] = 0;
            }
            //if (GVdr.Cells[7].Text != "&nbsp;")
            if (GVdr.Cells[8].Text != "&nbsp;")
            {
                //drNew["Income_Date"] = Convert.ToDateTime(GVdr.Cells[7].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                drNew["Income_Date"] = Convert.ToDateTime(GVdr.Cells[8].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                drNew["Income_Date"] = Convert.ToDateTime("1900/01/01").ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }

            TextBox txtUnit_Price = (TextBox)GVdr.FindControl("txtUnit_Price");
            if (txtUnit_Price.Text.Trim() != "")
            {
                drNew["Unit_Price"] = txtUnit_Price.Text.Replace(",", "");
            }

            //drNew["未稅單價"] = GVdr.Cells[9].Text.Replace(",", "");
            drNew["未稅單價"] = GVdr.Cells[10].Text.Replace(",", "");

            if (GVdr.Cells[0].Text == "罰款金額")
            {
                //TextBox txtFineMoney = (TextBox)GVdr.Cells[11].FindControl("txtFineMoney");
                TextBox txtFineMoney = (TextBox)GVdr.Cells[12].FindControl("txtFineMoney");
                drNew["罰款金額"] = txtFineMoney.Text.Replace(",", "");
            }
            //Label lblSumContanUnit_Price = (Label)GVdr.Cells[11].FindControl("lblSumContanUnit_Price");
            Label lblSumContanUnit_Price = (Label)GVdr.Cells[12].FindControl("lblSumContanUnit_Price");
            drNew["含稅總金額"] = lblSumContanUnit_Price.Text.Replace(",", "");
            //drNew["未稅總金額"] = GVdr.Cells[12].Text.Replace(",", "");
            drNew["未稅總金額"] = GVdr.Cells[13].Text.Replace(",", "");
            //TextBox txtRequisition_Count = (TextBox)GVdr.Cells[10].FindControl("txtRequisition_Count");
            TextBox txtRequisition_Count = (TextBox)GVdr.Cells[11].FindControl("txtRequisition_Count");
            drNew["實際請款數量"] = txtRequisition_Count.Text.Replace(",", "");

            //if (GVdr.Cells[13].Text == "")
            if (GVdr.Cells[14].Text == "")
            {
                drNew["遲交天數"] = "0";
            }
            else
            {
                //drNew["遲交天數"] = GVdr.Cells[13].Text.Replace(",", "");
                string strDelayDays = GVdr.Cells[14].Text.Replace(",", "").ToLower().Replace("</font>","").Replace("<font color='red'>","");
                if (strDelayDays.IndexOf("(") >= 0)
                {
                    strDelayDays = strDelayDays.Replace("(", "").Replace(")", "");
                    strDelayDays = "-" + strDelayDays;
                }
                drNew["遲交天數"] = strDelayDays;

            }
            //TextBox txtComment = (TextBox)GVdr.Cells[15].FindControl("txtComment");
            TextBox txtComment = (TextBox)GVdr.Cells[16].FindControl("txtComment");
            drNew["Comment"] = txtComment.Text;
            //drNew["Check_ID"] = GVdr.Cells[14].Text;
            Label lbInvoiceNumber = (Label)GVdr.Cells[15].FindControl("lbInvoiceNumber");
            drNew["Check_ID"] = lbInvoiceNumber.Text;
            if (drNew["NAME"].ToString() != "罰款金額" && drNew["NAME"].ToString() != "合計")
            {
                if (drNew[7].ToString() != "退貨")
                {
                    drNew["Change_UnitPrice"] = dtSplitWorkOld.Rows[i]["Change_UnitPrice"].ToString();
                }
                else
                {
                    drNew["Change_UnitPrice"] = drNew[10];
                }
            }
            dtSplitWorkNew.Rows.Add(drNew);

            i++;
        }
        // add chaoma end
        string strTime = DateTime.Now.ToString("yyyyMMddhhmmss");
        Finance0011BL.ADD_CARD_YEAR_FORCAST_PRINT(dtSplitWorkNew, strTime);

        string Comment = "";
        string Fine = "";
        DataRow[] drows;
        drows = dtSplitWorkNew.Select("罰款金額 is not null ");
        foreach (DataRow dr in drows)
        {
            if (dr["罰款金額"].ToString() != "")
            {
                Fine = dr["罰款金額"].ToString();
            }
            else
            {
                Fine = "0";
            }
        }

        drows = dtSplitWorkNew.Select("NAME = '罰款金額' ");
        if (drows.Length != 0)
        {
            foreach (DataRow dr in drows)
            {
                Comment = dr["Comment"].ToString();
            }
        }

        Response.Write("<script>window.open('Finance0011Add_2Print.aspx?Time=" + strTime + "&SAP_Serial_Number=" + txtSAP_Serial_Number.Text + "&Comment=" + Server.UrlEncode(Comment) + "&Fine=" + Fine + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");

    }

    /// <summary>
    /// 確定，暫存狀態
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        if (this.chkDel.Checked)
        {
            try
            {
                Finance0011BL.Delete(this.HiddenField_SAP_RID.Value.ToString());
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Finance0011.aspx?Con=1");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        else
        {
            string strType = "1";
            AskMoneySubmitOROK(strType);
        }
    }

    /// <summary>
    /// 提交，待放行狀態
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOK1_Click(object sender, EventArgs e)
    {
        string strType = "3";
        AskMoneySubmitOROK(strType);

        string[] arg = new string[1];
        Warning.SetWarning(GlobalString.WarningType.BlankCardMoney, arg);
    }

    /// <summary>
    /// 放行，已放行狀態
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPass2_Click(object sender, EventArgs e)
    {
        string strType = "4";
        AuditAskMoney(strType);
    }

    /// <summary>
    /// 退回，退回狀態
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUntread1_Click(object sender, EventArgs e)
    {
        string strType = "2";
        AuditAskMoney(strType);
    }

    /// <summary>
    /// 取消。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0011.aspx?Con=1");
    }


    /// <summary>
    /// 填寫發票號
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnInvoiceNumber_Click(object sender, EventArgs e)
    {
        CheckBox cbInvoiceNumber = new CheckBox();
        Label lbInvoiceNumber = new Label();
        bool blChecked = false;
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        {
            cbInvoiceNumber = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbInvoiceNumber");
            if (cbInvoiceNumber.Checked == true)
            {
                blChecked = true;
            }
        }

        if (!blChecked)
        {
            ShowMessage("沒有選擇要設置的發票編號，請先選擇。");
            return;
        }

        if (this.txtInvoiceNumber.Text.Trim() == "")
        {
            ShowMessage("發票編號不能為空，請先填寫發票編號。");
            return;
        }

        // 設置發票編號到對應的欄目
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        {
            cbInvoiceNumber = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbInvoiceNumber");
            lbInvoiceNumber = (Label)gvpbRequisitionWork.Rows[i].FindControl("lbInvoiceNumber");
            if (cbInvoiceNumber.Checked == true)
            {
                cbInvoiceNumber.Checked = false;
                lbInvoiceNumber.Text = txtInvoiceNumber.Text;
            }
        }

        txtInvoiceNumber.Text = "";
    }
}
