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

public partial class BaseInfo_BaseInfo008Edit : PageBase
{
    BaseInfo008BL bizlogic = new BaseInfo008BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];
        UctrlCardType.Is_Using = true;
        if (!IsPostBack)
        {

            MATERIAL_SPECIAL eiModel = bizlogic.GetMaterialByRid(strRID);

            //設置控件的值
            SetControls(eiModel); 
            //載入預算附加信息
            DataSet dstCardType = bizlogic.GetCard(strRID);

            //卡種
            UctrlCardType.SetRightItem = dstCardType.Tables[0];
        }
    }
    protected void btnEditUp_click(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];
        try
        {
            if (chkDel.Checked)         //刪除
            {
                bizlogic.Delete(strRID);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "BaseInfo008.aspx?Con=1");
            }
            else                        //修改 
            {
                MATERIAL_SPECIAL eiModel = new MATERIAL_SPECIAL();
                SetData(eiModel);
                eiModel.RID = int.Parse(strRID);
                bizlogic.Update(eiModel, UctrlCardType.GetRightItem);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo008.aspx?Con=1");
            }


        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void btnEditDown_click(object sender, EventArgs e)
    {
        btnEditUp_click(sender, e);
    }
    
}
