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

public partial class CardType_CardType005ModBase : PageBase
{
    CardType005BL bl = new CardType005BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        uctrlCARDNAME.Is_Using = true;
      
        if (!IsPostBack)
        {
            string strFactory_RID = Request.QueryString["Factory_RID"];
           
            if (!StringUtil.IsEmpty(strFactory_RID))
            {
                this.lbFactory_RID.Text = bl.GetFactory(strFactory_RID);
            }
            this.uctrlCARDNAME.SetRightItem = bl.GetSelectedCardType(strFactory_RID);
            ViewState["Old_uctrlCARDNAME_GetRightItem"] = uctrlCARDNAME.GetRightItem;
        }
        //查詢沒有做基本設定的卡種SEL_UNSELECTED_CARDTYPE
        this.uctrlCARDNAME.SetLeftItem = CardType005BL.SEL_UNSELECTED_CARDTYPE;
        uctrlCARDNAME.Is_Using = true;
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        try
        {
            string strFactory_RID = Request.QueryString["Factory_RID"];
            // 刪除
            if (!StringUtil.IsEmpty(strFactory_RID))
            {

                if (this.chkDel.Checked)
                {
                    bl.DeleteBase(strFactory_RID,lbFactory_RID.Text, uctrlCARDNAME.GetRightItem);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType005.aspx?Con=1");
                }
                // 保存
                else
                {
                    //pcModel.RST = GlobalString.RST.ACTIVED;
                    DataTable Old_dtblCardType = (DataTable)ViewState["Old_uctrlCARDNAME_GetRightItem"];
                    bl.UpdateBase(strFactory_RID,lbFactory_RID.Text, uctrlCARDNAME.GetRightItem, Old_dtblCardType);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "CardType005.aspx?Con=1");
                }

                Session.Remove("Percentage_Number");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType005.aspx?Con=1");
    }
}
