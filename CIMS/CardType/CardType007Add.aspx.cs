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


public partial class CardType_CardType007Add : PageBase
{
    CardType007BL bizLogic = new CardType007BL();


    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //取得批次下拉bar中的内容
                dropMakeCardType_RID.DataSource = bizLogic.GetTypeAndMakeCardType().Tables[1];
                dropMakeCardType_RID.DataBind();
                //dropMakeCardType_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

                //定位光標的初始位置
                txtFile_Name.Focus();        
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
    protected void btnAdd1_Click(object sender, EventArgs e)
    {

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

        try
        {
            ////當前類別為小計檔，判斷轉換檔名
            //if (radType.Value.Equals(adrtSmallTotal.Value))
            //{
            //    if (txtTransfer_Name.Text.Trim().Equals(""))
            //    {
            //        ShowMessage("轉換檔名不能為空");
            //        lblStra.Visible = true;
            //        return;
            //    }
            //}

            IMPORT_PROJECT ipModel = new IMPORT_PROJECT();

            //獲取頁面控件的值
            SetData(ipModel);

            //判斷當前類別,進行處理
            if (!(radType.Value.Equals(adrtSmallTotal.Value)))
            {
                ipModel.MakeCardType_RID = -1;
            }
            
            
            bizLogic.Add(ipModel);
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
            ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType007Add.aspx");
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
        Response.Redirect("CardType007.aspx?Con=1");
    }

    #endregion

    #region 事件綁定
    #endregion

   #region 欄位/資料補充說明
    /// <summary>
    /// 驗證File_Name是否已存在於資料庫
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ajvFile_Name_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = !bizLogic.ContainsID(e.QueryData.Trim());//驗證File_Name是否已存在於資料庫
    }

    #endregion

}
