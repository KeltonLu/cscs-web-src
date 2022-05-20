using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Reflection;

public partial class CardType_CardType002Add :PageBase
{

    CardType002BL blManager = new CardType002BL();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        grvbCardTypeGroup.NoneData = "";
        grvbImg.NoneData = "";

        if (!Page.IsPostBack)
        {
            try
            {
                // 換卡版面
                DataSet dstCardTypeName = null;
                dstCardTypeName = blManager.GetCardName();
                dropChange_Space_RID.DataTextField = "Name";
                dropChange_Space_RID.DataValueField = "RID";
                dropChange_Space_RID.DataSource = dstCardTypeName.Tables[0];
                dropChange_Space_RID.DataBind();
                dropChange_Space_RID.Items.Insert(0, new ListItem("", "0"));

                // 替換卡版面
                dropReplace_Space_RID.DataTextField = "Name";
                dropReplace_Space_RID.DataValueField = "RID";
                dropReplace_Space_RID.DataSource = dstCardTypeName.Tables[0];
                dropReplace_Space_RID.DataBind();
                dropReplace_Space_RID.Items.Insert(0, new ListItem("", "0"));

                DataTable dtImg = new DataTable();
                dtImg.Columns.Add(new DataColumn("File_Name", Type.GetType("System.String")));
                dtImg.Columns.Add(new DataColumn("IMG_File_URL", Type.GetType("System.String")));
                this.Session["CardType002Img"] = dtImg;
                this.Session["CardType002ImgNum"] = 0;

                // 設定卡種群組信息
                grvbCardTypeGroup.BindData();
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
            if (StringUtil.GetByteLength(txtComment.Text) > 50)
            {
                ShowMessage("備註不能超過50個字符");
                return;
            }


            int total_Type = Convert.ToInt32(this.txtTYPE.Text.Trim());
            int photo = Convert.ToInt32(this.txtPHOTO.Text.Trim());
            int affinity = Convert.ToInt32(this.txtAFFINITY.Text);
            if (blManager.ContainsTypePhotoWholeName(total_Type, photo, affinity, ""))
            {
                ShowMessage("CARDTYPE+PHOTO+AFFINITY組合必須唯一");
                this.txtTYPE.Focus();
                return;
            }

            // 終止日期
            DateTime endTime = Convert.ToDateTime("1900-01-01");
            if (!StringUtil.IsEmpty(this.txtEnd_Time.Text.Trim()))
            {
                endTime = Convert.ToDateTime(this.txtEnd_Time.Text.Trim());
                if (endTime < Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd 23:59:59")))
                {
                    ShowMessage("終止日期必須比當天日期晚");
                    return;
                }

                // 停用選中
                if (this.adrtStop.Checked)
                {
                    ShowMessage("有效期間為有效時，不可停用。");
                    return;
                }
            }

            // 所有用途對應的群組必須選擇群組
            List<string> selectedCardTypeGroup = new List<string>();
            for (int intRow = 0; intRow < this.grvbCardTypeGroup.Rows.Count; intRow++)
            {
                DropDownList dropCardTypeGroup = (DropDownList)this.grvbCardTypeGroup.Rows[intRow].FindControl("dropCard_Group");
                if (StringUtil.IsEmpty(dropCardTypeGroup.SelectedValue.ToString()))
                {
                    ShowMessage("所有用途對應的群組必須選擇群組");
                    return;
                }
                if (dropCardTypeGroup.SelectedValue.ToString() == "0")
                {
                    ShowMessage("所有用途對應的群組必須選擇群組");
                    return;
                }
                selectedCardTypeGroup.Add(dropCardTypeGroup.SelectedValue.ToString());
            }

            CARD_TYPE ctModel = new CARD_TYPE();
            ctModel.TYPE = Convert.ToString(txtTYPE.Text);
            txtAFFINITY.Text = txtAFFINITY.Text.Replace(",", "");
            ctModel.AFFINITY = txtAFFINITY.Text;
            ctModel.PHOTO = Convert.ToString(txtPHOTO.Text);
            if (txtBIN.Text.Trim().ToString() != "")
            {
                txtBIN.Text = txtBIN.Text.Trim().Replace(",", "");
                ctModel.BIN = Convert.ToInt32(txtBIN.Text);
            }

            ctModel.Name = txtName.Text;
            ctModel.Change_Space_RID = Convert.ToInt32(dropChange_Space_RID.SelectedValue);
            ctModel.Replace_Space_RID = Convert.ToInt32(dropReplace_Space_RID.SelectedValue);

            ctModel.Begin_Time = DateTime.Now.Date;
            if (txtEnd_Time.Text.Trim().ToString() != "")
            {
                ctModel.End_Time = Convert.ToDateTime(txtEnd_Time.Text);
            }

            if (this.adrtUse.Checked)
            {
                ctModel.Is_Using = GlobalString.YNType.Yes;
            }
            else
            {
                ctModel.Is_Using = GlobalString.YNType.No;
            }
            ctModel.Print_Back = this.txtPrint_Back.Text.Trim();
            ctModel.Print_Cover = this.txtPrint_Cover.Text.Trim();
            ctModel.Comment = this.txtComment.Text.Trim();

            SetData(ctModel);
            blManager.Add(ctModel, selectedCardTypeGroup, (DataTable)this.Session["CardType002Img"]);

            this.Session.Remove("CardType002Img");
            this.Session.Remove("CardType002ImgNum");

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "CardType002Add.aspx");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 卡種版面簡稱AJAX驗證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AjaxValidator_CardType_Name_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = true;
        string CardTypeName = e.QueryData;
        try
        {
            // 檢查版面簡稱是否唯一
            if (blManager.ContainsCardTypeName(CardTypeName))
                e.IsAllowSubmit = false;
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #region 卡群組GRIDVIEW綁定
    /// <summary>
    /// 設定資料源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbCardTypeGroup_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataSet dstPARAM = null;
        this.Session["CardType002GroupNum"] = 0;
        try
        {
            dstPARAM = blManager.GetParamUse();
            if (null != dstPARAM)
            {
                e.Table = dstPARAM.Tables[0];
                e.RowCount = dstPARAM.Tables[0].Rows.Count;
                this.Session["CardType002GroupNum"] = e.RowCount;
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbCardTypeGroup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtPARAM = (DataTable)grvbCardTypeGroup.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (0 == Convert.ToInt32(this.Session["CardType002GroupNum"]))
                return;

            try
            {
                DropDownList dropCard_Group = null;
                dropCard_Group = (DropDownList)e.Row.FindControl("dropCard_Group");

                // 取Param_Code
                string Param_Code = Convert.ToString(dtPARAM.Rows[e.Row.RowIndex]["Param_Code"]);

                // 以Param_Code取卡群組
                DataSet dstCard_Group = (DataSet)blManager.GetCardGoupByParam_Code(Param_Code);
                if (null != dstCard_Group && dstCard_Group.Tables.Count > 0)
                {
                    dropCard_Group.DataSource = dstCard_Group.Tables[0];
                    dropCard_Group.DataValueField = "RID";
                    dropCard_Group.DataTextField = "Group_Name";
                    dropCard_Group.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    #endregion 卡群組GRIDVIEW綁定

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
            ShowMessage("请选择上传檔案");
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
                    ShowMessage("檔案上傳成功");
                }
                else
                    return;

                DataTable dtImg = (DataTable)this.Session["CardType002Img"];
                if (img.Count > 0)
                {
                    DataRow drImg = dtImg.NewRow();
                    drImg["File_Name"] = img[0].ToString();
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
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
            string fileoldname = filename;
            filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filetype;
            if (filetype.ToLower().Equals("gif") || filetype.ToLower().Equals("jpg") || filetype.ToLower().Equals("jpeg") || filetype.ToLower().Equals("bmp") || filetype.ToLower().Equals("pdf"))
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
                        imglist.Add(fileoldname);
                        imglist.Add(path);
                    }
                    else
                    {
                        ShowMessage("上傳檔案不能大於10M");
                        return null;
                    }
                }
                catch
                {
                    ShowMessage("上傳檔案失敗");
                    return null;
                }
            }
            else
            {
                ShowMessage("上傳格式必須為bmp、jpg、gif、pdf");
                return null;
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
            hl.Attributes.Add("onclick", "javascript:window.open('"+dtbl.Rows[e.Row.RowIndex]["IMG_File_URL"].ToString()+"','','width=800,height=600,scrollbars=yes,resizable=yes,top=0,left=0');");
            //hl.Attributes.Add("onclick", "window.showModalDialog('CardType002Img.aspx?ActionType=Add&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');");
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
