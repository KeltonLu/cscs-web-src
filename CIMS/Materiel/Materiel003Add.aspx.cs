using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Materiel_Materiel003Add : PageBase
{
    Materiel003BL bl = new Materiel003BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        UctrlCardType.Is_Using = true;
        if (!IsPostBack)
        {
            txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            txtEnd_Date.Text = DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            DataSet dstlMakeType = new DataSet();
            dstlMakeType = bl.MakeTypeList();
            mlbMakeType.DataSource = dstlMakeType.Tables[0];
            mlbMakeType.DataBind();
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "checkRadio();", true);

        if (TypePart.Checked)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "setEnabled();", true);
            if (UctrlCardType.GetRightItem.Rows.Count <= 0)
            {
                ShowMessage("請選擇卡種！");
                return;
            }
        }
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "setDisabled();", true);


        string radioSafeType = null;
        if (this.storageNum.Checked)
        {
            radioSafeType = GlobalString.SafeType.storage;
            if (StringUtil.IsEmpty(this.txtStorageNum.Text))
            {
                ShowMessage("請輸入\"最低安全庫存\"欄位的值！");
                return;
            }
        }
        else if (this.daysNum.Checked)
        {
            radioSafeType = GlobalString.SafeType.days;
            if (StringUtil.IsEmpty(this.txtDays.Text))
            {
                ShowMessage("請輸入\"安全天數\"欄位的值！");
                return;
            }
            if (int.Parse(txtDays.Text) > 60)
            {
                ShowMessage("安全天數不能大於60");
                return;
            }
        }
        else
        {
            radioSafeType = GlobalString.SafeType.over;
        }
        string linkType = null;
        if (this.TypeAll.Checked)
        {
            linkType = GlobalString.AllPart.All;
        }
        else
        {
            linkType = GlobalString.AllPart.Part;
        }



        DMTYPE_INFO DMInfo = new DMTYPE_INFO();

        SetData(DMInfo);

        DMInfo.Card_Type_Link_Type = linkType;

        DMInfo.Safe_Type = radioSafeType;
        if (this.usingRadio.Checked)
        {
            DMInfo.Is_Using = GlobalString.YNType.No;
        }
        else if (this.stopRadio.Checked)
        {
            DMInfo.Is_Using = GlobalString.YNType.Yes;
        }
        if (radioSafeType.Equals(GlobalString.SafeType.storage))
        {
            DMInfo.Safe_Number = Convert.ToInt32(StringUtil.SpecHDelComma(this.txtStorageNum.Text));
        }
        else if (radioSafeType.Equals(GlobalString.SafeType.days))
        {
            DMInfo.Safe_Number = Convert.ToInt32(this.txtDays.Text);
        }
        else if (radioSafeType.Equals(GlobalString.SafeType.over))
        {
            DMInfo.Safe_Number = Convert.ToInt32(0);
        }
        if (this.adrtBlank.Checked)
        {
            DMInfo.Billing_Type = "2";
        } if (this.adrtCard.Checked)
        {
            DMInfo.Billing_Type = "1";
        }
        // 開始日期
        DateTime beginTime = Convert.ToDateTime(this.txtBegin_Date.Text);
        // 終止日期
        DateTime endTime = Convert.ToDateTime(this.txtEnd_Date.Text.Trim());
        if (endTime < beginTime)
        {
            ShowMessage("終止日期不能比起始日期還早");
            return;
        }

        // 停用選中
        if (this.stopRadio.Checked)
        {
            if (DateTime.Parse(beginTime.ToString("yyyy/MM/dd 00:00:00")) < DateTime.Now &&
                DateTime.Parse(endTime.ToString("yyyy/MM/dd 23:59:59")) > DateTime.Now)
            {
                ShowMessage("有效期間為有效時，不可停用。");
                return;
            }
        }
        try
        {
            bl.Add(DMInfo, this.mlbMakeType, UctrlCardType.GetRightItem);
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Materiel003Add.aspx");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    
    #endregion
}