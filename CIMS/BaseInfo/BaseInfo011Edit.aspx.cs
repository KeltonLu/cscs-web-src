//******************************************************************
//*  作    者：WangxiaoYan
//*  功能說明：卡種狀況修改/刪除頁面
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

public partial class BaseInfo_BaseInfo011Edit : PageBase
{
    BaseInfo011BL bizLogic = new BaseInfo011BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //有值，代表是修改！
            string strRID = Request.QueryString["RID"];
            if (!StringUtil.IsEmpty(strRID))
            {
                CARDTYPE_STATUS ctsModel = bizLogic.GetCardTypeStatus(strRID);
                //設置控件的值
                SetControls(ctsModel);

                if (radlIs_UptDepository.SelectedValue == "Y")
                    dropOperate.Enabled = true;
                else
                {
                    dropOperate.Enabled = false;
                    radlIs_UptDepository.Enabled = false;
                }
                
            }
        }
    }
    protected void btnEditUp_click(object sender, EventArgs e)
    {
        CARDTYPE_STATUS ctsModel = new CARDTYPE_STATUS();

        string strRID = Request.QueryString["RID"];
        try
        {
            if (chkDel.Checked)         //刪除
            {
                bizLogic.Delete(strRID);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "BaseInfo011.aspx?Con=1");
            }
            else                        //修改 
            {
                if (StringUtil.GetByteLength(txtComment.Text) > 50)
                {
                    ShowMessage("備註不能超過50個字符");
                    return;
                }

                ctsModel = bizLogic.GetCardTypeStatus(strRID);
                //ctsModel.Status_Code = txtStatus_Code.Text;
                //ctsModel.Status_Name = txtStatus_Name.Text;
                //ctsModel.Is_Display = radlIs_Display.Text;
                //ctsModel.Comment = txtComment.Text;
                SetData(ctsModel);
                bizLogic.Update(ctsModel);
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "BaseInfo011.aspx?Con=1");
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
