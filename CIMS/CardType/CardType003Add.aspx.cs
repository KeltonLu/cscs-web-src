//******************************************************************
//*  作    者：JunWang
//*  功能說明：卡片版面送審檢核管理作業
//*  創建日期：2008-08-29
//*  修改日期：2008-08-29
//*  修改記錄：
//*            □2008-08-29
//*              1.創建 王俊
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

public partial class CardType_CardType003Add : PageBase
{
    CardType003BL ctAudit = new CardType003BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        UrctrlCardTypeSelect1.Is_Using = true;
        if (!IsPostBack)
        {
            dropSendCheck_Status.DataSource = ctAudit.getParamSendCheck().Tables[0];
            dropSendCheck_Status.DataBind();

            string strSerial_Number = Request.QueryString["Serial_Number"];
            string strCardType = Request.QueryString["CardType"];

            if (!StringUtil.IsEmpty(strSerial_Number)) {
                txtSerial_Number.Text = strSerial_Number;
            }

            if (!StringUtil.IsEmpty(strCardType))
            {
                UrctrlCardTypeSelect1.CardType = strCardType;
            }
        }
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {

        if (this.txtFinish_Date.Text != "" && this.txtBegin_Date.Text == "")
        {
            ShowMessage("請填寫送審日期！");
            this.txtBegin_Date.Focus();
            return;
        }




        try
        {
            CARD_HALLMARK chModel = new CARD_HALLMARK();
            SetData(chModel);
            if (this.UrctrlCardTypeSelect1.CardType != "" && this.UrctrlCardTypeSelect1.CardType != null)
            {
                chModel.CardType_RID = Convert.ToInt32(this.UrctrlCardTypeSelect1.CardType);
                ctAudit.Add(chModel);
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType003Add.aspx");
            }
            else
            {
                ShowMessage("卡種不能為空");
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType003.aspx?Con=1");
    }
    #endregion
}
