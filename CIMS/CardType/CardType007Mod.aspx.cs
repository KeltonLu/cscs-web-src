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

public partial class CardType_CardType007Mod : PageBase
{
    CardType007BL bizLogic = new CardType007BL();

    #region �ƥ�B�z

    //�����[��
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
                IMPORT_PROJECT ipModel = bizLogic.GetImportProject(strRID); 

                // �妸�]�m
                dropMakeCardType_RID.DataValueField = "Value";
                dropMakeCardType_RID.DataTextField = "Text";
                dropMakeCardType_RID.DataSource = bizLogic.GetTypeAndMakeCardType().Tables[1];
                dropMakeCardType_RID.DataBind();                
                //�]�m���󪺭�
                SetControls(ipModel);


                
                //���O�]�m
                if (adrtSmallTotal.Value == ipModel.Type)
                {
                    lbParam_Name.Text = "�p�p��";
                    this.adrtSmallTotal.Checked = true;
                    this.adrtNextMonth.Checked = false;
                    this.adrtYear.Checked = false;
                    trCardType1.Visible = true;
                }
                else if(adrtNextMonth.Value == ipModel.Type)
                {
                    lbParam_Name.Text = "����w��";
                    this.adrtSmallTotal.Checked = false;
                    this.adrtNextMonth.Checked = true;
                    this.adrtYear.Checked = false;
                    trCardType1.Visible = false;
                }
                else if (adrtYear.Value == ipModel.Type)
                {
                    lbParam_Name.Text = "�~�׹w��";
                    this.adrtSmallTotal.Checked = false;
                    this.adrtNextMonth.Checked = false;
                    this.adrtYear.Checked = true;
                    trCardType1.Visible = false;
                }

                Session["File_Name"] = ipModel.File_Name;
                //�w����Ъ���l��m
                txtFile_Name.Focus();
           }
            catch (Exception ex)
            {
                ExceptionFactory.CreateAlertException(this, ex.Message);
            }
        }
    }

    //�T�w���s
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {


        IMPORT_PROJECT ipModel = new IMPORT_PROJECT();//�w�q�ƾڼҫ�
        string strRID = Request.QueryString["RID"];
        try
        {
            
            if (chkDel.Checked)
            { //�R���B�z

                bizLogic.Delete(strRID);
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType007.aspx?Con=1");
            }
            else//�ק�B�z
            {
                //��������p�p�ɻP�פJ���س]�w���H��
                ipModel = bizLogic.GetImportProject(strRID);

                //�����e��ܪ����O
                if (adrtSmallTotal.Checked)
                {
                    radType.Value = adrtSmallTotal.Value;
                }
                else if (adrtNextMonth.Checked)
                {
                    radType.Value = adrtNextMonth.Value;
                }
                else if (adrtYear.Checked)
                {
                    radType.Value = adrtYear.Value;
                }

                SetData(ipModel);

                if (!(Session["File_Name"].Equals(txtFile_Name.Text.Trim())))
                {
                    if (bizLogic.ContainsID(txtFile_Name.Text.Trim()))//����File_Name�O�_�w�s�b���Ʈw
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
                        ShowMessage("�D���ɮצW�٤w�g�s�b");
                        return;
                    }

                }

                ////�P�_��e���O,�i��B�z
                if (ipModel.Type.Equals(adrtSmallTotal.Value))
                {
                //    if (txtTransfer_Name.Text.Trim().Equals(""))
                //    {
                //        ShowMessage("�ഫ�ɦW���ର��");
                //        lblStra.Visible = true;
                //        return;
                //    }
                }
                else
                {
                    ipModel.MakeCardType_RID = -1;
                }

                
                //����ק�ƾڨ�ƾڮw
                bizLogic.Update(ipModel);
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doLoad();", true);
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "CardType007.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }


    //�������s
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType007.aspx?Con=1");
    }

    #endregion 

    #region ���/�ƾڸɥR����
    /// <summary>
    /// ����File_Name�O�_�w�s�b���Ʈw
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void ajvFile_Name_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    //{
    //    if (!(Session["File_Name"].Equals(e.QueryData.Trim())))
    //    {
    //        e.IsAllowSubmit = !bizLogic.ContainsID(e.QueryData.Trim());//����File_Name�O�_�w�s�b���Ʈw]
    //    }
    //    else
    //    {
    //        e.IsAllowSubmit = true;
    //        return;
    //    }
    //}

    #endregion

}
