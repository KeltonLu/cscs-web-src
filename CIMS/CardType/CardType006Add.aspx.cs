//******************************************************************
//*  作    者：wangxiaoyan
//*  功能說明：製卡類別設定新增頁面
//*  創建日期：2008-08-29
//*  修改日期： 
//*  修改記錄：

//*******************************************************************
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

public partial class CardType_CardType006Add : PageBase
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
        //初始化下拉框資料
        if (!IsPostBack)
        {
            dropGroup_Name.DataSource = bizLogic.GetCardTypeGroup().Tables[0];
            dropGroup_Name.DataBind();
        }

    }
    /// <summary>
    /// 新增製卡類別
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddUp_Click(object sender, EventArgs e)
    {
        try
        {
            MAKE_CARD_TYPE cardTypeModel = new MAKE_CARD_TYPE();
            SetData(cardTypeModel);
            cardTypeModel.CardGroup_RID = Convert.ToInt32(dropGroup_Name.SelectedValue);
            cardTypeModel.Is_Report = radlIs_Report.Text;
            cardTypeModel.Is_Import = radlIs_Import.Text;
            bizLogic.Add(cardTypeModel);
            ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType006Add.aspx");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }

    }
    
    /// <summary>
    /// 新增製卡類別
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddDown_Click(object sender, EventArgs e)
    {
        btnAddUp_Click(sender, e);
    }
    
    #endregion
}
