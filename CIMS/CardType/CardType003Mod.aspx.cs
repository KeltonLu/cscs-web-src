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

public partial class CardType_CardType003Mod : PageBase
{
    CardType003BL ctAudit = new CardType003BL();
    CARD_TYPE Card_Type = new CARD_TYPE();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            if (StringUtil.IsEmpty(strRID))
            {
                return;
            }
            ////獲取卡種信息
            CARD_HALLMARK Card_HallMark = ctAudit.GetCardExponentModelByRID(strRID);
            CARD_TYPE Card_Type = ctAudit.GetCardName(Card_HallMark.CardType_RID);
            HidCardType_RID.Value = Card_HallMark.CardType_RID.ToString();
            lbCardType.Text = Card_Type.Display_Name;
            txtSerial_Number.Text = Card_HallMark.Serial_Number;
            if (Card_HallMark.Begin_Date.ToShortDateString() == "1900/01/01" || Card_HallMark.Begin_Date.ToShortDateString() == "1900/1/1")
            {
                txtBegin_Date.Text = "";
            }
            else
            {
                txtBegin_Date.Text = Card_HallMark.Begin_Date.ToShortDateString();
            }
            if (Card_HallMark.Finish_Date.ToShortDateString() == "1900/01/01" || Card_HallMark.Finish_Date.ToShortDateString() == "1900/1/1")
            {
                txtFinish_Date.Text = "";
            }
            else
            {
                txtFinish_Date.Text = Card_HallMark.Finish_Date.ToShortDateString();
            }
            dropSendCheck_Status.SelectedValue = Card_HallMark.SendCheck_Status;

            txtValidate_Number.Text = Card_HallMark.Validate_Number;

            dropSendCheck_Status.DataSource = ctAudit.getParamSendCheck().Tables[0];
            dropSendCheck_Status.DataBind();
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (this.chkDel.Checked != true)
        {
            if (this.txtFinish_Date.Text != "" && this.txtBegin_Date.Text == "")
            {
                ShowMessage("請填寫送審日期！");
                this.txtBegin_Date.Focus();
                return;
            }
        }
      

        try
        {
            CARD_HALLMARK chModel = new CARD_HALLMARK();
            string strRID = Request.QueryString["RID"];

            chModel.RID = Convert.ToInt32(strRID);
            chModel.CardType_RID = Convert.ToInt32(HidCardType_RID.Value);
            chModel.Serial_Number = txtSerial_Number.Text;
            if (txtBegin_Date.Text != "")
            {
                chModel.Begin_Date = Convert.ToDateTime(txtBegin_Date.Text);
            }
            if (txtFinish_Date.Text != "")
            {
                chModel.Finish_Date = Convert.ToDateTime(txtFinish_Date.Text);

            }
            chModel.SendCheck_Status = dropSendCheck_Status.SelectedValue;
            chModel.Validate_Number = txtValidate_Number.Text;
            // 刪除
            if (!StringUtil.IsEmpty(strRID))
            {

                if (this.chkDel.Checked)
                {
                    chModel.RST = GlobalString.RST.DELETE;
                    ctAudit.Delete(strRID);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType003.aspx?Con=1");
                }
                // 保存
                else
                {
                    chModel.RST = GlobalString.RST.ACTIVED;
                    ctAudit.Update(chModel);
                    ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType003.aspx?Con=1");
                }
               
            }
        }
        catch (Exception ex)
        {

            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
       
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType003.aspx?Con=1");
    }
    #endregion
}
