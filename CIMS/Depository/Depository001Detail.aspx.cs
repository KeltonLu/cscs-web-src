//******************************************************************
//*  作    者：FangBao
//*  功能說明：案件歷程明細
//*  創建日期：2008-11-24
//*  修改日期：2008-11-24 12:00
//*  修改記錄：
//*            □2008-11-24
//*              1.創建 鮑方
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
using System.Collections.Generic;

public partial class Depository_Depository001Detail : PageBase
{
    Depository001BL bl = new Depository001BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";
        gvpbBudget.NoneData = "";

        if (!IsPostBack)
        {
            string strType = Request.QueryString["Type"];
            string strID = Request.QueryString["ID"];
            DataSet dstDetail = null;

            if (StringUtil.IsEmpty(strType))
                MainTb.Visible = false;
            else
                MainTb.Visible = true;

            if (strType == "1")
            {
                trname.Visible = true;                                //版面簡稱
                trOrder_Date.Visible = true;                          //訂購日
                trNumber.Visible = true;                              //訂購數量
                trStock_Number.Visible = true;                        //進貨量
                trBlemish_Number.Visible = true;                      //瑕疵量
                trSample_Number.Visible = true;                       //抽樣卡數
                trIncome_Number.Visible = true;                       //入庫量
                trIncome_Date.Visible = true;                         //入庫日期
                trSerial_Number.Visible = true;                       //卡片批號
                trPerso_Factory_Name.Visible = true;                  //Perso廠
                trBlank_Factory_NAME.Visible = true;                  //空白卡廠
                trWafer_Name.Visible = true;                          //晶片名稱
                trCase_Status.Visible = true;                         //批號狀態
                trComment.Visible = true;                             //備註

                trRestock_Number.Visible = false;                     //再入庫日期
                trCancel_Number.Visible = false;                      //退貨量
                trReincome_Number.Visible = false;                    //再入庫量
                trCancel_Date.Visible = false;                        //退貨日期
                trReincome_Date.Visible = false;                      //再入庫日期

                
              
                dstDetail = bl.GetDSDetail(strID);
                if (dstDetail != null)
                {
                    if (dstDetail.Tables[0].Rows.Count > 0)
                    {
                        SetControlsForDataRow(dstDetail.Tables[0].Rows[0]);
                        lblTitle.Text = "入庫(" + strID + "," + lblIncome_Date.Text + "," + lblIncome_Number.Text + "," + dstDetail.Tables[0].Rows[0]["Is_AskFinance"].ToString().Replace("Y", "已請款").Replace("N", "未請款") + ")";
                        lblCase_Status.Text = lblCase_Status.Text.Replace("Y", "已結案").Replace("N", "未結案");

                        if (dstDetail.Tables[0].Rows[0]["Is_AskFinance"].ToString() == "N")
                            return;

                        DataSet dstFinanceDetail = bl.GetFinance(strType, dstDetail.Tables[0].Rows[0]["RID"].ToString());
                        gvpbBudget.DataSource = dstFinanceDetail.Tables[0];
                        gvpbBudget.DataBind();

                        if (dstFinanceDetail.Tables[0].Rows.Count > 0)
                        {
                            lblCheck_Serial_Number.Text = dstFinanceDetail.Tables[0].Rows[0]["Check_Serial_Number"].ToString();

                            DataSet dstSAP = bl.GetCARD_TYPE_SAP(dstFinanceDetail.Tables[0].Rows[0]["SAP_RID"].ToString());
                            if (dstSAP.Tables[0].Rows.Count > 0)
                            {
                                if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0]["Ask_Date"].ToString()))
                                    lblAsk_Date.Text = Convert.ToDateTime(dstSAP.Tables[0].Rows[0]["Ask_Date"].ToString()).ToString("yyyyMMdd");
                                if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0]["Pay_Date"].ToString()))
                                    lblPay_Date.Text = Convert.ToDateTime(dstSAP.Tables[0].Rows[0]["Pay_Date"].ToString()).ToString("yyyyMMdd").Replace("19000101", "");
                                lblSAP_Serial_Number.Text = dstSAP.Tables[0].Rows[0]["SAP_Serial_Number"].ToString();
                            }
                        }
                    }
                }
            }
            else if (strType == "3")
            {
                trname.Visible = true;                                //版面簡稱
                trCancel_Number.Visible = true;                      //退貨量
                trCancel_Date.Visible = true;                        //退貨日期
                trBlank_Factory_NAME.Visible = true;                  //空白卡廠
                trWafer_Name.Visible = true;                          //晶片名稱
                trComment.Visible = true;                             //備註

                trOrder_Date.Visible = false;                          //訂購日
                trNumber.Visible = false;                              //訂購數量
                trStock_Number.Visible = false;                        //進貨量
                trRestock_Number.Visible = false;                     //再入庫日期
                trBlemish_Number.Visible = false;                      //瑕疵量
                trSample_Number.Visible = false;                       //抽樣卡數
                trIncome_Number.Visible = false;                       //入庫量
                trReincome_Number.Visible = false;                    //再入庫量
                trIncome_Date.Visible = false;                         //入庫日期
                trReincome_Date.Visible = false;                      //再入庫日期
                trSerial_Number.Visible = false;                       //卡片批號
                trPerso_Factory_Name.Visible = false;                  //Perso廠
                trCase_Status.Visible = false;                         //批號狀態

                dstDetail = bl.GetDCDetail(strID);
                if (dstDetail != null)
                {
                    if (dstDetail.Tables[0].Rows.Count > 0)
                    {
                        SetControlsForDataRow(dstDetail.Tables[0].Rows[0]);
                        lblTitle.Text = "退貨(" + strID + "," + lblCancel_Date.Text + "," + lblCancel_Number.Text + "," + dstDetail.Tables[0].Rows[0]["Is_AskFinance"].ToString().Replace("Y", "已請款").Replace("N", "未請款") + ")";
                        lblCase_Status.Text = lblCase_Status.Text.Replace("Y", "已結案").Replace("N", "未結案");

                        if (dstDetail.Tables[0].Rows[0]["Is_AskFinance"].ToString() == "N")
                            return;

                        DataSet dstFinanceDetail = bl.GetFinance(strType, dstDetail.Tables[0].Rows[0]["RID"].ToString());
                        gvpbBudget.DataSource = dstFinanceDetail.Tables[0];
                        gvpbBudget.DataBind();



                        if (dstFinanceDetail.Tables[0].Rows.Count > 0)
                        {
                            lblCheck_Serial_Number.Text = dstFinanceDetail.Tables[0].Rows[0]["Check_Serial_Number"].ToString();

                            DataSet dstSAP = bl.GetCARD_TYPE_SAP(dstFinanceDetail.Tables[0].Rows[0]["SAP_RID"].ToString());
                            if (dstSAP.Tables[0].Rows.Count > 0)
                            {
                                if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0]["Ask_Date"].ToString()))
                                    lblAsk_Date.Text = Convert.ToDateTime(dstSAP.Tables[0].Rows[0]["Ask_Date"].ToString()).ToString("yyyyMMdd");
                                if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0]["Pay_Date"].ToString()))
                                    lblPay_Date.Text = Convert.ToDateTime(dstSAP.Tables[0].Rows[0]["Pay_Date"].ToString()).ToString("yyyyMMdd").Replace("19000101", "");
                                lblSAP_Serial_Number.Text = dstSAP.Tables[0].Rows[0]["SAP_Serial_Number"].ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                trname.Visible = true;                                //版面簡稱
                trRestock_Number.Visible = true;                     //再入庫日期
                trBlemish_Number.Visible = true;                      //瑕疵量
                trReincome_Number.Visible = true;                    //再入庫量
                trReincome_Date.Visible = true;                      //再入庫日期
                trSerial_Number.Visible = true;                       //卡片批號
                trPerso_Factory_Name.Visible = true;                  //Perso廠
                trBlank_Factory_NAME.Visible = true;                  //空白卡廠
                trWafer_Name.Visible = true;                          //晶片名稱
                trCase_Status.Visible = true;                         //批號狀態
                trComment.Visible = true;                             //備註

                trOrder_Date.Visible = false;                          //訂購日
                trNumber.Visible = false;                              //訂購數量
                trStock_Number.Visible = false;                        //進貨量
                trSample_Number.Visible = false;                       //抽樣卡數
                trIncome_Number.Visible = false;                       //入庫量
                trCancel_Number.Visible = false;                      //退貨量
                trIncome_Date.Visible = false;                         //入庫日期
                trCancel_Date.Visible = false;                        //退貨日期

                dstDetail = bl.GetDRDetail(strID);
                if (dstDetail != null)
                {
                    if (dstDetail.Tables[0].Rows.Count > 0)
                    {
                        SetControlsForDataRow(dstDetail.Tables[0].Rows[0]);
                        lblTitle.Text = "再入庫(" + strID + "," + lblReincome_Date.Text + "," + lblReincome_Number.Text + "," + dstDetail.Tables[0].Rows[0]["Is_AskFinance"].ToString().Replace("Y", "已請款").Replace("N", "未請款") + ")";
                        lblCase_Status.Text = lblCase_Status.Text.Replace("Y", "已結案").Replace("N", "未結案");

                        if (dstDetail.Tables[0].Rows[0]["Is_AskFinance"].ToString() == "N")
                            return;

                        DataSet dstFinanceDetail = bl.GetFinance(strType, dstDetail.Tables[0].Rows[0]["RID"].ToString());
                        gvpbBudget.DataSource = dstFinanceDetail.Tables[0];
                        gvpbBudget.DataBind();

                        if (dstFinanceDetail.Tables[0].Rows.Count > 0)
                        {
                            lblCheck_Serial_Number.Text = dstFinanceDetail.Tables[0].Rows[0]["Check_Serial_Number"].ToString();

                            DataSet dstSAP = bl.GetCARD_TYPE_SAP(dstFinanceDetail.Tables[0].Rows[0]["SAP_RID"].ToString());
                            if (dstSAP.Tables[0].Rows.Count > 0)
                            {
                                if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0]["Ask_Date"].ToString()))
                                    lblAsk_Date.Text = Convert.ToDateTime(dstSAP.Tables[0].Rows[0]["Ask_Date"].ToString()).ToString("yyyyMMdd");
                                if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0]["Pay_Date"].ToString()))
                                    lblPay_Date.Text = Convert.ToDateTime(dstSAP.Tables[0].Rows[0]["Pay_Date"].ToString()).ToString("yyyyMMdd").Replace("19000101","");
                                lblSAP_Serial_Number.Text = dstSAP.Tables[0].Rows[0]["SAP_Serial_Number"].ToString();
                            }
                        }
                    }
                }
            }
        }
    }

}
