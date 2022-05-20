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

public partial class Materiel_Materiel001Add : PageBase
{
    Materiel001BL bl = new Materiel001BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.UctrlCardType.SetLeftItem = Materiel001BL.SEL_CARD_TYPE_3;
        UctrlCardType.Is_Using = true;
        if (!IsPostBack)
        {
            this.radSafe_Type1.Checked = true;
            this.radSafe_Type2.Checked = false;
            this.txtSafe_Number1.Enabled = true;
            this.txtSafe_Number2.Enabled = false;
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "checkRadio();", true);

        if (radSafe_Type1.Checked)
        {
            radSafe_Type.Value = "1";
            if (txtSafe_Number1.Text.Trim().Equals(""))
            {
                ShowMessage("最低安全庫存不能為空");
                return;
            }
            txtSafe_Number.Value = txtSafe_Number1.Text;
        }
        if (radSafe_Type2.Checked)
        {
            radSafe_Type.Value = "2";
            if (txtSafe_Number2.Text.Trim().Equals(""))
            {
                ShowMessage("安全天數不能為空");
                return;
            }
            if (int.Parse(txtSafe_Number2.Text) > 60)
            {
                ShowMessage("安全天數不能大於60");
                return;
            }

            txtSafe_Number.Value = txtSafe_Number2.Text;

            
        }
   

        txtUnit_Price.Text = txtUnit_Price.Text.Replace(",", "");
        txtSafe_Number.Value = txtSafe_Number.Value.Replace(",", "");

        try
        {
            ENVELOPE_INFO eiModel = new ENVELOPE_INFO();
            if (this.adrtBlank.Checked) {
                eiModel.Billing_Type = "2";
            } if (this.adrtCard.Checked) {
                eiModel.Billing_Type = "1";
            }
            
            SetData(eiModel);
            bl.Add(eiModel, UctrlCardType.GetRightItem);

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Materiel001Add.aspx");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
   
    #endregion

}
