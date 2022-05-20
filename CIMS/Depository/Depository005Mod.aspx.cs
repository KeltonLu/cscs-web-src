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

public partial class Depository_Depository005Mod : PageBase
{
    Depository005BL BL = new Depository005BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        //txtSerial_Number.Text = txtSerial_Number.Text.Trim().Replace(",", "");
        //txtRestock_Number.Text = txtRestock_Number.Text.Trim().Replace(",", "");
        //txtBlemish_Number.Text = txtBlemish_Number.Text.Trim().Replace(",", "");
        //txtSample_Number.Text = txtSample_Number.Text.Trim().Replace(",", "");

        if (!IsPostBack)
        {
            string strStock_RID = Request.QueryString["RID"];
            string strReport_RID = Request.QueryString["ID"];

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strStock_RID))
            {
                //查詢當前畫面所需要的數據
                DataSet dst = BL.GetModData(strStock_RID, strReport_RID);
                //如果查詢結果不為空
                if (dst != null)
                {
                    //取查詢結果的第一個表結構
                    DataRow drow = dst.Tables[0].Rows[0];
                    //向控件中置值
                    SetControlsForDataRow(drow);
                    //再入庫資料未日結、未出帳的時候才能修改，否則不能
                    if (drow["Is_AskFinance"].ToString() == "Y" || drow["Is_Check"].ToString() == "Y")
                    {
                        btnSubmitDn.Enabled = false;
                        btnSubmitUp.Enabled = false;
                    }
                    //如果入庫資料為人工入庫
                    if (drow["Stock_RID"].ToString().Substring(8, 4) == "9999")
                    {
                        RequiredFieldValidator1.EnableClientScript = false;
                        RequiredFieldValidator1.Enabled = false;
                    }
                    lblNUMBER.Text = Convert.ToInt32(dst.Tables[1].Rows[0][0]).ToString("N0");
                    //if (!StringUtil.IsEmpty(lblNUMBER.Text))
                    //{
                    //    lblNUMBER.Text = int.Parse(lblNUMBER.Text).ToString("N0");
                    //}
                    //else
                    //{
                    //    lblNUMBER.Text = "0";
                    //}
                    lblStock_RID.Text = drow["Stock_RID"].ToString();
                    txtSerial_Number.Text = drow["Serial_Number"].ToString();
                    txtRestock_Number.Text = int.Parse(txtRestock_Number.Text).ToString("N0");
                    txtBlemish_Number.Text = int.Parse(txtBlemish_Number.Text).ToString("N0");
                    txtSample_Number.Text = int.Parse(txtSample_Number.Text).ToString("N0");
                    lblReincome_Date.Text = Convert.ToDateTime(lblReincome_Date.Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
                    lblBlank_Factory_Name.Text = drow["Blank_Factory_Name"].ToString();
                    lblPerso_Factory_Name.Text = drow["Perso_Factory_Name"].ToString();
                    lblSendCheck_Status.Text = lblSendCheck_Status.Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");

                    rblCheck_Type.SelectedValue = dst.Tables[0].Rows[0]["Check_Type"].ToString();

                    ViewState["Space_Short_Name"] = dst.Tables[0].Rows[0]["Space_Short_Name"].ToString();
                }
            }

            if (this.IsCheck())
            {
                ShowMessage("今天已經日結，不可修改再入庫信息");
                btnSubmitDn.Enabled = false;
                btnSubmitUp.Enabled = false;
            }
        }

        CalNum();
    }
    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        if (!chkDel.Checked)
        {
            if (StringUtil.GetByteLength(txtComment.Text.Trim()) > 100)
            {
                ShowMessage("備註不能超過100個字符");
                return;
            }

            if (!CheckSerialNum())
                return;

            if (int.Parse(lblReincome_Number.Text.Replace(",", "")) < 0)
            {
                ShowMessage("入庫量不能為負數");
                return;
            }

            if (int.Parse(lblReincome_Number.Text.Replace(",", "")) == 0)
            {
                ShowMessage("入庫量不能為0");
                return;
            }
        }

        string strStock_RID = Request.QueryString["RID"];
        string strReport_RID = Request.QueryString["ID"];
        string strRID = Request.QueryString["RID1"];

        try
        {
            if (chkDel.Visible)
            {
                if (chkDel.Checked)
                {
                    BL.Delete(strRID);
                    ShowMessageAndGoPage("刪除成功", "Depository005.aspx?Con=1");
                    return;
                }
            }
            DEPOSITORY_RESTOCK dsModel = new DEPOSITORY_RESTOCK();
            txtRestock_Number.Text = txtRestock_Number.Text.Trim().Replace(",", "");
            txtBlemish_Number.Text = txtBlemish_Number.Text.Trim().Replace(",", "");
            txtSample_Number.Text = txtSample_Number.Text.Trim().Replace(",", "");
            //txtIncome_Number.Text = txtIncome_Number.Text.Trim().Replace(",", "");
            lblReincome_Number.Text = lblReincome_Number.Text.Trim().Replace(",", "");

            //SetData(dsModel);

            dsModel = BL.getDRModel(strRID);
            dsModel.Restock_Number = Convert.ToInt32(txtRestock_Number.Text);
            dsModel.Blemish_Number = Convert.ToInt32(txtBlemish_Number.Text);
            dsModel.Sample_Number = Convert.ToInt32(txtSample_Number.Text);
            dsModel.Reincome_Number = Convert.ToInt32(lblReincome_Number.Text);

            dsModel.Check_Type = rblCheck_Type.SelectedValue;
            dsModel.Stock_RID = strStock_RID;
            dsModel.Report_RID = strReport_RID;
            dsModel.Comment = txtComment.Text;
            dsModel.Serial_Number = txtSerial_Number.Text.Trim();

            BL.Update(dsModel);
            ShowMessageAndGoPage("儲存成功", "Depository005.aspx?Con=1");
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

    #region 欄位/數據補充說明
    private void CalNum()
    {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num = 0;
        if (!StringUtil.IsEmpty(txtRestock_Number.Text))
            num1 = int.Parse(txtRestock_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtBlemish_Number.Text))
            num2 = int.Parse(txtBlemish_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtSample_Number.Text))
            num3 = int.Parse(txtSample_Number.Text.Replace(",", ""));

        num = num1 - num2 - num3;

        lblReincome_Number.Text = num.ToString("N0");

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
            string strCheckStatus = BL.GetCheckStatus(txtSerial_Number.Text, ViewState["Space_Short_Name"].ToString());

            lblSendCheck_Status.Text = strCheckStatus.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");

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
