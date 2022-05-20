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

public partial class CardType_CardType0041Mod : PageBase
{
    CardType004BL bl = new CardType004BL();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            if (StringUtil.IsEmpty(strRID))
            {
                return;
            }

            try
            {
                ////獲取Perso項目信息
                PROJECT_STEP psModel = new PROJECT_STEP();
                DataSet dstFactory = new DataSet();
                bl.ListModel1(strRID, ref psModel, ref dstFactory);
               
                this.lbName.Text = psModel.Name;
                txtUse_Date_Begin.Text = psModel.Use_Date_Begin.ToShortDateString();
                txtUse_Date_End.Text = psModel.Use_Date_End.ToShortDateString();
                txtComment.Text = psModel.Comment;
                txtPrice.Text = psModel.Price.ToString("N4");

                lbFactory.Text = dstFactory.Tables[0].Rows[0][1].ToString();

                gvPerso_Project.DataSource = bl.GetPersoProjectNameByStepID(psModel.Step_ID.ToString());
                gvPerso_Project.DataBind();


                txtPrice.Enabled = false;
                txtUse_Date_Begin.Enabled = false;
                txtUse_Date_End.Enabled = false;
                chkDel.Visible = false;
                
                if (bl.IsLastStep(psModel.Name, psModel.Factory_RID.ToString(), psModel.RID.ToString()))
                {
                    txtPrice.Enabled = true;
                    txtUse_Date_End.Enabled = true;
                    chkDel.Visible = true;
                }

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];

        try
        {
            // 刪除Perso項目檔
            if (this.chkDel.Checked)
            {
                bl.Delete1(strRID);

                ShowMessageAndGoPage("刪除成功", "CardType004.aspx?Con=1&List=radStep");
            }
            // 保存Perso項目檔
            else
            {
                if (StringUtil.IsEmpty(txtPrice.Text))
                {
                    ShowMessage("單價不能為空");
                    return;
                }

                if (Convert.ToDateTime(txtUse_Date_End.Text) < Convert.ToDateTime(txtUse_Date_Begin.Text))
                {
                    ShowMessage("價格期間迄必須大於起");
                    return;
                }

                if (StringUtil.GetByteLength(txtComment.Text) > 100)
                {
                    ShowMessage("備註不能超過100個字符");
                    return;
                }

                PROJECT_STEP psModel = new PROJECT_STEP();
                txtPrice.Text = this.txtPrice.Text.Replace(",", "");

                SetData(psModel);
                // 設置廠商資料

                psModel.RID = int.Parse(strRID);
                bl.Update1(psModel);

                ShowMessageAndGoPage("修改成功", "CardType004.aspx?Con=1&List=radStep");
            }
        }
        catch (Exception ex)
        {
            if (!StringUtil.IsEmpty(txtPrice.Text))
            {
                txtPrice.Text = Convert.ToDecimal(txtPrice.Text.Replace(",", "")).ToString("N4");
            }

            ShowMessage(ex.Message);
        }
    }
   
}
