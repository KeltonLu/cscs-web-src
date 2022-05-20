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

public partial class CardType_CardType002Mod :PageBase
{
    CardType002BL blManager = new CardType002BL();
   
    protected void Page_Load(object sender, EventArgs e)
    {
        grvbCardTypeGroup.NoneData = "";
        grvbPersoFactory.NoneData = "";

        grvbImg.NoneData = "";
        string strRID = Request.QueryString["RID"];
        ajvCardTypeName.QueryInfo = strRID;

        if (!Page.IsPostBack)
        {
            
            this.hdRID.Value = strRID;


            #region 設置當前訊息
            try
            {
                //初始卡种
                CARD_TYPE ctModel = blManager.GetCardType(strRID);

                // 換卡版面
                DataSet dstCardTypeName = null;
                dstCardTypeName = blManager.GetCardName();
                dropChange_Space_RID.DataTextField = "Name";
                dropChange_Space_RID.DataValueField = "RID";
                dropChange_Space_RID.DataSource = dstCardTypeName.Tables[0];
                dropChange_Space_RID.DataBind();
                if (dropChange_Space_RID.Items.FindByValue(strRID) != null)
                    dropChange_Space_RID.Items.Remove(dropChange_Space_RID.Items.FindByValue(strRID));
                dropChange_Space_RID.Items.Insert(0, new ListItem("", "0"));

                // 替換卡版面
                dropReplace_Space_RID.DataTextField = "Name";
                dropReplace_Space_RID.DataValueField = "RID";
                dropReplace_Space_RID.DataSource = dstCardTypeName.Tables[0];
                dropReplace_Space_RID.DataBind();
                if (dropReplace_Space_RID.Items.FindByValue(strRID) != null)
                    dropReplace_Space_RID.Items.Remove(dropReplace_Space_RID.Items.FindByValue(strRID));
                dropReplace_Space_RID.Items.Insert(0, new ListItem("", "0"));

                SetControls(ctModel);

                ShowIsUsingDrop(dropChange_Space_RID, ctModel.Change_Space_RID.ToString());
                ShowIsUsingDrop(dropReplace_Space_RID, ctModel.Replace_Space_RID.ToString());

                //txtAFFINITY.Text = (int.Parse(ctModel.AFFINITY)).ToString("N0");
                //if (0 != ctModel.BIN)
                //    this.txtBIN.Text = ctModel.BIN.ToString("N0");
                //else
                //    this.txtBIN.Text = "";

                // 開始日期
                this.lblBegin_Time.Text = ctModel.Begin_Time.ToString("yyyy/MM/dd");
                // 終止日期
                if ("1900/01/01" != ctModel.End_Time.ToString("yyyy/MM/dd"))
                    this.txtEnd_Time.Text = ctModel.End_Time.ToString("yyyy/MM/dd");
                else
                    this.txtEnd_Time.Text = "";

                // 是否停用
                if (ctModel.Is_Using==GlobalString.YNType.Yes)
                {
                    this.adrtUse.Checked = true;
                }
                else
                {
                    this.adrtStop.Checked = true;
                }
                this.hdExponent_RID.Value = Convert.ToString(ctModel.Exponent_RID);
                this.hdEnvelope_RID.Value = Convert.ToString(ctModel.Envelope_RID);


                // 查詢出所有此卡種用到的材質
                this.lblMateriel.Text = "";
                DataSet dstMaterial = blManager.GetMaterialByCardTypeRID(strRID);
                if (null != dstMaterial && dstMaterial.Tables.Count > 0 && dstMaterial.Tables[0].Rows.Count > 0)
                {
                    for (int intRow = 0; intRow < dstMaterial.Tables[0].Rows.Count; intRow++)
                    { 
                        if (intRow==0)
                        {
                            this.lblMateriel.Text = Convert.ToString(dstMaterial.Tables[0].Rows[intRow]["Material_Name"]);
                        }else
                        {
                            this.lblMateriel.Text = this.lblMateriel.Text + "、" + Convert.ToString(dstMaterial.Tables[0].Rows[intRow]["Material_Name"]);
                        }
                    }
                }

                // 查詢該卡種使用的信封
                DataSet dstExponentAndEnvelope = blManager.GetExponentAndEnvelopeByCardTypeRID(strRID);
                if (null != dstExponentAndEnvelope && dstExponentAndEnvelope.Tables.Count > 0 &&
                        dstExponentAndEnvelope.Tables[0].Rows.Count > 0)
                {
                    // 寄卡單
                    this.lblCARD_EXPONENT.Text = Convert.ToString(dstExponentAndEnvelope.Tables[0].Rows[0]["Name"]);
                }
                if (null != dstExponentAndEnvelope && dstExponentAndEnvelope.Tables.Count > 0 &&
                                        dstExponentAndEnvelope.Tables[1].Rows.Count > 0)
                {
                    // 信封
                    this.lblEnvelope.Text = Convert.ToString(dstExponentAndEnvelope.Tables[1].Rows[0]["Name"]);
                }

                // 綁定Perso廠商及代製訊息
                grvbPersoFactory.DataSource = blManager.GetProjectStep(strRID);
                grvbPersoFactory.DataBind();

                // 取卡種群組
                DataSet dstGroupRID = blManager.GetGroupRIDByCardTypeRID(strRID);
                if (null != dstGroupRID && dstGroupRID.Tables.Count > 0)
                {
                    this.ViewState["CardType002ModGroupRID"] = dstGroupRID.Tables[0];
                    this.ViewState["CardType002ModGroupRIDNum"] = dstGroupRID.Tables[0].Rows.Count;
                }
                else
                {
                    this.ViewState["CardType002ModGroupRID"] = null;
                    this.ViewState["CardType002ModGroupRIDNum"] = 0;
                }
                grvbCardTypeGroup.BindData();

                // 取卡種對應的圖片
                DataSet dstImg = blManager.GetImgByCardTypeRID(strRID);
                if (null != dstImg && dstImg.Tables.Count > 0)
                {
                    this.ViewState["CardType002ModImg"] = dstImg.Tables[0];
                    this.ViewState["CardType002ModImgNum"] = dstImg.Tables[0].Rows.Count;
                }
                else
                {
                    this.ViewState["CardType002ModImg"] = null;
                    this.ViewState["CardType002ModImgNum"] = 0;
                }
                grvbImg.BindData();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
            #endregion 設置當前訊息
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
            if (blManager.ContainsCardTypeName(CardTypeName, e.QueryInfo))
                e.IsAllowSubmit = false;
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    
    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strRID = this.hdRID.Value.ToString();
        try
        {
            if (this.chkDelete.Checked)
            {
                // 刪除
                blManager.Delete(strRID);

                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType002.aspx?Con=1");
            }
            else
            {
                if (StringUtil.GetByteLength(txtComment.Text) > 50)
                {
                    ShowMessage("備註不能超過50個字符");
                    return;

                }

                // 修改
                int total_Type = Convert.ToInt32(this.txtTYPE.Text.Trim());
                int photo = Convert.ToInt32(this.txtPHOTO.Text.Trim());
                int affinity = Convert.ToInt32(this.txtAFFINITY.Text);
                if (blManager.ContainsTypePhotoWholeName(total_Type, photo, affinity, strRID))
                {
                    ShowMessage("CARDTYPE+PHOTO+AFFINITY組合必須唯一");
                    this.txtTYPE.Focus();
                    return;
                }

                // 開始日期
                DateTime beginTime = Convert.ToDateTime(this.lblBegin_Time.Text + " 23:59:59");
                // 終止日期
                DateTime endTime = Convert.ToDateTime("1900-01-01");
                if (!StringUtil.IsEmpty(this.txtEnd_Time.Text.Trim()))
                {
                    endTime = Convert.ToDateTime(this.txtEnd_Time.Text.Trim());
                    if (endTime < beginTime)
                    {
                        ShowMessage("終止日期不能比起始日期還早");
                        return;
                    }

                    // 停用選中
                    if (this.adrtStop.Checked)
                    {
                        if (DateTime.Parse(beginTime.ToString("yyyy/MM/dd 00:00:00")) < DateTime.Now && 
                            DateTime.Parse(endTime.ToString("yyyy/MM/dd 23:59:59")) > DateTime.Now)
                        {
                            ShowMessage("有效期間為有效時，不可停用。");
                            return;    
                        }
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
                    if (dropCardTypeGroup.SelectedValue.ToString()=="0")
                    {
                        ShowMessage("所有用途對應的群組必須選擇群組");
                        return;
                    }
                    selectedCardTypeGroup.Add(dropCardTypeGroup.SelectedValue.ToString());
                }

                CARD_TYPE ctModel = new CARD_TYPE();
                
                ctModel.TYPE = Convert.ToString(txtTYPE.Text);
                this.txtAFFINITY.Text = this.txtAFFINITY.Text.Replace(",","");
                ctModel.AFFINITY = Convert.ToString(txtAFFINITY.Text);
                ctModel.PHOTO = Convert.ToString(txtPHOTO.Text);
                if (txtBIN.Text.Trim()!= "")
                {
                    txtBIN.Text = txtBIN.Text.Replace(",","");
                    ctModel.BIN = Convert.ToInt32(txtBIN.Text);
                }

                ctModel.Name = txtName.Text;
                ctModel.Change_Space_RID = Convert.ToInt32(dropChange_Space_RID.SelectedValue);
                ctModel.Replace_Space_RID = Convert.ToInt32(dropReplace_Space_RID.SelectedValue);
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

                SetData(ctModel);
                ctModel.RID = int.Parse(strRID);
                if (!StringUtil.IsEmpty(this.hdEnvelope_RID.Value.ToString()))
                    ctModel.Envelope_RID = int.Parse(this.hdEnvelope_RID.Value.ToString());
                if (!StringUtil.IsEmpty(this.hdExponent_RID.Value.ToString()))
                    ctModel.Exponent_RID = int.Parse(this.hdExponent_RID.Value.ToString());
                blManager.Update(ctModel, selectedCardTypeGroup, (DataTable)this.ViewState["CardType002ModImg"]);

                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "CardType002.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType002.aspx?Con=1"));
    }

    #region 卡群組訊息設定
    /// <summary>
    /// 設定資料源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbCardTypeGroup_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataSet dstPARAM = null;
        this.ViewState["CardType002GroupNum"] = 0;
        try
        {
            dstPARAM = blManager.GetParamUse();
            if (null != dstPARAM)
            {
                e.Table = dstPARAM.Tables[0];
                e.RowCount = dstPARAM.Tables[0].Rows.Count;
                this.ViewState["CardType002GroupNum"] = e.RowCount;
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
            if (0 == Convert.ToInt32(this.ViewState["CardType002GroupNum"]))
                return;

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

                // 設定卡群組選定的值
                DataTable dtblGroupRID = (DataTable)this.ViewState["CardType002ModGroupRID"];
                if (null!=dtblGroupRID && dtblGroupRID.Rows.Count>0)
                {
                    bool blFoundGroupRID = false;// 是否已經找到了卡群組
                    for (int intIndex = 0; intIndex < dropCard_Group.Items.Count; intIndex++)
                    {
                        for (int intRow = 0; intRow < dtblGroupRID.Rows.Count; intRow++)
                        {
                            if (dropCard_Group.Items[intIndex].Value.ToString() ==
                                Convert.ToString(dtblGroupRID.Rows[intRow]["Group_RID"]))
                            {
                                dropCard_Group.Items[intIndex].Selected = true;
                                blFoundGroupRID = true;
                                break;
                            }
                        }
                        if (blFoundGroupRID)
                        {
                            break;
                        }
                    }    
                }
            }
        }
    }
    #endregion 卡群組訊息設定

    #region 卡片圖檔處理
    /// <summary>
    /// 設置圖片GRIDVIEW資料源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvbImg_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtImg = (DataTable)ViewState["CardType002ModImg"];
        if (null!=dtImg)
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
            DataTable dtImg = (DataTable)ViewState["CardType002ModImg"];
            dtImg.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
            this.ViewState["CardType002ModImg"] = dtImg;
            this.ViewState["CardType002ModImgNum"] = Convert.ToInt32(this.ViewState["CardType002ModImgNum"]) - 1;

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
            if (0 == Convert.ToInt32(this.ViewState["CardType002ModImgNum"]))
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
            hl.Attributes.Add("onclick", "javascript:window.open('" + dtbl.Rows[e.Row.RowIndex]["IMG_File_URL"].ToString() + "','','width=800,height=600,scrollbars=yes,resizable=yes,top=0,left=0');");
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

                DataTable dtImg = (DataTable)this.ViewState["CardType002ModImg"];
                if (img.Count > 0)
                {
                    DataRow drImg = dtImg.NewRow();
                    drImg["File_Name"] = img[0].ToString();
                    drImg["IMG_File_URL"] = img[1].ToString();
                    dtImg.Rows.Add(drImg);
                    this.ViewState["CardType002ModImg"] = dtImg;
                    this.ViewState["CardType002ModImgNum"] = Convert.ToInt32(this.ViewState["CardType002ModImgNum"]) + 1;

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
    #endregion 卡片圖檔上傳

}
