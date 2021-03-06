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

public partial class Finance_Finance0012Edit : PageBase
{
    Finance0012BL Finance0012BL = new Finance0012BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            ViewState["Amt"] = "0"; 
            ViewState["Amt1"] = "0";
            //ViewState["AmtNum1"] = "0";
            ViewState["AmtNum2"] = "0";//實際請款數量
            string strSAP_Serial_Number = Request.QueryString["SAP_Serial_Number"];
            if (StringUtil.IsEmpty(strSAP_Serial_Number))
            {
                return;
            }
            this.lblSAP_Serial_Number.Text = strSAP_Serial_Number;
            Session["dtRequisitionWork_SAP_Edit"] = Finance0012BL.getSAP(strSAP_Serial_Number);
            //獲得sap單的rid
            DataTable dtRequisitionWork_SAP = (DataTable)Session["dtRequisitionWork_SAP_Edit"];
            if (dtRequisitionWork_SAP.Rows.Count != 0)
            {
                HiddenField_SAP_RID.Value = dtRequisitionWork_SAP.Rows[0]["RID"].ToString();
                HiddenField_Ask_Date.Value = Convert.ToDateTime(dtRequisitionWork_SAP.Rows[0]["Ask_Date"]).ToString("yyyy-MM-dd");
                HiddenField_Pay_Date.Value = Convert.ToDateTime(dtRequisitionWork_SAP.Rows[0]["Pay_Date"]).ToString("yyyy-MM-dd");
                HiddenField_Is_Finance.Value = dtRequisitionWork_SAP.Rows[0]["Is_Finance"].ToString();
                if (HiddenField_Pay_Date.Value == "1900-01-01")
                {
                    txtBegin_Date.Text = "";
                }
                else
                {
                    txtBegin_Date.Text = HiddenField_Pay_Date.Value;
                }
            }

            DataTable dtRequisitionWork_SAPDetail = Finance0012BL.getSAPDetail(strSAP_Serial_Number);
            //計算含稅總金額和未稅總金額
            Session["dtRequisitionWork_SAPDetail_Edit"] = Calculate_Price(dtRequisitionWork_SAPDetail);
            gvpbRequisitionWork.BindData();
            if (dtRequisitionWork_SAP.Rows.Count != 0)
            {
                if (HiddenField_Pay_Date.Value != "1900-01-01")
                {
                    if (lblSumUnit_Price.Text != "")
                    {
                        txtReal_Pay_Money.Text = Convert.ToDecimal(dtRequisitionWork_SAP.Rows[0]["Real_Pay_Money"].ToString()).ToString("N2");
                        decimal DiffUnit_Price = Convert.ToDecimal(lblSumUnit_Price.Text.Replace(",", "")) - Convert.ToDecimal(txtReal_Pay_Money.Text.Replace(",", ""));
                        lblDiffUnit_Price.Text = DiffUnit_Price.ToString("N2");
                    }
                    else
                    {
                        lblDiffUnit_Price.Text = "";
                    }
                }
                if (HiddenField_Pay_Date.Value != "1900-01-01")
                {
                    if (lblUnit_PriceNO.Text != "")
                    {
                        txtReal_Pay_Money_No.Text =Convert.ToDecimal(dtRequisitionWork_SAP.Rows[0]["Real_Pay_Money_No"].ToString()).ToString("N2");
                        decimal DiffUnit_PriceNO = Convert.ToDecimal(lblUnit_PriceNO.Text.Replace(",", "")) - Convert.ToDecimal(txtReal_Pay_Money_No.Text.Replace(",", ""));
                        lblDiffUnit_PriceNO.Text = DiffUnit_PriceNO.ToString("N2");
                    }
                    else
                    {
                        lblDiffUnit_PriceNO.Text = "";
                    }
                }
            }
        }
    }

    protected void gvpbRequisitionWork_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork_SAPDetail_Edit"];
        e.Table = dtRequisitionWork;//要綁定的資料表
        e.RowCount = dtRequisitionWork.Rows.Count;//查到的行數
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        try
        {
            string Ask_Date = "";
            TimeSpan ts = Convert.ToDateTime(HiddenField_Ask_Date.Value).Subtract(Convert.ToDateTime(txtBegin_Date.Text));
            if (ts.Days > 0)
            {
                ShowMessage("付款日期不能小於請款日期!");
                return;
            }
            if (txtBegin_Date.Text == "")
            {
                Ask_Date = "1900/01/01";
            }
            else
            {
                Ask_Date = txtBegin_Date.Text;
            }

            DataTable dtRequisitionWork_SAP_Edit = (DataTable)Session["dtRequisitionWork_SAP_Edit"];
            if (Convert.ToDateTime(dtRequisitionWork_SAP_Edit.Rows[0]["Pay_Date"]).ToString("yyyy-MM-dd") == "1900-01-01")
            {
                DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork_SAPDetail_Edit"];
                Finance0012BL.Save(txtReal_Pay_Money.Text.Replace(",", ""),
                            txtReal_Pay_Money_No.Text.Replace(",", ""),
                            Ask_Date,
                            lblSAP_Serial_Number.Text,
                            HiddenField_Is_Finance.Value,
                            dtRequisitionWork);
            }
            else
                Finance0012BL.Save(txtReal_Pay_Money.Text.Replace(",", ""),
                            txtReal_Pay_Money_No.Text.Replace(",", ""),
                            Ask_Date,
                            lblSAP_Serial_Number.Text,
                            HiddenField_Is_Finance.Value);
            
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Finance0012.aspx?Con=1");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0012.aspx?Con=1");
    }
    protected void gvpbRequisitionWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[16].Visible = false;
            e.Row.Cells[17].Visible = false;
            e.Row.Cells[18].Visible = false;
            e.Row.Cells[19].Visible = false;
            e.Row.Cells[20].Visible = false;
            e.Row.Cells[21].Visible = false;
            e.Row.Cells[22].Visible = false;
            ViewState["Amt"] = "0";
            ViewState["Amt1"] = "0";
            //ViewState["AmtNum1"] = "0";
            ViewState["AmtNum2"] = "0";//實際請款數量
        }
        DataTable dtRequisitionWork = (DataTable)gvpbRequisitionWork.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dtRequisitionWork_SAPDetail_Edit = (DataTable)Session["dtRequisitionWork_SAPDetail_Edit"];
            DataTable dtRequisitionWork_SAP = (DataTable)Session["dtRequisitionWork_SAP_Edit"];
            string Fine = "";
            string Comment = "";

            // 不是第一行
            if (e.Row.RowIndex != 0)
            {
                // 如果該行和上一行的RID相同，則不顯示數量。
                if (dtRequisitionWork_SAPDetail_Edit.Rows[e.Row.RowIndex - 1]["Operate_Type"].ToString() ==
                    dtRequisitionWork_SAPDetail_Edit.Rows[e.Row.RowIndex]["Operate_Type"].ToString() &&
                    dtRequisitionWork_SAPDetail_Edit.Rows[e.Row.RowIndex - 1]["Operate_RID"].ToString() ==
                    dtRequisitionWork_SAPDetail_Edit.Rows[e.Row.RowIndex]["Operate_RID"].ToString()
                    )
                {
                    e.Row.Cells[6].Text = "";
                }
            }

            if (dtRequisitionWork_SAP.Rows.Count != 0)
            {
                Fine = dtRequisitionWork_SAP.Rows[0]["Fine"].ToString();
                Comment = dtRequisitionWork_SAP.Rows[0]["Comment"].ToString();
            }
            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "入庫";

                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00")
                {
                    TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(e.Row.Cells[7].Text).Ticks);
                    TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(dtRequisitionWork.Rows[e.Row.RowIndex]["Fore_Delivery_Date"].ToString()).Ticks);
                    TimeSpan ts = ts1 - ts2;
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                    if (ts.Days < 0)
                    //    e.Row.Cells[13].Text = "0";
                        e.Row.Cells[13].Text = "<font color='red'>" + "(" + ts.Days.ToString("N0").Replace("-", "") + ")" + " </font>";
                    else
                        e.Row.Cells[13].Text = ts.Days.ToString("N0");

                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 End
                }
                else
                    e.Row.Cells[13].Text = "0";
            }

            Label lblSumRequisition_Count = (Label)e.Row.FindControl("lblSumRequisition_Count");
            if (e.Row.Cells[18].Text != "" && e.Row.Cells[18].Text != "&nbsp;")
                lblSumRequisition_Count.Text = Convert.ToInt32(e.Row.Cells[18].Text.Replace(",","")).ToString("N0");

            if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "再入庫";
                e.Row.Cells[13].Text = "0";
            }

            if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "<font color='red'>退貨 </font>";
                e.Row.Cells[0].Text = "<font color='red'>" + e.Row.Cells[0].Text + " </font>";
                e.Row.Cells[6].Text = "<font color='red'>" + e.Row.Cells[6].Text + " </font>";
                e.Row.Cells[1].Text = "<font color='red'>" + e.Row.Cells[1].Text + " </font>";
                e.Row.Cells[13].Text = "0";
            }
            if (e.Row.Cells[7].Text != "&nbsp;")
            {
                e.Row.Cells[7].Text = Convert.ToDateTime(e.Row.Cells[7].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }

            Label lblComment = (Label)e.Row.FindControl("lblComment");
            lblComment.Text = e.Row.Cells[17].Text;


            if (e.Row.Cells[0].Text == "罰款金額")
            {
                //總計不包含罰款金額
                //double Amt = Convert.ToDouble(ViewState["Amt"]);
                //double Amt1 = Convert.ToDouble(ViewState["Amt1"]);

                Label lblContanUnit_Price1 = (Label)e.Row.FindControl("lblContanUnit_Price");
                if (Fine != "")
                {
                    lblContanUnit_Price1.Text = Convert.ToDecimal(Fine).ToString("N2");
                    Label lblNOContanUnit_Price = (Label)e.Row.FindControl("lblNOContanUnit_Price");
                    double Fine1 = Convert.ToDouble(Fine)/1.05;
                    lblNOContanUnit_Price.Text = Fine1.ToString("N2");
                    //Amt += Convert.ToDouble(Fine);
                    //Amt1 += Convert.ToDouble(Fine1);
                    //ViewState["Amt"] = Amt;
                    //ViewState["Amt1"] = Amt1;
                }
                
                Label lblComment1 = (Label)e.Row.FindControl("lblComment");
                lblComment1.Text = Comment;
            }

            if (e.Row.Cells[0].Text == "合計")
            {
                double Amt = Convert.ToDouble(ViewState["Amt"]);
                double Amt1 = Convert.ToDouble(ViewState["Amt1"]);
                //int AmtNum1 = Convert.ToInt32(ViewState["AmtNum1"]);
                int AmtNum2 = Convert.ToInt32(ViewState["AmtNum2"]);
                if (e.Row.Cells[20].Text == "&nbsp;")
                    e.Row.Cells[20].Text = "0";
                Label lblContanUnit_Price2 = (Label)e.Row.FindControl("lblContanUnit_Price");
                lblContanUnit_Price2.Text = Amt.ToString("N0")+".00";
                lblSumUnit_Price.Text = Amt.ToString("N0") + ".00";


                Label lblComment2 = (Label)e.Row.FindControl("lblComment");
                lblComment2.Visible = false;
                
                //if(e.Row.Cells[21].Text!=""&&e.Row.Cells[21].Text!="&nbsp;")
                
                Label lblNOContanUnit_Price = (Label)e.Row.FindControl("lblNOContanUnit_Price");
                //if (e.Row.Cells[21].Text != "" && e.Row.Cells[21].Text != "&nbsp;")
                lblNOContanUnit_Price.Text = Amt1.ToString("N0") + ".00";
                lblUnit_PriceNO.Text = Amt1.ToString("N0") + ".00";
                //e.Row.Cells[6].Text = AmtNum1.ToString("N0");
                lblSumRequisition_Count.Text = AmtNum2.ToString("N0");//實際請款數量
            }
            //含稅總金額 未稅總金額
            if (e.Row.Cells[0].Text != "罰款金額" && e.Row.Cells[0].Text != "合計")
            {
                double Amt = Convert.ToDouble(ViewState["Amt"]);
                double Amt1 = Convert.ToDouble(ViewState["Amt1"]);
                //int AmtNum1 = Convert.ToInt32(ViewState["AmtNum1"]);
                int AmtNum2 = Convert.ToInt32(ViewState["AmtNum2"]);
                Label lblContanUnit_Price = (Label)e.Row.FindControl("lblContanUnit_Price");
                if (e.Row.Cells[20].Text != "" && e.Row.Cells[20].Text != "&nbsp;")
                    lblContanUnit_Price.Text = Convert.ToDecimal(e.Row.Cells[20].Text.Replace(",", "")).ToString("N0") + ".00";//金額取整后保留小數點兩位
                Label lblNOContanUnit_Price = (Label)e.Row.FindControl("lblNOContanUnit_Price");
                if (e.Row.Cells[21].Text != "" && e.Row.Cells[21].Text != "&nbsp;")
                    lblNOContanUnit_Price.Text = Convert.ToDecimal(e.Row.Cells[21].Text.Replace(",", "")).ToString("N0") + ".00";//金額取整后保留小數點兩位

                //
                //string Income_Number="0";
                //if (e.Row.Cells[6].Text != "" && e.Row.Cells[6].Text != "&nbsp;")
                //    Income_Number = Convert.ToInt32(e.Row.Cells[6].Text.Replace(",", "")).ToString("N0");
                if (e.Row.Cells[5].Text.Contains("退貨"))
                {
                    lblContanUnit_Price.Text = "-" + lblContanUnit_Price.Text;
                    lblNOContanUnit_Price.Text = "-" + lblNOContanUnit_Price.Text;
                    lblSumRequisition_Count.Text = "-" + lblSumRequisition_Count.Text;
                    //if (Income_Number != "0") Income_Number = "-" + Income_Number;
                    //e.Row.Cells[6].Text = "<font color='red'>" + e.Row.Cells[6].Text + " </font>";
                }
               
                
                Amt += Convert.ToDouble(lblContanUnit_Price.Text.Replace(",",""));
                Amt1 += Convert.ToDouble(lblNOContanUnit_Price.Text.Replace(",", ""));
                //AmtNum1 += Convert.ToInt32(Income_Number.Replace(",", ""));
                AmtNum2 += Convert.ToInt32(lblSumRequisition_Count.Text.Replace(",", ""));//實際請款數量

                ViewState["Amt"] = Amt.ToString();
                ViewState["Amt1"] = Amt1.ToString();
                //ViewState["AmtNum1"] = AmtNum1.ToString();
                ViewState["AmtNum2"] = AmtNum2.ToString();
            }
            e.Row.Cells[16].Visible = false;
            e.Row.Cells[17].Visible = false;
            e.Row.Cells[18].Visible = false;
            e.Row.Cells[19].Visible = false;
            e.Row.Cells[20].Visible = false;
            e.Row.Cells[21].Visible = false;
            e.Row.Cells[22].Visible = false;
        }
    }
    //計算含稅總金額和未稅總金額
    public DataTable Calculate_Price(DataTable dtRequisitionWork_SAPDetail)
    {
        int Income_Number = 0;//際請款數量
        int SumRequisition_Count = 0;//際請款數量
        Decimal SumUnit_Price = 0;//含稅總金額 
        Decimal SumONUnit_Price = 0;//未稅總金額
        for (int i = 0; i < dtRequisitionWork_SAPDetail.Rows.Count - 2; i++)
        {
            dtRequisitionWork_SAPDetail.Rows[i]["含稅總金額"] = Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["Real_Ask_Number"]) * Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["Unit_Price"]);
            dtRequisitionWork_SAPDetail.Rows[i]["未稅總金額"] = Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["Real_Ask_Number"]) * Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["Unit_Price_No"]);
        }

        for (int i = 0; i < dtRequisitionWork_SAPDetail.Rows.Count; i++)
        {
            if (dtRequisitionWork_SAPDetail.Rows[i][3].ToString() == "罰款金額")
            {
                continue;
            }
            if (dtRequisitionWork_SAPDetail.Rows[i][3].ToString() == "合計")
            {
                dtRequisitionWork_SAPDetail.Rows[i]["Income_Number"] = Income_Number;
                dtRequisitionWork_SAPDetail.Rows[i]["Real_Ask_Number"] = SumRequisition_Count;
                dtRequisitionWork_SAPDetail.Rows[i]["含稅總金額"] = SumUnit_Price;
                dtRequisitionWork_SAPDetail.Rows[i]["未稅總金額"] = SumONUnit_Price;
            }
            else
            {
                if (dtRequisitionWork_SAPDetail.Rows[i]["Income_Number"].ToString() != "")
                {
                    if (dtRequisitionWork_SAPDetail.Rows[i]["Operate_Type"].ToString() == "3")
                    {
                        Income_Number -= Convert.ToInt32(dtRequisitionWork_SAPDetail.Rows[i]["Income_Number"]);
                    }
                    else
                    {
                        Income_Number += Convert.ToInt32(dtRequisitionWork_SAPDetail.Rows[i]["Income_Number"]);
                    }
                }
                if (dtRequisitionWork_SAPDetail.Rows[i]["Real_Ask_Number"].ToString() != "")
                {
                    if (dtRequisitionWork_SAPDetail.Rows[i]["Operate_Type"].ToString() == "3")
                    {
                        SumRequisition_Count -= Convert.ToInt32(dtRequisitionWork_SAPDetail.Rows[i]["Real_Ask_Number"]);
                    }
                    else
                    {
                        SumRequisition_Count += Convert.ToInt32(dtRequisitionWork_SAPDetail.Rows[i]["Real_Ask_Number"]);
                    }
                }
                if (dtRequisitionWork_SAPDetail.Rows[i]["含稅總金額"].ToString() != "")
                {
                    if (dtRequisitionWork_SAPDetail.Rows[i]["Operate_Type"].ToString() == "3")
                    {
                        SumUnit_Price -= Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["含稅總金額"]);
                    }
                    else
                    {
                        SumUnit_Price += Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["含稅總金額"]);
                    }
                }
                if (dtRequisitionWork_SAPDetail.Rows[i]["未稅總金額"].ToString() != "")
                {
                    if (dtRequisitionWork_SAPDetail.Rows[i]["Operate_Type"].ToString() == "3")
                    {
                        SumONUnit_Price -= Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["未稅總金額"]);
                    }
                    else
                    {
                        SumONUnit_Price += Convert.ToDecimal(dtRequisitionWork_SAPDetail.Rows[i]["未稅總金額"]);
                    }
                }
            }
        }
        return dtRequisitionWork_SAPDetail;
    }
    protected void txtReal_Pay_Money_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (lblSumUnit_Price.Text != "" && txtReal_Pay_Money.Text != "")
            {
                decimal DiffUnit_Price = Convert.ToDecimal(lblSumUnit_Price.Text.Replace(",", "")) - Convert.ToDecimal(txtReal_Pay_Money.Text.Replace(",", ""));
                lblDiffUnit_Price.Text = DiffUnit_Price.ToString("N2");
            }
            else
            {
                lblDiffUnit_Price.Text = "";
            }

            txtReal_Pay_Money.Text = Convert.ToDecimal(txtReal_Pay_Money.Text.Replace(",", "")).ToString("N2");
        }
        catch
        {
            txtReal_Pay_Money.Text = "0";
        }
    }
    protected void txtReal_Pay_Money_No_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (lblUnit_PriceNO.Text != "" && txtReal_Pay_Money_No.Text != "")
            {
                decimal DiffUnit_PriceNO = Convert.ToDecimal(lblUnit_PriceNO.Text.Replace(",", "")) - Convert.ToDecimal(txtReal_Pay_Money_No.Text.Replace(",", ""));
                lblDiffUnit_PriceNO.Text = DiffUnit_PriceNO.ToString("N2");
            }
            else
            {
                lblDiffUnit_PriceNO.Text = "";
            }
            txtReal_Pay_Money_No.Text = Convert.ToDecimal(txtReal_Pay_Money_No.Text.Replace(",", "")).ToString("N2");
        }
        catch
        {
            txtReal_Pay_Money_No.Text = "0";
        }
    }
}
