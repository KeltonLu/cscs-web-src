using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class CardType_CardType004 : PageBase
{
    CardType004BL bl = new CardType004BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbPersoProject.PageSize = GlobalStringManager.PageSize;
        gvpbPROJECT_STEP.PageSize = GlobalStringManager.PageSize;
        ViewState["Factory_ShortName_CN"] = "";
        ViewState["Project_Code"] = "";
        if (!IsPostBack)
        {
            // 獲取 Perso廠商資料
            DataSet dstFactory = bl.GetFactoryList();
            dropFactory.DataValueField = "RID";
            dropFactory.DataTextField = "Factory_ShortName_CN";
            dropFactory.DataSource = dstFactory.Tables[0];
            dropFactory.DataBind();
            dropFactory.Items.Insert(0, new ListItem("全部", ""));

            if (Request.QueryString["List"] != null)
            {
                if (Request.QueryString["List"].ToString() == "radStep")
                {
                    radStep.Checked = true;
                    radProject_Name.Checked = false;

                    lblProjectName.Text = "製程";
                    lblTime.Text = "價格期間";

                   
                }
                else
                {
                    radStep.Checked = false;
                    radProject_Name.Checked = true;

                    lblProjectName.Text = "代製項目";
                    lblTime.Text = "價格期間";
                }
            }

            // 從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (SetConData())
                if (radStep.Checked)
                {
                    gvpbPROJECT_STEP.BindData();
                }
                else
                {
                    gvpbPersoProject.BindData();
                }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (radStep.Checked)
        {
            gvpbPROJECT_STEP.Visible = true;
            gvpbPersoProject.Visible = false;
            gvpbPROJECT_STEP.BindData();
        }
        else
        {
            gvpbPROJECT_STEP.Visible = false;
            gvpbPersoProject.Visible = true;
            gvpbPersoProject.BindData();
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (radProject_Name.Checked)
        {
            Response.Redirect(string.Concat("CardType004Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
        }
        else
        {
            Response.Redirect(string.Concat("CardType0041Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
        }
    }
    #endregion

    #region 欄位/資料補充說明
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbPersoProject_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtProject_Name", txtName.Text);
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("txtBegin_Date", txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", txtFinish_Date.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlPersoProject = null;

        ViewState["Factory_ShortName_CN1"] = "";
        ViewState["Project_Code1"] = "";
        ViewState["Project_Name1"] = "";

        try
        {
            dstlPersoProject = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlPersoProject != null)//如果查到了資料
            {
                e.Table = dstlPersoProject.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbPersoProject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblPersoProject = (DataTable)gvpbPersoProject.DataSource;

         if (e.Row.RowType == DataControlRowType.DataRow)
         {
             if (dtblPersoProject.Rows.Count == 0)
                 return;

             if (e.Row.Cells[0].Text != "&nbsp;")
             {
                 if (ViewState["Factory_ShortName_CN1"] != null)
                 {
                     if (ViewState["Factory_ShortName_CN1"].ToString() == e.Row.Cells[0].Text)
                     {
                         e.Row.Cells[0].Text = "&nbsp;";
                     }
                 }
                 if (e.Row.Cells[0].Text != "&nbsp;")
                 {
                     ViewState["Factory_ShortName_CN1"] = e.Row.Cells[0].Text;
                 }
             }

             if (e.Row.Cells[1].Text != "&nbsp;")
             {
                 if (ViewState["Project_Code1"] != null)
                 {
                     if (ViewState["Project_Code1"].ToString() == e.Row.Cells[1].Text)
                     {
                         e.Row.Cells[1].Text = "&nbsp;";
                     }
                 }
                 if (e.Row.Cells[1].Text != "&nbsp;")
                 {
                     ViewState["Project_Code1"] = e.Row.Cells[1].Text;
                 }
             }
         }

    }
    #endregion

    protected void radStep_CheckedChanged(object sender, EventArgs e)
    {
        
        lblProjectName.Text = "製程";
        lblTime.Text = "價格期間";

        gvpbPROJECT_STEP.Visible = true;
        gvpbPersoProject.Visible = false;

        gvpbPROJECT_STEP.DataSource = null;
        gvpbPROJECT_STEP.DataBind();
        gvpbPersoProject.DataSource = null;
        gvpbPersoProject.DataBind();
    }
    protected void radProject_Name_CheckedChanged(object sender, EventArgs e)
    {
        lblProjectName.Text = "代製項目";
        lblTime.Text = "價格期間";
        

        gvpbPROJECT_STEP.Visible = false;
        gvpbPersoProject.Visible = true;

        gvpbPROJECT_STEP.DataSource = null;
        gvpbPROJECT_STEP.DataBind();
        gvpbPersoProject.DataSource = null;
        gvpbPersoProject.DataBind();
    }
    protected void gvpbPROJECT_STEP_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtName", txtName.Text);
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("txtBegin_Date", this.txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", this.txtFinish_Date.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlPersoProject = null;

        try
        {
            dstlPersoProject = bl.List1(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlPersoProject != null)//如果查到了資料
            {
                e.Table = dstlPersoProject.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void gvpbPROJECT_STEP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[3].Text = Convert.ToDateTime(e.Row.Cells[3].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            if (ViewState["Factory_ShortName_CN"].ToString() == e.Row.Cells[0].Text)
            {
                e.Row.Cells[0].Text = "";
            }
            if (e.Row.Cells[0].Text != "")
            {
                ViewState["Factory_ShortName_CN"] = e.Row.Cells[0].Text;
            }
        }
    }
}
