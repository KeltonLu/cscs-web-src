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

public partial class Finance_Finance0031Mod : PageBase
{
    Finance0031BL Finance0031BL = new Finance0031BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbMATERIEL_PURCHASE_FORM.NoneData = "";
        if (!IsPostBack)
        {
            gvpbMATERIEL_PURCHASE_FORM.BindData();
        }
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        try
        {
            string strAlert;
            string strRID = Request.QueryString["RID"].ToString();
            if (chkDel.Checked == true)//刪除
            {
                if (Finance0031BL.Con_Delete(strRID))
                {
                    ShowMessage("已出帳,不能刪除");
                    return;
                }
                if (ViewState["Materiel_Type"].ToString().Trim() == "7" || ViewState["Materiel_Type"].ToString().Trim() == "8")
                {
                    if (Finance0031BL.CheckPostCostUpdate(strRID, Convert.ToDecimal(txtPay_Money.Text), ViewState["Materiel_Type"].ToString(), out strAlert, txtPay_Money.Visible))
                    {
                        ShowMessage(strAlert);
                        return;
                    }
                    if (strAlert != "")
                    {
                        ShowMessage(strAlert);
                    }
                }
                Finance0031BL.Delete(strRID);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Finance0031.aspx?Con=1");
            }
            else
            {
                if (txtAsk_Date.Text == "")
                {
                    ShowMessage("請款日期不能為空");
                    return;
                }
                if (txtPay_Date.Text != "")
                {
                    TimeSpan ts = Convert.ToDateTime(txtAsk_Date.Text).Subtract(Convert.ToDateTime(txtPay_Date.Text));
                    if (ts.Days > 0)
                    {
                        ShowMessage("出帳日期不能小於請款日期!");
                        return;
                    }
                }

                if (txtPay_Money.Visible == true)
                {
                    if (txtPay_Money.Text == "")
                    {
                        ShowMessage("出帳金額不能為空");
                        return;
                    }
                }
                if (txtPay_Money.Visible == false)
                {
                    if (txtSAP_Serial_Number.Text.Trim() == "")
                    {
                        ShowMessage("SAP單號不能為空");
                        return;
                    }
                    //if (this.txtSAP_Serial_Number.Text.Trim().Length != 15)
                    //{
                    //    ShowMessage("SAP單號必須是15位字符。");
                    //    return;
                    //}
                }
                if (this.txtSAP_Serial_Number.Text.Trim() != "")
                {
                    if (txtSAP_Serial_Number.Text.Trim() == "")
                    {
                        ShowMessage("SAP單號不能為空");
                        return;
                    }
                }
                if (strRID != "")
                {
                    if (Finance0031BL.GetCon_SAP(strRID, txtSAP_Serial_Number.Text.Trim()))
                    {
                        ShowMessage("SAP單重復");
                        return;
                    }
                }

                if (strRID != "")
                {
                    string strDateTime = "1900-01-01";
                    if (txtPay_Date.Text != "")
                        strDateTime = txtPay_Date.Text;
                    if (ViewState["Materiel_Type"].ToString().Trim() == "7" || ViewState["Materiel_Type"].ToString().Trim() == "8")
                    {
                        if (Finance0031BL.CheckPostCostUpdate(strRID, Convert.ToDecimal(txtPay_Money.Text), ViewState["Materiel_Type"].ToString(), out strAlert, txtPay_Money.Visible))
                        {
                            ShowMessage(strAlert);
                            return;
                        }
                        if (strAlert != "")
                        {
                            ShowMessage(strAlert);
                        }
                    }
                    Finance0031BL.Update(strRID, Convert.ToDecimal(txtPay_Money.Text), Convert.ToDateTime(txtAsk_Date.Text), Convert.ToDateTime(strDateTime), txtSAP_Serial_Number.Text.Trim());
                    ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "Finance0031.aspx?Con=1");
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0031.aspx?con=1");
    }
    protected void gvpbMATERIEL_PURCHASE_FORM_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        string strRID = Request.QueryString["RID"].ToString();
        DataSet dsMATERIEL_SAP = null;//SAP單訊息
        DataSet dsMATERIEL_PURCHASE_FORM_EDIT = null;//SAP單關聯的物料採購
        try
        {
            if (strRID != "")
            {
                dsMATERIEL_SAP = Finance0031BL.getMaterial_SAP(strRID, out dsMATERIEL_PURCHASE_FORM_EDIT);
            }
            if (dsMATERIEL_SAP != null)
            {
                lblAsk_Project.Text = dsMATERIEL_SAP.Tables[0].Rows[0]["Material_Type_Name"].ToString().Trim();
                lblPay_Money.Text = Convert.ToDecimal(dsMATERIEL_SAP.Tables[0].Rows[0]["Sum"].ToString().Trim()).ToString("N2");
                txtPay_Money.Text = Convert.ToDecimal(dsMATERIEL_SAP.Tables[0].Rows[0]["Sum"].ToString().Trim()).ToString("N2");
                txtAsk_Date.Text = Convert.ToDateTime(dsMATERIEL_SAP.Tables[0].Rows[0]["Ask_Date"]).ToString("yyyy-MM-dd").Replace("1900-01-01", "");
                txtPay_Date.Text = Convert.ToDateTime(dsMATERIEL_SAP.Tables[0].Rows[0]["Pay_Date"]).ToString("yyyy-MM-dd").Replace("1900-01-01", "");
                txtSAP_Serial_Number.Text = dsMATERIEL_SAP.Tables[0].Rows[0]["SAP_ID"].ToString().Trim();
                ViewState["Materiel_Type"] = dsMATERIEL_SAP.Tables[0].Rows[0]["Materiel_Type"].ToString().Trim();
            }
            txtPay_Money.Visible = true;
            lblPay_Money.Visible = false;

            if (dsMATERIEL_PURCHASE_FORM_EDIT != null)//如果查到了資料
            {
                txtPay_Money.Visible = false;
                lblPay_Money.Visible = true;
                e.Table = dsMATERIEL_PURCHASE_FORM_EDIT.Tables[0];//要綁定的資料表
                e.RowCount = dsMATERIEL_PURCHASE_FORM_EDIT.Tables[0].Rows.Count;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
}
