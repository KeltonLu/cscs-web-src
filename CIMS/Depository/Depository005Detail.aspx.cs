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

public partial class Depository_Depository005Detail : PageBase
{
    Depository005BL BL = new Depository005BL();

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
            SetControlsForDataRow(drowImp);
            lblTotalNum.Text = int.Parse(lblTotalNum.Text).ToString("N0");
            txtRestock_Number.Text = int.Parse(txtRestock_Number.Text).ToString("N0");
            txtBlemish_Number.Text = int.Parse(txtBlemish_Number.Text).ToString("N0");
            lblReincome_Date.Text = Convert.ToDateTime(lblReincome_Date.Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
            ViewState["CheckStatus"] = lblSendCheck_Status.Text;
            lblSendCheck_Status.Text = lblSendCheck_Status.Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");
            lblOrderForm_Detail_RID.Text = drowImp["Stock_RID"].ToString();

        }

        CalNum();
    }

    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        if (!CheckSerialNum())
            return;

        if (StringUtil.GetByteLength(txtComment.Text.Trim()) > 100)
        {
            ShowMessage("備註不能超過100個字符");
            return;
        }


        string strRID = Request.QueryString["RID"];

        if (int.Parse(lblIncome_Number.Text.Replace(",", "")) < 0)
        {
            ShowMessage("再入庫量不能為負數");
            return;
        }

        DataTable dtblImp = (DataTable)Session["dtblImp"];

        DataRow drowImp = dtblImp.Rows[int.Parse(strRID)];

        DataTable dtbl = (DataTable)ViewState["dtbl"];
        DataRow drow = dtbl.Rows[0];

        drowImp["OrderForm_Detail_RID"] = lblOrderForm_Detail_RID.Text.Substring(0, 12);
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
        drowImp["Restock_Number"] = txtRestock_Number.Text.Trim().Replace(",", "");
        drowImp["Blemish_Number"] = txtBlemish_Number.Text.Trim().Replace(",", "");
        drowImp["Sample_Number"] = txtSample_Number.Text.Trim().Replace(",", "");
        drowImp["Reincome_Number"] = lblIncome_Number.Text.Trim().Replace(",", "");
        drowImp["Check_Type"] = rblCheck_Type.SelectedValue;


        ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "returnValue='1';window.close();", true);

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
        if (!StringUtil.IsEmpty(txtRestock_Number.Text))
            num1 = int.Parse(txtRestock_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtBlemish_Number.Text))
            num2 = int.Parse(txtBlemish_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtSample_Number.Text))
            num3 = int.Parse(txtSample_Number.Text.Replace(",", ""));

        income = num1 - num2 - num3;
        lblIncome_Number.Text = income.ToString("N0");
    }

    private bool CheckSerialNum()
    {
        if (StringUtil.IsEmpty(txtSerial_Number.Text))
        {
            lblSendCheck_Status.Text = "";
            return true;
        }
        else
        {
            string strRID = Request.QueryString["RID"];

            DataTable dtblImp = (DataTable)Session["dtblImp"];

            string strCheckStatus = BL.GetCheckStatus(txtSerial_Number.Text, dtblImp.Rows[int.Parse(strRID)]["Space_Short_Name"].ToString());

            lblSendCheck_Status.Text = strCheckStatus.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");
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
