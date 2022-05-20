//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片入庫自動匯入
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

public partial class Depository_Depository003Mod : PageBase
{
    Depository003BL BL = new Depository003BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strStock_RID = Request.QueryString["RID"];
            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strStock_RID))
            {
                DataSet dst = BL.GetModData(strStock_RID);
                if (dst != null)
                {
                    DataRow drow = dst.Tables[0].Rows[0];

                    ViewState["RID"] = drow["RID"].ToString();

                    SetControlsForDataRow(drow);

                    //結案,請款
                    if (drow["Case_Status"].ToString() == "Y" || drow["Is_AskFinance"].ToString() == "Y" || drow["Is_Check"].ToString() == "Y")
                    {
                        txtBlemish_Number.Enabled = false;
                        txtComment.Enabled = false;
                        txtIncome_Number.Enabled = false;
                        txtSample_Number.Enabled = false;
                        txtSendCheck_Status.Enabled = false;
                        txtSerial_Number.Enabled = false;
                        txtStock_Number.Enabled = false;
                        rblCheck_Type.Enabled = false;
                        chkDel.Enabled = false;
                    }

                    //日結
                    if (this.IsCheck())
                    {
                        btnSubmitDn.Enabled = false;
                        btnSubmitUp.Enabled = false;
                        ShowMessage("今天已經日結，不可修改入庫信息");
                    }

                    if (drow["Stock_RID"].ToString().Substring(8, 4) == "9999")
                    {
                        RequiredFieldValidator1.EnableClientScript = false;
                        RequiredFieldValidator1.Enabled = false;
                    }

                    int RemainNum = 0;

                    if (!StringUtil.IsEmpty(lblNUMBER.Text))
                    {
                        lblNUMBER.Text = int.Parse(lblNUMBER.Text).ToString("N0");
                        RemainNum = int.Parse(dst.Tables[0].Rows[0]["number"].ToString()) - int.Parse(dst.Tables[1].Rows[0][0].ToString()) + int.Parse(dst.Tables[2].Rows[0][0].ToString()) - int.Parse(dst.Tables[3].Rows[0][0].ToString());
                    }
                    else
                    {
                        lblNUMBER.Text = "0";
                    }

                    lblRemainNum.Text = RemainNum.ToString("N0");
                    txtStock_Number.Text = int.Parse(txtStock_Number.Text).ToString("N0");
                    txtBlemish_Number.Text = int.Parse(txtBlemish_Number.Text).ToString("N0");
                    txtSample_Number.Text = int.Parse(txtSample_Number.Text).ToString("N0");
                    lblIncome_Date.Text = Convert.ToDateTime(lblIncome_Date.Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
                    txtSendCheck_Status.Text = txtSendCheck_Status.Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");
                    rblCheck_Type.SelectedValue = dst.Tables[0].Rows[0]["Check_Type"].ToString();

                    ViewState["Space_Short_RID"] = dst.Tables[0].Rows[0]["Space_Short_RID"].ToString();
                }
            }
        }

        CalNum();
    }
    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        string strStock_RID = Request.QueryString["RID"];

        if (!chkDel.Checked)
        {

            if (txtBlemish_Number.Enabled == false)
            {
                ShowMessageAndGoPage("儲存成功", "Depository003CaseClose.aspx?ActionType=Edit&RID=" + ViewState["RID"].ToString());
                return;
            }

            if (!CheckSerialNum())
                return;

            if (int.Parse(txtIncome_Number.Text.Replace(",", "")) < 0)
            {
                ShowMessage("入庫量不能為負數");
                return;
            }

            if (StringUtil.GetByteLength(txtComment.Text) > 200)
            {
                ShowMessage("備註不能超過200個字符");
                return;
            }
        }
        try
        {
            if (chkDel.Visible)
            {
                if (chkDel.Checked)
                {
                    BL.Delete(strStock_RID);
                    ShowMessageAndGoPage("刪除成功", "Depository003.aspx?Con=1");
                    return;
                }
            }
            DEPOSITORY_STOCK dsModel = new DEPOSITORY_STOCK();
            txtStock_Number.Text = txtStock_Number.Text.Replace(",", "");
            txtBlemish_Number.Text = txtBlemish_Number.Text.Replace(",", "");
            txtSample_Number.Text = txtSample_Number.Text.Replace(",", "");
            txtIncome_Number.Text = txtIncome_Number.Text.Replace(",", "");

            SetData(dsModel);

            dsModel.Check_Type = rblCheck_Type.SelectedValue;
            dsModel.Stock_RID = strStock_RID;
            BL.Update(dsModel);

            if (strStock_RID.Substring(8, 4) == "9999")
            {
                ShowMessageAndGoPage("儲存成功", "Depository003.aspx?Con=1");
            }
            else
            {
                ShowMessageAndGoPage("儲存成功", "Depository003CaseClose.aspx?ActionType=Edit&RID=" + ViewState["RID"].ToString());
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void btnCalNum_Click(object sender, EventArgs e)
    {
        CheckSerialNum();
    }
    
    #endregion

    #region 欄位/資料補充說明
    private void CalNum()
    {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num = 0;
        if (!StringUtil.IsEmpty(txtStock_Number.Text))
            num1 = int.Parse(txtStock_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtBlemish_Number.Text))
            num2 = int.Parse(txtBlemish_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtSample_Number.Text))
            num3 = int.Parse(txtSample_Number.Text.Replace(",", ""));

        num = num1 - num2 - num3;

        txtIncome_Number.Text = num.ToString("N0");

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
            string strCheckStatus = BL.GetCheckStatus(txtSerial_Number.Text, ViewState["Space_Short_RID"].ToString());

            txtSendCheck_Status.Text = strCheckStatus.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");

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
