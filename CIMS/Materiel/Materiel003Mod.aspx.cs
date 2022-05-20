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

public partial class Materiel_Materiel003Mod : PageBase
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
            string strRID = Request.QueryString["RID"];
            if (StringUtil.IsEmpty(strRID))
            {
                return;
            }
            //獲取DM信息
            DMTYPE_INFO dmModle = null;
            dmModle = bl.SelectedDMInfo(strRID);
            SetControls(dmModle);
            this.txtUnit_Price.Text = StringUtil.SpecDecimalAddComma(Convert.ToString(dmModle.Unit_Price));
            this.hidRID.Value = Convert.ToString(dmModle.RID);
            if (dmModle.Billing_Type.Equals("1")) {
                this.adrtCard.Checked = true;
            }else  if (dmModle.Billing_Type.Equals("2")) {
                this.adrtBlank.Checked = true;
            }
            if (dmModle.Is_Using.Equals(GlobalString.YNType.No))
            {
                this.usingRadio.Checked = true;
            }
            else if (dmModle.Is_Using.Equals(GlobalString.YNType.Yes))
            {
                this.stopRadio.Checked = true;
            }
            String radioSafeType = dmModle.Safe_Type;
            if (radioSafeType.Equals(GlobalString.SafeType.storage))
            {
                this.storageNum.Checked = true;
                this.daysNum.Checked = false;
                this.overNum.Checked = false;
                this.txtStorageNum.Text = StringUtil.SpecAddComma(Convert.ToString(dmModle.Safe_Number));
            }
            else if (radioSafeType.Equals(GlobalString.SafeType.days))
            {
                this.daysNum.Checked = true;
                this.storageNum.Checked = false;
                this.overNum.Checked = false;
                this.txtDays.Text = Convert.ToString(dmModle.Safe_Number);
            }
            else
            {
                this.daysNum.Checked = false;
                this.storageNum.Checked = false;
                this.overNum.Checked = true;
            }
            string linkType = dmModle.Card_Type_Link_Type;
            if (linkType.Equals(GlobalString.AllPart.All))
            {
                this.TypeAll.Checked = true;
                this.TypePart.Checked = false;
            }
            else
            {
                this.TypeAll.Checked = false;
                this.TypePart.Checked = true;
            }
            //獲取可選擇批次信息
            DataSet dstlMakeType = new DataSet();
            dstlMakeType = bl.MakeTypeList();
            mlbMakeType.DataSource = dstlMakeType.Tables[0];
            mlbMakeType.DataBind();
            //獲取已選擇批次信息
            DataSet selectedMakeType = new DataSet();
            selectedMakeType = bl.SelectedMakeTypeList(strRID);
            mlbMakeType.RightListBox.DataSource = selectedMakeType.Tables[0];
            mlbMakeType.RightListBox.DataBind();
            if (dmModle.Card_Type_Link_Type.Equals(GlobalString.AllPart.Part))
            {
                //獲取已選擇卡種
                DataSet selectedCardType = new DataSet();
                selectedCardType = bl.SelectedCardTypeList(strRID);
                UctrlCardType.SetRightItem = selectedCardType.Tables[0];
            }
        }
        if (this.TypeAll.Checked)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "setDisabled();", true);
            //UctrlCardType.Enable = false;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "setEnabled();", true);
            
            //UctrlCardType.Enable = true;
        }
        if (this.storageNum.Checked)
        {
            this.txtStorageNum.Enabled = true;
            this.txtDays.Enabled = false;
        }
        else if (this.daysNum.Checked)
        {
            this.txtDays.Enabled = true;
            this.txtStorageNum.Enabled = false;
        }
        else if (this.overNum.Checked)
        {
            this.txtStorageNum.Enabled = false;
            this.txtDays.Enabled = false;
        }

    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        DMTYPE_INFO DMInfo = bl.SelectedDMInfo(Request.QueryString["RID"].ToString());

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


        if (!this.chkDel.Checked)
        {
            string radioSafeType = null;
            if (this.storageNum.Checked)
            {
                radioSafeType = GlobalString.SafeType.storage;
                if (!this.chkDel.Checked)
                {
                    if (StringUtil.IsEmpty(this.txtStorageNum.Text))
                    {
                        ShowMessage("請輸入\"最低安全庫存\"欄位的值！");
                        return;
                    }
                }
            }
            else if (this.daysNum.Checked)
            {
                radioSafeType = GlobalString.SafeType.days;
                if (!this.chkDel.Checked)
                {
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
                 
            SetData(DMInfo);           
            DMInfo.Card_Type_Link_Type = linkType;
            if (!StringUtil.IsEmpty(this.txtUnit_Price.Text))
            {
                DMInfo.Unit_Price = Convert.ToDecimal(this.txtUnit_Price.Text);
            }
            if (this.usingRadio.Checked)
            {
                DMInfo.Is_Using = GlobalString.YNType.No;
            }
            else if (this.stopRadio.Checked)
            {
                DMInfo.Is_Using = GlobalString.YNType.Yes;
            }
            DMInfo.Safe_Type = radioSafeType;

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
            }
            if (this.adrtCard.Checked)
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
        }
        
        try
        {
            if (this.chkDel.Checked)
            {
                DMTYPE_INFO DMModle = bl.SelectedDMInfo(this.hidRID.Value.Trim());
                if (DMModle != null)
                {
                    if (DMModle.End_Date > DateTime.Now)
                    {
                        ShowMessage("該DM還在有效期內，無法刪除！");
                        return;
                    }
                }
                DMInfo.RST = GlobalString.RST.DELETE;
                bl.Delete(DMInfo);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Materiel003.aspx?Con=1");
            }
            else
            {
                DMInfo.RST = GlobalString.RST.ACTIVED;
                bl.Update(DMInfo, this.mlbMakeType, UctrlCardType.GetRightItem);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Materiel003.aspx?Con=1");
            }
            
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
        
    #endregion

}