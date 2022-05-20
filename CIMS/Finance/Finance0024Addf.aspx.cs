using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Finance_Finance0024Addf : PageBase
{
    Dictionary<string, object> inputs = new Dictionary<string, object>();
    Finance0024BL FinanceBL = new Finance0024BL();
    string type = "";
    string RID = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        type = Request.QueryString["ActionType"].ToString();
        if (type == "Edit")
        {
            RID = Request.QueryString["RID"].ToString();
        }

        if (!IsPostBack)
        {
            dropCard_Purpose_RIDBind();
            dropCard_Group_RIDBind();
            dropFactoryBind();
            dropParam_CodeBind();

            if (type == "Add")
            {
                dropYear.SelectedValue = System.DateTime.Now.Year.ToString();
                dropMonth.SelectedValue = System.DateTime.Now.Month.ToString();
            }
            else if (type == "Edit")
            {
                PERSO_PROJECT_CHANGE_DETAIL ppcdModel = new PERSO_PROJECT_CHANGE_DETAIL();
                ppcdModel = FinanceBL.getChangeProject(Convert.ToInt32(RID));
                dropYear.SelectedValue = ppcdModel.Project_Date.Year.ToString();
                dropMonth.SelectedValue = ppcdModel.Project_Date.Month.ToString();
                dropParam_Code.SelectedValue = ppcdModel.Param_Code;
                txtPrice.Text = ppcdModel.Price.ToString();
                txtComment.Text = ppcdModel.Comment.ToString();
                dropCard_Group_RID.SelectedValue = ppcdModel.CardGroup_RID.ToString();
                dropFactory.SelectedValue = ppcdModel.Perso_Factory.ToString();
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (StringUtil.IsEmpty(dropCard_Group_RID.SelectedValue))
        {
            ShowMessage("必須選擇群組");
            return;
        }
        if (StringUtil.IsEmpty(dropFactory.SelectedValue))
        {
            ShowMessage("必須選擇Perso廠");
            return;
        }   
        if (StringUtil.IsEmpty(txtPrice.Text))
        {
            ShowMessage("必須選擇金額");
            return;
        }       

        try
        {
            DateTime dtIn_Date = FinanceBL.getWorkDate(dropYear.SelectedValue,dropMonth.SelectedValue);
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("Name", dropParam_Code.SelectedValue);
            inputs.Add("Unit_Price", txtPrice.Text);
            inputs.Add("Project_Date", dtIn_Date);
            inputs.Add("Comment", txtComment.Text);
            inputs.Add("CardGroup_RID", dropCard_Group_RID.SelectedValue);
            inputs.Add("Perso_Factory_RID", dropFactory.SelectedValue);

            DataTable dtbFinance = (DataTable)Session["Finance"];

            if (type == "Add")
            {
                if (FinanceBL.ConAskedMoneyGroup(dtIn_Date, dropFactory.SelectedValue, dropCard_Group_RID.SelectedValue))
                    throw new Exception("當月已經有請款不能新增代製費用帳務異動。");

                int i= FinanceBL.SaveChangeProject(inputs);
                DataRow dr = dtbFinance.NewRow();
                dr["RID"] = RID;
                dr["Param_Name"] = dropCard_Purpose_RID.SelectedItem.Text;
                dr["Group_Name"] = dropCard_Group_RID.SelectedItem.Text;
                dr["Perso_Factory"] = dropFactory.SelectedItem.Text;
                dr["Project_Date"] = dropYear.SelectedValue+"/"+dropMonth.SelectedValue;
                dr["ProjectName"] = dropParam_Code.SelectedItem.Text;
                dr["Price"] = txtPrice.Text;
                dr["Comment"] = txtComment.Text;
                dtbFinance.Rows.Add(dr);
                Session["Finance"] = dtbFinance;

                ShowMessage("新增資料成功！");
            }
            else if (type == "Edit")
            {
                if (FinanceBL.ConAskedMoneyGroup(dtIn_Date, dropFactory.SelectedValue, dropCard_Group_RID.SelectedValue))
                    throw new Exception("當月已經有請款不能修改代製費用帳務異動。");

                FinanceBL.UpdateChangeProject(Convert.ToInt32(RID), inputs);

                DataRow[] datar = dtbFinance.Select("RID='" + RID + "'");
                if (datar.Length > 0)
                {
                    datar[0]["RID"] = RID;
                    datar[0]["Param_Name"] = dropCard_Purpose_RID.SelectedItem.Text;
                    datar[0]["Group_Name"] = dropCard_Group_RID.SelectedItem.Text;
                    datar[0]["Project_Date"] = dropYear.SelectedValue + "/" + dropMonth.SelectedValue;
                    datar[0]["Perso_Factory"] = dropFactory.SelectedItem.Text;
                    datar[0]["ProjectName"] = dropParam_Code.SelectedItem.Text;
                    datar[0]["Price"] = txtPrice.Text;
                    datar[0]["Comment"] = txtComment.Text;
                    Session["Finance"] = dtbFinance;
                }

                ShowMessage("修改資料成功！");
            }

            Response.Write("<script>returnValue='1';window.close();</script>");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //用途變更
    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            dropCard_Group_RIDBind();
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #region 初始化下拉框
    /// <summary>
    /// Perso卡廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        dropFactory.Items.Clear();

        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataValueField = "RID";
        dropFactory.DataSource = FinanceBL.getFactory();
        dropFactory.DataBind();

        //dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_Purpose_RIDBind()
    {
        dropCard_Purpose_RID.DataTextField = "PARAM_NAME";
        dropCard_Purpose_RID.DataValueField = "Param_Code";
        dropCard_Purpose_RID.DataSource = FinanceBL.getParam_Finance();
        dropCard_Purpose_RID.DataBind();

        //dropCard_Purpose_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_Group_RIDBind()
    {
        dropCard_Group_RID.Items.Clear();

        dropCard_Group_RID.DataTextField = "GROUP_NAME";
        dropCard_Group_RID.DataValueField = "RID";
        dropCard_Group_RID.DataSource = FinanceBL.getCardGroup(dropCard_Purpose_RID.SelectedValue);
        dropCard_Group_RID.DataBind();

        //dropCard_Group_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 代製費用帳務異動項目設定下拉框綁定
    /// </summary>
    protected void dropParam_CodeBind()
    {
        dropParam_Code.Items.Clear();

        dropParam_Code.DataTextField = "PARAM_NAME";
        dropParam_Code.DataValueField = "Param_Code";
        dropParam_Code.DataSource = FinanceBL.getParam_Change();
        dropParam_Code.DataBind();

        //dropCard_Group_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    #endregion 初始化下拉框  
}
