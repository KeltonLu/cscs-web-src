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


public partial class Finance_Finance0024Addp : PageBase
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

            if (type == "Add")
            {
                txtProject_Date.Text = System.DateTime.Now.ToString("yyyy/MM/dd",System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            else if(type=="Edit")
            {                
                EXCEPTION_PERSO_PROJECT eppModel = new EXCEPTION_PERSO_PROJECT();
                eppModel = FinanceBL.getExcepProject(Convert.ToInt32(RID));
                txtProject_Date.Text = eppModel.Project_Date.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                txtNumber.Text = eppModel.Number.ToString();
                txtName.Text = eppModel.Name.ToString();
                txtBase_price.Text = eppModel.Unit_Price.ToString();
                txtComment.Text = eppModel.Comment.ToString();
                dropCard_Group_RID.SelectedValue = eppModel.CardGroup_RID.ToString();
                dropFactory.SelectedValue = eppModel.Perso_Factory_RID.ToString();
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
        if (StringUtil.IsEmpty(txtProject_Date.Text))
        {
            ShowMessage("必須選擇日期");
            return;
        }
        if (StringUtil.IsEmpty(txtName.Text))
        {
            ShowMessage("必須選擇例外代製項目");
            return;
        }
        if (StringUtil.IsEmpty(txtNumber.Text))
        {
            ShowMessage("必須選擇數量");
            return;
        }
        if (StringUtil.IsEmpty(txtBase_price.Text))
        {
            ShowMessage("必須選擇單價");
            return;
        }

        try
        {
            txtNumber.Text = txtNumber.Text.Trim().Replace(",","");
            txtBase_price.Text = txtBase_price.Text.Trim().Replace(",","");

            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("Name", txtName.Text);
            inputs.Add("Number", txtNumber.Text);
            inputs.Add("Unit_Price", txtBase_price.Text);
            inputs.Add("Project_Date", txtProject_Date.Text);
            inputs.Add("Comment", txtComment.Text);
            inputs.Add("CardGroup_RID", dropCard_Group_RID.SelectedValue);
            inputs.Add("Perso_Factory_RID", dropFactory.SelectedValue);

            DataTable dtProject = (DataTable)Session["Project"];

            if (type == "Add")
            {
                if (FinanceBL.ConAskedMoneyGroup(DateTime.Parse(txtProject_Date.Text),
                            dropFactory.SelectedValue.ToString(),
                            dropCard_Group_RID.SelectedValue.ToString()))
                    throw new Exception("該時間內已經有代製費用請款，不能新增！");

                DataRow dr = dtProject.NewRow();
                dr["RID"] = FinanceBL.SaveExcepProject(inputs);
                dr["Param_Name"] = dropCard_Purpose_RID.SelectedItem.Text;
                dr["Group_Name"] = dropCard_Group_RID.SelectedItem.Text;
                dr["Project_Date"] = txtProject_Date.Text;
                dr["Name"] = txtName.Text;
                dr["Number"] = txtNumber.Text;
                dr["Unit_Price"] = txtBase_price.Text;
                dr["Comment"] = txtComment.Text;
                dtProject.Rows.Add(dr);
                Session["Project"] = dtProject;
                
                ShowMessage("新增資料成功！");
            }
            else if (type == "Edit")
            {
                if (FinanceBL.ConAskedMoneyGroup(DateTime.Parse(txtProject_Date.Text),
                            dropFactory.SelectedValue.ToString(),
                            dropCard_Group_RID.SelectedValue.ToString()))
                    throw new Exception("該時間內已經有代製費用請款，不能修改！");

                FinanceBL.UpdateExcepProject(Convert.ToInt32(RID), inputs);

                DataRow[] datar = dtProject.Select("RID='" + RID+"'");
                if (datar.Length > 0)
                {                    
                    datar[0]["RID"] = RID;
                    datar[0]["Param_Name"] = dropCard_Purpose_RID.SelectedItem.Text;
                    datar[0]["Group_Name"] = dropCard_Group_RID.SelectedItem.Text;
                    datar[0]["Project_Date"] = txtProject_Date.Text;
                    datar[0]["Name"] = txtName.Text;
                    datar[0]["Number"] = txtNumber.Text;
                    datar[0]["Unit_Price"] = txtBase_price.Text;
                    datar[0]["Comment"] = txtComment.Text;
                    Session["Project"] = dtProject;
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
    #endregion 初始化下拉框  
    
}
