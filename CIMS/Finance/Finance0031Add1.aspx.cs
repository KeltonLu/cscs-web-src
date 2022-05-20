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
public partial class Finance_Finance0031Add1 : PageBase
{
    Finance0031BL Finance0031BL = new Finance0031BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable dtRequisition_Money = new DataTable();
            dtRequisition_Money.Columns.Add(new DataColumn("Materiel_Type", Type.GetType("System.String")));
            dtRequisition_Money.Columns.Add(new DataColumn("Materiel_Type_Name", Type.GetType("System.String")));
            dtRequisition_Money.Columns.Add(new DataColumn("Sum_Money", Type.GetType("System.Decimal")));
            dtRequisition_Money.Columns.Add(new DataColumn("SAP_ID", Type.GetType("System.String")));
            dtRequisition_Money.Columns.Add(new DataColumn("Ask_Date", Type.GetType("System.DateTime")));
            dtRequisition_Money.Columns.Add(new DataColumn("Pay_Date", Type.GetType("System.DateTime")));
            dtRequisition_Money.Columns.Add(new DataColumn("Billing_Type", Type.GetType("System.String")));
            DataTable dtSearchMaterialPurchase = (DataTable)Session["dtSearchMaterialPurchase"];
            Session["dtRequisition_Money"] = Finance0031BL.getAskMoneyInfo(dtRequisition_Money, dtSearchMaterialPurchase);
            gvpbRequisition_Money.BindData();
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        DataTable dtRequisition_Money = (DataTable)Session["dtRequisition_Money"];
        string strMsgAll = "";
        // 取所有SAP單號
        //DataTable dtSAP = Finance0031BL.getAllSAP();

        for (int i = 0; i < gvpbRequisition_Money.Rows.Count; i++)
        {
            TextBox txtPay_Money = (TextBox)gvpbRequisition_Money.Rows[i].FindControl("txtPay_Money");
            TextBox txtSAP_ID = (TextBox)gvpbRequisition_Money.Rows[i].FindControl("txtSAP_ID");
            TextBox txtAsk_Date = (TextBox)gvpbRequisition_Money.Rows[i].FindControl("txtAsk_Date");
            TextBox txtPay_Date = (TextBox)gvpbRequisition_Money.Rows[i].FindControl("txtPay_Date");

            // 郵資費時，必須輸入郵資費金額
            if (dtRequisition_Money.Rows[i]["Materiel_Type"].ToString().Trim() == "7" ||
                dtRequisition_Money.Rows[i]["Materiel_Type"].ToString().Trim() == "8")
            {
                if (txtPay_Money.Text.Trim().Length == 0)
                {
                    ShowMessage("郵資費金額沒有輸入。");
                    return;
                }

                if (txtSAP_ID.Text.Trim()=="")
                {
                    ShowMessage("郵資費SAP單號必須輸入");
                    return;
                }
                
                // 檢查郵資費年度預算剩余金額是否足夠
                string strOutMsg = "";
                if (!Finance0031BL.CheckPostCostNew(Decimal.Parse(txtPay_Money.Text.Trim().Replace(",","")),
                        dtRequisition_Money.Rows[i]["Materiel_Type"].ToString(),
                        out strOutMsg))
                {
                    ShowMessage(strOutMsg);
                    return;
                }

                dtRequisition_Money.Rows[i]["Sum_Money"] = Decimal.Parse(txtPay_Money.Text.Trim().Replace(",",""));
            }
            else
            {
                if (txtSAP_ID.Text.Trim() == "")
                {
                    ShowMessage("" + gvpbRequisition_Money.Rows[i].Cells[0].Text + "SAP單號不能為空");
                    return;
                }
                //if (txtSAP_ID.Text.Trim().Length != 15)
                //{
                //    ShowMessage("" + gvpbRequisition_Money.Rows[i].Cells[0].Text + "SAP單號必須輸入，且是15位字符。");
                //    return;
                //}
            }

            // SAP單號
            if (txtSAP_ID.Text.Trim() != "")
            {
                if (Finance0031BL.GetCon_SAP("0", txtSAP_ID.Text.Trim()))
                {
                    ShowMessage("SAP單重復");
                    return;
                }
                dtRequisition_Money.Rows[i]["SAP_ID"] = txtSAP_ID.Text.Trim();
                if (IsExist(txtSAP_ID.Text.Trim(), i))
                {
                    ShowMessage("請款SAP單號不能重復。");
                    return;
                }
            }

            // 請款日期
            if (txtAsk_Date.Text.Trim() == "")
            {
                ShowMessage("請款日期不能為空，請先輸入。");
                return;
            }
            else
            {
                try
                {
                    DateTime.Parse(txtAsk_Date.Text.Trim());
                }
                catch
                {
                    ShowMessage("請款日期格式不正確。");
                    return;
                }
                dtRequisition_Money.Rows[i]["Ask_Date"] = Convert.ToDateTime(txtAsk_Date.Text.Trim());
            }

            // 出帳日期
            if (txtPay_Date.Text.Trim() != "")
            {
                try
                {
                    TimeSpan ts = Convert.ToDateTime(txtAsk_Date.Text).Subtract(Convert.ToDateTime(txtPay_Date.Text));
                    if (ts.Days > 0)
                    {
                        ShowMessage("出帳日期不能小於請款日期!");
                        return;
                    }
                    DateTime.Parse(txtPay_Date.Text.Trim());
                }
                catch
                {
                    ShowMessage("出帳日期格式不正確。");
                    return;
                }
                dtRequisition_Money.Rows[i]["Pay_Date"] = Convert.ToDateTime(txtPay_Date.Text.Trim());
            }

            // 檢查當天是否有同類別的請款
            string strMsg = Finance0031BL.CheckAskMoney(dtRequisition_Money.Rows[i]["Materiel_Type"].ToString(),
                Convert.ToDateTime(txtAsk_Date.Text.Trim()));
            if (strMsg != "")
            {
                strMsgAll = strMsgAll + "\\n";
            }
        }

        if (strMsgAll.Length > 0)
        {
            // 先提示，再保存
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('" + strMsgAll + "是否確定新增');if(aa==true){ __doPostBack('btnIsSave','');}", true);
        }
        else
        {
            // 保存
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "__doPostBack('btnIsSave','');", true);
        }
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0031Add.aspx?Con=1");
    }
    protected void gvpbRequisition_Money_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtRequisition_Money = (DataTable)Session["dtRequisition_Money"];
        e.Table = dtRequisition_Money;//要綁定的資料表
        e.RowCount = dtRequisition_Money.Rows.Count;//查到的行數
    }
    protected void gvpbRequisition_Money_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtRequisition_Money = (DataTable)Session["dtRequisition_Money"];
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtPay_Money = (TextBox)e.Row.FindControl("txtPay_Money");
            Label lblPay_Money = (Label)e.Row.FindControl("lblPay_Money");
            if (dtRequisition_Money.Rows[e.Row.RowIndex]["Materiel_Type"].ToString() == "7" ||
                  dtRequisition_Money.Rows[e.Row.RowIndex]["Materiel_Type"].ToString() == "8")
            {
                // 郵資費
                txtPay_Money.Visible = true;
                txtPay_Money.Text = "";
                lblPay_Money.Visible = false;
            }
            else
            {
                // 非郵資費
                txtPay_Money.Visible = false;
                txtPay_Money.Text = "";
                lblPay_Money.Visible = true;
                lblPay_Money.Text = Convert.ToDecimal(dtRequisition_Money.Rows[e.Row.RowIndex]["Sum_Money"].ToString()).ToString("N2");
            }

            // 請款日期默認為當天
            TextBox txtAsk_Date = (TextBox)e.Row.FindControl("txtAsk_Date");
            txtAsk_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    /// <summary>
    /// 檢查頁面SAP單是否重復
    /// </summary>
    /// <param name="SAP_ID"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public bool IsExist(string SAP_ID, int j)
    {
        for (int i = 0; i < gvpbRequisition_Money.Rows.Count; i++)
        {
            TextBox txtSAP_ID = (TextBox)gvpbRequisition_Money.Rows[i].FindControl("txtSAP_ID");
            if (i == j)
                continue;
            if (txtSAP_ID.Text.Trim() == SAP_ID)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 保存請款SAP單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnIsSave_Click(object sender, EventArgs e)
    {
        DataTable dtRequisition_Money = (DataTable)Session["dtRequisition_Money"];
        for (int intRowIndex = 0; intRowIndex < this.gvpbRequisition_Money.Rows.Count;
                intRowIndex++)
        {
            Label lblPay_Money = (Label)gvpbRequisition_Money.Rows[intRowIndex].FindControl("lblPay_Money");
            TextBox txtPay_Money = (TextBox)gvpbRequisition_Money.Rows[intRowIndex].FindControl("txtPay_Money");
            TextBox txtSAP_ID = (TextBox)gvpbRequisition_Money.Rows[intRowIndex].FindControl("txtSAP_ID");
            TextBox txtAsk_Date = (TextBox)gvpbRequisition_Money.Rows[intRowIndex].FindControl("txtAsk_Date");
            TextBox txtPay_Date = (TextBox)gvpbRequisition_Money.Rows[intRowIndex].FindControl("txtPay_Date");

            if (dtRequisition_Money.Rows[intRowIndex]["Materiel_Type"].ToString() == "7" ||
                dtRequisition_Money.Rows[intRowIndex]["Materiel_Type"].ToString() == "8")
            {
                dtRequisition_Money.Rows[intRowIndex]["Sum_Money"] = Decimal.Parse(txtPay_Money.Text.Trim().Replace(",",""));
            }
            else
            {
                dtRequisition_Money.Rows[intRowIndex]["Sum_Money"] = Decimal.Parse(lblPay_Money.Text.Trim().Replace(",",""));
            }

            if (txtSAP_ID.Text.Trim() != "")
            {
                dtRequisition_Money.Rows[intRowIndex]["SAP_ID"] = txtSAP_ID.Text.Trim();
            }

            if (txtAsk_Date.Text.Trim() != "")
            {
                dtRequisition_Money.Rows[intRowIndex]["Ask_Date"] = txtAsk_Date.Text.Trim();
            }

            if (txtPay_Date.Text.Trim() != "")
            {
                dtRequisition_Money.Rows[intRowIndex]["Pay_Date"] = txtPay_Date.Text.Trim();
            }
            else
            {
                dtRequisition_Money.Rows[intRowIndex]["Pay_Date"] = Convert.ToDateTime("1900-01-01");
            }
        }

        DataTable dtSearchMaterialPurchase = (DataTable)Session["dtSearchMaterialPurchase"];
        Finance0031BL.Save(dtRequisition_Money, dtSearchMaterialPurchase);
        ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "Finance0031.aspx?Con=1");
    }
}
