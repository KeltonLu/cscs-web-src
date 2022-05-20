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

public partial class CardType_CardType005AddBase : PageBase
{
    CardType005BL bl = new CardType005BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        uctrlCARDNAME.Is_Using = true;
        
        if (!IsPostBack)
        {
            // 獲取 Perso廠商資料
            DataSet dstFactory = bl.GetFactoryList();
            dropFactory.DataValueField = "RID";
            dropFactory.DataTextField = "Factory_ShortName_CN";
            dropFactory.DataSource = dstFactory.Tables[0];
            dropFactory.DataBind();
            //dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        }
        //查詢沒有做基本設定的卡種
        this.uctrlCARDNAME.SetLeftItem = CardType005BL.SEL_UNSELECTED_CARDTYPE;
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        //增加
        try
        {
            if (uctrlCARDNAME.GetRightItem.Rows.Count == 0)
            {
                ShowMessage("沒有已選擇卡種!");
                return;
            }
            PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
            pcModel.Factory_RID = Convert.ToInt32(dropFactory.SelectedValue);
            bl.AddBase(pcModel,uctrlCARDNAME.GetRightItem);

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "CardType005AddBase.aspx");
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
