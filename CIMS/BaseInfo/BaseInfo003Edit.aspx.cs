//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料編輯頁面
//*  創建日期：2008-08-27
//*  修改日期：2008-08-27 14:00
//*  修改記錄：
//*            □2008-08-27
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
using System.Web.UI.HtmlControls;

public partial class BaseInfo_BaseInfo003Edit : PageBase
{
    BaseInfo003BL fmManager = new BaseInfo003BL();

    protected void Page_Load(object sender, EventArgs e)
    {       
        if (!IsPostBack)
        {            
            string strRID = Request.QueryString["RID"];

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {
                FACTORY cbModel = fmManager.GetFactory(strRID);

                this.lbTitle.Text = "廠商資料修改/刪除";
                this.Page.Title = "廠商資料修改/刪除";
                //設置控件的值
                SetControls(cbModel);

                if (cbModel.Is_Cooperate == "Y")
                {
                    radlIs_Cooperate.SelectedIndex = 0;
                }
                else if (cbModel.Is_Cooperate == "N")
                {
                    radlIs_Cooperate.SelectedIndex = 1;
                }

                //製域處理
                trDel.Visible = true;
                bllfid.Visible = true;
            }
            else //無值，代表是新增！
            {                
                //製域處理
                trDel.Visible = false;
                bllfid.Visible = false;
                this.lbTitle.Text = "廠商資料新增";
                this.Page.Title = "廠商資料新增";
            }
        }
    }

    public void SetModel(FACTORY cbModel)
    {

        //if (!StringUtil.IsEmpty(txtCountry_Name.Text))
        //{
        //    cbModel.Country_Name = int.Parse(txtCountry_Name.Text);
        //} 
        SetData(cbModel);
        if (radlIs_Cooperate.Items[0].Selected)
        {
            cbModel.Is_Cooperate = "Y";
        }
        else if (radlIs_Cooperate.Items[1].Selected)
        {
            cbModel.Is_Cooperate = "N";
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        FACTORY cbModel = new FACTORY();

        string strRID = Request.QueryString["RID"];

        if (!chkDel.Checked)
        {
            if (StringUtil.GetByteLength(txtProduct_Main.Text) > 100)
            {
                ShowMessage("主要產品不能超過100個字符");
                return;
            }

            if (StringUtil.GetByteLength(txtComment.Text) > 100)
            {
                ShowMessage("備註不能超過100個字符");
                return;
            }
        }

        try
        {
            if (StringUtil.IsEmpty(strRID))    //增加
            {                
                //SetData(cbModel);
                SetModel(cbModel);

                if (cbModel.Is_Blank == "N" && cbModel.Is_Perso == "N")
                {
                    ShowMessage("廠商類別不能為空！");
                    return;
                }
                else
                {
                    fmManager.Add(cbModel);

                    ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "BaseInfo003Edit.aspx");
                }
            }
            else
            {               
                if (chkDel.Checked)         //刪除
                {
                    fmManager.Delete(strRID);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "BaseInfo003.aspx?Con=1");
                }
                else                        //修改 
                {
                    cbModel=fmManager.GetFactory(strRID);

                    SetModel(cbModel);

                    fmManager.Update(cbModel);

                    ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"],
                        "BaseInfo003.aspx?Con=1");
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
        Response.Redirect("BaseInfo003.aspx?Con=1");
    }

    protected void btnEditD_Click(object sender, EventArgs e)
    {
        btnEdit_Click(sender, e);
    }

    protected void btnCancelD_Click(object sender, EventArgs e)
    {
        btnCancel_Click(sender, e);
    }
}
