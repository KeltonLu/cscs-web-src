//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-08-29
//*  修改日期：2008-09-02 16:00
//*  修改記錄：
//*            □2008-08-29
//*              1.創建 潘秉奕
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
using Microsoft.Practices.EnterpriseLibrary.Data;

public partial class CardType_CardType001Edit : PageBase
{
    CardType001BL ctManager = new CardType001BL();
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];

            ctManager.dropCard_PurposeBind(drpParam_Name);

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {
                CARD_GROUP cgModel = ctManager.GetParam(strRID);

                //設置控件的值
                SetControls(cgModel);

                drpParam_Name.SelectedIndex = drpParam_Name.Items.IndexOf(drpParam_Name.Items.FindByText(ctManager.GetParamName(cgModel.Param_Code.ToString())));

                LbLeft.Items.Clear();
                DataSet dsltCardType = ctManager.GetCardType(drpParam_Name.SelectedValue);
                ViewState["GroupCardType"] = dsltCardType;
                LbLeft.DataTextField = "Display_Name";
                LbLeft.DataValueField = "RID";
                LbLeft.DataSource = dsltCardType;
                LbLeft.DataBind();

                LbRight.Items.Clear();
                LbRight.DataSource = ctManager.CardTypeList(txtGroup_Name.Text, drpParam_Name.SelectedValue);
                LbRight.DataTextField = "Display_Name";
                LbRight.DataValueField = "RID";
                LbRight.DataBind();
                for (int i = 0; i < LbRight.Items.Count; i++)
                {
                    LbLeft.Items.Remove(LbRight.Items[i]);
                }

                Hid_Code.Value = drpParam_Name.SelectedValue;
                Hid_Name.Value = txtGroup_Name.Text;

                //製域處理
                trDel.Visible = true;
                IsNew.Value = "";
                lblTitle.Text = "卡種群組維護作業修改/刪除";

                drpParam_Name.Enabled = false;
                //txtGroup_Name.Enabled = false;
            }
            else //無值，代表是新增！
            {
                LbLeftBind();

                //製域處理
                trDel.Visible = false;
                IsNew.Value = "insert";
                lblTitle.Text = "卡種群組維護作業新增";
            }
        }

        //this.chkDel.Attributes.Add("onclick", "ConfirmDel();");
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        CARD_GROUP cgModel = new CARD_GROUP();

        DataSet dstmp = null;

        string strRID = Request.QueryString["RID"];

        ArrayList al = new ArrayList();
        for (int i = 0; i < LbRight.Items.Count; i++)
        {
            al.Add(LbRight.Items[i].Value);
        }

        try
        {
            if (StringUtil.IsEmpty(strRID))    //增加
            {
                SetData(cgModel);

                dstmp = ctManager.LoadParamInfoByPName(drpParam_Name.SelectedValue);
                if (dstmp != null)
                {
                    cgModel.Param_Code = dstmp.Tables[0].Rows[0]["param_code"].ToString();
                }

                if (!ctManager.IsHave(cgModel.Group_Name, cgModel.Param_Code))
                {
                    ctManager.Add(cgModel,al);

                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc,"CardType001Edit.aspx");
                }
                else
                {
                    ShowMessage(BizMessage.BizMsg.ALT_CARDTYPE_001_01);
                    //throw new AlertException(GlobalStringManager.Default["Alert_CardGroupExist"]);
                }
            }
            else
            {
                if (chkDel.Checked)         //刪除
                {
                    ctManager.Delete(strRID);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType001.aspx?Con=1");
                }
                else                        //修改 
                {
                    cgModel = ctManager.GetParam(strRID);

                    SetData(cgModel);

                    dstmp = ctManager.LoadParamInfoByPName(drpParam_Name.SelectedValue);
                    if (dstmp != null)
                    {
                        cgModel.Param_Code = dstmp.Tables[0].Rows[0]["param_code"].ToString();
                    }

                    if (Hid_Name.Value != txtGroup_Name.Text.Trim() || Hid_Code.Value != drpParam_Name.SelectedValue)
                    {
                        if (!ctManager.IsHave(cgModel.Group_Name, cgModel.Param_Code))
                        {
                            ctManager.Update(cgModel,al);

                            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc,
                                "CardType001.aspx?Con=1");
                        }
                        else
                        {
                            ShowMessage(BizMessage.BizMsg.ALT_CARDTYPE_001_01);
                            //throw new AlertException(GlobalStringManager.Default["Alert_CardGroupExist"]);
                        }
                    }
                    else
                    {
                        ctManager.Update(cgModel,al);

                        ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc,
                            "CardType001.aspx?Con=1");
                        //ShowMessage("欄位值沒有變更，無需修改！");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    protected void drpParam_Name_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataSet dslt = ctManager.GetCardType(drpParam_Name.SelectedValue);
        LbLeft.DataTextField = "Display_Name";
        LbLeft.DataValueField = "RID";
        LbLeft.DataSource = dslt;
        LbLeft.DataBind();

        LbRight.Items.Clear();
    }

    #region 卡種選擇
    /// <summary>
    /// 資料綁定
    /// </summary>
    protected void LbLeftBind()
    {
        LbLeft.Items.Clear();

        DataSet dsltCardType = ctManager.GetCardType(drpParam_Name.Items[0].Value);
        ViewState["GroupCardType"] = dsltCardType;

        LbLeft.DataTextField = "Display_Name";
        LbLeft.DataValueField = "RID";
        LbLeft.DataSource = dsltCardType;
        LbLeft.DataBind();
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < LbLeft.Items.Count; i++)
        {
            LbRight.Items.Add(LbLeft.Items[i]);
        }
        LbLeft.Items.Clear();
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        int i = 0;
        while (i < LbLeft.Items.Count)
        {
            if (LbLeft.Items[i].Selected == true)
            {
                LbRight.Items.Add(LbLeft.Items[i]);
                LbLeft.Items.Remove(LbLeft.Items[i]);
            }
            else
                i += 1;
        }
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["GroupCardType"];

        int i = 0;
        while (i < LbRight.Items.Count)
        {
            if (LbRight.Items[i].Selected == true)
            {
                //foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
                //{
                //    if (LbRight.Items[i].Value == drowCardType["RID"].ToString())
                //    {
                //        LbLeft.Items.Add(LbRight.Items[i]);
                //        break;
                //    }
                //}
                LbLeft.Items.Add(LbRight.Items[i]);
                LbRight.Items.Remove(LbRight.Items[i]);
            }
            else
                i += 1;
        }
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["GroupCardType"];

        for (int i = 0; i < LbRight.Items.Count; i++)
        {
            //foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
            //{
            //if (LbRight.Items[i].Value == drowCardType["RID"].ToString())
            //{
            LbLeft.Items.Add(LbRight.Items[i]);
            //break;
            //}
            //}
        }
        LbRight.Items.Clear();
    }
    #endregion
    
}
