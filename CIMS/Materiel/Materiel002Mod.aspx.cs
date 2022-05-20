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

public partial class Materiel_Materiel002Mod : PageBase
{
    Materiel002BL bl = new Materiel002BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];
        UctrlCardType.SetLeftItem = "select * from (" + Materiel002BL.SEL_CARD_TYPE_1 + strRID + ") a where 1>0 ";
        UctrlCardType.Is_Using = true;
        grvbImg.NoneData = "";

        if (!IsPostBack)
        {
            if (StringUtil.IsEmpty(strRID))
            {
                return;
            }

            try
            {
                ////獲取寄卡單信息
                CARD_EXPONENT CardExponentModel = new CARD_EXPONENT();
                DataSet dstSelectedCardType = new DataSet();
                bl.ListModel(strRID, ref CardExponentModel, ref dstSelectedCardType);
                this.hidRID.Value = Convert.ToString(CardExponentModel.RID);
                this.hidRCU.Value = CardExponentModel.RCU;
                this.hidRCT.Value = CardExponentModel.RCT.ToString();
                this.txtName.Text = CardExponentModel.Name;
                this.lblSerial_Number.Text = CardExponentModel.Serial_Number;
                this.txtUnit_Price.Text = CardExponentModel.Unit_Price.ToString("N2");
                this.txtWear_Rate.Text = CardExponentModel.Wear_Rate.ToString("N0");
                //add by linhuanhuang start
                if ("1900/01/01" != CardExponentModel.Maturity_Date.ToString("yyyy/MM/dd"))
                {
                    this.txtMaturityDate.Text = CardExponentModel.Maturity_Date.ToString("yyyy/MM/dd");
                }
                //add by linhuanhuang end
                String radioSafeType = CardExponentModel.Safe_Type;
                if (radioSafeType.Equals(GlobalString.SafeType.storage))
                {
                    // 最低安全庫存
                    this.adrtSafe_Type1.Checked = true;
                    this.txtSafe_Number1.Text = StringUtil.SpecDecimalAddComma(CardExponentModel.Safe_Number.ToString("N0"));
                    this.txtSafe_Number2.Text = "";
                    this.txtSafe_Number2.Enabled = false;
                }
                else if (radioSafeType.Equals(GlobalString.SafeType.days))
                {
                    // 安全天數
                    this.adrtSafe_Type2.Checked = true;
                    this.txtSafe_Number2.Text = CardExponentModel.Safe_Number.ToString("N0");
                    this.txtSafe_Number1.Text = "";
                    this.txtSafe_Number1.Enabled = false;
                }
                string billType = CardExponentModel.Billing_Type;
                if (billType.Equals("1"))
                {
                    this.adrtCard.Checked = true;
                } if (billType.Equals("2"))
                {
                    this.adrtBlank.Checked = true;
                }

                //獲取已選擇卡種
                DataSet selectedCardType = new DataSet();
                selectedCardType = dstSelectedCardType;
                UctrlCardType.SetRightItem = selectedCardType.Tables[0];

                // 取對應的圖片
                DataSet dstImg = bl.GetImgByRID(strRID);
                if (null != dstImg && dstImg.Tables.Count > 0)
                {
                    this.Session["CardType002ModImg"] = dstImg.Tables[0];
                    this.Session["CardType002ModImgNum"] = dstImg.Tables[0].Rows.Count;
                }
                else
                {
                    this.Session["CardType002ModImg"] = null;
                    this.Session["CardType002ModImgNum"] = 0;
                }
                grvbImg.BindData();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "checkRadio();", true);

            // 刪除寄卡單種類檔
            if (this.chkDel.Checked)
            {
                // 檢查該寄卡單是否和卡種類關聯
                //if (bl.ContainsCardExponentCardType(this.hidRID.Value.ToString()))
                //{
                //    ShowMessage("尚有卡種使用中！");
                //    return;
                //}
                if (bl.ChkDelExponent(this.hidRID.Value))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "delConfirm();", true);
                }
                else
                {
                    bl.Delete(this.hidRID.Value);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Materiel002.aspx?Con=1");
                }
            }
            // 保存寄卡單種類檔
            else
            {
                if (this.adrtSafe_Type1.Checked)
                {
                    radSafe_Type.Value = GlobalString.SafeType.storage;
                    if (StringUtil.IsEmpty(this.txtSafe_Number1.Text.Trim()))
                    {
                        ShowMessage("-最低安全庫存不能為空！");
                        return;
                    }
                    txtSafe_Number.Value = this.txtSafe_Number1.Text.Trim().Replace(",", "");
                }
                else if (this.adrtSafe_Type2.Checked)
                {
                    radSafe_Type.Value = GlobalString.SafeType.days;
                    if (StringUtil.IsEmpty(this.txtSafe_Number2.Text))
                    {
                        ShowMessage("-安全天數不能為空！");
                        return;
                    }
                    if (int.Parse(txtSafe_Number2.Text) > 60)
                    {
                        ShowMessage("安全天數不能大於60");
                        return;
                    }
                    txtSafe_Number.Value = this.txtSafe_Number2.Text.Trim().Replace(",", "");
                }

                CARD_EXPONENT ceModel = new CARD_EXPONENT();

                txtName.Text = this.txtName.Text.Trim();
                txtUnit_Price.Text = this.txtUnit_Price.Text.Replace(",", "");
                if (this.adrtBlank.Checked)
                {
                    ceModel.Billing_Type = "2";
                }
                if (this.adrtCard.Checked)
                {
                    ceModel.Billing_Type = "1";
                }
                SetData(ceModel);
                ceModel.RID = Convert.ToInt32(hidRID.Value);
                ceModel.RCT = Convert.ToDateTime(hidRCT.Value);
                ceModel.RCU = Convert.ToString(hidRCU.Value);
                ceModel.RST = GlobalString.RST.ACTIVED;
                //add by linhuanhuang start
                if (txtMaturityDate.Text.ToString().Trim() != "")
                {
                    ceModel.Maturity_Date = Convert.ToDateTime(txtMaturityDate.Text.Trim());
                }
                //add by linhuanhuang end
                bl.Update(ceModel, UctrlCardType.GetRightItem, (DataTable)this.Session["CardType002ModImg"]);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Materiel002.aspx?Con=1");
            }



        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #endregion
    #region 卡片圖檔處理
    /// <summary>
    /// 設置圖片GRIDVIEW資料源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbImg_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtImg = (DataTable)Session["CardType002ModImg"];
        if (null != dtImg)
        {
            e.Table = dtImg;
            e.RowCount = dtImg.Rows.Count;
        }
    }

    /// <summary>
    /// 行刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteImg_Command(object sender, CommandEventArgs e)
    {
        try
        {
            DataTable dtImg = (DataTable)Session["CardType002ModImg"];
            dtImg.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
            this.Session["CardType002ModImg"] = dtImg;
            this.Session["CardType002ModImgNum"] = Convert.ToInt32(this.Session["CardType002ModImgNum"]) - 1;

            grvbImg.BindData();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 行綁定圖片GRIDVIEW
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbImg_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)grvbImg.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (0 == Convert.ToInt32(this.Session["CardType002ModImgNum"]))
                return;

            ImageButton ibtnDeleteImg = null;
            // 刪除的邦定事件
            ibtnDeleteImg = (ImageButton)e.Row.FindControl("ibtnDeleteImg");
            ibtnDeleteImg.CommandArgument = e.Row.RowIndex.ToString();
            ibtnDeleteImg.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            //圖片名的邦定事件
            HyperLink hl = (HyperLink)e.Row.FindControl("hlFile_Name");
            hl.Text = dtbl.Rows[e.Row.RowIndex]["File_Name"].ToString();
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "window.open('../CardType/Cardtype002Img.aspx?ActionType=Mod&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');");
        }
    }
    #endregion 卡片圖檔處理
    #region 卡片圖檔上傳
    /// <summary>
    /// 卡片圖檔上傳
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (fludFileUpload.PostedFile.FileName.Trim() == "")
        {
            ShowMessage("请选择上传图片");
            return;
        }
        else
        {
            try
            {
                ArrayList img = null;
                //返回结果
                img = FileUpload(fludFileUpload.PostedFile);
                if (null != img)
                {
                    ShowMessage(BizMessage.BizPublicMsg.ALT_PicUpdSucess);
                }
                else
                    return;

                string[] str = fludFileUpload.PostedFile.FileName.Split('\\');
                string fn = "";
                if (str.Length > 0)
                {
                    fn = str[str.Length - 1].ToString();
                    HyperLink.Text = fn;
                    HyperLink.NavigateUrl = img[1].ToString();
                }

                DataTable dtImg = (DataTable)this.Session["CardType002ModImg"];
                if (img.Count > 0)
                {
                    DataRow drImg = dtImg.NewRow();
                    //drImg["File_Name"] = img[0].ToString();
                    drImg["File_Name"] = fn;
                    drImg["IMG_File_URL"] = img[1].ToString();
                    dtImg.Rows.Add(drImg);
                    this.Session["CardType002ModImg"] = dtImg;
                    this.Session["CardType002ModImgNum"] = Convert.ToInt32(this.Session["CardType002ModImgNum"]) + 1;

                    grvbImg.BindData();
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 上傳圖片
    /// </summary>
    /// <param name="file"></param>
    new public ArrayList FileUpload(HttpPostedFile file)
    {
        string path = "";
        ArrayList imglist = null;
        if (IsFolderExist())
        {
            string tmpname = file.FileName;
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.IndexOf(".") + 1);
            filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filetype;
            if (filetype.ToLower().Equals("gif") || filetype.ToLower().Equals("jpg") || filetype.ToLower().Equals("jpeg") || filetype.ToLower().Equals("bmp"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        file.SaveAs(Server.MapPath(ConfigurationManager.AppSettings["WebName"] + filename));
                        path = ConfigurationManager.AppSettings["WebName"] + filename;
                        imglist = new ArrayList();
                        imglist.Clear();
                        imglist.Add(filename);
                        imglist.Add(path);
                    }
                    else
                    {
                        throw new AlertException(BizMessage.BizPublicMsg.ALT_PicTooLarge);
                    }
                }
                catch
                {
                    throw new AlertException(BizMessage.BizPublicMsg.ALT_PicUploadFail);
                }
            }
            else
            {
                throw new AlertException(BizMessage.BizPublicMsg.ALT_PicWrongFormat);
            }

            return imglist;
        }
        else
        {
            return imglist;
        }
    }
    #endregion 卡片圖檔上傳
    protected void Button1_Click(object sender, EventArgs e)
    {
        bl.Delete(this.hidRID.Value);
        ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Materiel002.aspx?Con=1");
    }
}