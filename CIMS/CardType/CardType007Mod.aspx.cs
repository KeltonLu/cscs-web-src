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
using System.Collections.Generic;

public partial class CardType_CardType007Mod : PageBase
{
    CardType007BL bizLogic = new CardType007BL();

    #region 事件處理

    //頁面加載
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            if (StringUtil.IsEmpty(strRID))
            {
                return;
            }
            try
            {
                IMPORT_PROJECT ipModel = bizLogic.GetImportProject(strRID); 

                // 批次設置
                dropMakeCardType_RID.DataValueField = "Value";
                dropMakeCardType_RID.DataTextField = "Text";
                dropMakeCardType_RID.DataSource = bizLogic.GetTypeAndMakeCardType().Tables[1];
                dropMakeCardType_RID.DataBind();                
                //設置控件的值
                SetControls(ipModel);


                
                //類別設置
                if (adrtSmallTotal.Value == ipModel.Type)
                {
                    lbParam_Name.Text = "小計檔";
                    this.adrtSmallTotal.Checked = true;
                    this.adrtNextMonth.Checked = false;
                    this.adrtYear.Checked = false;
                    trCardType1.Visible = true;
                }
                else if(adrtNextMonth.Value == ipModel.Type)
                {
                    lbParam_Name.Text = "次月預測";
                    this.adrtSmallTotal.Checked = false;
                    this.adrtNextMonth.Checked = true;
                    this.adrtYear.Checked = false;
                    trCardType1.Visible = false;
                }
                else if (adrtYear.Value == ipModel.Type)
                {
                    lbParam_Name.Text = "年度預測";
                    this.adrtSmallTotal.Checked = false;
                    this.adrtNextMonth.Checked = false;
                    this.adrtYear.Checked = true;
                    trCardType1.Visible = false;
                }

                Session["File_Name"] = ipModel.File_Name;
                //定位光標的初始位置
                txtFile_Name.Focus();
           }
            catch (Exception ex)
            {
                ExceptionFactory.CreateAlertException(this, ex.Message);
            }
        }
    }

    //確定按鈕
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {


        IMPORT_PROJECT ipModel = new IMPORT_PROJECT();//定義數據模型
        string strRID = Request.QueryString["RID"];
        try
        {
            
            if (chkDel.Checked)
            { //刪除處理

                bizLogic.Delete(strRID);
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType007.aspx?Con=1");
            }
            else//修改處理
            {
                //獲取有關小計檔與匯入項目設定的信息
                ipModel = bizLogic.GetImportProject(strRID);

                //獲取當前選擇的類別
                if (adrtSmallTotal.Checked)
                {
                    radType.Value = adrtSmallTotal.Value;
                }
                else if (adrtNextMonth.Checked)
                {
                    radType.Value = adrtNextMonth.Value;
                }
                else if (adrtYear.Checked)
                {
                    radType.Value = adrtYear.Value;
                }

                SetData(ipModel);

                if (!(Session["File_Name"].Equals(txtFile_Name.Text.Trim())))
                {
                    if (bizLogic.ContainsID(txtFile_Name.Text.Trim()))//驗證File_Name是否已存在於資料庫
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
                        ShowMessage("主機檔案名稱已經存在");
                        return;
                    }

                }

                ////判斷當前類別,進行處理
                if (ipModel.Type.Equals(adrtSmallTotal.Value))
                {
                //    if (txtTransfer_Name.Text.Trim().Equals(""))
                //    {
                //        ShowMessage("轉換檔名不能為空");
                //        lblStra.Visible = true;
                //        return;
                //    }
                }
                else
                {
                    ipModel.MakeCardType_RID = -1;
                }

                
                //提交修改數據到數據庫
                bizLogic.Update(ipModel);
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType007.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }


    //取消按鈕
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType007.aspx?Con=1");
    }

    #endregion 

    #region 欄位/數據補充說明
    /// <summary>
    /// 驗證File_Name是否已存在於資料庫
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void ajvFile_Name_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    //{
    //    if (!(Session["File_Name"].Equals(e.QueryData.Trim())))
    //    {
    //        e.IsAllowSubmit = !bizLogic.ContainsID(e.QueryData.Trim());//驗證File_Name是否已存在於資料庫]
    //    }
    //    else
    //    {
    //        e.IsAllowSubmit = true;
    //        return;
    //    }
    //}

    #endregion

}
