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

public partial class Materiel_Materiel001Mod : PageBase
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
        string strRID = Request.QueryString["RID"];

        this.UctrlCardType.SetLeftItem = "SELECT * FROM ("+Materiel001BL.SEL_CARD_TYPE_3 + " UNION " + Materiel001BL.SEL_CARD_TYPE_2 + strRID+") A WHERE 1>0 ";
        UctrlCardType.Is_Using = true;
        if (!IsPostBack)
        {

            ENVELOPE_INFO eiModel = bl.GetEnvelope(strRID);

            //設置控件的值
            SetControls(eiModel);
            if (eiModel.Safe_Type.Equals("1"))
            {
                radSafe_Type1.Checked = true;
                radSafe_Type2.Checked = false;
                txtSafe_Number1.Text = eiModel.Safe_Number.ToString("N0");
                txtSafe_Number2.Text = "";
                txtSafe_Number1.Enabled = true;
                txtSafe_Number2.Enabled = false;
            }
            else
            {
                radSafe_Type1.Checked = false;
                radSafe_Type2.Checked = true;
                txtSafe_Number1.Text = "";
                txtSafe_Number2.Text = eiModel.Safe_Number.ToString();
                txtSafe_Number1.Enabled = false;
                txtSafe_Number2.Enabled = true;
            }
            if (eiModel.Billing_Type.Equals("2")) {
                this.adrtBlank.Checked = true;

            } if (eiModel.Billing_Type.Equals("1")) {
                this.adrtCard.Checked = true;
            
            }
            txtUnit_Price.Text = eiModel.Unit_Price.ToString("N2");

            //載入預算附加信息
            DataSet dstEnvelope = bl.GetCard(strRID);

            //卡種
            UctrlCardType.SetRightItem = dstEnvelope.Tables[0];
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

        if (!chkDel.Checked)
        {
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
        }
        
        string strRID = Request.QueryString["RID"];

        

        try
        {
            if (chkDel.Checked)         //刪除
            {
                if (bl.ChkDelENVELOPE(strRID))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "delConfirm();", true);
                }
                else
                {
                    bl.Delete(strRID);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Materiel001.aspx?Con=1");
                }
            }
            else                        //修改 
            {
                ENVELOPE_INFO eiModel = new ENVELOPE_INFO();
                if (adrtCard.Checked)
                {
                    eiModel.Billing_Type = "1";

                } if (adrtBlank.Checked)
                {
                    eiModel.Billing_Type = "2";
                }
                SetData(eiModel);
            
                eiModel.RID = int.Parse(strRID);
                bl.Update(eiModel, UctrlCardType.GetRightItem);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Materiel001.aspx?Con=1");
            }

            
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    
    #endregion

    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            string strRID = Request.QueryString["RID"];
            bl.Delete(strRID);
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Materiel001.aspx?Con=1");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
}
