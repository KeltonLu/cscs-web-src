//******************************************************************
//*  作    者：wangxiaoyan
//*  功能說明：製卡類別設定修改/刪除
//*  創建日期：2008-09-01
//*  修改日期： 
//*  修改記錄：

//*******************************************************************
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

public partial class CardType_CardType006Edit : PageBase
{
    CardType006BL bizLogic = new CardType006BL();

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
            //初始化下拉框資料
            if (!IsPostBack)
            {
                dropGroup_Name.DataSource = bizLogic.GetCardTypeGroup().Tables[0];
                dropGroup_Name.DataBind();
            }
            //有值，代表是修改！
            string strRID = Request.QueryString["RID"];
            if (!StringUtil.IsEmpty(strRID))
            {
                MAKE_CARD_TYPE cardTypeModel = bizLogic.GetMakeCardType(strRID);
                //設置控件的值
                SetControls(cardTypeModel);
                dropGroup_Name.SelectedValue = (string)cardTypeModel.CardGroup_RID.ToString();
                radlIs_Report.Text = cardTypeModel.Is_Report;
                radlIs_Import.Text = cardTypeModel.Is_Import;
                //製域處理
                trDel.Visible = true;   
            }
        }
    }
    /// <summary>
    /// 修改/刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnEditUp_Click(object sender, EventArgs e)
    {
        MAKE_CARD_TYPE cardTypeModel = new MAKE_CARD_TYPE();

        string strRID = Request.QueryString["RID"];

        try
        {
            if (chkDel.Checked)         //刪除
            {
                bizLogic.Delete(strRID);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType006.aspx?Con=1");
            }
            else                        //修改 
            {
                cardTypeModel = bizLogic.GetMakeCardType(strRID);
                cardTypeModel.CardGroup_RID = int.Parse(dropGroup_Name.SelectedValue);
                cardTypeModel.Type_Name = txtType_Name.Text;
                cardTypeModel.Is_Report = radlIs_Report.Text;
                cardTypeModel.Is_Import = radlIs_Import.Text;
                bizLogic.Update(cardTypeModel);
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"],"CardType006.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    
    /// <summary>
    /// 同btnEdit_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnEditDown_Click(object sender, EventArgs e)
    {
        btnEditUp_Click(sender, e);
    }

      
    #endregion
}