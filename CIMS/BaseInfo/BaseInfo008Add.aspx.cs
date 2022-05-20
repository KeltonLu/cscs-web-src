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

public partial class BaseInfo_BaseInfo008Add : PageBase
{
    BaseInfo008BL bizLogic = new BaseInfo008BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        UctrlCardType.Is_Using = true;
    }
    protected void btnAddUp_click(object sender, EventArgs e)
    {

        try
        {
            MATERIAL_SPECIAL msModel = new MATERIAL_SPECIAL();
            SetData(msModel);
            bizLogic.add(msModel, UctrlCardType.GetRightItem);     
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo008Add.aspx");
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
    
}
