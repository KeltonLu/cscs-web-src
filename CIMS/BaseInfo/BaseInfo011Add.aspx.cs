//******************************************************************
//*  作    者：WangxiaoYan
//*  功能說明：卡種狀況設定頁面
//*  創建日期：2008-10-7
//*  修改日期： 
//*  修改記錄：
//*            □2008-10-7
//*              1.創建 王曉燕
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

public partial class BaseInfo_BaseInfo011Add : PageBase
{
    BaseInfo011BL bizLogic = new BaseInfo011BL();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnAddUp_click(object sender, EventArgs e)
    {
        if (StringUtil.GetByteLength(txtComment.Text) > 50)
        {
            ShowMessage("備註不能超過50個字符");
            return;
        }

        try
        {
            CARDTYPE_STATUS ctsModel = new CARDTYPE_STATUS();
            SetData(ctsModel);
            bizLogic.Add(ctsModel);     
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo011Add.aspx");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void btnAddDown_click(object sender, EventArgs e)
    {
        btnAddUp_click(sender,e);
    }

    protected void radlIs_UptDepository_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (radlIs_UptDepository.SelectedValue == "Y")
        //    dropOperate.Enabled = true;
        //else
        //{
        //    dropOperate.Enabled = false;
        //    radlIs_UptDepository.Enabled = false;
        //}
    }
}
