//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 9:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 潘秉奕
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
using System.Collections.Generic;

public partial class CardType_CardType008Edit : PageBase
{
    BaseInfo003BL ftManager = new BaseInfo003BL();
    CardType008BL ctManager = new CardType008BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];

            //空白卡廠資料綁定
            LbLeftBind();

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {
                WAFER_INFO wiModel = ctManager.GetWaferInfo(strRID);

                //設置控件的值
                SetControls(wiModel);

                DataSet dswf = ctManager.LoadFactoryByWRID(wiModel.RID.ToString());
                LbRight.DataSource = dswf;
                LbRight.DataTextField = "factory_shortNAME_cn";
                LbRight.DataValueField = "RID";
                LbRight.DataBind();
                for (int i = 0; i < LbRight.Items.Count; i++)
                {
                    LbLeft.Items.Remove(LbRight.Items[i]);
                }

                DataSet dswc = ctManager.LoadCardTypeByWRID(wiModel.RID.ToString());
                UctrlCardType1.SetRightItem = dswc.Tables[0];

                lblTitle.Text = "晶片基本資料檔修改/刪除";
                //製域處理
                trDel.Visible = true;
                txtWafer_Name.Enabled = false;
                AjaxValidatorName.Enabled = false;
                IsNew.Value = "";

                wn.Visible = false;
               
            }
            else //無值，代表是新增！
            {
                lblTitle.Text = "晶片基本資料檔新增";
                //製域處理
                trDel.Visible = false;
                txtWafer_Name.Enabled = true;
                IsNew.Value = "insert";

                wn.Visible = true;
            }
        }
        UctrlCardType1.Is_Using = true;
    }

    #region 空白卡廠選擇
    /// <summary>
    /// 資料綁定
    /// </summary>
    protected void LbLeftBind()
    {
        LbLeft.Items.Clear();

        DataSet dsltBlankFactory = ftManager.GetBlankFactory();
        ViewState["BlankFactory"] = dsltBlankFactory;

        LbLeft.DataTextField = "factory_shortNAME_cn";
        LbLeft.DataValueField = "RID";
        LbLeft.DataSource = dsltBlankFactory;
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
        DataSet dsltCardType = (DataSet)ViewState["BlankFactory"];

        int i = 0;
        while (i < LbRight.Items.Count)
        {
            if (LbRight.Items[i].Selected == true)
            {
                foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
                {
                    if (LbRight.Items[i].Value == drowCardType["RID"].ToString())
                    {
                        LbLeft.Items.Add(LbRight.Items[i]);
                        break;
                    }
                }
                LbRight.Items.Remove(LbRight.Items[i]);
            }
            else
                i += 1;
        }
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["BlankFactory"];

        for (int i = 0; i < LbRight.Items.Count; i++)
        {
            foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
            {
                if (LbRight.Items[i].Value == drowCardType["RID"].ToString())
                {
                    LbLeft.Items.Add(LbRight.Items[i]);
                    break;
                }
            }
        }
        LbRight.Items.Clear();
    }
    #endregion

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        WAFER_INFO wiModel = new WAFER_INFO();
        WAFER_FACTORY wfModel = new WAFER_FACTORY();
        WAFER_CARDTYPE wcModel = new WAFER_CARDTYPE();
                
        ArrayList fal = new ArrayList();
        ArrayList cal = new ArrayList();

        string strRID = Request.QueryString["RID"];

        if (!chkDel.Checked)
        {
            if (StringUtil.GetByteLength(txtComment_One.Text) > 1000)
            {
                ShowMessage("備註1不能超過1000個字符");
                return;
            }
            if (StringUtil.GetByteLength(txtComment_Second.Text) > 1000)
            {
                ShowMessage("備註2不能超過1000個字符");
                return;
            }
            if (StringUtil.GetByteLength(txtComment_Third.Text) > 1000)
            {
                ShowMessage("備註3不能超過1000個字符");
                return;
            }
        }

        txtROM_Capacity.Text = txtROM_Capacity.Text.Replace(",", "");
        txtWafer_Capacity.Text = txtWafer_Capacity.Text.Replace(",", "");

        try
        {
            if (StringUtil.IsEmpty(strRID))    //增加
            {
                SetData(wiModel);

                //if (!ctManager.IsHave(txtWafer_Name.Text.Trim()))
                //{
                    if (LbRight.Items.Count > 0)
                    {
                        fal.Clear();
                        for (int i = 0; i < LbRight.Items.Count; i++)
                        {
                            fal.Add(LbRight.Items[i].Value);
                        }
                    }

                    if (UctrlCardType1.GetRightItem.Rows.Count > 0)
                    {
                        cal.Clear();
                        for (int i = 0; i < UctrlCardType1.GetRightItem.Rows.Count; i++)
                        {
                            cal.Add(UctrlCardType1.GetRightItem.Rows[i][0].ToString());
                        }
                    }

                    ctManager.Add(wiModel, wfModel, fal, wcModel, cal);

                    ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType008Edit.aspx");
                //}
                //else
                //{
                //    ShowMessage(GlobalStringManager.Default["Alert_WaferExist"]);
                //}
            }
            else
            {
                if (chkDel.Checked)         //刪除
                {
                    ctManager.Delete(strRID);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType008.aspx?Con=1");
                }
                else                        //修改 
                {
                    wiModel = ctManager.GetWaferInfo(strRID);

                    SetData(wiModel);

                    if (LbRight.Items.Count > 0)
                    {
                        fal.Clear();
                        for (int i = 0; i < LbRight.Items.Count; i++)
                        {
                            fal.Add(LbRight.Items[i].Value);
                        }
                    }

                    if (UctrlCardType1.GetRightItem.Rows.Count > 0)
                    {
                        cal.Clear();
                        for (int i = 0; i < UctrlCardType1.GetRightItem.Rows.Count; i++)
                        {
                            cal.Add(UctrlCardType1.GetRightItem.Rows[i][0].ToString());
                        }
                    }

                    ctManager.Update(wiModel, wfModel, fal, wcModel, cal);

                    ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"],"CardType008.aspx?Con=1");                    
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType008.aspx?Con=1");
    }

    protected void btnEditD_Click(object sender, EventArgs e)
    {
        btnEdit_Click(sender, e);
    }

    protected void btnCancelD_Click(object sender, EventArgs e)
    {
        btnCancel_Click(sender, e);
    }

    protected void AjaxValidatorName_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        if (e.QueryData.Trim() != "")
            e.IsAllowSubmit = !ctManager.IsHave(e.QueryData.Trim());//驗證角色名稱是否已存在於資料庫
    }
}
