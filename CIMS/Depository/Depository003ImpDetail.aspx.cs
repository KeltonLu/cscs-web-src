//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片入庫自動匯入明細
//*  創建日期：2008-09-04
//*  修改日期：2008-09-04 12:00
//*  修改記錄：
//*            □2008-09-02
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

public partial class Depository_Depository003ImpDetail : PageBase
{
    Depository003BL BL = new Depository003BL();

    #region 事件處理

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];

            DataTable dtblImp = (DataTable)Session["dtblImp"];

            DataRow drowImp = dtblImp.Rows[int.Parse(strRID)];

            DataTable dtbl = dtblImp.Clone();
            dtbl.ImportRow(drowImp);

            ViewState["dtbl"] = dtbl;

            dropOrderForm_Detail_RID.DataSource = BL.GetOrderFormDetailNo(drowImp["Blank_Factory_RID"].ToString(), drowImp["Space_Short_RID"].ToString());
            dropOrderForm_Detail_RID.DataBind();
            SetControlsForDataRow(drowImp);
            lblTotalNum.Text = int.Parse(lblTotalNum.Text).ToString("N0");
            lblRemainNum.Text = int.Parse(lblRemainNum.Text).ToString("N0");
            txtStock_Number.Text = int.Parse(txtStock_Number.Text).ToString("N0");
            txtBlemish_Number.Text = int.Parse(txtBlemish_Number.Text).ToString("N0");
            lblIncome_Date.Text = Convert.ToDateTime(lblIncome_Date.Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
            ViewState["CheckStatus"] = txtSendCheck_Status.Text;
            rblCheck_Type.SelectedValue = drowImp["Check_Type"].ToString();
            txtSendCheck_Status.Text = txtSendCheck_Status.Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");

        }

        CalNum();
    }

    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        if (!CheckSerialNum())
            return;

        if (StringUtil.GetByteLength(txtComment.Text) > 200)
        {
            ShowMessage("備註不能超過200個字符");
            return;
        }


        string strRID = Request.QueryString["RID"];

        if (int.Parse(txtIncome_Number.Text.Replace(",", "")) < 0)
        {
            ShowMessage("入庫量不能為負數");
            return;
        }

        DataTable dtblImp = (DataTable)Session["dtblImp"];
        if (dtblImp.Select("OrderForm_Detail_RID='" + dropOrderForm_Detail_RID.SelectedValue + "'").Length > 2)
        {
            ShowMessage("重複的訂單流水編號");
            return;
        }

        DataRow drowImp = dtblImp.Rows[int.Parse(strRID)];

        DataTable dtbl = (DataTable)ViewState["dtbl"];
        DataRow drow = dtbl.Rows[0];

        drowImp["OrderForm_Detail_RID"] = dropOrderForm_Detail_RID.SelectedValue;
        drowImp["TotalNum"] = drow["TotalNum"];
        drowImp["RemainNum"] = drow["RemainNum"];
        drowImp["Perso_Factory_RID"] = drow["Perso_Factory_RID"];
        drowImp["Perso_Factory_Name"] = drow["Perso_Factory_Name"];
        drowImp["Blank_Factory_RID"] = drow["Blank_Factory_RID"];
        drowImp["Blank_Factory_Name"] = drow["Blank_Factory_Name"];
        drowImp["Wafer_RID"] = drow["Wafer_RID"];
        drowImp["Wafer_Name"] = drow["Wafer_Name"];
        drowImp["SendCheck_Status"] = ViewState["CheckStatus"].ToString();
        drowImp["Serial_Number"] = txtSerial_Number.Text.Trim();
        drowImp["Comment"] = txtComment.Text.Trim();
        drowImp["Stock_Number"] = txtStock_Number.Text.Trim().Replace(",","");
        drowImp["Blemish_Number"] = txtBlemish_Number.Text.Trim().Replace(",","");
        drowImp["Sample_Number"] = txtSample_Number.Text.Trim().Replace(",","");
        drowImp["Income_Number"] = txtIncome_Number.Text.Trim().Replace(",","");
        drowImp["Check_Type"] = rblCheck_Type.SelectedValue;


        ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "returnValue='1';window.close();", true);

    }

    protected void dropOrderForm_Detail_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataSet dst = BL.GetDetailByOrderFormDetailNo(dropOrderForm_Detail_RID.SelectedValue);
        if (dst != null)
        {
            int remainnum = int.Parse(dst.Tables[0].Rows[0]["number"].ToString()) - int.Parse(dst.Tables[1].Rows[0][0].ToString()) + int.Parse(dst.Tables[2].Rows[0][0].ToString()) - int.Parse(dst.Tables[3].Rows[0][0].ToString());
            lblTotalNum.Text = Convert.ToInt32(dst.Tables[0].Rows[0]["Number"].ToString()).ToString("N0");
            lblRemainNum.Text = remainnum.ToString("N0");
            lblPerso_Factory_Name.Text = dst.Tables[0].Rows[0]["Perso_Factory_Name"].ToString();
            lblBlank_Factory_Name.Text = dst.Tables[0].Rows[0]["Blank_Factory_Name"].ToString();
            lblWafer_Name.Text = dst.Tables[0].Rows[0]["Wafer_Name"].ToString();

            DataTable dtbl = (DataTable)ViewState["dtbl"];
            DataRow drow = dtbl.Rows[0];
            drow["TotalNum"] = dst.Tables[0].Rows[0]["Number"].ToString();
            drow["RemainNum"] = remainnum.ToString();
            drow["Perso_Factory_RID"] = dst.Tables[0].Rows[0]["Delivery_Address_RID"].ToString();
            drow["Perso_Factory_Name"] = dst.Tables[0].Rows[0]["Perso_Factory_Name"].ToString();
            drow["Blank_Factory_RID"] = dst.Tables[0].Rows[0]["Factory_RID"].ToString();
            drow["Blank_Factory_Name"] = dst.Tables[0].Rows[0]["Blank_Factory_Name"].ToString();
            drow["Wafer_RID"] = dst.Tables[0].Rows[0]["Wafer_RID"].ToString();
            drow["Wafer_Name"] = dst.Tables[0].Rows[0]["Wafer_Name"].ToString();

            ViewState["drowImp"] = dtbl;
        }
    }


    protected void btnCalNum_Click(object sender, EventArgs e)
    {
        CheckSerialNum();
    }
    #endregion

    #region 欄位/資料補充說明

    /// <summary>
    /// 計算入庫量
    /// </summary>
    private void CalNum()
    {
        int income = 0;
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        if (!StringUtil.IsEmpty(txtStock_Number.Text))
            num1 = int.Parse(txtStock_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtBlemish_Number.Text))
            num2 = int.Parse(txtBlemish_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtSample_Number.Text))
            num3 = int.Parse(txtSample_Number.Text.Replace(",", ""));

        income = num1 - num2 - num3;
        txtIncome_Number.Text = income.ToString("N0");

    }

    private bool CheckSerialNum()
    {
        if (StringUtil.IsEmpty(txtSerial_Number.Text))
        {
            txtSendCheck_Status.Text = "";
            return true;
        }
        else
        {
            string strRID = Request.QueryString["RID"];

            DataTable dtblImp = (DataTable)Session["dtblImp"];

            string strCheckStatus = BL.GetCheckStatus(txtSerial_Number.Text, dtblImp.Rows[int.Parse(strRID)]["Space_Short_RID"].ToString());

            txtSendCheck_Status.Text = strCheckStatus.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");

            ViewState["CheckStatus"] = strCheckStatus;

            if (StringUtil.IsEmpty(strCheckStatus))
            {
                ShowMessage("輸入了錯誤的卡片批號");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    #endregion
   
}
