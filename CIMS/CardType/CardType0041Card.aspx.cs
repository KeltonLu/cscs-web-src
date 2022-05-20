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

public partial class CardType_CardType0041Card : PageBase
{
    CardType004BL bl = new CardType004BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbPersoProject.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            try
            {
                // 獲取 Perso廠商資料
                DataSet dstFactory = bl.GetFactoryList();
                dropFactory_RID.DataValueField = "RID";
                dropFactory_RID.DataTextField = "Factory_ShortName_CN";
                dropFactory_RID.DataSource = dstFactory.Tables[0];
                dropFactory_RID.DataBind();
                dropFactory_RID.Items.Insert(0, new ListItem("全部", ""));
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    UctrlCardType.SetRightItem = (DataTable)(((Dictionary<string, object>)Session["Condition"])["UctrlCardType"]);
                }
                gvpbPersoProject.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbPersoProject.BindData();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType0041CardAdd.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    protected void gvpbPersoProject_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropFactory_RID", dropFactory_RID.SelectedValue);
        inputs.Add("txtProject_Name", txtProject_Name.Text);
        inputs.Add("txtUse_Date_Begin", txtUse_Date_Begin.Text);
        inputs.Add("txtUse_Date_End", txtUse_Date_End.Text);
        inputs.Add("UctrlCardType", UctrlCardType.GetRightItem);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlPersoProject = null;

        try
        {
            dstlPersoProject = bl.ListCard(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlPersoProject != null)//如果查到了資料
            {
                e.Table = dstlPersoProject.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
                ViewState["Factory_ShortName_CN"] = null;
                ViewState["Project_Code"] = null;
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void gvpbPersoProject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblPersoProject = (DataTable)gvpbPersoProject.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblPersoProject.Rows.Count == 0)
                return;

            if (e.Row.Cells[0].Text != "&nbsp;")
            {
                if (ViewState["Factory_ShortName_CN"] != null)
                {
                    if (ViewState["Factory_ShortName_CN"].ToString() == e.Row.Cells[0].Text)
                    {
                        e.Row.Cells[0].Text = "&nbsp;";
                    }
                }
                if (e.Row.Cells[0].Text != "&nbsp;")
                {
                    ViewState["Factory_ShortName_CN"] = e.Row.Cells[0].Text;
                }
            }

            if (e.Row.Cells[1].Text != "&nbsp;")
            {
                if (ViewState["Project_Code"] != null)
                {
                    if (ViewState["Project_Code"].ToString() == e.Row.Cells[1].Text)
                    {
                        e.Row.Cells[1].Text = "&nbsp;";
                    }
                }
                if (e.Row.Cells[1].Text != "&nbsp;")
                {
                    ViewState["Project_Code"] = e.Row.Cells[1].Text;
                }
            }

            //Label lblCardType = (Label)e.Row.FindControl("lblCardType");

            //DataTable dtblCardType = bl.GetCardtypePerso(dtblPersoProject.Rows[e.Row.RowIndex]["RID"].ToString());

            //foreach (DataRow drow in dtblCardType.Rows)
            //{
            //    lblCardType.Text += drow["Display_Name"] + "<BR>";
            //}

        }
    }
}

