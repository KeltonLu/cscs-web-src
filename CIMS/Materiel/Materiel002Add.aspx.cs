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

public partial class Materiel_Materiel002Add : PageBase
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

        this.UctrlCardType.SetLeftItem = Materiel002BL.SEL_CARD_TYPE_3;
        UctrlCardType.Is_Using = true;
        grvbImg.NoneData = "";
        
        if (!IsPostBack)
        {
            this.txtSafe_Number2.Enabled = false;
            DataTable dtImg = new DataTable();
            dtImg.Columns.Add(new DataColumn("File_Name", Type.GetType("System.String")));
            dtImg.Columns.Add(new DataColumn("IMG_File_URL", Type.GetType("System.String")));
            this.Session["CardType002Img"] = dtImg;
            this.Session["CardType002ImgNum"] = 0;
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

        if (this.adrtSafe_Type1.Checked)
        {
            radSafe_Type.Value = GlobalString.SafeType.storage;
            if (StringUtil.IsEmpty(this.txtSafe_Number1.Text.Trim()))
            {
                ShowMessage("-最低安全庫存不能為空");
                return;
            }
            txtSafe_Number.Value = this.txtSafe_Number1.Text.Trim().Replace(",","");
        }
        else if (this.adrtSafe_Type2.Checked)
        {
            radSafe_Type.Value = GlobalString.SafeType.days;
            if (StringUtil.IsEmpty(this.txtSafe_Number2.Text.Trim()))
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

        this.txtName.Text = this.txtName.Text.Trim();
        this.txtUnit_Price.Text = this.txtUnit_Price.Text.Trim().Replace(",", "");
        
        try
        {
            CARD_EXPONENT ceModel = new CARD_EXPONENT();
            if (adrtCard.Checked) {
                ceModel.Billing_Type = "1";
            }
            if (adrtBlank.Checked) {
                ceModel.Billing_Type = "2";
            }
            SetData(ceModel);
            //add by linhuanhuang start
            if (txtMaturityDate.Text.ToString().Trim() != "")
            {
                ceModel.Maturity_Date = Convert.ToDateTime(txtMaturityDate.Text.Trim());
            }
            //add by linhuanhuang end
            bl.Add(ceModel, UctrlCardType.GetRightItem, (DataTable)this.Session["CardType002Img"]);
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Materiel002Add.aspx");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #endregion
    #region 上傳圖片文件
    /// <summary>
    /// 上傳圖片文件
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

                DataTable dtImg = (DataTable)this.Session["CardType002Img"];
                if (img.Count > 0)
                {
                    DataRow drImg = dtImg.NewRow();
                    //drImg["File_Name"] = img[0].ToString();
                    drImg["File_Name"] = fn;
                    drImg["IMG_File_URL"] = img[1].ToString();
                    dtImg.Rows.Add(drImg);
                    this.Session["CardType002Img"] = dtImg;
                    this.Session["CardType002ImgNum"] = Convert.ToInt32(this.Session["CardType002ImgNum"]) + 1;

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
    #endregion 上傳圖片文件
    #region 圖片GRIDVIEW綁定
    /// <summary>
    /// 行刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteImg_Command(object sender, CommandEventArgs e)
    {
        try
        {
            DataTable dtImg = (DataTable)Session["CardType002Img"];
            dtImg.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
            this.Session["CardType002Img"] = dtImg;
            this.Session["CardType002ImgNum"] = Convert.ToInt32(this.Session["CardType002ImgNum"]) - 1;

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
            if (0 == Convert.ToInt32(this.Session["CardType002ImgNum"]))
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
            hl.Attributes.Add("onclick", "window.showModalDialog('../CardType/CardType002Img.aspx?ActionType=Add&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');");
        }
    }

    /// <summary>
    /// 設定圖片GRIDVIEW的資料源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbImg_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtImg = (DataTable)this.Session["CardType002Img"];
        if (null != dtImg)
        {
            e.Table = dtImg;
            e.RowCount = dtImg.Rows.Count;
        }
    }
    #endregion 圖片GRIDVIEW綁定
}